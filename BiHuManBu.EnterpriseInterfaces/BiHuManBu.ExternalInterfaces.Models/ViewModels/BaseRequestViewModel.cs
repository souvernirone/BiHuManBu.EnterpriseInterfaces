using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 请求基础模型
    /// </summary>
   public class BaseRequestViewModel
   {
       /// <summary>
       /// 
       /// </summary>
       [Required(ErrorMessage = "SecCode不能为空")]
       public string SecCode { get; set; }
       /// <summary>
       /// 
       /// </summary>
       [Required(ErrorMessage = "CustKey不能为空")]
       public string CustKey { get; set; }
    }
}
