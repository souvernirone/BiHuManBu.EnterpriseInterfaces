using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class UserClaimRepository : IUserClaimRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public List<bx_claim_detail> FindList(long buid)
        {
            List<bx_claim_detail> list = new List<bx_claim_detail>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_claim_detail.Where(x => x.b_uid == buid).ToList();

            }
            catch (Exception ex)
            {

                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
    }
}
