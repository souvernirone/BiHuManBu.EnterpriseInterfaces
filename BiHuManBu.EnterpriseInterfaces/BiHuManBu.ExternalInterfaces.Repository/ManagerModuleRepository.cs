using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerModuleRepository : IManagerModuleRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");

        private EntityContext db = DataContextFactory.GetDataContext();

        public List<manager_module_db> GetManagerModuleAll()
        {
            List<manager_module_db> listItem = new List<manager_module_db>();
            try
            {
                listItem = DataContextFactory.GetDataContext().manager_module_db.Where(x => x.module_status == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return listItem;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paterCode"></param>
        /// <returns></returns>
        public List<string> GetManagerModuleCodes(string paterCode,int type)
        {
            List<string> codes = new List<string>();
            try
            {
                codes = DataContextFactory.GetDataContext().manager_module_db.Where(x => x.pater_code == paterCode && x.module_type == type).Distinct().Select(x => x.module_code).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return codes;
        }

        /// <summary>
        /// 查询所有的菜单 zky 2017-08-04
        /// </summary>
        /// <returns></returns>
        public IList<manager_module_db> GetAllModule()
        {
            List<manager_module_db> listItem = new List<manager_module_db>();
            try
            {
                listItem = DataContextFactory.GetDataContext().manager_module_db.ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return listItem;
        }

        public IQueryable<manager_module_db> GetList(Expression<Func<manager_module_db, bool>> whereLamda)
        {
            return db.manager_module_db.Where(whereLamda);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Add(manager_module_db entity)
        {
            db.manager_module_db.Attach(entity);
            db.Entry(entity).State = EntityState.Added;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(manager_module_db entity)
        {
            db.manager_module_db.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public bool Delete(string moduleCode)
        {
            var entity = db.manager_module_db.Where(t => t.module_code == moduleCode).FirstOrDefault();
            db.Entry(entity).State = EntityState.Deleted;
            return db.SaveChanges() > 0;
        }
    }
}
