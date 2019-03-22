using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TeamIncomingSettingRepository : EfRepositoryBase<bx_zc_team_incoming_setting>, ITeamIncomingSettingRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public TeamIncomingSettingRepository(DbContext context) : base(context) { }
        public List<bx_zc_team_incoming_setting> FindList()
        {
            return Table.Where(i => i.setting_status == 1).ToList();
        }
        public int Add(string sql)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
                count = DataContextFactory.GetDataContext().SaveChanges();
                return count;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                count = 0;
            }
            return count;
        }

        public int Del()
        {
            int count = 0;
            try
            {
                string sql = string.Format("update bx_zc_team_incoming_setting set setting_status=0,update_time='{0}'", DateTime.Now.ToString());
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
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
