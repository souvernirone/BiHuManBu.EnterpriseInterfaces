
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.WChat
{
    /// <summary>
    /// 
    /// </summary>
    public class GetOpenIdByLicensenoRequest : BaseRequestViewModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 顶级代理人
        /// </summary>
        public int TopAgentId { get; set; }

        /// <summary>
        /// 1，根据车牌请求OpenId 2，根据OpenId请求车牌
        /// </summary>
        public int RequestType { get; set; }
    }
}
