using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class SettlementInsertedModel
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
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
        /// 数据所在代理人名称
        /// </summary>
        public string DataInAgentName { get { return _dataInAgentName; } set { _dataInAgentName = value; } }
        /// <summary>
        /// 数据所在代理人编号
        /// </summary>
        public int DataInAgentId { get; set; }
        private string _parentAgentName = string.Empty;
        /// <summary>
        /// 父级代理人名称
        /// </summary>
        public string ParentAgentName { get { return _parentAgentName; } set { _parentAgentName = value; } }

        private int _parentAgentId = -1;
        /// <summary>
        /// 父级代理人编号
        /// </summary>
        public int ParentAgentId { get { return _parentAgentId; } set { _parentAgentId = value; } }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public int SettleType { get; set; }
        private int _companyId = -1;
        /// <summary>
        /// 保司编号
        /// </summary>
        public int CompanyId { get { return _companyId; } set { _companyId = value; } }

        private string _companyName = string.Empty;
        /// <summary>
        /// 保司名称
        /// </summary>

        public string CompanyName { get { return _companyName; } set { _companyName = value; } }
        private string _channelName = string.Empty;
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get { return _channelName; } set { _channelName = value; } }

        private int _channelId = -1;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public int ChannelId { get { return _channelId; } set { _channelId = value; } }


    }
}
