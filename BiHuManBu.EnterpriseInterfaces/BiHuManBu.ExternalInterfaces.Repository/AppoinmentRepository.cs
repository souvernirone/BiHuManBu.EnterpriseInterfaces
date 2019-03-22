using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using IAgentRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository.IAgentRepository;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AppoinmentRepository : IAppoinmentRepository
    {
        private readonly IAgentRepository _agentRepository;
        private readonly MySqlHelper _sqlhelper;

        public AppoinmentRepository(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
            _sqlhelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
        }

        public bx_car_order GetCarOrderByOrderId(long orderId)
        {
            using (var _dbContext = new EntityContext())
            {
                return _dbContext.bx_car_order.Where(c => c.id == orderId && c.order_status != -1).SingleOrDefault();
            }
        }

        public bool UpdateAppoinmentInfo(AppoinmentInfoRequest request)
        {
            using (var dbContext = new EntityContext())
            {
                var carOrder = dbContext.bx_car_order.SingleOrDefault(o => o.id == request.Id);
                if (carOrder == null)
                {
                    return false;
                }
                carOrder.contacts_name = request.ContactsName;
                carOrder.mobile = request.Mobile;
                carOrder.insured_name = request.InsuredName;
                carOrder.id_type = request.IdType;
                carOrder.id_num = request.IdNum;
                carOrder.receipt_head = request.InvoiceType;
                carOrder.receipt_title = request.ReceiptTitle;
                //carOrder.id_img_firs = insurancePeopleInfo.IdCardFrontage_ImgUrl;
                //carOrder.id_img_secd = insurancePeopleInfo.IdCardContrary_ImgUrl;
                carOrder.imageUrls = request.ImageUrls;
                carOrder.pay_type = request.PayType;
                carOrder.insurance_price = request.InsurancePrice;
                carOrder.distribution_type = request.DistributionType;
                carOrder.distribution_address = request.DistributionAddress;
                carOrder.distribution_name = request.DistributionName;
                carOrder.distribution_phone = request.DistributionPhone;
                carOrder.distribution_time = request.DistributionTime;
                carOrder.addressid = request.Addressid;
                carOrder.GetOrderTime = request.GetOrderTime;
                carOrder.order_status = -2;
                carOrder.source = request.Source == 2 ? 0 : request.Source > 1 && request.Source != 9999 ? Convert.ToInt32(Math.Log((int)request.Source, 2)) : request.Source;
                carOrder.single_time = request.SingleTime;

                carOrder.total_price = request.TotalPrice;
                carOrder.bizRate = request.BizRate;
                return dbContext.SaveChanges() >= 0;
            }
        }

        /// <summary>
        /// 获得订单列表
        /// </summary>
        /// <returns></returns>
        public IList<AppointmentOrderRequest> GetOrderList(AppointmentOrderRequest request, out int totalCount)
        {
            var idList = _agentRepository.GetSonsList(request.AgentId, true);

            var ps = new List<MySqlParameter>();
            StringBuilder sb = new StringBuilder(string.Format(@" where bx_car_order.cur_agent IN({0}) AND bx_car_order.order_status!=-1
AND bx_car_order.order_status!=-2 ", string.Join(",", idList)));

            if (request.Source != -1)
            {
                sb.Append("  and bx_car_order.source=?source");
                ps.Add(new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "source",
                    Value = request.Source
                });
            }
            if (request.CategoryId != -1)
            {
                sb.Append("  and ui.CategoryInfoId=?CategoryInfoId");
                ps.Add(new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "CategoryInfoId",
                    Value = request.CategoryId
                });
            }
            if (!string.IsNullOrEmpty(request.CreateOrderTimeStart))
            {
                sb.Append(" and bx_car_order.create_time>=?CreateOrderTimeStart");
                ps.Add(new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CreateOrderTimeStart",
                    Value = request.CreateOrderTimeStart
                });
            }
            if (!string.IsNullOrEmpty(request.CreateOrderTimeEnd))
            {
                sb.Append(" and bx_car_order.create_time<?CreateOrderTimeEnd");
                ps.Add(new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CreateOrderTimeEnd",
                    Value = Convert.ToDateTime(request.CreateOrderTimeEnd).AddDays(1).ToString("yyyy-MM-dd")
                });
            }
            if (!string.IsNullOrEmpty(request.LicenseNo))
            {
                sb.Append(" and bx_order_userinfo.LicenseNo=?LicenseNo");
                ps.Add(new MySqlParameter()
                {

                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "LicenseNo",
                    Value = request.LicenseNo.ToUpper()
                });
            }
            if (!string.IsNullOrEmpty(request.AgentName))
            {
                sb.Append(" and bx_agent.AgentName =?AgentName");
                ps.Add(new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "AgentName",
                    Value = request.AgentName

                });
            }
            string tempSql = @"SELECT  bx_car_order.order_status AS OrderStaus ,bx_car_order.buid AS BuId,
