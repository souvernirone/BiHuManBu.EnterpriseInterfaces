using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public  interface IUserInfoService
    {
        /// <summary>
        /// 根据topAgentId和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        List<long> GetBuidsByTopAgentAndIsTest2(int topAgentId, int isTest);
        List<UserInfoIdAgentModel> GetBuidsByTopAgentAndIsTest(int topAgentId, int isTest);

        List<bx_userinfo> GetUserInfoByLicenseNo(string LicenseNo, int AgentId);
        
    }
}
