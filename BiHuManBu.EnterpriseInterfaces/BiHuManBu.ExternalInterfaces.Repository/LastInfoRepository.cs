using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class LastInfoRepository : ILastInfoRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public bx_lastinfo GetByBuid(long buid)
        {
            bx_lastinfo lastinfo = new bx_lastinfo();
            try
            {
                lastinfo = DataContextFactory.GetDataContext().bx_lastinfo.FirstOrDefault(x => x.b_uid == buid);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return lastinfo;
        }

        /// <summary>
        /// 根据buid获取上一年商业险和交强险到期时间
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public InsuranceEndDate GetEndDate(long buid)
        {
            var model = new InsuranceEndDate();
            try
            {
                var list = from li in DataContextFactory.GetDataContext().bx_lastinfo
                           where li.b_uid == buid
                           select new InsuranceEndDate
                           {
                               LastBusinessEndDdate = li.last_business_end_date,
                               LastForceEndDdate = li.last_end_date
                           };
                model = list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }

        /// <summary>
        /// 批量获取Buids上一年商业险和交强险到期时间
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<InsuranceEndDateWithBuid> GetEndDates(string buids)
        {
            var model = new List<InsuranceEndDateWithBuid>();
            try
            {
                string sqlList =
                    string.Format("select b_uid as Buid,last_business_end_date as LastBusinessEndDdate,last_end_date as LastForceEndDdate from bx_lastinfo where b_uid in ({0})", buids);
                model = DataContextFactory.GetDataContext().Database.SqlQuery<InsuranceEndDateWithBuid>(sqlList).ToList();
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
    }
}
