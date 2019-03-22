using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CodeValidationViewModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 验证码类型(用途) 0注册;1导出;3支付;4修改客户状态和类别(数据超过500条)
        /// </summary>
        public int CodeType { get; set; }
        public string SecCode { get; set; }
    }
}
