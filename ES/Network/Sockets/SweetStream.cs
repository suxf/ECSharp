using ES.Common.Utils;
using System;
using System.Collections.Concurrent;

namespace ES.Network.Sockets
{
    /// <summary>
    /// Sweet Stream 
    /// 糖流体拼装协议
    /// 固定16位包头+包尾
    /// </summary>
    public class SweetStream
    {
        /// <summary>
        /// 包体外 包头+包尾 固定大小
        /// </summary>
        public const int OUTSOURCING_SIZE = 16;
        /// <summary>
        /// 原始数据 解析缓冲区
        /// </summary>
        private byte[] decodeBuffer = null;
        /// <summary>
        /// 原始数据队列
        /// </summary>
        private ConcurrentQueue<StreamBuffer> originalQueue = new ConcurrentQueue<StreamBuffer>();

        /// <summary>
        /// 放入数据进行等待解析
        /// </summary>
        /// <param name="rbuffer">接受到的数据流</param>
        public void Decode(byte[] rbuffer)
        {
            rbuffer = ByteHelper.GetValidByte(rbuffer);
            // 如果接受的数据为空 则直接跳出
            if (rbuffer == null || rbuffer.Length <= 0) return;

            // 赋值解析
            if (decodeBuffer == null)
            {
                decodeBuffer = rbuffer;
            }
            else
            {
                int dbLen = decodeBuffer.Length;
                int rbLen = rbuffer.Length;
                byte[] contactBuffer = new byte[dbLen + rbLen];
                Buffer.BlockCopy(decodeBuffer, 0, contactBuffer, 0, dbLen);
                Buffer.BlockCopy(rbuffer, 0, contactBuffer, dbLen, rbLen);
                decodeBuffer = contactBuffer;
            }

            // 当前存在的索引
            int index = 0;
            // 得到缓存数组
            byte[] msByte = decodeBuffer;

            // 开始处理数据
            for (int i = 0, len = msByte.Length; i < len;)
            {
                StreamBufferPacket sbPacket = ExtractData(msByte, i, out int raw_len, out bool isOutRange);
                if (sbPacket != null) { index = i = i + raw_len; originalQueue.Enqueue(sbPacket.mStreamBuffer); }
                else if (isOutRange) break;
                else i++;
            }

            // 判断是否存在残留数据
            if (index < msByte.Length)
            {
                int oldLen = msByte.Length - index;
                decodeBuffer = new byte[oldLen];
                Buffer.BlockCopy(msByte, index, decodeBuffer, 0, oldLen);
            }
            else
            {
                decodeBuffer = null;
            }
        }

