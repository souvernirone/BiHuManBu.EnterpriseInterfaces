using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IManagerRoleService
    {
        ManagerRoleListViewModel GetManagerRoleInfo(int topAgentId);

        ManagerRoleListViewModel GetRoleList(int topAgentId);


        string RoleExistByAgentId(int agentId);
        /// <summary>
        /// 添加或修改角色 zky  2017-08-03 /crm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        BaseViewModel AddOrUpdateRole(int roleId, string roleName, int topAgentId, string agentName, int? isRequote, List<ManagerModuleViewModel> list);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel DeleteRole(DeleteRoleRequest request);

        /// <summary>
        /// 给顶级代理人添加助理角色（新版上线后注册顶级代理时会添加，老的数据没有助理角色） 
        /// zky 2017-09-01 /crm
        /// </summary>
        /// <param name="roleSuccess">成功添加助理角色的数量</param>
        /// <param name="moduleSuccess">成功添加菜单的数量</param>
        /// <returns></returns>
        void OldAgentAddHepler(int testCount, out int roleSuccess, out int moduleSuccess);

        /// <summary>
        /// 获取顶级代理人下面拥有某个三级按钮权限的角色列表
        /// </summary>
        /// <param name="btnCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        IList<string> GetRoleListByBtnCode(string btnCode, int topAgentId);

        /// <summary>
        /// 获取顶级代理人下面拥有某个二级菜单权限的角色列表
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        IList<string> GetRoleListByModule(string moduleCode, int topAgentId);

        BaseViewModel UpdateRequoteById(UpdateRoleRequest request);
    }
}