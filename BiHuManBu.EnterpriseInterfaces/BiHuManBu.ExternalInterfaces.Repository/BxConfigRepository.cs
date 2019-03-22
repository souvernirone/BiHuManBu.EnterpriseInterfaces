using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class BxConfigRepository : IBxConfigRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");

        private readonly EntityContext db = DataContextFactory.GetDataContext();

        public BxConfigRepository()
        {
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="config">实体对象</param>
        /// <returns></returns>
        public int Update(bx_config config)
        {
            int result = 0;
            try
            {
                db.Set<bx_config>().Attach(config);
                db.Entry<bx_config>(config).State = EntityState.Modified;
                result = db.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 根据配置键 获取配置值
        /// </summary>
        /// <param name="configKey">键</param>
        /// <returns></returns>
        public List<bx_config> FindByConfigKey(string configKey)
        {
            return db.bx_config.Where(t => t.config_key == configKey).ToList();
        }

        public int Add(bx_config model)
        {
            try
            {
                var result = db.bx_config.Add(model);
                db.SaveChanges();
                return result.id;
            }
            catch (DbEntityValidationException err)
            {
                return -1;
            }
           
        }

        /// <summary>
        /// 修改  是否验证版本号
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public bool UpdateByConfigKey_Isupload(RequestKeyConfig request)
        {
            int result = 0;
            try
            {
                bx_config model = db.bx_config.Where(t => t.config_key == request.ConfigKey).ToList().FirstOrDefault();
                
                if (model.config_value == request.KeyValue)
                {
                    result = 1;
                }
                else
                {
                    model.config_value = request.KeyValue;
                    result = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                
               logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result > 0 ? true : false;
        }

    }
}
