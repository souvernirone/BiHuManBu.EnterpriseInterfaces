using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using log4net;
using System.Collections.Generic;
using System.Data;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuoteHistoryRepository : IQuoteHistoryRepository
    {
        //报价历史 库连接串
        //private static readonly string DBQuoteHistory = ApplicationSettingsFactory.GetApplicationSettings().QuoteHistoryConfigString;
        //private readonly MySqlHelper _helpQuote = new MySqlHelper(DBQuoteHistory);

        //业务主 库连接串
        private static readonly string DBMain = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _helpMain = new MySqlHelper(DBMain);
        private ILog logError;

        private const string QUOTE_HISTORY_BYBUID = @"select * from bihustatistics.bx_quote_history  where b_uid = {0}  ";
        private const string QUOTE_HISTORY_BYAGENT = @"select * from bihustatistics.bx_quote_history where b_uid in ({0}) and  groupspan IN ({1})  ";
        private const string QUOTE_HISTORY_BYAGENTCOUNT = @" SELECT COUNT(1) FROM (   SELECT groupspan  FROM bihustatistics.bx_quote_history  WHERE b_uid in ({0}) and  groupspan IN ({1})   GROUP BY groupspan) AS  temp ";
        public QuoteHistoryRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }
        public List<QuoteHistoryModel> GetByBuid(long Buid)
        {
            var sql = string.Format(QUOTE_HISTORY_BYBUID + "order by createtime desc", Buid);
            return _helpMain.ExecuteDataTable(CommandType.Text, sql).ToList<QuoteHistoryModel>().ToList();
        }
        public List<QuoteHistoryModel> GetByLots(List<long> lots, List<long> buids, out int count)
        {
            var sqlCount = string.Format(QUOTE_HISTORY_BYAGENTCOUNT, string.Join(",", buids), string.Join(",", lots));
            count = _helpMain.ExecuteScalar<int>(sqlCount);
            var sql = string.Format(QUOTE_HISTORY_BYAGENT, string.Join(",", buids), string.Join(",", lots));
            return _helpMain.ExecuteDataTable(CommandType.Text, sql).ToList<QuoteHistoryModel>().ToList();
        }

        /// <summary>
        /// 报价历史库 获取报价数量
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        //public string GetQuoteHistoryCount(long buid)
        //{
        //    string createtime = string.Empty;
        //    try
        //    {
        //        string sqlCount = string.Format("SELECT createtime FROM bx_quote_history WHERE b_uid={0} and quotestatus>0 order by createtime desc limit 1", buid);
        //        DateTime dt = _helpQuote.ExecuteScalar<DateTime>(sqlCount);
        //        if (dt.Date == DateTime.MinValue)
        //        {
        //            return "";
        //        }
        //        return dt.ToString("yyyy-MM-dd HH:mm:ss");
        //    }
        //    catch (Exception ex)
        //    {
        //        logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //    }
        //    return createtime;
        //}

        /// <summary>
        /// 业务主库 获取报价数量
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        //public string GetMainQuoteHistoryCount(long buid)
        //{
        //    string createtime = string.Empty;
        //    try
        //    {
        //        string sqlCount = string.Format("SELECT createtime FROM bx_quote_history WHERE b_uid={0} and quotestatus>0 order by createtime desc limit 1", buid);
        //        DateTime dt = _helpMain.ExecuteScalar<DateTime>(sqlCount);
        //        if (dt.Date == DateTime.MinValue)
        //        {
        //            return "";
        //        }
        //        return dt.ToString("yyyy-MM-dd HH:mm:ss");
        //    }
        //    catch (Exception ex)
        //    {
        //        logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //    }
        //    return createtime;
        //}
    }
}
