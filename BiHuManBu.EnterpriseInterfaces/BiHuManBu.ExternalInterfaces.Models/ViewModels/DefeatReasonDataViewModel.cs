using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class DefeatReasonDataViewModel
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CustomerCategory { get; set; }
        /// <summary>
        /// bx_defeatreasonhistory.Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 战败原因
        /// </summary>
        public string DefeatReason { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 战败原因编号
        /// </summary>
        public int DefeatReasonId { get; set; }
        /// <summary>
        /// 创建时间字符串格式
        /// </summary>
        public string CreateTimeStr { get; set; }
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
    }

    public class DefeatReasonMobileDetails
    {
        public string LicenseNo { get; set; }
        public string DefeatReason { get; set; }
    }
}
