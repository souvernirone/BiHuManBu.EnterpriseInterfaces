namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IManagerUserRoleRelationService
    {
        /// <summary>
        /// 更新ManagerUser时级联更新ManagerUserRoleRelation
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="operatorName"></param>
        /// <param name="roleId"></param>
        bool UpdateManagerUserToUpdateUserRoleRelation(string accountName, string operatorName, int roleId);
    }
}
