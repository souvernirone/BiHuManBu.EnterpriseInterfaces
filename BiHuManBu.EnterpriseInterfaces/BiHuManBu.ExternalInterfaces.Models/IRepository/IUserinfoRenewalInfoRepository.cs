using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserinfoRenewalInfoRepository : IRepositoryBase<bx_userinfo_renewal_info>
    {
        int Add(bx_userinfo_renewal_info bxWorkOrder);
        int Update(bx_userinfo_renewal_info bxWorkOrder);
        bx_userinfo_renewal_info FindById(int workOrderId);

        /// <summary>
        /// 根据buid获取
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        bx_userinfo_renewal_info GetByBuid(int buid);

        bx_userinfo_renewal_info FindByBuid(long buid);

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<MinUserInfoRenewalInfo> FindBuIdAndClientMobile(List<long> buids);

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile(sql)
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<MinUserInfoRenewalInfo> FindBuIdAndClientMobileSql(List<long> buids);

        /// <summary>
        /// 添加车牌和关注微信的人的关系
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        int AddWChatLicenseNoOpenIdRelationship(string openId, string licenseNo, int topAgentId, int cityCode);

        /// <summary>
        /// 通过车牌，顶级代理获取openid
        /// </summary>
        /// <returns></returns>
        bx_agent_wchat GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType);
    }
}
