using ES.Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ES.Network.Sockets
{
    /// <summary>
    /// Sweet Stream 
    /// <para>糖流体拼装协议</para>
    /// <para>8~10位包头</para>
    /// </summary>
    public class SweetStream
    {
        /// <summary>
        /// 包头 8~10大小
        /// </summary>
        public const int OUTSOURCING_SIZE = 10;
        /// <summary>
        /// 原始数据 解析缓冲区
        /// </summary>
        private byte[] decodeBuffer = null;
        /// <summary>
        /// 原始数据队列
        /// </summary>
        private readonly ConcurrentQueue<StreamBuffer> originalQueue = new ConcurrentQueue<StreamBuffer>();

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
            if (decodeBuffer == null) decodeBuffer = rbuffer;
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
                if (sbPacket != null) { index = i += raw_len; originalQueue.Enqueue(sbPacket.mStreamBuffer); }
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
            else decodeBuffer = null;
        }

        /// <summary>
        /// 检查头部信息是否正确
        /// </summary>
        private StreamBufferPacket ExtractData(byte[] buffer, int index, out int raw_len, out bool isOutRange)
        {
            raw_len = 0;
            // 此处加2为了弥补存在8位头的情况
            int len1 = buffer.Length + 2;
            int len2 = len1 - index;
            if (len2 >= OUTSOURCING_SIZE)
            {
                int k = index;
                // 固定头部
                // 2 byte
                if (buffer[k++] == 0xAA && buffer[k++] == 0x01)
                {
                    // 会话ID 高位
                    // 1 byte 
                    byte highsessionId = buffer[k++];
                    // 会话ID 低位
                    // 1 byte
                    byte lowsessionId = buffer[k++];
                    // 会话ID
                    ushort sessionId = (ushort)(((highsessionId & 0xFF) << 8) | (lowsessionId & 0xFF));
                    // 指令验签（0x66混淆参数）
                    // 1 byte
                    int verifyCmd = highsessionId + lowsessionId + 0x66;
                    // 网络验签标记
                    var verifyCmdNet = buffer[k++];
                    if (/*(byte)((verifyCmd >> 8) & 0xFF) == buffer[k++] &&*/ (byte)(verifyCmd & 0b_01111111) == (verifyCmdNet & 0b_01111111))
                    {
                        // 主数据流长度
                        // 2~4 byte
                        int sblen;
                        // 是否为4位长度
                        bool isFourLen = (verifyCmdNet & 0b_10000000) > 0;
                        if (isFourLen) sblen = ((buffer[k++] & 0xFF) << 24) | ((buffer[k++] & 0xFF) << 16) | ((buffer[k++] & 0xFF) << 8) | (buffer[k++] & 0xFF);
                        else sblen = ((buffer[k++] & 0xFF) << 8) | (buffer[k++] & 0xFF);
                        // 数据流长度验签（结合指令数据和0x88混淆参数）
                        // 0.5 byte
                        if ((byte)((sblen + verifyCmd + 0x88) & 0x0F) == (buffer[k] & 0x0F))
                        {
                            // 补位长度获取
                            // 0.5 byte
                            int fixByteLen = (buffer[k++] & 0xF0) >> 4;
                            // 主数据写入
                            // n byte
                            if (k + sblen - fixByteLen <= len1)
                            {
                                // 所有验证结束， 达到此处 即可获得所有数据参数 提取数据
                                raw_len = OUTSOURCING_SIZE + sblen - (isFourLen ? 0 : 2);
                                StreamBufferPacket sbPacket = new StreamBufferPacket();
                                sbPacket.Length = sblen - fixByteLen;
                                byte[] data = new byte[sbPacket.Length];
                                Buffer.BlockCopy(buffer, k, data, 0, sbPacket.Length);
                                sbPacket.mStreamBuffer = new StreamBuffer(sessionId, data);
                                isOutRange = false;
                                return sbPacket;
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
            isOutRange = false;
            return null;
        }

        /// <summary>
        /// 提取并删除最先进入的数据
        /// </summary>
        public byte[] TakeBuffer()
        {
            StreamBuffer streamBuffer = TakeStreamBuffer();
            if (streamBuffer != null) return streamBuffer.buffer;
            else return null;
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
                    if (originalQueue.TryDequeue(out result)) return result;
                    else return null;
                } while (result != null);
            }
            return null;
        }

        /// <summary>
        /// 通过会话寻找buffer 并且返回，标记使用
        /// </summary>
        /// <returns></returns>
        public List<StreamBuffer> FindBufferByCommand(ushort sessionId)
        {
            var list = new List<StreamBuffer>();
            for (int i = 0, len = originalQueue.Count; i < len; i++)
            {
                if (originalQueue.TryDequeue(out StreamBuffer sb))
                {
                    if (sb.sessionId == sessionId) list.Add(sb);
                    else originalQueue.Enqueue(sb);
                }
            }
            return list;
        }

        /// <summary>
        /// 数据打包
        /// <para>用于发送字节流</para>
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="sbuffer">发送的数据流</param>
        public byte[] Encode(ushort sessionId, byte[] sbuffer)
        {
            // 补齐位数
            int aligncount = 4 - sbuffer.Length % 4;
            // 元数据 + 补齐位数之和
            int sblen = sbuffer.Length + aligncount;
            // 是否为4位长度
            bool isFourLen = (sblen & 0xFFFF0000) > 0;
            // 创建一个数据
            byte[] buffer = new byte[OUTSOURCING_SIZE + sblen - (isFourLen ? 0 : 2)];
            // 字节索引节点
            int index = 0;
            // 数据总长 = 8~10 byte 头部数据 + n byte 主数据 + 4 - n % 4 byte 主数据补齐参
            // n byte 需要4字对齐，不足部分在主数据结尾部分补齐 0x00

            #region 头部验证
            /* 固定头部 */
            // 2 byte
            buffer[index++] = 0xAA;
            buffer[index++] = 0x01;
            /* 会话ID 高位 */
            // 1 byte
            var highsessionId = buffer[index++] = (byte)((sessionId >> 8) & 0xFF);
            /* 会话ID 低位 */
            // 1 byte
            var lowsessionId = buffer[index++] = (byte)((sessionId) & 0xFF);
            /* 数据长度标识 指令验签（0x66混淆参数）*/
            // 1 byte
            int verifyCmd = highsessionId + lowsessionId + 0x66;
            // 高八位 高位不再验证 减少空间
            // buffer[index++] = (byte)((verifyCmd >> 8) & 0xFF);
            // 低八位
            if (isFourLen) buffer[index++] = (byte)(0b_10000000 | (verifyCmd & 0b_01111111));
            else buffer[index++] = (byte)(0b_00000000 | (verifyCmd & 0b_01111111));
            #endregion
            #region 数据长度
            /* 主数据流长度 */
            // 2~4 byte
            if (isFourLen)
            {
                buffer[index++] = (byte)((sblen >> 24) & 0xFF);
                buffer[index++] = (byte)((sblen >> 16) & 0xFF);
            }
            buffer[index++] = (byte)((sblen >> 8) & 0xFF);
            buffer[index++] = (byte)(sblen & 0xFF);
            /* 数据补位长度 数据流长度验签（结合指令数据和0x88混淆参数）*/
            // 1 byte
            buffer[index++] = (byte)((aligncount & 0x0F) << 4 | ((sblen + verifyCmd + 0x88) & 0x0F));
            #endregion
            #region 数据写入
            /* 主数据写入 */
            // n byte
            Buffer.BlockCopy(sbuffer, 0, buffer, index, sbuffer.Length);
            index += sbuffer.Length;
            /* 主数据写入 4 byte 对齐 补 0x00 操作 */
            // n % 4 byte
            for (int i = 0; i < aligncount; i++) buffer[index++] = 0x00;
            #endregion
            return buffer;
        }
    }

    /// <summary>
    /// 流缓冲对象
    /// </summary>
    public class StreamBuffer
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public ushort sessionId { get; private set; } = 0x00;
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
        /// <param name="sessionId"></param>
        /// <param name="buffer"></param>
        public StreamBuffer(ushort sessionId, byte[] buffer)
        {
            this.sessionId = sessionId;
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
