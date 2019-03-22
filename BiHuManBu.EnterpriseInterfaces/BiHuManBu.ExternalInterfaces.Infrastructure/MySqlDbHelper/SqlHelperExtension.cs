using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BiHuManBu.StoreFront.Infrastructure.DbHelper
{
    /// <summary>
    /// SqlHelper扩展(依赖AutoMapper.dll)
    /// </summary>
    public sealed partial class SqlHelper
    {

        #region 实例方法

        public T ExecuteObject<T>(string commandText, params SqlParameter[] parms)
        {
            return ExecuteObject<T>(this.ConnectionString, commandText, parms);
        }

        public List<T> ExecuteObjects<T>(string commandText, params SqlParameter[] parms)
        {
            return ExecuteObjects<T>(this.ConnectionString, commandText, parms);
        }

        #endregion

        #region 静态方法

        public static T ExecuteObject<T>(string connectionString, string commandText, params SqlParameter[] parms)
        {
            //DataTable dt = ExecuteDataTable(connectionString, commandText, parms);
            //return AutoMapper.Mapper.DynamicMap<List<T>>(dt.CreateDataReader()).FirstOrDefault();
            using (SqlDataReader reader = ExecuteDataReader(connectionString, commandText, parms))
            {
                return AutoMapper.Mapper.DynamicMap<List<T>>(reader).FirstOrDefault();
            }
        }

        public static List<T> ExecuteObjects<T>(string connectionString, string commandText, params SqlParameter[] parms)
        {
            //DataTable dt = ExecuteDataTable(connectionString, commandText, parms);
            //return AutoMapper.Mapper.DynamicMap<List<T>>(dt.CreateDataReader());
            using (SqlDataReader reader = ExecuteDataReader(connectionString, commandText, parms))
            {
                return AutoMapper.Mapper.DynamicMap<List<T>>(reader);
            }
        }

        #endregion
    }

}