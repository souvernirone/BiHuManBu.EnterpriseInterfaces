using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using log4net;
using MySql.Data.MySqlClient;
using ServiceStack.Text;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using System.Threading.Tasks;
using System.Text;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ConsumerDetailRepository : IConsumerDetailRepository
    {
        private ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);


         private ILog logError = LogManager.GetLogger("ERROR");

        public bx_consumer_review Find(int id)
        {
            bx_consumer_review model = new bx_consumer_review();
            try
            {
                model = DataContextFactory.GetDataContext().bx_consumer_review.Where(i => i.id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
        public int AddDetail(bx_consumer_review bxWorkOrderDetail)
        {
            int workOrderDetailId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_consumer_review.Add(bxWorkOrderDetail);
                DataContextFactory.GetDataContext().SaveChanges();
                workOrderDetailId = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderDetailId = 0;
            }
            return workOrderDetailId;
        }
        public int UpdateDetail(bx_consumer_review bxWorkOrderDetail)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_consumer_review.AddOrUpdate(bxWorkOrderDetail);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public List<bx_consumer_review> FindDetails(long buid)
        {
            List<bx_consumer_review> list = new List<bx_consumer_review>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_consumer_review.Where(x => x.b_uid == buid).OrderByDescending(o => o.create_time).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        /// <summary>
        /// 查找最后一条已出单记录
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bx_consumer_review FindNewClosedOrder(long buid, int status = 1)
        {
            bx_consumer_review model = new bx_consumer_review();
            try
            {
                model = DataContextFactory.GetDataContext().bx_consumer_review.Where(x => x.b_uid == buid && x.result_status == status).OrderByDescending(o => o.id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
        /// <summary>
        /// 获取所有的未读续保记录，未排序，消息列表用
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<bx_consumer_review> FindNoReadList(int agentId, out int total)
        {
            List<bx_consumer_review> list = new List<bx_consumer_review>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_consumer_review.Where(i => i.operatorId == agentId && i.read_status == 0 && i.next_review_date.HasValue && i.next_review_date.Value <= DateTime.Now).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }

        public int AddCrmSteps(bx_crm_steps bxCrmSteps)
        {
            try
            {
                DataContextFactory.GetDataContext().bx_crm_steps.Add(bxCrmSteps);
                return DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return 0;
            }

        }

        public int UpdateCrmSteps(bx_crm_steps bxCrmSteps)
        {
            try
            {
                var model = DataContextFactory.GetDataContext().bx_crm_steps.Where(n => n.id == bxCrmSteps.id).FirstOrDefault();
                model.json_content = bxCrmSteps.json_content;
                model.create_time = DateTime.Now;
                return DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bxCrmSteps"></param>
        /// <returns></returns>
        public async Task<int> AddCrmStepsAsync(bx_crm_steps bxCrmSteps)
        {
            try
            {
                DataContextFactory.GetDataContext().bx_crm_steps.Add(bxCrmSteps);
                return DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return 0;
            }

        }

        public async Task<bool> InsertBySqlAsync(List<bx_crm_steps> list)
        {
            var listSql = GenerateInsertSql(list);
            if (listSql.Count == 0)
                return true;
            // 执行sql
            foreach (var item in listSql)
            {
                await DataContextFactory.GetDataContext().Database.ExecuteSqlCommandAsync(item);
            }
            return true;
        }

        /// <summary>
        /// 生成插入的sql
        /// 因为插入的数据量大超过了max_allowed_packet，所以这里分批生成sql
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<string> GenerateInsertSql(List<bx_crm_steps> list, int scope = 2000)
        {
            var listSql = new List<string>();


            var listCount = list.Count;
            var loopCount = Math.Ceiling(listCount * 1.0 / scope);
            var now = DateTime.Now.ToString();
            for (int i = 0; i < loopCount; i++)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("INSERT INTO bx_crm_steps  (agent_id,b_uid,type,create_time,json_content) VALUES ");
                for (int j = i * scope; j < ((i + 1) * scope) && j < listCount; j++)
                {
                    builder.Append(string.Format("({0},{1},{2},'{3}','{4}'),", list[j].agent_id.ToString(), list[j].b_uid.ToString(), list[j].type.ToString(), list[j].create_time.ToString(), list[j].json_content.ToString()));
                }
                builder.Remove(builder.Length - 1, 1);
                listSql.Add(builder.ToString());
            }

            return listSql;
        }


        public bool AddCrmSteps(List<bx_crm_steps> listStep)
        {
            var listSql = GenerateInsertSql(listStep);
            if (listSql.Count == 0)
                return true;
            foreach (var item in listSql)
            {
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommandAsync(item);
            }
            return true;
        }
        /// <summary>
        /// 清空回收站数据加入到记录表
        /// </summary>
        /// <param name="isTest"></param>
        /// <param name="agentId">当前登录人AgentId</param>
        /// <returns></returns>
        public bool ClearRecycleBinAddSteps( int isTest, string strAgents)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO bx_crm_steps (json_content,agent_id,create_time,`type`,b_uid) ");
            sqlBuilder.Append(" SELECT '' AS json_content, Agent AS agent_id,NOW() AS create_time,5 AS `type`,id AS b_uid FROM bx_userinfo ");
            sqlBuilder.Append(" WHERE istest=@IsTest AND Agent in (" + strAgents + ") ");
            var sqlParams = new List<MySqlParameter>()
            {
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "IsTest",
                    Value = isTest
                }
            };
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sqlBuilder.ToString(), sqlParams.ToArray())>0;
             
        }
        /// <summary>
        /// 批量添加到步骤表
        /// </summary>
        /// <param name="strBuids"></param>
        /// <param name="IsTest"></param>
        /// <returns></returns>
        public bool BatchAddCrmStepsByBuid(string strBuids, int IsTest)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO bx_crm_steps (json_content,agent_id,create_time,`type`,b_uid) ");
            sqlBuilder.Append(" SELECT '' AS json_content, Agent AS agent_id,NOW() AS create_time,5 AS `type`,id AS b_uid FROM bx_userinfo ");
            sqlBuilder.Append(" WHERE istest=@IsTest AND Id in (" + strBuids + ") ");
            var sqlParams = new List<MySqlParameter>()
            {
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "IsTest",
                    Value = IsTest
                }
            };
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sqlBuilder.ToString(), sqlParams.ToArray()) > 0;
        }
        /// <summary>
        /// 批量添加到步骤表
        /// </summary>
        /// <param name="strBuids"></param>
        /// <param name="IsTest"></param>
        /// <returns></returns>
        public bool BatchAddCrmStepsByBuid(string strBuids,long agentId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("INSERT INTO bx_crm_steps (json_content,agent_id,create_time,`type`,b_uid) ");
            sqlBuilder.Append(" SELECT '' AS json_content, " + agentId + " AS agent_id,NOW() AS create_time,5 AS `type`,id AS b_uid FROM bx_userinfo ");
            sqlBuilder.Append(" WHERE Id in (" + strBuids + ") ");
            
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sqlBuilder.ToString()) > 0;
        }
        public List<bx_crm_steps> GetCrmStepsList(long buid)
        {
            try
            {
                string sql = string.Format("SELECT * FROM bx_crm_steps where b_uid = {0} order by id desc limit 0,100;", buid);
                return DataContextFactory.GetDataContext().Database.SqlQuery<bx_crm_steps>(sql).ToList();
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;

        }


      



        public string GetTopAgent(int agentId)
        {
            string getChildAgentId = string.Format("select getAgentTopParent({0});", agentId);
            string resultId = _dbHelper.ExecuteScalar(getChildAgentId).ToString();
            return resultId;
        }

        public bx_sms_account GetBxSmsAccount(int agentid)
        {
            return DataContextFactory.GetDataContext().bx_sms_account.FirstOrDefault(c => c.agent_id == agentid);
        }

        public void InsetBxSmsAccount(bx_sms_account bxSmsAccount)
        {
            DataContextFactory.GetDataContext().bx_sms_account.Add(bxSmsAccount);
            DataContextFactory.GetDataContext().SaveChanges();
        }

        public bx_quotereq_carinfo GetBxQuotereqCarinfo(long buid)
        {
            return DataContextFactory.GetDataContext().bx_quotereq_carinfo.FirstOrDefault(c => c.b_uid == buid);
        }

        public bx_quoteresult GetQuoteresult(long buid)
        {
            return DataContextFactory.GetDataContext().bx_quoteresult.FirstOrDefault(c => c.B_Uid == buid);
        }

        public bx_userinfo GetBxUserinfo(long buid)
        {
            return DataContextFactory.GetDataContext().bx_userinfo.FirstOrDefault(c => c.Id == buid);
        }

        public bx_lastinfo GetLastinfo(long buid)
        {
            return DataContextFactory.GetDataContext().bx_lastinfo.FirstOrDefault(c => c.b_uid == buid);
        }

        public void SaveNewQuoteInfo(RequestNewQuoteInfoViewModel requestNew)
        {
            //bx_quotereq_carinfo bxQuotereqCarinfo = DataContextFactory.GetDataContext().bx_quotereq_carinfo.FirstOrDefault(c => c.b_uid == requestNew.Buid);
            //bx_quoteresult bxQuoteresult = DataContextFactory.GetDataContext().bx_quoteresult.FirstOrDefault(c => c.B_Uid == requestNew.Buid);
            //if (bxQuotereqCarinfo != null)
            //{
            //    bxQuotereqCarinfo.force_start_date = requestNew.ForceStartDate;
            //    bxQuotereqCarinfo.biz_start_date = requestNew.BizStartDate;

            //    if (bxQuoteresult!=null)
            //    {
            //        bxQuoteresult.ForceStartDate = requestNew.ForceStartDate;
            //        bxQuoteresult.BizStartDate = requestNew.BizStartDate;
            //        DataContextFactory.GetDataContext().bx_quoteresult.AddOrUpdate(bxQuoteresult);
            //    }


            //    DataContextFactory.GetDataContext().bx_quotereq_carinfo.AddOrUpdate(bxQuotereqCarinfo);

            //}
            bx_userinfo bxUserinfo = DataContextFactory.GetDataContext().bx_userinfo.FirstOrDefault(c => c.Id == requestNew.Buid);
            if (bxUserinfo != null)
            {
                bxUserinfo.InsuredAddress = requestNew.InsuredAddress;
                bxUserinfo.InsuredIdCard = requestNew.InsuredIdCard;
                bxUserinfo.InsuredIdType = requestNew.InsuredIdType;
                bxUserinfo.InsuredMobile = requestNew.InsuredMobile;
                bxUserinfo.InsuredName = requestNew.InsuredName;
                bxUserinfo.Email = requestNew.Email;
                bxUserinfo.UpdateTime = DateTime.Now;
            }
            //bx_lastinfo bxLastinfo = DataContextFactory.GetDataContext().bx_lastinfo.FirstOrDefault(c => c.b_uid == requestNew.Buid);
            //if (bxLastinfo != null)
            //{
            //    bxLastinfo.last_year_claimtimes = requestNew.LastYearAcctimes;
            //    bxLastinfo.last_year_claimamount = requestNew.LastYearClaimamount;
            //}
            DataContextFactory.GetDataContext().SaveChanges();
        }


        public IEnumerable<bx_agent> OldNoManagerRoleBxAgents()
        {
            var result = DataContextFactory.GetDataContext().bx_agent.Where(c => c.ParentAgent == 0 && c.AgentAccount != null).ToList();
            return result;
        }

        public IEnumerable<manager_role_db> ManagerRoleDbs()
        {
            return DataContextFactory.GetDataContext().manager_role_db.Where(c => c.role_type != null).ToList();
        }

        public IEnumerable<manageruser> GetManagerusers(List<string> agentaccountList)
        {
            return DataContextFactory.GetDataContext().manageruser.WhereIn(c => c.Name, agentaccountList.ToArray()).ToList();
        }

        public manager_role_db GetManagerRoleDb(int topAgentId)
        {
            return DataContextFactory.GetDataContext().manager_role_db.FirstOrDefault(c => c.top_agent_id == topAgentId && c.role_type == 4);
        }

        public int AddRole(manager_role_db managerRole, List<manager_role_module_relation> managerRoleModuleRelation)
        {
            manager_role_db managerRoleDb = DataContextFactory.GetDataContext().manager_role_db.Add(managerRole);
            managerRoleModuleRelation.ForEach(c => c.role_id = managerRoleDb.id);
            foreach (var roleModuleRelation in managerRoleModuleRelation)
            {
                DataContextFactory.GetDataContext().manager_role_module_relation.Add(roleModuleRelation);
            }
            return DataContextFactory.GetDataContext().SaveChanges();
        }



        public int InsertManagerRoleModuleRelation(manager_role_db managerRole)
        {
            string sql = string.Format(@"   INSERT INTO manager_role_module_relation(
	role_id, 
	module_code, 
	creator_name, 
	creator_full_name, 
	creator_time) VALUES( {0} ,'addQuotedPrice','{1}','{2}','{3}')", managerRole.id, managerRole.creator_name, managerRole.creator_full_name, DateTime.Now);

            return _dbHelper.ExecuteNonQuery(sql);
        }

        public int AddConsumerRole(List<manager_role_module_relation> managerRoleModuleRelations, int roleid)
        {
            List<manager_role_module_relation> deleteRoleModuleRelation = DataContextFactory.GetDataContext().manager_role_module_relation.Where(c => c.role_id == roleid && (c.module_code == "batchRenewal_list" || c.module_code == "xubao_module" || c.module_code == "renewal_list" || c.module_code == "xubao_notice" || c.module_code == "RenewalSetting" || c.module_code == "notice_share" || c.module_code == "insure_module" || c.module_code == "addQuotedPrice" || c.module_code == "baojiadan" || c.module_code == "quote_list" || c.module_code == "appoinment_list" || c.module_code == "QuotationReceipt_List" || c.module_code == "huifang_notice" || c.module_code == "customer_list" || c.module_code == "customer_module" || c.module_code == "customer_checklist")).ToList();


            foreach (var managerRoleModuleRelation in deleteRoleModuleRelation)
            {
                DataContextFactory.GetDataContext().manager_role_module_relation.Remove(managerRoleModuleRelation);
            }

            foreach (var roleModuleRelation in managerRoleModuleRelations)
            {
                DataContextFactory.GetDataContext().manager_role_module_relation.Add(roleModuleRelation);
            }
            return DataContextFactory.GetDataContext().SaveChanges();
        }

        /// <summary>
        /// 获取未处理预约单数
        /// </summary>
        /// <param name="agents"></param>
        /// <returns></returns>
        public int GetAppoinmentInfoNum(string agents)
        {
            string sql = string.Format(@"SELECT count(1)
	                    FROM 
	                    bx_car_order  where order_status in (0,1,2,3,4,-3,-4) and cur_agent in  ({0})", agents);

            //sql = string.Concat(" select count(*) from (", sql, ") as t ");
            object obj = _dbHelper.ExecuteScalar(sql);
            if (obj == null || obj == Convert.DBNull)
            {
                return 0;
            }
            return int.Parse(_dbHelper.ExecuteScalar(sql).ToString());

        }


        public List<manager_module_db> GetBaseModuleDbs()
        {
            List<manager_module_db> listmodule =
                DataContextFactory.GetDataContext().manager_module_db.Where(c => c.module_status == 1).ToList();
            return listmodule;

        }

        public List<IsNewCarViewModel> GetIsNewCar(List<long> buid)
        {
            return (from quote in DataContextFactory.GetDataContext().bx_quotereq_carinfo
                    where buid.Contains(quote.b_uid)
                    select new IsNewCarViewModel
                    {
                        Buid = quote.b_uid,
                        IsNewCar = quote.is_newcar
                    })
                        .ToList();
        }
        /// <summary>
        /// 根据buid获取保额信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bx_savequote GetSaveQuoteByBuid(long buid)
        {
            bx_savequote model = new bx_savequote();
            try
            {
                model = DataContextFactory.GetDataContext().bx_savequote.Where<bx_savequote>(a => a.B_Uid == buid).ToList<bx_savequote>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logError.Error("根据buid获取保额信息发生异常：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }

        /// <summary>
        /// 获取保费信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public async Task<List<bx_quoteresult>> GetQuoteResultListByBuid(long buid)
        {
            List<bx_quoteresult> quoteResList = new List<bx_quoteresult>();
            try
            {
                //quoteResList = DataContextFactory.GetDataContext().bx_quoteresult.Where<bx_quoteresult>(a => a.B_Uid == buid).ToList<bx_quoteresult>();
                quoteResList = await DataContextFactory.GetDataContext().bx_quoteresult.Where<bx_quoteresult>(a => a.B_Uid == buid).ToListAsync<bx_quoteresult>();
            }
            catch (Exception ex)
            {
                _logError.Error("根据buid获取保费信息发生异常：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return quoteResList;
        }

        public bool AddCrmStepsOfCamera(string sql)
        {
            bool result = false;
            try
            {
                result= DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql) > 0;
            }
            catch (Exception ex)
            {
                _logError.Error("SQL脚本："+sql+";插入跟进记录表发生异常:"+ex);
            }
            return result;
        }
    }
}
