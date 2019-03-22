using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Data.Entity;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuoteReqCarInfoRepository : EfRepositoryBase<bx_quotereq_carinfo>, IQuoteReqCarInfoRepository
    {
        public QuoteReqCarInfoRepository(DbContext context)
            : base(context)
        {
        }
        public bx_quotereq_carinfo Add(bx_quotereq_carinfo item)
        {
            bx_quotereq_carinfo model = new bx_quotereq_carinfo();
            try
            {
                model = DataContextFactory.GetDataContext().bx_quotereq_carinfo.Add(item);
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                ILog logError = LogManager.GetLogger("ERROR");
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return model;
        }

        public int Update(bx_quotereq_carinfo item)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_quotereq_carinfo.AddOrUpdate(item);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                ILog logError = LogManager.GetLogger("ERROR");
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public List<IsNewCarViewModel> GetIsNewCarList(List<long> buid)
        {
            var query = from quote in DataContextFactory.GetDataContext().bx_quotereq_carinfo
                        where buid.Contains(quote.b_uid)
                        select new IsNewCarViewModel
                        {
                            Buid = quote.b_uid,
                            IsNewCar = quote.is_newcar
                        };
            return query.ToList();
        }

        /// <summary>
        /// 根据buid查询新车
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public List<IsNewCarViewModel> GetIsNewCar(List<long> buid)
        {
            string sql = "select b_uid,is_newcar from bx_quotereq_carinfo where b_uid in (" + string.Join(",", buid) + ")";
            return DataContextFactory.GetDataContext().Database.SqlQuery<IsNewCarViewModel>(sql).ToList();
        }
        /// <summary>
        /// 根据buid获取req对象
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bx_quotereq_carinfo Find(long buid)
        {
            return DataContextFactory.GetDataContext().bx_quotereq_carinfo.FirstOrDefault(x => x.b_uid == buid);
        }

        public async Task<bx_quotereq_carinfo> FindAsync(long buid)
        {
            var sql = String.Format(@"SELECT * FROM bx_quotereq_carinfo WHERE b_uid = {0} ORDER BY update_time DESC LIMIT 0,1", buid);
            bx_quotereq_carinfo model =
                DataContextFactory.GetDataContext().Database.SqlQuery<bx_quotereq_carinfo>(sql).ToList().FirstOrDefault();
            return model;
            
        }
    }
}