        /// <summary>
        /// 检查头部信息是否正确
        /// </summary>
        private StreamBufferPacket ExtractData(byte[] buffer, int index, out int raw_len, out bool isOutRange)
        {
            raw_len = 0;
            int len = buffer.Length - index;
            if (len >= 14)
            {
                int k = index;
                // 固定头部
                // 3 byte
                if (buffer[k++] == 0xAA && buffer[k++] == 0x01 && buffer[k++] == 0xFF)
                {
                    // 主要指令 次要指令 
                    // 1 byte   1 byte
                    byte main = buffer[k++];
                    byte second = buffer[k++];
                    // 指令验签（0x66混淆参数）
                    // 2 byte
                    int verifyCmd = main + second + 0x66;
                    if ((byte)((verifyCmd >> 8) & 0xFF) == buffer[k++] && (byte)(verifyCmd & 0xFF) == buffer[k++])
                    {
                        // 指令结束位
                        // 1 byte
                        if (buffer[k++] == 0x0A)
                        {
                            // 主数据流长度
                            // 4 byte
                            int sblen = ((buffer[k++] & 0xFF) << 24) | ((buffer[k++] & 0xFF) << 16) | ((buffer[k++] & 0xFF) << 8) | (buffer[k++] & 0xFF);
                            // 数据流长度验签（结合指令数据和0x88混淆参数）
                            // 1 byte
                            if ((byte)((sblen + verifyCmd + 0x88) & 0xFF) == buffer[k++])
                            {
                                // 补位长度获取
                                // 1 byte
                                int fixByteLen = buffer[k];
                                // 主数据写入
                                // n byte
                                if (k + sblen + 2 < len)
                                {
                                    // 数据流长度作为检验结束位（0x99混淆参数） 主数据结束位
                                    // 1 byte                                  1 byte
                                    if ((byte)((sblen + 0x99) & 0xFF) == buffer[k + sblen + 1] && buffer[k + sblen + 2] == 0x0C)
                                    {
                                        // 所有验证结束， 达到此处 即可获得所有数据参数 提取数据
                                        raw_len = 16 + sblen;
                                        StreamBufferPacket sbPacket = new StreamBufferPacket();
                                        sbPacket.Length = sblen - fixByteLen;
                                        byte[] data = new byte[sbPacket.Length];
                                        Buffer.BlockCopy(buffer, k + 1, data, 0, sbPacket.Length);
                                        sbPacket.mStreamBuffer = new StreamBuffer(main, second, data);
                                        isOutRange = false;
                                        return sbPacket;
                                    }
                                }
                                else
                                {
                                    // 优化实现 当已经捕获到数据长度后，说明数据段已确认，如果不足则直接跳过不再循环
                                    isOutRange = true;
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
            isOutRange = false;
            return null;
        }

        /// <summary>
        /// 提取并删除最先进入的数据
        /// </summary>
        public byte[] TakeBuffer()
        {
            StreamBuffer streamBuffer = TakeStreamBuffer();
            if (streamBuffer != null)
                return streamBuffer.buffer;
            else
                return null;
        }

        /// <summary>
        /// 提取并删除最先进入的数据
        /// </summary>
        public StreamBuffer TakeStreamBuffer()
        {
            if (!originalQueue.IsEmpty)
            {
                StreamBuffer result;
                do
                {
                    if (originalQueue.TryDequeue(out result))
                    {
                        return result;
                    }
                    else
                        return null;
                } while (result != null);
            }
            return null;
        }

        /// <summary>
        /// 通过指令寻找buffer 并且返回，标记使用
        /// </summary>
        /// <param name="main"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public StreamBuffer FindBufferByCommand(byte main, byte second)
        {
            for (int i = 0, len = originalQueue.Count; i < len; i++)
            {
                if (originalQueue.TryDequeue(out StreamBuffer sb))
                {
                    if (sb.main == main && sb.second == second)
                    {
                        return sb;
                    }
                    else
                    {
                        originalQueue.Enqueue(sb);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 数据打包
        /// 用于发送字节流
        /// </summary>
        /// <param name="sbuffer">发送的数据流</param>
        public byte[] Encode(byte[] sbuffer)
        {
            return Encode(0x01, 0xFF, sbuffer);
        }

        /// <summary>
        /// 数据打包
        /// 用于发送字节流
        /// </summary>
        /// <param name="sb">发送流缓冲对象</param>
        public byte[] Encode(StreamBuffer sb)
        {
            return Encode(sb.main, sb.second, sb.buffer);
        }

        /// <summary>
        /// 数据打包
        /// 用于发送字节流
        /// </summary>
        /// <param name="main">主命令</param>
        /// <param name="second">次要命令</param>
        /// <param name="sbuffer">发送的数据流</param>
        public byte[] Encode(byte main, byte second, byte[] sbuffer)
        {
            // 补齐位数
            int aligncount = 4 - sbuffer.Length % 4;
            // 元数据 + 补齐位数之和
            int sblen = sbuffer.Length + aligncount;
            // 创建一个数据
            byte[] buffer = new byte[OUTSOURCING_SIZE + sblen];
            // 字节索引节点
            int index = 0;
            // 数据总长 = 14 byte 头部数据 + n byte 主数据 + 4 - n % 4 byte 主数据补齐参 + 2 byte 结尾数据
            // n byte 需要4字对齐，不足部分在主数据结尾部分补齐 0x00

            // 固定头部
            // 3 byte
            buffer[index++] = 0xAA;
            buffer[index++] = 0x01;
            buffer[index++] = 0xFF;
            // 主要指令
            // 1 byte
            buffer[index++] = main;
            // 次要指令 
            // 1 byte
            buffer[index++] = (second);
            // 指令验签（0x66混淆参数）
            // 2 byte
            int verifyCmd = main + second + 0x66;
            // 高八位
            buffer[index++] = (byte)((verifyCmd >> 8) & 0xFF);
            // 低八位
            buffer[index++] = (byte)(verifyCmd & 0xFF);
            // 指令结束位
            // 1 byte
            buffer[index++] = 0x0A;
            // 主数据流长度
            // 4 byte
            buffer[index++] = (byte)((sblen >> 24) & 0xFF);
            buffer[index++] = (byte)((sblen >> 16) & 0xFF);
            buffer[index++] = (byte)((sblen >> 8) & 0xFF);
            buffer[index++] = (byte)(sblen & 0xFF);
            // 数据流长度验签（结合指令数据和0x88混淆参数）
            // 1 byte
            buffer[index++] = (byte)((sblen + verifyCmd + 0x88) & 0xFF);
            // 数据补位长度
            buffer[index++] = (byte)(aligncount & 0xFF);
            // 主数据写入
            // n byte
            Buffer.BlockCopy(sbuffer, 0, buffer, index, sbuffer.Length);
            index = index + sbuffer.Length;
            // 主数据写入 4 byte 对齐 补 0x00 操作
            // n % 4 byte
            for (int i = 0; i < aligncount; i++) buffer[index++] = 0x00;

            // 数据流长度作为检验结束位（0x99混淆参数）
            // 1 byte
            buffer[index++] = (byte)((sblen + 0x99) & 0xFF);
            // 主数据结束位
            // 1 byte
            buffer[index++] = 0x0C;
            return buffer;
        }
    }

    /// <summary>
    /// 流缓冲对象
    /// </summary>
    public class StreamBuffer
    {
        /// <summary>
        /// 主要命令
        /// 默认:0x01
        /// </summary>
        public byte main { get; private set; } = 0x01;
        /// <summary>
        /// 次要命令
        /// 默认:0xff
        /// </summary>
        public byte second { get; private set; } = 0xFF;
        /// <summary>
        /// 缓存比特数组
        /// </summary>
        public byte[] buffer { get; private set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer"></param>
        public StreamBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="main"></param>
        /// <param name="second"></param>
        /// <param name="buffer"></param>
        public StreamBuffer(byte main, byte second, byte[] buffer)
        {
            this.main = main;
            this.second = second;
            this.buffer = buffer;
        }

    }

    /// <summary>
    /// 流缓冲对象 解析包
    /// </summary>
    internal class StreamBufferPacket
    {
        /// <summary>
        /// 流缓冲主数据长度
        /// </summary>
        public int Length = 0;

        /// <summary>
        /// 流缓冲
        /// </summary>
        public StreamBuffer mStreamBuffer;
    }

}
