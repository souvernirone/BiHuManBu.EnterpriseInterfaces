using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRefreshRenewalService
{
    public class MySqlDBHelper
    {
        private static ILog logInfo = LogManager.GetLogger("INFO");
        private static ILog logError = LogManager.GetLogger("ERROR");
        private static readonly string conn = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        private static MySqlConnection mySqlConnection = null;
        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        public static void MySqlConnInit()
        {
             mySqlConnection = new MySqlConnection(conn);
        }
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public static void MySqlConnOpen()
        {
            if (mySqlConnection.State != ConnectionState.Open)
            {
                mySqlConnection.Open();
            }
        }        
        public static void MySqlConnClose()
        {
            if (mySqlConnection != null&&mySqlConnection.State== ConnectionState.Open)
            {
                mySqlConnection.Close();//关闭
                mySqlConnection.Dispose();//释放
            }
        }
        public static void MySqlConnDispose()
        {
            if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
            {
                mySqlConnection.Dispose();
            }
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public static int CommandTimeOut = 600;

        #region 执行SQL语句,返回结果集中的第一个数据表

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public static DataTable ExecuteDataTable(string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(CommandType.Text, commandText, parms).Tables[0];
        }
        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public static DataSet ExecuteDataSet(CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(mySqlConnection, commandType, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public static DataSet ExecuteDataSet(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteDataSet(connection, null, commandType, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        private static DataSet ExecuteDataSet(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            var command = new MySqlCommand();

            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            var adapter = new MySqlDataAdapter(command);

            var ds = new DataSet();
            adapter.Fill(ds);
            if (commandText.IndexOf("@") > 0)
            {
                commandText = commandText.ToLower();
                var index = commandText.IndexOf("where ");
                if (index < 0)
                {
                    index = commandText.IndexOf("\nwhere");
                }
                ds.ExtendedProperties.Add("SQL", index > 0 ? commandText.Substring(0, index - 1) : commandText);
            }
            else
            {
                ds.ExtendedProperties.Add("SQL", commandText);  //将获取的语句保存在表的一个附属数组里，方便更新时生成CommandBuilder
            }

            foreach (DataTable dt in ds.Tables)
            {
                dt.ExtendedProperties.Add("SQL", ds.ExtendedProperties["SQL"]);
            }

            command.Parameters.Clear();
            return ds;
        } 
        #endregion

        #region 执行SQL语句,返回影响的行数
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(string commandText, params MySqlParameter[] parms)
        {
            int count = -1;

            count = ExecuteNonQuery(mySqlConnection, CommandType.Text, commandText, parms);

            return count;
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            return ExecuteNonQuery(connection, null, commandType, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        private static int ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] parms)
        {
            MySqlCommand command = new MySqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, parms);
            int retval = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return retval;
        }

        #endregion
        private static void PrepareCommand(MySqlCommand command, MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] parms)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            command.Connection = connection;
            command.CommandTimeout = CommandTimeOut;
            // 设置命令文本(存储过程名或SQL语句)
            command.CommandText = commandText;
            // 分配事务
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            // 设置命令类型.
            command.CommandType = commandType;
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value;
                foreach (MySqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
                command.Parameters.AddRange(parms);
            }
        }
    }
}