bx_car_order.id AS Id,bx_agent.AgentName AS AgentName,bx_agent.id as agentid, cc.CategoryInfo as CustomerCategory,
case  
when ISNULL(bx_order_quoteresult.BizStartDate) then '--'
when DATE_FORMAT(bx_order_quoteresult.BizStartDate,'%Y-%m-%d')='0001-01-01' then '--'
else DATE_FORMAT(bx_order_quoteresult.BizStartDate,'%Y-%m-%d') end as BusinessRisks_StartTime
,
case when ISNULL( bx_order_quoteresult.ForceStartDate) then '--'
when DATE_FORMAT(bx_order_quoteresult.ForceStartDate,'%Y-%m-%d')='0001-01-01' then '--'
else DATE_FORMAT(bx_order_quoteresult.ForceStartDate,'%Y-%m-%d') end 
AS ForceRisks_StartTime,bx_order_userinfo.LicenseNo AS LicenseNo,bx_order_userinfo.MoldName AS MoldName,
DATE_FORMAT(bx_car_order.create_time,'%Y-%m-%d %H:%i')   AS OrderTime,CASE WHEN bx_car_order.source=0 THEN '平安'
 WHEN bx_car_order.source=1 THEN '太平洋'
 WHEN bx_car_order.source=2 THEN '人保' 
 WHEN bx_car_order.source=3 THEN '国寿财'
 ELSE '' END AS SourceName,
 bx_car_order.source AS Source FROM bx_car_order
left join bx_userinfo as ui
on bx_car_order.buid=ui.id
left join bx_customercategories as cc
on ui.CategoryInfoId=cc.id
LEFT JOIN bx_order_userinfo
ON bx_car_order.id=bx_order_userinfo.OrderId
LEFT JOIN bx_order_quoteresult
ON bx_car_order.id=bx_order_quoteresult.OrderId
LEFT JOIN bx_order_submit_info
ON bx_car_order.id =bx_order_submit_info.OrderId
LEFT JOIN bx_agent
ON bx_car_order.cur_agent=bx_agent.Id {0}
ORDER BY bx_car_order.create_time DESC limit {1},{2}";
            string sqlCount = @"select count(1) FROM bx_car_order
