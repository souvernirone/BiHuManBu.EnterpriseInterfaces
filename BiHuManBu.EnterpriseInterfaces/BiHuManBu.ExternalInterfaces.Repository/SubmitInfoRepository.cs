using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SubmitInfoRepository : ISubmitInfoRepository
    {

        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public SubmitInfoRepository()
        {

        }

        public bx_submit_info GetSubmitInfo(long buid, int source)
        {
            bx_submit_info submitInfo = new bx_submit_info();
            try
            {
                submitInfo = DataContextFactory.GetDataContext().bx_submit_info.FirstOrDefault(x => x.b_uid == buid && x.source == source);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return submitInfo;
        }

        public async Task<bx_submit_info> GetSubmitInfoAsync(long buid, int source)
        {
            bx_submit_info submitInfo = new bx_submit_info();
            try
            {
                submitInfo = DataContextFactory.GetDataContext().bx_submit_info.FirstOrDefault(x => x.b_uid == buid && x.source == source);
                var sql =
                    String.Format(
                        @"SELECT * FROM bx_submit_info WHERE b_uid = {0} AND source = {1} ORDER BY update_time DESC LIMIT 0,1",
                        buid, source);
                submitInfo =
                    DataContextFactory.GetDataContext().Database.SqlQuery<bx_submit_info>(sql).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return submitInfo;
        }

        /// <summary>
        /// 获取核保列表
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public List<bx_submit_info> GetSubmitInfoList(long buid)
        {
            var list = new List<bx_submit_info>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_submit_info.Where(x => x.b_uid == buid).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        /// <summary>
        /// 批量获取核保结果
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<bx_submit_info> GetSubmitInfos(string buids)
        {
            var list = new List<bx_submit_info>();
            try
            {
                string sqlList = string.Format("select * from bx_submit_info where b_uid in ({0})", buids);
                list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_submit_info>(sqlList).ToList();
                return list;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public bool HasSubmitInfo(long buid)
        {
            try
            {
                var model = from si in DataContextFactory.GetDataContext().bx_submit_info
                            where si.b_uid == buid
                            select 1;
                return model.Count() > 0;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
    }
}
