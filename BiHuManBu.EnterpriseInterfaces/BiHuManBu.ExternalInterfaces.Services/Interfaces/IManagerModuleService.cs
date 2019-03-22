using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IManagerModuleService
    {
        /// <summary>
        /// 展示菜单(添加、修改) zky 2017-08-03 /crm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        List<ManagerModuleViewModel> LoadManagerModule(int roleId = -1, int roleType = -1);

        /// <summary>
        /// 获取菜单状态 zky 2017-08-10
        /// </summary>
        /// <param name="list"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        string GetRoleModuleStatus(IList<manager_role_module_relation> list, string code);

        /// <summary>
        /// 刷库的方法（给系统管理员、管理员添加 三级菜单按钮的权限）zky 2017-12-21 /crm
        /// </summary>
        /// <returns></returns>
        int ManagerAddButton();

        BaseViewModel AddEditModule(AddEditModuleRequest request);


        bool DeleteModule(string moduleCode);
        
    }
}