left join bx_userinfo as ui
on bx_car_order.buid=ui.id
left join bx_customercategories as cc
on ui.CategoryInfoId=cc.id
LEFT JOIN bx_order_userinfo
ON bx_car_order.id=bx_order_userinfo.OrderId
LEFT JOIN bx_order_quoteresult
ON bx_car_order.id=bx_order_quoteresult.OrderId
LEFT JOIN bx_order_submit_info
ON bx_car_order.id =bx_order_submit_info.OrderId
LEFT JOIN bx_agent
ON bx_car_order.cur_agent=bx_agent.Id {0}
ORDER BY bx_car_order.create_time";
            totalCount = Convert.ToInt32(_sqlhelper.ExecuteScalar(string.Format(sqlCount, sb), ps.ToArray()));
            return _sqlhelper.ExecuteDataTable(string.Format(tempSql, sb, (request.PageIndex - 1) * request.PageSize, request.PageSize), ps.ToArray()).ToList<AppointmentOrderRequest>();
        }

        public IList<QuotationReceiptViewModel> GetQuotationReceiptList(QuotationReceiptRequest searchWhere, out int totalCount)
        {
            // bdx.PrintDate 暂时改成 bc.create_time > SingleTime
            int agent = searchWhere.AgentId > 0 ? searchWhere.AgentId : searchWhere.ChildAgent > 0 ? searchWhere.ChildAgent : 0;
            var idList = _agentRepository.GetSonsList(agent, true);
            string sqlGetData = @"SELECT bdx.Id as PolicyId, bc.buid as BuId, bc.id as OrderId, ba.AgentName as AgentName, bdx.BizStartDate as BusinessRisksStartTime,bdx.BizEndDate as BusinessRisksEndTime,cc.CategoryInfo as CustomerCategory,bc.insured_name as InsuredName,boq.BizStartDate,boq.ForceStartDate,
 bc.distribution_type as DistributionType,bdx.ForceEndDate as ForceRisksEndTime, bdx.ForceStartDate as ForceRisksStartTime,bc.create_time as CreateTime,bcu.LicenseNo as LicenseNo, bcu.MoldName as MoldName,IFNULL(bc.single_time,bc.create_time) AS SingleTime,    
bc.source as Source FROM bx_car_order AS bc
left join bx_userinfo as ui
on bc.buid=ui.id
left join bx_customercategories as cc
on ui.CategoryInfoId=cc.id
Left JOIN bx_order_userinfo AS bcu 
ON bc.id=bcu.OrderId
Left JOIN bd_baodaninfo_carorder_mapping AS bdm
ON bc.id=bdm.CarOrderId
Left JOIN bd_baodaninfo AS bdx
ON bdm.BaoDanInfoId=bdx.Id
LEFT JOIN bx_order_quoteresult as boq
ON bc.id= boq.OrderId
Left JOIN bx_agent AS ba
ON bcu.agent=ba.Id {0}";
            string sqlGetCount = @"select count(1) from bx_car_order AS bc
left join bx_userinfo as ui
on bc.buid=ui.id
left join bx_customercategories as cc
on ui.CategoryInfoId=cc.id
LEFT JOIN bx_order_userinfo AS bcu 
ON bc.id=bcu.OrderId
LEFT JOIN bd_baodaninfo_carorder_mapping AS bdm
ON bc.id=bdm.CarOrderId
LEFT JOIN bd_baodaninfo AS bdx
ON bdm.BaoDanInfoId=bdx.Id
LEFT JOIN bx_agent AS ba
ON bcu.agent=ba.Id  {0}";
            StringBuilder sb = new StringBuilder(string.Format("  where 1=1 and bc.order_status = -2 and bc.cur_agent in ({0}) ", string.Join(",", idList)));
            List<MySqlParameter> ps = new List<MySqlParameter>();

            if (searchWhere.Source != -1)
            {
                sb.Append(" and bc.source=?source ");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.Int32, ParameterName = "source", Value = searchWhere.Source });

            }
            if (searchWhere.CategoryId != -1)
            {
                sb.Append(" and ui.CategoryInfoId=?CategoryInfoId ");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.Int32, ParameterName = "CategoryInfoId", Value = searchWhere.CategoryId });
            }
            if (searchWhere.DistributionType != -1)
            {
                sb.Append(" and bc.distribution_type=?distribution_type ");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.Int32, ParameterName = "distribution_type", Value = searchWhere.DistributionType });
            }
            if (searchWhere.SingleStartTime.HasValue)
            {
                sb.Append(" and  bc.single_time>=?StartPrintDate");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StartPrintDate", Value = searchWhere.SingleStartTime });
            }
            if (searchWhere.SingleEndTime.HasValue)
            {
                sb.Append(" and  bc.single_time<?EndPrintDate");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "EndPrintDate", Value = searchWhere.SingleEndTime.Value.AddDays(1) });
            }
            //if (!string.IsNullOrEmpty(searchWhere.BusinessRisksStartTime_Str))
            //{
            //    sb.Append(" and bdx.BizEndDate>=?BizStartDate");
            //    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizStartDate", Value = Convert.ToDateTime(searchWhere.BusinessRisksStartTime_Str) });
            //}

            //if (!string.IsNullOrEmpty(searchWhere.BusinessRisksEndTime_Str))
            //{
            //    sb.Append(" and bdx.BizStartDate<?BizEndDate ");
            //    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDate", Value = Convert.ToDateTime(searchWhere.BusinessRisksEndTime_Str).AddDays(1) });
            //}
            //if (!string.IsNullOrEmpty(searchWhere.ForceRisksStartTime_Str))
            //{
            //    sb.Append(" and  bdx.ForceEndDate>=?ForceStartDate");
            //    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "ForceStartDate", Value = Convert.ToDateTime(searchWhere.ForceRisksStartTime_Str) });
            //}
            //if (!string.IsNullOrEmpty(searchWhere.ForceRisksEndTime_Str))
            //{
            //    sb.Append(" and bdx.ForceStartDate>=?ForceEndDate");
            //    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "ForceEndDate", Value = Convert.ToDateTime(searchWhere.ForceRisksEndTime_Str).AddDays(1) });
            //}
            if (!string.IsNullOrEmpty(searchWhere.LicenseNo))
            {
                sb.Append(" and bcu.LicenseNo=?LicenseNo");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.VarChar, ParameterName = "LicenseNo", Value = searchWhere.LicenseNo });
            }
            if (!string.IsNullOrEmpty(searchWhere.AgentName))
            {
                sb.Append(" and ba.AgentName=?AgentName");
                ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.VarChar, ParameterName = "AgentName", Value = searchWhere.AgentName });
            }

            totalCount = Convert.ToInt32(_sqlhelper.ExecuteScalar(string.Format(sqlGetCount, sb), ps.ToArray()));
            sb.Append(string.Format(" order by bc.single_time desc limit {0},{1}", (searchWhere.PageIndex - 1) * searchWhere.PageSize, searchWhere.PageSize));
            var orderList = _sqlhelper.ExecuteDataTable(string.Format(sqlGetData, sb), ps.ToArray()).ToList<QuotationReceiptViewModel>();
            return orderList;
        }
    }
}
