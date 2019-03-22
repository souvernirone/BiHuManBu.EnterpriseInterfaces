using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class LoginLogRepository: ILoginLogRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Add(bx_login_log entity)
        {
            db.bx_login_log.Attach(entity);
            db.Entry(entity).State = EntityState.Added;
            return db.SaveChanges() > 0;
        }
    }
}
