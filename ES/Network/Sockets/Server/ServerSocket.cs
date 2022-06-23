using ES.Alias.Collections;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ES.Network.Sockets.Server
{
    /// <summary>
    /// ESF套接字服务端管理
    /// <para>通过新建ESFSocket对象 装载到此类中即可生成一个服务器对象</para>
    /// <para>UDP访问仅仅通过本类不能监控其断开状态，需要自主定制</para>
    /// </summary>
    public class ServerSocket : ISocketIOEvent
    {
        /// <summary>
        /// 客户端连接合集 tcp
        /// </summary>
        protected readonly ConcurrentMap<int, RemoteConnection> TcpClients = new ConcurrentMap<int, RemoteConnection>();

        /// <summary>
        /// 客户端连接设计为同时处理的最大连接数
        /// the maximum number of connections the sample is designed to handle simultaneously 
        /// </summary>
        private readonly int numMaxConnections;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// buffer size to use for each socket I/O operation 
        /// </summary>
        private readonly int receiveBufferSize;
        /// <summary>
        /// 表示所有套接字操作的一组可重用的大缓冲区
        /// represents a large reusable set of buffers for all socket operations
        /// </summary>
        private readonly BufferManager bufferManager;
        /// <summary>
        /// 读、写（不为接受分配缓冲区空间）
        /// read, write (don't alloc buffer space for accepts)
        /// </summary>
        private const int opsToPreAlloc = 2;
        /// <summary>
        /// 用于侦听传入连接请求的ESF套接字
        /// the socket used to listen for incoming connection requests
        /// </summary>
        protected readonly Socket serverSocket;
        /// <summary>
        /// 用于写入、读取和接受套接字操作的可重用SocketAsyncEventArgs对象池
        /// pool of reusable System.Net.Sockets.SocketAsyncEventArgs objects for write, read and accept socket operations
        /// </summary>
        protected readonly ConcurrentBag<System.Net.Sockets.SocketAsyncEventArgs> readWritePool = new ConcurrentBag<System.Net.Sockets.SocketAsyncEventArgs>();
        /// <summary>
        /// 服务器接收的总字节数计数器
        /// counter of the total # bytes received by the server
        /// </summary>
        /// private int totalBytesRead;
        /// <summary>
        /// 连接到服务器的客户端总数
        /// the total number of clients connected to the server
        /// </summary>
        private int numConnectedSockets = 0;
        /// <summary>
        /// 最大容许客户端数量
        /// </summary>
        protected readonly Semaphore maxNumberAcceptedClients;
        /// <summary>
        /// 消息委托
        /// </summary>
        public IRemoteSocket? socketInvoke = null;
        /// <summary>
        /// 套接字状态监听回调器
        /// </summary>
        internal IServerSocket? socketStatusListener = null;
        /// <summary>
        /// 监听套接字状态任务
        /// </summary>
        protected MonitorSocketStatusTask? monitorSocketStatusTask = null;

        /// <summary>
        /// 已连接数
        /// </summary>
        public int ConnectedNum => numConnectedSockets;

        /// <summary>
        /// 运行状态
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// 新建套接字服务管理对象
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="numMaxConnections">可同时连接客户端最大数</param>
        /// <param name="receiveBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ServerSocket(string ip, int port, int numMaxConnections, int receiveBufferSize)
        {
            // 绑定原生套接字
            serverSocket = new Socket(ip, port);

            // 初始化部分参数
            // totalBytesRead = 0;
            // numConnectedSockets = 0;
            this.numMaxConnections = numMaxConnections;
            this.receiveBufferSize = receiveBufferSize + SweetStream.PACKAGE_MAX_SIZE;

            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            // write posted to the socket simultaneously  
            bufferManager = new BufferManager(this.receiveBufferSize * numMaxConnections * opsToPreAlloc, this.receiveBufferSize);
            maxNumberAcceptedClients = new Semaphore(numMaxConnections, numMaxConnections);
        }

        /// <summary>
        /// 新建套接字服务管理对象
        /// </summary>
        /// <param name="esfSocket">ESF套接字</param>
        /// <param name="numMaxConnections">可同时连接客户端最大数</param>
        /// <param name="receiveBufferSize">接受数据大小[UDP模式以传送理论最大值/TCP模式以传送最佳合适值]</param>
        public ServerSocket(Socket esfSocket, int numMaxConnections, int receiveBufferSize)
        {
            // 绑定原生套接字
            serverSocket = esfSocket;

            // 初始化部分参数
            // totalBytesRead = 0;
            // numConnectedSockets = 0;
            this.numMaxConnections = numMaxConnections;
            this.receiveBufferSize = receiveBufferSize + SweetStream.PACKAGE_MAX_SIZE;

            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            // write posted to the socket simultaneously  
            bufferManager = new BufferManager(this.receiveBufferSize * numMaxConnections * opsToPreAlloc, this.receiveBufferSize);
            maxNumberAcceptedClients = new Semaphore(numMaxConnections, numMaxConnections);
        }

        /// <summary>
        /// 初始化套接字服务管理
        /// </summary>
        /// <param name="addressFamily">地址协议簇 [ipv4或ipv6]</param>
        /// <param name="socketType">套接字类型 [stream或Dgram]</param>
        /// <param name="protocolType">协议类型 [tcp或者udp]</param>
        /// <param name="backlog">同时监听接入连接数 默认为0, 即 并发可监听数</param>
        /// <param name="socketInvoke">委托回调</param>
        /// <returns>连接是否成功</returns>
        public bool Init(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, int backlog, IRemoteSocket socketInvoke)
        {
            // 绑定委托
            this.socketInvoke = socketInvoke;
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            // bufferManager.InitBuffer();
            for (int i = 0; i < numMaxConnections; i++)
            {
                // preallocate pool of System.Net.Sockets.SocketAsyncEventArgs objects 
                // Pre -allocate a set of reusable System.Net.Sockets.SocketAsyncEventArgs
                MySocketAsyncEventArgs readWriteEventArg = new MySocketAsyncEventArgs(this, serverSocket, this);

                // 如果是报文结构则 初始化终端节点
                if (socketType == SocketType.Stream)
                {
                    RemoteConnection remoteClient = new RemoteConnection(this, socketInvoke);
                    readWriteEventArg.UserToken = remoteClient;
                    remoteClient.readWriteEventArg = readWriteEventArg;
                }
                else
                    readWriteEventArg.RemoteEndPoint = serverSocket.endPoint;

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                bufferManager.SetBuffer(readWriteEventArg);
                // add SocketAsyncEventArg to the pool
                readWritePool.Add(readWriteEventArg);
            }
            // 启动服务
            bool bSuccess = serverSocket.ConnectAsServer(addressFamily, socketType, protocolType, backlog);
            StartServer();
            // Log.Info("Init Sever Connecting!");
            return bSuccess;
        }

        /// <summary>
        /// 启动服务
        /// <para>现在不需要手动处理了，Init里直接执行</para>
        /// </summary>
        /// <returns></returns>
        private void StartServer()
        {
            isRunning = true;

            if (serverSocket.SocketType == SocketType.Stream)
                StartAccept(null);

            if (serverSocket.SocketType == SocketType.Dgram)
                StartAcceptReceiveFrom();
        }

        /// <summary>
        /// 返回客户端连接数
        /// </summary>
        /// <returns></returns>
        public int GetClientCount()
        {
            if (serverSocket.SocketType == SocketType.Stream)
                return TcpClients.Count;

            // else if (serverSocket.socketType == SocketType.Dgram)
            //    return remoteUDPClients.Count;
            // else
            return -1;
        }

        /// <summary>
        /// 查询是否还存在指定客户端
        /// </summary>
        /// <param name="client"></param>
        public bool CheckExistClient(RemoteConnection client)
        {
            if (serverSocket.SocketType == SocketType.Stream)
                return TcpClients.ContainsKey(client.hashCode);

            // else if (serverSocket.socketType == SocketType.Dgram)
            //     return remoteUDPClients.Values.Contains(client);
            return false;
        }

        /// <summary>
        /// 移除指定客户端
        /// </summary>
        /// <param name="client">指定客户端对象</param>
        internal void RemoveExistClient(RemoteConnection client)
        {
            if (serverSocket.SocketType != SocketType.Stream)
                return;

            if (!TcpClients.ContainsKey(client.hashCode))
            {
                return;
            }

            TcpClients.TryRemove(client.hashCode, out _);
            // close the socket associated with the client
            try
            {
                client.Socket?.Close();
            }
            // throws if client process has already closed
            catch (Exception ex)
            {
                // Log.Exception(ex, "ServerConnection", "CloseClientSocket", "Socket");
                socketInvoke?.SocketException(client, ex);
            }

            // 断开连接回调
            if (socketStatusListener != null)
                socketStatusListener.OnClose(client);

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref numConnectedSockets);
            // Free the SocketAsyncEventArg so they can be reused by another client
            if (client.readWriteEventArg != null)
                readWritePool.Add(client.readWriteEventArg);

            maxNumberAcceptedClients.Release();
        }

        /// <summary>
        /// 设置套接字事件监听
        /// <para>单独设置监听 有可能无法检测到断开连接情况</para>
        /// <para>TCP可以使用监控任务 UDP无法通过监控判断其断开情况</para>
        /// </summary>
        /// <param name="listener"></param>
        public void SetSocketEvent(IServerSocket listener)
        {
            SetSocketEvent(listener, new MonitorSocketStatusTask(-1));
        }

        /// <summary>
        /// 设置套接字事件监听且可定义监控任务
        /// <para>UDP无需使用方法，因为无效</para>
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="task"></param>
        public void SetSocketEvent(IServerSocket listener, MonitorSocketStatusTask task)
        {
            socketStatusListener = listener;
            monitorSocketStatusTask = task;
        }

        /// <summary>
        /// 开始接受新客户端连接
        /// <para>IOCP模型构建</para>
        /// </summary>
        /// <param name="acceptEventArg">异步事件参数</param>
        private void StartAccept(System.Net.Sockets.SocketAsyncEventArgs? acceptEventArg)
        {
            while (isRunning)
            {
                if (acceptEventArg == null)
                {
                    acceptEventArg = new MySocketAsyncEventArgs(this);
                }
                else
                {
                    // socket must be cleared since the context object is being reused
                    acceptEventArg.AcceptSocket = null;
                }
                maxNumberAcceptedClients.WaitOne();
                try
                {
                    bool willRaiseEvent = serverSocket.AcceptAsync(acceptEventArg);

                    if (!willRaiseEvent)
                        ProcessAccept(acceptEventArg, false);
                    else return;
                }
                catch (Exception ex)
                {
                    socketInvoke?.SocketException(null, ex);
                    return;
                }
            }
        }

        /// <summary>
        /// 处理新客户端连接
        /// </summary>
        private void ProcessAccept(System.Net.Sockets.SocketAsyncEventArgs e, bool isFirst)
        {
            // Get the socket for the accepted client connection and put it into the 
            //ReadEventArg object user token
            if (!readWritePool.TryTake(out System.Net.Sockets.SocketAsyncEventArgs? readEventArgs) || readEventArgs == null)
            {
                return;
            }

            if (e.AcceptSocket == null)
            {
                readWritePool.Add(e);
                // Accept the next connection request
                if (isFirst) StartAccept(e);
                return;
            }

            Socket? socket = Socket.FillAsClient(e.AcceptSocket);
            RemoteConnection? client = readEventArgs.UserToken as RemoteConnection;
            if (socket == null || client == null)
            {
                readWritePool.Add(e);
                e.AcceptSocket.Close();
                // Accept the next connection request
                if (isFirst)
                    StartAccept(e);

                return;
            }
            try
            {
                // 初始化
                client.Init(socket/*, IO_Completed!*/);
                // 加入已连接列表
                TcpClients.TryAdd(client.hashCode, client);
                // 新连接回调
                socketStatusListener?.OnConnect(client);
                // 加入监控中
                monitorSocketStatusTask?.PushCheck(client);

                // As soon as the client is connected, post a receive to the connection
                bool willRaiseEvent = socket.ReceiveAsync(readEventArgs);

                if (!willRaiseEvent)
                    ProcessReceive(readEventArgs);

                Interlocked.Increment(ref numConnectedSockets);
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(client, ex);
                client.Destroy();
            }

            if (isFirst)
                StartAccept(e);
        }

        /// <summary>
        /// 接受数据进程
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            while (isRunning)
            {
                // check if the remote host closed the connection
                RemoteConnection? client = e.UserToken as RemoteConnection;
                if (client == null)
                {
                    return;
                }

                if (e.BytesTransferred <= 0 || e.SocketError != System.Net.Sockets.SocketError.Success)
                {
                    client.Destroy();
                    return;
                }

                // 重置超时标记
                if (monitorSocketStatusTask != null)
                    Interlocked.Exchange(ref client.timeoutCount, 0);

                // increment the count of the total bytes receive by the server
                // Interlocked.Add(ref totalBytesRead, e.BytesTransferred);
                // log.Info("ProcessReceive", "The server has read a total of {0} bytes", totalBytesRead);
                // echo the data received back to the client
                if (client.Socket == null)
                {
                    client.Destroy();
                    return;
                }

                try
                {
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
                    client.RBuffer.Decode(e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred).Span);
#else
                    client.RBuffer.Decode(e.Buffer.AsSpan().Slice(e.Offset, e.BytesTransferred));
#endif
                    client.TriggerSocketInvoke();
                    bool willRaiseEvent = client.Socket?.ReceiveAsync(e) ?? true;

                    if (willRaiseEvent)
                        return;
                }
                catch (Exception ex)
                {
                    socketInvoke?.SocketException(client, ex);
                    return;
                }
            }
        }

        /// <summary>
        /// 开始接受新客户端连接
        /// <para>IOCP模型构建</para>
        /// </summary>
        protected void StartAcceptReceiveFrom()
        {
            while (isRunning)
            {
                // Get the socket for the accepted client connection and put it into the 
                //ReadEventArg object user token
                if (!readWritePool.TryTake(out System.Net.Sockets.SocketAsyncEventArgs? readEventArgs) || readEventArgs == null)
                {
                    return;
                }

                maxNumberAcceptedClients.WaitOne();
                Interlocked.Increment(ref numConnectedSockets);

                try
                {
                    bool willRaiseEvent = serverSocket.ReceiveFromAsync(readEventArgs);

                    if (!willRaiseEvent)
                        ProcessReceiveFrom(readEventArgs, false);
                    else
                        return;
                }
                catch (Exception ex)
                {
                    socketInvoke?.SocketException(null, ex);
                    return;
                }
            }
        }

        /// <summary>
        /// 接受数据进程 udp
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isFirst"></param>
        protected void ProcessReceiveFrom(System.Net.Sockets.SocketAsyncEventArgs e, bool isFirst)
        {
            // check if the remote host closed the connection
            // RemoteConnection client = (RemoteConnection)e.UserToken;
            // 初始化
            // client.Init(Socket.FillAsClient(e.RemoteEndPoint), this, socketInvoke);
            // 新连接回调
            // if (socketStatusListener != null) socketStatusListener.OnConnect(client);
            if (e.BytesTransferred <= 0 || e.SocketError != System.Net.Sockets.SocketError.Success)
            {
                // decrement the counter keeping track of the total number of clients connected to the server
                Interlocked.Decrement(ref numConnectedSockets);
                readWritePool.Add(e);

                maxNumberAcceptedClients.Release();

                if (isFirst)
                    StartAcceptReceiveFrom();
                return;
            }
            // 重置超时标记
            // if (monitorSocketStatusTask != null) Interlocked.Exchange(ref client.timeoutCount, 0);
            // increment the count of the total bytes receive by the server
            // Interlocked.Add(ref totalBytesRead, e.BytesTransferred);
            // log.Info("ProcessReceiveFrom", "The server has read a total of {0} bytes", totalBytesRead);
            // echo the data received back to the client
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
            ReadOnlySpan<byte> bufferSpan = e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred).Span;
