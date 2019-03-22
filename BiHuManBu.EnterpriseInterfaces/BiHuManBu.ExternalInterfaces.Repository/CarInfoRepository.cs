using System;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public  class CarInfoRepository: ICarInfoRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public bx_carinfo Find(string licenseno)
        {
            bx_carinfo carinfo = new bx_carinfo();
            try
            {
                carinfo=DataContextFactory.GetDataContext().bx_carinfo.FirstOrDefault(x => x.license_no == licenseno);
            }
            catch (Exception ex)
            {
                 logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return carinfo;
        }
    }
}
