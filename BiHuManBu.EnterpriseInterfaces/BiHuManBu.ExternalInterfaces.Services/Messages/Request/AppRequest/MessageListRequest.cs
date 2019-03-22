
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest

{
    public class MessageListRequest:BaseRequest
    {
        [Range(1,2100000000)]
        public int ChildAgent { get; set; }

        [Range(1, 10000)]
        public int PageSize { get; set; }
        [Range(1, 10000)]
        public int CurPage { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
