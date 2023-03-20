#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Diagnostics.Contracts;
using System.Security;

namespace ECSharp.Utils
{
    /// <summary>
    /// 字节助手
    /// </summary>
    public static class ByteConverter
    {
        /// <summary>
        /// This field indicates the "endianess" of the architecture.
        /// The value is set to true if the architecture is
        /// little endian; false if it is big endian.
        /// </summary>
#if BIGENDIAN
        public static readonly bool IsLittleEndian /* = false */;
#else
        public static readonly bool IsLittleEndian = true;
#endif

        /// <summary>
        /// 空数组
        /// </summary>
        public static readonly byte[] Empty = Array.Empty<byte>();

        /// <summary>
        /// 数组填充有效长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requiredLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static byte[] PadValidLength(byte[] data, int requiredLength)
        {
            if (data.Length == requiredLength)
            {
                return data;
            }

            if (data.Length > requiredLength)
            {
                throw new ArgumentException("Key length is already greater than or equal to the required length.");
            }

            byte[] paddedKey = new byte[requiredLength];
            Buffer.BlockCopy(data, 0, paddedKey, 0, data.Length);

            // Fill the remaining bytes with the padding character (0x00)
            for (int i = data.Length; i < requiredLength; i++)
            {
                paddedKey[i] = 0x00;
            }

            return paddedKey;
        }

        /// <summary>
        /// 获取byte的实际长度
        /// <para>数组中有连续9个字节连续为0的情况</para>
        /// <para>原理 默认基础类型字节占用情况最大为8个</para>
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        public static int GetValidLength(ReadOnlySpan<byte> bytes)
        {
            int i = 0;
            int len = bytes.Length;

            if (0 == len)
                return i;

            for (; i < len; i++)
            {
                int index = i;
                if (i + 8 < len)
                {
                    int r = bytes[index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index] + bytes[++index];
                    if (r == 0x00) break;
                }
            }

            return i;
        }

        /// <summary>
        /// 获取byte的实际数据
        /// <para>数组中有连续9个字节连续为0的情况</para>
        /// <para>原理 默认基础类型字节占用情况最大为8个</para>
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="retainNum">往有效数据后保留几位，默认为 0 不保留</param>
        /// <returns>实际长度的byte[]</returns>
        public static byte[]? GetValidByte(ReadOnlySpan<byte> bytes, int retainNum = 0)
        {
            int length = GetValidLength(bytes);
            if (0 == length)
                return null;

            // 如果总量大于等于有效长度加保留位数则改值
            int blen = bytes.Length;

            if (blen >= length + retainNum)
                length += retainNum;

            if (blen != length)
                return bytes.Slice(0, length).ToArray();
            else
                return bytes.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(bool value)
        {
            byte[] r = new byte[1];
            r[0] = (byte)(value ? 1 : 0);
            return r;
        }

        /// <summary>
        /// Converts a char into an array of bytes with length two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(char value)
        {
            return GetBytes((short)value);
        }

        /// <summary>
        /// Converts a short into an array of bytes with length two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static byte[] GetBytes(short value)
        {
            byte[] bytes = new byte[2];
            fixed (byte* b = bytes)
                *((short*)b) = value;
            return bytes;
        }

        /// <summary>
        /// Converts an int into an array of bytes with length four.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static byte[] GetBytes(int value)
        {
            byte[] bytes = new byte[4];
            fixed (byte* b = bytes)
                *((int*)b) = value;
            return bytes;
        }

        /// <summary>
        /// Converts a long into an array of bytes with length eight.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static byte[] GetBytes(long value)
        {
            byte[] bytes = new byte[8];
            fixed (byte* b = bytes)
                *((long*)b) = value;
            return bytes;
        }

        /// <summary>
        /// Converts an ushort into an array of bytes with length two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(ushort value)
        {
            return GetBytes((short)value);
        }

        /// <summary>
        /// Converts an uint into an array of bytes with length four.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(uint value)
        {
            return GetBytes((int)value);
        }

        /// <summary>
        /// Converts an unsigned long into an array of bytes with length eight.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(ulong value)
        {
            return GetBytes((long)value);
        }

        /// <summary>
        /// Converts a float into an array of bytes with length four.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static byte[] GetBytes(float value)
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == 4);

            return GetBytes(*(int*)&value);
        }

        /// <summary>
        /// Converts a double into an array of bytes with length eight.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static byte[] GetBytes(double value)
        {
            return GetBytes(*(long*)&value);
        }

        /// <summary>
        /// Converts an array of bytes into a char.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static char ToChar(byte[] value, int startIndex)
        {
            return (char)ToInt16(value, startIndex);
        }

        /// <summary>
        /// Converts an array of bytes into a short.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static unsafe short ToInt16(byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 2 == 0)
                { // data is aligned 
                    return *((short*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        return (short)((*pbyte) | (*(pbyte + 1) << 8));
                    }
                    else
                    {
                        return (short)((*pbyte << 8) | (*(pbyte + 1)));
                    }
                }
            }

        }

        /// <summary>
        /// Converts an array of bytes into an int.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static unsafe int ToInt32(byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 4 == 0)
                { // data is aligned 
                    return *((int*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        return (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                    }
                    else
                    {
                        return (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                    }
                }
            }
        }

        /// <summary>
        /// Converts an array of bytes into a long.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static unsafe long ToInt64(byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 8 == 0)
                { // data is aligned 
                    return *((long*)pbyte);
                }
                else
                {
                    if (IsLittleEndian)
                    {
                        int i1 = (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
                        int i2 = (*(pbyte + 4)) | (*(pbyte + 5) << 8) | (*(pbyte + 6) << 16) | (*(pbyte + 7) << 24);
                        return (uint)i1 | ((long)i2 << 32);
                    }
                    else
                    {
                        int i1 = (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
                        int i2 = (*(pbyte + 4) << 24) | (*(pbyte + 5) << 16) | (*(pbyte + 6) << 8) | (*(pbyte + 7));
                        return (uint)i2 | ((long)i1 << 32);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an array of bytes into an ushort.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return (ushort)ToInt16(value, startIndex);
        }

        /// <summary>
        /// Converts an array of bytes into an uint.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            return (uint)ToInt32(value, startIndex);
        }

        /// <summary>
        /// Converts an array of bytes into an unsigned long.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            return (ulong)ToInt64(value, startIndex);
        }

        /// <summary>
        /// Converts an array of bytes into a float.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        unsafe public static float ToSingle(byte[] value, int startIndex)
        {
            int val = ToInt32(value, startIndex);
            return *(float*)&val;
        }

        /// <summary>
        /// Converts an array of bytes into a double.  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        unsafe public static double ToDouble(byte[] value, int startIndex)
        {
            long val = ToInt64(value, startIndex);
            return *(double*)&val;
        }

        private static char GetHexValue(int i)
        {
            Contract.Assert(i >= 0 && i < 16, "i is out of range.");
            if (i < 10)
            {
                return (char)(i + '0');
            }

            return (char)(i - 10 + 'A');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static bool ToBoolean(byte[] value, int startIndex)
        {
            return (value[startIndex] == 0) ? false : true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static unsafe long DoubleToInt64Bits(double value)
        {
            return *((long*)&value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public static unsafe double Int64BitsToDouble(long value)
        {
            return *((double*)&value);
        }
    }
}
