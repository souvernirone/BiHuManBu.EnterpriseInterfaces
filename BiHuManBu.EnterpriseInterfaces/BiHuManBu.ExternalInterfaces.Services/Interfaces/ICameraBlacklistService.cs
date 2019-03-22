using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICameraBlacklistService
    {
        /// <summary>
        /// 添加摄像头进店数据黑名单
        /// </summary>
        /// <returns></returns>
        List<CameraBlackModel> AddCameraBlack(CameraBlackListRequest request);
        /// <summary>
        /// 查询当前代理当前车牌号是否在黑名单中
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bx_camera_blacklist GetCameraBlack(CameraBlackRequest request);
        /// <summary>
        /// 删除摄像头进店黑名单设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int DeleteCameraBlack(DelCameraBlackRequest request);
        /// <summary>
        /// 获取摄像头进店黑名单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<CameraBlackModel> GetCameraBlackList(BaseRequest2 request);
    }
}
