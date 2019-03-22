using System;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class HeBaoDianWeiRepository : IHeBaoDianWeiRepository
    {
        private readonly ILog _logError;
        public HeBaoDianWeiRepository()
        {
            _logError = LogManager.GetLogger("ERROR");
        }

        public bx_hebaodianwei GetHeBao(long buid, int source)
        {
            var item = new bx_hebaodianwei();
            try
            {
                item = DataContextFactory.GetDataContext().bx_hebaodianwei.FirstOrDefault(x => x.buid == buid && x.source == source);
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }
    }
}
