using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AreaRepository : IAreaRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");

        public List<bx_area> Find()
        {
            List<bx_area> bxAreas=new List<bx_area>();
            try
            {
                bxAreas= DataContextFactory.GetDataContext().bx_area.Where(x => x.Pid == 0 && x.Name != "中国").ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return bxAreas;
        }

        public List<bx_area> FindByPid(int pid)
        {
            List<bx_area> bxAreas = new List<bx_area>();
            try
            {
                bxAreas = DataContextFactory.GetDataContext().bx_area.Where(x => x.Pid == pid).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return bxAreas;
        }

        public List<bx_area> GetAll()
        {
            List<bx_area> bxAreas = new List<bx_area>();
            try
            {
                bxAreas = DataContextFactory.GetDataContext().bx_area.Where( x=>x.Name != "中国").ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return bxAreas;
        }
    }
}
