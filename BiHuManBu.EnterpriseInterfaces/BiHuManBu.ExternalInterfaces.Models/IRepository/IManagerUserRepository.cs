using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IManagerUserRepository
    {
        manageruser AddManagerUser(string name, string pwd, string mobile,int peopleType);
        void EditManagerUserRoleId(int id, int roleId);
        string GetUserRoleName(int roleId);
        void EditAgentRoleId(int AgentId, int roleId);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="user"></param>
        bool AddManagerUser(manageruser user);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool IsExist(Expression<Func<manageruser, bool>> predicate);

        /// <summary>
        /// 获取单个manageruser对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        manageruser Find(Expression<Func<manageruser, bool>> predicate);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool Update(manageruser user);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="oldRoleId"></param>
        /// <param name="newRoleId"></param>
        /// <returns></returns>
        bool UpdateRoleId(int oldRoleId, int newRoleId);

        /// <summary>
        /// 批量更新manageruser的managerRoleId  zky 2017-08-31 /crm
        /// </summary>
        /// <param name="names">名称</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        bool UpdateRoleIdByName(List<string> names, int roleId);

        /// <summary>
        /// 用户列表查询 用户列表查询 zky 2017-08-31 /运营后台
        /// </summary>
        /// <param name="accountType">0运营后台用户，1CRM用户</param>
        /// <param name="mobile">手机号</param>
        /// <param name="account">用户名</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IList<ManageruserViewModel> GetManageruserList(int accountType, string mobile, string account, int pageSize, int pageIndex, out int total);
    }
}
