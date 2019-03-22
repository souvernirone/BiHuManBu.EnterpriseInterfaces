using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class EnterpriseAgentController : ApiController
    {
        private IEnterpriseAgentService _enterpriseAgentService;
        private ILog _logInfo;
        private ILog _logError;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enterpriseAgentService"></param>
        public EnterpriseAgentController(IEnterpriseAgentService enterpriseAgentService)
        {
            _enterpriseAgentService = enterpriseAgentService;
            _logInfo = LogManager.GetLogger("INFO");
            _logError = LogManager.GetLogger("ERROR");
        }
        /// <summary>
        /// 核保成功后经纪人、直客点位计算入库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage CollectRate([FromUri] RateRequest request)
        {

            try
            {
                _logInfo.Info("创建经纪人点位" + Request.RequestUri);

                return _enterpriseAgentService.CollectRate(request).ResponseToJson();
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return 1.ResponseToJson();

        }


        /// <summary>
        ///   //微信端没有核保,直客费率
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="buid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage WechatZhiKeRate(int agentId, long buid)
        {
            try
            {
                _logInfo.Info("微信点位查询" + Request.RequestUri);

                return _enterpriseAgentService.WechatZhiKeRate(agentId, buid).ResponseToJson();
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return 1.ResponseToJson();

        }

        /// <summary>
        /// 手机报价单,经纪人
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="channel"></param>
        /// <param name="systemBizRate"></param>
        /// <param name="source"></param>
        /// <returns></returns>

        [HttpGet]
        public HttpResponseMessage PhoneAgentRate(int agentId, int channel, double systemBizRate, int source)
        {
            try
            {
                _logInfo.Info("手机报价单查询" + Request.RequestUri);

                return _enterpriseAgentService.PhoneAgentRate(agentId, channel, systemBizRate, source).ResponseToJson();
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return 1.ResponseToJson();
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="smsAccountContentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage AddOrUpdateSmsAccountContent([FromBody] AddOrUpdateSmsAccountContentRequest smsAccountContentRequest)
        {
            BaseViewModel bv = new BaseViewModel();
            try
            {
                _logInfo.Info("短信发送" + Request.RequestUri);
                int result = _enterpriseAgentService.AddOrUpdateSmsAccountContent(smsAccountContentRequest);
                bv.BusinessStatus = result;
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                bv.BusinessStatus = -1;
            }

            return bv.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage UpdateBxCarOrderStatus(long bxOrderId, int status)
        {
            BaseViewModel bv = new BaseViewModel();
            try
            {

                int result = _enterpriseAgentService.UpdateBxCarOrderStatus(bxOrderId, status);
                bv.BusinessStatus = result;
            }
            catch (Exception ex)
            {

                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                bv.BusinessStatus = -1;
            }
            return bv.ResponseToJson();
        }

        /// <summary>
        /// 获取报价单的核保报价状态
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetQuoteStatus(string buid)
        {
            var bv = new BaseViewModel();
            try
            {

                return _enterpriseAgentService.GetQuoteStatus(buid).ResponseToJson();
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                bv.BusinessStatus = -1;
            }
            return bv.ResponseToJson();
        }



    }
}