using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class FollowUpAllocationVM
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 未跟进数量
        /// </summary>
        public int NotFollowUpCount { get; set; }
        /// <summary>
        /// 跟进数量
        /// </summary>
        public int FollowUpCount { get; set; }
        /// <summary>
        /// 战败数量
        /// </summary>
        public int DefeatCount { get; set; }
        /// <summary>
        /// 出单数量
        /// </summary>
        public int OutOrderCount { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>
        public int TaskCount { get; set; }
        /// <summary>
        /// 渗透率
        /// </summary>
        public double PenetrationRate { get; set; }
    }
  

}
