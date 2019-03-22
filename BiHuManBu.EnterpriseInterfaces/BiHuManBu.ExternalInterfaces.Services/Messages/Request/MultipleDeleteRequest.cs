using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 批量删除客户数据
    /// </summary>
    public class MultipleDeleteRequest : BaseCustomerSearchRequest
    {
        /// <summary>
        /// 删除类型
        /// </summary>
        [RegularExpression("^(Soft|Thorough)$", ErrorMessage = "删除类型错误")]
        public string  DeleteType { get; set; }

        /// <summary>
        /// 是否全部
        /// </summary>
        [Range(0, 1, ErrorMessage = "IsAll参数只能是0或1")]
        public int IsAll { get; set; }

        #region 为了SecCode验证需要添加的参数，没有进行逻辑处理

        /// <summary>
        /// 
        /// </summary>
        public int OnlyCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CustomerYears { get; set; }
        #endregion
    }
}
