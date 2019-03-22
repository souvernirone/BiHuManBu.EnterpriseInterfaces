using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICameraRepository
    {
        /// <summary>
        /// 获取摄像头 业务员信息
        /// </summary>
        /// <param name="agentId">代理人Id</param>
        /// <param name="type">数据类型 1=已选业务员、2=全部业务员</param>
        /// <returns></returns>
        List<SealmanViewModel> FindSealman(int agentId, int type, string mobile, string name, string agents);
        List<long> FindAgentIdBySealman(int agentId = 0);

        List<long> FindAgentIdByMenber(int agentId);
        /// <summary>
        /// 查看是否已设置业务员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int isExistSealMan(int agentId);

        int isExistSealMan(int agentId, int userId);

        //<summary>
        //保存 业务员信息
        //</summary>
        //<param name="userId"></param>
        //<returns></returns>
        string SaveSealman(int agentId, int userId, ref bool isSuccess);
        /// <summary>
        /// 删除 业务员
        /// </summary>
        /// <param name="whereStr">删除的条件</param>
        /// <param name="type"></param>
        /// <returns></returns>
        string DelSealman(int agentId, int userId);

        /// <summary>
        /// 删除 业务员
        /// </summary>
        /// <param name="whereStr">删除的条件</param>
        /// <param name="isSuccess">是否成功</param>
        /// <returns></returns>
        string DelSealman(string whereStr, ref bool isSuccess);

        /// <summary>
        /// 获取摄像头 业务员请假信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<LeaveDate> FindSealmanLeave(int userId);
        List<long> FindSealmanLeave();

        /// <summary>
        /// 保存 业务员请假信息
        /// </summary>
        /// <param name="userId">操作人</param>
        /// <param name="leave">请假时间</param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string SaveSealmanLeave(int userId, DateTime leave, ref bool isSuccess);

        /// <summary>
        /// 删除业务员的请假信息
        /// </summary>
        /// <param name="whereStr"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string delSealmanLeave(string whereStr, ref bool isSuccess);

        List<carMold> FindCarModel(int agentId);

        List<int> FindCarType();
        string SaveCarModel(int agentId, string carModes, ref bool isSuccess);

        string delCarModel(int Id, ref bool isSuccess);

        string ModifyCarModel(int agentId, string carModes, ref bool isSuccess);

        /// <summary>
        /// 检查是否是摄像头进店 是否分配
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="renewalType"></param>
        /// <returns>true=未分配 false=已分配</returns>
        bool IsExistUser(int uid, int renewalType);

        /// <summary>
        /// 撤销过滤
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool RevokeFiles(int userId);

        /// <summary>
        /// isTest=3(回收站) isTest=1(删除) 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isTest">isTest=1是删除、isTest=3 进回收站</param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string Remove(int userId, int isTest, ref bool isSuccess);
        string RemoveList(string userIds, int isTest, ref bool isSuccess);

        /// <summary>
        /// 获取该代理人的顶级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetToAgentIdByAgentId(int agentId);

        /// <summary>
        /// 设置车型Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carModlId"></param>
        /// <returns></returns>
        bool SetCarModelId(int userId, int carModlId);

        bool ExecuteNonQuery(string sql);
        List<GetCameraConfigByAgentViewModel> GetCameraConfigByTopAgent(int Agent);
        List<GetCameraConfigByAgentViewModel> GetCameraConfigByAgent(int Agent);
    }
}
