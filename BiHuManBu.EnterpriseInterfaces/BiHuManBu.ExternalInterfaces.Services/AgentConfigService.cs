using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AgentConfigService : IAgentConfigService
    {
        private readonly IAgentConfigRepository _agentConfigRepository;
        private readonly ICacheService _cacheService;
        private readonly IAgentRepository _agentRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        private readonly CacheClient _cacheClient;
        private readonly string _hashKey = "BiHu.BaoXian.MappingTableManage";
        public AgentConfigService(IAgentConfigRepository agentConfigRepository, ICacheService cacheService, IAgentRepository agentRepository)
        {
            _agentConfigRepository = agentConfigRepository;
            _cacheService = cacheService;
            _agentRepository = agentRepository;
            //缓存 db
            this._cacheClient = new CacheClient(new RedisHashCache(Convert.ToInt32(GetAppSettings("dbNum"))));
        }
        public string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }
        /// <summary>
        /// 报价渠道是否可以开启 zky 2017-09-08 /crm
        /// (当前代理人同一个城市、同一个保险公司是否有已经开启的渠道)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanEditAgentConfigUsed(EditAgentUsedRequest request)
        {
            var agentConfig = _agentConfigRepository.GetList(t => t.city_id == request.CityId && t.source == request.Source && t.is_used == 1 && t.agent_id == request.AgentId).ToList().FirstOrDefault();
            return agentConfig == null;
        }

        /// <summary>
        /// 更新渠道的可用状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel UpdateCongfigUsed(EditAgentUsedRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            bool update = false;
            var agentSetting = _agentRepository.GetAgentSettingModel(request.AgentId);
            if (request.IsUsed == 1)
            {
                if (agentSetting == null || agentSetting.can_multiple_quote == 0)//单渠道报价
                {
                    bool canEdit = CanEditAgentConfigUsed(request);
                    if (!canEdit)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "保存失败,保险公司在同一城市已经有开启的渠道";
                        return viewModel;
                    }
                }
                else if (agentSetting.can_multiple_quote == 1)//开启了多渠道
                {
                    var countModel = _agentConfigRepository.GetAgentConfigCountList(t => t.city_id == request.CityId && t.source == request.Source && t.agent_id == request.AgentId).FirstOrDefault();
                    //多渠道报价配置允许开启的数量
                    var allowOpenCount = countModel != null ? countModel.config_count : 1;
                    //已经开启的数量
                    var hasOpenCount = _agentConfigRepository.GetList(t => t.source == request.Source && t.agent_id == request.AgentId && t.city_id == request.CityId && t.is_used == 1).Count;
                    if (hasOpenCount >= allowOpenCount)
                    {
                        viewModel.BusinessStatus = 0;
                        viewModel.StatusMessage = "保存失败,多渠道报价的数量超过到上限";
                        return viewModel;
                    }
                }
            }
            try
            {
                var config = _agentConfigRepository.GetList(t => t.id == request.ConfigId).FirstOrDefault();
                if (config != null)
                {
                    config.is_used = request.IsUsed;
                    config.update_time = DateTime.Now;
                    _agentConfigRepository.Update(config);
                    update = _agentConfigRepository.SaveChanges() > 0;

                    if (update)//清空缓存
                    {
                        var clearParam = new ClearUKeyCacheRequest();
                        clearParam.AgentIds = request.AgentId.ToString();
                        clearParam.CityId = request.CityId;
                        _cacheService.ClearUKeyCache(clearParam);
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        logError.Info(validationError.PropertyName + ":" + validationError.ErrorMessage);
                    }
                }
            }
            if (update)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 查询ukey的备用密码 zky 2017-10-25/crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UkeyBackupPwdViewModel GetUkeyBackupPwd(int id)
        {
            UkeyBackupPwdViewModel viewModel = new UkeyBackupPwdViewModel();
            var model = _agentConfigRepository.GetUkeyModel(id);
            if (model == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "查询失败";
            }
            viewModel.BusinessStatus = 1;
            if (string.IsNullOrWhiteSpace(model.backup_pwd_one) || string.IsNullOrWhiteSpace(model.backup_pwd_two) || string.IsNullOrWhiteSpace(model.backup_pwd_three))
            {
                viewModel.HasPassword = 0;
            }
            else
            {
                BackupPwd pwd = new BackupPwd()
                {
                    BackupPwdOne = model.backup_pwd_one,
                    BackupPwdTwo = model.backup_pwd_two,
                    BackupPwdThree = model.backup_pwd_three
                };
                viewModel.HasPassword = 1;
                viewModel.BackupPwd = pwd;
            }
            return viewModel;
        }

        /// <summary>
        /// 获取代理人多渠道报价数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IList<bx_agent_config_count> GetAgentConfigCountList(Expression<Func<bx_agent_config_count, bool>> where)
        {
            return _agentConfigRepository.GetAgentConfigCountList(where);
        }
        /// <summary>
        /// 根据ukey信息获取所有渠道的状态
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public List<AgentCacheChannelModel> GetUkeySource(List<bx_agent_ukey> configs)
        {
            var list = new List<AgentCacheChannelModel>();
            AgentCacheChannelModel model;
            //取哈希值的单条key，是配置表里的url作为key来取值的
            string redisUrlKey = string.Empty;
            foreach (var item in configs)
            {
                //获取渠道配置url
                redisUrlKey = item.isurl == 1 ? item.url : item.macurl;
                //获取单个渠道状态的模型
                model = new AgentCacheChannelModel();
                if (!string.IsNullOrEmpty(redisUrlKey))
                {
                    if (_cacheClient.KeyExists(_hashKey, redisUrlKey))
                    {
                        //if (_cacheClient.KeyExists(redisUrlKey))
                        //{
                        model = _cacheClient.Get<AgentCacheChannelModel>(_hashKey, redisUrlKey);
                        // }
                    }
                    //viewModel = _cacheClient.Get<List<AgentCacheChannelModel>>(_hashKey, agent.ToString());
                    //model = _cacheClient.Get<AgentCacheChannelModel>(_hashKey, redisUrlKey);
                    if (model != null)
                    {
                        // model.ChannelId = item.id;
                        //
                        list.Add(model);
                    }
                }

            }
            return list;
        }
    }
}
