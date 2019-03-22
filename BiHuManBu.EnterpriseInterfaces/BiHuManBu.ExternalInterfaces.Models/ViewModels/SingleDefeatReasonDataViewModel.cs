using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class SingleDefeatReasonDataViewModel
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 战败原因标签编号
        /// </summary>
        public int DefeatReasonId { get; set; }
        /// <summary>
        /// 当前代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }

        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
    }
}
