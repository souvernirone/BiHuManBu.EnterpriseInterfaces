using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class IsValidOrderByDrivingInfoRequest : BaseRequest2
    {
        public string LicenseNo { get; set; }
        public string CarVin { get; set; }
        public string EngineNo { get; set; }

        /// <summary>
        /// 请求类型，1车牌号，2车架号
        /// </summary>
        [Range(1, 2, ErrorMessage = "请求类型不能是1,2的其他数字")]
        public int TypeId { get; set; }
    }
}
