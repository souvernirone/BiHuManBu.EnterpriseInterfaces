using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable FunctionComplexityOverflow

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class EnterpriseAgentService : IEnterpriseAgentService
    {
        private IEnterpriseAgentRepository _enterpriseAgentRepository;
        private IAgentRepository _agentRepository;
        public EnterpriseAgentService(IEnterpriseAgentRepository enterpriseAgentRepository,IAgentRepository iAgentRepository)
        {
            _enterpriseAgentRepository = enterpriseAgentRepository;
            _agentRepository = iAgentRepository;
        }
        /// <summary>
        ///  核保成功后经纪人、直客点位计算入库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int CollectRate(RateRequest request)
        {
            double agentKoudian = 0;
            double agentForceKoudian = 0;
            double zhikeFeiLv = 0;
            double systemBiz = 0;
            double systemForce = 0;
            //获取报价信息
            var userinfo = _enterpriseAgentRepository.FindUserinfo(request.Buid);
            //获取顶级经纪人
         
            bx_agent agent = _agentRepository.GetAgent(int.Parse(userinfo.Agent));
            int topAgentId = agent.TopAgentId;

            //获取渠道点位
            bx_agent_rate bxQudaoRate = new bx_agent_rate();
            var resultQudaoRate =  _enterpriseAgentRepository.FindQudaoRate(request.ChannelId, topAgentId);
            if (resultQudaoRate != null)
            {
                bxQudaoRate = resultQudaoRate;
                if (resultQudaoRate.rate_type_gd == 0)
                {
                    zhikeFeiLv = (resultQudaoRate.zhike_budian_rate ?? 0);
                }
                else
                {
                    zhikeFeiLv = (resultQudaoRate.zhike_koudian_rate ?? 0);
                }
            }
            //获取渠道特殊点位
            List<bx_agent_special_rate> bxQudaoSpecialRates =
                _enterpriseAgentRepository.FindQudaoSpecialRate(request.ChannelId, topAgentId);
            //获取经纪人点位
            bx_agent_rate bxAgentRate = new bx_agent_rate();
            var result = _enterpriseAgentRepository.FindAgentRate(int.Parse(userinfo.Agent), request.Source);
            if (result != null)
            {
                bxAgentRate = result;
                agentKoudian = (result.agent_rate ?? 0);
                agentForceKoudian = (result.rate_three ?? 0);
                if (topAgentId == int.Parse(userinfo.Agent))
                {
                    if (resultQudaoRate != null)
                    {
                        if (resultQudaoRate.rate_type_gd == 0)
                        {
                            var d = bxQudaoRate.agent_rate == null ? 0 : bxQudaoRate.fixed_rate;
                            if (d != null)
                                agentKoudian = (double)d;
                        }
                        else
                        {
                            var d = bxQudaoRate.agent_rate == null ? 0 : bxQudaoRate.fixed_discount;
                            if (d != null)
                                agentKoudian = (double)d;
                        }
                    }
                    else
                    {
                        agentKoudian = 0;
                    }
                    agentForceKoudian = (bxQudaoRate.rate_three ?? 0);
                }
            }
            else
            {
                if (resultQudaoRate != null)
                {
                    if (resultQudaoRate.rate_type_gd == 0)
                    {
                        agentKoudian = (bxQudaoRate.agent_default_bd_rate ?? 0);
                    }
                    else
                    {
                        agentKoudian = (bxQudaoRate.agent_default_kd_rate ?? 0);
                    }
                }
                else
                {
                    agentKoudian = 0;
                }
                agentForceKoudian = 0;
            }
            //获取经纪人特殊点位
            List<bx_agent_special_rate> bxAgentSpecialRates =
                _enterpriseAgentRepository.FindBxAgentSpecialRate(int.Parse(userinfo.Agent)).ToList();
            //获取核保报价信息
            var resultSubmitInfo = _enterpriseAgentRepository.FindBxSubmitInfo(request.Buid, request.Source);
            if (resultSubmitInfo != null)
            {
                var bxSubmitInfo = resultSubmitInfo;
                systemBiz = (double)(bxSubmitInfo.biz_rate ?? 0);
                systemForce = (double)(bxSubmitInfo.force_rate ?? 0);
            }
            //获取旧的报价点位
            bx_hebaodianwei oldHebaodianwei = _enterpriseAgentRepository.FindBxHebaodianwei(request.Buid, request.Source);
            foreach (bx_agent_special_rate rate in bxAgentSpecialRates)
            {
                if (rate.system_rate == systemBiz)
                {
                    agentKoudian = (double)rate.budian_rate;
                }
            }
            foreach (bx_agent_special_rate rate in bxQudaoSpecialRates)
            {
                if (rate.system_rate == systemBiz)
                {
                    zhikeFeiLv = (double)rate.zhike_rate;
                    if (topAgentId == int.Parse(userinfo.Agent))
                    {
                        agentKoudian = (double)rate.budian_rate;
                    }
                    if (result == null)
                    {
                        agentKoudian = (double)rate.agent_default_kd_rate;
                    }
                }
            }
            bx_hebaodianwei hebaodianwei = new bx_hebaodianwei();
            hebaodianwei.agent_biz_rate = (systemBiz - agentKoudian) < 0 ? 0 : (systemBiz - agentKoudian);
            hebaodianwei.agent_budian = agentKoudian;
            hebaodianwei.agent_force_rate = agentForceKoudian;
            hebaodianwei.buid = request.Buid;
            hebaodianwei.agent_tax_rate = bxAgentRate.rate_four;
            hebaodianwei.zhike_biz_rate = zhikeFeiLv;
            hebaodianwei.channel_id = request.ChannelId;
            hebaodianwei.agent_id = int.Parse(userinfo.Agent);
            hebaodianwei.parent_agent_id = topAgentId;
            hebaodianwei.system_biz_rate = systemBiz;
            hebaodianwei.system_force_rate = systemForce;
            hebaodianwei.create_time = DateTime.Now;
            hebaodianwei.channel_type = bxQudaoRate.rate_type_gd;
            hebaodianwei.car_license = request.CarLicense;
            hebaodianwei.source = request.Source;
            if (oldHebaodianwei == null)
            {
                return _enterpriseAgentRepository.AddHeBaoDianWei(hebaodianwei);
            }
            hebaodianwei.id = oldHebaodianwei.id;
            return _enterpriseAgentRepository.UpdateHeBaoDianWei(hebaodianwei);
        }
        public IEnumerable<bx_hebaodianwei> WechatZhiKeRate(int agentId, long buid)
        {
            double zhikeFeiLv = 0;
            //获取顶级经纪人
            int topAgentId = _enterpriseAgentRepository.GetTopAgentId(agentId);
            //点位
            List<bx_hebaodianwei> hebaodianweis = new List<bx_hebaodianwei>();
            List<bx_submit_info> submitInfos = new List<bx_submit_info>();
            submitInfos = _enterpriseAgentRepository.FindBxSubmitInfos(buid);
            foreach (bx_submit_info item in submitInfos)
            {
                bx_hebaodianwei hebaodianwei = new bx_hebaodianwei();
                var resultQudaoRate = _enterpriseAgentRepository.FindBuidRate(int.Parse(item.channel_id.ToString()), topAgentId, buid, item.source);
                if (resultQudaoRate != null)
                {
                    zhikeFeiLv = (double)(resultQudaoRate.zhike_biz_rate == null ? 0 : resultQudaoRate.zhike_biz_rate);
                    //if (resultQudaoRate.rate_type_gd == 0)
                    //{
                    //    zhikeFeiLv =
                    //        (double)(resultQudaoRate.zhike_budian_rate == null ? 0 : resultQudaoRate.zhike_budian_rate);
                    //}
                    //else
                    //{
                    //    zhikeFeiLv =
                    //        (double)(resultQudaoRate.zhike_koudian_rate == null ? 0 : resultQudaoRate.zhike_koudian_rate);
                    //}
                }
                hebaodianwei.zhike_biz_rate = zhikeFeiLv;
                hebaodianwei.source = item.source;
                hebaodianweis.Add(hebaodianwei);
            }
            return hebaodianweis;
        }
        public bx_hebaodianwei PhoneAgentRate(int agentId, int channel, double systemBizRate, int source)
        {
            double agentKoudian = 0;
            double agentForceKoudian = 0;
            double agent_tax_rate = 0;
            var result = _enterpriseAgentRepository.FindAgentRate(agentId, source);
            if (result != null)
            {
                agentKoudian = (double)(result.agent_rate == null ? 0 : result.agent_rate);
                agentForceKoudian = (double)(result.rate_three == null ? 0 : result.rate_three);
                agent_tax_rate = (double)(result.rate_four == null ? 0 : result.rate_four);
            }
            //获取经纪人特殊点位
            List<bx_agent_special_rate> bxAgentSpecialRates =
                _enterpriseAgentRepository.FindBxAgentSpecialRate(agentId);
            foreach (bx_agent_special_rate rate in bxAgentSpecialRates)
            {
                if (rate.system_rate == systemBizRate)
                {
                    agentKoudian = (double)rate.budian_rate;
                }
            }
            bx_hebaodianwei hebaodianwei = new bx_hebaodianwei();
            hebaodianwei.agent_biz_rate = systemBizRate - agentKoudian;
            hebaodianwei.agent_force_rate = agentForceKoudian;
            hebaodianwei.agent_tax_rate = agent_tax_rate;
            return hebaodianwei;
        }
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="smsAccountContentRequest"></param>
        /// <returns></returns>
        public int AddOrUpdateSmsAccountContent(AddOrUpdateSmsAccountContentRequest smsAccountContentRequest)
        {
            bx_sms_account_content smsContent = new bx_sms_account_content
            {
                create_time = DateTime.Now,
                license_no = smsAccountContentRequest.license_no ?? "",
                agent_id = smsAccountContentRequest.agent_id,
                agent_name = smsAccountContentRequest.agent_name,
                content = smsAccountContentRequest.content,
                sent_mobile = smsAccountContentRequest.sent_mobile,
                sent_type = smsAccountContentRequest.sent_type,
                sendstatus = 1,
                BatchId = -1,
                isdelete = 0

            };
            return _enterpriseAgentRepository.AddOrUpdateSmsAccountContent(smsContent);
        }
        public int UpdateBxCarOrderStatus(long bxOrderId, int status)
        {
            bx_car_order bxCarOrder = _enterpriseAgentRepository.FindBxCarOrder(bxOrderId);
            bxCarOrder.order_status = status;
            return _enterpriseAgentRepository.UpdateBxCarOrderStatus(bxCarOrder);
        }
        #region 获取报价，核保状态内容
        /// <summary>
        /// 获取报价核保状态
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public IEnumerable<GetQuoteStatusViewModel> GetQuoteStatus(string buid)
        {
            var bxQuoteStatusViewModels = new List<GetQuoteStatusViewModel>();
            List<bx_userinfo> bxUserinfos = _enterpriseAgentRepository.FindUserinfos(buid).ToList();
            List<bx_quoteresult> bxQuoteresultList = _enterpriseAgentRepository.BxQuoteresultList(buid);
            List<bx_submit_info> bxSubmitInfoList = _enterpriseAgentRepository.BxSubmitInfosList(buid);
            foreach (bx_userinfo bxUserinfo in bxUserinfos)
            {
                //提交了哪家报价
                var isSingleSubmitList = new List<long>();
                //提交了哪家核保
                var sourcelList = new List<int>();
                //提交报价的报价状态
                var quoteStatuslList = new List<long>();
                //提交了哪家报价 老的source0123 平太人国
                var isSingleSubmitListOld = new List<int>();
                if (bxUserinfo.IsSingleSubmit.HasValue)
                {
                    var listSource = SourceGroupAlgorithm.ParseSource(bxUserinfo.IsSingleSubmit.Value);
                    isSingleSubmitList.AddRange(listSource);
                    isSingleSubmitList.ForEach(o =>
                    {
                        isSingleSubmitListOld.Add(SourceGroupAlgorithm.GetOldSource(o));
                    });
                }

                if (bxUserinfo.QuoteStatus.HasValue)
                {
                    var listSource = SourceGroupAlgorithm.ParseSource(bxUserinfo.QuoteStatus.Value);
                    quoteStatuslList.AddRange(listSource);
                }

                if (bxUserinfo.QuoteStatus == 0)
                {
                    List<bx_submit_info> bxSubmitInfos = bxSubmitInfoList.Where(c => c.b_uid == bxUserinfo.Id).ToList();
                    foreach (long t in isSingleSubmitList)
                    {
                        //2018-09-11  is_repeat_submit>0: 重复投保                       
                        var bxSubmitInfTemp = bxSubmitInfos.FirstOrDefault(c => c.source == SourceGroupAlgorithm.GetOldSource(t));
                        bxQuoteStatusViewModels.Add(new GetQuoteStatusViewModel
                        {
                            Buid = bxUserinfo.Id,
                            QuoteResult = "报价失败",
                            QuoteStatus = bxSubmitInfTemp != null && bxSubmitInfTemp.is_repeat_submit > 0 ? "重复投保" : "报价失败",
                            QuoteResultToc = "报价失败",
                            SubmitResult = "未核保",
                            SubmitStatus = "未核保",
                            SubmitResultToc = "未核保",
                            Source = t
                        });
                    }
                    //bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusViewModel
                    //{
                    //    Buid = bxUserinfo.Id,
                    //    QuoteResult = "报价失败",
                    //    QuoteStatus = "报价失败",
                    //    QuoteResultToc = "报价失败",
                    //    SubmitResult = "未核保",
                    //    SubmitStatus = "未核保",
                    //    SubmitResultToc = "未核保",
                    //    Source = t
                    //}));
                }
                if (bxUserinfo.QuoteStatus == -1)
                {
                    bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusViewModel
                    {
                        Buid = bxUserinfo.Id,
                        QuoteResult = "未报价",
                        QuoteStatus = "未报价",
                        QuoteResultToc = "未报价",
                        SubmitResult = "未核保",
                        SubmitStatus = "未核保",
                        SubmitResultToc = "未核保",
                        Source = t
                    }));
                }
                if (bxUserinfo.QuoteStatus > 0)
                {
                    List<bx_quoteresult> bxQuoteresults =
                        bxQuoteresultList.Where(c => c.B_Uid == bxUserinfo.Id).ToList();                    
                    List<bx_submit_info> bxSubmitInfos = bxSubmitInfoList.Where(c => c.b_uid == bxUserinfo.Id).ToList();
                    for (int i = 0; i < isSingleSubmitList.Count; i++)
                    {
                        var bxModel = new GetQuoteStatusViewModel();
                        //bx_quoteresult模型
                        bx_quoteresult bxQuoteresult = bxQuoteresults.FirstOrDefault(c => c.Source == isSingleSubmitListOld[i]);
                        if (bxQuoteresult != null)
                        {
                            bxModel.BizTotal = bxQuoteresult.BizTotal.HasValue ? bxQuoteresult.BizTotal.Value : 0;
                            double force = bxQuoteresult.ForceTotal.HasValue ? bxQuoteresult.ForceTotal.Value : 0;
                            double tax = bxQuoteresult.TaxTotal.HasValue ? bxQuoteresult.TaxTotal.Value : 0;
                            bxModel.ForceTotal = force + tax;
                            //bx_submit_info模型
                            bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            string submitStatus = "未核保";
                            if (bxSubmitInfo != null)
                            {
                                switch (bxSubmitInfo.submit_status)
                                {
                                    case 0:
                                        submitStatus = "核保失败"; break;
                                    case 1:
                                        submitStatus = "核保成功";
                                        break;
                                    case 2:
                                        submitStatus = "未核保";
                                        break;
                                    case 3:
                                        submitStatus = "核保中";
                                        break;
                                }
                            }
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteResult = bxSubmitInfo != null ? bxSubmitInfo.quote_result : "";
                            bxModel.QuoteStatus = "报价成功";
                            bxModel.QuoteResultToc = bxSubmitInfo != null ? bxSubmitInfo.quote_result_toc : "";
                            bxModel.SubmitResult = bxSubmitInfo != null ? bxSubmitInfo.submit_result : "";
                            bxModel.SubmitStatus = submitStatus;
                            bxModel.SubmitResultToc = bxSubmitInfo != null ? bxSubmitInfo.submit_result_toc : "";
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                        else
                        {
                            bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteResult = bxSubmitInfo != null ? bxSubmitInfo.quote_result : "";
                            //2018-09-11  bx_submit_info.is_repeat_submit >0:重复投保
                            bxModel.QuoteStatus = bxSubmitInfo != null&&bxSubmitInfo.is_repeat_submit>0? "重复投保" : "报价失败";
                            bxModel.QuoteResultToc = bxSubmitInfo != null ? bxSubmitInfo.quote_result_toc : "";
                            bxModel.SubmitResult = "报价失败未核保";
                            bxModel.SubmitStatus = "未核保";
                            bxModel.SubmitResultToc = "报价失败未核保";
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                    }
                }
            }
            return bxQuoteStatusViewModels;
        }



        /// <summary>
        /// 获取报价核保状态
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public IEnumerable<GetQuoteStatusForAppViewModel> GetQuoteStatusForApp(string buid)
        {
            var bxQuoteStatusViewModels = new List<GetQuoteStatusForAppViewModel>();
            List<bx_userinfo> bxUserinfos = _enterpriseAgentRepository.FindUserinfos(buid).ToList();
            List<bx_quoteresult> bxQuoteresultList = _enterpriseAgentRepository.BxQuoteresultList(buid);
            List<bx_submit_info> bxSubmitInfoList = _enterpriseAgentRepository.BxSubmitInfosList(buid);
            foreach (bx_userinfo bxUserinfo in bxUserinfos)
            {
                //提交了哪家报价
                var isSingleSubmitList = new List<long>();
                //提交了哪家核保
                var sourcelList = new List<int>();
                //提交报价的报价状态
                var quoteStatuslList = new List<long>();
                //提交了哪家报价 老的source0123 平太人国
                var isSingleSubmitListOld = new List<int>();
                if (bxUserinfo.IsSingleSubmit.HasValue)
                {
                    var listSoucre = SourceGroupAlgorithm.ParseSource(bxUserinfo.IsSingleSubmit.Value);
                    isSingleSubmitList.AddRange(listSoucre);
                    isSingleSubmitList.ForEach(o =>
                    {
                        isSingleSubmitListOld.Add(SourceGroupAlgorithm.GetOldSource(o));
                    });
                }

                if (bxUserinfo.QuoteStatus.HasValue)
                {
                    var listSource = SourceGroupAlgorithm.ParseSource(bxUserinfo.QuoteStatus.Value);
                    quoteStatuslList.AddRange(listSource);
                }

                if (bxUserinfo.QuoteStatus == 0)
                {
                    bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusForAppViewModel
                    {
                        Buid = bxUserinfo.Id,
                        QuoteStatus = 0,
                        QuoteResult = "报价失败",
                        SubmitStatus = 2,
                        SubmitResult = "未核保",
                        Source = t
                    }));
                }
                if (bxUserinfo.QuoteStatus == -1)
                {
                    bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusForAppViewModel
                    {
                        Buid = bxUserinfo.Id,
                        QuoteStatus = -1,
                        QuoteResult = "未报价",
                        SubmitStatus = 2,
                        SubmitResult = "未核保",
                        Source = t
                    }));
                }
                if (bxUserinfo.QuoteStatus > 0)
                {
                    List<bx_quoteresult> bxQuoteresults =
                        bxQuoteresultList.Where(c => c.B_Uid == bxUserinfo.Id).ToList();
                    List<bx_submit_info> bxSubmitInfos = bxSubmitInfoList.Where(c => c.b_uid == bxUserinfo.Id).ToList();
                    for (int i = 0; i < isSingleSubmitList.Count; i++)
                    {
                        var bxModel = new GetQuoteStatusForAppViewModel();
                        //bx_quoteresult模型
                        bx_quoteresult bxQuoteresult = bxQuoteresults.FirstOrDefault(c => c.Source == isSingleSubmitListOld[i]);
                        if (bxQuoteresult != null)
                        {
                            bxModel.BizTotal = bxQuoteresult.BizTotal.HasValue ? bxQuoteresult.BizTotal.Value : 0;
                            double force = bxQuoteresult.ForceTotal.HasValue ? bxQuoteresult.ForceTotal.Value : 0;
                            double tax = bxQuoteresult.TaxTotal.HasValue ? bxQuoteresult.TaxTotal.Value : 0;
                            bxModel.ForceTotal = force + tax;
                            //bx_submit_info模型
                            bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            string submitStatus = "未核保";
                            if (bxSubmitInfo != null)
                            {
                                switch (bxSubmitInfo.submit_status)
                                {
                                    case 0:
                                        submitStatus = "核保失败"; break;
                                    case 1:
                                        submitStatus = "核保成功";
                                        break;
                                    case 2:
                                        submitStatus = "未核保";
                                        break;
                                    case 3:
                                        submitStatus = "核保中";
                                        break;
                                }
                            }
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteStatus = 1;
                            bxModel.QuoteResult = "报价成功";
                            bxModel.SubmitStatus = bxSubmitInfo != null ? (bxSubmitInfo.submit_status ?? 2) : 2;
                            bxModel.SubmitResult = submitStatus;
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                        else
                        {
                            //bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteStatus = 0;
                            bxModel.QuoteResult = "报价失败";
                            bxModel.SubmitStatus = 2;
                            bxModel.SubmitResult = "未核保";//报价失败未核保
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                    }
                }
            }
            return bxQuoteStatusViewModels;
        }

        /// <summary>
        /// 获取报价核保状态
        /// 根据buid获取
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public IEnumerable<GetQuoteStatusForAppViewModel> GetQuoteStatusByBuidForApp(bx_userinfo bxUserinfo, List<bx_quoteresult> bxQuoteresultList, List<bx_submit_info> bxSubmitInfoList)
        {
            var bxQuoteStatusViewModels = new List<GetQuoteStatusForAppViewModel>();
            if (bxUserinfo==null)
            {
                return bxQuoteStatusViewModels;
            }
            //foreach (bx_userinfo bxUserinfo in bxUserinfos)
            //{
                //提交了哪家报价
                var isSingleSubmitList = new List<long>();
                //提交了哪家核保
                var sourcelList = new List<int>();
                //提交报价的报价状态
                var quoteStatuslList = new List<long>();
                //提交了哪家报价 老的source0123 平太人国
                var isSingleSubmitListOld = new List<int>();
                if (bxUserinfo.IsSingleSubmit.HasValue)
                {
                    var listSoucre = SourceGroupAlgorithm.ParseSource(bxUserinfo.IsSingleSubmit.Value);
                    isSingleSubmitList.AddRange(listSoucre);
                    isSingleSubmitList.ForEach(o =>
                    {
                        isSingleSubmitListOld.Add(SourceGroupAlgorithm.GetOldSource(o));
                    });
                }

                if (bxUserinfo.QuoteStatus.HasValue)
                {
                    var listSource = SourceGroupAlgorithm.ParseSource(bxUserinfo.QuoteStatus.Value);
                    quoteStatuslList.AddRange(listSource);
                }

                if (bxUserinfo.QuoteStatus == 0)
                {
                    bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusForAppViewModel
                    {
                        Buid = bxUserinfo.Id,
                        QuoteStatus = 0,
                        QuoteResult = "报价失败",
                        SubmitStatus = 2,
                        SubmitResult = "未核保",
                        Source = t
                    }));
                }
                if (bxUserinfo.QuoteStatus == -1)
                {
                    bxQuoteStatusViewModels.AddRange(isSingleSubmitList.Select(t => new GetQuoteStatusForAppViewModel
                    {
                        Buid = bxUserinfo.Id,
                        QuoteStatus = -1,
                        QuoteResult = "未报价",
                        SubmitStatus = 2,
                        SubmitResult = "未核保",
                        Source = t
                    }));
                }
                if (bxUserinfo.QuoteStatus > 0)
                {
                    List<bx_quoteresult> bxQuoteresults =
                        bxQuoteresultList.Where(c => c.B_Uid == bxUserinfo.Id).ToList();
                    List<bx_submit_info> bxSubmitInfos = bxSubmitInfoList.Where(c => c.b_uid == bxUserinfo.Id).ToList();
                    for (int i = 0; i < isSingleSubmitList.Count; i++)
                    {
                        var bxModel = new GetQuoteStatusForAppViewModel();
                        //bx_quoteresult模型
                        bx_quoteresult bxQuoteresult = bxQuoteresults.FirstOrDefault(c => c.Source == isSingleSubmitListOld[i]);
                        if (bxQuoteresult != null)
                        {
                            bxModel.BizTotal = bxQuoteresult.BizTotal.HasValue ? bxQuoteresult.BizTotal.Value : 0;
                            double force = bxQuoteresult.ForceTotal.HasValue ? bxQuoteresult.ForceTotal.Value : 0;
                            double tax = bxQuoteresult.TaxTotal.HasValue ? bxQuoteresult.TaxTotal.Value : 0;
                            bxModel.ForceTotal = force + tax;
                            //bx_submit_info模型
                            bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            string submitStatus = "未核保";
                            if (bxSubmitInfo != null)
                            {
                                switch (bxSubmitInfo.submit_status)
                                {
                                    case 0:
                                        submitStatus = "核保失败"; break;
                                    case 1:
                                        submitStatus = "核保成功";
                                        break;
                                    case 2:
                                        submitStatus = "未核保";
                                        break;
                                    case 3:
                                        submitStatus = "核保中";
                                        break;
                                }
                            }
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteStatus = 1;
                            bxModel.QuoteResult = "报价成功";
                            bxModel.SubmitStatus = bxSubmitInfo != null ? (bxSubmitInfo.submit_status ?? 2) : 2;
                            bxModel.SubmitResult = submitStatus;
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                        else
                        {
                            //bx_submit_info bxSubmitInfo = bxSubmitInfos.FirstOrDefault(c => c.source == isSingleSubmitListOld[i]);
                            bxModel.Buid = bxUserinfo.Id;
                            bxModel.QuoteStatus = 0;
                            bxModel.QuoteResult = "报价失败";
                            bxModel.SubmitStatus = 2;
                            bxModel.SubmitResult = "未核保";//报价失败未核保
                            bxModel.Source = isSingleSubmitList[i];
                            bxQuoteStatusViewModels.Add(bxModel);
                        }
                    }
                }
            //}
            return bxQuoteStatusViewModels;
        }
        #endregion
    }
}
