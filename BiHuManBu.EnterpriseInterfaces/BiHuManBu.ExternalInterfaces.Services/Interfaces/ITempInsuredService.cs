using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ITempInsuredService
    {
        /// <summary>
        /// 获取临时被保险人信息
        /// </summary>
        /// <param name="agentId">bx_agent.id</param>
        ///<param name="buId">bx_userinfo.Id</param>
        /// <returns></returns>
        Task<TempInsuredViewModel> GetTempInsuredInfoAsync(int agentId, long buId);
        /// <summary>
        /// 保存临时被保险信息
        /// </summary>
        /// <param name="tempInsuredViewModel">被保险人信息</param>
        /// <param name="isEdit">是否为编辑</param>
        /// <returns></returns>
        Task<bool> SaveTempInsuredInfoAsync(TempInsuredViewModel tempInsuredViewModel, bool isEdit);
        /// <summary>
        /// 报价用来存放临时邮箱电话
        /// </summary>
        /// <param name="userExpandRequest"></param>
        /// <returns></returns>
        Task<bool> SaveUserExpandAsync(UserExpandRequest userExpandRequest);
    }
}
