using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.OrderService.Interfaces;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;

namespace BiHuManBu.ExternalInterfaces.Services.OrderService.Implementations
{
    public class GetPolicyService : IGetPolicyService
    {
        private const string logSeparator = "\r\n---------------------------------------------------------------------------------------- \r\n";
        private IOrderRepository _orderRepository;
        private readonly ILog logMsg = LogManager.GetLogger("MSG");
        private readonly ISetPolicyService _setPolicyService;
        private readonly IUserInfoRepository _userInfoRepository;
        public GetPolicyService(IOrderRepository orderRepository, ISetPolicyService setPolicyService, IUserInfoRepository userInfoRepository)
        {
            _orderRepository = orderRepository;
            _setPolicyService = setPolicyService;
            _userInfoRepository = userInfoRepository;
        }

        public BaseViewModel GetPolicyAllInfo(TPolicy request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderNum))
                return SetBaseViewModel(-10019, "订单号不存在!");
            var ddOrder = _orderRepository.FindOrder(request.OrderNum);
            if (ddOrder == null)
                return SetBaseViewModel(-10019, "订单号不存在!");
            //调用中心接口
            FindByTnoQueueInfoResponse centerMsg = GetCenterMethod(request);
            //对返回值做处理
            if (centerMsg == null)
                return SetBaseViewModel(-10019, "请求超时,请重试!");
            if (centerMsg.ErrCode != 0)
                return SetBaseViewModel(centerMsg.ErrCode, centerMsg.ErrMsg);
            //else

