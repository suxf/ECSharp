using ECSharp.Utils;
using System;
using System.Collections.Concurrent;

namespace ECSharp.Network.Sockets
{
    /// <summary>
    /// Sweet Stream 
    /// <para>糖流体拼装协议</para>
    /// </summary>
    public class SweetStream
    {
        /// <summary>
        /// 包体最大大小
        /// </summary>
        public const int PACKAGE_MAX_SIZE = PACKAGE_SIZE + 4;
        /// <summary>
        /// 包体大小
        /// </summary>
        private const int PACKAGE_SIZE = 2;
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
            {
                return;
            }

            // 赋值解析
            if (decodeBuffer == ByteConverter.Empty)
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
            int msLen = msByte.Length;
            // 开始处理数据
            for (int i = 0; i < msLen;)
            {
                if (ExtractData(msByte, i, out byte[] sbPacket, out int raw_len, out bool isOutRange))
                {
                    index = i += raw_len;
                    originalQueue.Enqueue(sbPacket);
                }
                else if (isOutRange)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }

            // 判断是否存在残留数据
            if (index < msLen)
            {
                int oldLen = msLen - index;
                decodeBuffer = new byte[oldLen];
                Buffer.BlockCopy(msByte, index, decodeBuffer, 0, oldLen);
            }
            else
            {
                decodeBuffer = ByteConverter.Empty;
            }
        }

