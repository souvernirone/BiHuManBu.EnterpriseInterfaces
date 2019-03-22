using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using log4net;
using MySql.Data.MySqlClient;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System.Data;
using System.Data.Entity.Validation;

namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class OrderRepository : AppIRepository.IOrderRepository
    {
        //bx_car_order 中的topagent 主要是为了查询方便用的，能很清楚的知道当前数据的顶级是谁
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public int Update(bx_car_order order)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_car_order.AddOrUpdate(order);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public bx_car_order FindBy(long orderId)
        {
            bx_car_order carOrder = new bx_car_order();
            try
            {
                carOrder = DataContextFactory.GetDataContext().bx_car_order.FirstOrDefault(x => x.id == orderId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return carOrder;
        }

        public bx_car_order FindBy(string orderNo)
        {
            bx_car_order carOrder = new bx_car_order();
            try
            {
                carOrder = DataContextFactory.GetDataContext().bx_car_order.FirstOrDefault(x => x.order_num == orderNo);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return carOrder;
        }
        public AppViewModels.CarOrderModel FindCarOrderBy(long orderId)//, string openId
        {
            AppViewModels.CarOrderModel carOrder = new AppViewModels.CarOrderModel();
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@orderId", MySqlDbType.Int64)//,
                    //new MySqlParameter("@openId", MySqlDbType.String)
                };
                parameters[0].Value = orderId;
                //parameters[1].Value = openId;
                StringBuilder sql = new StringBuilder(@"SELECT bc.id,bc.order_num,bc.buid,bc.source,bc.insured_name,bc.contacts_name,bc.receipt_head,bc.receipt_title,bc.pay_type,bc.distribution_type,
bc.create_time,bc.user_id,bc.openid,bc.total_price,bc.carriage_price,bc.insurance_price,bc.id_type,bc.id_num,bc.distribution_address,bc.order_status,
bc.id_img_firs,bc.id_img_secd,bc.top_agent,bc.mobile,bc.addressid,bc.cur_agent,bc.pay_status,bc.distribution_name,bc.distribution_phone,bc.distribution_time,
bc.bizRate,bc.LicenseNo,date_format(bc.GetOrderTime,'%Y-%m-%d %H:%i') AS GetOrderTime,bc.InvoiceType,bc.imageUrls,bc.forcerate,bc.order_email,bc.order_mobile,bc.single_time,bu.LicenseOwner as UserName,bu.MoldName,bu.CityCode as CityCode,IFNULL(bc.order_email,'') as OrderEmail,IFNULL(bc.order_mobile,'') as OrderPhone,bs.CheSun,bs.SanZhe,bs.DaoQiang,bs.SiJi,bs.ChengKe,bs.BoLi,bs.HuaHen,bs.BuJiMianCheSun
                                                        ,bs.BuJiMianSanZhe,bs.BuJiMianDaoQiang,bs.BuJiMianRenYuan,bs.BuJiMianFuJian,bs.TeYue,bs.SheShui,bs.CheDeng,bs.ZiRan,bs.JiaoQiang,
bs.BuJiMianChengKe,bs.BuJiMianSiJi,bs.BuJiMianHuaHen,bs.BuJiMianSheShui,bs.BuJiMianZiRan,bs.BuJiMianJingShenSunShi,
bs.HcSheBeiSunshi,bs.HcHuoWuZeRen,bs.HcFeiYongBuChang,bs.HcJingShenSunShi,bs.HcSanFangTeYue,bs.HcXiuLiChang,
                                                        DATE_FORMAT(  bq.BizStartDate, '%Y-%m-%d') as  BizStartDate    ,  DATE_FORMAT( bq.ForceStartDate, '%Y-%m-%d') as ForceStartDate,
                                                        bq.CheSun AS CheSunBaoFei,
                                                        bq.SanZhe AS SanZheBaoFei,
                                                        bq.DaoQiang AS DaoQiangBaoFei,
                                                        bq.SiJi AS SiJiBaoFei,
                                                        bq.ChengKe AS ChengKeBaoFei,
                                                        bq.BoLi AS BoLiBaoFei,
                                                        bq.HuaHen AS HuaHenBaoFei,
                                                        bq.BuJiMianCheSun AS BuJiMianCheSunBaoFei,
                                                        bq.BuJiMianSanZhe AS BuJiMianSanZheBaoFei,
                                                        bq.BuJiMianDaoQiang AS BuJiMianDaoQiangBaoFei,
                                                        bq.BuJiMianRenYuan AS BuJiMianRenYuanBaoFei,
                                                        bq.BuJiMianFuJian AS BuJiMianFuJianFei,
                                                        bq.BuJiMianChengKe AS BuJiMianChengKeBaoFei,
                                                        bq.BuJiMianSiJi AS BuJiMianSiJiBaoFei,
                                                        bq.BuJiMianHuaHen AS BuJiMianHuaHenBaoFei,
                                                        bq.BuJiMianSheShui AS BuJiMianSheShuiBaoFei,
                                                        bq.BuJiMianZiRan AS BuJiMianZiRanBaoFei,
                                                        bq.BuJiMianJingShenSunShi AS BuJiMianJingShenSunShiBaoFei,
                                                        bq.HcSheBeiSunshi AS HcSheBeiSunshiBaoFei,
                                                        bq.HcHuoWuZeRen AS HcHuoWuZeRenBaoFei,
                                                        bq.HcFeiYongBuChang AS HcFeiYongBuChangBaoFei,
                                                        bq.HcJingShenSunShi AS HcJingShenSunShiBaoFei,
                                                        bq.HcSanFangTeYue AS HcSanFangTeYueBaoFei,
                                                        bq.HcXiuLiChang AS HcXiuLiChangBaoFei,
                                                        bq.TeYue AS TeYueBaoFei,
                                                        bq.SheShui AS SheShuiBaoFei,
                                                        bq.CheDeng AS CheDengBaoFei,
                                                        bq.ZiRan AS ZiRanBaoFei,
                                                        bq.BizTotal as BizTotal,
                                                        bq.ForceTotal as ForceTotal,
                                                        bq.TaxTotal as TaxTotal
                                                         FROM bx_car_order bc
                                                         LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId
                                                         LEFT JOIN bx_order_savequote bs ON bc.id=bs.OrderId 
                                                         LEFT JOIN bx_order_quoteresult bq ON bc.id=bq.OrderId 
                                                        where  bc.Id=@orderId limit 1");//bc.openid=@openid AND
                carOrder = DataContextFactory.GetDataContext().Database.SqlQuery<AppViewModels.CarOrderModel>(sql.ToString(),
                    parameters.ToArray()).FirstOrDefault();
                return carOrder;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return carOrder;
        }

        /// <summary>
        /// 获取我的订单列表；有个问题：二级看不到三级的直客订单
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="agentId"></param>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="isAgent"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<AppViewModels.CarOrderModel> FindListBy(int status, bool isAgent, List<bx_agent> sonself, string openId, int agentId, string search, int pageIndex, int pageSize, out int totalCount)
        {
            var carOrders = new List<AppViewModels.CarOrderModel>();
            string agentWhere = string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                agentWhere = " (bc.order_status>0 or order_status=-2) ";

            }
            else if (status == 0)
            {
                agentWhere = " bc.order_status>0 ";
            }
            else if (status == -2)
            {
                agentWhere = " bc.order_status=-2 ";
            }
            else if (status == -3)
            {
                agentWhere = " bc.order_status=-3 ";
            }
            else if (status == -4)
            {
                agentWhere = " bc.order_status=-4 ";
            }
            else
            {
                agentWhere = " bc.order_status>0 ";
            }
            agentWhere += " AND bc.top_agent=@agentId ";

            if (isAgent)
            {//代理人
                var strId = new StringBuilder();
                if (sonself.Any())
                {
                    foreach (var item in sonself)
                    {
                        strId.Append(item.Id).Append(',');
                    }
                }
                if (strId.Length > 1)
                {
                    strId.Remove(strId.Length - 1, 1);
                    agentWhere += " AND bc.cur_agent in (" + strId.ToString() + ")";
                }
                else
                {
                    agentWhere += " AND (bc.openid=@openId) ";
                }
            }
            else
            {//直客
                agentWhere += " AND bc.openid=@openId ";
            }
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@openId", MySqlDbType.String),
                    new MySqlParameter("@agentId", MySqlDbType.Int32),

                    new MySqlParameter("@license", MySqlDbType.String),
                    new MySqlParameter("@pagebegin", MySqlDbType.Int32),
                    new MySqlParameter("@pageend", MySqlDbType.Int32)
                };
                parameters[0].Value = openId;
                parameters[1].Value = agentId;
                parameters[2].Value = "%" + (string.IsNullOrEmpty(search) ? "" : search.ToUpper()) + "%";
                parameters[3].Value = (pageIndex - 1) * pageSize;
                parameters[4].Value = pageSize;
                var sql = new StringBuilder(string.Format(@"SELECT bc.id,bc.order_num,bc.buid,bc.source,bc.insured_name,bc.contacts_name,bc.receipt_head,bc.receipt_title,bc.pay_type,bc.distribution_type,
bc.create_time,bc.user_id,bc.openid,bc.total_price,bc.carriage_price,bc.insurance_price,bc.id_type,bc.id_num,bc.distribution_address,bc.order_status,
bc.id_img_firs,bc.id_img_secd,bc.top_agent,bc.mobile,bc.addressid,bc.cur_agent,bc.pay_status,bc.distribution_name,bc.distribution_phone,bc.distribution_time,
bc.bizRate,bc.LicenseNo,date_format(bc.GetOrderTime,'%Y-%m-%d %H:%i') AS GetOrderTime,bc.InvoiceType,bc.imageUrls,bc.forcerate,bc.order_email,bc.order_mobile,bc.single_time,bu.LicenseOwner as UserName,bu.MoldName,bu.CityCode as CityCode,bc.order_email as OrderEmail,bc.order_mobile as OrderPhone,
bs.CheSun,
bs.SanZhe,
bs.DaoQiang,
bs.SiJi,
bs.ChengKe,
bs.BoLi,
bs.HuaHen,
bs.BuJiMianCheSun ,
bs.BuJiMianSanZhe,
bs.BuJiMianDaoQiang,
bs.BuJiMianRenYuan,
bs.BuJiMianFuJian,
bs.TeYue,
bs.SheShui,
bs.CheDeng,
bs.ZiRan,
bs.JiaoQiang,
bs.BuJiMianChengKe,
bs.BuJiMianSiJi,
bs.BuJiMianHuaHen,
bs.BuJiMianSheShui,
bs.BuJiMianZiRan,
bs.BuJiMianJingShenSunShi,
 bs.HcSheBeiSunshi,
bs.HcHuoWuZeRen,
bs.HcFeiYongBuChang,
bs.HcJingShenSunShi,
bs.HcSanFangTeYue,
bs.HcXiuLiChang,

                                                        DATE_FORMAT(  bq.BizStartDate, '%Y-%m-%d') as  BizStartDate    ,  DATE_FORMAT( bq.ForceStartDate, '%Y-%m-%d') as ForceStartDate,
                                                        bq.CheSun AS CheSunBaoFei,
                                                        bq.SanZhe AS SanZheBaoFei,
                                                        bq.DaoQiang AS DaoQiangBaoFei,
                                                        bq.SiJi AS SiJiBaoFei,
                                                        bq.ChengKe AS ChengKeBaoFei,
                                                        bq.BoLi AS BoLiBaoFei,
                                                        bq.HuaHen AS HuaHenBaoFei,
                                                        bq.BuJiMianCheSun AS BuJiMianCheSunBaoFei,
                                                        bq.BuJiMianSanZhe AS BuJiMianSanZheBaoFei,
                                                        bq.BuJiMianDaoQiang AS BuJiMianDaoQiangBaoFei,
                                                        bq.BuJiMianRenYuan AS BuJiMianRenYuanBaoFei,
                                                        bq.BuJiMianFuJian AS BuJiMianFuJianFei,
                                                        bq.BuJiMianChengKe AS BuJiMianChengKeBaoFei,
                                                        bq.BuJiMianSiJi AS BuJiMianSiJiBaoFei,
                                                        bq.BuJiMianHuaHen AS BuJiMianHuaHenBaoFei,
                                                        bq.BuJiMianSheShui AS BuJiMianSheShuiBaoFei,
                                                        bq.BuJiMianZiRan AS BuJiMianZiRanBaoFei,
                                                        bq.BuJiMianJingShenSunShi AS BuJiMianJingShenSunShiBaoFei,
                                                        bq.HcSheBeiSunshi AS HcSheBeiSunshiBaoFei,
                                                        bq.HcHuoWuZeRen AS HcHuoWuZeRenBaoFei,
                                                        bq.HcFeiYongBuChang AS HcFeiYongBuChangBaoFei,
                                                        bq.HcJingShenSunShi AS HcJingShenSunShiBaoFei,
                                                        bq.HcSanFangTeYue AS HcSanFangTeYueBaoFei,
                                                        bq.HcXiuLiChang AS HcXiuLiChangBaoFei,
                                                        bq.TeYue AS TeYueBaoFei,
                                                        bq.SheShui AS SheShuiBaoFei,
                                                        bq.CheDeng AS CheDengBaoFei,
                                                        bq.ZiRan AS ZiRanBaoFei,
 bq.BizTotal as BizTotal,
                                                        bq.ForceTotal as ForceTotal,
                                                        bq.TaxTotal as TaxTotal,
                                                        bu.CarVIN as CarVin,
bu.EngineNo as EngineNo,
bu.RegisterDate as RegisterDate,
case when bu.CarUsedType is NULL then 0 else  bu.CarUsedType  end as CarUsedType
                                                         FROM bx_car_order bc
                                                         LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId
                                                         LEFT JOIN bx_order_savequote bs ON bc.id=bs.OrderId 
                                                         LEFT JOIN bx_order_quoteresult bq ON bc.id=bq.OrderId 
                                                        where {0}", agentWhere));
                if (!string.IsNullOrEmpty(search))
                {
                    sql.Append(" and  bc.LicenseNo like @license ");
                }
                sql.Append("  ORDER BY bc.id DESC limit @pagebegin,@pageend ");
                carOrders = DataContextFactory.GetDataContext().Database.SqlQuery<AppViewModels.CarOrderModel>(sql.ToString(),
                    parameters.ToArray()).ToList();

                var sqlcount = new StringBuilder(string.Format(@"SELECT count(1)
                 FROM bx_car_order bc
                LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId where {0}", agentWhere));
                if (!string.IsNullOrEmpty(search))
                {
                    sqlcount.Append(" and  bc.LicenseNo like @license ");
                }
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sqlcount.ToString(), parameters.ToArray()).FirstOrDefault();

                return carOrders;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            totalCount = 0;
            return carOrders;
        }

        public List<AppViewModels.CarOrderModel> FindListForApp(int status, List<string> sonself, string openId, int agentId, string search, int pageIndex, int pageSize, out int totalCount)
        {
            var carOrders = new List<AppViewModels.CarOrderModel>();
            string agentWhere = string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                agentWhere = " (bc.order_status>0 or order_status=-2) ";

            }
            else if (status == 0)
            {
                agentWhere = " bc.order_status>0 ";
            }
            else if (status == -2)
            {
                agentWhere = " bc.order_status=-2 ";
            }
            else if (status == -3)
            {
                agentWhere = " bc.order_status=-3 ";
            }
            else if (status == -4)
            {
                agentWhere = " bc.order_status=-4 ";
            }
            else
            {
                agentWhere = " bc.order_status>0 ";
            }



            var strId = string.Empty;
            if (sonself.Any())
            {
                strId=string.Join(",", sonself);
              
            }
            agentWhere += " AND bc.top_agent=@agentId ";

            agentWhere += " AND bc.cur_agent in (" + strId.ToString() + ")";

            try
            {
                MySqlParameter[] parameters =
                {
               
                    new MySqlParameter("@agentId", MySqlDbType.Int32),

                    new MySqlParameter("@license", MySqlDbType.String),
                    new MySqlParameter("@pagebegin", MySqlDbType.Int32),
                    new MySqlParameter("@pageend", MySqlDbType.Int32)
                };
          
                parameters[0].Value = agentId;
                parameters[1].Value = "%" + (string.IsNullOrEmpty(search) ? "" : search.ToUpper()) + "%";
                parameters[2].Value = (pageIndex - 1) * pageSize;
                parameters[3].Value = pageSize;
                var sql = new StringBuilder(string.Format(@"SELECT bc.id,bc.order_num,bc.buid,bc.source,bc.insured_name,bc.contacts_name,bc.receipt_head,IFNULL(bc.receipt_title,'') AS receipt_title,IFNULL(bc.receipt_title,'') AS receipt,bc.pay_type,bc.distribution_type,
bc.create_time,bc.user_id,bc.openid,bc.total_price,bc.carriage_price,bc.insurance_price,bc.id_type,bc.id_num,IFNULL(bc.distribution_address,'') AS distribution_address,bc.order_status,
bc.id_img_firs,bc.id_img_secd,bc.top_agent,bc.mobile,bc.addressid,bc.cur_agent,bc.pay_status,bc.distribution_name,IFNULL(bc.distribution_phone,'') AS distribution_phone,bc.distribution_time,
bc.bizRate,bc.LicenseNo,date_format(bc.GetOrderTime,'%Y-%m-%d %H:%i') AS GetOrderTime,
IFNULL(bc.InvoiceType,0) AS InvoiceType,IFNULL(bc.imageUrls,'') AS imageUrls,bc.forcerate,
bc.order_email,bc.order_mobile,bc.single_time,
bu.LicenseOwner as UserName,bu.MoldName,bu.CityCode as CityCode,IFNULL(bc.order_email,'') as OrderEmail,IFNULL(bc.order_mobile,'') as OrderPhone,
bs.CheSun,
bs.SanZhe,
bs.DaoQiang,
bs.SiJi,
bs.ChengKe,
bs.BoLi,
bs.HuaHen,
bs.BuJiMianCheSun ,
bs.BuJiMianSanZhe,
bs.BuJiMianDaoQiang,
bs.BuJiMianRenYuan,
bs.BuJiMianFuJian,
bs.TeYue,
bs.SheShui,
bs.CheDeng,
bs.ZiRan,
bs.JiaoQiang,
bs.BuJiMianChengKe,
bs.BuJiMianSiJi,
bs.BuJiMianHuaHen,
bs.BuJiMianSheShui,
bs.BuJiMianZiRan,
bs.BuJiMianJingShenSunShi,
 bs.HcSheBeiSunshi,
bs.HcHuoWuZeRen,
bs.HcFeiYongBuChang,
bs.HcJingShenSunShi,
bs.HcSanFangTeYue,
bs.HcXiuLiChang,

                                                 IFNULL(DATE_FORMAT(  bq.BizStartDate, '%Y-%m-%d'),'') as  BizStartDate    ,  IFNULL(DATE_FORMAT( bq.ForceStartDate, '%Y-%m-%d'),'') as ForceStartDate,
                                                        bq.CheSun AS CheSunBaoFei,
                                                        bq.SanZhe AS SanZheBaoFei,
                                                        bq.DaoQiang AS DaoQiangBaoFei,
                                                        bq.SiJi AS SiJiBaoFei,
                                                        bq.ChengKe AS ChengKeBaoFei,
                                                        bq.BoLi AS BoLiBaoFei,
                                                        bq.HuaHen AS HuaHenBaoFei,
                                                        bq.BuJiMianCheSun AS BuJiMianCheSunBaoFei,
                                                        bq.BuJiMianSanZhe AS BuJiMianSanZheBaoFei,
                                                        bq.BuJiMianDaoQiang AS BuJiMianDaoQiangBaoFei,
                                                        bq.BuJiMianRenYuan AS BuJiMianRenYuanBaoFei,
                                                        bq.BuJiMianFuJian AS BuJiMianFuJianFei,
                                                        bq.BuJiMianChengKe AS BuJiMianChengKeBaoFei,
                                                        bq.BuJiMianSiJi AS BuJiMianSiJiBaoFei,
                                                        bq.BuJiMianHuaHen AS BuJiMianHuaHenBaoFei,
                                                        bq.BuJiMianSheShui AS BuJiMianSheShuiBaoFei,
                                                        bq.BuJiMianZiRan AS BuJiMianZiRanBaoFei,
                                                        bq.BuJiMianJingShenSunShi AS BuJiMianJingShenSunShiBaoFei,
                                                        bq.HcSheBeiSunshi AS HcSheBeiSunshiBaoFei,
                                                        bq.HcHuoWuZeRen AS HcHuoWuZeRenBaoFei,
                                                        bq.HcFeiYongBuChang AS HcFeiYongBuChangBaoFei,
                                                        bq.HcJingShenSunShi AS HcJingShenSunShiBaoFei,
                                                        bq.HcSanFangTeYue AS HcSanFangTeYueBaoFei,
                                                        bq.HcXiuLiChang AS HcXiuLiChangBaoFei,
                                                        bq.TeYue AS TeYueBaoFei,
                                                        bq.SheShui AS SheShuiBaoFei,
                                                        bq.CheDeng AS CheDengBaoFei,
                                                        bq.ZiRan AS ZiRanBaoFei,
 bq.BizTotal as BizTotal,
                                                        bq.ForceTotal as ForceTotal,
                                                        bq.TaxTotal as TaxTotal,
                                                        bu.CarVIN as CarVin,
bu.EngineNo as EngineNo,
bu.RegisterDate as RegisterDate,
case when bu.CarUsedType is NULL then 0 else  bu.CarUsedType  end as CarUsedType
                                                         FROM bx_car_order bc
                                                         LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId
                                                         LEFT JOIN bx_order_savequote bs ON bc.id=bs.OrderId 
                                                         LEFT JOIN bx_order_quoteresult bq ON bc.id=bq.OrderId 
                                                        where {0}", agentWhere));
                if (!string.IsNullOrEmpty(search))
                {
                    sql.Append(" and  bc.LicenseNo like @license ");
                }
                sql.Append("  ORDER BY bc.id DESC limit @pagebegin,@pageend ");

                carOrders = DataContextFactory.GetDataContext().Database.SqlQuery<AppViewModels.CarOrderModel>(sql.ToString(),
                    parameters.ToArray()).ToList();

                var sqlcount = new StringBuilder(string.Format(@"SELECT count(1)
                 FROM bx_car_order bc
                LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId where {0}", agentWhere));
                if (!string.IsNullOrEmpty(search))
                {
                    sqlcount.Append(" and  bc.LicenseNo like @license ");
                }
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sqlcount.ToString(), parameters.ToArray()).FirstOrDefault();

                return carOrders;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            totalCount = 0;
            return carOrders;
        }

        /// <summary>
        /// 根据车牌号、openid、顶级代理人来获取预约单
        /// 下单的时候判断是否存在该预约单
        /// </summary>
        /// <param name="LicenseNo"></param>
        /// <param name="OpenId"></param>
        /// <param name="TopAgent"></param>
        /// <returns></returns>
        public bx_car_order FindBy(string licenseNo, string openId, int topAgent)
        {
            bx_car_order carOrder = new bx_car_order();
            try
            {
                carOrder = DataContextFactory.GetDataContext().bx_car_order.FirstOrDefault(x => x.LicenseNo == licenseNo && x.openid == openId && x.top_agent == topAgent && x.order_status > 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return carOrder;
        }

        public long CreateOrder(bx_car_order order, user user, bx_address address, bx_lastinfo lastinfo, bx_userinfo userinfo, bx_savequote savequote, bx_submit_info submitInfo, bx_quoteresult quoteresult, bx_quoteresult_carinfo carInfo, List<bx_claim_detail> claimDetails)
        {
            //如果此四张表数据为空，提示插入失败
            //if (userinfo == null || savequote == null || submitInfo == null || quoteresult == null)
            if (userinfo == null)
            {
                return 0;
            }
            long orderid = 0;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //插入订单
                    var neworder = DataContextFactory.GetDataContext().bx_car_order.Add(order);
                    DataContextFactory.GetDataContext().SaveChanges();
                    orderid = neworder.id;

                    if (orderid > 0)
                    {
                        #region CarOrderUserInfoRepository

                        bx_order_userinfo orderUserinfo = new bx_order_userinfo()
                        {
                            Id = userinfo.Id,
                            LicenseNo = userinfo.LicenseNo,
                            OpenId = userinfo.OpenId,
                            CityCode = userinfo.CityCode,
                            EngineNo = userinfo.EngineNo,
                            CarVIN = userinfo.CarVIN,
                            MoldName = userinfo.MoldName,
                            RegisterDate = userinfo.RegisterDate,
                            Agent = userinfo.Agent,
                            LicenseOwner = userinfo.LicenseOwner,
                            CarType = carInfo != null ? carInfo.car_type : null,
                            CarUsedType = carInfo != null ? carInfo.car_used_type : null,
                            JiaoQiangEndDate =
                                lastinfo != null
                                    ? (!string.IsNullOrEmpty(lastinfo.last_end_date) ? lastinfo.last_end_date : "")
                                    : "",
                            ShangYeEndDate =
                                lastinfo != null
                                    ? (!string.IsNullOrEmpty(lastinfo.last_business_end_date)
                                        ? lastinfo.last_business_end_date
                                        : "")
                                    : "",
                            //2017.2.7新增
                            LastYearSource = userinfo.LastYearSource,
                            InsuredName = userinfo.InsuredName,
                            InsuredMobile = userinfo.InsuredMobile,
                            InsuredIdCard = userinfo.InsuredIdCard,
                            InsuredAddress = userinfo.InsuredAddress,
                            RenewalType = userinfo.RenewalType,
                            InsuredIdType = userinfo.InsuredIdType,
                            OwnerIdCardType = userinfo.OwnerIdCardType,
                            Email = userinfo.Email,
                            nonclaim_discount_rate = lastinfo != null ? lastinfo.nonclaim_discount_rate : null,
                            last_year_acctimes = lastinfo != null ? lastinfo.last_year_acctimes : null,
                            last_year_claimamount = lastinfo != null ? lastinfo.last_year_claimamount : null,
                            last_year_claimtimes = lastinfo != null ? lastinfo.last_year_claimtimes : null,
                            //end新增
                            OrderId = orderid
                        };

                        #endregion

                        var re_ui = DataContextFactory.GetDataContext().bx_order_userinfo.Add(orderUserinfo);
                        DataContextFactory.GetDataContext().SaveChanges();

                        if (quoteresult != null)
                        {
                            #region CarOrderQuoteResultRepository

                            bx_order_quoteresult orderQuoteresult = new bx_order_quoteresult()
                            {
                                B_Uid = quoteresult.B_Uid,
                                CreateTime = quoteresult.CreateTime,
                                CheSun = quoteresult.CheSun,
                                SanZhe = quoteresult.SanZhe,
                                DaoQiang = quoteresult.DaoQiang,
                                SiJi = quoteresult.SiJi,
                                ChengKe = quoteresult.ChengKe,
                                BoLi = quoteresult.BoLi,
                                HuaHen = quoteresult.HuaHen,
                                BuJiMianCheSun = quoteresult.BuJiMianCheSun,
                                BuJiMianSanZhe = quoteresult.BuJiMianSanZhe,
                                BuJiMianDaoQiang = quoteresult.BuJiMianDaoQiang,
                                BuJiMianRenYuan = quoteresult.BuJiMianRenYuan,
                                BuJiMianFuJian = quoteresult.BuJiMianFuJian,

                                //2.1.5版本修改 增加6个字段
                                BuJiMianChengKe = quoteresult.BuJiMianChengKe,
                                BuJiMianSiJi = quoteresult.BuJiMianSiJi,
                                BuJiMianHuaHen = quoteresult.BuJiMianHuaHen,
                                BuJiMianSheShui = quoteresult.BuJiMianSheShui,
                                BuJiMianZiRan = quoteresult.BuJiMianZiRan,
                                BuJiMianJingShenSunShi = quoteresult.BuJiMianJingShenSunShi,

                                TeYue = quoteresult.TeYue,
                                SheShui = quoteresult.SheShui,
                                CheDeng = quoteresult.CheDeng,
                                ZiRan = quoteresult.ZiRan,
                                BizTotal = quoteresult.BizTotal,
                                ForceTotal = quoteresult.ForceTotal,
                                TaxTotal = quoteresult.TaxTotal,
                                BizContent = quoteresult.BizContent,
                                ForceContent = quoteresult.ForceContent,
                                SavedAmount = quoteresult.SavedAmount,
                                Source = quoteresult.Source,
                                BizStartDate = quoteresult.BizStartDate,
                                ForceStartDate = quoteresult.ForceStartDate,
                                HcSheBeiSunshi = quoteresult.HcSheBeiSunshi,
                                HcHuoWuZeRen = quoteresult.HcHuoWuZeRen,
                                HcFeiYongBuChang = quoteresult.HcFeiYongBuChang,
                                HcJingShenSunShi = quoteresult.HcJingShenSunShi,
                                HcSanFangTeYue = quoteresult.HcSanFangTeYue,
                                HcXiuLiChang = quoteresult.HcXiuLiChang,
                                InsuredName = quoteresult.InsuredName,
                                InsuredIdCard = quoteresult.InsuredIdCard,
                                InsuredIdType = quoteresult.InsuredIdType,
                                InsuredMobile = quoteresult.InsuredMobile,
                                HolderName = quoteresult.HolderName,
                                HolderIdCard = quoteresult.HolderIdCard,
                                HolderIdType = quoteresult.HolderIdType,
                                HolderMobile = quoteresult.HolderMobile,
                                RateFactor1 = quoteresult.RateFactor1,
                                RateFactor2 = quoteresult.RateFactor2,
                                RateFactor3 = quoteresult.RateFactor3,
                                RateFactor4 = quoteresult.RateFactor4,
                                //2017.2.7新增
                                HcXiuLiChangType = quoteresult.HcXiuLiChangType,
                                CheSunBE = quoteresult.CheSunBE,
                                ZiRanBE = quoteresult.ZiRanBE,
                                DaoQiangBE = quoteresult.DaoQiangBE,
                                //end新增
                                OrderId = orderid
                            };

                            #endregion

                            var re_qr = DataContextFactory.GetDataContext().bx_order_quoteresult.Add(orderQuoteresult);
                            DataContextFactory.GetDataContext().SaveChanges();
                        }

                        if (savequote != null)
                        {
                            #region CarOrderSaveQuoteRepository

                            bx_order_savequote orderSavequote = new bx_order_savequote()
                            {
                                B_Uid = savequote.B_Uid,
                                CheSun = savequote.CheSun,
                                SanZhe = savequote.SanZhe,
                                DaoQiang = savequote.DaoQiang,
                                SiJi = savequote.SiJi,
                                ChengKe = savequote.ChengKe,
                                BoLi = savequote.BoLi,
                                HuaHen = savequote.HuaHen,
                                BuJiMianCheSun = savequote.BuJiMianCheSun,
                                BuJiMianSanZhe = savequote.BuJiMianSanZhe,
                                BuJiMianDaoQiang = savequote.BuJiMianDaoQiang,
                                BuJiMianRenYuan = savequote.BuJiMianRenYuan,
                                BuJiMianFuJian = savequote.BuJiMianFuJian,

                                //2.1.5版本修改 增加6个字段
                                BuJiMianChengKe = savequote.BuJiMianChengKe,
                                BuJiMianSiJi = savequote.BuJiMianSiJi,
                                BuJiMianHuaHen = savequote.BuJiMianHuaHen,
                                BuJiMianSheShui = savequote.BuJiMianSheShui,
                                BuJiMianZiRan = savequote.BuJiMianZiRan,
                                BuJiMianJingShenSunShi = savequote.BuJiMianJingShenSunShi,

                                TeYue = savequote.TeYue,
                                SheShui = savequote.SheShui,
                                CheDeng = savequote.CheDeng,
                                ZiRan = savequote.ZiRan,
                                IsRenewal = savequote.IsRenewal,
                                CreateTime = savequote.CreateTime,
                                JiaoQiang = savequote.JiaoQiang,
                                BizStartDate = savequote.BizStartDate,
                                HcSheBeiSunshi = savequote.HcSheBeiSunshi,
                                HcHuoWuZeRen = savequote.HcHuoWuZeRen,
                                HcFeiYongBuChang = savequote.HcFeiYongBuChang,
                                HcJingShenSunShi = savequote.HcJingShenSunShi,
                                HcSanFangTeYue = savequote.HcSanFangTeYue,
                                HcXiuLiChang = savequote.HcXiuLiChang,
                                //2017.2.7新增
                                SheBeiSunShiConfig = savequote.SheBeiSunShiConfig,
                                FeiYongBuChangConfig = savequote.FeiYongBuChangConfig,
                                XiuLiChangConfig = savequote.XiuLiChangConfig,
                                co_real_value = savequote.co_real_value,
                                //end新增
                                OrderId = orderid
                            };

                            #endregion

                            var re_sqr = DataContextFactory.GetDataContext().bx_order_savequote.Add(orderSavequote);
                            DataContextFactory.GetDataContext().SaveChanges();
                        }

                        if (submitInfo != null)
                        {
                            #region CarOrderSubmitInfoRepository

                            bx_order_submit_info orderSubmitInfo = new bx_order_submit_info()
                            {
                                b_uid = submitInfo.b_uid,
                                license_no = submitInfo.license_no,
                                mobile = submitInfo.mobile,
                                source = submitInfo.source,
                                biz_tno = submitInfo.biz_tno,
                                biz_pno = submitInfo.biz_pno,
                                biz_start_time = submitInfo.biz_start_time,
                                biz_end_time = submitInfo.biz_end_time,
                                force_tno = submitInfo.force_tno,
                                force_pno = submitInfo.force_pno,
                                force_start_time = submitInfo.force_start_time,
                                force_end_time = submitInfo.force_end_time,
                                submit_status = submitInfo.submit_status,
                                submit_result = submitInfo.submit_result,
                                quote_status = submitInfo.quote_status,
                                quote_result = submitInfo.quote_result,
                                biz_rate = submitInfo.biz_rate,
                                force_rate = submitInfo.force_rate,
                                create_time = submitInfo.create_time,
                                update_time = submitInfo.update_time,
                                submit_result_toc = submitInfo.submit_result_toc,
                                quote_result_toc = submitInfo.quote_result_toc,
                                channel_id = submitInfo.channel_id,
                                //2017.2.7新增
                                err_code = submitInfo.err_code,
                                //end新增
                                OrderId = orderid
                            };

                            #endregion

                            var re_si = DataContextFactory.GetDataContext().bx_order_submit_info.Add(orderSubmitInfo);
                            DataContextFactory.GetDataContext().SaveChanges();
                        }
                        scope.Complete();
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            logError.Info(string.Format("Property: {0} Error: {1}",
                                validationError.PropertyName,
                                validationError.ErrorMessage));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:事务订单号(" + orderid + ")\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" +
                                  ex.InnerException);
                }
                finally
                {
                    //orderid = 0;
                    scope.Dispose();
                }
            }
            return orderid;
        }
        public AppViewModels.GetCountsViewModel GetCounts(string agentStr, string custKey, int agent)
        {
            var getCountsViewModel = new AppViewModels.GetCountsViewModel();
            var getAppoinmentOrdersCountSql = string.Format(@"SELECT count(1)
                 FROM bx_car_order bc
                LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId where  1=1 and bc.cur_agent in ({0}) and bc.top_agent={1}  and bc.order_status>0", agentStr, agent);
            var getQuotationReceiptOrdersCountSql = string.Format(@"SELECT count(1)
                 FROM bx_car_order bc
                LEFT JOIN bx_order_userinfo bu ON bc.id=bu.OrderId where  1=1 and bc.cur_agent in ({0}) and bc.top_agent={1}  and bc.order_status=-2", agentStr, agent);
            var getQuoteOrdersCount = string.Format(@"select count(1) FROM bx_car_renewal cr 
                                RIGHT JOIN bx_userinfo_renewal_index uri ON cr.Id=uri.car_renewal_id 
                                RIGHT JOIN bx_userinfo ui ON ui.Id =uri.b_uid WHERE LENGTH(ui.OpenId)>9 and  ui.Agent in ({0}) and ui.QuoteStatus>0", agentStr);

            getCountsViewModel.AppoinmentOrdersCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(getAppoinmentOrdersCountSql).FirstOrDefault();
            getCountsViewModel.QuotationReceiptOrdersCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(getQuotationReceiptOrdersCountSql).FirstOrDefault();
            getCountsViewModel.QuoteOrdersCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(getQuoteOrdersCount).FirstOrDefault();
            return getCountsViewModel;
        }
        public bx_car_order FindOrderByOrderId(long orderId, string orderNum)
        {
            return DataContextFactory.GetDataContext().bx_car_order.FirstOrDefault(x => x.id == orderId && x.order_num == orderNum);
        }
    }
}
