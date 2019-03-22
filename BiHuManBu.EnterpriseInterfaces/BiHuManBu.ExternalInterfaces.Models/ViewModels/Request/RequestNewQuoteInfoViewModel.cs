using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class RequestNewQuoteInfoViewModel
    {
       /// <summary>
       /// 代理人ID
       /// </summary>
       public int AgentId { get; set; }
       public long Buid { get; set; }
        ///// <summary>
        ///// 上年出险次数
        ///// </summary>
        //public int LastYearAcctimes { get; set; }
        ///// <summary>
        ///// 上年赔款金额
        ///// </summary>
        //public double LastYearClaimamount { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public DateTime? BizStartDate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public DateTime? ForceStartDate { get; set; }
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 被保险人电话
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人身份证
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人地址
        /// </summary>
        public string InsuredAddress { get; set; }
        /// <summary>
        /// 被保险人证件类型,1=身份证、2=组织机构代码证
        /// </summary>
        public int InsuredIdType { get; set; }

       public string Email { get; set; }
    }
}
