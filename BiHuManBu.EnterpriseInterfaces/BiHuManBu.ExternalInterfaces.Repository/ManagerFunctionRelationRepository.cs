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

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerFunctionRepository : IManagerFunctionRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

       public  List<manager_function_db> GetByParentCode(string parentCode)
        {


            return  DataContextFactory.GetDataContext().manager_function_db.Where(x=>x.function_parent_code==parentCode).ToList();
        }





    }
}
