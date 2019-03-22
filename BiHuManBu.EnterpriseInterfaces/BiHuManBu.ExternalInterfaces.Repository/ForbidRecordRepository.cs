using System;
using System.Data.Entity.Migrations;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ForbidRecordRepository : IForbidRecordRepository
    {
        private ILog logError;
        public ForbidRecordRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }
        public int AddForbidRecord(bx_forbid_record model)
        {
            int result = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_forbid_record.AddOrUpdate(model);
                result = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
    }
}
