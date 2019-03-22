using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TeamTaskSettingRepository : EfRepositoryBase<bx_zc_team_task_setting>, ITeamTaskSettingRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public TeamTaskSettingRepository(DbContext context) : base(context)
        {
        }

        public bx_zc_team_task_setting FindModel()
        {
            return Table.FirstOrDefault(i => i.is_delete == 0);
        }
        public int Add(bx_zc_team_task_setting model)
        {
            int id = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_zc_team_task_setting.Add(model);
                DataContextFactory.GetDataContext().SaveChanges();
                id = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                id = 0;
            }
            return id;
        }

        public int Del()
        {
            int count = 0;
            try
            {
                string sql = string.Format("update bx_zc_team_task_setting set is_delete=1,create_time='{0}'",DateTime.Now.ToString());
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
