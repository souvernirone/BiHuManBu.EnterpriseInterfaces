using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentMoblieServiceStateRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 手机唯一标识
        /// </summary>
        [Required(ErrorMessage = "手机唯一标识必须填写")]
        public string CustKey { get; set; }

        /// <summary>
        /// 网络状态  网络是否可用 0否 1是
        /// </summary>
        [Required(ErrorMessage = "网络状态必须填写")]
        public int IsAvailable { get; set; }

        /// <summary>
        /// 网络连接类型 1:wifi,2:4g,3:断开
        /// </summary>
        [Required(ErrorMessage = "网络连接类型必须填写")]
        public int NetWorkType { get; set; }

        /// <summary>
        /// 是否连接电源 0否 1是
        /// </summary>
        [Required(ErrorMessage = "是否连接电源必须填写")]
        public int IsConnectSupply { get; set; }

        /// <summary>
        /// 电池容量
        /// </summary>
        [Required(ErrorMessage = "电池容量必须填写")]
        public string BatteryCapacity { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }

    }
}
