using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IManagerRoleButtonRelationRepository:IRepositoryBase<manager_role_button_relation>
    {
        IQueryable<manager_role_button_relation> LoadAll();

        /// <summary>
        /// 判断代理人是否有btn权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="btn_code"></param>
        /// <returns></returns>
        bool HasBtnAuth(int agentId,string btn_code);

        /// <summary>
        /// 判断代理人是否有btn权限
        /// </summary>
        /// <param name="listAgentId"></param>
        /// <param name="btn_code"></param>
        /// <returns></returns>
        List<AgentIdAndBool> HasBtnAuth(List<int> listAgentId, string btn_code);

        /// <summary>
        /// 刷库的方法（给系统管理员、管理员添加 三级菜单按钮的权限）zky 2017-12-21 /crm
        /// </summary>
        /// <returns></returns>
        int ManagerAddButton();
    }
}
