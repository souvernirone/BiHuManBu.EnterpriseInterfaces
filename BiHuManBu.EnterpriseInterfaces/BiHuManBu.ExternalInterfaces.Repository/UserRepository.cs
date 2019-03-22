using System;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{ 
    public class UserRepository : IUserRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public int Add(user user)
        {
            int userid = 0;
            try
            {
                var tt = DataContextFactory.GetDataContext().user.Add(user);
                DataContextFactory.GetDataContext().SaveChanges();
                userid = tt.UserId;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return userid;
        }

        public user Find(int userId)
        {
            user user = new user();
            try
            {
                user = DataContextFactory.GetDataContext().user.FirstOrDefault(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return user;
        }

        public user FindByOpenId(string openId)
        {
            user user = new user();
            try
            {
                user = DataContextFactory.GetDataContext().user.FirstOrDefault(x => x.Openid == openId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return user;
        }

        public user FindByMobile(string mobile)
        {
            user user = new user();
            try
            {
                user = DataContextFactory.GetDataContext().user.FirstOrDefault(x => x.Mobile == mobile);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return user;
        }

        public int Delete(int userId)
        {
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand("DELETE FROM user WHERE userId=" + userId + "");
        }

        public int Update(user user)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().user.AddOrUpdate(user);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
    }
}