#else
            ReadOnlySpan<byte> bufferSpan = e.Buffer.AsSpan().Slice(e.Offset, e.BytesTransferred);
#endif
            // Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
            // client.rBuffer.Decode(buffer);
            // client.TriggerSocketInvokeUDP(e.RemoteEndPoint);
            if ((byte)(bufferSpan[0] + bufferSpan[1] + 0x66) != bufferSpan[2])
            {
                Interlocked.Decrement(ref numConnectedSockets);
                readWritePool.Add(e);
                maxNumberAcceptedClients.Release();
                if (isFirst)
                    StartAcceptReceiveFrom();
                return;
            }

            ushort sessionId = (ushort)(((bufferSpan[0] & 0xFF) << 8) | (bufferSpan[1] & 0xFF));
            try
            {
                socketInvoke?.OnReceivedCompleted(new RemoteSocketMsg(sessionId, bufferSpan.Slice(3, bufferSpan.Length - 3).ToArray(), e.RemoteEndPoint!));
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(null, ex);
            }
            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref numConnectedSockets);
            // Free the SocketAsyncEventArg so they can be reused by another client
            readWritePool.Add(e);
            maxNumberAcceptedClients.Release();
            if (isFirst)
                StartAcceptReceiveFrom();
        }

        /// <summary>
        /// IO完成端口
        /// </summary>
        /// <param name="e"></param>
        public void IO_Completed(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case System.Net.Sockets.SocketAsyncOperation.Accept:
                    ProcessAccept(e, true);
                    break;
                case System.Net.Sockets.SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case System.Net.Sockets.SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case System.Net.Sockets.SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFrom(e, true);
                    break;
                default:
                    // Log.Info("IO_Completed The last operation completed on the socket was not a receive or send");
                    break;
            }
        }

        /// <summary>
        /// 线程发送
        /// </summary>
        private static bool ProcessSend(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            ((MySocketAsyncEventArgsEx)e).Push();

            if (e.SocketError == System.Net.Sockets.SocketError.Success)
            {
                // done echoing data back to the client
                // RemoteUserToken token = (RemoteUserToken)e.UserToken;
                return true;
            }
            else
            {
                (e.UserToken as RemoteConnection)?.Destroy();
            }

            return false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="sessionId">会话ID</param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal bool SendAsyncEvent(RemoteConnection client, ushort sessionId, ReadOnlySpan<byte> buffer, int offset, int count)
        {
            if (client.Socket == null)
                return false;

            // 重置超时标记
            if (monitorSocketStatusTask != null)
                Interlocked.Exchange(ref client.timeoutCount, 0);

            // 数据打包
            ReadOnlySpan<byte> data = null;
            if (offset == 0 && buffer.Length == count)
            {
                data = buffer;
            }
            else if (offset > 0)
            {
                var data1 = new byte[count];
                Buffer.BlockCopy(buffer.ToArray(), offset, data1, 0, count);
                data = data1;
            }

            try
            {
                // 数据发送
                var SEAE = client.sendEventArgs?.Pop();

                if (SEAE == null)
                    return false;

                if (client.Socket.SocketType == SocketType.Stream)
                {
                    var mData = SweetStream.Encode(data);
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
                    SEAE.SetBuffer(mData);
#else
                    SEAE.SetBuffer(mData.ToArray(), 0, mData.Length);
#endif
                    bool willRaiseEvent = client.Socket.SendAsync(SEAE);
                    if (!willRaiseEvent)
                        return ProcessSend(SEAE);
                    else
                        return true;

                }
                else if (client.Socket.SocketType == SocketType.Dgram)
                {
                    byte[] sendBuffer = new byte[3 + buffer.Length];
                    /* 会话ID 高位 */
                    // 1 byte
                    sendBuffer[0] = (byte)((sessionId >> 8) & 0xFF);
                    /* 会话ID 低位 */
                    // 1 byte
                    sendBuffer[1] = (byte)((sessionId) & 0xFF);
                    /* 数据长度标识 指令验签（0x66混淆参数）*/
                    // 1 byte
                    sendBuffer[2] = (byte)(sendBuffer[0] + sendBuffer[1] + 0x66);
                    Buffer.BlockCopy(data.ToArray(), 0, sendBuffer, 3, data.Length);
#if !UNITY_2020_1_OR_NEWER && !NET462 && !NETSTANDARD2_0
                    SEAE.SetBuffer(sendBuffer);
#else
                    SEAE.SetBuffer(sendBuffer, 0, sendBuffer.Length);
#endif
                    bool willRaiseEvent = serverSocket.SendToAsync(SEAE);
                    if (!willRaiseEvent)
                        return ProcessSend(SEAE);
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                socketInvoke?.SocketException(client, ex);
                return false;
            }
            return false;
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void CloseServer()
        {
            isRunning = false;

            foreach (var item in TcpClients)
                item.Value.Destroy();

            TcpClients.Clear();

            if (!serverSocket.IsClosed)
                serverSocket.Close();

            monitorSocketStatusTask?.Close();
        }
    }
}
