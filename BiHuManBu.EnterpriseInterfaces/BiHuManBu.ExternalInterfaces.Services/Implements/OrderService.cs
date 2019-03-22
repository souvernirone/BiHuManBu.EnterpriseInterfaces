using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using log4net;
using ServiceStack.Text;
using EnumSourceNew = BiHuManBu.ExternalInterfaces.Models.ViewModels.EnumSourceNew;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class OrderService : CommonBehaviorService, IOrderService
    {
        #region 声明
        private Models.AppIRepository.IOrderRepository _orderRepository;
        private ILog _logInfo = LogManager.GetLogger("INFO");
        private ILog _logError = LogManager.GetLogger("ERROR");
        private Models.AppIRepository.IAgentRepository _agentRepository;
        private IUserService _userService;
        private ILastInfoRepository _lastInfoRepository;
        private Models.AppIRepository.IUserInfoRepository _userInfoRepository;
        private IQuoteResultRepository _quoteResultRepository;
        private ISaveQuoteRepository _saveQuoteRepository;
        private ISubmitInfoRepository _submitInfoRepository;
        private IQuoteReqCarInfoRepository _quoteReqCarinfoRepository;
        private IAddressRepository _addressRepository;
        private IUserClaimRepository _userClaimRepository;
        private IQuoteResultCarinfoRepository _carinfoRepository;
        public OrderService(Models.AppIRepository.IOrderRepository orderRepository,
            Models.AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper, IUserService userService,
            Models.AppIRepository.IUserInfoRepository userInfoRepository, ILastInfoRepository lastInfoRepository,
            IQuoteResultRepository quoteResultRepository, ISaveQuoteRepository saveQuoteRepository,
            ISubmitInfoRepository submitInfoRepository, IQuoteReqCarInfoRepository quoteReqCarinfoRepository,
            IAddressRepository addressRepository, IUserClaimRepository userClaimRepository, IQuoteResultCarinfoRepository carinfoRepository)
            : base(agentRepository, cacheHelper)
        {
            _orderRepository = orderRepository;
            _agentRepository = agentRepository;
            _userService = userService;
            _lastInfoRepository = lastInfoRepository;
            _userInfoRepository = userInfoRepository;
            _quoteResultRepository = quoteResultRepository;
            _saveQuoteRepository = saveQuoteRepository;
            _submitInfoRepository = submitInfoRepository;
            _quoteReqCarinfoRepository = quoteReqCarinfoRepository;
            _addressRepository = addressRepository;
            _userClaimRepository = userClaimRepository;
            _carinfoRepository = carinfoRepository;
        }
        #endregion

        /// <summary>
        /// 创建订单（同时复制历时的人员基础信息、险种信息、报价结果、核保状态）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public async Task<CreateOrderResponse> NewCreateOrder(Messages.Request.AppRequest.CreateOrderRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            Messages.Response.AppResponse.CreateOrderResponse response = new Messages.Response.AppResponse.CreateOrderResponse();
            //根据经纪人获取手机号 
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.Status = HttpStatusCode.NotFound;
                return response;
            }
            //if (!ValidateReqest(pairs, agentModel.SecretKey, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            try
            {
                int sourceValue = 0;
                if (request.IsNewSource.HasValue)
                {
                    sourceValue = (int)(request.IsNewSource == 1 ? SourceGroupAlgorithm.GetOldSource(request.Source) : request.Source);
                }
                else
                {
                    sourceValue = (int)request.Source;
                }

                bx_userinfo userinfo = _userInfoRepository.FindByBuid(request.Buid);
                if (userinfo == null)
                {
                    response.Status = HttpStatusCode.ExpectationFailed;
                    return response;
                }
                bx_lastinfo lastinfo = _lastInfoRepository.GetByBuid(request.Buid);
                bx_savequote savequote = _saveQuoteRepository.GetSavequoteByBuid(request.Buid);
                bx_submit_info submitInfo = _submitInfoRepository.GetSubmitInfo(request.Buid, sourceValue);
                bx_quoteresult quoteresult = _quoteResultRepository.GetQuoteResultByBuid(request.Buid, sourceValue);
                bx_quoteresult_carinfo carInfo = _carinfoRepository.Find(request.Buid, sourceValue);
                List<bx_claim_detail> claimDetails = _userClaimRepository.FindList(request.Buid);
                //取商业险和交强险开始时间
                var qrStartDate = userinfo.QuoteStatus > 0 ? _quoteResultRepository.GetStartDate(userinfo.Id) : new InsuranceStartDate();

                bx_car_order exitCarOrder = _orderRepository.FindBy(userinfo.LicenseNo, request.OpenId, request.Topagent);
                if (exitCarOrder != null)
                {
                    exitCarOrder.order_status = -1; //删除订单
                    _orderRepository.Update(exitCarOrder);
                }

                //根据openid，mobile，获取userid
                var user = _userService.AddUser(request.OpenId, request.Mobile);

                #region 地址

                int addressid = request.AddressId.HasValue ? request.AddressId.Value : 0;

                var address = new bx_address();
                if (addressid == 0)
                {
                    if (!string.IsNullOrEmpty(request.DistributionAddress) &&
                        !string.IsNullOrEmpty(request.DistributionName) &&
                        !string.IsNullOrEmpty(request.DistributionPhone))
                    {
                        address.address = request.DistributionAddress;
                        if (request.ProvinceId.HasValue) address.provinceId = request.ProvinceId.Value;
                        if (request.CityId.HasValue) address.cityId = request.CityId.Value;
                        if (request.AreaId.HasValue) address.areaId = request.AreaId.Value;
                        address.phone = request.DistributionPhone;
                        address.Name = request.DistributionName;
                        address.agentId = request.Agent;
                        address.createtime = DateTime.Now;
                        address.userid = user.UserId;
                        address.Status = 1;
                        addressid = _addressRepository.Add(address);
                    }
                }

                #endregion

                #region 订单实例

                var order = new bx_car_order
                {
                    //**为必填项
                    buid = request.Buid, //**
                    //order_num = request.OrderNum,
                    source = sourceValue, //**
                    insured_name = request.InsuredName, //被保险人
                    contacts_name = request.ContactsName, //**联系人
                    mobile = request.Mobile, //**联系电话
                    receipt_head = request.ReceiptHead, //发票类型
                    receipt_title = request.Receipt, //发票内容
                    create_time = DateTime.Now,
                    user_id = user != null ? user.UserId : 0,
                    openid = request.OpenId, //**
                    total_price = (decimal?)request.TotalPrice, //**
                    carriage_price = (decimal?)request.CarriagePrice, //**
                    insurance_price = (decimal?)request.InsurancePrice, //**
                    id_type = request.IdType, //证件类型
                    id_num = request.IdNum, //证件号
                    addressid = addressid, //配送关联id
                    distribution_type = request.DistributionType, //配送类型
                    distribution_address = request.DistributionAddress, //收件地址
                    distribution_name = request.DistributionName, //收件人
                    distribution_phone = request.DistributionPhone, //收件手机
                    distribution_time = request.DistributionTime, //配送时间
                    pay_type = request.PayType, //支付方式
                    id_img_firs = request.IdImgFirst,
                    id_img_secd = request.IdImgSecond,
                    top_agent = request.Topagent, //**顶级代理人id
                    cur_agent = request.Agent, //**当前代理人id
                    bizRate = (decimal?)request.BizRate, //商业险费率
                    LicenseNo = userinfo != null ? userinfo.LicenseNo : string.Empty, //车牌号
                    order_status = 1,
                    pay_status = 0,
                    InvoiceType = request.InvoiceType,
                    imageUrls = string.IsNullOrEmpty(request.ImageUrls) ? request.ImgUrls : request.ImageUrls,
                    forcerate = (decimal)request.ForceRate,
                    order_num = Guid.NewGuid().ToString().Replace("-", ""),
                    order_email = request.OrderEmail,
                    order_mobile = request.OrderPhone
                };
                #endregion
                //创建订单
                long orderid = _orderRepository.CreateOrder(order, user, address, lastinfo, userinfo, savequote,
                    submitInfo, quoteresult, carInfo, claimDetails);
                if (orderid > 0)
                {
                    //生成订单号
                    //var orderNum = GenerateOrderNum(orderid, request.Fountain);//EnumOrderPlatform.WeChat
                    //order.order_num = orderNum;
                    //_orderRepository.Update(order);

                    //将对象添加到缓存start
                    var orderCache = new Messages.Response.AppResponse.OrderCacheResponse
                    {
                        BxCarOrder = order,
                        BxLastInfo = lastinfo,
                        BxQuoteResult = quoteresult,
                        BxSaveQuote = savequote,
                        BxSubmitInfo = submitInfo,
                        BxUserInfo = userinfo,
                        BxCarInfo = carInfo,
                        BxClaimDetails = claimDetails,
                        QrStartDate = qrStartDate
                    };
                    var orderKey = string.Format("OrderDetail_{0}", orderid);
                    CacheProvider.Remove(orderKey);
                    CacheProvider.Set(orderKey, orderCache, 86400);
                    //将对象添加到缓存end

                    long newSourceValue = 0;
                    if (request.IsNewSource.HasValue)
                    {
                        newSourceValue = request.IsNewSource == 1
                            ? request.Source
                            : SourceGroupAlgorithm.GetNewSource((int)request.Source);
                    }
                    else
                    {
                        newSourceValue = SourceGroupAlgorithm.GetNewSource((int)request.Source);
                    }
                    //添加crm时间轴
                    PostCrmStep(request.InsurancePrice, request.Agent, orderid, request.Buid, newSourceValue, uri);

                    //更新userinfo的回访信息
                    userinfo.IsReView = 6;
                    userinfo.UpdateTime = DateTime.Now;
                    string strStatus = _userInfoRepository.Update(userinfo) > 0 ? "成功" : "失败";

                    response.Status = HttpStatusCode.OK;
                    response.OrderId = orderid;
                    response.TradeNum = order.order_num;
                    _logInfo.Info("创建订单成功，更新bx_userinfo记录" + strStatus + "，订单信息:" + request.ToJson());
                }
                else
                {
                    response.Status = HttpStatusCode.ExpectationFailed;
                    response.OrderId = 0;
                    response.TradeNum = string.Empty;
                    _logError.Info("创建订单失败，订单信息：" + request.ToJson());
                }
            }
            catch (Exception ex)
            {
                _logError.Info("创建订单异常，创建订单信息：" + request.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }

        public Messages.Response.AppResponse.UpdateOrderResponse UpdateOrder(Messages.Request.AppRequest.ModifyOrderRequest request)
        {
            var response = new Messages.Response.AppResponse.UpdateOrderResponse();
            try
            {
                bx_car_order orderItem = null;
                if (request.IsUpdatePayStatus)
                {
                    orderItem = _orderRepository.FindBy(request.OrderNum);
                }
                else
                {
                    orderItem = _orderRepository.FindBy(request.OrderID);//, request.OpenId
                }

                if (orderItem != null)
                {
                    if (request.IsUpdatePayStatus)
                    {
                        orderItem.pay_status = request.PayStatus;
                    }
                    else
                    {
                        //orderItem.source = request.Source;
                        orderItem.id_num = request.IdNum;
                        orderItem.id_type = request.IdType;

                        orderItem.insured_name = request.InsuredName;
                        orderItem.mobile = request.Mobile;
                        orderItem.contacts_name = request.ContactsName;

                        orderItem.receipt_title = request.Receipt;
                        orderItem.receipt_head = request.ReceiptHead;

                        orderItem.pay_type = request.PayType;
                        orderItem.insurance_price = (decimal?)request.InsurancePrice;
                        //orderItem.carriage_price = (decimal?) request.CarriagePrice;
                        //orderItem.total_price = (decimal?)request.TotalPrice;
                        orderItem.id_img_firs = request.IdImgFirst;
                        orderItem.id_img_secd = request.IdImgSecond;

                        orderItem.distribution_type = request.DistributionType;
                        orderItem.addressid = request.AddressId;
                        orderItem.distribution_address = request.DistributionAddress;
                        orderItem.distribution_name = request.DistributionName;
                        orderItem.distribution_phone = request.DistributionPhone;
                        orderItem.distribution_time = (DateTime?)request.DistributionTime;
                        orderItem.imageUrls = request.ImgUrls;
                        orderItem.InvoiceType = request.InvoiceType;
                        orderItem.bizRate = request.BizRate;
                        orderItem.forcerate = request.Forcerate;
                        orderItem.order_email = request.OrderEmail;
                        orderItem.order_mobile = request.OrderPhone;
                    }

                    _orderRepository.Update(orderItem);
                    response.Count = orderItem.id;

                    //20170209修改缓存对象
                    var orderKey = string.Format("OrderDetail_{0}", request.OrderID);
                    var cacheObject = CacheProvider.Get<Messages.Response.AppResponse.OrderCacheResponse>(orderKey);
                    if (cacheObject != null)
                    {
                        //将对象添加到缓存start
                        var orderCache = new Messages.Response.AppResponse.OrderCacheResponse
                        {
                            BxCarOrder = orderItem,
                            BxLastInfo = cacheObject.BxLastInfo,
                            BxQuoteResult = cacheObject.BxQuoteResult,
                            BxSaveQuote = cacheObject.BxSaveQuote,
                            BxSubmitInfo = cacheObject.BxSubmitInfo,
                            BxUserInfo = cacheObject.BxUserInfo,
                            BxCarInfo = cacheObject.BxCarInfo,
                            BxClaimDetails = cacheObject.BxClaimDetails,
                            QrStartDate = cacheObject.QrStartDate
                        };
                        CacheProvider.Remove(orderKey);
                        CacheProvider.Set(orderKey, orderCache, 86400);
                        //将对象添加到缓存end
                    }
                }
                else
                {
                    response.Count = 0;
                }
            }
            catch (Exception ex)
            {

                _logError.Info("更新订单异常，更新订单信息：" + request.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return response;
        }
        
        private CreateOrderResponse CreateSureOrderService(Messages.Request.AppRequest.CreateSureOrderRequest request, bx_agent agent, int sourceValue)
        {
            var response = new CreateOrderResponse();

            #region 更新预约单状态
            bx_car_order bxCarOrder = _orderRepository.FindBy(request.OrderId);
            if (bxCarOrder != null)
            {
                //-3微信端的订单终极状态：已收单。-2为整体流程的终极状态，-2由pc端审核 出单之后来改。
                //流程：报价->预约->收单->出单
                bxCarOrder.order_status = -3;
                //bxCarOrder.source = sourceValue;
                bxCarOrder.GetOrderTime = DateTime.Now;//20161011新增收到保单时间
                if (_orderRepository.Update(bxCarOrder) > 0)
                {
                    response.Status = HttpStatusCode.OK;
                    response.OrderId = request.OrderId;
                    _logInfo.Info("更新订单成功,订单信息:" + request.ToJson());
                }
                else
                {
                    response.Status = HttpStatusCode.ExpectationFailed;
                    response.OrderId = 0;
                    _logError.Info("更新订单失败，订单信息：" + request.ToJson());
                }
                return response;
            }
            #endregion

            #region 创建预约单
            bx_lastinfo lastinfo = _lastInfoRepository.GetByBuid(request.Buid);
            bx_userinfo userinfo = _userInfoRepository.FindByBuid(request.Buid);
            bx_savequote savequote = _saveQuoteRepository.GetSavequoteByBuid(request.Buid);
            bx_submit_info submitInfo = _submitInfoRepository.GetSubmitInfo(request.Buid, sourceValue);
            bx_quoteresult quoteresult = _quoteResultRepository.GetQuoteResultByBuid(request.Buid, sourceValue);
            bx_quotereq_carinfo quotereqCarinfo = _quoteReqCarinfoRepository.Find(request.Buid);
            bx_quoteresult_carinfo quoteresultCarinfo = _carinfoRepository.Find(request.Buid, sourceValue);
            List<bx_claim_detail> claimDetails = _userClaimRepository.FindList(request.Buid);

            bx_car_order exitCarOrder = _orderRepository.FindBy(userinfo != null ? userinfo.LicenseNo : string.Empty, agent.OpenId, request.Agent);
            if (exitCarOrder != null)
            {
                exitCarOrder.order_status = -1;//删除订单
                _orderRepository.Update(exitCarOrder);
            }

            //根据openid，mobile，获取userid
            var user = _userService.AddUser(agent.OpenId, agent.Mobile);

            int addressid = 0;

            //订单实例
            //start
            int idType = 2;
            double insurancePrice = 0;
            string insuredName = string.Empty;
            string idNum = string.Empty;
            if (quoteresult != null)
            {
                if (quoteresult.InsuredIdType.HasValue)
                {
                    if (quoteresult.InsuredIdType.Value == 1)
                        idType = 0;
                    else if (quoteresult.InsuredIdType.Value == 2)
                        idType = 1;
                }
                insuredName = quoteresult.InsuredName;
                idNum = quoteresult.InsuredIdCard;
                insurancePrice = (quoteresult.BizTotal.HasValue ? quoteresult.BizTotal.Value : 0) +
                    (quoteresult.ForceTotal.HasValue ? quoteresult.ForceTotal.Value : 0) +
                    (quoteresult.TaxTotal.HasValue ? quoteresult.TaxTotal.Value : 0);
            }

            var order = new bx_car_order
            {//**为必填项
                buid = request.Buid,//**
                //order_num = request.OrderNum,
                source = sourceValue,//**
                insured_name = insuredName,//被保险人
                contacts_name = agent.AgentName,//**联系人
                mobile = agent.Mobile,//**联系电话
                create_time = DateTime.Now,
                user_id = user != null ? user.UserId : 0,
                openid = agent.OpenId,//**
                total_price = (decimal?)insurancePrice,//**
                carriage_price = 0,//**
                insurance_price = (decimal?)insurancePrice,//**
                id_type = idType,//证件类型
                id_num = idNum,//证件号
                addressid = addressid,//配送关联id
                top_agent = request.Agent,//**顶级代理人id
                cur_agent = request.CurAgent,//**当前代理人id
                bizRate = request.BizRate,//商业险费率
                LicenseNo = userinfo != null ? userinfo.LicenseNo : string.Empty,//车牌号
                order_status = -2,//出单
                pay_status = 0,
                GetOrderTime = DateTime.Now //20161011新增收到保单时间
            };
            //订单实例
            //end


            var address = new bx_address();

            //创建订单
            long orderid = _orderRepository.CreateOrder(order, user, address, lastinfo, userinfo, savequote, submitInfo, quoteresult, quoteresultCarinfo, claimDetails);
            if (orderid > 0)
            {
                //生成订单号
                var orderNum = GenerateOrderNum(orderid, request.Fountain);//EnumOrderPlatform.WeChat
                order.order_num = orderNum;
                _orderRepository.Update(order);

                response.Status = HttpStatusCode.OK;
                response.OrderId = orderid;
                _logInfo.Info("创建订单成功,订单信息:" + request.ToJson());
            }
            else
            {
                response.Status = HttpStatusCode.ExpectationFailed;
                response.OrderId = 0;
                _logError.Info("创建订单失败，订单信息：" + request.ToJson());
            }
            return response;

            #endregion
            return response;
        }
        
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="request"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<CarOrderModel> GetOrders(Messages.Request.AppRequest.GetOrdersRequest request, int status, out int totalCount)
        {
            bool isAgent = false;
            var sonself = new List<bx_agent>();
            //当前根据openid获取当前经纪人 
            var curAgent = _agentRepository.GetAgentByTopParentAgent(request.OpenId, request.TopAgentId);
            if (curAgent != null)
            {//代理人
                if (request.IsOnlyMine.HasValue)
                {
                    if (request.IsOnlyMine.Value == 0)
                    {
                        sonself = _agentRepository.GetSonsAgent(curAgent.Id).ToList();
                    }
                }
                isAgent = true;
                sonself.Add(curAgent);
            }

            var orders = _orderRepository.FindListBy(status, isAgent, sonself, request.OpenId, request.TopAgentId, request.Search, request.PageIndex, request.PageSize, out totalCount);

            return orders;
        }
        public List<CarOrderModel> GetOrdersForApp(Messages.Request.AppRequest.GetOrdersRequest request, int status, out int totalCount)
        {
            var agentIds = _agentRepository.GetSonsList(request.ChildAgent, true);
            var orders = _orderRepository.FindListForApp(status, agentIds, request.OpenId, request.Agent, request.Search, request.PageIndex, request.PageSize, out totalCount);

            return orders;
        }
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="openId"></param>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.CarOrderModel FindCarOrderBy(Messages.Request.AppRequest.GetOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)//, string openId
        {

            return _orderRepository.FindCarOrderBy(request.OrderId);//, openId
        }
        
        /// <summary>
        ///     生成订单号
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="sorcePlatform">平台</param>
        /// <returns>订单号</returns>
        private string GenerateOrderNum(long id, int fountain)//EnumOrderPlatform sorcePlatform
        {
            var sb = new StringBuilder();
            sb.Append(fountain);//sb.Append((int)sorcePlatform);
            sb.Append(DateTime.Now.ConvertToTimeStmap().ToString());
            sb.Append((100000 + id).ToString());
            return sb.ToString();
        }

        #region crm时间轴接口调用
        /// <summary>
        /// CRM时间线步骤记录请求串
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="orderId"></param>
        /// <param name="buid"></param>
        /// <param name="source"></param>
        private void PostCrmStep(double insurancePrice, int agentId, long orderId, long buid, long source, Uri uri)
        {

            //string _url = ConfigurationManager.AppSettings["SystemCrmUrl"];

            var _url = string.Format("http://{0}:{1}/api/ConsumerDetail/AddCrmSteps", uri.Host, uri.Port);
            var obj = new JsonContain()
            {
                source = source,
                sourceName = ToEnumDescription(source, typeof(EnumSourceNew)),
                jingfei = insurancePrice,
                isEdit = "true",
                orderId = orderId,
                buid = buid
            };
            var datas = new JsonPost()
            {
                JsonContent = obj.ToJson(),
                AgentId = agentId,
                BUid = buid,
                Type = 3
            };
            _logInfo.Info(string.Format("CRM时间线步骤记录请求串: url:{0}; data:{1}", _url, datas.ToJson()));
            string result = HttpWebAsk.HttpClientPostAsync(datas.ToJson(), _url);
            _logInfo.Info(string.Format("CRM时间线步骤记录返回值:{0}", result));
        }

        public class JsonContain
        {//{"source":0,"jinfei":102,"isEdit":"true","orderId":1404,"buid":600962}
            public long source { get; set; }
            public string sourceName { get; set; }
            public double jingfei { get; set; }//净费
            public string isEdit { get; set; }
            public long orderId { get; set; }
            public long buid { get; set; }
        }

        public class JsonPost
        {
            public string JsonContent { get; set; }
            public int AgentId { get; set; }
            public int Type { get; set; }
            public long BUid { get; set; }
        }
        #endregion
        
        public bx_car_order FindOrderByOrderId(long orderId, string orderNum)
        {

            return _orderRepository.FindOrderByOrderId(orderId, orderNum);

        }

        private static String ToEnumDescription(long value, Type enumType)
        {
            NameValueCollection nvc = GetNvcFromEnumValue(enumType);
            return nvc[value.ToString()];
        }
        private static NameValueCollection GetNvcFromEnumValue(Type enumType)
        {
            var nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }

        public async Task<CreateSureOrderNewResponse> CreateSureOrderNew(Models.ViewModels.Request.CreateSureOrderNewRequest request)
        {
            _logInfo.Info("已承保的单子需要创建已出单" + request.ToJson());
            try
            {
                bx_agent agent = new bx_agent();
                agent = _agentRepository.GetAgent(request.OrderAgent);
                Messages.Request.AppRequest.CreateSureOrderRequest requestNew
                    = new Messages.Request.AppRequest.CreateSureOrderRequest()
                    {
                        CurAgent = request.OrderAgent,
                        Buid = request.BuId,
                        Source = request.Source,
                        Fountain = 2
                    };
                var response = CreateSureOrderService(requestNew, agent, request.Source);
                return new CreateSureOrderNewResponse() { OrderId = response.OrderId };
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new CreateSureOrderNewResponse() { OrderId = 0 };
        }

    }
}
