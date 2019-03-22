using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAgentService
    {
        bx_agent GetAgent(int agentId);
        GetAgentIdentityAndRateResponse GetAgent(GetAgentIdentityAndRateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        bool EditAgentAndManagerUserRoleId(int agentId, int isUsed, int roleId);

        GetAgentViewModel QueryAgentInfo(int topAgentId, int agentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum);

        GetAgentAddViewModel CopyAgentInfoAdd(int id, int ShareCode);
        int EditAgentChangeTopAgent(int id);
        AgentDistributedViewModel GetAgentDistributedInfo(int parentId);

        //int UpdateAgenrReport(string topAgentId, DateTime startTime, DateTime endTime);

        string GetToken(int agentid, string uniqueIdentifier);
        IEnumerable<bx_agent> GetAgentChildrenList(string blurname, int agentid);
        /// <summary>
        /// 添加经纪人信息
        /// </summary>
        int AddAgentInfo(string agentName, int sourcce, out bxAgent agentItem);
        int DelAgentInfo(DeleteAgentRequest request);

        AgentSourceViewModel GetAgentSource(BaseVerifyRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        HebaoRateViewModel GetHebaoRate(GetHebaoRateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        bool IsExistAgentInfo(int id);
        bool HasAgent(string mobile, string shareCode);



        /// <summary>
        /// 根据agentIds获取 所有的AgentData
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<AgentData> GetAgentData(IEnumerable<string> agentIds);

        BusinessStatisticsViewModel GetBusinessStatistics(int agentId, DateTime startTime, DateTime endTime);

        DefeatAnalyticsViewModel GetDefeatAnalytics(int agentId, DateTime startTime, DateTime endTime);

        DefeatAnalyticsViewModel GetReasonAnalytics(int agentId, DateTime startTime, DateTime endTime);


        DefeatAnalyticsViewModel GetAgentAnalytics(AgentAnalyticsRequest request);
        TrendMapViewModel GetTrendMap(int agentId, DateTime startTime, DateTime endTime);

        AgentStatisticsViewModel GetAgentDataByPage(int agentId, DateTime startTime, DateTime endTime, bool isDesc, string orderBy, int curPage, int pageSize, string searchTtx,bool isByLevel);

        SfH5ViewModel GetAgentData4SfH5ByPage(int groupId, DateTime startTime, DateTime endTime, bool isDesc, string orderBy, int pageIndex, int pageSize, string serachText);

        SfH5AverageViewModel GetAverageData(int groupId, DateTime startTime, DateTime endTime);

        AgentStatisticsViewModel GetSingleAgentData(int agentId, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取代理人通话有效时长
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        int GetAgentEffectiveCallDuration(int agentId);
        /// <summary>
        /// 更新代理人通话有效时长
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <param name="effectiveCallDuration">有效时常</param>
        /// <returns></returns>
        bool UpdateAgentEffectiveCallDuration(int agentId, int effectiveCallDuration);
        /// <summary>
        /// 初始化代理人分组到缓存
        /// </summary>
        /// <returns></returns>
        void InitAgentGroupToRedis();
        /// <summary>
        /// 获取当前代理人所有下级
        /// </summary>
        /// <param name="currentAgentId">当前代理人编号</param>
        /// <param name="isHasSelf">是否包含自己</param>

        /// <returns></returns>
        List<int> GetSonsListFromRedis(int currentAgentId, bool isHasSelf = true);
        List<int> GetAllSonsListFromRedis(int currentAgentId, bool isHasSelf = true);
        List<int> GetDelSonListByDb(int currentAgent);
        /// <summary>
        /// 将GetSonsListFromRedis返回的int转换成string
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="isHasSelf">是否包含自己</param>
        /// <returns></returns>
        List<string> GetSonsListFromRedisToString(int currentAgentId, bool isHasSelf = true);
        List<string> GetAllSonsListFromRedisToString(int currentAgentId, bool isHasSelf = true);

        /// <summary>
        /// 添加新的代理人到redis
        /// </summary>
        /// <param name="currentAgentId">当前代理人编号</param>
        /// <param name="parentAgentId">父级代理人编号</param>
        /// <param name="agentLevel">当前代理人级别</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        void AddAgentGroupToRedis(int currentAgentId, int parentAgentId, int topAgentId, int agentLevel);

     

        /// <summary>
        /// 从redis中删除此代理人
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="parentAgentId">父级代理人编号</param>
        /// <param name="agentLevel">当前代理热级别</param>
        void DeleteAgentGroupFromRedis(int currentAgentId, int parentAgentId, int topAgentId, int agentLevel);
        /// <summary>
        /// 更新代理人
        /// </summary>
        /// <param name="currentAgentId">当前代理人编号</param>
        ///<param name="fromParentAgentIdKeys">原代理人集合</param>
        ///<param name="operationType">操作类型</param>
        ///<param name="toAgentIdKey">去向代理人</param>
        void UpdateAgentGroupInRedis(int currentAgentId, List<int> fromParentAgentIdKeys, int toAgentIdKey, int operationType);

        /// <summary>
        /// 更新代理人层级时通知redis接口
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <param name="parentAgentShareCode"></param>
        /// <param name="oldParentAgentId"></param>
        void UpdateAgentParentShareCodeNotifyRedis(int currentAgentId, int parentAgentShareCode, int oldParentAgentId);

        /// <summary>
        /// 设置代理手机号与微信同号  李金友 2017-09-08 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool SetAgentsPhoneIsWechat(SetPhoneAndWechatAgentRequest request);

        /// <summary>
        /// 获取代理人的所有下级代理人
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetSonAgentsViewModel GetSonAgents(BaseVerifyRequest request);
        /// ///<summary>
        /// 修改代理人账号密码 2017-08-06 zky /app
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passWord"></param>
        /// <param name="isUsed"></param>
        /// <param name="editAgent"></param>
        /// <returns></returns>
        bool EditAgentInfo(string account, string passWord, int isUsed, int editAgent);

        /// <summary>
        /// 除了当前代理人以外其他用户使用这个账号的数量 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        int SameAccountCount(int agentId, string account);
        /// <summary>
        /// 获取source
        /// </summary>
        /// <param name="source">source值</param>
        /// <param name="sourceType"> 0:传递新source  1：传递老source</param>
        /// <returns></returns>
        companyRelationModel GetSource(long source, int sourceType);

        /// <summary>
        /// 获取代理人名称和父级代理人Id，包括删除、禁用代理人
        /// </summary>
        /// <param name="listAgentId"></param>
        /// <returns></returns>
        List<AgentIdAndAgentName> GetAgentNames(List<int> listAgentId);
 
        IList<bx_agent> GetList(Expression<Func<bx_agent, bool>> where);

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds">批量更新的代理id</param>
        /// <param name="messagePayType">短信扣费方式</param>
        /// <param name="usedStatus">启用状态</param>
        /// <param name="isShowRate">是否展示费率</param>
        /// <param name="isSubmit">是否可核保</param>
        /// <returns></returns>
        bool AgentBatchAudit(List<int> agentIds, int messagePayType, int usedStatus, int isShowRate, int isSubmit);
        /// <summary>
        /// 获取全部代理人编号
        /// </summary>
        /// <returns></returns>
        List<int> GetAllAgentIds();
        /// <summary>
        /// 获取代理人的所有下级代理人
        /// </summary>
        /// <param name="agentId">顶级代理人</param>
        /// <param name="childAgent">当前代理人</param>
        /// <param name="isPaging">是否分页</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        GetSonAgentsViewModel GetSonAgents(int agentId, int childAgent);

        /// <summary>
        /// 获取代理人的所有下级代理人，并且删除或者禁用的在bx_userinfo 表中存在数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="childAgent"></param>
        /// <returns></returns>
        GetSonAgentsViewModel GetSonAgents2(int agentId, int childAgent);
        /// <summary>
        /// 获取集团账号下的机构列表 zky 2017-11-13 /crm、统计
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="authenState"></param>
        /// <param name="groupId"></param>
        /// <param name="needPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IList<OrgListDto> GetOrgList(string orgName, int authenState, int groupId, bool needPage,int orgId, int pageIndex, int pageCount, out int total);

        IList<OrgListDto> GetAgentIdNameList(int groupId);

        /// <summary>
        /// 是否可以更换上级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="shareCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CanChangeParentAgent(bx_agent curAgent, int shareCode, out string msg);

        CusListViewModel GetCustomerList(CustomerRequest request);

        bool IsTopAgentId(int agentId);
		
		 CusListViewModel GetCustomerListCount(CustomerRequest request);

        /// <summary>
        /// 获取集团下所有代理人
        /// </summary>
        /// <returns></returns>
        List<bx_agent> GetAgentIdAndNameByGroupId(string groupIds);

        /// <summary>
        /// 根据顶级代理人获取下级代理人数量
        /// 这里将获取到的数量放在缓存中，并且设置了30分钟的过期时间
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="hasSelf">是否包含自己，默认包含</param>
        /// <returns></returns>
        int GetSonAgentCountByTopAgentId(int topAgentId,bool hasSelf=true);

        /// <summary>
        /// 获取代理人配置信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bx_agent_setting GetAgentSettingModel(int agentId);

        /// <summary>
        /// 获取tx_agent表中的顶级信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        tx_agent GetBusinessModel(int topAgentId);

        List<bx_agent_config> GetAgentConfigs(int agentId, int modelType);
        List<bx_agent> GetAgentsByAgentIdAndModelTypeAndSearchType(int agentId, int modelType, int searchType);

        /// <summary>
        /// 获取代理人的下级、下下级列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        SonAndGrandsonViewModel AgentSonAndGrandson(int agentId, string createTime="");
      //  SonAndGrandsonViewModel AgentSonAndGrandson(int agentId);

        List<NewAgentCity> GetNewSourceList(string url, int agentId);

        NewAgentSourceViewModel NewGetAgentSource(BaseVerifyRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);




         int VerificationThirdAccount(int agentId);

         string GetAgentWhere(int RoleType, int ChildAgent, int Agent);
         string GetConditionSql(Tuple<List<int>, string> tuple);

        MobileStatisticsBaseVM<MobileDefeatAnalyticsVM> GetDefeatAnalytics4Mobile(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName);

        DefeatAnalyticsViewModel GetReasonAnalytics4Mobile(int agentId, DateTime startTime, DateTime endTime, string categoryName, int isViewAllData);
        /// <summary>
        /// 根据当前代理人获取其他代理人ID
        /// 1.当前代理人是二级代理人则获取其他同级二级代理人和当代理人的下级代理人（三级代理人）
        /// 2.当前代理人是三级代理人则获取其他同级三级代理人和当前代理人的父级代理人（二级代理人）
        /// </summary>
        /// <param name="childAgent"></param>
        /// <param name="flag">1：二级代理人和它的下级（三级）代理人；2：二级代理人和其他的二级代理人和其它二级代理人下级（三级）</param>
        /// <param name="isHasSelf"></param>
        /// <returns></returns>
        List<string> GetOtherAgentIdList(int childAgent, int flag, bool isHasSelf = true);
        List<string> GetOtherAgentList(int AgentId, int TopAgentId);
        bool VerifySecond(int agentId, int agentLevel);
        bx_agent VerifyLevel(int agentId);

        /// <summary>
        /// 获取短信设置
        /// </summary>
        /// <param name="agentId">经济人ID</param>
        /// <returns></returns>
        ShortMsgSettingResponse GetShortMsgSetting(int agentId);

        /// <summary>
        /// 设置短信设置
        /// </summary>
        /// <param name="request">短信设置数据模型</param>
        /// <returns></returns>
        bool SetShortMsgSetting(ShortMsgSettingRequest request);

        /// <summary>
        /// 根据顶级ID列表获取所有下级
        /// </summary>
        /// <param name="topAgentIds"></param>
        /// <returns></returns>
        List<int> GetChildAgentIdByTopAgentIds(List<int> topAgentIds);
    }
}
