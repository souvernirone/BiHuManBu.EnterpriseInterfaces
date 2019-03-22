using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using MySql.Data.MySqlClient;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class EnterpriseAgentRepository:IEnterpriseAgentRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public bx_agent_rate FindQudaoRate(int qudaoId,int agentId)
        {
            bx_agent_rate agetRate = new bx_agent_rate();
            try
            {
                agetRate = DataContextFactory.GetDataContext().bx_agent_rate.FirstOrDefault(x => x.qudao_id == qudaoId && x.agent_id==agentId);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agetRate;
        }

        public bx_hebaodianwei FindBuidRate(int qudaoId, int agentId,long buid,int? source)
        {
            bx_hebaodianwei agetRate = new bx_hebaodianwei();
            try
            {
                agetRate = DataContextFactory.GetDataContext().bx_hebaodianwei.FirstOrDefault(x => x.channel_id == qudaoId && x.agent_id == agentId && x.buid == buid&&x.source==source);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agetRate;
        }

        public bx_userinfo FindUserinfo(Int64 buid)
        {
            bx_userinfo userinfo = new bx_userinfo();
            try
            {
                userinfo = DataContextFactory.GetDataContext().bx_userinfo.FirstOrDefault(x => x.Id == buid);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return userinfo;
        }
        public IEnumerable<bx_userinfo> FindUserinfos(string buid)
        {
           List<bx_userinfo>  userinfo = new List<bx_userinfo>(); 
            try
            {
                string[] partnoArr = buid.Split(',');

                var listLongBuid =new List<long>();
                for (int i = 0; i < partnoArr.Length; i++)
                {
                    listLongBuid.Add(long.Parse(partnoArr[i]));
                }
                userinfo = DataContextFactory.GetDataContext().bx_userinfo.Where(x => listLongBuid.Contains(x.Id)).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return userinfo;
        }
        public int GetTopAgentId(int agentId)
        {
            MySqlParameter[] parms = new MySqlParameter[1];
            MySqlParameter parentAgentid = new MySqlParameter("agentId", agentId);
            parentAgentid.MySqlDbType = MySqlDbType.Int32;
            parentAgentid.Size = 128;
            parms[0] = parentAgentid;
            var ts = DataContextFactory.GetDataContext().Database.SqlQuery<int>("select `getAgentTopParent`(@agentId)", parms).FirstOrDefault();
            return ts;
        }

        public List<bx_agent_special_rate> FindQudaoSpecialRate(int qudaoId,int agentId)
        {
            List<bx_agent_special_rate> bxAgentSpecialRates = new List<bx_agent_special_rate>();
            try
            {
                bxAgentSpecialRates =
                    DataContextFactory.GetDataContext().bx_agent_special_rate.Where(c => c.agent_id == agentId && c.qudao_id == qudaoId).ToList();
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxAgentSpecialRates;
        }
        public bx_agent_rate FindAgentRate( int agentId,int source)
        {
            bx_agent_rate agetRate = new bx_agent_rate();
            try
            {
                agetRate = DataContextFactory.GetDataContext().bx_agent_rate.FirstOrDefault(x => x.agent_id == agentId && (x.is_qudao == 0 || x.is_qudao == null) && x.company_id == source);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agetRate;
        }

        public List<bx_agent_special_rate> FindBxAgentSpecialRate(int agentId)
        {
            List<bx_agent_special_rate> bxAgentSpecialRates=new List<bx_agent_special_rate>();
            try
            {
                bxAgentSpecialRates =
                    DataContextFactory.GetDataContext().bx_agent_special_rate.Where(c => c.agent_id == agentId && c.is_qudao == 0).ToList();
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxAgentSpecialRates;
        }

        public bx_submit_info FindBxSubmitInfo(long buid, int source)
        {
            bx_submit_info submitInfo=new bx_submit_info();
            try
            {
                submitInfo = DataContextFactory.GetDataContext().bx_submit_info.FirstOrDefault(c => c.b_uid == buid && c.source == source);
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return submitInfo;
        }
        public int AddHeBaoDianWei(bx_hebaodianwei hebaodianwei)
        {
            int  count = 0;
            try
            {
                //logInfo.Info(" insert agent_budian：" + hebaodianwei.agent_budian + ",agent_budian" );
                bx_hebaodianwei tt = DataContextFactory.GetDataContext().bx_hebaodianwei.Add(hebaodianwei);
             count= DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return count;
        }

        public int UpdateHeBaoDianWei(bx_hebaodianwei hebaodianwei)
        {
            int count = 0;
            try
            {
                //logInfo.Info("Update agent_budian：" + hebaodianwei.agent_budian + ",agent_budian");
                DataContextFactory.GetDataContext().bx_hebaodianwei.AddOrUpdate(hebaodianwei);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public bx_hebaodianwei FindBxHebaodianwei(long buid,int source)
        {
            bx_hebaodianwei hebaodianwei=new bx_hebaodianwei();
            try
            {
                hebaodianwei = DataContextFactory.GetDataContext().bx_hebaodianwei.FirstOrDefault(c => c.buid == buid && c.source == source);
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return hebaodianwei;
        }


        public List<bx_submit_info> FindBxSubmitInfos(long buid)
        {
            List<bx_submit_info> submitInfos=new List<bx_submit_info>();
            try
            {
                submitInfos = DataContextFactory.GetDataContext().bx_submit_info.Where(x => x.b_uid == buid).ToList();
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return submitInfos;
        }


        public int AddOrUpdateSmsAccountContent(bx_sms_account_content bxSmsAccountContent)
        {
            try
            {
                int count = 0;
                DataContextFactory.GetDataContext().bx_sms_account_content.AddOrUpdate(bxSmsAccountContent);
                count = DataContextFactory.GetDataContext().SaveChanges();
                return count;
            }
            catch (Exception ex )
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return -1;
        }
        public int UpdateBxCarOrderStatus(bx_car_order bxCarOrder)
        {
            try
            {
                int count = 0;
                DataContextFactory.GetDataContext().bx_car_order.AddOrUpdate(bxCarOrder);
                count = DataContextFactory.GetDataContext().SaveChanges();
                return count;
            }
            catch (Exception ex)
            {

                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return -1;
        }

        public bx_car_order FindBxCarOrder(long bxCarOrderId)
        {
            try
            {
                bx_car_order bxCarOrder = new bx_car_order();
                try
                {
                    bxCarOrder = DataContextFactory.GetDataContext().bx_car_order.FirstOrDefault(x => x.id == bxCarOrderId);
                }
                catch (Exception ex)
                {
                    logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                return bxCarOrder;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<bx_quoteresult> BxQuoteresults(long buid)
        {
            List<bx_quoteresult> bxQuoteresults = new List<bx_quoteresult>();
            try
            {
                bxQuoteresults = DataContextFactory.GetDataContext().bx_quoteresult.Where(x => x.B_Uid == buid).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return bxQuoteresults;
        }

        public List<bx_submit_info> BxSubmitInfos(long buid)
        {
            List<bx_submit_info> bxSubmitInfos = new List<bx_submit_info>();
            try
            {
                bxSubmitInfos = DataContextFactory.GetDataContext().bx_submit_info.Where(x => x.b_uid == buid).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return bxSubmitInfos;
        }

        public List<bx_quoteresult> BxQuoteresultList(string buids)
        {
            List<bx_quoteresult> bxQuoteresults = new List<bx_quoteresult>();
            try
            {
                string[] partnoArr = buids.Split(',');
                long?[] a = new long?[partnoArr.Length];
                for (int i = 0; i < partnoArr.Length; i++)
                {
                    //先由字符串转换成char,再转换成String,然后Integer
                    a[i] = int.Parse(partnoArr[i]);
                }
                bxQuoteresults = DataContextFactory.GetDataContext().bx_quoteresult.Where(x => a.Contains(x.B_Uid)).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return bxQuoteresults;
        }
     
        public List<bx_submit_info> BxSubmitInfosList(string buids)
        {
            List<bx_submit_info> bxSubmitInfos = new List<bx_submit_info>();
            try
            {
                string[] partnoArr = buids.Split(',');
                long?[] a = new long?[partnoArr.Length];
                for (int i = 0; i < partnoArr.Length; i++)
                {
                    //先由字符串转换成char,再转换成String,然后Integer
                    a[i] = long.Parse(partnoArr[i]);
                }
                bxSubmitInfos = DataContextFactory.GetDataContext().bx_submit_info.Where(x => a.Contains(x.b_uid)).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw;
            }
            return bxSubmitInfos;
        }
    }
}
