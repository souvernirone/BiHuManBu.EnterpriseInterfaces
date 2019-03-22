using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SettlementResponseVM
    {
        /// <summary>
        /// 结算单编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 刷卡开始时间
        /// </summary>

        public DateTime SwingCardStartDate { get; set; }
        /// <summary>
        /// 刷卡结束时间
        /// </summary>

        public DateTime SwingCardEndDate { get; set; }
        private string _dataInAgentName = string.Empty;
        /// <summary>
        /// 业务来源代理人姓名
        /// </summary>
        public string DataInAgentName { get { return _dataInAgentName; } set { _dataInAgentName = value; } }
        private string _parentAgentName = string.Empty;
        /// <summary>
        /// 业务来源父级代理人姓名
        /// </summary>
        public string ParentAgentName { get { return _parentAgentName; } set { _parentAgentName = value; } }
        /// <summary>
        /// 结算状态
        /// </summary>
        public int SettleStatus { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>

        public double SettlePrice { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>

        public DateTime? SettleDate { get; set; }
        /// <summary>
        /// 承保公司编号
        /// </summary>
        public int CompanyId { get; set; }
        private string _companyName = string.Empty;
        /// <summary>
        /// 保司名称
        /// </summary>
        public string CompanyName { get { return _companyName; } set { _companyName = value; } }

        private string _channelName = string.Empty;
        /// <summary>
        /// 渠道id
        /// </summary>

        public int ChannelId { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get { return _channelName; } set { _channelName = value; } }
        /// <summary>
        /// 台账数量
        /// </summary>
        public int ReconciliationCount { get; set; }
        /// <summary>
        /// 对账状态
        /// </summary>
        public int ReconciliationStatus { get; set; }
        /// <summary>
        /// 发票状态
        /// </summary>
        public int ReceiptStatus { get; set; }
        /// <summary>
        /// 回款状态
        /// </summary>
        public int BackPriceStatus { get; set; }
    }
}
