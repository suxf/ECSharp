using ES.Utils;
using System;
using System.Collections.Concurrent;

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
        /// 包体 8~10大小
        /// </summary>
        public const int OUTSOURCING_SIZE = 7;
        /// <summary>
        /// 原始数据 解析缓冲区
        /// </summary>
        private byte[] decodeBuffer = ByteConverter.Empty;
        /// <summary>
        /// 原始数据队列
        /// </summary>
        private readonly ConcurrentQueue<byte[]> originalQueue = new ConcurrentQueue<byte[]>();

        /// <summary>
        /// 放入数据进行等待解析
        /// </summary>
        /// <param name="buffer">接受到的数据流</param>
        public void Decode(ReadOnlySpan<byte> buffer)
        {
            var rbuffer = ByteConverter.GetValidByte(buffer);
            // 如果接受的数据为空 则直接跳出
            if (rbuffer == null || rbuffer.Length <= 0)
                return;

            // 赋值解析
            if (decodeBuffer == ByteConverter.Empty) decodeBuffer = rbuffer;
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
            int msLen = msByte.Length;
            // 开始处理数据
            for (int i = 0; i < msLen;)
            {
                if (ExtractData(msByte, i, out byte[] sbPacket, out int raw_len, out bool isOutRange))
                {
                    index = i += raw_len;
                    originalQueue.Enqueue(sbPacket);
                }
                else if (isOutRange) break;
                else i++;
            }

            // 判断是否存在残留数据
            if (index < msLen)
            {
                int oldLen = msLen - index;
                decodeBuffer = new byte[oldLen];
                Buffer.BlockCopy(msByte, index, decodeBuffer, 0, oldLen);
            }
            else decodeBuffer = ByteConverter.Empty;
        }

        /// <summary>
        /// 检查头部信息是否正确
        /// </summary>
        private static bool ExtractData(ReadOnlySpan<byte> buffer, int index, out byte[] result, out int raw_len, out bool isOutRange)
        {
            raw_len = 0;
            // 此处加2为了弥补存在位头的情况
            int len = buffer.Length;
            if (len + 2 - index < OUTSOURCING_SIZE)
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            int k = index;
            // 固定头部
            // 2 byte
            if (buffer[k++] != 0xAA || buffer[k++] != 0x01)
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            // 网络验签标记
            var verifyCmdNet = buffer[k++];
            // 主数据流长度
            // 2~4 byte
            int sblen;
            // 是否为4位长度
            bool isFourLen = (verifyCmdNet & 0b_10000000) > 0;
            if (isFourLen) sblen = ((buffer[k++] & 0xFF) << 24) | ((buffer[k++] & 0xFF) << 16) | ((buffer[k++] & 0xFF) << 8) | (buffer[k++] & 0xFF);
            else sblen = ((buffer[k++] & 0xFF) << 8) | (buffer[k++] & 0xFF);
            // 数据流长度验签（结合指令数据和0x88混淆参数）
            // 0.5 byte
            if ((byte)((sblen + 0x88) & 0x0F) != (verifyCmdNet & 0x0F))
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            // 补位长度获取
            // 0.5 byte
            int fixByteLen = (verifyCmdNet >> 4) & 0b_01111111;
            // 主数据写入
            // n byte
            int blen = sblen - fixByteLen;
            if (k + blen > len)
            {
                // 优化实现 当已经捕获到数据长度后，说明数据段已确认，如果不足则直接跳过不再循环
                isOutRange = true;
                result = ByteConverter.Empty;
                return false;
            }

            // 所有验证结束， 达到此处 即可获得所有数据参数 提取数据
            raw_len = OUTSOURCING_SIZE + sblen - (isFourLen ? 0 : 2);
            result = buffer.Slice(k, blen).ToArray();
            isOutRange = false;
            return true;
        }

        /// <summary>
        /// 数据打包
        /// <para>用于发送字节流</para>
        /// </summary>
        /// <param name="sbuffer">发送的数据流</param>
        public static Memory<byte> Encode(ReadOnlySpan<byte> sbuffer)
        {
            int slen = sbuffer.Length;
            // 补齐位数
            int aligncount = (slen + 1) % 2;
            // 元数据 + 补齐位数之和
            int sblen = slen + aligncount;
            // 是否为4位长度
            bool isFourLen = (sblen & 0xFFFF0000) > 0;
            // 外围数据真实长度
            int outlineRealLen = OUTSOURCING_SIZE - (isFourLen ? 0 : 2);
            // 创建一个数据
            byte[] buffer = new byte[outlineRealLen + sblen];
            // 字节索引节点
            int index = 0;
            Span<byte> header = stackalloc byte[OUTSOURCING_SIZE];
            // 数据总长 = 头部数据 + 主数据 + 主数据补齐参
            // 不足偶数部分在主数据结尾部分补齐 0x00
            #region 头部验证
            /* 固定头部 */
            // 2 byte
            header[index++] = 0xAA;
            header[index++] = 0x01;
            /* 数据补位长度 数据流长度验签（结合指令数据和0x88混淆参数）*/
            // 1 byte
            int signIndex = index++;
            header[signIndex] = (byte)((aligncount & 0x0F) << 4 | ((sblen + 0x88) & 0x0F));
            /* 主数据流长度 */
            // 2~4 byte
            if (isFourLen)
            {
                header[signIndex] = (byte)(0b_11111111 & header[signIndex]);
                header[index++] = (byte)((sblen >> 24) & 0xFF);
                header[index++] = (byte)((sblen >> 16) & 0xFF);
            }
            else header[signIndex] = (byte)(0b_01111111 & header[signIndex]);
            header[index++] = (byte)((sblen >> 8) & 0xFF);
            header[index++] = (byte)(sblen & 0xFF);
            #endregion
            #region 数据写入
            /* 主数据写入 */
            // n byte
            Buffer.BlockCopy(header.ToArray(), 0, buffer, 0, outlineRealLen);
            Buffer.BlockCopy(sbuffer.ToArray(), 0, buffer, index, slen);
            index += slen;
            /* 主数据写入对齐 补 0x00 操作 */
            // 结尾
            /*for (int i = 0; i < aligncount; i++)*/
            if (aligncount == 1) buffer[index++] = 0xFF;
            #endregion
            return buffer;
        }

        /// <summary>
        /// 提取并删除最先进入的数据
        /// </summary>
        public byte[]? TakeStreamBuffer()
        {
            if (originalQueue.TryDequeue(out var result)) return result;
            else return null;
        }
    }
}
