using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CameraConfigRepository : ICameraConfigRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public int Update(bx_camera_config model)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_camera_config.AddOrUpdate(model);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }

            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public List<bx_camera_config> Get(int agentId)
        {
            List<bx_camera_config> list = new List<bx_camera_config>();
            try
            {
                string agentIdStr = agentId.ToString();
                list = DataContextFactory.GetDataContext().bx_camera_config.Where(x => x.park_id == agentIdStr).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return list;
        }

    }
}
