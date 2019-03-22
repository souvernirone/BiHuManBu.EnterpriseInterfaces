using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Models.Dtos;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    /// <summary>
    /// 角色
    /// </summary>
    public class ManagerRoleRepository : IManagerRoleRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");

        private EntityContext db = DataContextFactory.GetDataContext();

        /// <summary>
        /// 如果注册的是顶级经纪人，需要添加相应的角色
        /// 销售顾问 续保顾问 普通  manager
        /// </summary>
        /// <param name="angentId"></param>
        /// <param name="name"></param>
        public List<manager_role_db> AddManagerRole(int agentId, string name, int zhenBangType)
        {
            var items = new List<manager_role_db>()
            {
                new manager_role_db(){
                role_name = "系统管理员",
                role_status = 0,
                creator_name = name,
                creator_time = DateTime.Now,
                creator_full_name = name,
                top_agent_id = agentId,
                modifi_full_name = name,
                modifi_name = name,
                modifi_time = DateTime.Now,
                role_type=3
                }
                ,new manager_role_db(){
                role_name = "管理员",
                role_status = 0,
                creator_name = name,
                creator_time = DateTime.Now,
                creator_full_name = name,
                top_agent_id = agentId,
                modifi_full_name = name,
                modifi_name = name,
                modifi_time = DateTime.Now,
                role_type=4
                }
                ,new manager_role_db(){
                role_name = "普通员工",
                role_status = 0,
                creator_name = name,
                creator_time = DateTime.Now,
                creator_full_name = name,
                top_agent_id = agentId,
                modifi_full_name = name,
                modifi_name = name,
                modifi_time = DateTime.Now,
                role_type=0
                },
                new manager_role_db(){
                role_name = "理赔主管",
                role_status = 0,
                creator_name = name,
                creator_time = DateTime.Now,
                creator_full_name = name,
                top_agent_id = agentId,
                modifi_full_name = name,
                modifi_name = name,
                modifi_time = DateTime.Now,
                role_type=7
                }

                //2017-07-31删除销售顾问、续保顾问角色 新增接待员角色
                //new manager_role_db(){
                //role_name = "销售顾问",
                //role_status = 0,
                //creator_name = name,
                //creator_time = DateTime.Now,
                //creator_full_name = name,
                //top_agent_id = agentId,
                //modifi_full_name = name,
                //modifi_name = name,
                //modifi_time = DateTime.Now,
                //role_type=1
                //},
                //new manager_role_db(){
                //role_name = "续保顾问",
                //role_status = 0,
                //creator_name = name,
                //creator_time = DateTime.Now,
                //creator_full_name = name,
                //top_agent_id = agentId,
                //modifi_full_name = name,
                //modifi_name = name,
                //modifi_time = DateTime.Now,
                //role_type=2
                //},
                ,new manager_role_db(){
                role_name = "助理",
                role_status = 0,
                creator_name = name,
                creator_time = DateTime.Now,
                creator_full_name = name,
                top_agent_id = agentId,
                modifi_full_name = name,
                modifi_name = name,
                modifi_time = DateTime.Now,
                role_type=5
                }
            };
            if (zhenBangType == 1)//正邦机构账号添加 网点角色
            {
                items.Add(new manager_role_db()
                {
                    role_name = "网点",
                    role_status = 0,
                    creator_name = name,
                    creator_time = DateTime.Now,
                    creator_full_name = name,
                    top_agent_id = agentId,
                    modifi_full_name = name,
                    modifi_name = name,
                    modifi_time = DateTime.Now,
                    role_type = 6
                });
            }

            try
            {
                //foreach (var item in items)
                //{
                //    this.db.manager_role_db.Add(item);
                //    this.db.SaveChanges();
                //}
                db.manager_role_db.AddRange(items);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return items;
        }

        public manager_role_db GetRoleInfo(int roleId)
        {
            try
            {
                var role = this.db.manager_role_db.FirstOrDefault(x => x.id == roleId);
                return role;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }


        public string RoleExistByAgentId(int agentId)
        {
            string sqlStr = "SELECT manager_role_db.role_name FROM manager_role_db INNER JOIN bx_agent  ON bx_agent.ManagerRoleId = manager_role_db.id WHERE bx_agent.id =?id";
            try
            {
                MySqlParameter[] param = new MySqlParameter[]
           {
                new MySqlParameter{ParameterName= "id",MySqlDbType=MySqlDbType.Int32,Value=agentId},

           };
                return
                     db.Database.SqlQuery<string>(sqlStr, param).FirstOrDefault();
            }
            catch (Exception ex)
            {

                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return "";
            }



        }

        public List<manager_role_db> GetManagerRoleInfo(int topAgentId)
        {
            try
            {
                return this.db.manager_role_db.Where(x => x.top_agent_id == topAgentId).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 根据角色id得到角色名称
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetRoleName(int roleId)
        {
            string roleName = string.Empty;
            try
            {
                roleName = (this.db.manager_role_db.FirstOrDefault(x => x.id == roleId) ?? new manager_role_db()).role_name;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return roleName;
        }

        /// <summary>
        /// 根据条件筛选数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public manager_role_db Find(Expression<Func<manager_role_db, bool>> predicate)
        {
            return this.db.manager_role_db.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 添加角色 zky 2017-08-03
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool AddRole(manager_role_db role)
        {
            this.db.manager_role_db.Attach(role);
            this.db.Entry(role).State = EntityState.Added;
            return this.db.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新角色信息 zky 2017-08-03
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool UpdateRole(manager_role_db role)
        {
            this.db.manager_role_db.Attach(role);
            this.db.Entry(role).State = EntityState.Modified;
            return this.db.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool DeleteRole(manager_role_db role)
        {
            DataContextFactory.GetDataContext().manager_role_db.Remove(role);
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        public int GetRoleTypeByAgentId(int agentId)
        {
            var roleType = GetRoleTypeByAgentId(new List<int> { agentId });
            if (roleType.Any())
            {
                return roleType.FirstOrDefault().RoleType;
            }
            return 0;
        }

        public List<AgentIdAndRoleTyoeDto> GetRoleTypeByAgentId(List<int> listAgent)
        {
            var sqlBuilder = @"
                SELECT 
                    bx_agent.Id AS AgentId,
                    manager_role_db.role_type AS RoleType
                FROM
                    bx_agent
                        INNER JOIN
                    manager_role_db ON bx_agent.ManagerRoleId = manager_role_db.id
                WHERE
                    bx_agent.id IN ({0});
                ";
            var sql = string.Format(sqlBuilder, string.Join(",", listAgent));
            return db.Database.SqlQuery<AgentIdAndRoleTyoeDto>(sql).ToList();
        }

        /// <summary>
        /// 给顶级代理添加助理角色 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="agentList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddHelperRole(IList<bx_agent> agentList, out int count)
        {
            foreach (var agent in agentList)
            {
                manager_role_db role = new manager_role_db();
                role.role_name = "助理";
                role.role_status = 0;
                role.creator_name = agent.AgentName;
                role.creator_time = DateTime.Now;
                role.creator_full_name = agent.AgentName;
                role.top_agent_id = agent.Id;
                role.modifi_full_name = agent.AgentName;
                role.modifi_name = agent.AgentName;
                role.modifi_time = DateTime.Now;
                role.role_type = 5;
                role.yunYingRole = 2;//表示批量插入的角色
                db.manager_role_db.Add(role);
            }
            count = db.SaveChanges();
            return count == agentList.Count;
        }

        /// <summary>
        /// 根据添加查询角色 zky 2017-09-01 /crm
        /// </summary>
        /// <param name="lamda"></param>
        /// <returns></returns>
        public IQueryable<manager_role_db> GetList(Expression<Func<manager_role_db, bool>> lamda)
        {
            return db.manager_role_db.Where(lamda);
        }

        public int UpdateYunYingRole(int yunYingRole)
        {
            var list = db.manager_role_db.Where(t => t.yunYingRole == yunYingRole).ToList();
            if (list.Count > 0)
            {
                foreach (var role in list)
                {
                    role.yunYingRole = 0;
                    db.manager_role_db.Attach(role);
                    db.Entry(role).State = EntityState.Modified;
                }
            }
            return db.SaveChanges();
        }

        /// <summary>
        /// 获取顶级代理人下面拥有某个三级按钮权限的角色列表
        /// </summary>
        /// <param name="btnCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public IList<string> GetRoleListByBtnCode(string btnCode, int topAgentId)
        {
            string sql = @"select role_Name from manager_role_db where id in (
                            select role_id from manager_role_button_relation where module_code =?btnCode)
                            and top_agent_id = ?topAgentId";
            var result = new List<string>();
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter{ParameterName= "topAgentId",MySqlDbType=MySqlDbType.Int32,Value=topAgentId},
                new MySqlParameter{ParameterName= "btnCode",MySqlDbType=MySqlDbType.VarChar,Value=btnCode}
            };
            result = db.Database.SqlQuery<string>(sql, param).ToList();
            return result;
        }

        /// <summary>
        /// 获取顶级代理人下面拥有某个二级菜单权限的角色列表
        /// </summary>
        /// <param name="btnCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public IList<string> GetRoleListByModule(string moduleCode, int topAgentId)
        {
            string sql = @"select role_Name from manager_role_db where id in (
                            select role_id from manager_role_module_relation where module_code =?moduleCode)
                            and top_agent_id = ?topAgentId";
            var result = new List<string>();
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter{ParameterName= "topAgentId",MySqlDbType=MySqlDbType.Int32,Value=topAgentId},
                new MySqlParameter{ParameterName= "moduleCode",MySqlDbType=MySqlDbType.VarChar,Value=moduleCode}
            };
            result = db.Database.SqlQuery<string>(sql, param).ToList();
            return result;
        }

        /// <summary>
        /// 更新是否代报价
        /// </summary>
        /// <param name="managerRoleId">主键</param>
        /// <param name="isRequote"></param>
        /// <returns></returns>
        public bool UpdateRequoteById(int managerRoleId, int isRequote)
        {
            bool result = false;
            try
            {
                string sql = "update manager_role_db set isRequote=" + isRequote + " where Id=" + managerRoleId;
                return db.Database.ExecuteSqlCommand(sql) > 0;
            }
            catch (Exception ex)
            {
                logError.Error("managerRoleId=" + managerRoleId + ";isRequote=" + isRequote + ";发生异常：" + ex);
            }
            return result;
        }
    }
}
