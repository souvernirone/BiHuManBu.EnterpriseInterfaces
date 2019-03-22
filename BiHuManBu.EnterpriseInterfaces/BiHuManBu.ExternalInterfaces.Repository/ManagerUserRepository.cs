using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Linq;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerUserRepository : IManagerUserRepository
    {
        private ILog logError;
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        private readonly EntityContext _db = DataContextFactory.GetDataContext();
        public ManagerUserRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        public manageruser AddManagerUser(string name, string pwd, string mobile,int peopleType=0)
        {
            try
            {
                manageruser user = new manageruser();
                user.Name = name;
                user.PwdMd5 = pwd;
                user.Mobile = mobile;
                user.AccountType = 1;//经纪人平台账号
                user.CreateTime = DateTime.Now.ToLocalTime().ToString();
                user.OperatorTime = DateTime.Now;
                user.OperatorName = name;
                user.department_id = peopleType;
                DataContextFactory.GetDataContext().manageruser.Add(user);
                DataContextFactory.GetDataContext().SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="user"></param>
        public bool AddManagerUser(manageruser user)
        {
            DataContextFactory.GetDataContext().manageruser.Add(user);
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        public void EditManagerUserRoleId(int id, int roleId)
        {
            try
            {
                var item = DataContextFactory.GetDataContext().manageruser.FirstOrDefault(x => x.ManagerUserId == id);
                item.ManagerRoleId = roleId;
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void EditAgentRoleId(int AgentId, int roleId)
        {
            try
            {
                var item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == AgentId);
                item.ManagerRoleId = roleId;
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string GetUserRoleName(int ManagerRoleId)
        {
            string roleName = string.Empty;
            try
            {
                //var ManagerRoleId = (DataContextFactory.GetDataContext().manageruser.FirstOrDefault(x => x.Name == name && x.PwdMd5 == pwd) ?? new manageruser()).ManagerRoleId;
                //roleId = ManagerRoleId;
                roleName = new ManagerRoleRepository().GetRoleName(ManagerRoleId) ?? "";
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return roleName;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool IsExist(Expression<Func<manageruser, bool>> predicate)
        {
            return DataContextFactory.GetDataContext().manageruser.Any(predicate);
        }

        /// <summary>
        /// 获取单个manageruser对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public manageruser Find(Expression<Func<manageruser, bool>> predicate)
        {
            return DataContextFactory.GetDataContext().manageruser.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Update(manageruser user)
        {
            AttachIfNot(user);

            DataContextFactory.GetDataContext().Entry(user).State = EntityState.Modified;//System.Data.EntityState.Modified;

            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        private void AttachIfNot(manageruser user)
        {
            if (!DataContextFactory.GetDataContext().manageruser.Local.Contains(user))
            {
                DataContextFactory.GetDataContext().manageruser.Attach(user);
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="oldRoleId"></param>
        /// <param name="newRoleId"></param>
        /// <returns></returns>
        public bool UpdateRoleId(int oldRoleId, int newRoleId)
        {
            var sql = " UPDATE manageruser SET ManagerRoleId=?newRoleId WHERE ManagerRoleId=?oldRoleId";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.Double,
                    ParameterName="newRoleId",
                    Value=newRoleId
                },
                new MySqlParameter
                {
                    MySqlDbType=MySqlDbType.Double,
                    ParameterName="oldRoleId",
                    Value=oldRoleId
                }
            };
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql, param) > 0;
        }

        /// <summary>
        /// 批量更新manageruser的managerRoleId  zky 2017-08-31 /crm
        /// </summary>
        /// <param name="names">名称</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool UpdateRoleIdByName(List<string> names, int roleId)
        {
            string sql = "UPDATE manageruser set managerRoleId=?roleId where name in (?names)";
            MySqlParameter[] param = new MySqlParameter[] {
                new MySqlParameter
                {
                    MySqlDbType =MySqlDbType.Int32,
                    ParameterName="roleId",
                    Value=roleId
                },
                new MySqlParameter
                {
                    MySqlDbType =MySqlDbType.VarChar,
                    ParameterName="names",
                    Value=string.Join(",",names.ToArray())
                }
            };

            return _dbHelper.ExecuteNonQuery(sql, param) == names.Count;

        }

        /// <summary>
        /// 用户列表查询 用户列表查询 zky 2017-08-31 /运营后台
        /// </summary>
        /// <param name="accountType">0运营后台用户，1CRM用户</param>
        /// <param name="mobile">手机号</param>
        /// <param name="account">用户名</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<ManageruserViewModel> GetManageruserList(int accountType, string mobile, string account, int pageSize, int pageIndex, out int total)
        {

            string sqlWhere = "where manageruser.AccountType=0  ";
            if (!string.IsNullOrEmpty(account))
            {
                sqlWhere += "and Name like '%{0}%'";
                sqlWhere = string.Format(sqlWhere, account);

            }
            if (!string.IsNullOrEmpty(mobile))
            {
                sqlWhere += "and Mobile ='{0}'";
                sqlWhere = string.Format(sqlWhere, mobile);

            }

            string querySql = @"SELECT
            `ManagerUserId`,
            `Name`,
            `PwdMd5`,
            `Mobile`,
            `Remarks`,
            `OperatorId`,
            `OperatorName`,
            `OperatorTime`,
            `ManagerRoleId`,
            `CreateTime`,
            manager_role_db.role_name as ManagerRoleName
            FROM manageruser
            INNER JOIN manager_role_db ON manageruser.ManagerRoleId=manager_role_db.id  {2}
                        GROUP BY manageruser.ManagerUserId
            LIMIT {0},{1}";

            string countSql = @"SELECT count(*)as num
                 FROM manageruser   INNER JOIN manager_role_db ON manageruser.ManagerRoleId=manager_role_db.id  {0} ";
            total = int.Parse(_dbHelper.ExecuteScalar(string.Format(countSql, sqlWhere)).ToString());

            var query =
                _dbHelper.ExecuteDataTable(string.Format(querySql, (pageIndex - 1) * pageSize, pageSize, sqlWhere));
            return query.ToList<ManageruserViewModel>();
        }
    }
}
