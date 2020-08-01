using System.Collections.Generic;

namespace ES.Network.Sockets
{
    using System.Net.Sockets;

    /// <summary>
    /// 数据管理器
    /// </summary>
    internal class BufferManager
    {
        int numBytes;                 // the total number of bytes controlled by the buffer pool
        byte[] buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> freeIndexPool;     // 
        int currentIndex;
        int bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            numBytes = totalBytes;
            currentIndex = 0;
            this.bufferSize = bufferSize;
            freeIndexPool = new Stack<int>();
        }

        // Allocates buffer space used by the buffer pool
        public void InitBuffer()
        {
            // create one big large buffer and divide that 
            // out to each SocketAsyncEventArg object
            buffer = new byte[numBytes];
        }

        // Assigns a buffer from the buffer pool to the 
        // specified SocketAsyncEventArgs object
        // <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (freeIndexPool.Count > 0)
            {
                args.SetBuffer(buffer, freeIndexPool.Pop(), bufferSize);
            }
            else
            {
                if ((numBytes - bufferSize) < currentIndex)
                {
                    return false;
                }
                args.SetBuffer(buffer, currentIndex, bufferSize);
                currentIndex += bufferSize;
            }
            return true;
        }

        // Removes the buffer from a SocketAsyncEventArg object.  
        // This frees the buffer back to the buffer pool
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }
}
