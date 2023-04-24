#if !UNITY_2020_1_OR_NEWER
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace ECSharp.Database.MySQL
{
    /// <summary>
    /// ES存储过程参数
    /// </summary>
    public static class Parameter
    {
        /// <summary>
        /// SQL过程函数参数 0
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, object value)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                Value = value
            };
        }

        /// <summary>
        /// SQL过程函数参数 1
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <param name="mySqlDbType">值类型</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, object value, MySqlDbType mySqlDbType)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                MySqlDbType = mySqlDbType
            };
        }

        /// <summary>
        /// SQL过程函数参数 2
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <param name="mySqlDbType">值类型</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, object value, MySqlDbType mySqlDbType, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                MySqlDbType = mySqlDbType,
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 3
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="mySqlDbType">值类型</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, MySqlDbType mySqlDbType, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                MySqlDbType = mySqlDbType,
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 4
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="mySqlDbType">值类型</param>
        /// <param name="size">值预设大小</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, MySqlDbType mySqlDbType, int size, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                MySqlDbType = mySqlDbType,
                Size = (size >= 0 ? size : 0),
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 5
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <param name="mySqlDbType">值类型</param>
        /// <param name="size">值预设大小</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static DbParameter Create(string parameterName, object value, MySqlDbType mySqlDbType, int size, ParameterDirection direction)
        {
            return new MySqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                MySqlDbType = mySqlDbType,
                Size = (size >= 0 ? size : 0),
                Direction = direction
            };
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DbParameter ToParameter(this string parameterName, object value)
        {
            return Create(parameterName, value);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="mySqlDbType"></param>
        /// <returns></returns>
        public static DbParameter ToParameter(this string parameterName, object value, MySqlDbType mySqlDbType)
        {
            return Create(parameterName, value, mySqlDbType);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="mySqlDbType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static DbParameter ToParameter(this string parameterName, object value, MySqlDbType mySqlDbType, ParameterDirection direction)
        {
            return Create(parameterName, value, mySqlDbType, direction);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="mySqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static DbParameter ToParameter(this string parameterName, MySqlDbType mySqlDbType, int size, ParameterDirection direction)
        {
            return Create(parameterName, mySqlDbType, size, direction);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="mySqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static DbParameter ToParameter(this string parameterName, object value, MySqlDbType mySqlDbType, int size, ParameterDirection direction)
        {
            return Create(parameterName, value, mySqlDbType, size, direction);
        }
    }
}

#endif