using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerRoleButtonRelationRepository : EfRepositoryBase<manager_role_button_relation>, IManagerRoleButtonRelationRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();

        public ManagerRoleButtonRelationRepository(DbContext context) : base(context)
        {
        }

        public bool HasBtnAuth(int agentId, string btn_code)
        {
            var result = HasBtnAuth(new List<int> { agentId }, btn_code);
            if (result.Any())
                return result.FirstOrDefault().Result;
            return false;
        }

        public List<AgentIdAndBool> HasBtnAuth(List<int> listAgentId, string btn_code)
        {
            var sql = @"
                SELECT
                    bx_agent.Id As AgentId,
                    IF(COUNT(1)>0,1,0)  AS Result   
                FROM bx_agent
                  LEFT JOIN manager_role_db
                    ON bx_agent.ManagerRoleId = manager_role_db.id
                  LEFT JOIN manager_role_button_relation
                    ON manager_role_db.id = manager_role_button_relation.role_id
                  LEFT JOIN manager_module_button
                    ON manager_module_button.id = manager_role_button_relation.button_id
                WHERE bx_agent.Id in ({0})
                    AND manager_module_button.button_code = ?btn_code
                GROUP BY bx_agent.Id;
                ";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="btn_code",
                    MySqlDbType=MySqlDbType.VarChar,
                    Value=btn_code
                }
            };
            sql = string.Format(sql, string.Join(",", listAgentId));
            return Context.Database.SqlQuery<AgentIdAndBool>(sql, param).ToList();
        }

        public IQueryable<manager_role_button_relation> LoadAll()
        {
            return GetAll();
        }

        /// <summary>
        /// 刷库的方法（给系统管理员、管理员添加 三级菜单按钮的权限）zky 2017-12-21 /crm
        /// </summary>
        /// <returns></returns>
        public int ManagerAddButton()
        {
            int allBtnCount = 0;
            int reviewCount = 0;
            int settingCount = 0;
            try
            {
                using (TransactionScope ts=new TransactionScope())
                {
                    #region 系统管理员添加客户列表下的所有按钮
                    string selectOne = @"SELECT id FROM manager_role_db WHERE id NOT IN (SELECT role_id FROM manager_role_button_relation) AND role_type IN (3)";
                    var roleListOne = db.Database.SqlQuery<int>(selectOne).ToList();

                    //需要添加的按钮
                    var buttonList = db.manager_module_button.Where(t => t.pater_module == "customer_list").ToList();
                    StringBuilder sbOne = new StringBuilder();
                    sbOne.Append("INSERT into manager_role_button_relation (role_id, button_id, module_code, creator_name, creator_time) VALUES ");
                    foreach (var roleId in roleListOne)
                    {
                        string dataStr = string.Format("({0}, {1}, '{2}', 'zky', NOW()),({0}, {3}, '{4}', 'zky', NOW()),({0}, {5}, '{6}', 'zky', NOW()),({0}, {7}, '{8}', 'zky', NOW()),", roleId, buttonList[0].id, buttonList[0].button_code, buttonList[1].id, buttonList[1].button_code, buttonList[2].id, buttonList[2].button_code, buttonList[3].id, buttonList[3].button_code);

                        sbOne.Append(dataStr);
                    }
                    string execute = sbOne.ToString().TrimEnd(',');
                    allBtnCount = db.Database.ExecuteSqlCommand(execute);
                    #endregion

                    #region 有客户列表的(排除系统管理员)添加 录入跟新回访按钮(btn_review)
                    string reviewSql = @"select id from manager_role_db where role_type not in (3) and id in (
                            select role_id from manager_role_module_relation where module_code = 'customer_list')";
                    var idList = db.Database.SqlQuery<int>(reviewSql).ToList();

                    StringBuilder sbSql = new StringBuilder();
                    sbSql.Append("INSERT into manager_role_button_relation (role_id, button_id, module_code, creator_name, creator_time) VALUES ");
                    //需要添加的按钮
                    var reviewBtn = buttonList.Where(t => t.button_code == "btn_review").FirstOrDefault();
                    foreach (var item in idList)
                    {
                        string itemSql = string.Format("({0}, {1}, '{2}', 'zky', NOW()),", item, reviewBtn.id, reviewBtn.button_code);
                        sbSql.Append(itemSql);

                    }
                    string executeSb = sbSql.ToString().TrimEnd(',');
                    reviewCount = db.Database.ExecuteSqlCommand(executeSb);
                    #endregion

                    #region 有分配权限的（排除系统管理员）添加分配回收设置
                    string settingSql = @"select id from manager_role_db where role_type not in (3) and id in (
                            select role_id from manager_role_module_relation where module_code = 'RenewalSetting')";
                    var settingList = db.Database.SqlQuery<int>(settingSql).ToList();

                    StringBuilder sbSet = new StringBuilder();
                    sbSet.Append("INSERT into manager_role_button_relation (role_id, button_id, module_code, creator_name, creator_time) VALUES ");
                    //需要添加的按钮
                    var settingBtn = buttonList.Where(t => t.button_code == "btn_recycle").FirstOrDefault();
                    foreach (var item in settingList)
                    {
                        string dataItem = string.Format("({0}, {1}, '{2}', 'zky', NOW()),", item, settingBtn.id, settingBtn.button_code);
                        sbSql.Append(dataItem);
                    }
                    string executeSet = sbSql.ToString().TrimEnd(',');
                    settingCount = db.Database.ExecuteSqlCommand(executeSet);
                    #endregion

                    ts.Complete();
                }
            }
            catch (System.Exception ex)
            {
                Transaction.Current.Rollback();
            }
            return allBtnCount + reviewCount + settingCount;
        }
    }
}
