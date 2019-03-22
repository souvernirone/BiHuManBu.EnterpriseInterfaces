using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BatchRefreshRenewalService
{
    public class DbCommon
    {
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logError = LogManager.GetLogger("ERROR");
        private readonly string conn = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        //每个顶级代理人前5行数据
        private readonly int preRow = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PreRow"]);
        //前20个顶级代理人
        private readonly int preAgent = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PreAgent"]);
        /// <summary>
        /// 获取批量刷新续保表中的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetBatchRefreshRenewal()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT bx_userinfo.id AS Buid, LicenseNo, SixDigitsAfterIdCard, RenewalCarType, CarVIN, EngineNo, CityCode, OpenId, Agent AS AgentId, t.Id,t.UpdateTime,t.CreateTime,t.SendTimes,t.TopAgentId ");
            sqlBuilder.Append(" FROM bx_Userinfo ");
            sqlBuilder.Append(" INNER JOIN ");
            sqlBuilder.Append(" ( ");
            sqlBuilder.Append(" SELECT buid,id,createtime,updatetime,SendTimes,alldata.topagentid");
            sqlBuilder.Append(" FROM (");
            sqlBuilder.Append(" SELECT  topagentid,buid,id,createtime,updatetime,SendTimes");
            sqlBuilder.Append(" FROM ");
            sqlBuilder.Append(" (");
            sqlBuilder.Append(" SELECT  b.topagentid,b.buid,b.id ,b.createtime,b.updatetime, b.SendTimes,IF(@grp=b.topagentid,@rank:=@rank+1,@rank:=1) rank,@grp:=b.topagentid ");
            sqlBuilder.Append(" FROM   (SELECT  topagentid,buid,id ,createtime,updatetime,SendTimes FROM bx_batchrefreshrenewal WHERE refrenewalstatus >= 5 AND is_deleted = 0  ORDER BY  TopAgentId,createtime ) AS b,(SELECT  @grp:=0,@rank:=0) AS a ");
            sqlBuilder.Append(" ) AS ranked");
            sqlBuilder.Append(" WHERE rank<=" + preRow);
            sqlBuilder.Append(" ) AS alldata");
            sqlBuilder.Append(" INNER JOIN ");
            sqlBuilder.Append(" ( ");
            sqlBuilder.Append(" SELECT  TopAgentId FROM  bx_batchrefreshrenewal  WHERE refrenewalstatus >= 5 AND is_deleted = 0 GROUP BY  TopAgentId LIMIT " + preAgent);
            sqlBuilder.Append(" )AS toplists ");
            sqlBuilder.Append(" ON alldata.topagentid=toplists.TopAgentId ");
            sqlBuilder.Append(" ) AS t ");
            sqlBuilder.Append(" ON bx_userinfo.id=t.buid");
           
            return MySqlDBHelper.ExecuteDataTable( sqlBuilder.ToString());

        }
        /// <summary>
        /// 更新该表中续保状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateBatchRefreshRenewalStatus(DataRow row, int refrenewalstatus)
        {
            bool result = false;
            //try
            //{
            string sql = "UPDATE bx_batchrefreshrenewal SET refrenewalstatus=" + refrenewalstatus + ", refreshtimes=" + int.Parse(row["SendTimes"].ToString());
            if (DateTime.Parse(row["UpdateTime"].ToString()) < DateTime.Now.Date)
            {
                sql += ",updatetime=CURDATE() ";
            }
            if (DateTime.Parse(row["CreateTime"].ToString()) < DateTime.Now.Date)
            {
                sql += " ,createtime=CURDATE() ";
            }
            sql += " where is_deleted=0 and id=" + int.Parse(row["Id"].ToString());
            return result = MySqlDBHelper.ExecuteNonQuery( sql) > 0;
            //}
            //catch (Exception ex)
            //{
            //    logError.Error("更新续保状态异常：" + ex);
            //}
            //return result;
        }
        /// <summary>
        /// 多个SQL脚本更新数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool UpdateBatchRefreshRenewalStatusBySql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return false;
            }
            return MySqlDBHelper.ExecuteNonQuery( sql) > 0;
        }
        public bool UpdateRefRenewalStatusById(string ids, int refrenewalstatus)
        {
            //try
            //{
            string sql = "UPDATE bx_batchrefreshrenewal SET refrenewalstatus=" + refrenewalstatus + " WHERE id in(" + ids + ") AND is_deleted=0";
            return MySqlDBHelper.ExecuteNonQuery( sql) > 0;
            //}
            //catch (Exception ex)
            //{
            //    logError.Error("UpdateRefRenewalStatusById主键："+ids+";更新续保状态为【续保中】发生异常：" + ex);
            //}
            //return false;
        }
    }
}
