namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// manager_user_role_relation 仓储
    /// </summary>
    public interface IManagerUserRoleRelationRepository : IRepositoryBase<manager_user_role_relation>
    {
        /// <summary>
        /// 添加manager_user_role_relation，由于id被定义为double，并且是自增的，ef在插入时有问题，所以写sql插入
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        bool Add(manager_user_role_relation relation);
    }
}