        /// <summary>
        /// 检查头部信息是否正确
        /// </summary>
        private static bool ExtractData(ReadOnlySpan<byte> buffer, int index, out byte[] result, out int raw_len, out bool isOutRange)
        {
            raw_len = 0;
            // 此处加2为了弥补存在位头的情况
            int len = buffer.Length;
            if (len - index < PACKAGE_SIZE)
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            int k = index;
            // 固定头部
            // 1 byte
            byte header = buffer[k++];
            if ((header & 0b_000_1_0000) != 0b_000_1_0000)
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            // 外围数据真实长度
            int outlineRealLen = PACKAGE_SIZE;
            // 数位类型
            int digitType = (0b_111_0_0000 & header) >> 5;
            // 主数据流长度
            int sblen = 0;
            switch (digitType)
            {
                case 0:
                    sblen = 0x0F & header;
                    break;
                case 1:
                    sblen = (0x0F & header) | (buffer[k++] << 4);
                    outlineRealLen += 1;
                    break;
                case 2:
                    sblen = (0x0F & header) | (buffer[k++] << 4) | (buffer[k++] << 12);
                    outlineRealLen += 2;
                    break;
                case 3:
                    sblen = (0x0F & header) | (buffer[k++] << 4) | (buffer[k++] << 12) | (buffer[k++] << 20);
                    outlineRealLen += 3;
                    break;
                case 4:
                    sblen = buffer[k++] | (buffer[k++] << 8) | (buffer[k++] << 16) | (buffer[k++] << 24);
                    outlineRealLen += 4;
                    break;
            }

            // 重新判定是否足够包体判断
            if (len - index < outlineRealLen)
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            // 网络验签标记
            var verifyCmdNet = buffer[k++];

            // 当已经捕获到数据长度后，说明数据段已确认，如果不足则直接跳过不再循环
            if (k + sblen > len)
            {
                isOutRange = true;
                result = ByteConverter.Empty;
                return false;
            }

            if (sblen == 0)
            {
                if (((verifyCmdNet >> 4) & 0xF) != 0x00)
                {
                    isOutRange = false;
                    result = ByteConverter.Empty;
                    return false;
                }
            }
            else if (sblen == 1)
            {
                if (((verifyCmdNet >> 4) & 0xF) != (buffer[k] & 0xF))
                {
                    isOutRange = false;
                    result = ByteConverter.Empty;
                    return false;
                }
            }
            else
            {
                if (((verifyCmdNet >> 4) & 0xF) != ((buffer[k] + buffer[k + sblen - 1]) & 0xF))
                {
                    isOutRange = false;
                    result = ByteConverter.Empty;
                    return false;
                }
            }

            // 是否为4位长度
            // 数据流长度验签（结合指令数据和0x88混淆参数）
            if (((sblen + 0x88) & 0x0F) != (verifyCmdNet & 0x0F))
            {
                isOutRange = false;
                result = ByteConverter.Empty;
                return false;
            }

            // 所有验证结束， 达到此处 即可获得所有数据参数 提取数据
            raw_len = outlineRealLen + sblen;
            result = buffer.Slice(k, sblen).ToArray();
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
            int sblen = sbuffer.Length;
            // 外围数据真实长度
            int outlineRealLen = PACKAGE_SIZE;
            // 数位类型
            byte digitType;
            if (sblen <= 0xF) { digitType = 0; }
            else if (sblen <= 0xF_FF) { digitType = 1; outlineRealLen += 1; }
            else if (sblen <= 0xF_FF_FF) { digitType = 2; outlineRealLen += 2; }
            else if (sblen <= 0xF_FF_FF_FF) { digitType = 3; outlineRealLen += 3; }
            else { digitType = 4; outlineRealLen += 4; }

            // 创建一个数据
            byte[] buffer = new byte[outlineRealLen + sblen];
            // 字节索引节点
            int index = 0;
            byte[] header = new byte[outlineRealLen];
            // 数据总长 = 头部数据 + 主数据 + 主数据补齐参
            // 不足偶数部分在主数据结尾部分补齐 0x00
            /* 固定头部 */
            // 1 byte
            switch (digitType)
            {
                case 0:
                    header[index++] = (byte)(0b_000_1_0000 + (0b_111_0_0000 & digitType << 5) + (0b_000_0_1111 & sblen));
                    break;
                case 1:
                    header[index++] = (byte)(0b_000_1_0000 + (0b_111_0_0000 & digitType << 5) + (0b_000_0_1111 & sblen));
                    header[index++] = (byte)(0xFF & sblen >> 4);
                    break;
                case 2:
                    header[index++] = (byte)(0b_000_1_0000 + (0b_111_0_0000 & digitType << 5) + (0b_000_0_1111 & sblen));
                    header[index++] = (byte)(0xFF & sblen >> 4);
                    header[index++] = (byte)(0xFF & sblen >> 12);
                    break;
                case 3:
                    header[index++] = (byte)(0b_000_1_0000 + (0b_111_0_0000 & digitType << 5) + (0b_000_0_1111 & sblen));
                    header[index++] = (byte)(0xFF & sblen >> 4);
                    header[index++] = (byte)(0xFF & sblen >> 12);
                    header[index++] = (byte)(0xFF & sblen >> 20);
                    break;
                case 4:
                    header[index++] = (byte)(0b_000_1_0000 + (0b_111_0_0000 & digitType << 5));
                    header[index++] = (byte)(0xFF & sblen);
                    header[index++] = (byte)(0xFF & sblen >> 8);
                    header[index++] = (byte)(0xFF & sblen >> 16);
                    header[index++] = (byte)(0xFF & sblen >> 24);
                    break;
            }
            /* 数据流长度验签（结合指令数据和0x88混淆参数）*/
            // 1 byte
            if (sblen == 0)
            {
                header[index++] = (byte)((sblen + 0x88) & 0x0F);
            }
            else if (sblen == 1)
            {
                header[index++] = (byte)((sbuffer[0] & 0x0F) << 4 | ((sblen + 0x88) & 0x0F));
            }
            else
            {
                header[index++] = (byte)(((sbuffer[0] + sbuffer[sblen - 1]) & 0x0F) << 4 | ((sblen + 0x88) & 0x0F));
            }
            /* 主数据写入 */
            // n byte
            Buffer.BlockCopy(header, 0, buffer, 0, outlineRealLen);
            Buffer.BlockCopy(sbuffer.ToArray(), 0, buffer, index, sblen);
            // index += len;

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
