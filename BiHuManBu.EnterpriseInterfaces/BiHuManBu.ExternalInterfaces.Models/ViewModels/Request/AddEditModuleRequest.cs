using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AddEditModuleRequest
    {
        [Required]
        public string saveType { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "module_code的长度应在2到50之间")]
        public string moduleCode { get; set; }

        [StringLength(30, MinimumLength = 2, ErrorMessage = "module_name的长度应在2到30之间")]
        public string moduleName { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "pater_code的长度应在2到50之间")]
        public string paterCode { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "action_url的长度应在2到50之间")]
        public string actionUrl { get; set; }

        [Range(1, 50, ErrorMessage = "order_by的值应在1到50之间")]
        public int orderby { get; set; }
        public string operateName { get; set; }
        /// <summary>
        /// 状态 1:启用、0：禁用
        /// </summary>
        [Range(0, 1)]
        public int status { get; set; }



       


    }
}
