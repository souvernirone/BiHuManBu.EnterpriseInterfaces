using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ITempUserService
    {
        ///// <summary>
        ///// 获取临时车主信息 
        ///// </summary>
        ///// <param name="agentId">代理人主键编号</param>
        ///// <param name="buId">userinfo主键编号</param>
        ///// <param name="TempUserType">临时车主类型查询方式-默认null:查询全部,false:查询个人,true：查询公户</param>
        ///// <returns></returns>
        //Task<List<TempUserViewModel>> GetTempUserInfoAsync(int agentId, long buId, bool? TempUserType);
        /// <summary>
        /// 获取临时关系人信息 
        /// </summary>
        /// <param name="agentId">代理人主键编号</param>
        /// <param name="buId">userinfo主键编号</param>
        /// <param name="TempUserType">临时关系人类型查询方式-默认null:查询全部,false:查询个人,true：查询公户</param>
        /// <param name="temptype">1：临时车主 2：临时保险人</param>
        /// <returns></returns>
        List<TempUser> GetTempRelationAsync(int agentId, long buId, bool? tempUserType, int tempType);

        /// <summary>
        /// 老数据转新数据
        /// </summary>
        /// <returns></returns>
        bool DataChange();

        ///// <summary>
        ///// 保存临时车主
        ///// </summary>
        ///// <param name="tempUserViewModel">临时车主模型</param>
        ///// <param name="isEdit">修改标示：0:不修改 1：修改</param>
        ///// <returns></returns>
        //Task<bool> SaveTempUserInfoAsync(List<TempUserViewModel> tempUserViewModel, bool isEdit);

        /// <summary>
        /// 保存临时关系人
        /// </summary>
        /// <param name="tempRelationViewModel">实体模型：TempUser：临时关系人类  RelationDetailInfo：中间表关系类</param>
        /// <param name="step">1：临时车主 2：临时被保险人</param>
        /// <returns></returns>
        bool SaveTempRelationAsync(TempUserViewModel tempRelationViewModel, int step);
    }
}
