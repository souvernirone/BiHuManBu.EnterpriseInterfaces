using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SaveQuoteRepository : ISaveQuoteRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public bx_savequote GetSavequoteByBuid(long buid)
        {
            bx_savequote savequote = new bx_savequote();
            try
            {
                savequote = DataContextFactory.GetDataContext().bx_savequote.FirstOrDefault(x => x.B_Uid == buid);
            }
            catch (Exception ex)
            {

                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return savequote;
        }
        /// <summary>
        /// 批量获取报价请求
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<bx_savequote> GetSavequotes(string buids)
        {
            var savequotes = new List<bx_savequote>();
            try
            {
                string sqlList = string.Format("select * from bx_savequote where b_uid in ({0})", buids);
                savequotes = DataContextFactory.GetDataContext().Database.SqlQuery<bx_savequote>(sqlList).ToList();
                return savequotes;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return savequotes;
        }
        public long Add(bx_savequote savequote)
        {
            long count = 0;
            try
            {
                var item = DataContextFactory.GetDataContext().bx_savequote.Add(savequote);
                var returnResult = DataContextFactory.GetDataContext().SaveChanges();
                count = item.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return count;
        }
        public int Update(bx_savequote savequote)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_savequote.AddOrUpdate(savequote);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
    }
}
