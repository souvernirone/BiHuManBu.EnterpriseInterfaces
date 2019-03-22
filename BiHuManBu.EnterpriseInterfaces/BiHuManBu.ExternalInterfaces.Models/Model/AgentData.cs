using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 业务员数据
    /// </summary>
    public class AgentData
    {
        /// <summary>
        /// 业务员id
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 业务员名字
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 报价车辆
        /// </summary>
        public int QuoteCarCount { get; set; }

        /// <summary>
        /// 报价次数
        /// </summary>
        public int QuoteCount { get; set; }

        /// <summary>
        /// 短信发送量
        /// </summary>
        public int SmsSendCount { get; set; }

        /// <summary>
        /// 回访次数
        /// </summary>
        public int ReturnVisitCount { get; set; }

        /// <summary>
        /// 预约到店
        /// </summary>
        public int AppointmentCount { get; set; }

        /// <summary>
        /// 出单量(从对账列表中抓取)
        /// </summary>
        public int SingleCount { get; set; }

        /// <summary>
        /// 出单量(已出保单列表中抓取数据)
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 战败单数
        /// </summary>
        public int DefeatCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// 父级业务员id
        /// </summary>
        public int ParentAgentId { get; set; }

        /// <summary>
        ///顶级业务员id
        /// </summary>
        public int TopAgentId { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime DataInTime { get; set; }

        /// <summary>
        ///续保车的数量
        /// </summary>
        public int BatchRenewalCount { get; set; }

        /// <summary>
        /// 代理人层级
        /// </summary>
        public int AgentLevel { get; set; }
    }
}
