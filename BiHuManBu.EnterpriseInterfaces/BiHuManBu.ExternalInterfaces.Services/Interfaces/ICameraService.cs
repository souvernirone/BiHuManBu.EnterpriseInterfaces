using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICameraService
    {
        #region  设置
        /// <summary>
        /// 获取摄像头 业务员信息
        /// </summary>
        /// <param name="Camera"></param>
        /// <returns></returns>
        List<SealmanViewModel> FindSealman(CameraRequest Camera);
        /// <summary>
        /// 查看是否已设置业务员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool isExistSealMan(int agentId);
        /// <summary>
        /// 保存业务员
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string SaveSealman(CameraRequest Camera, ref bool isSuccess);
        /// <summary>
        /// 删除业务员
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string DelSealman(CameraRequest Camera, ref bool isSuccess);
        /// <summary>
        ///  获取摄像头 业务员请假信息
        /// </summary>
        /// <param name="Camera"></param>
        /// <returns></returns>
        List<LeaveDate> FindSealmanLeave(CameraRequest Camera);
        /// <summary>
        /// 保存 业务员请假信息
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string SaveSealmanLeave(CameraRequest Camera, ref bool isSuccess);
        /// <summary>
        /// 获取设置的车型
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<carMold> FindCarType(int agentId);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<int> FindCarType();
        /// <summary>
        /// 保存车型
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string SaveCarModel(CameraRequest Camera, ref bool isSuccess);
        #endregion
        #region  列表
        /// <summary>
        /// 摄像头用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CustomerListViewModel> FindUserListAsync(GetCustomerListRequest request);
        /// <summary>
        /// 摄像头用户列表总数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DistributedCountViewModel FindUserCount(GetCustomerListRequest request);

        /// <summary>
        /// 导出摄像头进店列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="type"></param>
        /// <param name="isFiles"></param>
        /// <returns></returns>
        CustomerListExportViewModel GetExportUserList(GetCustomerListRequest request, int type = 1, int isFiles = 1);

        /// <summary>
        /// 获取摄像头进店列表导出总条数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DistributedCountViewModel GetExportCount(GetCustomerListRequest request);
        #endregion
        #region 删除、回撤
        /// <summary>
        /// 撤销过滤
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        BaseViewModel RevokeFiles(int userId);
        /// <summary>
        /// isTest=3(回收站) isTest=1(删除) 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isTest">isTest=1是删除、isTest=3 进回收站</param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        string Remove(int userId, int isTest, ref bool isSuccess);
        string RemoveList(List<long> userList, int isTest, ref bool isSuccess);
        #endregion
        /// <summary>
        /// 为摄像头进店车辆设置车型标签
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <param name="carModelKey"></param>
        /// <returns></returns>
        bool SetCarModlId(int agentId, int userId, string carModelKey);
        /// <summary>
        /// 保存销售到缓存中
        /// </summary>
        /// <param name="agentId"></param>
        void SaveSealToRedirs(int agentId);
        /// <summary>
        /// 把业务员请假信息保存到缓存中
        /// </summary>
        /// <param name="userId"></param>
        void SaveLeaveToRedirs(int userId);

        int FindUserCountNew(GetCustomerListRequest request);

        //int FindUserAllCount(GetCustomerListRequest request, int type = 1, int isFiles = 1);

        /// <summary>
        /// 摄像头进店历史记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel GetCameraHistoryList(CameraHistoryListRequest request);

        /// <summary>
        /// 摄像头进店历史记录数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel GetCameraHistoryCount(CameraHistoryCountRequest request);

        /// <summary>
        /// 摄像头进店历史记录导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel GetCameraHistoryExport(CameraHistoryExportRequest request);

        /// <summary>
        /// 是否有摄像头
        /// </summary>
        /// <param name="childagent"></param>
        /// <returns></returns>
        bool HasCamera(int childagent);
        BaseViewModel GetCameraConfigByAgent(BaseRequest2 request);
        BaseViewModel SetCameraConfig(SetCameraConfigRequest request);
        List<bx_camera_config> GetCameraConfig(int agentId);
        List<bx_camera_config> GetCameraConfigSet(int agentId);
    }
}
