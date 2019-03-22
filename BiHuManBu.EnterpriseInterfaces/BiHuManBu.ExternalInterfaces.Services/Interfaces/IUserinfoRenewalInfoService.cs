using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result.WChat;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IUserinfoRenewalInfoService
    {
        ///// <summary>
        ///// 根据buid获取bx_userinfo_renewal_info
        ///// </summary>
        ///// <param name="buid"></param>
        ///// <returns></returns>
        //bx_userinfo_renewal_info GetByBuid(int buid);

        ///// <summary>
        ///// 更新bx_userinfo_renewal_info
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //bool Update(bx_userinfo_renewal_info info);

        /// <summary>
        /// 根据buid更新clientMobile
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="clientMobile"></param>
        /// <returns></returns>
        bool UpdateClientMobileByBuid(int buid, string clientMobile);

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile(sql)
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<MinUserInfoRenewalInfo> FindBuIdAndClientMobileSql(List<long> buids);

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<MinUserInfoRenewalInfo> FindBuIdAndClientMobile(List<long> buids);
        /// <summary>
        /// 人保禁呼
        /// </summary>
        /// <param name="licenseno">车牌</param>
        /// <param name="cityid">城市id</param>
        /// <param name="source">禁呼保险公司</param>
        /// <returns></returns>
        bool IsForbidMobile(string licenseno, int cityid, int source, int childagent);
        /// <summary>
        /// 判断是否在一个顶级下面有其他人算过请求的车牌
        /// </summary>
        /// <param name="topAgentId">顶级代理人</param>
        /// <param name="agentId">代理人</param>
        /// <param name="licenseno">车牌</param>
        /// <param name="vinNo">车架号</param>
        /// <param name="type">请求类型1车牌2车架号</param>
        /// <returns></returns>
        AgentNameViewModel IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type);


        /// <summary>
        /// 添加车牌和关注微信的人的关系
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        BaseViewModel AddWChatLicenseNoOpenIdRelationship(string openId, string licenseNo, int topAgentId, int cityCode);

        string GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType, out int cityCode);

        GetOpenIdByLicensenoResult GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType);
        bx_userinfo_renewal_info GetByBuid(int buid);
        bool Update(bx_userinfo_renewal_info info);
    }
}
