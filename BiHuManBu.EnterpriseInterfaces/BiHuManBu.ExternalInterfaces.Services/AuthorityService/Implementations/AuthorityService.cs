using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services.AuthorityService.Implementations
{
    public class AuthorityService : IAuthorityService
    {
        /// <summary>
        /// 系统默认角色
        /// </summary>
        private static int[] SystemRoleType = { 0, 3, 4, 5 };

        private readonly IManagerRoleRepository _managerRoleRepository;
        private readonly IManagerRoleButtonRelationRepository _managerRoleButtonRelationRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IManagerRoleModuleRelationRepository _managerRoleModuleRelationRepository;
        private readonly IManagerModuleRepository _managerModuleRepository;

        public AuthorityService(
            IManagerRoleRepository managerRoleRepository,
            IManagerRoleButtonRelationRepository managerRoleButtonRelationRepository,
            IAgentRepository agentRepository,
            IManagerRoleModuleRelationRepository managerRoleModuleRelationRepository,
            IManagerModuleRepository managerModuleRepository)
        {
            _managerRoleRepository = managerRoleRepository;
            _managerRoleButtonRelationRepository = managerRoleButtonRelationRepository;
            _agentRepository = agentRepository;
            _managerRoleModuleRelationRepository = managerRoleModuleRelationRepository;
            _managerModuleRepository = managerModuleRepository;
        }

        public bool IsHasDistributeAuth(int agentId)
        {
            return _managerRoleButtonRelationRepository.HasBtnAuth(agentId, BtnAuthType.btn_recycle.ToString());
        }

        public bool IsSystemAdminOrAdmin(int agentId)
        {
            var result = false;
            var roleTyoe = _managerRoleRepository.GetRoleTypeByAgentId(agentId);
            if (roleTyoe == 3 || roleTyoe == 4)
                result = true;
            return result;
        }
        public bool IsAdmin(int agentId)
        {
            var result = false;
            var roleTyoe = _managerRoleRepository.GetRoleTypeByAgentId(agentId);
            if (roleTyoe == 3 || roleTyoe == 4)
                result = true;
            return result;
        }

        public bool IsSystemRole(int agentId)
        {
            var roleTyoe = _managerRoleRepository.GetRoleTypeByAgentId(agentId);
            return SystemRoleType.Contains(roleTyoe);
        }

        public Dictionary<int, bool> HasDistributeAuth(List<int> listAgentId)
        {
            var dic = new Dictionary<int, bool>();
            // 为了避免有的agent不存在角色，所以将最后结果初始化为false
            foreach (var item in listAgentId)
            {
                dic.Add(item, false);
            }

            // 判断是否有分配权限
            var hasDistriAuth = _managerRoleButtonRelationRepository.HasBtnAuth(listAgentId, BtnAuthType.btn_recycle.ToString());
            foreach (var item in hasDistriAuth)
            {
                if (item.Result)
                    dic[item.AgentId] = true;
            }
            return dic;
        }

        /// <summary>
        /// 是否拥有分配标签
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool HasDistributedLabel(int agentId)
        {
            var agentItem = _agentRepository.GetAgent(agentId);
            if (agentItem == null)
            {
                return false;
            }
            var roleInfo = _managerRoleRepository.GetRoleInfo(agentItem.ManagerRoleId);
            if (roleInfo == null)
            {
                return false;
            }

            if (roleInfo.role_type == 3 || roleInfo.role_type == 4)
            {
                // 管理员和系统管理员始终显示未分配标签
                return true;
            }
            else
            {

                //当前代理人拥有的权限
                var moduleCode = _managerRoleModuleRelationRepository.GetList(t => t.role_id == agentItem.ManagerRoleId).ToList();
                var batchRenewal = moduleCode.Where(t => t.module_code == "batchRenewal_list").FirstOrDefault();

                //判断是否有批续 batchRenewal_list
                if (batchRenewal != null)
                {
                    return true;
                }
                else
                {
                    var buttonCode = _managerRoleButtonRelationRepository.GetList(t => t.role_id == agentItem.ManagerRoleId);
                    var distributeBtn = buttonCode.Where(t => t.module_code == "btn_recycle").FirstOrDefault();
                    // 判断是否有分配
                    if (distributeBtn != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
