using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetUserInfoStatusRequest:BaseRequest
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 续保报价的代理人
        /// </summary>
        [Range(1,2100000000)]
        public int ChildAgent { get; set; }
    }
}
