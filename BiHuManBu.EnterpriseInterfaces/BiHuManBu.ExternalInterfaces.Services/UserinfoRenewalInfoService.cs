using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result.WChat;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class UserinfoRenewalInfoService : IUserinfoRenewalInfoService
    {
        private IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository;
        private IForbidRecordRepository _forbidRecordRepository;
        private IUserInfoRepository _userInfoRepository;
        private readonly ITwoLevelHaveLicensenoService _twoLevelHaveLicensenoService;
        public UserinfoRenewalInfoService(IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository, IUserInfoRepository iuUserInfoRepository, IForbidRecordRepository forbidRecordRepository, ITwoLevelHaveLicensenoService twoLevelHaveLicensenoService)
        {
            this._userinfoRenewalInfoRepository = _userinfoRenewalInfoRepository;
            _userInfoRepository = iuUserInfoRepository;
            _forbidRecordRepository = forbidRecordRepository;
            _twoLevelHaveLicensenoService = twoLevelHaveLicensenoService;
        }

        /// <summary>
        /// 根据Buid更新clientMobile
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="clientMobile"></param>
        /// <returns></returns>
        public bool UpdateClientMobileByBuid(int buid, string clientMobile)
        {
            if (!FormatHelper.IsMobile(clientMobile))
                return false;

            var info = _userinfoRenewalInfoRepository.GetByBuid(buid);

            if (info == null)
            {
                info = new bx_userinfo_renewal_info
                {
                    b_uid = buid,
                    client_mobile = clientMobile,
                    create_time = DateTime.Now
                };
                _userinfoRenewalInfoRepository.Insert(info);
                return _userinfoRenewalInfoRepository.SaveChanges() > 0;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(info.client_mobile))
                {
                    info.client_mobile = clientMobile;
                    _userinfoRenewalInfoRepository.Update(info);
                    return _userinfoRenewalInfoRepository.SaveChanges() > 0;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<MinUserInfoRenewalInfo> FindBuIdAndClientMobile(List<long> buids)
        {
            if (buids.Count == 0)
                return new List<MinUserInfoRenewalInfo>();

            return _userinfoRenewalInfoRepository.FindBuIdAndClientMobile(buids);
        }

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile(sql)
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<MinUserInfoRenewalInfo> FindBuIdAndClientMobileSql(List<long> buids)
        {
            if (buids.Count == 0)
                return new List<MinUserInfoRenewalInfo>();

            return _userinfoRenewalInfoRepository.FindBuIdAndClientMobileSql(buids);
        }

        /// <summary>
        /// 人保禁呼
        /// </summary>
        /// <param name="licenseno">车牌</param>
        /// <param name="cityid">城市id</param>
        /// <param name="source">禁呼保险公司</param>
        /// <returns></returns>
        public bool IsForbidMobile(string licenseno, int cityid, int source, int childagent)
        {
            var forbidMobile = _userInfoRepository.GetForbidMobile(licenseno, cityid, source);
            if (forbidMobile == null)
            {
                return false;
            }
            else
            {
                if (forbidMobile.id == 0)
                {
                    return false;
                }
                //添加禁呼记录
                bx_forbid_record model = new bx_forbid_record();
                model.forbid_record_id = forbidMobile.id;
                model.child_agent = childagent;
                model.create_time = DateTime.Now;
                _forbidRecordRepository.AddForbidRecord(model);
            }
            return true;//_userInfoRepository.IsForbidMobile(licenseno, cityid, source);
        }
        /// <summary>
        /// 判断一个顶级下面有其他人算过请求的车牌 /pc、微信、app
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public AgentNameViewModel IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type)
        {
            List<AgentNameViewModel> agentList = _userInfoRepository.IsHaveLicenseno(topAgentId, agentId, licenseno, vinNo, type);
            var agent = agentList.FirstOrDefault();
            //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
            if (topAgentId != agentId && agent != null && agent.AgentId == topAgentId)
            {
                string agentMd5 = agentId.ToString().GetMd5();
                _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agent.Buid, agentId);
                agent = null;
            }
            //20180129逻辑修改
            //下级能拿到上级的未分配的数据
            //_twoLevelHaveLicensenoService.
            //顶级试算下级已有车牌，不拦截跳转到下级对应车牌的详情页。
            if (topAgentId == agentId && agent != null && agent.AgentId != topAgentId)
            {
                agent.Type = 1;
            }
            return agent;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public BaseViewModel AddWChatLicenseNoOpenIdRelationship(string openId, string licenseNo, int topAgentId, int cityCode)
        {
            BaseViewModel viewModel = new BaseViewModel();
            int result = _userinfoRenewalInfoRepository.AddWChatLicenseNoOpenIdRelationship(openId, licenseNo, topAgentId, cityCode);
            if (result > 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加成功";
                return viewModel;
            }
            viewModel.BusinessStatus = -10017;
            viewModel.StatusMessage = "添加失败";
            return viewModel;

        }


        public string GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType, out int cityCode)
        {
            bx_agent_wchat wchat = _userinfoRenewalInfoRepository.GetOpenIdByLicenseNo(licenseNo, topAgentId, openId, requestType);
            if (wchat != null && wchat.id > 0)
            {
                cityCode = wchat.CityCode.HasValue ? wchat.CityCode.Value : 1;
                switch (requestType)
                {//1车牌->openid  2openid->车牌
                    case 1:
                        return wchat.open_id;

                    case 2:
                        return wchat.licenseno;

                }
            }
            cityCode = 0;
            return null;
        }

        public GetOpenIdByLicensenoResult GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType)
        {
            GetOpenIdByLicensenoResult result = new GetOpenIdByLicensenoResult();
            bx_agent_wchat wchat = _userinfoRenewalInfoRepository.GetOpenIdByLicenseNo(licenseNo, topAgentId, openId, requestType);
            if (wchat != null && wchat.id > 0)
            {
                result.CityCode = wchat.CityCode.HasValue ? wchat.CityCode.Value : 1;
                switch (requestType)
                {//1车牌->openid  2openid->车牌
                    case 1:
                        result.StrResult = wchat.open_id;
                        break;
                    case 2:
                        result.StrResult = wchat.licenseno;
                        break;
                }
                result.LicenseNo = wchat.licenseno;
                result.Openid = wchat.open_id;
                var userinfo = _userInfoRepository.FindByOpenIdAndLicense(result.Openid, result.LicenseNo);
                if (userinfo != null && userinfo.Id > 0)
                {
                    result.CarVIN = userinfo.CarVIN;
                    result.EngineNo = userinfo.EngineNo;
                    result.MoldName = userinfo.MoldName;
                }
                else
                {
                    result.CarVIN = "";
                    result.EngineNo = "";
                    result.MoldName = "";
                }
                return result;
            }
            return null;
        }
        public bx_userinfo_renewal_info GetByBuid(int buid)
        {
            return _userinfoRenewalInfoRepository.GetByBuid(buid);
        }
        public bool Update(bx_userinfo_renewal_info info)
        {
            _userinfoRenewalInfoRepository.Update(info);
            return true;
        }
    }
}
