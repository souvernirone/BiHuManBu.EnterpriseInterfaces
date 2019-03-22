using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces
{
    public interface IIsHaveLicensenoService
    {
        /// <summary>
        /// 判断一个顶级下面有其他人算过请求的车牌 /pc、微信、app
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        AgentNameViewModel IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type, int? repeatQuote);


        /// <summary>
        /// 根据当前代理人获取重复报价数据
        /// </summary>
        /// <param name="topAgentId">顶级代理人</param>
        /// <param name="agentId">当前代理人</param>
        /// <param name="licenseno">车牌号</param>
        /// <param name="vinNo">车架号</param>
        /// <param name="type"></param>
        /// <param name="repeatQuote"></param>
        /// <param name="isBehalfQuote">是否代报价</param>
        /// <returns></returns>
        AgentNameViewModel GetRepeatQuoteInfo(int topAgentId, int agentId, string licenseno, string vinNo, int type, int? repeatQuote, int isBehalfQuote);



    }
}
