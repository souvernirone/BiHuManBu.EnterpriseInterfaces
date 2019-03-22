using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 角色
    /// </summary>
    public interface IManagerRoleRepository
    {
        List<manager_role_db> AddManagerRole(int agentId, string name, int zhenBangType);
        manager_role_db GetRoleInfo(int roleId);
        List<manager_role_db> GetManagerRoleInfo(int topAgentId);
        string GetRoleName(int roleId);

        string RoleExistByAgentId(int agentId);
        /// <summary>
        /// 根据条件筛选数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        manager_role_db Find(Expression<Func<manager_role_db, bool>> predicate);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool AddRole(manager_role_db role);

        /// <summary>
        /// 跟新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool UpdateRole(manager_role_db role);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool DeleteRole(manager_role_db role);

        /// <summary>
        /// 根据代理人获取角色类型
        /// 陈亮  17-8-31  
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        int GetRoleTypeByAgentId(int agentId);

        /// <summary>
        /// 根据代理人获取角色类型
        /// </summary>
        /// <param name="listAgent"></param>
        /// <returns></returns>
        List<AgentIdAndRoleTyoeDto> GetRoleTypeByAgentId(List<int> listAgent);

        /// <summary>
        /// 给顶级代理添加助理角色 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="agentList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool AddHelperRole(IList<bx_agent> agentList, out int count);

        /// <summary>
        /// 根据添加查询角色 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="lamda"></param>
        /// <returns></returns>
        IQueryable<manager_role_db> GetList(Expression<Func<manager_role_db, bool>> lamda);

        int UpdateYunYingRole(int yunYingRole);

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

        /// <summary>
        /// 更新是否代报价
        /// </summary>
        /// <param name="managerRoleId">主键</param>
        /// <param name="isRequote"></param>
        /// <returns></returns>
        bool UpdateRequoteById(int managerRoleId, int isRequote);
    }
}
