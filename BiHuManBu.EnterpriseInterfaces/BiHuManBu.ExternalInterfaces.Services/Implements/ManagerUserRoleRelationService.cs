using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class ManagerUserRoleRelationService : IManagerUserRoleRelationService
    {
        private readonly IManagerUserRoleRelationRepository _managerUserRoleRelationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="managerUserRoleRelationRepository"></param>
        public ManagerUserRoleRelationService(IManagerUserRoleRelationRepository managerUserRoleRelationRepository)
        {
            _managerUserRoleRelationRepository = managerUserRoleRelationRepository;
        }

        /// <summary>
        /// 更新ManagerUser时级联更新ManagerUserRoleRelation
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="operatorName"></param>
        /// <param name="roleId"></param>
        public bool UpdateManagerUserToUpdateUserRoleRelation(string accountName, string operatorName, int roleId)
        {
            //修改manager_user_role_relation
            manager_user_role_relation relation = new manager_user_role_relation
            {
                user_name = accountName,
                role_id = roleId,
                creator_full_name = operatorName,
                creator_time = DateTime.Now,
                creator_name = operatorName
            };

            return _managerUserRoleRelationRepository.Add(relation);
        }
    }
}
