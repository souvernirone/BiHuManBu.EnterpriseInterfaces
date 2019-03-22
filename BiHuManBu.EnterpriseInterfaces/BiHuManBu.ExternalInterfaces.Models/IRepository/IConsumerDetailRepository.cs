using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models
{        
    public interface  IConsumerDetailRepository
    {
        bx_consumer_review Find(int id);
        int AddDetail(bx_consumer_review bxWorkOrderDetail);
        int UpdateDetail(bx_consumer_review bxWorkOrderDetail);
        List<bx_consumer_review> FindDetails(long buid);
        bx_consumer_review FindNewClosedOrder(long buid, int status = 1);
        List<bx_consumer_review> FindNoReadList(int agentId, out int total);

        int AddCrmSteps(bx_crm_steps bxCrmSteps);

        int UpdateCrmSteps(bx_crm_steps bxCrmSteps);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> AddCrmStepsAsync(bx_crm_steps model);

        Task<bool> InsertBySqlAsync(List<bx_crm_steps> list);

        /// <summary>
        /// 添加步骤列表
        /// </summary>
        /// <param name="listStep"></param>
        /// <returns></returns>
        bool AddCrmSteps(List<bx_crm_steps> listStep);
        /// <summary>
        /// 将回收站要回收的数据记录到记录表
        /// </summary>
        /// <param name="isTest">3</param>
        /// <param name="agentId">当前代理人ID</param>
        /// <returns></returns>
        bool ClearRecycleBinAddSteps(int isTest, string strAgents);
        bool BatchAddCrmStepsByBuid(string strBuids, int IsTest);
        bool BatchAddCrmStepsByBuid(string strBuids, long agentId);
         List<bx_crm_steps> GetCrmStepsList(long buid);
        string GetTopAgent(int agentId);
        bx_sms_account GetBxSmsAccount(int agentid);
        void InsetBxSmsAccount(bx_sms_account bxSmsAccount);
        bx_quotereq_carinfo GetBxQuotereqCarinfo(long buid);
         bx_userinfo GetBxUserinfo(long buid);
        bx_lastinfo GetLastinfo(long buid);
        bx_quoteresult GetQuoteresult(long buid);
        void SaveNewQuoteInfo(RequestNewQuoteInfoViewModel requestNew);
        IEnumerable<bx_agent> OldNoManagerRoleBxAgents();
        IEnumerable<manager_role_db> ManagerRoleDbs();

        IEnumerable<manageruser> GetManagerusers(List<string> agentaccountList);
        manager_role_db GetManagerRoleDb(int topAgentId);
        int AddRole(manager_role_db managerRole, List<manager_role_module_relation> managerRoleModuleRelation);
        int AddConsumerRole(List<manager_role_module_relation> managerRoleModuleRelations, int roleid);
       int InsertManagerRoleModuleRelation(manager_role_db managerRole);

        int GetAppoinmentInfoNum(string agentid);

        List<manager_module_db> GetBaseModuleDbs();
        /// <summary>
        /// 根据给定的buid查找是否是新车
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<IsNewCarViewModel> GetIsNewCar(List<long> buid);

        /// <summary>
        /// 根据buid获取保额信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        bx_savequote GetSaveQuoteByBuid(long buid);
        /// <summary>
        /// 根据buid获取保费信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        Task<List<bx_quoteresult>> GetQuoteResultListByBuid(long buid);
        bool AddCrmStepsOfCamera(string sql);
    }
}
