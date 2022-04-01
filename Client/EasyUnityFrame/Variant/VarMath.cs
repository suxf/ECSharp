namespace ES.Variant
{
    public readonly partial struct Var
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator +(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.NULL: return Empty;
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.NULL: return Empty;
                        case VarType.INT32: return left.intValue + right.intValue;
                        case VarType.UINT32: return left.intValue + (uint)right.intValue;
                        case VarType.INT64: return left.intValue + right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return ((ulong)right.longValue) - ((uint)-left.intValue);
                            else return (uint)left.intValue + (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue + right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.NULL: return Empty;
                        case VarType.INT32: return (uint)left.intValue + right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue + right.intValue);
                        case VarType.INT64: return (uint)left.intValue + right.longValue;
                        case VarType.UINT64: return (uint)left.intValue + (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue + right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.NULL: return Empty;
                        case VarType.INT32: return left.longValue + right.intValue;
                        case VarType.UINT32: return left.longValue + (uint)right.intValue;
                        case VarType.INT64: return left.longValue + right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return ((ulong)right.longValue) - ((ulong)-left.longValue);
                            else return (ulong)left.longValue + (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue + right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.NULL: return Empty;
                        case VarType.INT32:
                            if (right.intValue < 0) return ((ulong)left.longValue) - ((uint)-right.intValue);
                            else return (ulong)left.longValue + (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue + (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return ((ulong)left.longValue) - ((ulong)-right.longValue);
                            else return (ulong)left.longValue + (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue + right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue + right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.NULL: return Empty;
                        case VarType.INT32: return left.doubleValue + right.intValue;
                        case VarType.UINT32: return left.doubleValue + (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue + right.longValue;
                        case VarType.UINT64: return left.doubleValue + (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue + right.doubleValue;
                    }
                    break;
            }
            return left.ToString() + right.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator -(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue - right.intValue;
                        case VarType.UINT32: return left.intValue - (uint)right.intValue;
                        case VarType.INT64: return left.intValue - right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return ((uint)-left.intValue) + ((ulong)right.longValue);
                            else return (uint)left.intValue - (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue - right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue - right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue - right.intValue);
                        case VarType.INT64: return (uint)left.intValue - right.longValue;
                        case VarType.UINT64: return (uint)left.intValue - (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue - right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue - right.intValue;
                        case VarType.UINT32: return left.longValue - (uint)right.intValue;
                        case VarType.INT64: return left.longValue - right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return ((ulong)-left.longValue) + ((ulong)right.longValue);
                            else return (ulong)left.longValue - (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue - right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return ((ulong)left.longValue) + ((uint)-right.intValue);
                            else return (ulong)left.longValue - (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue - (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return ((ulong)left.longValue) + ((ulong)-right.longValue);
                            else return (ulong)left.longValue - (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue - right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue - right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue - right.intValue;
                        case VarType.UINT32: return left.doubleValue - (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue - right.longValue;
                        case VarType.UINT64: return left.doubleValue - (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue - right.doubleValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator *(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue * right.intValue;
                        case VarType.UINT32: return left.intValue * (uint)right.intValue;
                        case VarType.INT64: return left.intValue * right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return left.intValue * right.longValue;
                            else return (uint)left.intValue * (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue * right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue * right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue * right.intValue);
                        case VarType.INT64: return (uint)left.intValue * right.longValue;
                        case VarType.UINT64: return (uint)left.intValue * (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue * right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue * right.intValue;
                        case VarType.UINT32: return left.longValue * (uint)right.intValue;
                        case VarType.INT64: return left.longValue * right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue * right.longValue;
                            else return (ulong)left.longValue * (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue * right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue * right.intValue;
                            else return (ulong)left.longValue * (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue * (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue * right.longValue;
                            else return (ulong)left.longValue * (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue * right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue * right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue * right.intValue;
                        case VarType.UINT32: return left.doubleValue * (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue * right.longValue;
                        case VarType.UINT64: return left.doubleValue * (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue * right.doubleValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator /(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue / right.intValue;
                        case VarType.UINT32: return left.intValue / (uint)right.intValue;
                        case VarType.INT64: return left.intValue / right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return left.intValue / right.longValue;
                            else return (uint)left.intValue / (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue / right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue / right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue / right.intValue);
                        case VarType.INT64: return (uint)left.intValue / right.longValue;
                        case VarType.UINT64: return (uint)left.intValue / (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue / right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue / right.intValue;
                        case VarType.UINT32: return left.longValue / (uint)right.intValue;
                        case VarType.INT64: return left.longValue / right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue / right.longValue;
                            else return (ulong)left.longValue / (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue / right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue / right.intValue;
                            else return (ulong)left.longValue / (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue / (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue / right.longValue;
                            else return (ulong)left.longValue / (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue / right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue / right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue / right.intValue;
                        case VarType.UINT32: return left.doubleValue / (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue / right.longValue;
                        case VarType.UINT64: return left.doubleValue / (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue / right.doubleValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator %(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue % right.intValue;
                        case VarType.UINT32: return left.intValue % (uint)right.intValue;
                        case VarType.INT64: return left.intValue % right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return left.intValue % right.longValue;
                            else return (uint)left.intValue % (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue % right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue % right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue % right.intValue);
                        case VarType.INT64: return (uint)left.intValue % right.longValue;
                        case VarType.UINT64: return (uint)left.intValue % (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue % right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue % right.intValue;
                        case VarType.UINT32: return left.longValue % (uint)right.intValue;
                        case VarType.INT64: return left.longValue % right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue % right.longValue;
                            else return (ulong)left.longValue % (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue % right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue % right.intValue;
                            else return (ulong)left.longValue % (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue % (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue % right.longValue;
                            else return (ulong)left.longValue % (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue % right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue % right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue % right.intValue;
                        case VarType.UINT32: return left.doubleValue % (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue % right.longValue;
                        case VarType.UINT64: return left.doubleValue % (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue % right.doubleValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator ++(Var value)
        {
            switch (value.type)
            {
                case VarType.INT32:
                    return value.intValue + 1;
                case VarType.UINT32:
                    return (uint)value.intValue + 1;
                case VarType.INT64:
                    return value.longValue + 1;
                case VarType.UINT64:
                    return (ulong)value.longValue + 1;
                case VarType.FLOAT:
                    return (float)value.doubleValue + 1;
                case VarType.DOUBLE:
                    return value.doubleValue + 1;
                default:
                    return Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator --(Var value)
        {
            switch (value.type)
            {
                case VarType.INT32:
                    return value.intValue - 1;
                case VarType.UINT32:
                    return (uint)value.intValue - 1;
                case VarType.INT64:
                    return value.longValue - 1;
                case VarType.UINT64:
                    return (ulong)value.longValue - 1;
                case VarType.FLOAT:
                    return (float)value.doubleValue - 1;
                case VarType.DOUBLE:
                    return value.doubleValue - 1;
                default:
                    return Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">可变变量</param>
        public static Var operator ~(Var value)
        {
            switch (value.type)
            {
                case VarType.INT32: return ~value.intValue;
                case VarType.UINT32: return (uint)~value.intValue;
                case VarType.INT64: return ~value.longValue;
                case VarType.UINT64: return (ulong)~value.longValue;
                default: return Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator &(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue & right.intValue;
                        case VarType.UINT32: return left.intValue & (uint)right.intValue;
                        case VarType.INT64: return left.intValue & right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return left.intValue & right.longValue;
                            else return (uint)left.intValue & (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue & right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue & right.intValue);
                        case VarType.INT64: return (uint)left.intValue & right.longValue;
                        case VarType.UINT64: return (uint)left.intValue & (ulong)right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue & right.intValue;
                        case VarType.UINT32: return left.longValue & (uint)right.intValue;
                        case VarType.INT64: return left.longValue & right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue & right.longValue;
                            else return (ulong)left.longValue & (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue & right.intValue;
                            else return (ulong)left.longValue & (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue & (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue & right.longValue;
                            else return (ulong)left.longValue & (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue & right.longValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator |(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue | right.intValue;
                        case VarType.UINT32: return (long)left.intValue | (uint)right.intValue;
                        case VarType.INT64: return (long)left.intValue | right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return (long)left.intValue | right.longValue;
                            else return (uint)left.intValue | (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue | (long)right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue | right.intValue);
                        case VarType.INT64: return (uint)left.intValue | right.longValue;
                        case VarType.UINT64: return (uint)left.intValue | (ulong)right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue | (long)right.intValue;
                        case VarType.UINT32: return left.longValue | (uint)right.intValue;
                        case VarType.INT64: return left.longValue | right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue | right.longValue;
                            else return (ulong)left.longValue | (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue | (long)right.intValue;
                            else return (ulong)left.longValue | (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue | (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue | right.longValue;
                            else return (ulong)left.longValue | (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue | right.longValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static Var operator ^(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue ^ right.intValue;
                        case VarType.UINT32: return left.intValue ^ (uint)right.intValue;
                        case VarType.INT64: return left.intValue ^ right.longValue;
                        case VarType.UINT64:
                            if (left.intValue < 0) return left.intValue ^ right.longValue;
                            else return (uint)left.intValue ^ (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue ^ right.intValue;
                        case VarType.UINT32: return (uint)(left.intValue ^ right.intValue);
                        case VarType.INT64: return (uint)left.intValue ^ right.longValue;
                        case VarType.UINT64: return (uint)left.intValue ^ (ulong)right.longValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue ^ right.intValue;
                        case VarType.UINT32: return left.longValue ^ (uint)right.intValue;
                        case VarType.INT64: return left.longValue ^ right.longValue;
                        case VarType.UINT64:
                            if (left.longValue < 0) return left.longValue ^ right.longValue;
                            else return (ulong)left.longValue ^ (ulong)right.longValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32:
                            if (right.intValue < 0) return left.longValue ^ right.intValue;
                            else return (ulong)left.longValue ^ (uint)right.intValue;
                        case VarType.UINT32: return (ulong)left.longValue ^ (uint)right.intValue;
                        case VarType.INT64:
                            if (right.longValue < 0) return left.longValue ^ right.longValue;
                            else return (ulong)left.longValue ^ (ulong)right.longValue;
                        case VarType.UINT64: return left.longValue ^ right.longValue;
                    }
                    break;
            }
            return Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">可变变量</param>
        /// <param name="right">位移量</param>
        public static Var operator <<(Var left, int right)
        {
            switch (left.type)
            {
                case VarType.INT32: return left.intValue << right;
                case VarType.UINT32: return (uint)left.intValue << right;
                case VarType.INT64: return left.longValue << right;
                case VarType.UINT64: return (ulong)left.longValue << right;
                default: return Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">可变变量</param>
        /// <param name="right">位移量</param>
        public static Var operator >>(Var left, int right)
        {
            switch (left.type)
            {
                case VarType.INT32: return left.intValue >> right;
                case VarType.UINT32: return (uint)left.intValue >> right;
                case VarType.INT64: return left.longValue >> right;
                case VarType.UINT64: return (ulong)left.longValue >> right;
                default: return Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator <(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue < right.intValue;
                        case VarType.UINT32: return left.intValue < (uint)right.intValue;
                        case VarType.INT64: return left.intValue < right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.intValue < right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue < right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue < right.intValue;
                        case VarType.UINT32: return left.intValue < right.intValue;
                        case VarType.INT64: return (uint)left.intValue < right.longValue;
                        case VarType.UINT64: return left.intValue < right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue < right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue < right.intValue;
                        case VarType.UINT32: return left.longValue < (uint)right.intValue;
                        case VarType.INT64: return left.longValue < right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.longValue < right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue < right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= 0 && left.longValue < right.intValue;
                        case VarType.UINT32: return left.longValue < right.intValue;
                        case VarType.INT64: return left.longValue >= 0 && left.longValue < right.longValue;
                        case VarType.UINT64: return left.longValue < right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue < right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue < right.intValue;
                        case VarType.UINT32: return left.doubleValue < (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue < right.longValue;
                        case VarType.UINT64: return left.doubleValue < (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue < right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator >(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue > right.intValue;
                        case VarType.UINT32: return left.intValue > (uint)right.intValue;
                        case VarType.INT64: return left.intValue > right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.intValue > right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue > right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue > right.intValue;
                        case VarType.UINT32: return left.intValue > right.intValue;
                        case VarType.INT64: return (uint)left.intValue > right.longValue;
                        case VarType.UINT64: return left.intValue > right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue > right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue > right.intValue;
                        case VarType.UINT32: return left.longValue > (uint)right.intValue;
                        case VarType.INT64: return left.longValue > right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.longValue > right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue > right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= 0 && left.longValue > right.intValue;
                        case VarType.UINT32: return left.longValue > right.intValue;
                        case VarType.INT64: return left.longValue >= 0 && left.longValue > right.longValue;
                        case VarType.UINT64: return left.longValue > right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue > right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue > right.intValue;
                        case VarType.UINT32: return left.doubleValue > (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue > right.longValue;
                        case VarType.UINT64: return left.doubleValue > (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue > right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator <=(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue <= right.intValue;
                        case VarType.UINT32: return left.intValue <= (uint)right.intValue;
                        case VarType.INT64: return left.intValue <= right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.intValue <= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue <= right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue <= right.intValue;
                        case VarType.UINT32: return left.intValue <= right.intValue;
                        case VarType.INT64: return (uint)left.intValue <= right.longValue;
                        case VarType.UINT64: return left.intValue <= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue <= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue <= right.intValue;
                        case VarType.UINT32: return left.longValue <= (uint)right.intValue;
                        case VarType.INT64: return left.longValue <= right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.longValue <= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue <= right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= 0 && left.longValue <= right.intValue;
                        case VarType.UINT32: return left.longValue <= right.intValue;
                        case VarType.INT64: return left.longValue >= 0 && left.longValue <= right.longValue;
                        case VarType.UINT64: return left.longValue <= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue <= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue <= right.intValue;
                        case VarType.UINT32: return left.doubleValue <= (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue <= right.longValue;
                        case VarType.UINT64: return left.doubleValue <= (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue <= right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator >=(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue >= right.intValue;
                        case VarType.UINT32: return left.intValue >= (uint)right.intValue;
                        case VarType.INT64: return left.intValue >= right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.intValue >= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue >= right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue >= right.intValue;
                        case VarType.UINT32: return left.intValue >= right.intValue;
                        case VarType.INT64: return (uint)left.intValue >= right.longValue;
                        case VarType.UINT64: return left.intValue >= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue >= right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= right.intValue;
                        case VarType.UINT32: return left.longValue >= (uint)right.intValue;
                        case VarType.INT64: return left.longValue >= right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.longValue >= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue >= right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= 0 && left.longValue >= right.intValue;
                        case VarType.UINT32: return left.longValue >= right.intValue;
                        case VarType.INT64: return left.longValue >= 0 && left.longValue >= right.longValue;
                        case VarType.UINT64: return left.longValue >= right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue >= right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue >= right.intValue;
                        case VarType.UINT32: return left.doubleValue >= (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue >= right.longValue;
                        case VarType.UINT64: return left.doubleValue >= (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue >= right.doubleValue;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(Var left, Var right)
        {
            switch (left.type)
            {
                case VarType.INT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.intValue == right.intValue;
                        case VarType.UINT32: return left.intValue == (uint)right.intValue;
                        case VarType.INT64: return left.intValue == right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.intValue == right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.intValue == right.doubleValue;
                    }
                    break;
                case VarType.UINT32:
                    switch (right.type)
                    {
                        case VarType.INT32: return (uint)left.intValue == right.intValue;
                        case VarType.UINT32: return left.intValue == right.intValue;
                        case VarType.INT64: return (uint)left.intValue == right.longValue;
                        case VarType.UINT64: return left.intValue == right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return (uint)left.intValue == right.doubleValue;
                    }
                    break;
                case VarType.INT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue == right.intValue;
                        case VarType.UINT32: return left.longValue == (uint)right.intValue;
                        case VarType.INT64: return left.longValue == right.longValue;
                        case VarType.UINT64: return right.longValue >= 0 && left.longValue == right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue == right.doubleValue;
                    }
                    break;
                case VarType.UINT64:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.longValue >= 0 && left.longValue == right.intValue;
                        case VarType.UINT32: return left.longValue == right.intValue;
                        case VarType.INT64: return left.longValue >= 0 && left.longValue == right.longValue;
                        case VarType.UINT64: return left.longValue == right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.longValue == right.doubleValue;
                    }
                    break;
                case VarType.FLOAT:
                case VarType.DOUBLE:
                    switch (right.type)
                    {
                        case VarType.INT32: return left.doubleValue == right.intValue;
                        case VarType.UINT32: return left.doubleValue == (uint)right.intValue;
                        case VarType.INT64: return left.doubleValue == right.longValue;
                        case VarType.UINT64: return left.doubleValue == (ulong)right.longValue;
                        case VarType.FLOAT:
                        case VarType.DOUBLE: return left.doubleValue == right.doubleValue;
                    }
                    break;
                case VarType.BOOL:
                    if (right.type == VarType.BOOL)
                        return left.intValue == right.intValue;
                    else
                        return false;
                case VarType.STRING:
                    if (right.type == VarType.STRING)
                        return left.stringValue == right.stringValue;
                    else
                        return false;
                case VarType.OBJECT:
                    if (right.type == VarType.OBJECT)
                        return left.objectValue == right.objectValue;
                    else
                        return false;
                case VarType.VARLIST:
                    if (right.type == VarType.VARLIST)
                        return left.listValue == right.listValue;
                    else
                        return false;
                case VarType.VARMAP:
                    if (right.type == VarType.VARMAP)
                        return left.mapValue == right.mapValue;
                    else
                        return false;
                case VarType.NULL:
                    if (right.type == VarType.NULL)
                        return true;
                    else
                        return false;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(Var left, Var right)
        {
            return !(left == right);
        }

    }
}
