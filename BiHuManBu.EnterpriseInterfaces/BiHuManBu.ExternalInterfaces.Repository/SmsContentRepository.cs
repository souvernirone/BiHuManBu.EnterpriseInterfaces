using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Linq;
using System.Data.Entity;
using log4net;
using System.Data.Entity.Migrations;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SmsContentRepository : EfRepositoryBase<bx_sms_account>, ISmsContentRepository
    {
        private ILog logError;

        public SmsContentRepository(DbContext context) : base(context)
        {
            logError = LogManager.GetLogger("ERROR");
        }

        public bx_sms_account Find(int agent)
        {
            bx_sms_account smsAccount = new bx_sms_account();
            try
            {
                smsAccount = Table.FirstOrDefault(x => x.agent_id == agent);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return smsAccount;
        }

        public int Add(bx_sms_account bxSmsAccount)
        {
            int smsAccountId = 0;
            try
            {
                var t = Table.Add(bxSmsAccount);
                SaveChanges();
                smsAccountId = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                smsAccountId = 0;
            }
            return smsAccountId;
        }

        public int Update(bx_sms_account bxSmsAccount)
        {
            int count = 0;
            try
            {
                Table.AddOrUpdate(bxSmsAccount);
                count = SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

    }
}