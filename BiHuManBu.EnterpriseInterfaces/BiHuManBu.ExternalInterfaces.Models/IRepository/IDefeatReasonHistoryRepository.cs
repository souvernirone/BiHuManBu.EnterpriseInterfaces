using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IDefeatReasonHistoryRepository
    {
        /// <summary>
        /// 获取战败历史数据
        /// </summary>
        /// <param name="seachDefeatReasonHistoryCondition">过滤条件</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="agentIds">当前代理人所有下级（包括自己）</param>
        /// <returns></returns>
        List<DefeatReasonDataViewModel> GetDefeatReasonHistory(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, List<int> agentIds, out int totalCount);
        /// <summary>
        /// 添加数据到战败历史
        /// </summary>
        /// <param name="defeatReasonHistory">需要添加的数据模型</param>
        /// <returns></returns>

        bool AddToDefeatReasonHistory(bx_defeatreasonhistory defeatReasonHistory);
        /// <summary>
        /// 删除战败历史
        /// </summary>
        /// <param name="buId">bx_userinfo.Id</param>
        void DeleteDefeatReasonHistory(long buId);

        List<DefeatReasonMobileDetails> GetDefeatAnalyticsDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName, out int totalCount);

        int GetDefeatReasonHistoryCount(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, List<int> agentIds);
    }

}
