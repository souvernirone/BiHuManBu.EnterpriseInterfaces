using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class UnSettlementResponseVM
    {
        public int Id { get; set; }
        /// <summary>
        /// 刷卡日期
        /// </summary>
        public string SwingCardDate { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 保单号
        /// </summary>
        public string ReconciliationNum { get; set; }

        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 险种类型
        /// </summary>
        public int InsuranceType { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 保司id
        /// </summary>
        /// 
        public int CompanyId { get; set; }
        private string _companyName = string.Empty;
        /// <summary>
        /// 保司名称
        /// </summary>
        public string CompanyName { get { return _companyName; } set { _companyName = value; } }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

       
        public string InsuranceName { get; set; }

        /// <summary>
        /// 保费金额
        /// </summary>
        public decimal InsurancePrice { get; set; }
        /// <summary>
        /// 业务来源代理人名称
        /// </summary>

        public string DataInAgentName { get; set; }
        /// <summary>
        /// 业务来源父级代理人名称
        /// </summary>
        public string ParentAgentName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>

        public decimal Price { get; set; }
        private string _catchSingleTime = string.Empty;
        /// <summary>
        /// 抓单时间
        /// </summary>

        public string CatchSingleTime { get { return _catchSingleTime; } set { _catchSingleTime = value; } }

        public string  Guid { get; set; }
        /// <summary>
        /// 车船税
        /// </summary>
        public double TaxPrice { get; set; }
        /// <summary>
        /// 保司费率
        /// </summary>
        public double SettleRate { get; set; }

    }
}
