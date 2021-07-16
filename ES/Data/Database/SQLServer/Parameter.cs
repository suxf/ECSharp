using System.Data;
using System.Data.SqlClient;

namespace ES.Data.Database.SQLServer
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
        public static SqlParameter Create(string parameterName, object value)
        {
            return new SqlParameter
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
        /// <param name="sqlDbType">值类型</param>
        /// <returns>返回一个参数对象</returns>
        public static SqlParameter Create(string parameterName, object value, SqlDbType sqlDbType)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                SqlDbType = sqlDbType
            };
        }

        /// <summary>
        /// SQL过程函数参数 2
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <param name="sqlDbType">值类型</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static SqlParameter Create(string parameterName, object value, SqlDbType sqlDbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                SqlDbType = sqlDbType,
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 3
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="sqlDbType">值类型</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static SqlParameter Create(string parameterName, SqlDbType sqlDbType, ParameterDirection direction)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 4
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="sqlDbType">值类型</param>
        /// <param name="size">值预设大小</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static SqlParameter Create(string parameterName, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Size = (size >= 0 ? size : 0),
                Direction = direction
            };
        }

        /// <summary>
        /// SQL过程函数参数 5
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">值</param>
        /// <param name="sqlDbType">值类型</param>
        /// <param name="size">值预设大小</param>
        /// <param name="direction">方向</param>
        /// <returns>返回一个参数对象</returns>
        public static SqlParameter Create(string parameterName, object value, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                Value = value,
                SqlDbType = sqlDbType,
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
        public static SqlParameter ToParameter(this string parameterName, object value)
        {
            return Create(parameterName, value);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static SqlParameter ToParameter(this string parameterName, object value, SqlDbType sqlDbType)
        {
            return Create(parameterName, value, sqlDbType);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static SqlParameter ToParameter(this string parameterName, object value, SqlDbType sqlDbType, ParameterDirection direction)
        {
            return Create(parameterName, value, sqlDbType, direction);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static SqlParameter ToParameter(this string parameterName, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            return Create(parameterName, sqlDbType, size, direction);
        }

        /// <summary>
        /// 转换为SQL Parameter对象
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static SqlParameter ToParameter(this string parameterName, object value, SqlDbType sqlDbType, int size, ParameterDirection direction)
        {
            return Create(parameterName, value, sqlDbType, size, direction);
        }
    }
}
