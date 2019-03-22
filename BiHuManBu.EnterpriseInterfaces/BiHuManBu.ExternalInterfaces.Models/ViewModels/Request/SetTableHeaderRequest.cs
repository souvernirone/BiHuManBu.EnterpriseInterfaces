using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SetTableHeaderRequest:BaseRequest2
    {
        /// <summary>
        /// 表格名称
        /// </summary>
        [Required]
        public string TableName { get; set; }
        
        /// <summary>
        /// 表格配置信息
        /// </summary>
        public string Json { get; set; }
    }
}
