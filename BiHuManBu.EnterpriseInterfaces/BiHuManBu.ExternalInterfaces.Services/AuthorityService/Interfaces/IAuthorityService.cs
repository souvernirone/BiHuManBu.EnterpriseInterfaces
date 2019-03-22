using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces
{
    public interface IAuthorityService
    {
        /// <summary>
        /// 判断是否是系统管理员或者管理员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsSystemAdminOrAdmin(int agentId);

        /// <summary>
        /// 判断是否是管理员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsAdmin(int agentId);

        /// <summary>
        /// 是否有分配权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsHasDistributeAuth(int agentId);

        /// <summary>
        /// 此代理人的角色是否是系统角色
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsSystemRole(int agentId);

        ///// <summary>
        ///// 是否有分配权限
        ///// </summary>
        ///// <param name="agentId"></param>
        ///// <returns></returns>
        //bool HasDistributeAuth(int agentId);

        /// <summary>
        /// 是否有分配权限
        /// </summary>
        /// <param name="listAgentId"></param>
        /// <returns></returns>
        Dictionary<int, bool> HasDistributeAuth(List<int> listAgentId);

        /// <summary>
        /// 是否存在分配+批续权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool HasDistributedLabel(int agentId);

    }
}