            var result = _setPolicyService.SetPolicyAllInfo(centerMsg, request);
            return SetBaseViewModel(result.BusinessStatus, result.StatusMessage);
        }

        /// <summary>
        /// 设置返回值
        /// </summary>
        /// <param name="code">消息码</param>
        /// <param name="msg">消息内容</param>
        /// <param name="data">结果</param>
        /// <returns></returns>
        private BaseViewModel SetBaseViewModel(int code, string msg, object data = null)
        {
            var model = new BaseViewModel();
            model.BusinessStatus = code;
            model.StatusMessage = msg;
            model.Data = data;
            return model;
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
                case "bx_findorderbytno": md = "投保单抓取"; break;
            }
            logMsg.Info(string.Format("【订单支付-{4}】向中心发送 消息地址：{0}, \n  参数：{1}, \n 返回结果：{2}{3}", url, postData, result, logSeparator, md));
            return result;
        }


        public FindByTnoQueueInfoViewModel GetPolicyAllInfoOut(TPolicy request)
        {
            #region 校验
            if (string.IsNullOrWhiteSpace(request.ForceTNo) && string.IsNullOrWhiteSpace(request.BizTNo))
            {
                return new FindByTnoQueueInfoViewModel()
                {
                    BusinessStatus = -10019,
                    StatusMessage = "投保单不能为空!"
                };
            }
            if (request.Source < 1)
            {
                return new FindByTnoQueueInfoViewModel()
                {
                    BusinessStatus = -10019,
                    StatusMessage = "保险公司值不能为空!"
                };
            }
            if (request.ChannelId < 1)
            {
                return new FindByTnoQueueInfoViewModel()
                {
                    BusinessStatus = -10019,
                    StatusMessage = "渠道不能为空!"
                };
            }
            if (string.IsNullOrWhiteSpace(request.CustKey))
            {
                return new FindByTnoQueueInfoViewModel()
                {
                    BusinessStatus = -10019,
                    StatusMessage = "用户标识CustKey不能为空!"
                };
            }
            #endregion
            #region 处理buid
            if (request.BuId == 0)
            {
                bx_userinfo userinfo = new bx_userinfo();
                request.ChildAgent = request.ChildAgent < 1 ? request.Agent : request.ChildAgent;
                string agent = request.Agent.ToString();
                if (request.Agent != request.ChildAgent)
                {
                    agent = request.ChildAgent.ToString();
                }
                userinfo = _userInfoRepository.Find(request.LicenseNo, request.CustKey, agent, 0);
                if (userinfo!=null&&userinfo.Id > 0)
                {
                    request.BuId = userinfo.Id;
                }
                else
                {
                    userinfo = new bx_userinfo();
                    //保存userinfo
                    userinfo.LicenseNo = request.LicenseNo;
                    userinfo.OpenId = request.CustKey;
                    userinfo.Agent = agent;
                    userinfo.top_agent_id = request.Agent;
                    userinfo.agent_id = request.ChildAgent;
                    userinfo.CreateTime = DateTime.Now;
                    userinfo.UpdateTime = DateTime.Now;
                    userinfo.RenewalType = request.RenewalType;
                    userinfo.CityCode = request.CityId.ToString();
                    userinfo.IsLastYear = 0;
                    userinfo.LastYearSource = -1;
                    userinfo.QuoteStatus = -1;
                    userinfo.RenewalStatus = -1;
                    userinfo.NeedEngineNo = 1;
                    userinfo.IsDistributed = 0;
                    userinfo.IsSingleSubmit = 0;
                    userinfo.Source = 0; userinfo.IsReView = 0;
                    userinfo.IsTest = 0;
                    userinfo.IsSingleLicenseno = 1;
                    var buid = _userInfoRepository.Add(userinfo);
                    request.BuId = buid;
                }
            }
            #endregion
            //调用中心接口
            FindByTnoQueueInfoResponse centerMsg = GetCenterMethod(request);
            //对返回值做处理
            if (centerMsg == null)
                return new FindByTnoQueueInfoViewModel()
                {
                    BusinessStatus = -10019,
                    StatusMessage = "请求超时,请重试!"
                };
            FindByTnoQueueInfoViewModel model = new FindByTnoQueueInfoViewModel();
            if (centerMsg.ErrCode == 0)
            {
                model.BusinessStatus = 1;
                model.StatusMessage = "获取信息成功";
                model.Quoteresult = centerMsg.quoteresult ?? new OrderQuoteresult();
                model.Order = centerMsg.order ?? new OrderInfo();
                model.Related = centerMsg.related ?? new OrderRelatedInfo();
                model.Savequote = centerMsg.savequote ?? new OrderSavequote();
            }
            else
            {
                model.BusinessStatus = 0;
                model.StatusMessage = centerMsg.ErrMsg;
                model.Quoteresult = new OrderQuoteresult();
                model.Order = new OrderInfo();
                model.Related = new OrderRelatedInfo();
                model.Savequote = new OrderSavequote();
            }
            return model;
        }

        /// <summary>
        /// 调用中心抓保单接口，返回通用信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private FindByTnoQueueInfoResponse GetCenterMethod(TPolicy request)
        {

            //传给炎龙的消息体参数 2018-01-10 19:10 wyy  
            request.NotifyCacheKey = Guid.NewGuid().ToString();
            request.Source = SourceGroupAlgorithm.GetOldSource(request.Source);
            var sendMode = new
            {
                request.Source,
                request.NotifyCacheKey,
                request.ForceTNo,
                request.BizTNo,
                request.ChannelId,
                B_Uid = request.BuId,
                request.LicenseNo,
                request.EngineNo,
                request.CarVin,
                request.CityId
            };
            //向中心发送消息
            SendToMessageCenter(CommonHelper.JsonSerializeObject(sendMode), "bx_findorderbytno");
            //缓存key
            var notifyCacheKey = string.Format("{0}-findorderbytno", request.NotifyCacheKey);
            var num = 0;
            FindByTnoQueueInfoResponse centerMsg = null;
            var PayWaitDate = Convert.ToInt32(CommonHelper.GetAppSettings("PayWaitDate"));
            do
            {
                num++;
                Thread.Sleep(1000);
                try
                {
                    centerMsg = CacheProvider.Get<FindByTnoQueueInfoResponse>(notifyCacheKey);
                }
                catch (Exception)
                {
                    logMsg.Info(string.Format("投保单抓取，中心返回数据异常：{0}", CacheProvider.Get<string>(notifyCacheKey)));
                }

            } while (centerMsg == null && num <= PayWaitDate);//1分钟
            logMsg.Info(string.Format("【订单支付-投保单抓取】中心返回消息：{0}", centerMsg == null ? "无数据返回" : CommonHelper.JsonSerializeObject(centerMsg)) + logSeparator);
            return centerMsg;
        }
    }
}
