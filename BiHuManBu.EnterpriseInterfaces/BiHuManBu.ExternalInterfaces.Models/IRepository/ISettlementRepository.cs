using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ISettlementRepository
    {

        /// <summary>
        /// 批量增加待结算单
        /// </summary>
        /// <param name="unSettlementListVM">添加数据集合</param>
        /// <returns></returns>
        bool AddUnSettlementRange(List<UnSettlementRequestVM> unSettlementListVM);
        /// <summary>
        /// 获取待结算单列表关于保司
        /// </summary>
        /// <param name="swingCardStartDate">刷卡开始日期</param>
        /// <param name="swingCardEndDate">刷卡结束日期</param>
        /// <param name="channelIds">渠道集合</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="reconciliationCount">保单数</param>
        /// <param name="agentIds">代理人集合</param>
        /// <returns></returns>
        List<UnSettlementResponseVM> GetUnSettlementListAboutCompany(List<int> channelIds, int pageIndex, int pageSize,List<int> agentIds,out int totalCount, out double totalPrice, out int reconciliationCount, DateTime? swingCardStartDate = default(DateTime?), DateTime? swingCardEndDate = default(DateTime?));

        /// <summary>
        /// 获取待结算单列表关于保司
        /// </summary>
        /// <param name="swingCardEndDate">刷卡开始日期</param>
        /// <param name="swingCardStartDate">刷卡结束日期</param>
        /// <param name="agentIds">代理人集合</param>
        /// <param name="searchAgentType"> 1->代理人、2->网点、3->机构佣金、4->机构毛利</param>
        /// <param name="pageIndex">索引页</param>
        /// <param name="pageSize">页数量</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="reconciliationCount">保单数</param>
        /// <returns></returns>
        List<UnSettlementResponseVM> GetUnSettlementListNotAboutCompany(int searchAgentType, List<int> agentIds, int pageIndex, int pageSize, out int totalCount, out double totalPrice, out int reconciliationCount, DateTime? swingCardStartDate = default(DateTime?), DateTime? swingCardEndDate = default(DateTime?));

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="ids">待结算编号集合</param>
        /// <param name="settleType">结算类型：1->代理人佣金结算、2->网点佣金结算、3->机构佣金结算、4->机构毛利结算、5->保司手续费结算</param>
        /// <returns></returns>
        Tuple<bool, string> CreateSettlement(List<int> ids, int settleType);

        /// <summary>
        /// 获取结算列表关于保司
        /// </summary>
        /// <param name="channelIds">渠道集合</param>
        /// <param name="settlementListRequestVM">查询条件模型</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="agentIds">代理人集合</param>
        /// <param name="settleCount">结算数量</param>
        /// <returns></returns>
        List<SettlementResponseVM> GetSettlementListAboutCompany(List<int> channelIds, List<int>agentIds,SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice,out int settleCount);
        /// <summary>
        /// 获取结算列表非保司
        /// </summary>
        /// <param name="agentIds">查询代理人集合</param>
        /// <param name="settlementListRequestVM">查询条件模型</param>
        /// <param name="totalCount">返回总数</param>
        /// <param name="totalPrice">总金额</param>
        /// <param name="settleCount">结算数量</param>
        /// <returns></returns>

        List<SettlementResponseVM> GetSettlementListNotAboutCompany(List<int> agentIds, SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice,out int settleCount);

        /// <summary>
        /// 标记状态操作
        /// </summary>
        /// <param name="settlementUpdateStatusVM">标记状态操作模型</param>
        /// <param name="guids">操作的保单标识集合</param>


        /// <returns></returns>
     bool UpdateStaus(SettlementUpdateStatusVM settlementUpdateStatusVM, out List<string> guids);
        /// <summary>
        /// 回退到待结算单
        /// </summary>
        /// <param name="batchId">结算批次号</param>
        /// <param name="settleIds">结算明细号</param>
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
        ///<param name="licenseNo">车牌号</param>
        ///<param name="reconciliationNum">保单号</param>
        /// <returns></returns>

        List<UnSettlementResponseVM> GetSettleListDetail(int batchId, string licenseNo, string reconciliationNum, out int totalCount, out int reconciliationCount, int pageIndex = 1, int pageSize = 10);
    }
}
