using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public interface IAgentRepository
    {
        bx_agent GetAgent(int agentId);

        bx_agent GetAgent(string openId);

        bx_agent GetAgentByAgentAccount(string agentAccount);
        IList<string> GetUsedSons(int agentId);
        List<bx_agent> GetAgentById(List<int> agentIds);
        /// <summary>
        /// 根据代理Id获取姓名
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        string GetAgentName(int agentId);

        /// <summary>
        /// 获取代理人名称，父级代理人Id
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<AgentIdAndAgentName> GetAgentName(List<int> agentIds);
        /// <summary>
        /// 获取代理人名称，父级代理人Id包括删除禁用
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<AgentIdAndAgentName> GetAllAgentName(List<int> agentIds, int curAgentId, int agentLevel);
        /// <summary>
        /// 获取代理的顶级ID
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        string GetTopAgentId(int currentAgent);

        List<AgentAndTopAgent> GetTopAgentByIds(string agentIds);

        /// <summary>
        /// 根据id和agentname模糊查询所有子集
        /// </summary>
        /// <param name="curAgentId"></param>
        /// <param name="curAgentName"></param>
        /// <param name="hasSelf"></param>
        /// <returns></returns>
        List<string> GetSonsList(int curAgentId, string curAgentName, bool hasSelf = true);

        List<bx_agent> GetAgentByAgentAccount(string agentAccount, string pwd);

        bool AddAgent(string agentName, string mobile, int agentType, string region, string name, string pwd, int isDaiLi, int shareCode, int regType, string address, bool isUsed, int agentLevel, out bx_agent agentItem, int ManagerRoleId, int commodity, int platfrom, int repeatQuote, int accountType, DateTime? endDate, int openQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, int zhenBangType, int peopleType);

        bx_agent EditAgent(int agentId);

        string GetParentAgentName(int parentAgent);

        AagentGroupAuthen GetAgentItemByAgentAccount(string agentAccount);

        bx_agent GetTopAgent(int agentId);

        bool EditAgentAndManagerUserRoleId(int agentId, int isUsed, int roleId);

        List<bx_agent> QueryAgentInfo(int topAgentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum);

        List<bx_agent> QueryAgentInfo(int topAgentId, int agentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum);

        bool CopyAgentInfoAdd(int id, int ShareCode);

        bool IsExistAgentInfo(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sonAgentIds">这里的代理人不包含自己</param>
        /// <returns></returns>
        int EditAgentChangeTopAgent(int id, List<string> sonAgentIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="listSonAgent"></param>
        /// <returns></returns>
        List<bx_agent> GetAgentDistributedInfo(int parentId, List<int> listSonAgent);

        int GetAgentIsSue(int agentId);

        //int UpdateAgenrReport(string topAgentId, DateTime startTime, DateTime endTime);

        string GetRegion(int region);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="quoteSuccessNum"></param>
        /// <param name="quoteFailNum"></param>
        /// <param name="notQuoteNum"></param>
        /// <param name="sTime"></param>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        int GetQuoteCalculation(int agentId, out int quoteSuccessNum, out int quoteFailNum, out int notQuoteNum, DateTime sTime, string agentIds);

        string GetToken(int agentid, string uniqueIdentifier);
        /// <summary>
        /// 获取某个agent所属的 某个顶级代理人下面的所有经纪人
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<string> GetAllChildrenByAgentId(int agentId);

        IList<bx_agent> FindAgentChildrenList(List<string> agentlist, string blurname);
        /// <summary>
        /// 添加经纪人信息
        /// </summary>
        int AddAgentInfo(string agentName, int sourcce, out bx_agent agentItem);

        /// <summary>
        /// 删除代理人信息 （只删除没有下级代理人的信息）
        /// 新增顶级的逻辑删除功能
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <returns>0:报错；1:成功；2:失败；3:删除顶级代理人时，顶级代理人已经是删除状态</returns>
        int DelAgentInfo(int agentId, string agentName, int deleteUserId, string deleteAccount, int deletePaltform);

        List<bx_agent> FindList();

        /// <summary>
        /// 通过ID集合查询列表
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<AgentNameViewModel> FindAgentList(string agentIds);

        /// <summary>
        /// 查询agentIds所有的统计数据
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<AgentData> GetAgentData(IEnumerable<string> agentIds);

        /// <summary>
        /// 查询agentId下的所有的统计数据
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        BusinessStatisticsViewModel GetBusinessStatistics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime);

        List<DefeatAnalysis> GetDefeatAnalytics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime);

        List<DefeatAnalysis> GetReasonAnalytics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime);
        string GetAgentAnalytics(AgentAnalyticsRequest request, IList<string> agentIds, ref string defeatAnalysis);

        List<AgentData> GetTrendMap(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime);

        List<AgentData> GetAgentDataByPage(int agentId, DateTime startTime, DateTime endTime,
            bool isDesc, string orderBy, int curPage, int pageSize, string searchTxt, bool isByLevel, ref int totalCount);

        List<AgentData> GetSingleAgentData(int agentId, DateTime startTime, DateTime endTime);

        List<SfAgentData> GetAgentData4SfH5ByPage(int groupId, DateTime startTime, DateTime endTime, bool isDesc, string orderBy, int pageIndex, int pageSize, string serachText, ref int totalCount);

        SfH5AverageViewModel GetAverageData(int groupId, DateTime startTime, DateTime endTime);

        bool HasAgent(string mobile, string shareCode);

        /// <summary>
        /// 获取顶级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetTopAgentIdByAgentId(int agentId);

        /// <summary>
        /// 获取角色名称
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        string GetRoleNameByAgentId(int agentId);

        /// <summary>
        /// 获取所有可用的顶级代理人id
        /// </summary>
        /// <returns></returns>
        List<int> GetAllTopAgentId();


        /// <summary>
        /// 更新agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool UpdateAgent(bx_agent agent);


        bool UpdateCallMoblie(int topAgentId, int agentId, string mobile, int isGrabOrder);
        bool UpdateIsRequoteByAgentId(int agentId, int isRequote);
        /// <summary>
        /// 顶级是否存在手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        bool GetAgentByPhoneTopAgent(string mobile, int parentAgent);
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
        /// 获得代理人分组
        /// </summary>
        /// <returns></returns>
        IEnumerable<AgentGroupVM> GetAgentGroups();
        /// <summary>
        /// 初始化特定的代理人组入redis
        /// </summary>
        /// <param name="currentAgentId"></param>

        /// <returns></returns>
        AgentGroupVM GetSpecifiedAgentGroupToRedis(int currentAgentId);
        List<int> GetSonListByDb(int currentAgent, bool hasSelf = true);
        List<int> GetAllSonListByDb(int currentAgent, bool hasSelf = true);
        /// <summary>
        /// 获取删除的代理人信息
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        List<int> GetDelSonListByDb(int currentAgent);
        /// <summary>
        /// 注册顶级代理人时判断手机号是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        bool IsExistMobileForTopAgent(string mobile, int topAgentId);

        /// <summary>
        /// 是否存在指定角色的代理人
        /// </summary>
        /// <param name="managerRoleId"></param>
        /// <returns></returns>
        bool IsExistManagerRoleId(int managerRoleId);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="oldRoleId"></param>
        /// <param name="newRoleId"></param>
        /// <returns></returns>
        bool UpdateRoleId(int oldRoleId, int newRoleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<AgentIdAndAgentName> GetByTopAgentId(int topAgentId);
        List<AgentIdAndAgentName> GetByTopAgentId2(int topAgentId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentAgentId"></param>
        /// <returns></returns>
        List<AgentIdAndAgentName> GetByParentAgentId(int parentAgentId);
        List<AgentIdAndAgentName> GetByParentAgentId2(int parentAgentId);
        /// <summary>
        /// 除了当前代理人以外其他用户使用这个账号的数量 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        int SameAccountCount(int agentId, string account);

        /// <summary>
        /// 查询没有助理角色的顶级代理人 zky 2017-01-01 /crm
        /// </summary>
        /// <returns></returns>
        IList<bx_agent> GetNoHelperTopAgent();

        string GetUkeyName(int ukeyId);
        /// <summary>
        /// 获取source
        /// </summary>
        /// <param name="source">source值</param>
        /// <param name="sourceType"> 0:传递新source  1：传递老source</param>
        /// <returns></returns>
        bx_companyrelation GetSource(long source, int sourceType);

        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds">批量更新的代理id</param>
        /// <param name="messagePayType">短信扣费方式</param>
        /// <param name="usedStatus">启用状态</param>
        /// <param name="isShowRate">是否展示费率</param>
        /// <param name="isSubmit">是否可核保</param>
        /// <returns></returns>
        bool AgentBatchAudit(List<int> agentIds, int messagePayType, int usedStatus, int isShowRate, int isSubmit, int zhenBangType);

        /// <summary>
        /// 根据代理人Id获取代理人账号 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        List<string> GetAgentAccountByAgentId(List<int> agentIds);

        /// <summary>
        /// 根据条件查询  zky 2017-09-21 /crm
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IList<bx_agent> GetList(Expression<Func<bx_agent, bool>> where);
        /// <summary>
        /// 获取全部代理人编号
        /// </summary>
        /// <returns></returns>
        List<int> GetAllAgentIds();

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
        IList<OrgListDto> GetOrgList(string orgName, int authenState, int groupId, bool needPage, int orgId, int pageIndex, int pageCount, out int total);

        IList<OrgListDto> AgentNameIdList(int groupId);

        /// <summary>
        /// 判断代理人是否在顶级下 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="shareCode"></param>
        /// <returns></returns>
        bool IsTopAgentSonShareCode(int agentId, int shareCode);

        /// <summary>
        /// 判断邀请码是否是三级代理的 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="shareCode"></param>
        /// <returns></returns>
        bool IsThreeShareCode(int shareCode);

        /// <summary>
        /// 判断代理人是否是三级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsThreeAgent(int agentId);

        /// <summary>
        /// 判断代理人是否是二级并且没有三级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsTwoAgentAndNoSon(int agentId);

        /// <summary>
        /// 获取业务员列表 zky 2017-12-8 /crm(如果查询条件修改请修改下面 GetAgetListCount 条件)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Mobile"></param>
        /// <param name="StateDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="IsUsed"></param>
        /// <param name="ParentAgentName"></param>
        /// <param name="ParentAgentId"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="AgentId"></param>
        /// <param name="AuthenState"></param>
        /// <param name="QueryZBType">需要查询的 账号类型</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="OnlySite"></param>
        /// <param name="AgentZBType">代理人 振邦账号的类型</param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        IList<CustomerModel> GetAgentList(string Name, string Mobile, string StateDateTime, string EndDateTime, int IsUsed, string ParentAgentName, string ParentAgentId, int TopAgentId, int AgentId, int AuthenState, int QueryZBType, int OnlySite, int AgentZBType, int PageIndex, int PageSize, int testState, out int RecordCount);


        /// <summary>
        /// 获取集团下所有机构
        /// </summary>
        /// <returns></returns>
        List<bx_agent> GetAgentIdAndNameByGroupId(string groupIds);

        /// <summary>
        /// 根据顶级代理人获取下级代理人数量
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        int GetChildAgentCountByTopAgentId(int topAgentId);

        /// <summary>
        /// 查询业务员总数量接口 zky 2017-12-26 /crm(查询条件和上面获取业务员列表 GetAgentList 一致)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Mobile"></param>
        /// <param name="StateDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="IsUsed"></param>
        /// <param name="ParentAgentName"></param>
        /// <param name="ParentAgentId"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="AgentId"></param>
        /// <param name="AuthenState"></param>
        /// <param name="QueryZBType">需要查询的 账号类型</param>
        /// <param name="OnlySite"></param>
        /// <param name="AgentZBType">代理人 振邦账号的类型</param>
        /// <returns></returns>

        int GetAgetListCount(string Name, string Mobile, string StateDateTime, string EndDateTime, int IsUsed, string ParentAgentName, string ParentAgentId, int TopAgentId, int AgentId, int AuthenState, int QueryZBType, int OnlySite, int AgentZBType);

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
        /// 根据代理人id获取代理人配置信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bx_agent_setting GetAgentSettingModelByAgentId(int agentId);


        /// <summary>
        /// 添加代理人配置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AddAgentSetting(bx_agent_setting entity);
        tx_agent GetTxAgent(int agentId);


        /// <summary>
        /// 获取父级下第一个子级的代理ID
        /// </summary>
        /// <param name="parentAgentId"></param>
        /// <returns></returns>
        List<int> GetByParentId(int parentAgentId);

        /// <summary>
        /// 更新代理人账号的启用状态
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        bool UpdateAgentIsUsed(string agentIds, int isUsed);

        /// <summary>
        /// 验证该顶级Id是否在该集团下
        /// </summary>
        /// <param name="TopAgentId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        bool AgentInGroup(int TopAgentId, int GroupId);



        int VerificationThirdAccount(int agentId);

        List<ChannelModel> GetListChannel(long ChildAgent);
        int UpdateChannelIsUesd(AgentUKeyRequest request);

        string GetAgentWhere(int RoleType, int ChildAgent, int Agent);
        string GetConditionSql(Tuple<List<int>, string> tuple);
        /// <summary>
        /// 根据代理人获得二级、三级代理人，同一分叉下面的
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<string> GetSonLevelAgent(int agentId);

        List<MobileDefeatAnalyticsVM> GetDefeatAnalytics4Mobile(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName, out int totalCount);

        List<DefeatAnalysis> GetReasonAnalytics4Mobile(int agentId, DateTime startTime, DateTime endTime, string categoryName, int isViewAllData);

        /// <summary>
        /// 获得下级代理人Id
        /// 获得上级代理人Id
        /// </summary>
        /// <param name="currentAgent">当前代理人</param>
        /// <param name="hasSelf">是否包含自己</param>
        /// <returns></returns>
        List<int> GetJuniorAgentIdList(int currentAgent, int flag, bool hasSelf = true);
        List<int> GetOtherAgentList(int AgentId, int TopAgentId);
        bx_agent VerifySecond(int agentId, int agentLevel);
        bx_agent VerifyLevel(int agentId);
        /// <summary>
        /// 根据顶级获取所有管理员Id
        /// </summary>
        /// <param name="topagentid"></param>
        /// <returns></returns>
        List<int> GetManagerId(int topagentid);
        bool IsUsedByAgentId(string agentIds);
        List<string> GetUsedDisableByAgentId(string agentIds);
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="childAgent"></param>
        /// <param name="isUsed"></param>
        void PushSignal(int childAgent, int isUsed);

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
        IList<bx_agent> GetList(List<long> ids);

        List<int> GetChildAgentIdByTopAgentIds(List<int> topAgentIds);
    }
}

