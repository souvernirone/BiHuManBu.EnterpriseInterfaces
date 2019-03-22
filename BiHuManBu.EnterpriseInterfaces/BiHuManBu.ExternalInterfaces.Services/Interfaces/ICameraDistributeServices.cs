using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 摄像头录入 接口
    /// </summary>
    public interface ICameraDistributeServices
    {
        /// <summary>
        /// 分配人员及发送通知
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        string DistributeAndSendMsg(CameraDistributeModel request, bool carMoldTmp, int agentId, Tuple<bool, bool, bool> isTopAgent, ref bool isSuccess, List<bx_userinfo> existUserInfo);

        long GerRedirsSealman(int agentId, long index = 0, long index1 = 0);
        List<long> GetRedirsSealmans(int agentId);
        List<LeaveDate> FindSealmanLeave(int agentId);
        long GerRedirsMenber(int agentId, long index = 0, long _index = 0);

        List<long> GetRedirsMenber(int agentId);

        /// <summary>
        /// 设置出单员到缓存
        /// </summary>
        /// <param name="request"></param>
        void SetOrderAgentRedis(GetOrderAgentRequest request);

        List<bx_userinfo> GetUserinfoByLicenseAndAgent(long buid, int agent, string licenseno);
    }
}
