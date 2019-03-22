using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class StoreAddressRquest
    {
        [Range(1, 2100000000, ErrorMessage = "代理人id不正确")]
        public int TopAgentId { get; set; }

        [Required(ErrorMessage = "请输入地址后再进行保存")]
        public string Address { get; set; }
    }
}
