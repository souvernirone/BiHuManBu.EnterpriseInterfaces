using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class BatchrefreshrenewalRepository
    {
        private readonly EntityContext db = new EntityContext();


        /// <summary>
        /// 获取前20代理人的第一条数据
        /// </summary>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByAgentList()
        {
            //1.查询数据
            //string sql = "select id,a.agentid from ( select * from bx_batchrefreshrenewal where is_deleted=0 AND refrenewalstatus=6 order by updatetime ) a group by a.agentid limit 20 ;";
            //string sql = "select a.* from ( select * from bx_batchrefreshrenewal where is_deleted=0 AND refrenewalstatus=6 order by updatetime ) a group by a.operate_agent limit 20 ;";
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE is_deleted=0 AND refrenewalstatus=6 AND id IN (SELECT MIN(Id) FROM bx_batchrefreshrenewal where is_deleted=0 AND refrenewalstatus=6 GROUP BY operate_agent) LIMIT 20";
            List<bx_batchrefreshrenewal> list = db.Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            return list;
        }

        /// <summary>
        /// 获取排队中数据的时间最早的5条记录
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalLimit(int rows)
        {
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE is_deleted=0 AND refrenewalstatus=6 ORDER BY updatetime LIMIT " + rows;
            List<bx_batchrefreshrenewal> list = db.Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            return list;
        }

        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByTimes()
        {
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE sendtimes>refreshtimes and sendtimes>1 and refrenewalstatus<6 AND is_deleted=0 LIMIT 5";
            List<bx_batchrefreshrenewal> list = db.Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return list;
        }

        /// <summary>
        /// 更新数据我续保中
        /// </summary>
        /// <param name="list"></param>
        public void UpdateRefRenewalStatus(List<bx_batchrefreshrenewal> list)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            foreach (bx_batchrefreshrenewal item in list)
            {
                sqlBuilder.Append("UPDATE bx_batchrefreshrenewal SET refrenewalstatus=5 WHERE Id=" + item.id + ";");
            }
            db.Database.ExecuteSqlCommand(sqlBuilder.ToString());
        }


    }

    public class BatchRefreshRenewalConsumerRepository
    {
        private readonly EntityContext db = new EntityContext();
        /// <summary>
        /// 当启动程序后获取续保中数据重新续保
        /// </summary>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetInRenewalList()
        {
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE refrenewalstatus=5 AND is_deleted=0 limit 15";
            List<bx_batchrefreshrenewal> list = db.Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            return list;
        }
        /// <summary>
        /// 更新该表中续保状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateBatchRefreshRenewalStatus(bx_batchrefreshrenewal model)
        {
            bool result = false;
            //try
            //{
            if (model.id <= 0)
            {
                return result;
            }
            string sql = "UPDATE bx_batchrefreshrenewal SET refrenewalstatus=" + model.refrenewalstatus + ", refreshtimes=" + model.sendtimes;
            if (model.updatetime < DateTime.Now.Date)
            {
                sql += ",updatetime=CURDATE() ";
            }
            if (model.createtime < DateTime.Now.Date)
            {
                sql += " ,createtime=CURDATE() ";
            }
            sql += " where is_deleted=0 and id=" + model.id;

            return result = db.Database.ExecuteSqlCommand(sql) > 0;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("更新批量刷新续保状态异常：" + ex);
            //}
            //return result;
        }

        /// <summary>
        /// 判断是否已经刷新续保完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsCompleteRenewal(int id)
        {
            string tempsql = "";
            bool result = false;
            //try
            //{
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE is_deleted=0 AND id= " + id;
            tempsql = sql;
            bx_batchrefreshrenewal model = db.Database.SqlQuery<bx_batchrefreshrenewal>(sql).FirstOrDefault();
            if (model == null || model.id == 0)
            {
                return false;
            }
            if (model.refrenewalstatus < 5)
            {
                result = true;
            }
            return result;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("发生异常:" + tempsql + Environment.NewLine + ex);
            //}
            //return false;
        }



        /// <summary>
        /// 查询排队中的数据是否删除，如果删除则不更新续保状态
        /// 将数据更新为续保中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateRefRenewalStatusById(int id, int refrenewalstatus)
        {
            //try
            //{
            string sql = "UPDATE bx_batchrefreshrenewal SET refrenewalstatus=" + refrenewalstatus + " WHERE id=" + id + " AND is_deleted=0";
            return db.Database.ExecuteSqlCommand(sql) > 0;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("更新续保状态为【续保中】发生异常：" + ex);
            //}
            //return false;
        }

        public bx_userinfo GetUserModel(int buid)
        {
            //try
            //{
            string sql = "SELECT * FROM bx_userinfo WHERE id=" + buid;
            return db.Database.SqlQuery<bx_userinfo>(sql).FirstOrDefault();
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error("发生异常" + ex);
            //}
            //return new bx_userinfo();
        }

    }
}
