using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class EntryOverViewRequest : AppAgentUKeyRequest
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int RoleType { get; set; }

        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 1-昨天,2-近7天,3-近30天
        /// </summary>
        public int TimeType { get; set; }
    }

    public class CustomerAnalysisRequest : AppAgentUKeyRequest
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int RoleType { get; set; }

        /// <summary>
        /// 保险到期时间
        /// </summary>
        [Required]
        public DateTime DueDate { get; set; }
    }
}
