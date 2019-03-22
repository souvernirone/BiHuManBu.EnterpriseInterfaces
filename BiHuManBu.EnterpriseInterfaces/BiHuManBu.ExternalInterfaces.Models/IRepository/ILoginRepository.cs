using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILoginRepository
    {
        manageruser Find(string username, string userpwd,bool checkPwd);
        List<bx_agent> FindAgent(string agentAccount,string pwd);
    
        List<manager_role_module_relation> GetManagerRoleModuleRelation(List<int> ids);
        List<manager_module_db> GetManagerModule(List<string> moduleCode);
        bool IsExist(string name);

        bool AddManagerUser(string name, string pwd, string mobile, string agentName, int agentType, string region, int isDaiLi, int shareCode, int regType, string address, bool isUsed, out string isExistAgent, int commodity, int platfrom, int repeatQuote, int accountType, string endDate, int openQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, int zhenBangType, Dictionary<int, int> dicSource, int configCityId, int openMultiple, int settlement, int structType, int desensitization, out bx_agent registedAgent,int  peopleType,int ceditOpenTuiXiu);
        List<manager_module_db> GetManagerModuleAlls();
        List<string> GetModuleCodes(string paterCode, int type);

        BiHuManBu.ExternalInterfaces.Models.ReportModel.AagentGroupAuthen GetAgentItemByAgentAccount(string agentAccount);
    
        manager_role_db GetRoleInfo(int roleId);
        List<bx_agent> GetAgentByAgentAccount(string agentAccount, string pwd);
        /// <summary>
        /// 校验邀请码适合合法
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        bool IsInvitationCode(int parentAgent);
        List<bx_agent_ukey> GetUkeyList(int agentId);

        int CreateUserToken(int agentid, string uniqueIdentifier, out string token);
        List<bx_agent_config> GetAgentConfigs(int agentId);
        bx_cityquoteday GetBxCityquoteday(int cityId);
        List<AgentDredgeCity> AgentDredgeCityList(int topAgentId);
        //int GetTopAgent(int agentId);
        List<bx_agent> CheckWChantUser(string openId);
        CqaLoginResultModel FindCqa(string username, string userpwd);
        int CreateWCahtAgentAccount(string opendId, string account, string passWord, manageruser manageruser,int agentId);
        int UpdateTopAgentId(int agentid, int topagentid);
        int IsExistMobile(int topAgentId, string mobile);
        List<AgentIdParentIdTopIdViewModel> GetAgentHaveCamera();
        bx_camera_config GetCameraConfig(int childagent);

        SfMobileLoginViewModel SfMobileLogin(string agentAccount, string agentPassWord);

        /// <summary>
        /// 微信用户授权登录 2018-09-18 张克亮 小V盟项目加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="agentId">顶级经济人Id</param>
        /// <returns></returns>
        BaseViewModel WeChatFind(string uniqueIdentifier,int agentId=0);
    }
}
