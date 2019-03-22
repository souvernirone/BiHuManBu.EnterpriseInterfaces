using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class CameraBlackListRequest : BaseRequest2
    {
        public List<CameraBlackModel> ListCameraBlack { get; set; }
    }


    public class CameraBlackRequest : BaseRequest2
    {
        public string LicenseNo { get; set; }
    }
    public class CameraBlackModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 状态： selected 不做任何处理； add 新增；  del 删除
        /// </summary>
        public string Status { get; set; }
    }

    public class DelCameraBlackRequest : BaseRequest2
    {
        public int CameraBlackId { get; set; }
    }
}
