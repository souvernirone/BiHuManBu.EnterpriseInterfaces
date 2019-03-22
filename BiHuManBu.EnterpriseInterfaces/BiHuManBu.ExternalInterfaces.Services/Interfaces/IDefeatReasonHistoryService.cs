using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IDefeatReasonHistoryService
    {
        /// <summary>
        /// 获取战败历史数据
        /// </summary>
        /// <param name="seachDefeatReasonHistoryCondition">过滤条件</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        List<DefeatReasonDataViewModel> GetDefeatReasonHistory(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, out int totalCount);
        /// <summary>
        /// 添加数据到战败历史
        /// </summary>
        /// <param name="singleDefeatReasonDataViewModel">需要添加的数据模型</param>
        /// <returns></returns>

        bool AddToDefeatReasonHistory(SingleDefeatReasonDataViewModel singleDefeatReasonDataViewModel);

        /// <summary>
        /// 删除战败历史
        /// </summary>
        /// <param name="buId">bx_userinfo.Id</param>
        void DeleteDefeatReasonHistory(long buId);

        MobileStatisticsBaseVM<DefeatReasonMobileDetails> GetDefeatAnalyticsDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName);

        int GetDefeatReasonHistoryCount(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition);
        BaseViewModel UpdateDefeatHistoryOldList();
    }
}
