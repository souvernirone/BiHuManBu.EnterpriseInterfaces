namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IUserRepository
    {
        int Add(user user);
        user Find(int userId);
        user FindByOpenId(string openId);
        user FindByMobile(string mobile);
        int Delete(int userId);
        int Update(user user);
    }
}
