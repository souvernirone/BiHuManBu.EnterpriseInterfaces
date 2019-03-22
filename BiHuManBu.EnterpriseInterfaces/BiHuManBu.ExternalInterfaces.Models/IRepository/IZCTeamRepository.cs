using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IZCTeamRepository
    {
        /// <summary>
        /// 获取分享用户数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetChildAgent(int agentId);
        /// <summary>
        /// 出单数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetAgentOrder(int agentId);
        /// <summary>
        /// 获取下级代理人单人净保费列表
        /// </summary>
        /// <param name="agentList"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<AgentSonPremium> GetAgentSonPremium(List<int> agentList, string startDate, string endDate);
        /// <summary>
        /// 完成团队任务创建团队
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int UpdateCompleteTask(bx_group_authen model);
        /// <summary>
        /// 获取团队列表数据
        /// </summary>
        /// <param name="AgentName">代理人姓名</param>
        /// <param name="Mobile">手机号</param>
        /// <param name="CommissionTimeStart"></param>
        /// <param name="CommissionTimeEnd"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        List<TeamModel> GetTeamList(int TopAgentId, string AgentName, string Mobile, string CommissionTimeStart, string CommissionTimeEnd, int PageIndex, int PageSize, out int RecordCount);

        /// <summary>
        /// 二级团员保费明细
        /// </summary>
        /// <param name="ChildAgent"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="SecCode"></param>
        /// <param name="CommissionTimeStart"></param>
        /// <param name="CommissionTimeEnd"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        List<TeamChildLevelModel> GetTeamChildLevelList(int ChildAgent,string AgentName,string Mobile, int TopAgentId, string SecCode,
                string CommissionTimeStart, string CommissionTimeEnd, int PageIndex, int PageSize, out int RecordCount);

       List<NextLevelAgentModel> GetNextLevelAgentList(int ChildAgent, int TopAgentId,
                 int PageIndex, int PageSize , bool IsAll, out int RecordCount); 
    }
}
