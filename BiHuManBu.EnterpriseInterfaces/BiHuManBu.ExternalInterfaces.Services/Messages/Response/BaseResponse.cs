using System.Net;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class BaseResponse
    {
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrMsg { get; set; }

    }
}
