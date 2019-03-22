using System.Collections.Generic;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomerTopLevelService
    {

        Task<CustomerListViewModel> GetCustomerListAsync(GetCustomerListRequest request);

        Task<DistributedCountViewModel> GetCustomerCountAsync(GetCustomerCountRequest request);

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int GetCustomerCount(BaseCustomerSearchRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CustomerListExportViewModel ExportCustomerList(GetCustomerListRequest request);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<long> GetBuidsList(ExcuteDistributeRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<int> GetListAgent(int agentId, int topAgentId);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //int GetExpiredCustomerCount(GetCustomerListRequest request);

        /// <summary>
        /// 获取摄像头进店提醒
        /// 陈亮  2017-9-2  改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetCountLoopViewModel GetCountLoop(GetCustomerListRequest request);

        //int GetCountDistributed(GetCustomerListRequest request, int type);

        ///// <summary>
        ///// 拼接where语句
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="joinWhere">需要关联表的SQL的where部分</param>
        ///// <returns>拼接的不需要关联的SQL的where部分</returns>
        //StringBuilder GetWhereByRequest(BaseCustomerSearchRequest request, out string joinWhere);

        /// <summary>
        /// 拼接where语句第二个版本：将部分搜索条件存起来，在repository中生成SQL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchCustomerListDto GetWhereByRequest2(BaseCustomerSearchRequest request);

        /// <summary>
        /// 这里调用了GetWhereByRequest2和GetJoinType
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        SearchCustomerListDto GetWhereAndJoinType(BaseCustomerSearchRequest request, int orderBy = -1);

        /// <summary>
        /// 判断关联那个表
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <param name="orderBy">排序规则，因为排序也需要关联表</param>
        /// <returns>0:不关联表  1:关联bx_consumer_review</returns>
        int GetJoinType(BaseCustomerSearchRequest request, int orderBy = -1);

        // string AgentSonsJoinToCount(GetCustomerListRequest request, bool hasSelf = true);

        ///// <summary>
        ///// 根据lable生成where 的sql语句
        ///// 下级代理人的标签
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //SearchCustomerListDto GetWhereSqlByLableForChildAgent(BaseCustomerSearchRequest request);

        ///// <summary>
        ///// 根据lable生成where 的sql语句
        ///// 顶级代理人的标签
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //SearchCustomerListDto GetWhereSqlByLableForTopAgent(BaseCustomerSearchRequest request);

        int GetCustomerCountNew(GetCustomerListRequest request);

        //int GetCustomerAllCount(GetCustomerListRequest request);

        /// <summary>
        /// crm用户批量删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel MultipleDelete(MultipleDeleteRequest request);
        /// <summary>
        /// crm用户批量删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel MultipleDeleteUserInfo(MultipleDeleteRequest request);
        /// <summary>
        /// 将listbuid的数据放在我们的代理人下面，并且插入步骤表
        /// </summary>
        /// <param name="listBuid"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool MoveToOurAgentAndInsertIntoSteps(List<long> listBuid, int agentId);

        /// <summary>
        /// 数据转移接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> TransferDataAsync(ExcuteDistributeRequest request);

        /// <summary>
        /// 分配前的校验
        /// 陈亮 2107-10-11 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CheckDto<List<long>> DistributeCheck(ExcuteDistributeRequest request);

        /// <summary>
        /// 分配的核心逻辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CheckDto<List<UpdateUserInfoModel>>> DistributeKernelAsync(DistributeDto model);

        /// <summary>
        /// 为业务员平均分配车辆信息方法
        /// </summary>
        /// <param name="agentIds">分配的代理人id</param>
        /// <param name="stepType"></param>
        /// <param name="listBuid">需要分配的数据</param>
        /// <param name="averageCount">平均分派数</param>
        /// <param name="topAgent">顶级代理人</param>
        /// <param name="fountain">平台编号 1crm2微信4app</param>
        /// <param name="operateAgentId">执行分配操作的代理人id</param>
        /// <returns></returns>
        Task<BaseViewModel> UpdateGroupDistributeAsync(List<int> agentIds, int stepType, List<long> listBuid, int averageCount, int topAgent, int fountain, int operateAgentId);

        /// <summary>
        /// 组装分页的参数
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <param name="showPageNum"></param>
        void SetPage(ref SearchCustomerListDto search, int pageSize, int curPage, int showPageNum = -1);





         bool BatchUpdateCustomerStatusAndCategories(BatchUpdateCustomerStatusAndCategoriesModel model);

        /// <summary>
        /// 获取客户列表重复数据
        /// </summary>
         /// <param name="request"></param>
        /// <returns></returns>
         UserInfoRepeatViewModel GetCustomerRepeatList(GetCustomerRepeatListRequest request);

        /// <summary>
        /// 删除客户列表重复数据
        /// </summary>
        /// <param name="Buids">要删除的buid</param>
        /// <param name="TopAgentId">顶级代理人</param>
        /// <param name="ChildAgent">当前代理人</param>
        /// <returns></returns>
         BaseViewModel MultipleDeleteRepeat(List<long> Buids, int TopAgentId, int ChildAgent);
         /// <summary>
         /// 回收站批量撤销
         /// </summary>
         /// <param name="request"></param>
         /// <returns></returns>
         BatchBackViewModel BatchBackout(DeleteRepeatCustomerRequest request);
        /// <summary>
        /// 回收站批量删除（多选删除）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
         BaseViewModel BatchDeleteRecycle(DeleteRepeatCustomerRequest request);
        

        /// <summary>
        /// 将已批改车牌添加到bx_userinfo用户表
        /// </summary>
        /// <param name="id">保单信息ID</param>
        /// <param name="agentId">车牌号属于的代理人</param>
        /// <returns></returns>
         CorrectCarViewModel AddUserInfo(string recGuid, int agentId);

        
    }
}
