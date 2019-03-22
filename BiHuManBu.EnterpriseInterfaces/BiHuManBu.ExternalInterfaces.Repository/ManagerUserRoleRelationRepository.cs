using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySql.Data.MySqlClient;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerUserRoleRelationRepository : EfRepositoryBase<manager_user_role_relation>, IManagerUserRoleRelationRepository
    {
        public ManagerUserRoleRelationRepository(DbContext context) : base(context)
        {

        }

        /// <summary>
        /// 添加manager_user_role_relation，由于id被定义为double，并且是自增的，ef在插入时有问题，所以写sql插入
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public bool Add(manager_user_role_relation relation)
        {
            var sql = @"
delete  from manager_user_role_relation where user_name=?user_name;
INSERT INTO manager_user_role_relation (user_name,role_id,creator_name,creator_full_name,creator_time) VALUES (?user_name,?role_id,?creator_name,?creator_full_name,?creator_time);";

            #region 参数
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.VarChar,
                    ParameterName="user_name",
                    Value=relation.user_name
                },
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.Double,
                    ParameterName="role_id",
                    Value=relation.role_id
                },
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.VarChar,
                    ParameterName="creator_name",
                    Value=relation.creator_name
                },
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.VarChar,
                    ParameterName="creator_full_name",
                    Value=relation.creator_full_name
                },
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.DateTime,
                    ParameterName="creator_time",
                    Value=relation.creator_time
                }
            };
            #endregion

            return  Context.Database.ExecuteSqlCommand(sql,param)>0;
        }
    }
}
