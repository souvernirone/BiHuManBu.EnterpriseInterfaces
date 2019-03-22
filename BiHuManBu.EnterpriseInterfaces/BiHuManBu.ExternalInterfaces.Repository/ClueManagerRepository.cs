using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;


namespace BiHuManBu.ExternalInterfaces.Repository
{
  public   class ClueManagerRepository: IClueManagerRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        public bool CluesIsExist(string mobile)
        {
            string sql = string.Format("SELECT id FROM tx_clues  WHERE  mobile={0}   AND TO_DAYS(createtime) = TO_DAYS(NOW())", mobile);
            return _dbHelper.ExecuteScalar(CommandType.Text, sql) != null;
        }



        public int Add(tx_clues model) {

            db.tx_clues.Add(model);
            if (db.SaveChanges() > 0)
            {
                return model.id;
            }
            return 0;
            //string sql = string.Format(@"insert into tx_clues 
            //    (agentid,smsid,licenseno,source,sourcename,casetype,followupstate,accidentremark,smsrecivedtime
            //    ,dangerarea,mobile,ReportCaseNum,ReportCasePeople,HasInsureInfo,CreateTime,UpdateTime,Deleted,MoldName,CarVIN,city_name) values 
            //    ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')",
            //    model.agentid, model.smsid, model.licenseno, model.source, model.sourcename, model.casetype, model.followupstate, model.accidentremark, model.smsrecivedtime.ToString()== "0001-01-01 00:00:00"? Convert.ToDateTime(DBNull.Value): model.smsrecivedtime, model.dangerarea, model.mobile, model.ReportCaseNum, model.ReportCasePeople, model.HasInsureInfo, model.CreateTime, model.UpdateTime, model.Deleted, model.MoldName, model.CarVIN, model.city_name);
            //return db.Database.ExecuteSqlCommand(sql);

            //db.tx_clues.Add(model);
            //return db.SaveChanges() > 0;
        }


        public List<TXPushPeople> GetPushPeople(int topAgentId, int type)
        {
            string sqlStr = string.Empty;
            if (type == 1)
            {
                sqlStr = string.Format(@"SELECT DISTINCT(bx_agent.Id),manager_role_db.role_type RoleType FROM bx_agent
 INNER JOIN manager_role_function_relation  ON bx_agent.ManagerRoleId = manager_role_function_relation.role_id
 INNER JOIN manager_role_db  ON manager_role_db.id = bx_agent.ManagerRoleId
 WHERE function_code = 'reciveclue' AND bx_agent.id  IN(SELECT id FROM bx_agent WHERE bx_agent.TopAgentId ={0})  and role_type not in (3,7)", topAgentId);
            }
            else {
                sqlStr = string.Format(@"SELECT DISTINCT(bx_agent.Id),manager_role_db.role_type RoleType FROM bx_agent
 INNER JOIN manager_role_function_relation  ON bx_agent.ManagerRoleId = manager_role_function_relation.role_id
 INNER JOIN manager_role_db  ON manager_role_db.id = bx_agent.ManagerRoleId
 WHERE function_code = 'reciveclue' AND bx_agent.id  IN(SELECT id FROM bx_agent WHERE bx_agent.TopAgentId ={0})", topAgentId);
            }
            
            return db.Database.SqlQuery<TXPushPeople>(sqlStr).ToList();
        }



        public List<TXPushPeople> GetPushLeader(int topAgentId)
        {
            string sqlStr = string.Format(@"SELECT DISTINCT(bx_agent.Id),manager_role_db.role_type RoleType FROM bx_agent
 INNER JOIN manager_role_function_relation  ON bx_agent.ManagerRoleId = manager_role_function_relation.role_id
 INNER JOIN manager_role_db  ON manager_role_db.id = bx_agent.ManagerRoleId
 WHERE function_code = 'reciveclue' AND bx_agent.id  IN(SELECT id FROM bx_agent WHERE bx_agent.TopAgentId ={0}) and role_type in (3,7)", topAgentId);
            return db.Database.SqlQuery<TXPushPeople>(sqlStr).ToList();
        }




    }
}
