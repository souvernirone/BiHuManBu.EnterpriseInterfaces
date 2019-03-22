using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class UserInfoRepeatModel
    {
        public int Buid { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 车型
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 最后一次跟进时间
        /// </summary>
        public string NextReviewDate { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 客户电话
        /// </summary>
        public string ClientMobile { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
    }
}
