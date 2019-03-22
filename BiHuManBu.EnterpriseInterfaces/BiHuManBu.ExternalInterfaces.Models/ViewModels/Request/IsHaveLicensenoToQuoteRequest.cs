using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class IsHaveLicensenoToQuoteRequest:BaseRequest
    {
        /// <summary>
        /// 当前试算代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "代理人Id不能小于1")]
        public int ChildAgent { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string VinNo { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 请求类型，1车牌号，2车架号发动机号
        /// </summary>
        [Range(1, 2, ErrorMessage = "请求类型不能是1,2的其他数字")]
        public int TypeId { get; set; }
    }
}
