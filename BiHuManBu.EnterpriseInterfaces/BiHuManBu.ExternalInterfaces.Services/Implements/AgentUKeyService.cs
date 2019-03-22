using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// AgentUKey业务逻辑
    /// </summary>
    public class AgentUKeyService : IAgentUKeyService
    {
        private readonly IAgentConfigRepository _agentConfigRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentUkeyRepository _agentUkeyRepository;
        private readonly ICacheService _cacheService;
        private readonly IAppVerifyService _appVerifyService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentConfigRepository"></param>
        /// <param name="agentRepository"></param>
        /// <param name="agentUkeyRepository"></param>
        /// <param name="cacheService"></param>
        /// <param name="appVerifyService"></param>
        public AgentUKeyService(IAgentConfigRepository agentConfigRepository, IAgentRepository agentRepository, IAgentUkeyRepository agentUkeyRepository, ICacheService cacheService
            , IAppVerifyService appVerifyService)
        {
            _agentConfigRepository = agentConfigRepository;
            _agentRepository = agentRepository;
            _agentUkeyRepository = agentUkeyRepository;
            _cacheService = cacheService;
            _appVerifyService = appVerifyService;
        }


        /// <summary>
        /// 批量更新UKey是否为报价渠道
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseViewModel> MultipleUpdateIsUsedAsync(MultipleUpdateIsUsedRequest request)
        {
            List<bx_agent_config> listConfig;

            //批量开启时检查批量列表中相同城市下相同的保险公司是否有已经开启的渠道
            if (request.IsUsed == 1)
            {
                //获取要批量开启渠道列表
                var willOpenList = _agentConfigRepository.GetList(t => request.ListConfigId.Contains(t.id));

                #region 多渠道报价拦截
                var agentSettingModel = _agentRepository.GetAgentSettingModel(request.AgentId);
                if (agentSettingModel == null || agentSettingModel.can_multiple_quote == 0)//单渠道报价
                {
                    //对要开启的保险公司进行分组
                    var sourceCount = willOpenList.Select(t => t.source.Value).GroupBy(t => t).Where(t => t.Count() >= 2).ToList();
                    //获取顶级下当前城市已经开启渠道的保险公司
                    var hasOpenList = _agentConfigRepository.GetList(t => t.agent_id == request.Agent && t.city_id == request.CityId && t.is_used == 1).Select(t => t.source.Value).ToList();
                    //把要开启的列表中已经开启的保险公司去掉(拿到没有开启的)
                    var noOpenList = willOpenList.Where(t => t.is_used == 0).Select(t => t.source.Value).ToList();
                    if (sourceCount.Count > 0) //要开启的保险公司列表中如果存在同一保险公司有2个及两个以上的禁止开启
                    {
                        return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "同一城市下禁止启用多个相同的保险公司");
                    }
                    else
                    {
                        //如果已经开启的列表hasOpenList中存在noOpenList中的数据则禁止开启，否则允许开启
                        if (hasOpenList.Where(t => noOpenList.Contains(t)).ToList().Count > 0)
                        {
                            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "选择的列表中已经有相同的保险公司开启了报价渠道");
                        }
                    }
                }
                else if (agentSettingModel.can_multiple_quote == 1)//多渠道报价
                {
                    var countList = _agentConfigRepository.GetAgentConfigCountList(t => t.city_id == request.CityId && t.agent_id == request.AgentId).ToList();
                    var hasOpenList = _agentConfigRepository.GetList(t => t.agent_id == request.AgentId && t.city_id == request.CityId && t.is_used == 1);
                    var souceList = willOpenList.Select(t => t.source).Distinct().ToList();
                    foreach (var item in souceList)
                    {
                        var hasOpenCount = hasOpenList.Where(t => t.source.Value == item).Count();
                        var allowOpenCount = countList.Where(t => t.source.Value == item).Count();
                        var willOpenCount = willOpenList.Where(t => t.agent_id == request.AgentId && t.city_id == request.CityId && t.source == item).Count();
                        if (willOpenCount + hasOpenCount > allowOpenCount)
                        {
                            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "多渠道报价的数量超过到上限");
                        }
                    }
                }
                #endregion
            }

            if (request.IsAll)
            {
                if (string.IsNullOrEmpty(request.QuDaoName))
                {
                    listConfig = await _agentConfigRepository.GetListAsync(o => o.agent_id == request.AgentId && o.city_id == request.CityId);
                }
                else
                {
                    listConfig = await _agentConfigRepository.GetListAsync(o => o.agent_id == request.AgentId && o.city_id == request.CityId && o.config_name.Contains(request.QuDaoName));
                }
            }
            else
            {
                if (request.ListConfigId == null || request.ListConfigId.Count == 0)
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "ListConfigId参数错误");

                listConfig = await _agentConfigRepository.GetListAsync(o => o.agent_id == request.AgentId && o.city_id == request.CityId && request.ListConfigId.Contains(o.id));
            }

            if (listConfig.Count == 0)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "数据不存在");
            }

            listConfig.ForEach(o =>
            {
                o.is_used = request.IsUsed;
                o.update_time = DateTime.Now;
                _agentConfigRepository.Update(o);
            });

            var result = await _agentConfigRepository.SaveChangesAsync();
            if (result > 0)
            {
                ClearUKeyCacheRequest clearRequest = new ClearUKeyCacheRequest();
                clearRequest.AgentIds = request.AgentId.ToString();
                clearRequest.CityId = request.CityId;
                _cacheService.ClearUKeyCache(clearRequest);
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        /// <summary>
        /// 分页获取ukey
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetPageUKeyViewModel GetPageUKey(GetPageUKeyRequest request)
        {
            if (request.OrgAgentId > 0)
            {
                //集团账号查看自己机构账号的渠道
                var orgAgent = _agentRepository.GetAgent(request.OrgAgentId);
                if (orgAgent.group_id != request.Agent)//验证当前机构是否是属于该集团账号
                {
                    GetPageUKeyViewModel viewModel = new GetPageUKeyViewModel();
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "查询失败，该机构不属于当前集团账号";
                    return viewModel;
                }
                return _agentConfigRepository.GetPageUKey(request.PageIndex, request.PageSize, request.QuDaoName, request.CityId, request.OrgAgentId);
            }

            //代理人查看自己的渠道
            return _agentConfigRepository.GetPageUKey(request.PageIndex, request.PageSize, request.QuDaoName, request.CityId, request.Agent);
        }

        public IQueryable<bx_agent_ukey> GetList(Expression<Func<bx_agent_ukey, bool>> where)
        {
            return _agentUkeyRepository.GetList(where);
        }

        public ChannelViewModel GetListChannel(AppAgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new ChannelViewModel() { BusinessStatus = 1, StatusMessage = "执行成功" };
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            var lisrchannel = _agentRepository.GetListChannel(request.ChildAgent);
            viewModel.List = lisrchannel;
            #endregion
            return viewModel;
        }

        public BaseViewModel UpdateChannelIsUsed(AgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "执行成功" };
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            var status = _agentRepository.UpdateChannelIsUesd(request);
            viewModel.BusinessStatus = status;
            viewModel.StatusMessage = status == 2 ? "此保险公司已经存在一个启用的渠道，请先关闭后再开启" : status == 0 ? "更新失败" : "执行成功";
            return viewModel;
        }

        public BaseViewModel EditAgentUKey(EditAgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "执行成功" };
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            string url = string.Format("{0}/api/AgentUKey/EditAgentUKey", ApplicationSettingsFactory.GetApplicationSettings().BaoJiaJieKou);
            string postData = string.Format("Agent={0}&SecCode={1}&UserCode={2}&UkeyId={3}&OldPassWord={4}&NewPassWord={5}&ReqSource={6}",
                request.Agent, request.SecCode, request.UserCode, request.UkeyId, request.OldPassWord, request.NewPassWord, request.ReqSource);
            string result;
            int ret = HttpWebAsk.Post(url, postData, out result);
            var model = JsonHelper.DeSerialize<BaseViewModel>(result);
            return model;
            #endregion

        }
    }
}
