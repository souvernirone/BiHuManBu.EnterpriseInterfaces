using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetTableHeaderRequest:BaseRequest2
    {
        /// <summary>
        /// 表格名称
        /// </summary>
        [Required,MaxLength(100,ErrorMessage = "TableName的长度不能超过100个字符")]
        public string TableName { get; set; }
    }
}
