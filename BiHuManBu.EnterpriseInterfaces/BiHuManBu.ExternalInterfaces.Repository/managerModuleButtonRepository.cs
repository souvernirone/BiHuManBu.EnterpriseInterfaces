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
    public class ManagerModuleButtonRepository : IManagerModuleButtonRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        public IQueryable<manager_module_button> LoadAll()
        {
            return db.manager_module_button.AsQueryable();
        }

        
      



        /// <summary>
        /// 查询角色拥有的按钮权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<manager_module_button> GetRoleButtonList(int roleId)
        {
            string sql = @"SELECT
                            btn.id,
	                        btn.button_name, 
	                        btn.pater_module,
	                        btn.action_url,
	                        btn.button_code
                        FROM
                            manager_module_button btn
                        INNER JOIN manager_role_button_relation relation
                        on btn.id = relation.button_id
                        where role_id =?roleId";

            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter(){ParameterName="roleId",Value=roleId,MySqlDbType=MySqlDbType.Int32 }
            };

            var query = _dbHelper.ExecuteDataTable(sql, param).ToList<manager_module_button>();
            return query;
        }
    }
}
