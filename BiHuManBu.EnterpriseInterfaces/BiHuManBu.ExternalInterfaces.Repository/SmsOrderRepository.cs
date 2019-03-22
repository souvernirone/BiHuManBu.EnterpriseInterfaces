using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public  class SmsOrderRepository:EfRepositoryBase<bx_sms_order>, ISmsOrderRepository
    {
        private ILog logError;

        public SmsOrderRepository(DbContext context) : base(context)
        {
            logError = LogManager.GetLogger("ERROR");
        }


        public bx_sms_order Add(bx_sms_order bxSmsOrder)
        {
            bx_sms_order model = new bx_sms_order();
            try
            {
                model = Table.Add(bxSmsOrder);
                SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return model;
        }
        public bx_sms_order_error AddSMSOrderError(bx_sms_order_error smsOrderError)
        {
            bx_sms_order_error model = new bx_sms_order_error();
            try
            {
                model = DataContextFactory.GetDataContext().bx_sms_order_error.Add(smsOrderError);
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
        public int Update(bx_sms_order bxSmsOrder)
        {
            int count = 0;
            try
            {
                Table.AddOrUpdate(bxSmsOrder);
                count = SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public bx_sms_order Find(int orderId)
        {
            bx_sms_order smsOrder = new bx_sms_order();
            try
            {
                smsOrder = Table.FirstOrDefault(x => x.Id == orderId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return smsOrder;
        }

        /// <summary>
        /// 根据OrderNum获取对象
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bx_sms_order Find(string orderNum)
        {
            bx_sms_order smsOrder = new bx_sms_order();
            try
            {
                smsOrder = Table.FirstOrDefault(x => x.OrderNum == orderNum);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return smsOrder;
        }

    }
}
