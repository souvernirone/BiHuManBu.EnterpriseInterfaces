using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Linq.Expressions;
using System;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Models
{
    /// <summary>
    ///  
    /// </summary>
    public interface IUserInfoRepository
    {
        /// <summary>
        /// app端用查询userinfo，由于无openid限制，故3个条件不用openid查
        /// 顶级代理下的车辆有可能重复，以最新的一条记录来处理，即使查看老数据，也展示最新的一条数据
        /// </summary>
        /// <param name="licenseno"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        bx_userinfo FindByAgentLicense(string licenseno, string agent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        List<bx_userinfo> FindList(string agent);

        /// <summary>
        /// 获取客户列表，没有返回总数
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<GetCustomerViewModel> FindCustomerList(SearchCustomerListDto search);
        /// <summary>
        /// 根据buid查询单条记录
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        GetCustomerViewModel FindCustomerModel(int buid);
        /// <summary>
        /// 关联了bx_consumer_review表的列表查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<GetCustomerViewModel> FindCustomerListJoinConsumerReview(SearchCustomerListDto search);

        /// <summary>
        /// 获取总条数，包含了DistributedCount
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        DistributedCountViewModel FindCustomerCountContainDistributedCount(SearchCustomerListDto search);

        /// <summary>
        /// 通用查询客户数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<int> FindCustomerCountAsync(SearchCustomerListDto search);

        /// <summary>
        /// 通用查询客户数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        int FindCustomerCount(SearchCustomerListDto search);

        /// <summary>
        /// 查询客户列表所有数据，应用于导出列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<GetCustomerViewModel> FindCustomerListForExport(SearchCustomerListDto search);

        /// <summary>
        /// 摄像头进店列表导出
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<GetCustomerViewModel> FindCustomerListForCameraExport(SearchCustomerListDto search);

        /// <summary>
        ///获取摄像头进店列表导出总条数
        /// </summary>
        /// <returns></returns>
        DistributedCountViewModel GetExportCount(SearchCustomerListDto search);

        /// <summary>
        /// 获取新进店车辆数量
        /// </summary>
        /// <param name="updateDateStart"></param>
        /// <param name="updateDateEnd"></param>
        /// <param name="agentId"></param>
        /// <param name="renewalType"></param>
        /// <param name="listSonAgent"></param>
        /// <returns></returns>
        List<long> FindBuidLoop(string updateDateStart, string updateDateEnd, int agentId, int renewalType, List<string> listSonAgent);

        /// <summary>
        /// 只查询符合条件的buid
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<long> FindCustomerBuid(SearchCustomerListDto search);

        List<long> FindCustomerBuid2(SearchCustomerListDto search, int DistributeAgentIds, int AverageCount, int OrderBy);
        List<UserInfoIdAgentModel> FindCustomerBuidAndAgent(SearchCustomerListDto search);

        /// <summary>
        /// 获取部分userinfo的数据
        /// </summary>
        /// <param name="listBuid"></param>
        /// <param name="orderBy">排序和客户列表排序一样</param>
        /// <param name="takeCount">取多少条数据</param>
        /// <returns></returns>
        List<DistributeUserinfoDto> FindCustomerBuidOrderBy(List<long> listBuid, int orderBy, int takeCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lecenseNo"></param>
        /// <returns></returns>
        List<bx_userinfo> FindAgentListByLicenseNo(string lecenseNo);
        /// <summary>
        /// 根据id或者bx_userinfo
        /// </summary>
        /// <returns></returns>
        bx_userinfo GetUserInfo(long id);

        /// <summary>
        /// 是否是新车
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        int Selquotereqcarinfo(long buId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="reqRenewalType"></param>
        /// <param name="distributed"></param>
        /// <param name="exitUserinfo"></param>
        /// <param name="isDistributedUserInfo"></param>
        /// <returns></returns>
        int UpdateUserRenewalTypeAndDistributed(int buId, int reqRenewalType, int distributed, bool exitUserinfo = false, bool isDistributedUserInfo = false);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<bx_userinfo> UdapteDateupdate();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="licenseno"></param>
        /// <returns></returns>
        List<bx_car_renewal> GetBxCarRenewals(string licenseno);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bxUserinfoRenewalIndex"></param>
        /// <param name="buid"></param>
        void UpdatebxUserinfoRenewalIndex(bx_userinfo_renewal_index bxUserinfoRenewalIndex, long buid);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        bx_userinfo_renewal_index GetBxUserinfoRenewalIndex(long buid);

        /// <summary>
        /// 
        /// </summary>
        void UpdateCarRenewalIndex();

        /// <summary>
        /// 根据agent获取客户数据的buid集合
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        List<long> GetListBuId(string agent);
        /// <summary>
        /// 根据agentid和buids修改istest
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="agentId"></param>
        /// <param name="isTest"></param>
        /// /// <param name="strAgents"></param>
        /// <returns></returns>
        bool UpdateIsTest(string buids, int agentId, int isTest, string strAgents);
        /// <summary>
        /// 删除数据后更新统计表
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        bool UpdateReviewdetailRecord(string buids);
        /// <summary>
        /// 修改buid对应的agentid
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="newAgentId">新的agentid</param>
        /// <returns></returns>
        bool UpdateAgent(string buids, int newAgentId);
        bool UpdateAgent( int newAgentId, string strAgents);
        /// <summary>
        /// 根据buid更新用户数据
        /// </summary>
        /// <param name="newAgentId"></param>
        /// <param name="strBuids"></param>
        /// <returns></returns>
        bool UpdateAgentByBuid(int newAgentId, string strBuids,int IsTest);
        List<bx_userinfo> ReportForReInfoList(List<string> sonself, string strDate, string licenseNo, out int totalCount);

        /// <summary>
        /// 判断是否有重复数据，取新一条
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="licenseNo"></param>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        bx_userinfo FindAgentListByLicenseNo(long buid, string licenseNo, List<string> agentIds);
        /// <summary>
        /// 根据车架号发动机号获取数据
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="carVin"></param>
        /// <param name="engineNo"></param>
        /// <param name="agent"></param>
        /// <param name="citycode"></param>
        /// <param name="renewalCarType"></param>
        /// <returns></returns>
        bx_userinfo FindByCarVin(string openid, string carVin, string engineNo, string agent, string citycode, int renewalCarType);

        int GetQuoteLimitCount(string openid, string agent,  int QuoteLimit);
        /// <summary>
        /// 删除某个buid
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="deleteAgentId"></param>
        /// <returns></returns>
        bool DeleteUserinfo(long buid, string deleteAgentId);

        int Update(bx_userinfo model);

        bx_forbid_mobile GetForbidMobile(string licenseno, int cityid, int source);

        bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno, string agent, string citycode, int renewalCarType);

        List<AgentNameViewModel> IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type);

        void UpdateBxUserinfoAgent(string agentMd5, long buid, int agentid);

        bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno);

        /// <summary>
        /// 查询剩余条数
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        int FindCustomerCountNew(SearchCustomerListDto search);

        /// <summary>
        /// 根据agentIds和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        List<long> FindByAgentsAndIsTest2(string agentIds, int isTest);
        /// <summary>
        /// 根据agentIds和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        List<UserInfoIdAgentModel> FindByAgentsAndIsTest(string agentIds, int isTest);
        /// <summary>
        /// 根据代理人和车牌号获取数据   陈亮  2017-08-03
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="licenseNo"></param>
        /// <returns></returns>
        List<RevokeUserInfoDto> FindByAgentsAndLicenseNo(string agentIds, string licenseNo);

        bx_userinfo FindByBuid(long buid);
        
        /// <summary>
        /// 分配回收时获取要回收的数据的信息
        /// </summary>
        /// <param name="listBuid"></param>
        /// <returns></returns>
        Task<List<DistributedRecycleDto>> GetDistributedRecycleAsync(List<long> listBuid);

        IList<bx_userinfo> GetList(Expression<Func<bx_userinfo, bool>> where);

        DataSet GetMyListDateInformation(string sql);

        /// <summary>
        /// 根据buid获取IsDistributed
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<UserInfoIdDistribute> GetIsDistributed(SearchCustomerListDto search);

        List<long> GetListUseridByAgentIds(string ids, string licenseno, string vinNo, int type);





        List<GetCustomerViewModel> FillCustomerInformation(List<long> listBuid);


         List<UpdateCustomerStatusAndCategoriesJson> GetCustomerStatusAndCategories(List<long> userIds);


         List<AgentNameViewModel> SecondRepeat(List<string> agentIds, string licenseno, string vinNo, int TypeId);


         AgentNameViewModel GetTopData( int topagentId);


         bool DistributionData(int topAgentId, int agentId);

         List<bx_userinfo> GetUserInfoByLicenseNo(string LicenseNo, int AgentId);

        List<bx_userinfo> GetUserinfoByLicenseAndAgent(long buid, string licenseNo, List<string> agentIds);
        List<bx_userinfo> GetUserinfoByCarVinAndAgent(string carvin, string engino, List<string> agentIds);

        /// <summary>
        /// 获取客户列表重复数据
        /// </summary>
        /// <param name="childAgent"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="groupByType">1.车牌号，2.车架号，3.客户电话和客户名称</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentContidion"></param>
        /// <param name="joinCondition"></param>
        /// <param name="agentWhere"></param>
        /// <returns></returns>
        List<UserInfoRepeatModel> GetCustomerRepeatList(int childAgent, int TopAgentId, int groupByType, int pageIndex, int pageSize, string agentContidion, string joinCondition, string agentWhere);
        /// <summary>
        /// 获取重复数据总数量
        /// </summary>
        /// <param name="groupByType">分组类型：1.车牌号，2.车架号，3.客户电话和客户名称</param>
        /// <param name="agentContidion">agent查询条件</param>
        /// <param name="joinCondition">连表查询条件</param>
        /// <returns></returns>
        int GetCustomerRepeatListCount(int pageIndex,int pageSize,int groupByType, string agentContidion, string joinCondition);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddRangeUserInfoExpand(List<bx_userinfo_expand> list);
        /// <summary>
        /// 根据buids获取对应的数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<UserInfoPartModel> GetUserInfoList(List<long> list);
        /// <summary>
        /// 根据代理人和车牌号
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="licenseNos"></param>
        /// <param name="agentList"></param>
        /// <param name="licenseNoList"></param>
        /// <returns></returns>
        List<UserInfoPartModel> FindByAgentsAndLicenseNos(string agentIds, string licenseNos);
        /// <summary>
        /// 回收站批量撤销
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        bool BatchRevokeFiles(string buids);        

        /// <summary>
        /// 顶级代理人下边对licenseno报价过得数据，是否有重复
        /// </summary>
        /// <param name="topAgentId">顶级代理人</param>
        /// <param name="agentId">当前代理人</param>
        /// <param name="licenseno">车牌号</param>
        /// <param name="vinNo">车架号</param>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        List<AgentNameViewModel> GetJuniorRepeat(int topAgentId, int agentId, string licenseno, string vinNo, int TypeId);

        /// <summary>
        /// 根据buid获取数据
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<bx_userinfo> GetUserListByBuid(List<long> buids);
        List<int> GetUserByAgentId(List<int> agentIds);
        long GetMaxBuid();
        List<UserInfoModel2> GetUserListByQuotationReceipt(long startBuid, long endBuid);
        List<UserInfoModel2> GetDefeatHistoryUserList(long startBuid, long endBuid);

        /// <summary>
        /// 添加bx_userinfo
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        long Add(bx_userinfo userinfo);
        /// <summary>
        /// 获取bx_userinfo
        /// </summary>
        /// <param name="licenseno"></param>
        /// <param name="custkey"></param>
        /// <param name="agent"></param>
        /// <param name="renewalcartype"></param>
        /// <returns></returns>
        bx_userinfo Find(string licenseno, string custkey, string agent, int renewalcartype);
       
        /// <summary>
        /// 根据主键获取baodanxinxi数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        dz_baodanxinxi BaoDanXinXiModel(int id);
        dz_baodanxinxi BaoDanXinXiModelByRecDuid(string recGuid);
        dz_reconciliation ReconciliationModel(string recGuid);
        /// <summary>
        /// 将已批改车牌添加到bx_userinfo
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool AddUserInfo(bx_userinfo model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clpId">dz_correct_license_plate.id</param>
        /// <returns></returns>
        bool AddUserInfo(bx_userinfo model,int clpId, dz_baodanxinxi baodanModel);
        /// <summary>
        /// 更新表中存在的数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateUserInfo(bx_userinfo model);
        /// <summary>
        /// 根据条件判断是否存在数据
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <param name="openId"></param>
        /// <param name="agentId"></param>
        /// <param name="renewalCarType"></param>
        /// <returns></returns>
        bx_userinfo GetUserInfoByCondition(string licenseNo, string openId, string agentId, int renewalCarType);
        //判断是否有车牌
        List<GetCustomerViewModel> GetUserByVehLicense(string agentWhere, string carVin, string engineNo);
        List<GetCustomerViewModel> GetUserByVehLicense(int buid);
        dz_correct_license_plate GetCorrectLicensePlate(string engineNo, string carVIN);
        dz_correct_license_plate GetCorrectLicensePlate(int id);
        bx_userinfo GetCustomerById(long buid);
    }
}
