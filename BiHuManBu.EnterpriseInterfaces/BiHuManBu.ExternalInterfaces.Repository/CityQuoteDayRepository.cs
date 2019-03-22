using System;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CityQuoteDayRepository : EfRepositoryBase<bx_cityquoteday>, ICityQuoteDayRepository
    {
        private readonly ILog _logError = LogManager.GetLogger("ERROR");

        public CityQuoteDayRepository(DbContext context) : base(context)
        {
        }

        public int GetDaysNum(int cityId)
        {
            int daysNum = 0;
            try
            {
                bx_cityquoteday model = DataContextFactory.GetDataContext().bx_cityquoteday.FirstOrDefault(i => i.cityid == cityId);
                if (model != null)
                {
                    daysNum = model.quotedays.HasValue ? model.quotedays.Value : 90; //如果库中没值，默认90天
                }
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" +
                              ex.InnerException);
            }
            return daysNum;
        }

    }
}
