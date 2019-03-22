using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class CarRenewalStatisticsDetailTable
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
        public DateTime BizEndDate { get; set; }
        /// <summary>
        /// 商业险到期年份
        /// </summary>
        public string BizEndDateYear { get; set; }
        /// <summary>
        /// 较强险到期时间
        /// </summary>
        public DateTime ForceEndDate { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public string CustomerStatusName { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 数据回访状态
        /// </summary>
        public int CustomerStatus { get; set; }
        ///// <summary>
        ///// 是否跟进
        ///// </summary>
        //public int IsFollowUp { get; set; }
        ///// <summary>
        ///// 是否出单
        ///// </summary>
        //public int IsOutOrder { get; set; }
        ///// <summary>
        ///// 是否战败
        ///// </summary>
        //public int IsDefeat { get; set; }

        /// <summary>
        /// 录入状态创建时间
        /// </summary>
        public DateTime StatusCreateTime { get; set; }
        /// <summary>
        /// 原数据创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 原数据更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int Deleted { get; set; }
        public int CategoryInfoId { get; set; }
        public string StatusCreateTime_Year_Month { get; set; }
    }
}
