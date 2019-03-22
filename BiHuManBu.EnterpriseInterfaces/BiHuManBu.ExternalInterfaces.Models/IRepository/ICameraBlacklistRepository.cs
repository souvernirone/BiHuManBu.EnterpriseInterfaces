using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ICameraBlacklistRepository
    {
        int AddCameraBlack(string sql);
        bx_camera_blacklist GetCameraBlack(CameraBlackRequest request);
        int DeleteCameraBlack(DelCameraBlackRequest request);
        List<CameraBlackModel> GetCameraBlackList(BaseRequest2 request);
    }
}
