using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IManagerModuleButtonRepository
    {
        IQueryable<manager_module_button> LoadAll();

        /// <summary>
        /// 查询角色拥有的按钮权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IList<manager_module_button> GetRoleButtonList(int roleId);
    }
}
