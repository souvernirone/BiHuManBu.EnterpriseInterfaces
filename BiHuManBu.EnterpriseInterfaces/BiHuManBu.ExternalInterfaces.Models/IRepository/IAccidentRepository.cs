using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAccidentRepository
    {
        List<AccidentClueModel> GetClueList(int pageIndex, int pageSize, out int totalCount, int followState, int timeType, string carInfo, int agentId, int topAgentId, int roleType);

        List<AccidentClueModel> GetClueList(AccidentListRequest accidentListRequest,out int totalCount);
        Dictionary<int, int> GetCluesCountWithState(AccidentListRequest accidentListRequest);
        int SaveSMS(tx_sms model);


     bool   SMSIsExist(string mobile,string smsContent);

        AccidentClueTotalModel GetTotalCount(int timeType, int agentId, int topAgentId, int roleType);

        ClueStatisticalViewModel GetClueStatisticalViewModel(int agentId, DateTime startTime, DateTime endTime, int roleType);

        List<ClueStatisticalWithCompany> GetClueStatisticalWithCompany(int agentId, DateTime startTime, DateTime endTime);

        List<ClueResponsivity> GetClueResponsivity(int agentId, DateTime startTime, DateTime endTime);

        List<LossStatistical> GetLossStatistical(int agentId, DateTime startTime, DateTime endTime);

        AccidentClueModel GetClusDetails(int clueId);


        AccidentClueModel GetClusDetail(int clueId);

        List<string> GetClusImage(int clueId);

        ClueOrderStatistical ClueOrderStatistical(int topAgentId, DateTime startTime, DateTime endTime);

        List<AccidentFollowRecord> GetFollowUpRecords(int clueId);

        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<AccidentSMSTempModel> GetTempList(int topAgentId);
        /// <summary>
        /// 获取当前线索提醒时间内是否具有下次跟进状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        int HasNextState(int id);

        int CluesState(int cluesId);


        List<ClueState> GetFollowUpStates(int agentId);

        List<ClueLossReason> GetLossReasons(int agentId);

        List<RecivesCarPeople> GetRecivesCarPeoples(int topAgentId);

        int UpdateClue(int state, int clueId, int followId);

        string GetReInfo(int clueId);

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        manager_role_db GetRoleModelById(int roleId);

        /// <summary>
        /// 获取顶级代理人下的所有理赔主管ID
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        List<int> GetManagerIdByTopAgentId(int topAgentId);

        /// <summary>
        /// 添加或修改推修手机运行状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int MobileServiceAddOrUpdate(tx_mobileservicestatus model);

        /// <summary>
        /// 根据顶级id更新门店地址信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        int InsertAddressModel(int topAgentId, string address);

        tx_storeaddress GetAddressModel(int topAgentId);

        List<FunctionCodeModel> GetFunctionCodeByAgentId(int agentId);
    }
}
