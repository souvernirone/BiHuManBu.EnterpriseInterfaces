using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using log4net;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Data.Entity;
using System.Diagnostics;
using System.Data.Entity.Validation;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private ILog logError;
        private EntityContext context;
        /// <summary>
        /// 身份证验证码创建时间是否是当天的
        /// 这是判断身份证是否过期时使用，现在不用了
        /// </summary>
        private const string VERIFICATION_CODE_CREATE_TIME_IS_TODAY = " IF(ISNULL(dd_order.verification_code_create_time),0, TO_DAYS(dd_order.verification_code_create_time)=TO_DAYS(NOW())) ";
        private static readonly string CheckFromMethod = System.Configuration.ConfigurationManager.AppSettings["CheckFromMethod"];
        public OrderRepository()
        {
            logError = LogManager.GetLogger("ERROR");
            context = DataContextFactory.GetDataContext();
        }
        public long FindTempOrderId(long buId)
        {
            long orderId = DataContextFactory.GetDataContext().dd_order.OrderByDescending(l => l.update_time).Where(x => x.b_uid == buId && x.order_type == 0).Select(s => s.id).FirstOrDefault();
            return orderId;
        }

        public List<dd_order> FindOrderListByBuid(long buId)
        {
            return DataContextFactory.GetDataContext().dd_order.Where(x => x.b_uid == buId).ToList();
        }

        /// <summary>
        /// 查询buid对应订单的ordernum
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public dd_order FindOrderNum(long buId)
        {
            dd_order order = DataContextFactory.GetDataContext().dd_order.OrderByDescending(l => l.update_time).Where(x => x.b_uid == buId && x.order_lapse_time.HasValue && x.order_lapse_time > DateTime.Now && (new int?[] { 0, 3, 41, 42 }).Contains(x.order_type)).FirstOrDefault();
            return order;
        }
        public dd_order FindOrderNum(List<long> buIds)
        {
            string buidstr = string.Join(",", buIds);
            buidstr = buidstr == "" ? "-1" : buidstr;
            string sql = "select * from dd_order where b_uid in (" + buidstr +
                         ") and order_lapse_time > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                         "' and order_type in (0,3,41,42,5) order by update_time desc;";
            //dd_order order = DataContextFactory.GetDataContext().dd_order.OrderByDescending(l => l.update_time).Where(x => buIds.Contains(x.b_uid) && x.order_lapse_time.HasValue && x.order_lapse_time > DateTime.Now && (new int?[] { 0, 3, 41, 42 }).Contains(x.order_type)).FirstOrDefault();
            dd_order order = DataContextFactory.GetDataContext().Database.SqlQuery<dd_order>(sql).ToList().FirstOrDefault();
            return order;
        }
        public dd_order FindOrder(long orderId)
        {
            dd_order model = DataContextFactory.GetDataContext().dd_order.FirstOrDefault(x => x.id == orderId);
            return model;
        }
        public dd_order FindOrder(string orderNum)
        {
            dd_order model = DataContextFactory.GetDataContext().dd_order.FirstOrDefault(x => x.order_num == orderNum);
            return model;
        }
        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int UpdateOrder(dd_order order)
        {
            DataContextFactory.GetDataContext().dd_order.AddOrUpdate(order);
            int count = DataContextFactory.GetDataContext().SaveChanges();
            return count;
        }

        public int Update(dd_order_quoteresult quoteresult)
        {
            DataContextFactory.GetDataContext().dd_order_quoteresult.AddOrUpdate(quoteresult);
            int count = DataContextFactory.GetDataContext().SaveChanges();
            return count;
        }
        public int UpdateSubmitinfo(bx_submit_info submitInfo)
        {
            DataContextFactory.GetDataContext().bx_submit_info.AddOrUpdate(submitInfo);
            int count = DataContextFactory.GetDataContext().SaveChanges();
            return count;
        }

        public dd_order_quoteresult FindDdOrderQuoteresult(long orderId)
        {
            dd_order_quoteresult model = DataContextFactory.GetDataContext().dd_order_quoteresult.OrderByDescending(l => l.update_time).FirstOrDefault(x => x.dd_order_id == orderId);
            return model;
        }

        public async Task<dd_order_quoteresult> FindDdOrderQuoteresultAsync(long orderId)
        {
            var sql = String.Format(@"SELECT * FROM dd_order_quoteresult WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1", orderId);
            dd_order_quoteresult model =
                DataContextFactory.GetDataContext().Database.SqlQuery<dd_order_quoteresult>(sql).ToList().FirstOrDefault();
            return model;
        }

        public dd_pay_way FindBankWayId(int cityId)
        {
            var sql = String.Format(@"SELECT A.id,A.pay_way,A.pay_bank_id FROM dd_pay_way AS A LEFT JOIN dd_pay_bank AS B ON A.PAY_BANK_ID = B.ID WHERE CITY_ID = {0} and pay_way='weixin' LIMIT 0,1", cityId);
            dd_pay_way model =
                DataContextFactory.GetDataContext().Database.SqlQuery<dd_pay_way>(sql).ToList().FirstOrDefault();
            return model;
        }

        public List<dd_order_savequote> FinDdOrderSavequotes(long[] ids)
        {
            List<dd_order_savequote> models = DataContextFactory.GetDataContext().dd_order_savequote.Where(x => ids.Contains(x.dd_order_id)).ToList();
            return models;
        }

        public dd_order_savequote FindDdOrderSavequote(long orderId)
        {
            dd_order_savequote model = DataContextFactory.GetDataContext().dd_order_savequote.OrderByDescending(l => l.update_time).FirstOrDefault(x => x.dd_order_id == orderId);
            return model;
        }

        public async Task<dd_order_savequote> FindDdOrderSavequoteAsync(long orderId)
        {
            var sql = String.Format(@"SELECT * FROM dd_order_savequote WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1", orderId);
            dd_order_savequote model =
                DataContextFactory.GetDataContext().Database.SqlQuery<dd_order_savequote>(sql).ToList().FirstOrDefault();
            return model;
        }

        public dd_order_related_info FindDdOrderRelatedinfo(long orderId)
        {
            dd_order_related_info model = DataContextFactory.GetDataContext().dd_order_related_info.OrderByDescending(l => l.update_time).FirstOrDefault(x => x.dd_order_id == orderId);
            return model;
        }
        public async Task<dd_order_related_info> FindDdOrderRelatedinfoAsync(long orderId)
        {
            var sql = String.Format(@"SELECT * FROM dd_order_related_info WHERE dd_order_id = {0} ORDER BY update_time DESC LIMIT 0,1", orderId);
            dd_order_related_info model =
                DataContextFactory.GetDataContext().Database.SqlQuery<dd_order_related_info>(sql).ToList().FirstOrDefault();
            return model;
        }

        public bx_quotereq_carinfo FindQuotereqCarinfo(long buid)
        {
            var sql = String.Format(@"SELECT * FROM bx_quotereq_carinfo WHERE b_uid = {0} ORDER BY update_time DESC LIMIT 0,1; ", buid);
            bx_quotereq_carinfo model =
                DataContextFactory.GetDataContext().Database.SqlQuery<bx_quotereq_carinfo>(sql).ToList().FirstOrDefault();
            return model;
        }

        public List<dd_order_paymentresult> GetOrderPayResult(string orderNum)
        {
            List<dd_order_paymentresult> list = DataContextFactory.GetDataContext().dd_order_paymentresult.Where(x => x.order_num == orderNum).ToList();
            // && x.find_pay_result == 1 && x.type == 1
            return list;
        }

        public async Task<List<dd_order_paymentresult>> GetOrderPayResultAsync(string orderNum)
        {
            var sql = String.Format(@"SELECT * FROM dd_order_paymentresult WHERE order_num = {0}", orderNum);
            List<dd_order_paymentresult> model =
                DataContextFactory.GetDataContext().Database.SqlQuery<dd_order_paymentresult>(sql).ToList();
            return model;
        }

        public SearchOrderDto SearchOrder(SearchOrderRequest search, List<int> listAgentId, List<int> listIssuingPerpleId, int isAndOr, int? CarOwnerId = null)
        {
            //2018-10-08 张克亮加入查询字段BUID 
            var sql = @"
                    SELECT 
                        dd_order.id as Id
                        ,dd_order.order_num  AS OrderNum
                        ,dd_order.create_time  AS CreateTime
                        ,dd_order.order_type  AS OrderType
                        ,dd_order.licenseno  AS LicenseNo
                        ,dd_order.source  AS Source
                        ,dd_order.consumer_pay_status AS CPayStatus
                        ,dd_order.verification_code_status  AS VerificationCodeStatus
                        ,dd_order.insurance_company_pay_status  AS InsuranceCompanyPayStatus
                        ,dd_order.issue_time  AS IssueTime
                        ,dd_order.agent_name  AS AgentName
                        ,dd_order.order_lapse_time  AS OrderLapseTime
                        ,dd_order.issuing_people_name  AS IssuingPeopleName
                        ,dd_order.pay_code_create_time  AS PayCodeCreateTime
                        ,dd_order.verification_code_create_time AS VerificationCodeCreateTime
                        ,dd_order.MoldName
                        ,dd_order.total_amount AS TotalAmount
                        ,dd_order.purchase_amount AS PurchaseAmount
                        ,dd_order.LastBizEndDate 
                        ,dd_order.LastForceEndDate
                        ,dd_order.payee AS Payee
                        ,dd_order.pay_type AS PayType
                        ,dd_order.get_order_method AS GetOrderMethod
                        ,dd_order.verification_code_status AS VerificationCodeStatus
                        ,dd_order_related_info.holder_name AS HolderName
                        ,dd_order_quoteresult.quote_status AS QuoteStatus
                        ,dd_order_quoteresult.submit_status AS SubmitStatus 
                        ,dd_order_paymentresult.payment_time as PaymentTime
                        ,dd_order_related_info.holder_name AS HolderName
                        ,dd_order.b_uid as BUID
                        ,'' AS Commission
                        ,0 AS Integral
                    FROM dd_order 
                    left join dd_order_related_info on dd_order.id = dd_order_related_info.dd_order_id 
                    left join dd_order_quoteresult on dd_order.id = dd_order_quoteresult.dd_order_id  
                    left join dd_order_paymentresult on dd_order.order_num = dd_order_paymentresult.order_num AND dd_order_paymentresult.type = 1
                    where 
                        {0} 
                    order by 
                        dd_order.create_time desc 
                    limit 
                        {1},{2}

                    ";

            string sqlList = "";
            string sqlCount = "";
            if (CarOwnerId == null)
            {
                var sqlAndParm = GenerateSearchWhereSql(search, listAgentId, listIssuingPerpleId, isAndOr, search.FromMethod);
                sqlList = string.Format(sql, sqlAndParm.Item1, (search.CurPage - 1) * search.PageSize, search.PageSize);
                sqlCount = string.Format("SELECT count(1) from dd_order left join dd_order_related_info on dd_order.id = dd_order_related_info.dd_order_id  where  {0}", sqlAndParm.Item1);
                return new SearchOrderDto
                {
                    ListOrder = context.Database.SqlQuery<DDOrder>(sqlList, sqlAndParm.Item2).ToList(),
                    TotalCount = context.Database.SqlQuery<int>(sqlCount, sqlAndParm.Item2).FirstOrDefault()
                };
            }
            else
            {
                sqlList = string.Format(sql, "dd_order.order_type!=3 and dd_order.CarOwnerId = " + CarOwnerId.ToString(), (search.CurPage - 1) * search.PageSize, search.PageSize);
                sqlCount = string.Format("SELECT count(1) from dd_order left join dd_order_related_info on dd_order.id = dd_order_related_info.dd_order_id  where dd_order.order_type!=3 and dd_order.CarOwnerId = " + CarOwnerId.ToString());
                return new SearchOrderDto
                {
                    ListOrder = context.Database.SqlQuery<DDOrder>(sqlList).ToList(),
                    TotalCount = context.Database.SqlQuery<int>(sqlCount).FirstOrDefault()
                };
            }

        }



        /// <summary>
        /// 生成订单查询的where部分的sql
        /// 这里考虑到以后要加上级代理人可以查看下级代理人的订单，所以业务员和出单员都是list的，并且保留了isAndOr
        /// </summary>
        /// <param name="search"></param>
        /// <param name="listAgentId">业务员集合</param>
        /// <param name="listIssuingPerpleId">出单员集合</param>
        /// <param name="isAndOr">业务员和出单员是and还是or的关系 1:and   2:or</param>
        /// <returns></returns>
        private Tuple<string, MySqlParameter[]> GenerateSearchWhereSql(BaseOrderSearchRequest search, List<int> listAgentId, List<int> listIssuingPerpleId, int isAndOr, int FromMethod)
        {
            List<MySqlParameter> param = new List<MySqlParameter>();
            var now = DateTime.Now;
            StringBuilder sb = new StringBuilder(" 1=1 ");
            #region 查询是否需要检查来源，判断是否过滤数据
            if (CheckFromMethod == "1")
            {
                //2017-11-01前 当请求来自APP端   不显示太保的数据   2018-05-09 添加显示太保数据
                if (FromMethod == 4)
                {
                    sb.Append(string.Format(" and  dd_order.source in (0,1,2,3) "));
                }
            }
            #endregion
            #region 业务员和出单员搜索，判断and还是or

            if (listAgentId.Count != 0 && listIssuingPerpleId.Count != 0)
            {
                if (isAndOr == 2)
                {
                    sb.Append(string.Format(" and  (dd_order.agent_id in ({0}) or dd_order.issuing_people_id in ({1}))", string.Join(",", listAgentId), string.Join(",", listIssuingPerpleId)));
                }
                else
                {
                    sb.Append(string.Format(" and  dd_order.agent_id in ({0}) and dd_order.issuing_people_id in ({1})", string.Join(",", listAgentId), string.Join(",", listIssuingPerpleId)));
                }
            }
            else
            {
                if (listAgentId.Count != 0)
                {
                    sb.Append(string.Format(" and dd_order.agent_id in ({0})", string.Join(",", listAgentId)));
                }
                if (listIssuingPerpleId.Count != 0)
                {
                    sb.Append(string.Format(" and dd_order.issuing_people_id in ({0})", string.Join(",", listIssuingPerpleId)));
                }
            }
            #endregion
            #region 投保人
            if (!string.IsNullOrWhiteSpace(search.HolderName))
            {
                sb.Append(" and  dd_order_related_info.holder_name=?holder_name");
                param.Add(new MySqlParameter
                {
                    Value = search.HolderName,
                    ParameterName = "holder_name",
                    MySqlDbType = MySqlDbType.VarChar
                });
            }
            #endregion

            #region 净费支付状态  客户付款状态
            if (search.ConsumerPayStatus != -1)
            {
                switch (search.ConsumerPayStatus)
                {
                    case 0:
                    case 1:
                        sb.Append(" and  dd_order.consumer_pay_status=?consumer_pay_status");
                        break;
                }
                param.Add(new MySqlParameter
                {
                    Value = search.ConsumerPayStatus,
                    ParameterName = "consumer_pay_status",
                    MySqlDbType = MySqlDbType.Int32
                });
            }
            #endregion
            #region 提交订单时间
            if (!string.IsNullOrEmpty(search.CreateBegainTime) && !string.IsNullOrWhiteSpace(search.CreateEndTime))
            {
                sb.Append(" and  dd_order.Create_Time BETWEEN ?CreateBegainTime AND ?CreateEndTime ");
                param.Add(new MySqlParameter
                {
                    Value = search.CreateBegainTime,
                    ParameterName = "CreateBegainTime",
                    MySqlDbType = MySqlDbType.DateTime
                });
                param.Add(new MySqlParameter
                {
                    Value = search.CreateEndTime,
                    ParameterName = "CreateEndTime",
                    MySqlDbType = MySqlDbType.DateTime
                });
            }
            #endregion
            #region 身份证验码状态 身份证采集状态
            if (search.IDCardVerifyStatus != -1)
            {
                sb.Append(" AND  dd_order.verification_code_status=?verification_code_status ");
                param.Add(new MySqlParameter
                {
                    Value = search.IDCardVerifyStatus,
                    ParameterName = "verification_code_status",
                    MySqlDbType = MySqlDbType.Int32
                });

                #region 已过期的逻辑，目前不用了
                //// 已经有采集状态了
                //if (search.IDCardVerifyStatus == 1)
                //{
                //    // 已验证，并且没有失效的
                //    sb.Append(" AND  dd_order.verification_code_status=?verification_code_status ");
                //    sb.Append(string.Format(" AND {0} =1 ", VERIFICATION_CODE_CREATE_TIME_IS_TODAY));
                //    param.Add(new MySqlParameter
                //    {
                //        Value = search.IDCardVerifyStatus,
                //        ParameterName = "verification_code_status",
                //        MySqlDbType = MySqlDbType.Int32
                //    });
                //}
                //else if (search.IDCardVerifyStatus == 2)
                //{
                //    // 由于目前没有写服务区更新库里身份证采集状态的值，所有库里是没有2（失效状态的），这里还需要从1（已验证）的状态中找失效的，所以目前将value写死成1；
                //    // 已失效的
                //    sb.Append(" AND  dd_order.verification_code_status=?verification_code_status ");
                //    sb.Append(string.Format(" AND {0} =0 ", VERIFICATION_CODE_CREATE_TIME_IS_TODAY));
                //    param.Add(new MySqlParameter
                //    {
                //        Value = 1,
                //        ParameterName = "verification_code_status",
                //        MySqlDbType = MySqlDbType.Int32
                //    });
                //}
                //else
                //{
                //    // 未采集
                //    sb.Append(" AND  dd_order.verification_code_status=?verification_code_status ");
                //    param.Add(new MySqlParameter
                //    {
                //        Value = search.IDCardVerifyStatus,
                //        ParameterName = "verification_code_status",
                //        MySqlDbType = MySqlDbType.Int32
                //    });
                //}

                #endregion
            }
            #endregion
            #region 支付保险公司全款状态
            if (search.InsuranceCompanyPayStatus != -1)
            {
                sb.Append(" and  dd_order.insurance_company_pay_status=?insurance_company_pay_status");
                param.Add(new MySqlParameter
                {
                    Value = search.InsuranceCompanyPayStatus,
                    ParameterName = "insurance_company_pay_status",
                    MySqlDbType = MySqlDbType.Int32
                });
            }
            #endregion
            #region 保险核心系统的报价录入时间/签单时间
            if (!string.IsNullOrWhiteSpace(search.IssueBegainTime) && !string.IsNullOrWhiteSpace(search.IssueEndTime))
            {
                sb.Append(" and  dd_order.issue_time BETWEEN ?IssueBegainTime AND ?IssueEndTime ");
                param.Add(new MySqlParameter
                {
                    Value = search.IssueBegainTime,
                    ParameterName = "IssueBegainTime",
                    MySqlDbType = MySqlDbType.DateTime
                });
                param.Add(new MySqlParameter
                {
                    Value = search.IssueEndTime,
                    ParameterName = "IssueEndTime",
                    MySqlDbType = MySqlDbType.DateTime
                });
            }
            #endregion
            #region 车牌号
            if (!string.IsNullOrWhiteSpace(search.LicenseNo))
            {
                sb.Append(" and  dd_order.licenseno=?licenseno");
                param.Add(new MySqlParameter
                {
                    Value = search.LicenseNo,
                    ParameterName = "licenseno",
                    MySqlDbType = MySqlDbType.VarChar
                });
            }
            #endregion
            #region 订单状态
            if (search.OrderStatus != -1)
            {

                if (search.OrderStatus == 1)
                {
                    // 已过期，0暂存、1已过期、3被踢回、41待支付、42待核保 才可能存在已过期的状态
                    sb.Append(string.Format(" and dd_order.order_type in (0,1,3,41,42) and dd_order.order_lapse_time <'{0}'", now.ToString()));
                }
                else if (search.OrderStatus == 2)
                {
                    sb.Append(" and  dd_order.order_type=?order_type");
                    param.Add(new MySqlParameter
                    {
                        Value = search.OrderStatus,
                        ParameterName = "order_type",
                        MySqlDbType = MySqlDbType.Int32
                    });
                }
                else if (search.OrderStatus == 5)
                {
                    sb.Append(" and  dd_order.order_type=?order_type");
                    param.Add(new MySqlParameter
                    {
                        Value = search.OrderStatus,
                        ParameterName = "order_type",
                        MySqlDbType = MySqlDbType.Int32
                    });
                }
                else
                {
                    sb.Append(" and  dd_order.order_type=?order_type");
                    param.Add(new MySqlParameter
                    {
                        Value = search.OrderStatus,
                        ParameterName = "order_type",
                        MySqlDbType = MySqlDbType.Int32
                    });
                    // 未过期
                    sb.Append(string.Format(" and dd_order.order_lapse_time >='{0}'", now.ToString()));
                }
            }
            #endregion
            #region 意向投保公司
            if (search.Source != -1)
            {
                sb.Append(" and  dd_order.source=?source");
                param.Add(new MySqlParameter
                {
                    Value = search.Source,
                    ParameterName = "source",
                    MySqlDbType = MySqlDbType.Int32
                });
            }
            #endregion
            return new Tuple<string, MySqlParameter[]>(sb.ToString(), param.ToArray());
        }

        #region wyy 2017-08-17 11:30 add
        /// <summary>
        /// 获取机采集的设备码
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="configId">渠道</param>
        /// <param name="source">保险公司</param>
        /// <returns></returns>
        public List<Machine> GetMachines(int agentId, int configId, int source)
        {
            try
            {
                var param = new List<MySqlParameter>();
                //var sqlQuery = "select Id as MachineId,machineCode,SaleChannel,InsuranceType,Remark from bx_busiusersetting where UserId in (select busiuser_id from bx_agent_busiuser where agent_id=?Agent and is_used=1) and IsAvailable=1 and InsuranceType in (1,2,3,10);";
                var sqlQuery = @"select  b.Id as MachineId,machineCode,SaleChannel,InsuranceType,Remark from bx_busiusersetting b
                              INNER JOIN bx_agent_busiuser ab on b.Id = ab.bx_busiusersetting_id
                              and agent_id =?Agent and ab.bx_agent_config_id =?ConfigId
                              and ab.source =?Source
                              and ab.is_used = 1;";
                param.Add(new MySqlParameter
                {
                    Value = agentId,
                    ParameterName = "Agent",
                    MySqlDbType = MySqlDbType.Int32
                });
                param.Add(new MySqlParameter
                {
                    Value = configId,
                    ParameterName = "ConfigId",
                    MySqlDbType = MySqlDbType.Int32
                });
                param.Add(new MySqlParameter
                {
                    Value = source,
                    ParameterName = "Source",
                    MySqlDbType = MySqlDbType.Int32
                });
                List<Machine> listuserinfo = context.Database.SqlQuery<Machine>(sqlQuery, param.ToArray()).ToList();
                return listuserinfo;
            }
            catch (Exception ex)
            {
                logError.Info("查看验证码的时间是否有效,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        public List<Machine> GetMachines(int agentId, int source)
        {
            try
            {
                var param = new List<MySqlParameter>();
                var sqlQuery = "select Id as MachineId,machineCode,SaleChannel,InsuranceType,Remark from bx_busiusersetting where UserId in (select busiuser_id from bx_agent_busiuser where agent_id=?Agent and is_used=1) and IsAvailable=1 and InsuranceType=?source;";
                param.Add(new MySqlParameter
                {
                    Value = agentId,
                    ParameterName = "Agent",
                    MySqlDbType = MySqlDbType.Int32
                });
                param.Add(new MySqlParameter
                {
                    Value = source,
                    ParameterName = "source",
                    MySqlDbType = MySqlDbType.Int32
                });
                List<Machine> listuserinfo = context.Database.SqlQuery<Machine>(sqlQuery, param.ToArray()).ToList();
                return listuserinfo;
            }
            catch (Exception ex)
            {
                logError.Info("查看验证码的时间是否有效,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 更新验证码的有效时间
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        public bool UpdateOrderVerificationDate(string orderNum)
        {
            try
            {
                var modl = context.dd_order.Where(s => s.order_num == orderNum.Trim()).FirstOrDefault();
                if (modl == null)
                    return false;
                modl.verification_code_create_time = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("更新验证码的有效时间,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 身份证验码状态 0未采集 1已验证 2已失效
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="status">状态 0未采集 1已验证 2已失效</param>
        /// <returns></returns>
        public bool UpdateVerificationCodeStatus(string orderNum, int status)
        {
            try
            {
                var modl = context.dd_order.Where(s => s.order_num == orderNum.Trim()).FirstOrDefault();
                if (modl == null)
                    return false;
                modl.verification_code_status = status;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("更改身份证采集状态,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 查看验证码的时间是否有效
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        public bool CheckOrderVerificationDate(string orderNum)
        {
            try
            {
                var model = context.dd_order.Where(s => s.order_num == orderNum.Trim()).FirstOrDefault();
                if (model == null)
                    return false;
                //两个小时 117分钟
                if (model.verification_code_create_time < DateTime.Now.AddMinutes(-117))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("查看验证码的时间是否有效,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 获取支付状态
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="type">支付类型 1=全款支付 、2=净费支付</param>
        /// <returns></returns>
        public dd_order_paymentresult GetOrderPayResult(string orderNum, int type)
        {
            try
            {
                return context.dd_order_paymentresult.FirstOrDefault(s => s.order_num == orderNum.Trim() && s.type == type);
            }
            catch (Exception ex)
            {
                logError.Info("获取支付状态,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 添加支付结果
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertOrderPayResult(dd_order_paymentresult model)
        {
            try
            {
                context.dd_order_paymentresult.Add(model);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("添加支付结果,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 通过代理人获取绑定的采集系统账号
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_agent_busiuser> GetBusiuserIdsByAgentId(int agentId)
        {
            try
            {
                return context.bx_agent_busiuser.Where(x => x.agent_id == agentId && x.is_used == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("通过代理人获取绑定的采集系统账号,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 修改订单支付维码创建时间及地址
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="qrUrl">二维码地址</param>
        /// <returns></returns>
        public bool ModifyOrderQR(string orderNum, string qrUrl)
        {
            try
            {
                var model = context.dd_order.FirstOrDefault(o => o.order_num == orderNum.Trim());
                if (model == null)
                    return false;
                //获取支付信息时间
                model.pay_code_create_time = DateTime.Now;
                //支付链接地址（微信）
                model.pay_code_url = qrUrl;
                //订单失效时间
                var endDate = DateTime.Parse(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), "23:45:00"));
                //判断失效时间(太平洋)
                if (model.order_lapse_time > endDate && model.source == 1)
                    model.order_lapse_time = endDate;

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {    //DbEntityValidationException ex
                logError.Info("修改订单支付维码创建时间及地址,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 根据订单获取支付的二维码
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="qrUrl">二维码地址</param>
        /// <returns></returns>
        public string GetQROrderQR(string orderNum)
        {
            try
            {
                var model = context.dd_order.Where(o => o.order_num == orderNum).FirstOrDefault();
                if (model == null)
                    return null;
                //两个小时 117分钟
                if (model.pay_code_create_time < DateTime.Now.AddMinutes(-117))
                    return null;
                //支付的二维码
                return model.pay_code_url;
            }
            catch (Exception ex)
            {
                logError.Info("根据订单获取支付的二维码,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 根据订单修改支付状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payType">支付类型 1=净费支付 2=全款支付</param>
        ///  <param name="payStatus">支付状态 0=未支付  1=已支付</param>
        /// <returns></returns>
        public bool ModifyOrderPayStatus(string orderNum, int payType, int payStatus)
        {
            try
            {
                var model = context.dd_order.FirstOrDefault(o => o.order_num == orderNum.Trim());
                if (model == null)
                    return false;
                if (payType == 1)
                {
                    model.consumer_pay_status = payStatus;
                }
                else
                {
                    model.insurance_company_pay_status = payStatus;
                    model.order_type = 5;
                }

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("根据订单修改支付状态,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        public bool ModifyOrder(dd_order model)
        {
            try
            {
                if (model != null)
                {
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("根据订单修改支付状态,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 添加orderSerial
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertOrderSerial(dd_order_serial model)
        {
            try
            {
                //var date = DateTime.Now;
                //var obj = context.dd_order_serial.Where(x => x.order_num.Equals(model.order_num) && x.failure_time > date).FirstOrDefault();
                // if (obj == null)
                context.dd_order_serial.Add(model);
                //else
                //    obj.update_time = date;
                //提交
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("添加支付流水,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取该订单的最近一条支付流水
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        public dd_order_serial FindOrderSerial(string orderNum)
        {
            try
            {
                return context.dd_order_serial.Where(s => s.order_num == orderNum.Trim()).OrderByDescending(x => x.id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("获取该订单的最近一条支付流水,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// 修改支付流水结果
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="payResult">状态 0=初始、1=成功</param>
        /// <returns></returns>
        public bool ModifyOrderSerialPayResult(string orderNum, int payResult)
        {
            try
            {
                var model = context.dd_order_serial.Where(s => s.order_num == orderNum.Trim()).OrderByDescending(x => x.create_time).FirstOrDefault();
                if (model != null)
                {
                    model.status = payResult;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("修改支付流水结果,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 修改支付流水结果
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="payResult">状态 0=初始、1=成功</param>
        /// <returns></returns>
        public bool ModifyOrderSerialPayResult(dd_order_serial model)
        {
            try
            {
                if (model != null)
                {
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("修改支付流水结果,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
            return true;
        }

        public List<PayWayBanksModel> GetPayWayBanks(PayWayBanksModel model)
        {
            try
            {
                var sqlQuery = new StringBuilder();
                if (model.CityId > 0)
                    sqlQuery.Append(string.Format(" AND city_id={0}", model.CityId));
                if (!string.IsNullOrWhiteSpace(model.PayWay))
                    sqlQuery.Append(string.Format(" AND pay_way='{0}'", model.PayWay));

                var sql = string.Format(@"SELECT w.id as Id,city_id as CityId,pay_way as PayWay,bank_id as BankId,bank_name as BankName FROM  dd_pay_way as w
                                          INNER JOIN dd_pay_bank as b on w.pay_bank_id=b.id where invalid=1 {0};", sqlQuery);

                return DataContextFactory.GetDataContext().Database.SqlQuery<PayWayBanksModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("获取支付方式合作银行,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">dd_pay_way.id</param>
        /// <returns></returns>
        public PayWayBanksModel GetPayWayBank(int id)
        {
            try
            {
                var query = from pb in context.dd_pay_bank
                            join pw in context.dd_pay_way on pb.id equals pw.pay_bank_id
                            where pw.id == id
                            select new
                            {
                                pw.id,
                                pw.pay_way,
                                pb.bank_id,
                                pb.bank_name,
                                pb.city_id
                            };
                if (!query.Any())
                    return null;
                var model = query.FirstOrDefault();
                var pbModel = new PayWayBanksModel();
                pbModel.BankId = model.bank_id;
                pbModel.BankName = model.bank_name;
                pbModel.PayWay = model.pay_way;
                pbModel.Id = model.id;
                pbModel.CityId = model.city_id;
                return pbModel;
            }
            catch (Exception ex)
            {
                logError.Info("获取支付方式合作银行,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 获取最近的一次采集信息记录
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public dd_order_collection GetOrdderCollection(string orderNum, int? status)
        {
            try
            {
                if (status.HasValue)
                {
                    var query = from oc in context.dd_order_collection
                                where oc.order_num == orderNum && oc.status == status
                                orderby oc.id descending
                                select oc;
                    return query.FirstOrDefault();
                }
                else
                {
                    var query = from oc in context.dd_order_collection
                                where oc.order_num == orderNum
                                select oc;
                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logError.Info("获取最近的一次采集信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// 添加次采集信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        public int InsertOrdderCollection(dd_order_collection model)
        {
            try
            {
                context.dd_order_collection.Add(model);
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("添加次采集信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 修改次采集信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        public int ModifyOrdderCollection(dd_order_collection model)
        {
            try
            {
                if (model == null)
                    return 0;
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("修改次采集信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 添加安心支付信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        public dd_pay_ax InsertPayAx(dd_pay_ax model)
        {
            try
            {
                var o = context.dd_pay_ax.Add(model);
                context.SaveChanges();
                return o;
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
                throw dbEx;
            }
            catch (Exception ex)
            {

                logError.Info("添加安心支付信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 修改安心支付信息信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        public int ModifyPayAx(dd_pay_ax model)
        {
            try
            {
                if (model == null)
                    return 0;
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("修改安心支付信息信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 获取最新的一条记录
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="transNo">商户订单号</param>
        /// <returns></returns>
        public dd_pay_ax GetPayAx(string transNo)
        {
            try
            {
                var query = from oc in context.dd_pay_ax
                            where oc.trans_no.Equals(transNo)
                            orderby oc.id descending
                            select oc;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("获取最近的一条安心支付记录信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        /// <summary>
        /// 获取最新的一条记录
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="transNo">商户订单号</param>
        /// <returns></returns>
        public dd_pay_ax GetPayAx(long buId, string orderNum)
        {
            try
            {
                var query = new dd_pay_ax();
                if (string.IsNullOrWhiteSpace(orderNum))
                {
                    query = (from oc in context.dd_pay_ax
                             where oc.b_uid == buId
                             orderby oc.id descending
                             select oc).FirstOrDefault();
                }
                else
                {
                    query = (from oc in context.dd_pay_ax
                             where oc.b_uid == buId && oc.attach.Equals(orderNum)
                             orderby oc.id descending
                             select oc).FirstOrDefault();
                }
                return query;
            }
            catch (Exception ex)
            {
                logError.Info("获取最近的一条安心支付记录信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        public dd_pay_ax GetPayAxByOrderNo(string orderNo, long buId = 0)
        {
            try
            {
                var query = new dd_pay_ax();
                if (buId <= 0)
                {
                    query = (from oc in context.dd_pay_ax
                             where oc.orderNo.Equals(orderNo)
                             orderby oc.id descending
                             select oc).FirstOrDefault();
                }
                else
                {
                    query = (from oc in context.dd_pay_ax
                             where oc.b_uid == buId && oc.orderNo.Equals(orderNo)
                             orderby oc.id descending
                             select oc).FirstOrDefault();
                }
                return query;
            }
            catch (Exception ex)
            {
                logError.Info("获取最近的一条安心支付记录信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }
        /// <summary>
        /// bx_anxin_delivery
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="transNo">商户订单号</param>
        /// <returns></returns>
        public bx_anxin_delivery GetAnxinDelivery(long buId)
        {
            try
            {
                var query = from oc in context.bx_anxin_delivery
                            where oc.b_uid == buId && oc.status == 1
                            orderby oc.id descending
                            select oc;
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("获取最近的一条安心支付记录信息记录,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                throw ex;
            }
        }

        #endregion
        public async Task<bx_busiuser> GetBusiuserByUserNamePwdAsync(string userName, string pwd)
        {
            return await context.bx_busiuser.FirstOrDefaultAsync(o => o.Username == userName && o.Password == pwd && o.IsAvailable == 1);
        }
        public async Task<bool> BandAgentBusiuserAsync(List<bx_agent_busiuser> listAgentBusiuser)
        {
            foreach (var item in listAgentBusiuser)
            {
                context.bx_agent_busiuser.Add(item);
            }
            return (await context.SaveChangesAsync()) > 0;
        }
        public bool UpdateAgentBusiuser(bx_agent_busiuser agentBusiuser)
        {
            context.Entry(agentBusiuser).State = System.Data.Entity.EntityState.Modified;
            return context.SaveChanges() > 0;
        }
        public async Task<bx_agent_busiuser> FindAgentBusiuserAsync(int agentBusiuserId)
        {
            return await context.bx_agent_busiuser.FirstOrDefaultAsync(o => o.id == agentBusiuserId && o.is_used == 1);
        }
        public List<ListBusiuserDto> GetBusiuserByAgent(int agent)
        {
            var sql = @"
            SELECT busiuser.id as AgentBusiuserId
                ,busiuser.source
                ,setting.machineCode
                ,config.config_name as ConfigName
            FROM 
                bx_agent_busiuser AS busiuser 
            LEFT JOIN 
                bx_busiusersetting AS setting ON busiuser.bx_busiusersetting_id=setting.id
            LEFT JOIN 
                bx_agent_config AS config ON busiuser.bx_agent_config_id=config.id
            WHERE 
                busiuser.agent_id=?agent_id
                AND busiuser.is_used=1;
                        ";
            var param = new MySqlParameter[] {
                new MySqlParameter
                {
                    Value=agent,
                    ParameterName="agent_id",
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return context.Database.SqlQuery<ListBusiuserDto>(sql, param).ToList();
        }
        public long CreateOrderDetail(dd_order order, dd_order_related_info orderRelatedInfo, bx_userinfo userinfo, bx_savequote savequote, bx_submit_info submitInfo, bx_quoteresult quoteresult, bx_quoteresult_carinfo carInfo, out CreateOrderReturnModel model)
        {
            model = new CreateOrderReturnModel();
            //如果此四张表数据为空，提示插入失败
            if (userinfo == null || savequote == null || submitInfo == null || order == null)
            {
                return 0;
            }
            //返回订单时间
            long orderid = 0;
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    dd_order neworder = new dd_order();
                    order.PayMentRemark = order.PayMentRemark ?? "";
                    //插入订单
                    if (order.id > 0)
                    {
                        DataContextFactory.GetDataContext().dd_order.AddOrUpdate(order);
                        int countOrder = DataContextFactory.GetDataContext().SaveChanges();
                        orderid = order.id;
                    }
                    else
                    {
                        neworder = DataContextFactory.GetDataContext().dd_order.Add(order);
                        DataContextFactory.GetDataContext().SaveChanges();
                        orderid = neworder.id;
                    }
                    if (orderid > 0)
                    {
                        //插入关系人表
                        orderRelatedInfo.dd_order_id = orderid;
                        DataContextFactory.GetDataContext().dd_order_related_info.AddOrUpdate(orderRelatedInfo);
                        DataContextFactory.GetDataContext().SaveChanges();
                        //保存userinfo
                        #region holder关系人
                        userinfo.HolderName = orderRelatedInfo.holder_name;
                        userinfo.HolderIdType = orderRelatedInfo.holder_id_type;
                        userinfo.HolderIdCard = orderRelatedInfo.holder_id_card;
                        userinfo.HolderSex = orderRelatedInfo.holder_sex;
                        userinfo.HolderNation = orderRelatedInfo.holder_nation;
                        userinfo.HolderBirthday = orderRelatedInfo.holder_birthday.HasValue ? orderRelatedInfo.holder_birthday.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.HolderAddress = orderRelatedInfo.holder_address;
                        userinfo.HolderCertiStartdate = orderRelatedInfo.holder_certi_start_date.HasValue ? orderRelatedInfo.holder_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.HolderCertiEnddate = orderRelatedInfo.holder_certi_end_date.HasValue ? orderRelatedInfo.holder_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.HolderIssuer = orderRelatedInfo.holder_authority;
                        userinfo.HolderMobile = orderRelatedInfo.holder_mobile;
                        userinfo.HolderEmail = orderRelatedInfo.holder_email;
                        #endregion
                        #region owner关系人
                        userinfo.LicenseOwner = orderRelatedInfo.ower_name;
                        userinfo.OwnerIdCardType = orderRelatedInfo.ower_id_type.HasValue ? orderRelatedInfo.ower_id_type.Value : 0;
                        userinfo.IdCard = orderRelatedInfo.ower_id_card;
                        userinfo.OwnerSex = orderRelatedInfo.ower_sex;
                        userinfo.OwnerNation = orderRelatedInfo.ower_nation;
                        userinfo.OwnerBirthday = orderRelatedInfo.ower_birthday.HasValue ? orderRelatedInfo.ower_birthday.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.Address = orderRelatedInfo.ower_address;
                        userinfo.OwnerCertiStartdate = orderRelatedInfo.ower_certi_start_date.HasValue ? orderRelatedInfo.ower_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.OwnerCertiEnddate = orderRelatedInfo.ower_certi_end_date.HasValue ? orderRelatedInfo.ower_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.OwnerIssuer = orderRelatedInfo.ower_authority;
                        userinfo.Mobile = orderRelatedInfo.ower_mobile;
                        userinfo.Email = orderRelatedInfo.ower_email;
                        #endregion
                        #region insured关系人
                        userinfo.InsuredName = orderRelatedInfo.insured_name;
                        userinfo.InsuredIdType = orderRelatedInfo.insured_id_type;
                        userinfo.InsuredIdCard = orderRelatedInfo.insured_id_card;
                        userinfo.InsuredSex = orderRelatedInfo.insured_sex;
                        userinfo.InsuredNation = orderRelatedInfo.insured_nation;
                        userinfo.InsuredBirthday = orderRelatedInfo.insured_birthday.HasValue ? orderRelatedInfo.insured_birthday.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.InsuredAddress = orderRelatedInfo.insured_address;
                        userinfo.InsuredCertiStartdate = orderRelatedInfo.insured_certi_start_date.HasValue ? orderRelatedInfo.insured_certi_start_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.InsuredCertiEnddate = orderRelatedInfo.insured_certi_end_date.HasValue ? orderRelatedInfo.insured_certi_end_date.Value.ToString("yyyy-MM-dd") : "";
                        userinfo.InsuredIssuer = orderRelatedInfo.insured_authority;
                        userinfo.InsuredMobile = orderRelatedInfo.insured_mobile;
                        userinfo.InsuredEmail = orderRelatedInfo.insured_email;
                        #endregion
                        userinfo.UpdateTime = DateTime.Now;
                        DataContextFactory.GetDataContext().bx_userinfo.AddOrUpdate(userinfo);
                        var rw_userinfo = DataContextFactory.GetDataContext().SaveChanges();
                        //保存quoteresult
                        if (quoteresult != null)
                        {
                            if (quoteresult.Id > 0 && quoteresult.B_Uid.HasValue)
                            {
                                #region CarOrderQuoteResultRepository
                                dd_order_quoteresult orderQuoteresult = new dd_order_quoteresult()
                                {
                                    B_Uid = quoteresult.B_Uid.Value,
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
                                    HcXiuLiChangType = quoteresult.HcXiuLiChangType,
                                    CheSunBE = quoteresult.CheSunBE,
                                    ZiRanBE = quoteresult.ZiRanBE,
                                    DaoQiangBE = quoteresult.DaoQiangBE,
                                    BuJiMianSheBeiSunshi = quoteresult.BuJiMianSheBeiSunshi,
                                    HcFeiYongBuChangDays = quoteresult.HcFeiYongBuChangDays,
                                    PingAnScore = quoteresult.PingAnScore,
                                    TotalRate = quoteresult.TotalRate,
                                    //end新增
                                    dd_order_id = orderid,
                                    //2018-7-30新增
                                    SanZheJieJiaRi = quoteresult.SanZheJieJiaRi
                                };
                                #endregion
                                model.ddorderquoteresult = orderQuoteresult;
                                var re_oqr = DataContextFactory.GetDataContext().dd_order_quoteresult.Add(orderQuoteresult);
                            }
                        }

                        //保存savequote
                        if (savequote.Id > 0 && savequote.B_Uid.HasValue)
                        {
                            #region CarOrderSaveQuoteRepository
                            dd_order_savequote orderSavequote = new dd_order_savequote()
                            {
                                B_Uid = savequote.B_Uid.Value,
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
                                dd_order_id = orderid,
                                //2018-7-30新增
                                SanZheJieJiaRi = savequote.SanZheJieJiaRi
                            };
                            #endregion
                            model.ddordersavequote = orderSavequote;
                            var re_osq = DataContextFactory.GetDataContext().dd_order_savequote.Add(orderSavequote);
                        }
                        DataContextFactory.GetDataContext().SaveChanges();
                        scope.Complete();

                        model.ddorder = order;
                        if (model.ddorder != null) {
                            model.ddorder.id = orderid;
                        }

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

        public int AddOrderSteps(dd_order_steps orderSteps)
        {
            DataContextFactory.GetDataContext().dd_order_steps.Add(orderSteps);
            int orderstepsid = DataContextFactory.GetDataContext().SaveChanges();
            return orderstepsid;
        }

        public List<dd_order_steps> GetOrderSteps(long[] orderIds)
        {
            List<dd_order_steps> list = DataContextFactory.GetDataContext().dd_order_steps.Where(l => orderIds.Contains(l.order_id)).ToList();
            return list;
        }

        public List<dd_order_steps> GetOrderSteps(long orderId)
        {
            List<dd_order_steps> list = DataContextFactory.GetDataContext().dd_order_steps.Where(l => l.order_id == orderId).ToList();
            return list;
        }

        public async Task<List<dd_order_steps>> GetOrderStepsAsync(long orderId)
        {
            var sql = String.Format(@"SELECT * FROM dd_order_steps WHERE order_id = {0} ORDER BY Id DESC ", orderId);
            List<dd_order_steps> model =
                context.Database.SqlQuery<dd_order_steps>(sql).ToList();
            return model;
        }

        public async Task<bool> IsExistPaymentResultAsync(long orderId, int type)
        {
            return await context.dd_order_paymentresult.AnyAsync(o => o.order_id == orderId && o.type == type);
        }

        public async Task<List<BusiuserSettingDto>> GetBusiuserListAsync(int busiuserId, int agentId)
        {
            var sql = @"
                SELECT
                    setting.Id,
                    setting.InsuranceType,
                    setting.machineCode
                FROM bx_busiusersetting AS setting
                WHERE setting.UserId =?busiuserId 
                    AND setting.IsAvailable = 1
                    AND setting.Id NOT IN(SELECT
                                            bususer.bx_busiusersetting_id
                                            FROM bx_agent_busiuser AS bususer
                                            WHERE bususer.agent_id = ?agentId
                                                AND bususer.is_used = 1);
                ";
            var param = new MySqlParameter[] {
                new MySqlParameter()
                {
                    MySqlDbType=MySqlDbType.Int32,
                    ParameterName="busiuserId",
                    Value=busiuserId
                },
                new MySqlParameter()
                {
                    MySqlDbType=MySqlDbType.Int32,
                    ParameterName="agentId",
                    Value=agentId
                }
            };
            return await context.Database.SqlQuery<BusiuserSettingDto>(sql, param).ToListAsync();
        }

        public async Task<List<BandBusiuserSettingDto>> GetBusiusersettingPartialAsync(List<int> listId)
        {
            var strId = string.Join(",", listId);
            var sql = string.Format("SELECT id,InsuranceType,userid as BusiuserId FROM bx_busiusersetting WHERE id IN ({0})", strId);
            return await context.Database.SqlQuery<BandBusiuserSettingDto>(sql).ToListAsync();
        }

        public async Task<bx_busiusersetting> GetBusiusersettingAsync(int id)
        {
            return await context.bx_busiusersetting.Where(o => o.Id == id && o.IsAvailable == 1).FirstOrDefaultAsync();
        }

        public OrderInformationViewModel GetOrderInformation(string sql)
        {

            var renewalInformation = new OrderInformationViewModel() { BusinessStatus = 1, StatusMessage = "查询信息成功" };
            string connctionStr = DataContextFactory.GetDataContext().Database.Connection.ConnectionString;
            var dataSet = MySqlHelper.ExecuteDataset(connctionStr, sql);

            string orderQuoteresultStr = CommonHelper.TToJson(dataSet.Tables[0]);
            string orderSavequoteStr = CommonHelper.TToJson(dataSet.Tables[1]);
            string submitInfoStr = CommonHelper.TToJson(dataSet.Tables[2]);
            string orderRelatedInfoStr = CommonHelper.TToJson(dataSet.Tables[3]);
            string listoOrderPaymentresultsStr = CommonHelper.TToJson(dataSet.Tables[4]);
            string quotereqCarinfoStr = CommonHelper.TToJson(dataSet.Tables[5]);
            string listOrderStepsesStr = CommonHelper.TToJson(dataSet.Tables[6]);
            string agentConfigStr = CommonHelper.TToJson(dataSet.Tables[7]);
            string agentStr = CommonHelper.TToJson(dataSet.Tables[9]);
            string listCommissions = CommonHelper.TToJson(dataSet.Tables[8]);

            renewalInformation.OrderQuoteresult =
                orderQuoteresultStr.ToListT<dd_order_quoteresult>().FirstOrDefault();
            renewalInformation.OrderSavequote =
                orderSavequoteStr.ToListT<dd_order_savequote>().FirstOrDefault();
            renewalInformation.SubmitInfo =
                submitInfoStr.ToListT<bx_submit_info>().FirstOrDefault();
            renewalInformation.OrderRelatedInfo =
                orderRelatedInfoStr.ToListT<dd_order_related_info>().FirstOrDefault();
            renewalInformation.ListoOrderPaymentresults =
                listoOrderPaymentresultsStr.ToListT<dd_order_paymentresult>();
            renewalInformation.QuotereqCarinfo =
                quotereqCarinfoStr.ToListT<bx_quotereq_carinfo>().FirstOrDefault();
            renewalInformation.ListOrderStepses =
                listOrderStepsesStr.ToListT<dd_order_steps>();
            renewalInformation.AgentConfig =
                agentConfigStr.ToListT<bx_agent_config>().FirstOrDefault();
            renewalInformation.ListAgent =
                agentStr.ToListT<bx_agent>();
            renewalInformation.ListCommissions =
                listCommissions.ToListT<dd_order_commission>();

            return renewalInformation;
        }

        /// <summary>
        /// 只展示北京的渠道
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="source"></param>
        /// <param name="agentBusiuserId"></param>
        /// <returns></returns>
        public List<BusiuserAgentConfigDto> GetCanUseAgentConfig(int agent, int source, int agentBusiuserId)
        {
            var sql = @"
SELECT
  id,
  config_name
FROM bx_agent_config
WHERE agent_id = ?agent_id
    AND source = ?source
    AND is_used = 1
    AND city_id = 1
    AND id NOT IN(SELECT
                    bx_agent_config_id
                  FROM bx_agent_busiuser
                  WHERE agent_id = ?agent_id
                      AND is_used = 1
                      AND id != ?agentBusiuserId)
";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="agent_id",
                    Value=agent,
                    MySqlDbType=MySqlDbType.Int32
                },
                new MySqlParameter
                {
                    ParameterName="source",
                    Value=source,
                    MySqlDbType=MySqlDbType.Int32
                },
                new MySqlParameter
                {
                    ParameterName="agentBusiuserId",
                    Value=agentBusiuserId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return context.Database.SqlQuery<BusiuserAgentConfigDto>(sql, param).ToList();
        }

        public List<OrderAgentAmountViewModel> GetTeamOrder(string sql)
        {
            return context.Database.SqlQuery<OrderAgentAmountViewModel>(sql).ToList();
        }

        /// <summary>
        /// 修改订单配送信息 2018-09-14 张克亮
        /// </summary>
        /// <param name="request">修改订单请求模型</param>
        /// <returns></returns>
        public int UpdateOrderDeliveryInfo(OrderDeliveryInfoRequest request)
        {
            int executeRows = 0;

            try
            {
                //更新SQL语句
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("update dd_order set ");
                sbSql.Append(" delivery_method=@deliveryMethod,");
                sbSql.Append(" delivery_address=@deliveryAddress,");
                sbSql.Append(" delivery_contacts=@deliveryContacts,");
                sbSql.Append(" delivery_contacts_mobile=@deliveryContactsMobile,");
                sbSql.Append(" delivery_address_id=@deliveryAddressId ");
                sbSql.Append(" where order_num=@orderNum and ");
                sbSql.Append(" agent_id=@agentId");

                #region 赋值更新参数
                MySqlParameter[] sqlParams = new MySqlParameter[7];
                sqlParams[0] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "deliveryMethod",
                    Value = request.DeliveryMethod
                };
                sqlParams[1] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "deliveryAddress",
                    Value = request.DeliveryAddress
                };
                sqlParams[2] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "deliveryContacts",
                    Value = request.DeliveryContacts
                };
                sqlParams[3] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "deliveryContactsMobile",
                    Value = request.DeliveryContactsMobile
                };
                sqlParams[4] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "deliveryAddressId",
                    Value = request.DeliveryAddressId
                };
                sqlParams[5] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "orderNum",
                    Value = request.OrderNum
                };
                sqlParams[6] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = request.AgentId
                };
                #endregion

                executeRows = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sbSql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return executeRows;
        }

        /// <summary>
        /// 获取订单配送信息 2018-09-17 张克亮
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <returns></returns>
        public OrderDeliveryInfoResponse GetOrderDeliveryInfo(string orderNum)
        {
            OrderDeliveryInfoResponse response = new OrderDeliveryInfoResponse();

            try
            {
                //订单表关联配送地址表查询订单下的配送信息
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("SELECT ");
                sbSql.Append("o.order_num AS OrderNum,");//订单号
                sbSql.Append("o.agent_id AS AgentId,");//经济人id
                sbSql.Append("o.delivery_address_id AS DeliveryAddressId,");//配送地址ID
                sbSql.Append("o.delivery_method AS DeliveryMethod,");//配送方法
                sbSql.Append("a.Name AS DeliveryContacts,");//配送人
                sbSql.Append("a.phone AS DeliveryContactsMobile,");//配送人联系方式
                sbSql.Append("a.address AS DeliveryAddress,");//配送详细地址
                sbSql.Append("a.provinceId AS ProvinceId,");//省ID
                sbSql.Append("a.cityId AS CityId,");//市ID
                sbSql.Append("a.areaId AS AreaId,");//地区ID
                sbSql.Append("a.province_name AS ProvinceName,");//省名称
                sbSql.Append("a.city_name AS CityName,");//市名称
                sbSql.Append("a.area_name AS AreaName ");//地区名称
                sbSql.Append("FROM dd_order AS o,bx_address AS a ");
                sbSql.Append("WHERE o.delivery_address_id=a.id ");
                sbSql.Append("AND a.status=1 ");//状态为可用的
                sbSql.Append("AND order_num=@orderNum");

                MySqlParameter[] sqlParams = new MySqlParameter[1];
                sqlParams[0] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "orderNum",
                    Value = orderNum
                };

                response = DataContextFactory.GetDataContext().Database.SqlQuery<OrderDeliveryInfoResponse>(sbSql.ToString(), sqlParams).FirstOrDefault();

            }
            catch (Exception ex)
            {
                response = null;
                throw ex;
            }

            return response;
        }

        public List<dd_order_quoteresult> GetOrderQuoteResultListByOrderId(List<long> orderIds)
        {
            return DataContextFactory.GetDataContext().dd_order_quoteresult.Where(a => orderIds.Contains(a.dd_order_id)).ToList();
        }
    }
}
