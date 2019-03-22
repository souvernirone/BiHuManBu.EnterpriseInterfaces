using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AddOrUpdateRoleRequest: BaseRequest
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Range(-1, 1000000)]
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        public string RoleName { get; set; }
        /// <summary>
        /// 数据Json字符串
        /// </summary>
        public string JsonString { get; set; }
        /// <summary>
        /// CustKey
        /// </summary>
        [Required]
        public string CustKey { get; set; }
        /// <summary>
        /// 当前代理人名称
        /// </summary>
        [Required]
        public string AgentName { get; set; }

        public Nullable<int> IsRequote { get; set; }
    }
}
