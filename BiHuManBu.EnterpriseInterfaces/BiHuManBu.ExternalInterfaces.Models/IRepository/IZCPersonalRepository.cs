using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IZCPersonalRepository
    {
        /// <summary>
        /// 根据用户获取银行卡信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<bx_group_authen> GetBankCardMessage(int agentId);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        PersonInfoModel GetPerson(int agentId);
        /// <summary>
        /// 判断能否报价 sjy 2018/2/7
        /// </summary>
        /// <returns></returns>
        int AgentCanQuote(int childAgentId);
    }
}