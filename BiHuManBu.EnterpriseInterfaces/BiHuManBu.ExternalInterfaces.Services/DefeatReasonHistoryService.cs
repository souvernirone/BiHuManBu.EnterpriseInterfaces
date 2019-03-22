using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class DefeatReasonHistoryService : IDefeatReasonHistoryService
    {
        readonly IDefeatReasonHistoryRepository _defeatReasonHistoryRepository;
        readonly IAgentRepository _agentRepository;
        readonly ILog _logInfo = LogManager.GetLogger("INFO");
        readonly ILog _logError = LogManager.GetLogger("ERROR");
        private readonly IAgentService _agentService;
        private IUserInfoRepository _userInfoRepository;

        public DefeatReasonHistoryService(IDefeatReasonHistoryRepository defeatReasonHistoryRepository, IAgentRepository agentRepository, IAgentService agentService, IUserInfoRepository userInfoRepository)
        {
            _defeatReasonHistoryRepository = defeatReasonHistoryRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
            _userInfoRepository = userInfoRepository;
        }

        public bool AddToDefeatReasonHistory(SingleDefeatReasonDataViewModel singleDefeatReasonDataViewModel)
        {
            var defeatreasonhistory = new bx_defeatreasonhistory();
            defeatreasonhistory.AgentId = singleDefeatReasonDataViewModel.AgentId;
            defeatreasonhistory.AgentName = singleDefeatReasonDataViewModel.AgentName;
            defeatreasonhistory.CreateTime = DateTime.Now;
            defeatreasonhistory.DefeatReasonId = singleDefeatReasonDataViewModel.DefeatReasonId;
            defeatreasonhistory.Deleted = singleDefeatReasonDataViewModel.IsDelete;
            defeatreasonhistory.LicenseNo = singleDefeatReasonDataViewModel.LicenseNo;
            defeatreasonhistory.LicenseOwner = singleDefeatReasonDataViewModel.LicenseOwner;
            defeatreasonhistory.MoldName = singleDefeatReasonDataViewModel.MoldName;

            defeatreasonhistory.UpdateTime = DateTime.Now;
            defeatreasonhistory.BuId = singleDefeatReasonDataViewModel.BuId;
            return _defeatReasonHistoryRepository.AddToDefeatReasonHistory(defeatreasonhistory);
        }

        public List<DefeatReasonDataViewModel> GetDefeatReasonHistory(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, out int totalCount)
        {
            //接待员需要看到和顶级一样的数据
            List<int> agentIds;
            if (seachDefeatReasonHistoryCondition.AgentRoleType == 5)
            {
                agentIds = _agentService.GetSonsListFromRedis(seachDefeatReasonHistoryCondition.Agent).ToList();
            }
            else
            {
                agentIds = _agentService.GetSonsListFromRedis(seachDefeatReasonHistoryCondition.AgentId).ToList();
            }

            return _defeatReasonHistoryRepository.GetDefeatReasonHistory(seachDefeatReasonHistoryCondition, agentIds, out totalCount);
        }

        public int GetDefeatReasonHistoryCount(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition)
        {
            //接待员需要看到和顶级一样的数据
            List<int> agentIds;
            if (seachDefeatReasonHistoryCondition.AgentRoleType == 5)
            {
                agentIds = _agentService.GetSonsListFromRedis(seachDefeatReasonHistoryCondition.Agent).ToList();
            }
            else
            {
                agentIds = _agentService.GetSonsListFromRedis(seachDefeatReasonHistoryCondition.AgentId).ToList();
            }
            return _defeatReasonHistoryRepository.GetDefeatReasonHistoryCount(seachDefeatReasonHistoryCondition, agentIds);
        }
        public void DeleteDefeatReasonHistory(long buId)
        {
            _defeatReasonHistoryRepository.DeleteDefeatReasonHistory(buId);
        }

        public MobileStatisticsBaseVM<DefeatReasonMobileDetails> GetDefeatAnalyticsDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName)
        {
            int totalCount = 0;
            var result = _defeatReasonHistoryRepository.GetDefeatAnalyticsDetailsByPage(startTime, endTime, topAgentId, isViewAllData, pageIndex, pageSize, categoryName, out totalCount);
            return new MobileStatisticsBaseVM<DefeatReasonMobileDetails>() { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }

        /// <summary>
        /// 战败数据刷新续保（刷新历史数据）
        /// </summary>
        /// <returns></returns>
        public BaseViewModel UpdateDefeatHistoryOldList()
        {
            BaseViewModel viewModel = new BaseViewModel();
            //获取最大buid
            long maxBuid = _userInfoRepository.GetMaxBuid();
            long startBuid = 1;
            long interval = 10000;
            long endBuid = 10000;
            while (startBuid > maxBuid && endBuid > maxBuid)
            {
                try
                {
                    List<UserInfoModel2> list = _userInfoRepository.GetDefeatHistoryUserList(startBuid, endBuid);
                    GetReInfo(list);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("开始BUID：(" + startBuid + "),结束BUId：(" + endBuid + ")；发生异常：" + ex);
                    try
                    {
                        List<UserInfoModel2> list = _userInfoRepository.GetDefeatHistoryUserList(startBuid, endBuid);
                        GetReInfo(list);
                    }
                    catch (Exception ex1)
                    {
                        LogHelper.Error("开始BUID：(" + startBuid + "),结束BUId：(" + endBuid + ")；发生异常：" + ex1);
                    }
                }
                finally
                {
                    startBuid = startBuid + 1;
                    endBuid = startBuid + interval;
                }

            }
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "战败数据刷新续保成功";
            return null;
        }
        private void GetReInfo(List<UserInfoModel2> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }
            foreach (UserInfoModel2 userModel in data)
            {
                string url = CreateReInfoUrl(userModel);
                LogHelper.Info("战败续保URL：" + url);
                string result = HttpHelper.GetHttpClientAsync(url);
            }
        }
        private string CreateReInfoUrl(UserInfoModel2 model)
        {
            Dictionary<string, string> queryparm = new Dictionary<string, string>();
            queryparm.Add("ChildAgent", model.Agent.ToString());
            queryparm.Add("CustKey", model.OpenId.ToString());
            if (!string.IsNullOrWhiteSpace(model.LicenseNo.ToString()))
            {
                queryparm.Add("LicenseNo", model.LicenseNo.ToUpper());
                if (!string.IsNullOrWhiteSpace(model.SixDigitsAfterIdCard))
                {
                    queryparm.Add("SixDigitsAfterIdCard", model.SixDigitsAfterIdCard);
                }
                //号牌种类
                queryparm.Add("RenewalCarType", model.RenewalCarType.Value.ToString());
            }
            //获取号牌种类默认值为1（获取）  
            queryparm.Add("ShowRenewalCarType", "1");
            queryparm.Add("CityCode", model.CityCode);
            queryparm.Add("Group", "1");//续保新接口
            if (!string.IsNullOrEmpty(model.CarVIN))
            {
                queryparm.Add("CarVin", model.CarVIN.ToUpper());
            }
            if (!string.IsNullOrEmpty(model.EngineNo))
            {
                queryparm.Add("EngineNo", model.EngineNo.ToUpper());
            }
            queryparm.Add("CanShowNo", "1");
            queryparm.Add("Agent", model.TopAgentId.ToString());
            queryparm.Add("RenewalType", "4");
            queryparm.Add("ShowXiuLiChangType", "1");
            queryparm.Add("ShowInnerInfo", "1");
            queryparm.Add("ShowAutoMoldCode", "1");
            //llj edit 2017.05.24 
            queryparm.Add("ShowFybc", "1");//修理期间费用补偿险：0:（默认）:否  1：是
            queryparm.Add("ShowSheBei", "1");//新增设备险种：0:（默认）:否  1：是
            //llj add 2017.09.15 关系人 性别和出生日期
            queryparm.Add("ShowRelation", "1");
            //添加是否展示 三责险附加法定节假日限额翻倍险 
            queryparm.Add("ShowSanZheJieJiaRi", "1");
            //按照实时起保返回到期日期
            queryparm.Add("TimeFormat", "1");
            string url = ConfigurationManager.AppSettings["BaoJiaJieKou"];
            url = url + "api/CarInsurance/getreinfo" + queryparm.ToQueryString();
            return url;

        }
    }
}
