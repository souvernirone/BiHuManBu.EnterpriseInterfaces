using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ISettlementService
    {
        /// <summary>
        /// 批量增加待结算单
        /// </summary>
        /// <param name="unSettlementListVM">添加数据集合</param>
        /// <returns></returns>
        bool AddUnSettlementRange(List<UnSettlementRequestVM> unSettlementListVM);
        /// <summary>
        /// 获取待结算单列表
        /// </summary>
        /// <param name="unSettlementListSearchVM">待结算列表获取条件</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="reconciliationCount">保单数</param>
        /// <returns></returns>
        List<UnSettlementResponseVM> GetUnSettlementList(UnSettlementListSearchRequestVM unSettlementListSearchVM, out int totalCount, out double totalPrice, out int reconciliationCount);

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="ids">待结算编号集合</param>
        /// <param name="settleType">结算类型：1->代理人佣金结算、2->网点佣金结算、3->机构佣金结算、4->机构毛利结算、5->保司手续费结算</param>
        /// <returns></returns>
        Tuple<bool, string> CreateSettlement(List<int> ids, int settleType);

        /// <summary>
        /// 获取结算列表
        /// </summary>
        /// <param name="settlementListRequestVM">结算单搜索条件模型</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="settleCount">结算数量</param>
        /// <returns></returns>
        List<SettlementResponseVM> GetSettlementList(SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice,out int settleCount);

        /// <summary>
        /// 标记状态操作
        /// </summary>
        /// <param name="settlementUpdateStatusVM">标记状态模型</param>
        /// <param name="guids">操作的保单标识集合</param>

        /// <returns></returns>
        bool UpdateStaus(SettlementUpdateStatusVM settlementUpdateStatusVM, out List<string> guids);
        /// <summary>
        /// 退回结算单
        /// </summary>
        /// <param name="batchId">结算单编号</param>
        /// <param name="settleIds">结算单明细编号</param>
        /// <param name="rollbackCount">回退数量</param>
        /// <returns></returns>
        bool RollBackSettlementList(int batchId, List<int> settleIds,out int rollbackCount);
        /// <summary>
        /// 添加到结算单
        /// </summary>
        /// <param name="batchId">结算批次号</param>
        /// <param name="unSettleIds">待结算号集合</param>
        /// <returns></returns>
        Tuple<bool, string> AddToSettlementList(int batchId, List<int> unSettleIds);
        /// <summary>
        /// 检查数据是否为同一类型
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <param name="settleType">结算类型</param>
        /// <param name="batchId">结算批次号</param>
        /// <returns></returns>

        bool CheckSameData(List<int> ids, int settleType, int batchId);
        /// <summary>
        /// 获取结算单明细
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalCount">返回数量</param>
        /// <param name="reconciliationCount">保单数量</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="reconciliationNum">保单号</param>
        /// <returns></returns>

        List<UnSettlementResponseVM> GetSettleListDetail(int batchId, string licenseNo, string reconciliationNum, out int reconciliationCount, out int totalCount, int pageIndex = 1, int pageSize = 10);



    }
}
