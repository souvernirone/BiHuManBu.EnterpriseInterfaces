using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRenewalStatisticsService.Models
{
    public class AnswerCallTimsStatisticsDetailTable
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// bx_userinfo.id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 父级代理人编号
        /// </summary>
        public int ParentAgentId { get; set; }
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string BizEndDate { get; set; }
        /// <summary>
        /// 较强险到期时间
        /// </summary>
        public string ForceEndDate { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public string CustomerStatus { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 通话创建时间
        /// </summary>
        public string DataInTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
