using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Qiniu.Util;
using Qiniu.Storage;
using System.Security.Cryptography;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using IAgentConfigRepository = BiHuManBu.ExternalInterfaces.Models.IAgentConfigRepository;
using IOrderRepository = BiHuManBu.ExternalInterfaces.Models.IOrderRepository;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class PayService : IPayService
    {
        private IOrderRepository _orderRepository;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private const string logSeparator = "\r\n---------------------------------------------------------------------------------------- \r\n";
        private readonly ILog logErro = LogManager.GetLogger("ERROR");
        private readonly ILog logMsg = LogManager.GetLogger("MSG");
        private IUserInfoRepository _userInfoRepository;
        private ISubmitInfoRepository _submitInfoRepository;
        private IAgentConfigRepository _agentConfigRepository;
        private readonly IOrderService _orderService;
        private readonly IEpolicyRepository _epolicyRepository;
        private readonly IUpdateCompleteTaskService _updateCompleteTaskService;
        private readonly IAgentAnswernCityRepository _agentAnswernCityReposity;
        private readonly ISetSaveCommissionService _setSaveCommissionService;
        private readonly IOrderPostThirdService _orderPostThirdService;
        public PayService(IAgentConfigRepository agentConfigRepository,
                          IOrderRepository orderRepository,
                          IUserInfoRepository userInfoRepository,
                          ISubmitInfoRepository submitInfoRepository,
                          IOrderService orderService,
                          IEpolicyRepository epolicyRepository,
                          ISetSaveCommissionService setSaveCommissionService,
                          IUpdateCompleteTaskService updateCompleteTaskService,
                          IAgentAnswernCityRepository agentAnswernCityReposity, IOrderPostThirdService orderPostThirdService)
        {
            _agentConfigRepository = agentConfigRepository;
            _userInfoRepository = userInfoRepository;
            _orderRepository = orderRepository;
            _submitInfoRepository = submitInfoRepository;
            _orderService = orderService;
            _epolicyRepository = epolicyRepository;
            _setSaveCommissionService = setSaveCommissionService;
            _updateCompleteTaskService = updateCompleteTaskService;
            _agentAnswernCityReposity = agentAnswernCityReposity;
            _orderPostThirdService = orderPostThirdService;
        }

        #region 身份证采集
        /// <summary>
        /// 获取采集设备 wyy 2017-08-17 10:18 add
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IList<Machine> Machines(CollectIdCardRequest request)
        {
            #region old请求接口
            //目前支持 人、太、平、国
            if (request.Source > 0 && request.Source < 9 && !request.ChannelId.HasValue)
            {
                //设备类型：1人保，2太平洋，3平安，10国寿财移动
                //新Source: 1、天平洋  2、平安  4、人保  8、国寿财
                switch (request.Source)
                {
                    case 1:
                        request.Source = 2;
                        break;
                    case 2:
                        request.Source = 3;
                        break;
                    case 4:
                        request.Source = 1;
                        break;
                    case 8:
                        request.Source = 10;
                        break;
                }
                //保险公司
                return _orderRepository.GetMachines(request.Agent, Convert.ToInt32(request.Source));
            }
            #endregion
            //新的获取采集设备接口(根据渠道和保险公司获取)
            request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            return _orderRepository.GetMachines(request.Agent, Convert.ToInt32(request.ChannelId), Convert.ToInt32(request.Source));
        }

        /// <summary>
        /// 身份证采集是否成功或者验证是否通过
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel CollectIdCard(CollectIdCardRequest request)
        {
            if (!string.IsNullOrEmpty(request.Code) && (!CommonHelper.isLetterOrNumber(request.Code) || request.Code.Length > 8 || request.Code.Length < 6))
                return SetBaseViewModel(-10001, "短信验证码不正确!");
            //安心采集
            if (request.Source == 4096)
            {
                return CollectIdCardBase(request);
            }
            var source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            var machines = _orderRepository.GetMachines(request.Agent, Convert.ToInt32(request.ChannelId), source);
            //安心不需要采集器
            if ((!machines.Any() || machines.Count() > 1) && request.Source != 4096)
                return SetBaseViewModel(-10001, "请先绑定采集设备!");
            if (machines.Any())
            {
                request.MachineCode = machines.FirstOrDefault().MachineCode;
                request.MachineId = machines.FirstOrDefault().MachineId;
            }
            #region 请求自动赋值
            //内部的
            if (!string.IsNullOrWhiteSpace(request.OrderNum))
            {
                var orderObj = _orderRepository.FindOrder(request.OrderNum);
                if (orderObj == null)
                    return SetBaseViewModel(-10014, "订单号不存在!");
                #region //太平洋的验证第二步(太保的第二步验证实际上是获取支付地址信息接口校验出来的,只有获取到了支付地址信息才说明验证通过)
                if (!string.IsNullOrWhiteSpace(request.Code) && orderObj.source == 1)
                {
                    var orderPayRequest = new OrderPayRequest();
                    orderPayRequest.OrderNum = request.OrderNum;
                    orderPayRequest.Code = request.Code;
                    orderPayRequest.PayMent = request.PayMent ?? 1;
                    var viewModel = new BaseViewModel();
                    var qrModel = PayQR(orderPayRequest, orderObj);
                    if (qrModel != null && qrModel.BusinessStatus == 1)
                    {
                        //添加采集日志
                        if (!InsertCollectionLog(request))
                            return SetBaseViewModel(-10019, "请输入真实验证码后，再验证！");
                        viewModel.BusinessStatus = 1;
                        viewModel.StatusMessage = "验证成功!";
                        viewModel.Data = qrModel;
                        VoidPayQR(orderPayRequest);//添加作废原支付
                    }
                    else
                    {
                        if (qrModel != null && qrModel.BusinessStatus.HasValue)
                            viewModel.BusinessStatus = (int)qrModel.BusinessStatus;
                        else
                            viewModel.BusinessStatus = -10019;
                        if (qrModel != null)
                            viewModel.StatusMessage = string.IsNullOrWhiteSpace(qrModel.StatusMessage) ? "验证失败!" : qrModel.StatusMessage;
                        else
                            viewModel.StatusMessage = string.IsNullOrWhiteSpace(qrModel.StatusMessage) ? "验证失败!" : qrModel.StatusMessage;
                    }
                    return viewModel;
                }
                #endregion
                #region 平安、人保
                request.LicenseNo = orderObj.licenseno; //车牌
                request.ChannelId = orderObj.quote_config_id;//渠道Id
                request.Source = orderObj.source;//保险公司
                request.BiztNo = orderObj.biz_tno;//商业险投保单号
                request.ForcetNo = orderObj.force_tno;//交强险保单号 
                var userInfo = _userInfoRepository.FindByBuid(orderObj.b_uid);
                request.MobileNumber = string.IsNullOrEmpty(request.MobileNumber) ? userInfo.HolderMobile : request.MobileNumber; //默认投保人手机号
                request.CarVin = userInfo.CarVIN;//车架号
                //投保人
                request.HolderAddress = userInfo.HolderAddress;
                request.HolderBirthday = userInfo.HolderBirthday;
                request.HolderCertiEnddate = userInfo.HolderCertiEnddate;
                request.HolderCertiStartdate = userInfo.HolderCertiStartdate;
                request.HolderEmail = userInfo.HolderEmail;
                request.HolderIdCard = userInfo.HolderIdCard;
                request.HolderIdType = userInfo.HolderIdType.ToString();
                request.HolderIssuer = userInfo.HolderIssuer;
                request.HolderMobile = userInfo.HolderMobile;
                request.HolderName = userInfo.HolderName;
                request.HolderNation = userInfo.HolderNation;
                request.HolderSex = userInfo.HolderSex;
                //被保险人信息
                request.InsuredAddress = userInfo.InsuredAddress;
                request.InsuredBirthday = userInfo.InsuredBirthday;
                request.InsuredCertiEnddate = userInfo.InsuredCertiEnddate;
                request.InsuredCertiStartdate = userInfo.InsuredCertiStartdate;
                request.InsuredEmail = userInfo.InsuredEmail;
                request.InsuredIdCard = userInfo.InsuredIdCard;
                request.InsuredIssuer = userInfo.InsuredIssuer;
                request.InsuredMobile = userInfo.InsuredMobile;
                request.InsuredName = userInfo.InsuredName;
                request.InsuredNation = userInfo.InsuredNation;
                request.InsuredSex = userInfo.InsuredSex;
                #endregion
            }
            else
            {
                //外部的
                if (!request.IsOut.HasValue || request.IsOut == false)
                    return SetBaseViewModel(-10014, "数据有误!");
                #region  外部的太平洋的验证第二步(太保的第二步验证实际上是获取支付地址信息接口校验出来的,只有获取到了支付地址信息才说明验证通过)
                if (!string.IsNullOrWhiteSpace(request.Code) && request.Source == 1 && request.IsOut.HasValue && request.IsOut == true)
                {
                    var orderPayRequest = new OrderPayRequest();
                    orderPayRequest.OrderNum = request.OrderNum;
                    orderPayRequest.Code = request.Code;
                    orderPayRequest.CarVin = request.CarVin;
                    orderPayRequest.ChannelId = request.ChannelId;
                    orderPayRequest.BiztNo = request.BiztNo;
                    orderPayRequest.ForcetNo = request.ForcetNo;
                    orderPayRequest.BankId = request.BankId;
                    orderPayRequest.LicenseNo = request.LicenseNo;
                    orderPayRequest.Source = request.Source;
                    orderPayRequest.PayWay = request.PayWay;
                    orderPayRequest.BankId = request.BankId;
                    orderPayRequest.PayMent = request.PayMent ?? 1;
                    orderPayRequest.IsOut = true;
                    var viewModel = new BaseViewModel();
                    var qrModel = PayQR(orderPayRequest);
                    if (qrModel != null && qrModel.BusinessStatus == 1)
                    {
                        //添加采集日志
                        if (!InsertCollectionLog(request))
                            return SetBaseViewModel(-10019, "添加采集日志失败!");
                        qrModel.BusinessStatus = null;
                        qrModel.StatusMessage = null;
                        viewModel.BusinessStatus = 1;
                        viewModel.StatusMessage = "验证成功!";
                        //viewModel.Data = qrModel;
                        VoidPayQR(orderPayRequest);//添加作废原支付
                    }
                    else
                    {
                        viewModel.BusinessStatus = qrModel.BusinessStatus.HasValue ? (int)qrModel.BusinessStatus : -10001;
                        if (qrModel != null)
                            viewModel.StatusMessage = string.IsNullOrWhiteSpace(qrModel.StatusMessage) ? "验证失败!" : qrModel.StatusMessage;
                        else
                            viewModel.StatusMessage = string.IsNullOrWhiteSpace(qrModel.StatusMessage) ? "验证失败!" : qrModel.StatusMessage;
                    }
                    return viewModel;
                }
                #endregion
            }
            #endregion
            return CollectIdCardBase(request);
        }

        private BaseViewModel CollectIdCardBase(CollectIdCardRequest request)
        {
            #region 正则 民族校验
            //安心的不需要校验
            if (request.Source != 4096)
            {
                if (string.IsNullOrWhiteSpace(request.HolderNation))
                    return SetBaseViewModel(-10001, "投保人民族不能为空!");
                if (string.IsNullOrWhiteSpace(request.InsuredNation))
                    return SetBaseViewModel(-10001, "被保人民族不能为空!");
                //民族列表
                var nationList = EnumHelper.GetEnumItemDesc(typeof(EnumNation));
                var isHolderNation = false;
                var isInsuredNation = false;
                foreach (var item in nationList)
                {
                    if (request.HolderNation.Equals(item.Value))
                        isHolderNation = true;
                    if (request.InsuredNation.Equals(item.Value))
                        isInsuredNation = true;
                }
                if (!isHolderNation)
                    return SetBaseViewModel(-10001, "投保人民族不正确!");
                if (!isInsuredNation)
                    return SetBaseViewModel(-10001, "被保人民族不正确!");
                //外部需要转换保险公司
                if (string.IsNullOrWhiteSpace(request.OrderNum))
                    request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
                request.HolderCertiEnddate = Convert.ToDateTime(request.HolderCertiEnddate).ToString("yyyyMMdd");
                request.HolderCertiStartdate = Convert.ToDateTime(request.HolderCertiStartdate).ToString("yyyyMMdd");
                request.InsuredCertiEnddate = Convert.ToDateTime(request.InsuredCertiEnddate).ToString("yyyyMMdd");
                request.InsuredCertiStartdate = Convert.ToDateTime(request.InsuredCertiStartdate).ToString("yyyyMMdd");
                if (request.InsuredCertiEnddate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期止期不正确!");
                if (request.HolderCertiEnddate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "投保人证件有效期起期不正确!");
                if (request.HolderCertiStartdate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期止期不正确!");
                if (request.InsuredCertiStartdate.Equals("00010101"))
                    return SetBaseViewModel(-10001, "被保人证件有效期起期不正确!");
                if (string.IsNullOrWhiteSpace(request.HolderBirthday))
                    return SetBaseViewModel(-10001, "投保人生日日期不正确!");
                if (string.IsNullOrWhiteSpace(request.InsuredBirthday))
                    return SetBaseViewModel(-10001, "被保人保人生日日期不正确!");
            }
            else
            {
                //安心的不传投保单号
                var submitInfo = _submitInfoRepository.GetSubmitInfo(request.buId ?? 0, 12);
                if (submitInfo != null)
                {
                    request.BiztNo = submitInfo.biz_tno;
                    request.ForcetNo = submitInfo.force_tno;
                }
                else
                {
                    return SetBaseViewModel(-10001, "不存在报价信息!");
                }
            }
            #endregion
            //对外的缓存key
            var verifyCodeKey = string.Format("{0}_{1}_{2}", request.Agent, request.CarVin, request.MachineId);
            //提交时判断验证码是否有效
            //去掉安心
            if (request.Source != 4096)
            {
                if (request.IsOut == null && !string.IsNullOrEmpty(request.Code) && !_orderRepository.CheckOrderVerificationDate(request.OrderNum))
                {
                    return SetBaseViewModel(-10001, "验证码已失效!");
                }
                else
                {
                    //对外接口查询验证码是否有效 key=agent+CarVin+MachineId        _cacheClientString.KeyExists(verifyCodeKey)          
                    if (request.IsOut != null && !string.IsNullOrEmpty(request.Code) && CacheProvider.Get<int>(verifyCodeKey) < 1)
                        return SetBaseViewModel(-10001, "验证码已失效!");
                }
                request.settingid = request.MachineId;
            }

            #region//向炎龙发送消息体
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");
            //传递消息体的缓存key
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //缓存key         
            var sendMode = new
            {
                request.BiztNo,
                request.busiuserId,
                request.CarVin,
                request.ChannelId,
                request.Code,
                request.ForcetNo,
                request.HolderAddress,
                request.HolderBirthday,
                request.HolderCertiEnddate,
                request.HolderCertiStartdate,
                request.HolderEmail,
                request.HolderIdCard,
                request.HolderIdType,
                request.HolderIssuer,
                request.HolderMobile,
                request.HolderName,
                request.HolderNation,
                request.HolderSex,
                request.InsuredAddress,
                request.InsuredBirthday,
                request.InsuredCertiEnddate,
                request.InsuredCertiStartdate,
                request.InsuredEmail,
                request.InsuredIdCard,
                request.InsuredIssuer,
                request.InsuredMobile,
                request.InsuredName,
                request.InsuredNation,
                request.InsuredSex,
                request.LicenseNo,
                request.MachineCode,
                request.MobileNumber,
                request.NotifyCacheKey,
                request.SaleChannel,
                request.settingid,
                //安心的需要转换
                Source = request.Source == 4096 ? 12 : request.Source,
                B_Uid = request.buId
            };
            #endregion

            #region 判断是验证还是发送阅知单(验证)
            //安心的没有验证
            if (string.IsNullOrEmpty(request.Code) || (!string.IsNullOrEmpty(request.Code) && request.Source != 4096))
            {
                var notifyCacheKey = string.Empty;
                if (string.IsNullOrEmpty(request.Code))
                {
                    SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_idCardCollection");
                    notifyCacheKey = string.Format("{0}-IdCard", request.NotifyCacheKey);
                }
                else
                {
                    SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_cardVerify");
                    notifyCacheKey = string.Format("{0}-CardVerify", request.NotifyCacheKey);
                }
                // #endregion
                // #region //读取缓存 数据校验
                CollectIdCardResult centerMsg = null;
                centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
                var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
                if (null == centerMsg)
                {
                    for (int i = 0; i < PayWaitDate; i++)
                    {
                        centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
                        if (centerMsg != null)
                            break;
                        Task.Delay(TimeSpan.FromMilliseconds(1000)).Wait(); //500毫秒
                    }
                }
                if (string.IsNullOrEmpty(request.Code))
                    logMsg.Info(string.Format("【订单支付-采集发送阅知单接口】中心返回信息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
                else
                    logMsg.Info(string.Format("【订单支付-采集验证接口】中心返回信息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
                if (centerMsg == null)
                    return SetBaseViewModel(-10019, "请求超时,请重试!");
                //已经采集过了，无需采集 2017/10/09 wyy add  -60029
                if (centerMsg.ErrCode == -33029)
                {
                    //不是对外时修改订单的采集状态 
                    if (!request.IsOut.HasValue || request.IsOut == false)
                    {
                        if (_orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1))
                            return SetBaseViewModel((int)centerMsg.ErrCode, "已经采集过了,无需采集!");
                        else
                            return SetBaseViewModel(-10014, "订单不存在,验证失败!");
                    }
                    return SetBaseViewModel((int)centerMsg.ErrCode, "无需采集!");
                }
                if (centerMsg.ErrCode != 0)
                    return SetBaseViewModel((int)centerMsg.ErrCode, string.IsNullOrEmpty(centerMsg.ErrMsg) ? " 请求失败！" : centerMsg.ErrMsg);
            }
            #endregion

            #region //结果处理
            //添加采集日志
            if (!InsertCollectionLog(request))
                return SetBaseViewModel(-10019, "添加采集日志失败!");
            if (string.IsNullOrEmpty(request.Code) || (!string.IsNullOrEmpty(request.Code) && request.Source != 4096))
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    if (request.IsOut == null && request.Source != 4096)
                    {
                        //更新订单表的验证码时间 dd_order.verification_code_lapse_time
                        if (_orderRepository.UpdateOrderVerificationDate(request.OrderNum))
                            return SetBaseViewModel(1, "验证码已发送!");
                        else
                            return SetBaseViewModel(-10014, "更改验证有误!");
                    }
                    else
                    {
                        //有效时间117秒
                        CacheProvider.Set(verifyCodeKey, 1, 117 * 60);
                        return SetBaseViewModel(1, "验证码已发送!");
                    }
                }
                else
                {
                    if (request.IsOut == null && request.Source != 4096)
                    {
                        if (_orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1))
                            return SetBaseViewModel(1, "验证成功!");
                        else
                            return SetBaseViewModel(-10014, "订单不存在,验证失败!");
                    }
                    else
                    {
                        return SetBaseViewModel(1, "验证成功!");
                    }
                }
            }
            else
            {
                return SetBaseViewModel(1, "操作成功!");
            }
            #endregion
        }

        /// <summary>
        /// 添加采集日志
        /// </summary>
        /// <returns></returns>
        private bool InsertCollectionLog(CollectIdCardRequest request)
        {
            var o = new dd_order_collection();
            o.biz_tno = request.BiztNo;
            o.force_tno = request.ForcetNo;
            o.order_num = string.IsNullOrWhiteSpace(request.OrderNum) ? request.CarVin : request.OrderNum;
            if (request.IsOut.HasValue && request.IsOut == true)
                o.is_out = 1;
            else
                o.is_out = 0;
            o.licenseno = request.LicenseNo;
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                o.status = -1;
                o.verify_code = string.IsNullOrWhiteSpace(request.Code) ? " " : request.Code;
                o.update_time = DateTime.Now;
                o.creat_time = o.update_time;
                if (_orderRepository.InsertOrdderCollection(o) > 0)
                    return true;
            }
            else
            {
                var m = _orderRepository.GetOrdderCollection(o.order_num, null);
                if (m != null)
                {
                    m.status = 1;
                    m.verify_code = request.Code;
                    m.update_time = DateTime.Now;
                    if (_orderRepository.ModifyOrdderCollection(m) > 0)
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获取最近的一条采集信息验证码
        /// </summary>
        /// <param name="num">订单号或者车架号</param>
        /// <param name="status">状态</param>
        /// <returns>获取采集的信息</returns>
        private dd_order_collection GetcollectionLog(string num, int? status)
        {
            return _orderRepository.GetOrdderCollection(num, status);
        }

        /// <summary>
        /// 获取采集状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel CollectStatus(CollectIdCardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderNum))
                return SetBaseViewModel(-10019, "订单号不存在!");
            var ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
            if (ddOrder == null)
                return SetBaseViewModel(-10019, "订单号不存在!");


            request.Source = ddOrder.source;
            request.ForcetNo = ddOrder.force_tno;
            request.BiztNo = ddOrder.biz_tno;
            request.LicenseNo = ddOrder.licenseno;
            request.ChannelId = ddOrder.quote_config_id;
            var userInfo = _userInfoRepository.FindByBuid(ddOrder.b_uid);
            if (userInfo == null)
                return SetBaseViewModel(-10019, "信息不全!");
            request.CarVin = userInfo.CarVIN;

            if (string.IsNullOrWhiteSpace(request.CarVin) || string.IsNullOrWhiteSpace(request.LicenseNo))
                return SetBaseViewModel(-10019, "信息不全!");

            //传给炎龙的消息体参数 2017-12-26 wyy  
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            var sendMode = new
            {
                request.Source,
                request.NotifyCacheKey,
                request.ForcetNo,
                request.BiztNo,
                request.CarVin,
                request.LicenseNo,
                request.ChannelId
            };
            //向中心发送消息
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_collectionStatus");
            //缓存key
            var notifyCacheKey = string.Format("{0}-CollectionStatus", request.NotifyCacheKey);
            var num = 0;
            CollectIdCardResult centerMsg = null;
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);
                centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
            } while (centerMsg == null && num <= PayWaitDate);//1分钟
            logMsg.Info(string.Format("【订单支付-获取采集状态】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            if (centerMsg == null)
                return SetBaseViewModel(-10019, "请求超时,请重试!");
            if (centerMsg.ErrCode.HasValue)
                return SetBaseViewModel((int)centerMsg.ErrCode, centerMsg.ErrMsg);
            else
                return SetBaseViewModel(-10019, "返回消息错误!");
        }

        /// <summary>
        /// 设置返回值
        /// </summary>
        /// <param name="code">消息码</param>
        /// <param name="msg">消息内容</param>
        /// <param name="data">结果</param>
        /// <returns></returns>
        public BaseViewModel SetBaseViewModel(int code, string msg, object data = null)
        {
            var model = new BaseViewModel();
            model.BusinessStatus = code;
            model.StatusMessage = msg;
            model.Data = data;
            return model;
        }

        #endregion

        #region 全支付(支付地址信息、支付状态、作废支付)
        /// <summary>
        /// 获取支付地址信息(内部)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ddOrder"></param>
        /// <returns></returns>
        public OrderPayment PayQR(OrderPayRequest request, dd_order ddOrder = null)
        {
            try
            {
                if (request.PayMent == 0)
                {
                    return SetOrderPayment(-10019, "支付方式必选!");
                }
                else
                {
                    if (request.PayMent == 1)
                    {
                        request.PayWay = "weixin";
                        request.BankId = string.Empty; //微信支付没有合作银行
                    }
                    else
                        request.PayWay = request.PayMent.ToString();
                }
                //外部的
                if (request.IsOut.HasValue && request.IsOut == true)
                {

                    var tempAgentString = request.Agent.ToString();

                    var user = _userInfoRepository.GetList((o) =>
                      o.Agent == tempAgentString &&
                      o.OpenId == request.CustKey &&
                      o.LicenseNo == request.LicenseNo).FirstOrDefault();

                    if (user != null)
                    {
                        request.B_Uid = user.Id;
                        logMsg.Info(string.Format("【查询buid】成功-车牌：{0},Agent:{1},CustKey:{2}，Buid:{3}", request.LicenseNo, request.Agent, request.CustKey, request.B_Uid));
                    }
                    else
                    {
                        logMsg.Info(string.Format("【查询buid】失败-车牌：{0},Agent:{1},CustKey:{2}", request.LicenseNo, request.Agent, request.CustKey));
                    }

                    //新source替换旧source
                    request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
                    return PayQRBase(request);

                }
                else
                {
                    if (ddOrder == null)
                        ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
                    if (ddOrder != null)
                    {

                        //判断支付方式是否修改了 2018.11.27 wyy
                        //if (GetCenterPayType(ddOrder.pay_type) != request.PayMent)
                        //{
                        //    var op = VoidPayQR(request);
                        //    if (!op.BusinessStatus.HasValue|| op.BusinessStatus != 0)
                        //    {
                        //        return SetOrderPayment(-10019, "切换支付方式失败!");
                        //    }
                        //}

                        //起保日期是否小于当前日期
                        if (!ddOrder.order_lapse_time.HasValue || ddOrder.order_lapse_time <= DateTime.Now)
                            return SetOrderPayment(-10019, "支付时间已过期,请重新报价核保!");
                        request.BiztNo = ddOrder.biz_tno;
                        request.ForcetNo = ddOrder.force_tno;
                        if (string.IsNullOrEmpty(request.BiztNo) && string.IsNullOrWhiteSpace(request.ForcetNo))
                            return SetOrderPayment(-10019, "必须是核保报价成功的投保单!");
                        //新增参数 2017年8月25日 (周五) 16:58  
                        request.B_Uid = ddOrder.b_uid;
                        request.Source = ddOrder.source;
                        request.LicenseNo = ddOrder.licenseno;
                        request.ChannelId = ddOrder.quote_config_id;
                        //获取支付方式及合作银行    只有刷卡的时候才会选择合作银行
                        if (ddOrder.source == 1 && request.PayWay.TrimStart('0') == "2")
                        {
                            if (ddOrder.pay_way_id > 0)
                            {
                                var pbmodel = _orderRepository.GetPayWayBank(ddOrder.pay_way_id);
                                if (pbmodel == null)
                                    return SetOrderPayment(-10019, "请选择支付方式及合作银行！");
                                //request.PayWay = pbmodel.PayWay;
                                request.BankId = pbmodel.BankId;
                            }
                            else
                            {
                                request.PayWay = "weixin";
                                request.PayMent = 1;
                                request.BankId = string.Empty;
                            }
                        }
                        //车架号
                        var userInfo = _userInfoRepository.FindByBuid(ddOrder.b_uid);
                        request.CarVin = userInfo.CarVIN;
                    }
                    else
                    {
                        return SetOrderPayment(-10019, "订单号不存在!");
                    }
                    return PayQRBase(request, ddOrder);
                }

            }
            catch (Exception ee)
            {
                logMsg.Info(string.Format("【PayQR】异常-车牌：{0},Agent:{1},CustKey:{2}，Buid:{3}，Ex:{4}", request.LicenseNo, request.Agent, request.CustKey, request.B_Uid, ee.ToString()));
                throw ee;
            }
        }
        /// <summary>
        /// 获取中心的支付方式
        /// </summary>
        /// <param name="paytype">-1默认值 0=微信支付 1=支付宝支付 2=现金支付 3=POS机刷卡  4=银行卡转账  5=支票支付</param>
        /// <returns>1=微信 2=pos 3=支付宝</returns>
        public int GetCenterPayType(int paytype)
        {
            switch (paytype)
            {
                case -1:
                    return -1;
                case 0: //0=微信支付
                    return 1;
                case 1: //1=支付宝支付
                    return 3;
                case 2://2=现金支付
                    return -1;
                case 3://3=POS机刷卡
                    return 2;
                default:
                    return -1;
            }
        }


        public OrderPayment PayQRBase(OrderPayRequest request, dd_order order = null)
        {
            //发送请求获取链接
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //太平洋&&划卡支付
            if (request.Source == 1 && request.PayWay.TrimStart('0') == "2")
            {
                if (string.IsNullOrWhiteSpace(request.BankId))
                    return SetOrderPayment(-10019, "请选择支付方式及合作银行！");
            }
            if (!IsUsedChannel(request.ChannelId))
                return SetOrderPayment(-10015, "渠道不可用!");

            //太平洋的问题 wyy 2017-12-06
            if (request.Source == 1 && string.IsNullOrWhiteSpace(request.Code))
            {
                //获取一下验证码 wyy 2017-12-06(太保和安心需要)
                var orderNum = string.IsNullOrWhiteSpace(request.OrderNum) ? request.CarVin : request.OrderNum;
                var o = GetcollectionLog(orderNum, 1);
                if (o != null)
                    request.Code = o.verify_code;
            }

            //传给炎龙的消息体参数 2017-10-19 wyy
            var sendMode = new
            {
                request.Source,
                request.NotifyCacheKey,
                request.ForcetNo,
                request.BiztNo,
                request.CarVin,
                request.LicenseNo,
                request.ChannelId,
                request.B_Uid,
                request.PayMent,//1=微信支付，2=poss支付
                request.PayWay, //支付方式——支付链接太平洋专用
                CooperBant = request.BankId, //合作银行——支付链接太平洋专用
                request.Code  //验证码(太平洋验证)
            };
            //向中心发送消息
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_insurance");
            //缓存key
            var notifyCacheKey = string.Format("{0}-Insurance", request.NotifyCacheKey);
            var num = 0;
            WaPayInfoResponse centerMsg = null;
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);
                centerMsg = CacheProvider.Get<WaPayInfoResponse>(notifyCacheKey);
            } while (centerMsg == null && num <= PayWaitDate);//1分钟

            #region 校验数据
            logMsg.Info(string.Format("【订单支付】获取支付链接,中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            if (centerMsg == null)
                return SetOrderPayment(-10019, "请求超时!");
            //支付已过期
            if (centerMsg.ErrCode == -33010)
            {
                //修改订单状态
                if (request.IsOut == null && !string.IsNullOrEmpty(request.OrderNum))
                {
                    if (order != null)
                    {
                        order.order_type = 2;//已过期
                        _orderRepository.ModifyOrder(order);
                    }
                }
            }
            if (centerMsg.ErrCode != 0)
                return SetOrderPayment((int)centerMsg.ErrCode, centerMsg.ErrMsg);
            #endregion
            #region //支付流水表
            var nowDate = DateTime.Now;
            var orderSerial = new dd_order_serial();
            orderSerial.amount = Convert.ToDouble(centerMsg.Amount);
            orderSerial.create_time = nowDate;
            orderSerial.update_time = nowDate;
            orderSerial.status = 0;
            //支付流水号(太平洋=验证码)
            orderSerial.serialr_no = string.IsNullOrWhiteSpace(centerMsg.SerialrNo) ? centerMsg.CheckCode : centerMsg.SerialrNo;
            orderSerial.pay_url = centerMsg.PayUrl;
            orderSerial.pay_status = centerMsg.PayStatus;
            orderSerial.order_num = request.OrderNum;
            orderSerial.notice_no = centerMsg.NoticeNo;
            orderSerial.licenseno = centerMsg.Licenseno;
            orderSerial.failure_time = centerMsg.FailureTime;
            orderSerial.customer_name = centerMsg.CustomerName;
            orderSerial.currency = centerMsg.Currency;
            #endregion
            #region 返回模型
            var model = new OrderPayment();
            model.LicenseNo = centerMsg.Licenseno;
            //支付流水号（太平洋=校验码）
            model.PayNum = string.IsNullOrWhiteSpace(centerMsg.SerialrNo) ? centerMsg.CheckCode : centerMsg.SerialrNo;
            model.TransactionNum = string.IsNullOrWhiteSpace(centerMsg.NoticeNo) ? " " : centerMsg.NoticeNo;
            model.PayUrl = centerMsg.PayUrl;
            model.Money = Convert.ToDouble(centerMsg.Amount);
            model.OrderNum = request.OrderNum;
            model.FailureTime = centerMsg.FailureTime;
            model.Name = centerMsg.CustomerName;
            //model.CustomerName = centerMsg.CustomerName;
            model.SerialrNo = centerMsg.SerialrNo;
            //返回支付方式  wyy 2018.10.30 add
            if (request.IsGetPayWay == 1)
                model.PayWay = centerMsg.PayTypeNo;

            #endregion
            //添加支付流水信息
            if (!string.IsNullOrEmpty(request.OrderNum))
            {
                orderSerial.isout = 0;
                //返回支付地址即QR
                if (_orderRepository.ModifyOrderQR(request.OrderNum, centerMsg.PayUrl) &&
                     _orderRepository.InsertOrderSerial(orderSerial))
                {
                    //lzl 2017-11-25 添加 获取支付成功后更新验证状态
                    _orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1);
                    return SetOrderPayment(1, "成功！", model);
                }
                return SetOrderPayment(-10019, "更新支付流水或更新支付结果失败！");
            }
            else
            {
                orderSerial.isout = 1;
                if (_orderRepository.InsertOrderSerial(orderSerial))
                {
                    //lzl 2017-11-25 添加 获取支付成功后更新验证状态
                    _orderRepository.UpdateVerificationCodeStatus(request.OrderNum, 1);
                    return SetOrderPayment(1, "成功！", model);
                }
                return SetOrderPayment(-10019, "更新支付流水信息失败！");
            }
        }

        /// <summary>
        /// 发送电子投保单确认单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel SendSms(OrderPayRequest request)
        {
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //不存在，内部调用
            if (request.IsOut.HasValue && request.IsOut == true)
            {
                //新source替换旧source
                request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            }
            else
            {
                var ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
                if (ddOrder == null)
                    return BaseViewModel.GetBaseViewModel(-10019, "订单不存在！");
                request.Source = ddOrder.source;
                request.ForcetNo = ddOrder.force_tno;
                request.BiztNo = ddOrder.biz_tno;
                request.LicenseNo = ddOrder.licenseno;
                request.ChannelId = ddOrder.quote_config_id;
                var user = _userInfoRepository.FindByBuid(ddOrder.b_uid);
                if (user != null)
                    request.CarVin = user.CarVIN;
            }
            //传给炎龙的消息体参数 2018-11-15 wyy
            var sendMode = new
            {
                request.Source,
                request.NotifyCacheKey,
                request.ForcetNo,
                request.BiztNo,
                request.CarVin,
                request.LicenseNo,
                request.ChannelId
            };
            //向中心发送消息
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_smsInform");
            //缓存key
            var notifyCacheKey = string.Format("{0}-SmsInform", request.NotifyCacheKey);
            var num = 0;
            WaPayInfoResponse centerMsg = null;
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);
                centerMsg = CacheProvider.Get<WaPayInfoResponse>(notifyCacheKey);
            } while (centerMsg == null && num <= PayWaitDate);//1分钟

            #region 校验数据
            logMsg.Info(string.Format("【发送电子投保确认单】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            if (centerMsg == null)
                return BaseViewModel.GetBaseViewModel(-10001, "超时");
            if (centerMsg.ErrCode != 0)
                return BaseViewModel.GetBaseViewModel(centerMsg.ErrCode ?? 0, centerMsg.ErrMsg);

            return BaseViewModel.GetBaseViewModel(1, "电子投保确认单短信已发送!");
            #endregion
        }

        /// <summary>
        /// 获取支付成功信息,如果库里面不存在就需要调中心的到账查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OrderPayment> Pay(OrderPayRequest request)
        {
            var model = new OrderPayment();
            dd_order_paymentresult orderPayment = null;
            WaPayResultInfoResponse centerMsg = null;
            dd_order ddOrder = null;
            bx_userinfo userInfo = null;
            var CarVin = string.Empty;
            var PolicyHoderCard = string.Empty;
            if (!string.IsNullOrWhiteSpace(request.OrderNum))
            {
                ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
                if (ddOrder != null)
                {
                    request.B_Uid = ddOrder.b_uid; //新增参数 2017年8月25日 (周五) 16:58  
                    request.BiztNo = ddOrder.biz_tno; //投保单号
                    request.ForcetNo = ddOrder.force_tno;
                    request.Source = ddOrder.source;
                    request.LicenseNo = ddOrder.licenseno;
                    request.ChannelId = ddOrder.quote_config_id;
                    if (string.IsNullOrWhiteSpace(request.BiztNo) && string.IsNullOrWhiteSpace(request.ForcetNo))
                        return SetOrderPayment(-10019, "必须是核保成功的投保单!");
                    userInfo = _userInfoRepository.FindByBuid(ddOrder.b_uid);
                    if (userInfo == null)
                    {
                        return SetOrderPayment(-10019, "用户信息不存在!");
                    }
                    request.CarVin = userInfo.CarVIN;
                    //太平洋查出流水号
                    if (request.Source == 1)
                    {
                        var serial = _orderRepository.FindOrderSerial(request.OrderNum.Trim());
                        if (serial == null || serial.notice_no == null)
                            return SetOrderPayment(-10018, "请先付款后,再查询到账状态!");
                        request.TransactionNum = serial.notice_no;
                    }
                }
                else
                {
                    return SetOrderPayment(-10019, "订单号不存在!");
                }
            }

            #region 如果没有支付结果就向中心发送查询请求
            //向中心请求支付信息_到账查询接口
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //外部的需要转换source
            if (string.IsNullOrWhiteSpace(request.OrderNum))
                request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            //对外的检验
            if (request.Source == 1 && string.IsNullOrWhiteSpace(request.TransactionNum))
                return SetOrderPayment(-10018, "请先付款后,再查询到账状态!");
            if (!IsUsedChannel(request.ChannelId))
                return SetOrderPayment(-10015, "渠道不可用!");
            //向炎龙发送消息体参数
            var sendModel = new
            {
                request.NotifyCacheKey,
                request.BiztNo,
                request.ForcetNo,
                request.CarVin,
                request.LicenseNo,
                request.ChannelId,
                request.Source,
                request.B_Uid,
                NoticeNo = request.TransactionNum  //缴费通知单号(太——支付号)
            };
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendModel), "pay_outcome");
            var notifyCacheKey = string.Format("{0}-payOutcome", request.NotifyCacheKey);
            var num = 0;
            //读取配置等待时间
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);//读取时间主键加快
                centerMsg = CacheProvider.Get<WaPayResultInfoResponse>(notifyCacheKey);
            } while (centerMsg == null && num < PayWaitDate);

            logMsg.Info(string.Format("【订单支付-到账查询】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);

            if (centerMsg == null)
                return SetOrderPayment(-10019, "请求超时,请重试!");
            if (centerMsg.FindPayResult == 0)
                return SetOrderPayment(-10016, "待缴费!");

            #region 业务逻辑处理
            //到账实体事务
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    orderPayment = new dd_order_paymentresult();
                    //0、获取该订单的支付链接的最近的一条流水
                    var orderSerialModel = _orderRepository.FindOrderSerial(request.OrderNum);
                    if (orderSerialModel != null)
                    {
                        orderSerialModel.status = 1;
                        //修改该支付流水的支付状态    
                        _orderRepository.ModifyOrderSerialPayResult(orderSerialModel);
                        orderPayment.name = orderSerialModel.customer_name;
                        orderPayment.pay_num = orderSerialModel.serialr_no;
                        orderPayment.transaction_num = orderSerialModel.notice_no; //交易通知单号==缴费通知单号
                    }
                    //1、插入支付结果表(dd_order_paymentresult)                     
                    orderPayment.order_num = string.IsNullOrEmpty(request.OrderNum) ? request.CarVin : request.OrderNum;
                    orderPayment.order_id = request.OrderId;
                    orderPayment.biz_no = centerMsg.BizpNo;
                    orderPayment.force_no = centerMsg.ForcepNo;
                    orderPayment.create_time = DateTime.Now;
                    orderPayment.licenseNo = request.LicenseNo;
                    orderPayment.money = centerMsg.Amount;
                    orderPayment.pay_type = 1; //默认微信
                    orderPayment.remarks = "全款支付,默认微信支付!";
                    orderPayment.type = 1; //支付类型 1 = 全款支付 、2 = 净费支付
                    orderPayment.find_pay_result = centerMsg.FindPayResult; //支付结果 0=待缴费、1=已承保
                    orderPayment.pay_source = request.PaySource;
                    orderPayment.payment_time = centerMsg.PaymentTime;

                    //支付结果表
                    if (!_orderRepository.InsertOrderPayResult(orderPayment))
                        return SetOrderPayment(-10017, "记录支付结果出错!");
                    //对外接口不处理
                    if (request.IsOut == null)
                    {
                        //全款支付状态
                        ddOrder.insurance_company_pay_status = 1;
                        //净费支付成功的单子才算完成
                        if (ddOrder.consumer_pay_status == 1 || ddOrder.payee == 1)
                            ddOrder.order_type = 5; //订单已完成

                        ddOrder.biz_pno = orderPayment.biz_no;
                        ddOrder.force_pno = orderPayment.force_no;
                        ddOrder.pay_biz_time = string.IsNullOrEmpty(orderPayment.biz_no) ? DateTime.Parse("1970-01-01 00:00:00") : orderPayment.payment_time;
                        ddOrder.pay_force_time = string.IsNullOrEmpty(orderPayment.force_no) ? DateTime.Parse("1970-01-01 00:00:00") : orderPayment.payment_time;
                        ddOrder.transaction_no = orderPayment.transaction_num;
                        ddOrder.serial_no = orderPayment.pay_num;
                        ddOrder.pay_time = orderPayment.payment_time;

                        if (!_orderRepository.ModifyOrder(ddOrder))
                            return SetOrderPayment(-10018, "更新订单状态失败!");
                        //订单改为已承保 调用这个 service wyy 2017-12-04
                        var mo = new CreateSureOrderNewRequest() { BuId = ddOrder.b_uid, DDOrderId = ddOrder.id, OrderAgent = ddOrder.agent_id, Source = ddOrder.source };
                        await _orderService.CreateSureOrderNew(mo);
                    }
                    //完成事务，这个必须加上，要不然事务不会有效
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                logMsg.Info("【订单支付-到账查询】事务异常消息：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            #endregion
            #endregion
            List<dd_order_paymentresult> paylist = new List<dd_order_paymentresult>();
            paylist.Add(orderPayment);
            //发送post请求调用第三方
            _orderPostThirdService.SendPost(request.Agent, "", 2, null, null, ddOrder, null, paylist, null);
            //更新佣金和积分 鹏洁的接口
            _setSaveCommissionService.Save(request.OrderNum, 1);
            //团队任务接口 鹏洁的接口
            _updateCompleteTaskService.UpdateCompleteTask(request.ChildAgent);

            #region // 3、给model赋返回值
            model.BusinessStatus = 1;
            model.BizpNo = centerMsg.BizpNo;
            model.ForcepNo = centerMsg.ForcepNo;
            //缴费时间(保司返回)
            model.PaymentDateStr = centerMsg.PaymentTime.ToString();
            if (request.IsOut == null)
            {
                model.OrderNum = orderPayment.order_num;
                model.PayNum = orderPayment.pay_num;
                model.Name = orderPayment.name;
                model.Money = Convert.ToDouble(orderPayment.money);
                model.LicenseNo = orderPayment.licenseNo;
                model.TransactionNum = orderPayment.transaction_num;
                //承保时间
                model.GetPolicyDateStr = orderPayment.payment_time.ToString();
            }
            else
            {
                model.FindPayResult = centerMsg.FindPayResult;
                model.Money = centerMsg.Amount;
            }
            //主动拉取时才返回这些字段 wyy 2018.08.27 15:53
            if (request.IsAddAllInfo == 1)
            {
                model.BuId = ddOrder.b_uid;
                model.ForcetNo = ddOrder.force_tno;
                model.BiztNo = ddOrder.biz_tno;
                model.CarVin = userInfo.CarVIN;
                model.PolicyHoderCard = userInfo.HolderIdCard;
            }
            #endregion
            return model;
        }

        /// <summary>
        /// 作废原支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrderPayment VoidPayQR(OrderPayRequest request)
        {
            try
            {
                var baseView = new OrderPayment();
                dd_order ddOrder = null;
                dd_order_serial SerialModel = null;
                if (!string.IsNullOrEmpty(request.OrderNum))
                {
                    ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
                    if (ddOrder != null)
                    {
                        request.B_Uid = ddOrder.b_uid; //新增参数 2018年12月20日  易孟飞
                        request.BiztNo = ddOrder.biz_tno;
                        request.ForcetNo = ddOrder.force_tno;
                        request.Source = ddOrder.source;
                        request.LicenseNo = ddOrder.licenseno;
                        request.ChannelId = ddOrder.quote_config_id;
                        if (string.IsNullOrWhiteSpace(request.BiztNo) && string.IsNullOrWhiteSpace(request.ForcetNo))
                            return SetOrderPayment(-10019, "必须是核保成功的投保单!");
                        //交易通知单号
                        SerialModel = _orderRepository.FindOrderSerial(ddOrder.order_num);
                        if (SerialModel == null)
                            return SetOrderPayment(1, "已作废!");
                        //return SetOrderPayment(-10019, "不存在该支付交易通知单号!");
                        
                        //交易通知单号
                        request.TransactionNum = SerialModel.notice_no;
                        //获取原支付方式及合作银行(太保需要)                  
                        if (ddOrder.source == 1)
                        {
                            var pbmodel = _orderRepository.GetPayWayBank(ddOrder.pay_way_id);
                            if (pbmodel == null)
                                return SetOrderPayment(-10019, "不存在该支付方式!");
                            request.PayWay = pbmodel.PayWay;
                        }
                    }
                    else
                    {
                        return SetOrderPayment(-10019, "订单号不存在!");
                    }
                }
                if (!IsUsedChannel(request.ChannelId))
                    return SetOrderPayment(-10015, "渠道不可用!");

                //转换保险资源
                if (request.IsOut.HasValue)
                {
                    var tempAgentString = request.Agent.ToString();

                    var user = _userInfoRepository.GetList((o) =>
                    o.Agent == tempAgentString &&
                    o.OpenId == request.CustKey &&
                    o.LicenseNo == request.LicenseNo).FirstOrDefault();

                    if (user != null)
                        request.B_Uid = user.Id;

                    request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
                }

                //传递消息体的缓存key
                request.NotifyCacheKey = Guid.NewGuid().ToString();

                //向炎龙发送消息体
                var sendMode = new
                {
                    request.Source,
                    request.NotifyCacheKey,
                    request.BiztNo,
                    request.ForcetNo,
                    NoticeNo = request.TransactionNum,
                    request.ChannelId,
                    request.PayWay, //原支付方式(太保需要)
                    request.B_Uid,  //新增参数 2018年12月20日  易孟飞
                };

                SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_rejectPolicy");
                var notifyCacheKey = string.Format("{0}-RejectPolicy", request.NotifyCacheKey);
                WaPayInfoResponse centerMsg = null;
                centerMsg = CacheProvider.Get<WaPayInfoResponse>(notifyCacheKey);
                var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
                #region //读取缓存
                if (null == centerMsg)
                {
                    for (int i = 0; i < PayWaitDate; i++)
                    {
                        centerMsg = CacheProvider.Get<WaPayInfoResponse>(notifyCacheKey);
                        if (null != centerMsg)
                            break;
                        Task.Delay(TimeSpan.FromMilliseconds(1000)).Wait(); //500毫秒
                    }
                }
                #endregion
                logMsg.Info(string.Format("【订单支付-作废支付】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
                if (centerMsg == null)
                    return SetOrderPayment(-10019, "请求超时,请重试!");
                if (centerMsg.ErrCode != 0)
                    return SetOrderPayment((int)centerMsg.ErrCode, centerMsg.ErrMsg);
                if (centerMsg.ErrCode == 0)
                {
                    if (SerialModel != null)
                    {
                        SerialModel.id = SerialModel.id;
                        SerialModel.notice_no = centerMsg.NoticeNo;
                        SerialModel.pay_url = centerMsg.PayUrl;
                        SerialModel.update_time = new DateTime();
                        SerialModel.amount = centerMsg.Amount;
                        SerialModel.failure_time = centerMsg.FailureTime;
                        SerialModel.pay_status = centerMsg.PayStatus;
                        SerialModel.payment_time = centerMsg.PaymentTime;
                        SerialModel.licenseno = centerMsg.Licenseno;
                        _orderRepository.ModifyOrderSerialPayResult(SerialModel);
                    }
                    return SetOrderPayment(1, string.IsNullOrEmpty(centerMsg.ErrMsg) ? "原支付方式作废成功" : centerMsg.ErrMsg);
                }
                return baseView;
            }
            catch (Exception ee)
            {
                logMsg.Info(string.Format("【VoidPayQR】异常-车牌：{0},Agent:{1},CustKey:{2}，Buid:{3}，Ex:{4}", request.LicenseNo, request.Agent, request.CustKey, request.B_Uid, ee.ToString()));
                throw ee;
            }
        }

        public OrderPayment SetOrderPayment(int code, string msg, OrderPayment model = null)
        {

            if (model != null)
            {
                model.BusinessStatus = code;
                model.StatusMessage = msg;
                return model;
            }
            return new OrderPayment
            {
                BusinessStatus = code,
                StatusMessage = msg
            };
        }


        #endregion

        #region 安心支付
        /// <summary>
        /// 安心支付获取加密串
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isOut">是否对外 false(对内) true(对外)</param>
        /// <returns></returns>
        public AXPayEntity GetAXPaySecrek(AXPayRequest model, bool isOut = false)
        {
            var entity = new AXPayEntity();
            var agentAnswernCity = GetModelByUId(model.BuId, model.Agent);
            if (agentAnswernCity == null || agentAnswernCity.Id == 0) return null;
            //安心账户
            entity.RequestCode = agentAnswernCity.Accout;
            var transNo = string.Format("{0}{1}{2}", entity.RequestCode, DateTime.Now.ToString("yyyyMMddHHmmssfff"), new Random().Next(10, 99));
            var dic = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(entity.RequestCode))
                dic.Add("requestCode", entity.RequestCode);
            if (!string.IsNullOrWhiteSpace(model.OrderNo))
                dic.Add("orderNo", model.OrderNo);
            if (model.PayAmt > 0)
                dic.Add("payAmt", model.PayAmt.ToString());
            var strOrigin = string.Format("{0}{1}", agentAnswernCity.PayPwd, string.Join("&", dic.OrderBy(t => t.Key).Select(t => t.Key + "=" + t.Value)));
            //得到签名
            entity.checkValue = AXMD5(strOrigin);
            //添加记录(入库)
            var axEntity = _orderRepository.GetPayAxByOrderNo(model.OrderNo, model.BuId);
            if (axEntity == null) axEntity = new dd_pay_ax();
            if (isOut)
            {
                axEntity.licenseNo = model.LicenseNo;
                axEntity.biz_tno = model.Biztno;
                axEntity.force_tno = model.Forcetno;
                axEntity.agent_id = model.Agent;
                axEntity.attach = string.Empty;
                axEntity.pay_type = model.PayMent;
                //描述
                axEntity.remark = string.Empty;
            }
            else
            {
                var orderModel = string.IsNullOrWhiteSpace(model.OrderNum) ? null : _orderRepository.FindOrder(model.OrderNum.Trim());
                if (orderModel != null)
                {
                    //车牌号、订单号 
                    axEntity.licenseNo = orderModel.licenseno;
                    axEntity.order_num = (int)orderModel.id;
                    axEntity.agent_id = orderModel.agent_id;
                    //附加数据 订单号(dd_order.order_num)
                    axEntity.attach = orderModel.order_num;
                    axEntity.pay_type = orderModel.pay_type;
                    //描述
                    axEntity.remark = orderModel.remark;
                }
                //投保单号
                var submitInfo = _submitInfoRepository.GetSubmitInfo(model.BuId, 12);
                if (submitInfo != null)
                {
                    axEntity.biz_tno = submitInfo.biz_tno;
                    axEntity.force_tno = submitInfo.force_tno;
                }
            }

            axEntity.app_nameList = string.Empty;

            ////投保单号即安心的报价单号
            axEntity.b_uid = model.BuId;
            axEntity.cardNoList = string.Empty;
            axEntity.card_typeList = string.Empty;
            axEntity.create_time = DateTime.Now;
            axEntity.is_true_name = string.Empty;
            axEntity.limit_time = string.Empty;
            //订单Id(dd_order.id)
            axEntity.pay_name = string.Empty;
            axEntity.status = -1;
            axEntity.trans_no = transNo;
            axEntity.update_time = axEntity.create_time;
            axEntity.trans_amt = model.PayAmt;
            axEntity.cardNoList = string.Empty;
            axEntity.orderNo = model.OrderNo;
            if (axEntity.id > 0)
            {
                _orderRepository.ModifyPayAx(axEntity);
            }
            else
            {
                _orderRepository.InsertPayAx(axEntity);
            }
            return entity;
        }

        /// <summary>
        /// 安心对外获取支付地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string GetAxPayPath(AXPayRequest model, out string message)
        {
            message = "";
            var axModel = GetAXPaySecrek(model, true);
            if (axModel == null) return string.Empty;
            string parameters = "requestCode=" + axModel.RequestCode + "&checkValue=" + axModel.checkValue + "&orderNo=" + model.OrderNo + "&payAmt=" + model.PayAmt.ToString();
            byte[] byteArray = Encoding.Default.GetBytes(parameters);
            var response = HttpWebAsk.Post(ApplicationSettingsFactory.GetApplicationSettings().AX_PayPath, byteArray);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                var payurl = string.Empty;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var jObject = JObject.Parse(result);
                    message = jObject["msg"] != null ? jObject["msg"].ToString() : "";
                    if (jObject["code"] != null && jObject["code"].ToString() == "1")
                    {
                        var jObject2 = jObject["data"] == null ? null : JObject.Parse(jObject["data"].ToString());
                        if (jObject2 != null && jObject2["payUrl"] != null)
                        {
                            payurl = jObject2["payUrl"].ToString();
                        }
                    }
                }
                return payurl;
            }
            return string.Empty;
        }
        public static string AXMD5(string strOrigin)
        {
            char[] hexDigitsTemp = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            var hexDigits = hexDigitsTemp.Select(t => (byte)t).ToArray();
            var b = Encoding.UTF8.GetBytes(strOrigin);
            var md = new MD5CryptoServiceProvider().ComputeHash(b);
            int j = md.Length;
            byte[] str = new byte[j * 2];
            int k = 0;
            for (int i = 0; i < j; i++)
            {
                byte byte0 = md[i];
                str[k++] = hexDigits[(byte0 >> 4) & 0xf];
                str[k++] = hexDigits[byte0 & 0xf];
            }
            return Encoding.UTF8.GetString(str).ToLower();
        }

        /// <summary>
        /// 安心后台支付回调
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool GetAXPayBack(AXPayResponse model)
        {
            logMsg.Info(string.Format("【安心支付后台回调】返回参数：{0}", CommonHelper.JsonSerializeObject(model)));
            var entity = new AXPayEntity();
            #region //0、校验数据
            var payEntity = _orderRepository.GetPayAxByOrderNo(model.orderNo);
            if (payEntity == null)
            {
                logMsg.Info(string.Format("【安心支付异步回调通知】通知数据不存在,商户订单号：{0}!", model.transNo));
                return false;
            }
            var agentAnswernCity = GetModelByUId(payEntity.b_uid);
            if (agentAnswernCity == null || agentAnswernCity.Id == 0) return false;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            entity.RequestCode = agentAnswernCity.Accout;//CommonHelper.GetAppSettings("AX_cooperation");
            if (!string.IsNullOrWhiteSpace(model.transNo))
                dic.Add("transNo", model.transNo);
            if (!string.IsNullOrWhiteSpace(model.transDate))
                dic.Add("transDate", model.transDate);
            if (!string.IsNullOrWhiteSpace(model.requestCode))
                dic.Add("requestCode", model.requestCode);
            if (!string.IsNullOrWhiteSpace(model.payNo))
                dic.Add("payNo", model.payNo);
            if (!string.IsNullOrWhiteSpace(model.payType))
                dic.Add("payType", model.payType);
            if (!string.IsNullOrWhiteSpace(model.payAmt))
                dic.Add("payAmt", model.payAmt);
            if (!string.IsNullOrWhiteSpace(model.payDate))
                dic.Add("payDate", model.payDate);
            if (!string.IsNullOrWhiteSpace(model.payResult))
                dic.Add("payResult", model.payResult);
            if (!string.IsNullOrWhiteSpace(model.payRemark))
                dic.Add("payRemark", model.payRemark);//System.Web.HttpUtility.UrlDecode(model.payRemark, Encoding.UTF8)
            //附加数据为订单号
            if (!string.IsNullOrWhiteSpace(model.attach))
                dic.Add("attach", model.attach);
            if (!string.IsNullOrWhiteSpace(model.orderId))
                dic.Add("orderId", model.orderId);
            if (!string.IsNullOrWhiteSpace(model.referNo))
                dic.Add("referNo", model.referNo);
            if (!string.IsNullOrWhiteSpace(model.orderNo))
                dic.Add("orderNo", model.orderNo);
            //请求码不对
            if (!model.requestCode.Equals(entity.RequestCode))
            {
                logMsg.Info(string.Format("【安心支付异步回调通知】返回的请求码不正确,本地请求码：{0}--返回的请求码：{1}", entity.RequestCode, model.requestCode));
                return false;
            }
            var strOrigin = string.Format("{0}{1}", agentAnswernCity.PayPwd, string.Join("&", dic.OrderBy(t => t.Key).Select(t => t.Key + "=" + t.Value)));
            var md5Str = AXMD5(strOrigin);
            //校验码不对
            if (!model.checkValue.Equals(AXMD5(strOrigin)))
            {
                logMsg.Info(string.Format("【安心支付异步回调通知】校验码不正确!"));
                return false;
            }

            //金额不对
            if (payEntity.trans_amt != Convert.ToInt32(model.payAmt))
            {
                logMsg.Info(string.Format("【安心支付异步回调通知】通知数据的金额与请求时的金额不相等！"));
                return false;
            }
            payEntity.trans_date = CommonHelper.ConvertStringToDate(model.transDate, DateType.Second);
            payEntity.order_id = model.orderId;
            payEntity.pay_date = CommonHelper.ConvertStringToDate(model.payDate, DateType.Second);
            payEntity.pay_no = model.payNo;
            payEntity.pay_remark = System.Web.HttpUtility.UrlDecode(model.payRemark, Encoding.UTF8);
            payEntity.pay_result = Convert.ToInt32(model.payResult);
            payEntity.refer_no = model.referNo;
            //支付结果
            payEntity.status = (int)payEntity.pay_result;
            payEntity.update_time = DateTime.Now;
            #endregion
            //修改安心支付记录信息
            _orderRepository.ModifyPayAx(payEntity);
            //支付成功后处理
            if (model.payResult.Equals("1"))
                return true;
            return false;
        }

        /// <summary>
        /// 安心支付信息,通过订单号或者保单Id(承保接口)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isOut">是否对外 true（对外） false（对内）</param>
        /// <returns></returns>
        public async Task<AXPayEntity> GetAXPayInfo(AXPayResponse model, bool isOut = false)
        {
            var entity = new AXPayEntity();
            entity.Money = 0;
            entity.FindPayResult = 0;
            if (string.IsNullOrWhiteSpace(model.OrderNum) && !isOut)
            {
                entity.ErrorMsg = "订单号不能为空!";
                entity.Code = -1;
                return entity;
            }
            var orderPay = isOut ? null : _orderRepository.GetOrderPayResult(model.OrderNum, 1);
            if (orderPay == null)
            {
                orderPay = new dd_order_paymentresult();
                var policyObj = new PolicyGenerateRequestMain();
                //北京地区的短信验证码
                var cAppValidateNo = string.Empty;
                var quoteConfigId = 0;
                if (isOut)
                {
                    quoteConfigId = model.ChannelId;
                    cAppValidateNo = model.CAppValidateNo;
                }
                else
                {
                    var orderMolde = _orderRepository.FindOrder(model.OrderNum.Trim());
                    if (orderMolde != null)
                    {
                        quoteConfigId = orderMolde.quote_config_id;
                        model.BuId = orderMolde.b_uid;
                        var axModel = _orderRepository.GetAnxinDelivery(model.BuId);
                        if (axModel != null)
                            cAppValidateNo = axModel.appvalidateno;
                    }
                    else
                    {
                        entity.ErrorMsg = "订单不存在!";
                        entity.Code = -1;
                        return entity;
                    }
                }

                //1、处理数据,更新dd_pay_ax数据
                var payEntity = isOut ? _orderRepository.GetPayAxByOrderNo(model.orderNo) : _orderRepository.GetPayAx(model.BuId, model.OrderNum.Trim());
                if (payEntity == null)
                {
                    entity.ErrorMsg = "待缴费!";
                    entity.Code = -1;
                    return entity;
                }
                //查询投保单号
                var lst = new List<PayConfirmInfoVO>();
                if (!string.IsNullOrWhiteSpace(payEntity.force_tno) || !string.IsNullOrWhiteSpace(model.ForcetNo))
                    lst.Add(new PayConfirmInfoVO
                    {
                        cAppNo = string.IsNullOrWhiteSpace(payEntity.force_tno) ? model.ForcetNo : payEntity.force_tno,
                        cAppValidateNo = cAppValidateNo
                    });
                if (!string.IsNullOrWhiteSpace(payEntity.biz_tno) || !string.IsNullOrWhiteSpace(model.Biztno))
                    lst.Add(new PayConfirmInfoVO
                    {
                        cAppNo = string.IsNullOrWhiteSpace(payEntity.biz_tno) ? model.Biztno : payEntity.biz_tno,
                        cAppValidateNo = cAppValidateNo
                    });
                //申请单号即投保单号
                policyObj.PayConfirmInfoList = lst;
                //订单号
                policyObj.cPaySequence = payEntity.orderNo;
                //支付交易流水号
                policyObj.cBizConsultNo = payEntity.pay_no;
                ////支付方式
                policyObj.cPayTyp = payEntity.pay_type.ToString();
                //支付时间
                policyObj.tChargeTm = Convert.ToDateTime(payEntity.pay_date).ToString("yyyy-MM-dd hh:MM:ss");
                //渠道Id
                policyObj.ChannelId = quoteConfigId;

                var num = 0;
                var obj = new PolicyGenerateResponse();
                do
                {
                    //向中心发送API请求
                    var parms = CommonHelper.JsonSerializeObject(policyObj);
                    logMsg.Info(string.Format("【安心支付-调用承保接口】请求中心参数：{0}", parms));
                    string msg = HttpWebAsk.HttpClientPostAsync(parms, string.Format("{0}/api/PolicyGenerate/PolicyGenerateInfo", CommonHelper.GetAppSettings("CenterApiAddress")));
                    if (string.IsNullOrWhiteSpace(msg))
                        logMsg.Info(string.Format("【安心支付-调用承保接口】中心返回数据为空！"));
                    else
                        logMsg.Info(string.Format("【安心支付-调用承保接口】中心返回数据:{0}！", msg));
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PolicyGenerateResponse>(msg);
                    Thread.Sleep(1000);
                    num++;
                } while ((obj.ErrCode != 0 || !obj.PolicyGenerateResponseMain.Any()) && num < 11);

                if (obj.ErrCode != 0 || !obj.PolicyGenerateResponseMain.Any())
                {
                    entity.ErrorMsg = "没有查询到到账信息!";
                    entity.Code = -1;
                    return entity;
                }
                //商业险保单号
                if (payEntity.biz_tno != null && !string.IsNullOrWhiteSpace(payEntity.biz_tno))
                {
                    var o = obj.PolicyGenerateResponseMain.Where(s => s.cAppNo.Equals(payEntity.biz_tno)).FirstOrDefault();
                    if (o != null)
                        orderPay.biz_no = o.cPlyNo;
                }
                //交强险保单号
                if (payEntity.force_tno != null && !string.IsNullOrWhiteSpace(payEntity.force_tno))
                {
                    var o = obj.PolicyGenerateResponseMain.Where(s => s.cAppNo.Equals(payEntity.force_tno)).FirstOrDefault();
                    if (o != null)
                        orderPay.force_no = o.cPlyNo;
                }
                orderPay.licenseNo = payEntity.licenseNo;
                orderPay.money = payEntity.trans_amt / 100;
                orderPay.name = "安心保险";//name
                orderPay.order_id = payEntity.order_num;
                orderPay.pay_num = model.payNo;
                orderPay.pay_source = 0;
                //微信扫码-PC端
                if (payEntity.pay_type.Equals("3"))
                    orderPay.pay_type = 0;
                //支付宝-PC端
                if (payEntity.pay_type.Equals("4"))
                    orderPay.pay_type = 1;
                orderPay.find_pay_result = 1;
                //附加数据为订单号
                orderPay.order_num = payEntity.attach;
                orderPay.order_id = payEntity.order_num;
                orderPay.remarks = string.Format("安心支付:{0}!", model.payRemark);
                //交易流水号
                orderPay.transaction_num = model.payNo;
                orderPay.type = 1;
                //承保时间 
                orderPay.payment_time = payEntity.pay_date;
                //添加支付结果记录      
                if (_orderRepository.InsertOrderPayResult(orderPay))
                {
                    if (!isOut)
                    {
                        //修改订单表状态
                        var ddOrder = _orderRepository.FindOrder(model.OrderNum.Trim());
                        if (ddOrder != null)
                        {
                            //全款支付状态
                            ddOrder.insurance_company_pay_status = 1;
                            //净费支付成功的单子才算完成
                            if (ddOrder.consumer_pay_status == 1 || ddOrder.payee == 1)
                                ddOrder.order_type = 5; //订单已完成

                            if (!_orderRepository.ModifyOrder(ddOrder))
                            {
                                return entity;
                            }
                            //订单改为已承保 调用这个 service wyy 2017-12-04
                            var mo = new CreateSureOrderNewRequest() { BuId = ddOrder.b_uid, DDOrderId = ddOrder.id, OrderAgent = ddOrder.agent_id };
                            await _orderService.CreateSureOrderNew(mo);
                        }
                    }
                }
            }
            entity.Code = 1;
            entity.ErrorMsg = "ok!";
            entity.BizpNo = orderPay.biz_no;
            entity.ForcepNo = orderPay.force_no;
            entity.Money = orderPay.money ?? 0;
            entity.FindPayResult = 1;
            return entity;
        }


        #endregion

        #region 补发电子邮件
        /// <summary>
        /// 补发电子保单
        /// </summary>
        /// <returns></returns>
        public BaseViewModel ReissueElectronicPolicy(OrderPayRequest request)
        {
            var baseView = new BaseViewModel();
            if (!string.IsNullOrEmpty(request.OrderNum))
            {
                var ddOrder = _orderRepository.FindOrder(request.OrderNum.Trim());
                if (ddOrder != null)
                {
                    request.BizpNo = ddOrder.biz_tno;//商业险投保单号
                    request.ForcepNo = ddOrder.force_tno;//交强险保单号
                    request.Source = ddOrder.source;
                    request.LicenseNo = ddOrder.licenseno;
                    request.ChannelId = ddOrder.quote_config_id;
                    var userInfo = _userInfoRepository.FindByBuid(ddOrder.b_uid);
                    request.CarVin = userInfo.CarVIN;
                    request.InsuredEmail = string.IsNullOrEmpty(request.InsuredEmail) ? userInfo.HolderEmail : request.InsuredEmail;
                    var orderPayment = _orderRepository.GetOrderPayResult(request.OrderNum, 1);
                    if (orderPayment == null)
                        return SetBaseViewModel(-10019, "支付结果信息不存在!");
                    request.BizpNo = orderPayment.biz_no;//商业险保单号
                    request.ForcepNo = orderPayment.force_no;//交强险保单号
                }
                else
                {
                    return SetBaseViewModel(-10019, "订单号不存在!");
                }
            }
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");

            if (request.IsOut.HasValue)
                request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            //传递消息体的缓存key
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            //向炎龙发送消息体
            var sendMode = new
            {
                request.Source,
                request.NotifyCacheKey,
                request.BiztNo,
                request.ForcetNo,
                request.BizpNo,
                request.ForcepNo,
                request.CarVin,
                request.ChannelId,
                request.LicenseNo,
                request.InsuredEmail, //邮箱
                MobileNumber = request.InsuredMobile //手机
            };
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "reissue_policy");
            var notifyCacheKey = string.Format("{0}-reissuePolicy", request.NotifyCacheKey);
            //读取炎龙返回的缓存               
            CollectIdCardResult centerMsg = null;
            centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            #region //读取缓存
            if (null == centerMsg)
            {
                for (int i = 0; i < PayWaitDate; i++)
                {
                    centerMsg = CacheProvider.Get<CollectIdCardResult>(notifyCacheKey);
                    if (centerMsg != null)
                        break;
                    Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();  //500毫秒                       
                }
            }
            #endregion
            logMsg.Info(string.Format("【订单支付】补发电子保单,中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            if (centerMsg == null)
                return SetBaseViewModel(-10019, "请求超时,请重试!");
            ////查无此保单
            //if (centerMsg.ErrCode == -61002)
            //    return SetBaseViewModel(-61002, centerMsg.ErrMsg);
            if (centerMsg.ErrCode != 0)
                return SetBaseViewModel((int)centerMsg.ErrCode, centerMsg.ErrMsg);
            if (centerMsg.ErrCode == 0)
                return SetBaseViewModel(1, string.IsNullOrEmpty(centerMsg.ErrMsg) ? "补发电子保单成功" : centerMsg.ErrMsg);
            return baseView;
        }
        #endregion

        #region 电子保单下载
        /// <summary>
        /// 申请下载电子保单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel ApplyElecPolicy(EPolicycs request)
        {
            if (!request.Source.HasValue)
                return SetBaseViewModel(-10019, "缺少source参数！");
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");
            //转换source
            request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            var msg = string.Empty;
            //检查是否已申请
            var model = IsApplyElecPolicy(request, true, out msg);
            if (model != null)
                return SetBaseViewModel(2, "已申请,请勿重复申请!");
            //发送申请,传给中心的消息体参数 2017-12-27 wyy  
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            var sendMode = new
            {
                AgentId = request.Agent,
                Source = request.Source,
                request.NotifyCacheKey,
                request.ForcetNo,
                request.BiztNo,
                request.ForcepNo,
                request.BizpNo,
                request.CarVin,
                request.LicenseNo,
                request.ChannelId,
                B_Uid = request.BuId,
                request.InsuredIdCard,
                HolderIdCard = request.PolicyHoderCard
            };
            //向中心发送消息
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_epolicyDownload");
            //缓存key
            var notifyCacheKey = string.Format("{0}-EPolicyDownload-key", request.NotifyCacheKey);
            var num = 0;
            var centerMsg = string.Empty;
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);
                centerMsg = CacheProvider.Get<string>(notifyCacheKey);
            } while (string.IsNullOrWhiteSpace(centerMsg) && num <= PayWaitDate);//1分钟
            logMsg.Info(string.Format("【订单支付-获取采集状态】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            if (string.IsNullOrWhiteSpace(centerMsg))
                return SetBaseViewModel(-10019, "请求超时,请重试!");
            //添加日志
            var obj = new bx_epolicy_log();
            obj.license_no = request.LicenseNo;
            obj.BiztNo = request.BiztNo;
            obj.BizpNo = request.BizpNo;
            obj.ForcepNo = request.ForcepNo;
            obj.ForcetNo = request.ForcetNo;
            obj.source = (int)request.Source;
            //0下载中  1下载成功  2下载失败
            obj.status = Convert.ToInt32(centerMsg);
            obj.agent_id = request.Agent;
            obj.b_uid = request.BuId;
            obj.InsuredIdCard = request.InsuredIdCard;
            obj.is_out = request.IsOut;
            obj.NotifyCacheKey = request.NotifyCacheKey;
            obj.channel_id = request.ChannelId;

            var m = _epolicyRepository.InsertEpolicyLog(obj);
            if (m != null)
                return SetBaseViewModel(1, "申请成功!");

            return SetBaseViewModel(-10019, "申请失败!");
        }

        /// <summary>
        /// 获取下载的申请状态【作废】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel GetElecPolicyApply(EPolicycs request)
        {
            if (!request.Source.HasValue)
                return SetBaseViewModel(-10019, "缺少source参数！");
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");
            //转换source
            request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            var msg = string.Empty;
            var model = IsApplyElecPolicy(request, false, out msg);
            if (model == null)
                return SetBaseViewModel(-1, "未申请!");
            if (model.status == 2)
                return SetBaseViewModel(2, "下载失败!");
            if (model.status == 0)
            {
                var notifyCacheKey = string.Format("{0}-EPolicyDownload-key", model.NotifyCacheKey);
                var centerMsg = CacheProvider.Get<string>(notifyCacheKey);
                if (!string.IsNullOrWhiteSpace(centerMsg))
                {
                    if (centerMsg.Equals("0"))
                        return SetBaseViewModel(0, "下载中!");
                    if (centerMsg.Equals("2"))
                        return SetBaseViewModel(-10019, "下载失败!");
                    if (!centerMsg.Equals("1"))
                        return SetBaseViewModel(-10019, "缓存失败!");
                }
                else
                {
                    //读库
                    var dts = _epolicyRepository.FindEpolicyByAgentAnd(request.Agent, (int)request.Source, request.LicenseNo);
                    if (!dts.Any())
                        return SetBaseViewModel(0, "下载中!");
                    var epolicy = dts.OrderByDescending(s => s.id).FirstOrDefault();
                    if (epolicy.status == 0)
                        return SetBaseViewModel(0, "下载中!");
                    if (epolicy.status == 2)
                        return SetBaseViewModel(0, "下载失败!");
                }
            }
            return SetBaseViewModel(1, "下载成功!");
        }


        /// <summary>
        /// 下载电子保单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel DownloadElecPolicy(EPolicycs request)
        {
            if (!request.Source.HasValue)
                return SetBaseViewModel(-10019, "缺少source参数！");
            if (!IsUsedChannel(request.ChannelId))
                return SetBaseViewModel(-10015, "渠道不可用!");
            //转换source
            request.Source = SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(request.Source));
            //是否已申请
            var msg = string.Empty;
            //检查是否已申请
            var model = IsApplyElecPolicy(request, false, out msg);
            if (model == null)
                return SetBaseViewModel(-1, msg.Equals("ok!") ? "未申请,请先申请电子保单下载!" : msg);
            //1、先查询缓存内是否存在
            var notifyCacheKey = string.Format("{0}-EPolicyDownload-key", model.NotifyCacheKey);
            var centerMsg = string.Empty;
            //var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            //do
            //{
            //    num++;
            //    Thread.Sleep(1000);
            centerMsg = CacheProvider.Get<string>(notifyCacheKey);
            // } while (string.IsNullOrWhiteSpace(centerMsg) && num <= PayWaitDate);//1分钟
            var epolicy = new bx_epolicy();
            //存在缓存
            if (!string.IsNullOrWhiteSpace(centerMsg))
            {
                // 0下载中  1下载成功  2下载失败
                if (centerMsg.Equals("0"))
                    return SetBaseViewModel(0, "下载中!");
                if (centerMsg.Equals("2"))
                    return SetBaseViewModel(2, "下载失败!");
                if (!centerMsg.Equals("1"))
                    return SetBaseViewModel(-10019, "缓存失败!");
                //查缓存
                notifyCacheKey = string.Format("{0}-EPolicyDownload", model.NotifyCacheKey);
                epolicy = CacheProvider.Get<bx_epolicy>(notifyCacheKey);
                //缓存没有值就读库
                if (epolicy == null)
                {
                    //读库
                    var dts = _epolicyRepository.FindEpolicyByAgentAnd(request.Agent, (int)request.Source, request.LicenseNo);
                    if (dts.Any())
                        epolicy = dts.OrderByDescending(s => s.id).FirstOrDefault();
                }
            }
            else
            {
                //读库
                var dts = _epolicyRepository.FindEpolicyByAgentAnd(request.Agent, (int)request.Source, request.LicenseNo);
                if (dts.Any())
                    epolicy = dts.OrderByDescending(s => s.id).FirstOrDefault();
            }
            if (epolicy == null || epolicy.id < 1)
                return SetBaseViewModel(2, "下载失败啦!");
            //拼接下载url
            return SetBaseViewModel(1, "下载成功!", ElecPolicyUrl(epolicy.key));
        }

        /// <summary>
        /// 是否已申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="msg"></param>
        /// <param name="isCheckStatus">是够校验下载状态</param>
        /// <returns>true=已申请,false=未申请</returns>
        public bx_epolicy_log IsApplyElecPolicy(EPolicycs request, bool isCheckStatus, out string msg)
        {
            msg = "ok!";
            if (string.IsNullOrWhiteSpace(request.LicenseNo))
            {
                msg = "缺少LicenseNo参数!";
                return null;
            }
            var objs = _epolicyRepository.FindEpolicyLogByAgentAnd(request.Agent, (int)request.Source, request.LicenseNo, request.ChannelId);
            if (!objs.Any())
                return null;
            var obj = objs.OrderByDescending(s => s.id).FirstOrDefault();
            //5分钟内申请过 status=2下载失败
            if (isCheckStatus && obj.status == 2)
                return null;
            //已申请过了
            return obj;
        }

        /// <summary>
        /// 获取下载的url(有效期1小时)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ElecPolicyUrl(string key)
        {
            Mac mac = new Mac(CommonHelper.GetAppSettings("QiNiu_AccessKey"), CommonHelper.GetAppSettings("QiNiu_SecretKey"));
            string privateUrl = DownloadManager.CreatePrivateUrl(mac, CommonHelper.GetAppSettings("QiNiu_domain"), key, 3600);
            return privateUrl;
        }

        #endregion

        /// <summary>
        /// 获取支付的合作银行
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IList<PayWayBanksModel> GetPayWayBanks(PayWayBanksModel model)
        {
            return _orderRepository.GetPayWayBanks(model);
        }

        /// <summary>
        /// 渠道是否可用
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public bool IsUsedChannel(int? channelId)
        {
            if (!channelId.HasValue || channelId < 1)
                return false;
            var agentConfig = _agentConfigRepository.GetAgentConfigById((long)channelId);
            if (agentConfig == null || agentConfig.is_used != 1)
                return false;
            return true;
        }

        /// <summary>
        ///  向中心发送获取保单请求
        /// </summary>
        /// <param name="data">参数字符串</param>
        /// <param name="method">方法名</param>
        private string SendToMessageCenter(string data, string method)
        {
            string url = string.Format("{0}/SetBusinessMessage", CommonHelper.GetAppSettings("MessageCenter"), method);
            string postData = string.Format("data={0}&queueName={1}", data, method);
            string result;
            int ret = HttpWebAsk.Post(url, postData, out result);
            var md = string.Empty;
            switch (method)
            {
                case "reissue_policy": md = "补发电子保单"; break;
                case "bx_idCardCollection": md = "身份采集"; break;
                case "bx_cardVerify": md = "采集验证"; break;
                case "bx_insurance": md = "获取支付链接"; break;
                case "pay_outcome": md = "到账查询"; break;
                case "bx_rejectPolicy": md = "作废支付"; break;
                case "bx_epolicyDownload": md = "电子保单下载"; break;
                case "bx_smsInform": md = "发送电子投保确认单"; break;
            }
            logMsg.Info(string.Format("【订单支付-{4}】向中心发送 消息地址：{0}, \n  参数：{1}, \n 返回结果：{2}{3}", url, postData, result, logSeparator, md));
            return result;
        }

        /// <summary>
        /// 通过代理人id查找城市编号，并找到对应的安心保险账户和密码
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        private bx_agent_answern_city GetModelByUId(long buid, int agentId = 0)
        {
            var userInfoModel = _userInfoRepository.FindByBuid(buid);
            if (userInfoModel == null || userInfoModel.Id == 0) return null;
            if (agentId == 0) agentId = userInfoModel.agent_id;

            var city_id = userInfoModel.CityCode;
            int cityId;
            bool resultParse = int.TryParse(city_id, out cityId);
            if (!resultParse) return null;
            var agentAnswernCity = _agentAnswernCityReposity
                                    .FirstOrDefault(c =>
                                        c.AgentId == agentId
                                        && c.CityId == cityId);
            return agentAnswernCity;
        }
    }
}
