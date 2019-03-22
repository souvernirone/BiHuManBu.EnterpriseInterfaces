using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ValidationViewModel : BaseViewModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string MsgCode { get; set; }
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        public int TopAgent { get; set; }
    }
}
