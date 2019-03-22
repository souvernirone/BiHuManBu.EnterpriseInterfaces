using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface ITempUserRepository
    {
        ///// <summary>
        ///// 获取临时车主信息
        ///// </summary>
        ///// <param name="agentId">bx_agent.id</param>
        /////<param name="buId">bx_userinfo.buid</param>
        ///// <returns></returns>
        //List<bx_tempuserinfo> GetTempUserInfoAsync(int agentId, long buId, bool? TempUserType);

        /// <summary>
        /// 获取临时关系人信息
        /// </summary>
        /// <param name="agentId">bx_agent.id</param>
        ///<param name="buId">bx_userinfo.buid</param>
        /// <returns></returns>
        List<bx_tempuserinfo> GetTempRelationAsync(int agentId, long buId, bool? TempUserType, int temptype);
        ///// <summary>
        ///// 保存临时车主信息
        ///// </summary>
        ///// <param name="tempInsuredInfo">被保险人信息</param>
        ///// <param name="isEdit">是否为编辑</param>
        ///// <returns></returns>
        //Task<bool> SaveTempUserInfoAsync(List<bx_tempuserinfo> tempUserInfo, bool isEdit);

        List<bx_tempinsuredinfo> GetOldData();
        /// <summary>
        /// 保存临时关系人信息
        /// </summary>
        /// <param name="tempRelation">保存临时关系人</param>
        /// <param name="isEdit">是否为编辑</param>
        /// <returns></returns>
        bool SaveTempRelationAsync(List<bx_tempuserinfo> tempRelation,List<RelationDetailInfo> relation, int step);


        /// <summary>
        /// 保存临时关系人信息
        /// </summary>
        /// <param name="tempRelation">保存临时关系人</param>
        /// <param name="isEdit">是否为编辑</param>
        /// <returns></returns>
        bool SaveoldTempUser(List<bx_tempuserinfo> tempRelation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempUserInfo"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        bool UpdateTempUserInfo(bx_tempuserinfo tempUserInfo, bool isEdit);
    }
}
