#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Net;

namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// ESF框架Socket模型
    /// <para>此模型只能一次性使用 [无法重复连接]</para>
    /// <para>不建议直接使用此类创建连接操作</para>
    /// </summary>
    public class Socket
    {
        /// <summary>
        /// ip地址
        /// </summary>
        public string Ip { get; private set; } = "";
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// ip地址对象
        /// </summary>
        internal IPAddress address;
        /// <summary>
        /// ip地址解析终端
        /// </summary>
        internal IPEndPoint endPoint;
        /// <summary>
        /// 地址簇
        /// </summary>
        public AddressFamily AddressFamily { get; private set; }
        /// <summary>
        /// 套接字类型
        /// </summary>
        public SocketType SocketType { get; private set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        public ProtocolType ProtocolType { get; private set; }
        /// <summary>
        /// 套接字连接
        /// </summary>
        private System.Net.Sockets.Socket? socket = null;

        /// <summary>
        /// 连接状态
        /// <para>只要使用类中连接函数且绑定或连接成功，此变量就会为true</para>
        /// </summary>
        public bool IsConnected { get; private set; } = false;
        /// <summary>
        /// 关闭状态
        /// <para>只要调用过类中Close函数或者心跳检测断开连接此变量就会为true</para>
        /// </summary>
        public bool IsClosed { get; private set; } = false;

        /// <summary>
        /// 私有构造
        /// </summary>
        private Socket(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            address = endPoint.Address;
        }

        /// <summary>
        /// 创建一个ESFSocket对象
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        public Socket(string ip, int port)
        {
            Ip = ip;
            Port = port;

            address = IPAddress.Parse(ip);
            endPoint = new IPEndPoint(address, port);
        }

        /// <summary>
        /// 作为服务器连接
        /// </summary>
        /// <param name="addressFamily">地址协议簇 [ipv4或ipv6]</param>
        /// <param name="socketType">套接字类型 [stream或Dgram]</param>
        /// <param name="protocolType">协议类型 [tcp或者udp]</param>
        /// <param name="backlog">同时监听接入连接数 默认为0</param>
        /// <returns>连接是否成功</returns>
        internal bool ConnectAsServer(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, int backlog)
        {
            if (IsConnected && !IsClosed)
                return true;
            else if (IsClosed)
                return false;

            AddressFamily = addressFamily;
            SocketType = socketType;
            ProtocolType = protocolType;
            // try
            // {
            // 创建套接字 并且绑定接口和设置监听
            socket = new System.Net.Sockets.Socket((System.Net.Sockets.AddressFamily)addressFamily, (System.Net.Sockets.SocketType)socketType, (System.Net.Sockets.ProtocolType)protocolType);
            socket.Bind(endPoint);

            if (socketType == SocketType.Stream)
                socket.Listen(backlog);

            IsConnected = true;
            return true;
            // }
            // catch (Exception ex)
            // {
            //     // Log.Exception(ex, "Socket", "ConnectAsServer", "Socket");
            //     return false;
            // }
        }

        /// <summary>
        /// 连接服务器端
        /// <para>作为客户端连接</para>
        /// </summary>
        /// <param name="addressFamily">地址协议簇 [ipv4或ipv6]</param>
        /// <param name="socketType">套接字类型 [stream或Dgram]</param>
        /// <param name="protocolType">协议类型 [tcp或者udp]</param>
        /// <returns></returns>
        public bool Connect(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            if (IsConnected && !IsClosed)
                return true;
            else if (IsClosed)
                return false;

            AddressFamily = addressFamily;
            SocketType = socketType;
            ProtocolType = protocolType;
            try
            {
                // 创建套接字 并且连接服务器
                socket = new System.Net.Sockets.Socket((System.Net.Sockets.AddressFamily)addressFamily, (System.Net.Sockets.SocketType)socketType, (System.Net.Sockets.ProtocolType)protocolType);

                if (socketType == SocketType.Stream)
                    socket.Connect(endPoint);
                else if (socketType == SocketType.Dgram)
                    socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "Socket Connect");
                return false;
            }
        }

        /// <summary>
        /// 获取原生套接字对象
        /// </summary>
        public System.Net.Sockets.Socket? GetSocket()
        {
            return socket;
        }

        /// <summary>
        /// 静态函数：作为客户端填充函数并且返回对象(服务器用) tcp
        /// <para>默认获得一个已连接的socket，将其载入ESFSocket中便于统一管理</para>
        /// </summary>
        /// <param name="s">已连接的套接字</param>
        /// <returns>返回一个ESFSocket</returns>
        internal static Socket? FillAsClient(System.Net.Sockets.Socket s)
        {
            try
            {
                if (s.RemoteEndPoint == null)
                    return null;

                Socket esfSocket = new Socket((IPEndPoint)s.RemoteEndPoint);
                // 再次填充的套接字默认为连接好的
                esfSocket.IsConnected = true;

                esfSocket.AddressFamily = (AddressFamily)s.AddressFamily;
                esfSocket.SocketType = (SocketType)s.SocketType;
                esfSocket.ProtocolType = (ProtocolType)s.ProtocolType;

                esfSocket.Ip = esfSocket.address.ToString();
                esfSocket.Port = esfSocket.endPoint.Port;

                esfSocket.socket = s;

                return esfSocket;
            }
            catch { return null; }
        }

        /// <summary>
        /// 静态函数：作为客户端填充函数并且返回对象(服务器用) udp
        /// <para>默认获得一个已连接的socket，将其载入ESFSocket中便于统一管理</para>
        /// </summary>
        /// <param name="endPoint">链接终端</param>
        /// <returns>返回一个ESFSocket</returns>
        internal static Socket FillAsClient(EndPoint endPoint)
        {
            Socket esfSocket = new Socket((IPEndPoint)endPoint);
            // 再次填充的套接字默认为连接好的
            esfSocket.IsConnected = true;
            esfSocket.AddressFamily = (AddressFamily)endPoint.AddressFamily;
            esfSocket.SocketType = SocketType.Dgram;
            esfSocket.ProtocolType = ProtocolType.Udp;

            esfSocket.Ip = esfSocket.address.ToString();
            esfSocket.Port = esfSocket.endPoint.Port;

            esfSocket.socket = null;

            return esfSocket;
        }

        /// <summary>
        /// 更新EndPoint信息
        /// </summary>
        /// <param name="endPoint">链接终端</param>
        internal void UpdateEndPoint(EndPoint endPoint)
        {
            AddressFamily = (AddressFamily)endPoint.AddressFamily;

            this.endPoint = (IPEndPoint)endPoint;
            address = this.endPoint.Address;

            Ip = address.ToString();
            Port = this.endPoint.Port;
        }

        /// <summary>
        /// 关闭ESFSocket
        /// </summary>
        public void Close()
        {
            if (IsClosed) return;

            if (socket == null) return;

            if (!socket.Connected) return;

            try
            {
                socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            }
            catch { }

            try
            {
                socket.Close();
            }
            catch { }
            IsClosed = true;
        }

        #region 原生Socket部分函数移植 已隐藏
        /// <summary>
        /// TCP异步发送
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool SendAsync(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            if (!IsConnected || IsClosed || socket == null)
                return false;

            return socket.SendAsync(e);
        }

        /// <summary>
        /// UDP异步发送
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool SendToAsync(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            if (!IsConnected || IsClosed || socket == null)
                return false;

            return socket.SendToAsync(e);
        }

        /// <summary>
        /// TCP开始接受数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="socketFlags"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        internal IAsyncResult? BeginReceive(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            if (!IsConnected || IsClosed || socket == null)
                return null;

            return socket.BeginReceive(buffer, offset, size, socketFlags, callback, state);
        }

        /// <summary>
        /// TCP结束接受数据
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        internal int EndReceive(IAsyncResult asyncResult)
        {
            if (!IsConnected || IsClosed || socket == null)
            {
                return 0;
            }    

            return socket.EndReceive(asyncResult);
        }

        /// <summary>
        /// UDP结束接受数据
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        internal int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
        {
            if (!IsConnected || IsClosed || socket == null)
            {
                return 0;
            }

            return socket.EndReceiveFrom(asyncResult, ref endPoint);
        }

        /// <summary>
        /// UDP开始接受数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="socketFlags"></param>
        /// <param name="endPoint"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        internal IAsyncResult? BeginReceiveFrom(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, ref EndPoint endPoint, AsyncCallback callback, object state)
        {
            if (!IsConnected || IsClosed || socket == null)
                return null;

            return socket.BeginReceiveFrom(buffer, offset, size, socketFlags, ref endPoint, callback, state);
        }

        /// <summary>
        /// TCP异步接受新连接
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool AcceptAsync(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            if (!IsConnected || IsClosed || socket == null)
                return false;

            return socket.AcceptAsync(e);
        }

        /// <summary>
        /// TCP异步接受数据
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool ReceiveAsync(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            if (!IsConnected || IsClosed || socket == null)
                return false;

            return socket.ReceiveAsync(e);
        }

        /// <summary>
        /// UDP异步接受数据
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool ReceiveFromAsync(System.Net.Sockets.SocketAsyncEventArgs e)
        {
            if (!IsConnected || IsClosed || socket == null)
                return false;

            return socket.ReceiveFromAsync(e);
        }

        #endregion

    }
}
