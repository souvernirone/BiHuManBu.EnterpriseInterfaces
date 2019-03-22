using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerFunctionRelationRepository : IManagerFunctionRelationRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);


        public bool Add(manager_role_function_relation entity)
        {
            var sql = string.Format(@"INSERT INTO manager_role_function_relation    (role_id,function_code,operator_name,operator_time)  VALUES({0},'{1}','{2}','{3}')", entity.role_id,entity.function_code,entity.operator_name,DateTime.Now);
            return db.Database.ExecuteSqlCommand(sql) > 0;


            //db.manager_role_function_relation.Attach(entity);
            //db.Entry(entity).State = EntityState.Added;
            //return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public bool Delete(int roleId)
        {

            var sql = string.Format(@"DELETE FROM manager_role_function_relation WHERE role_id={0};", roleId);
            return db.Database.ExecuteSqlCommand(sql) > 0;

               



        //var entity = db.manager_role_function_relation.Where(t => t.role_id == roleId).FirstOrDefault();
        //if (entity==null) {
        //    return true;
        //}
        //db.Entry(entity).State = EntityState.Deleted;
        //return db.SaveChanges() > 0;
    }


     public    List<manager_role_function_relation> Get(int roleId) {
            return DataContextFactory.GetDataContext().manager_role_function_relation.Where(x => x.role_id == roleId).ToList();
        }





    }
}
