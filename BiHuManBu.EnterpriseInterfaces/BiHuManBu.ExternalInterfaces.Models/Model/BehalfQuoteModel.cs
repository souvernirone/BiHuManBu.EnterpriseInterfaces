using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 代报价记录模型
    /// </summary>
    public class BehalfQuoteModel
    {
        /// <summary>
        /// 代报价人
        /// </summary>
        public string ReQuoteName { get; set; }
        /// <summary>
        /// 投保公司
        /// </summary>
        public string InsuredCompany { get; set; }
        /// <summary>
        /// 报价内容
        /// </summary>
        public string BehalfContent { get; set; }
        ///// <summary>
        ///// 代报价日期
        ///// </summary>
        //public string BehalfDate { get; set; }
        ///// <summary>
        ///// 代报价时间
        ///// </summary>
        //public string BehalfTime { get; set; }
    }
}
