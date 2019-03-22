using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IManagerRoleModuleRelationRepository
    {
        void AddRoleModuleRelation(List<manager_role_db> role, string name, int regType,int zhenBangType);

        /// <summary>
        /// 删除角色所有的功能 zky 2017-08-03
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        bool DeleteRoleRelation(int roleId,int crmModuleType= 1);

        /// <summary>
        /// 删除角色所有的权限按钮 zky 2017-11-29
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        bool DeleteRoleButton(int roleId, int crmModuleType = 1);

        /// <summary>
        /// 给角色添加菜单 zky 2017-08-03
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="agentName">代理人姓名</param>
        /// <param name="list">菜单列表</param>
        /// <returns></returns>
        bool AddRoleRelationList(int roleId, string agentName, List<ManagerModuleViewModel> list);

        /// <summary>
        /// 根据条件查询List zky 2017-08-03
        /// </summary>
        /// <param name="lamda"></param>
        /// <returns></returns>
        List<manager_role_module_relation> GetList(Expression<Func<manager_role_module_relation, bool>> lamda);

        /// <summary>
        /// 给助理添加菜单 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="roleList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool HelperAddModule(IList<manager_role_db> roleList, out int count);

        /// <summary>
        /// 添加 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Add(manager_role_module_relation entity);

        /// <summary>
        /// 删除 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// 给多个角色添加同一个菜单 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="moduleCode"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        bool AddList(List<int> roleIds, string moduleCode, string creator);

        /// <summary>
        /// 根据id批量删除 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        bool DeleteList(List<int> idList);

    }
}
