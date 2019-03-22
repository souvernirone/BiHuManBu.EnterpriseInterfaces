using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Transactions;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Data;
using System.Text.RegularExpressions;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentRepository : IAgentRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        private ILog logError;
        private ILog logInfo;
        private readonly MySqlHelper _sqlhelper;
        private EntityContext db = DataContextFactory.GetDataContext();

        private readonly string _messageCenterHost = ConfigurationManager.AppSettings["SendMessage"];

        //增城人保的顶级id
        private string zcTopAgentId = ConfigurationManager.AppSettings["autoOpenUsedId"].ToString();
        public AgentRepository()
        {
            _sqlhelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
        }

        private static string DbConfigString
        {
            get { return ConfigurationManager.ConnectionStrings["zb"].ConnectionString; }
        }

        public bx_agent GetAgent(int agentId)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == agentId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        public bx_agent GetAgentByAgentAccount(string agentAccount)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.AgentAccount == agentAccount);

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 根据代理Id获取姓名
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public string GetAgentName(int agentId)
        {
            string agentName = string.Empty;
            try
            {
                agentName = (from c in DataContextFactory.GetDataContext().bx_agent
                             where c.Id == agentId
                             select c.AgentName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agentName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="curAgentId">当前代理人是下级代理人</param>
        /// <returns></returns>
        public List<AgentIdAndAgentName> GetAllAgentName(List<int> agentIds, int curAgentId, int agentLevel)
        {
            var result = new List<AgentIdAndAgentName>();
            try
            {
                string ids = string.Join(",", agentIds);
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT Id AS AgentId,AgentName,ParentAgent,IsUsed FROM bx_agent WHERE Id IN (" + ids + ") and isused=1");
                sqlBuilder.Append(" UNION ");
                sqlBuilder.Append(" SELECT Id AS AgentId,CONCAT(AgentName,'（已禁）'),ParentAgent,IsUsed  FROM bx_agent WHERE isused>1 AND Id in(" + ids + ")");
                sqlBuilder.Append(" UNION ");
                //if (agentLevel == 1)
                //{
                //    sqlBuilder.Append(" SELECT AgentId ,CONCAT(agent_name,'（已删）') AS AgentName," + curAgentId + " AS ParentAgent,3 AS IsUsed FROM bx_delete_agent_log  ");
                //    sqlBuilder.Append(" WHERE delete_userid IN(SELECT id FROM bx_agent WHERE topagentid=" + curAgentId + ")");
                //}
                //else
                //{
                //    sqlBuilder.Append(" SELECT AgentId ,CONCAT(agent_name,'（已删）') AS AgentName," + curAgentId + " AS ParentAgent,3 AS IsUsed FROM bx_delete_agent_log  ");
                //    sqlBuilder.Append(" WHERE delete_userid IN(SELECT id FROM bx_agent WHERE parentagent=" + curAgentId + " OR bx_agent.id = " + curAgentId + ") ");
                //}

                //string sql = "select Id as AgentId,AgentName,ParentAgent from bx_agent where Id in(" + ids + ")";
                result = DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sqlBuilder.ToString()).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
        public List<AgentIdAndAgentName> GetAgentName(List<int> agentIds)
        {
            var result = new List<AgentIdAndAgentName>();
            try
            {
                string ids = string.Join(",", agentIds);
                string sql = "select Id as AgentId,AgentName,ParentAgent,IsUsed from bx_agent where Id in(" + ids + ")";
                result = DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sql).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        public bx_agent GetAgent(string openid)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.First(x => x.OpenId == openid);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 获取代理的顶级ID
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        public string GetTopAgentId(int currentAgent)
        {
            MySqlParameter[] parms = new MySqlParameter[1];
            var parentAgentid = new MySqlParameter("currentAgentId", currentAgent);
            parentAgentid.MySqlDbType = MySqlDbType.Int32;
            parentAgentid.Size = 128;
            parms[0] = parentAgentid;

            var sql = "SELECT topagentid FROM bx_agent WHERE id=?currentAgentId ";

            try
            {
                var ts = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, parms).FirstOrDefault();
                return ts.ToString();
            }
            catch
            {
                LogHelper.Error("AgentRepository->GetTopAgentId：代理人Id=" + currentAgent.ToString() + "的topagentid有问题");
                return null;
            }
        }

        public List<int> GetTopAgentIdList(string agentIds)
        {
            string sql = string.Format(@"SELECT TopAgentId FROM bx_agent WHERE id IN ({0}) ", agentIds);

            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql).ToList();
        }

        public List<AgentAndTopAgent> GetTopAgentByIds(string agentIds)
        {
            string sql = string.Format(@"SELECT Id,TopAgentId FROM bx_agent WHERE id IN ({0}) ", agentIds);

            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentAndTopAgent>(sql).ToList();
        }

        /// <summary>
        /// 获取代理人下所有的子集，这个不用了，用鹏洁union的那个
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        [Obsolete("这个方法不用了，使用鹏洁写的GetSonsList方法")]
        public string GetSonId(int currentAgent)
        {
            MySqlParameter[] parms = new MySqlParameter[1];
            var parentAgentid = new MySqlParameter("parentAgentid", currentAgent);
            parentAgentid.MySqlDbType = MySqlDbType.Int32;
            parentAgentid.Size = 128;
            parms[0] = parentAgentid;
            var ts = DataContextFactory.GetDataContext().Database.SqlQuery<string>("select `getAgentChildList`(@parentAgentid)", parms).FirstOrDefault();
            return ts;
        }

        /// <summary>
        /// 获取当前代理人的所有下级代理人
        /// </summary>
        /// <param name="currentAgent">当前代理人</param>
        /// <param name="hasSelf">是否包含自己</param>
        /// <returns></returns>
        public List<string> GetSonsList(int currentAgent, bool hasSelf = true)
        {

            var listAgents = new List<string>();
            try
            {
                var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "curAgent",
                        Value = currentAgent
                    }
                };
                #region SQL语句
                var strSql = new StringBuilder();
                if (hasSelf)
                {
                    strSql.Append("select SQL_CACHE  ?curAgent ")
                        .Append(" union all ");
                }
                strSql.Append(" select id from  bx_agent where parentagent=?curAgent ")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent
                            where parentagent in (select id from bx_agent where parentagent=?curAgent ) ")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent where parentagent in (
                            select id from bx_agent where parentagent in (
                            select id from  bx_agent where parentagent=?curAgent) ) ");
                //#第五级代理
                //union  all
                //select id from  bx_agent
                //where  parentagent in (
                //select id from bx_agent 
                //where  parentagent in (
                //select id from bx_agent
                //where  parentagent in (
                //select id from  bx_agent where  parentagent=@curAgent
                //) )
                //)
                #endregion
                //查询列表
                List<int> intList = DataContextFactory.GetDataContext()
                    .Database.SqlQuery<int>(strSql.ToString(), parameters.ToArray()).ToList();
                listAgents = intList.ConvertAll<string>(x => x.ToString());
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<string>();
        }

        public List<string> GetSonsList(int curAgentId, string curAgentName, bool hasSelf = true)
        {
            var listAgents = new List<string>();
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@curAgent", MySqlDbType.Int32),
                    new MySqlParameter("@agentName", MySqlDbType.VarChar)
                };
                parameters[0].Value = curAgentId;
                parameters[1].Value = curAgentName;
                #region SQL语句
                var strSql = new StringBuilder();
                if (hasSelf)
                {
                    strSql.Append("select SQL_CACHE id from  bx_agent where agentName=@agentName and id=@curAgent")
                        .Append(" union all ");
                }
                strSql.Append(" select id from bx_agent where parentagent=@curAgent and agentName=@agentName")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent
                            where parentagent in (select id from bx_agent where parentagent=@curAgent ) and agentName=@agentName")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent where parentagent in (
                            select id from bx_agent where parentagent in (
                            select id from bx_agent where parentagent=@curAgent)) and agentName=@agentName");
                //#第五级代理
                //union  all
                //select id from  bx_agent
                //where  parentagent in (
                //select id from bx_agent 
                //where  parentagent in (
                //select id from bx_agent
                //where  parentagent in (
                //select id from  bx_agent where  parentagent=@curAgent
                //) )
                //)
                #endregion
                //查询列表
                List<int> intList = DataContextFactory.GetDataContext()
                    .Database.SqlQuery<int>(strSql.ToString(), parameters.ToArray()).ToList();
                listAgents = intList.ConvertAll<string>(x => x.ToString());
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<string>();
        }

        public List<bx_agent> GetAgentByAgentAccount(string agentAccount, string pwd)
        {
            var item = new List<bx_agent>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.AgentAccount == agentAccount && (x.AgentPassWord == pwd || x.AgentPassWord == null)).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }


        public AagentGroupAuthen GetAgentItemByAgentAccount(string agentAccount)
        {
            var item = new AagentGroupAuthen();
            try
            {
                //modify by qidakang:增加is_complete_task、authen_state字段。微信每天首次登陆会检查是否实名认证等。
                string sql = " SELECT a.*,b.is_complete_task AS IsCompleteTask,b.authen_state AS AuthenState,b.TestState FROM bx_agent a  LEFT JOIN bx_group_authen b ON a.Id=b.agentId WHERE a.agentAccount=@agentAccount";
                var sqlParams = new List<MySqlParameter>(){
                    new MySqlParameter(){
                        MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "agentAccount",
                    Value = agentAccount
                    }
                };
                var bxAgent = DataContextFactory.GetDataContext().Database.SqlQuery<AagentGroupAuthen>(sql, sqlParams.ToArray()).ToList();
                //var bxAgent = DataContextFactory.GetDataContext().bx_agent.Where(x => x.AgentAccount == agentAccount).ToList();
                bxAgent.ForEach(x =>
                {
                    if (agentAccount.CompareTo(x.AgentAccount) == 0)
                    {
                        item = x;
                    }
                });
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 添加经纪人信息
        /// </summary>
        public bool AddAgent(string agentName, string mobile, int agentType, string region, string name, string pwd, int isDaiLi, int shareCode, int regType, string address, bool isUsed, int agentLevel, out bx_agent agentItem, int ManagerRoleId, int commodity, int platfrom, int repeatQuote, int accoutType, DateTime? endDate, int openQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, int zhenBangType, int peopleType)
        {
            bool result = false;
            agentItem = new bx_agent();
            try
            {
                bx_agent agent = new bx_agent();
                agent.IsUsed = isUsed ? 1 : 0;
                agent.AgentName = agentName;
                agent.AgentAccount = name;
                agent.AgentPassWord = pwd;
                agent.IsDaiLi = isDaiLi;
                agent.AgentType = agentType;
                agent.Mobile = mobile;
                agent.Region = region;
                agent.BatchRenewalTotalCount = 50;
                agent.BatchRenewalFrequency = 10;
                agent.RegType = regType;
                agent.AgentAddress = address;
                agent.IsShow = 1;
                if (peopleType == 1)
                {
                    agent.MessagePayType = 0;
                }
                else
                {
                    agent.MessagePayType = 1;
                }

                agent.IsQuote = 1;
                agent.IsSubmit = 1;
                agent.ManagerRoleId = ManagerRoleId;
                agent.agent_level = agentLevel;
                //1是顶级，0不是
                if (isDaiLi == 0)
                {
                    agent.ParentAgent = shareCode - 1000;
                }
                agent.CreateTime = DateTime.Now;
                agent.repeat_quote = repeatQuote;
                agent.openQuote = openQuote;
                agent.accountType = accoutType;
                agent.endDate = endDate;
                agent.loginType = loginType;
                agent.robotCount = robotCount;
                agent.agentBrand = brand;
                agent.contractEndDate = contractEnd;
                agent.quoteCompany = quoteCompany;
                agent.picc_account = addRenBao;
                agent.hide_phone = hidePhone;
                agent.zhen_bang_type = zhenBangType;

                DataContextFactory.GetDataContext().bx_agent.Add(agent);
                DataContextFactory.GetDataContext().SaveChanges();

                agent.OpenId = agent.Id.ToString().GetMd5();
                if (agent.IsDaiLi == 1)
                {
                    agent.EffectiveCallDuration = 10;
                    agent.SecretKey = agent.OpenId.Substring(0, 11);
                    agent.TopAgentId = agent.Id;
                    agent.agent_level = 1;

                    if (agent.accountType == 1)
                    {
                        agent.BatchRenewalTotalCount = 1000;
                    }
                    else
                    {
                        //新增顶级测试账号
                        agent.IsSubmit = 2;
                        agent.BatchRenewalTotalCount = 100;
                    }
                }
                else
                {
                    agent.TopAgentId = int.Parse(GetTopAgentId(agent.ParentAgent));
                }
                agent.ShareCode = (agent.Id + 1000).ToString();

                agent.commodity = commodity;
                agent.platform = platfrom;

                DataContextFactory.GetDataContext().SaveChanges();

                var itemsms = DataContextFactory.GetDataContext().bx_sms_account.FirstOrDefault(x => x.agent_id == agent.Id);
                if (itemsms == null)
                {
                    bx_sms_account sms = new bx_sms_account();
                    sms.agent_id = agent.Id;
                    sms.sms_account = "bihu-" + agent.Id.ToString();
                    sms.sms_password = agent.OpenId;
                    if (agent.IsDaiLi == 1)
                    {
                        sms.total_times = 100;
                        sms.avail_times = 100;
                    }
                    else
                    {
                        sms.total_times = 0;
                        sms.avail_times = 0;
                    }
                    sms.status = 1;
                    sms.create_time = DateTime.Now;
                    DataContextFactory.GetDataContext().bx_sms_account.Add(sms);
                    DataContextFactory.GetDataContext().SaveChanges();
                }
                result = true;
                agentItem = agent;
            }
            catch (Exception ex)
            {
                result = false;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        public bool IsExistAgentInfo(int id)
        {
            bool result = true;
            try
            {
                int[] arrAgentLevel = { 1, 2, 3 };
                return DataContextFactory.GetDataContext().bx_agent.Any(x => x.Id == id && arrAgentLevel.Contains(x.agent_level));

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 是否存在指定角色的代理人
        /// </summary>
        /// <param name="managerRoleId"></param>
        /// <returns></returns>
        public bool IsExistManagerRoleId(int managerRoleId)
        {
            var sql = "SELECT 1 FROM bx_agent WHERE ManagerRoleId=?ManagerRoleId LIMIT 1 ";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="ManagerRoleId",
                    Value =managerRoleId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            var result = DataContextFactory.GetDataContext().Database.SqlQuery<string>(sql, param);
            return result != null;
        }

        /// <summary>
        /// 修改 Agent ShareCode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bx_agent EditAgent(int agentId)
        {
            bx_agent item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == agentId);
                if (item != null)
                {
                    if (item.IsDaiLi == 1)
                    {
                        item.SecretKey = item.Id.ToString().GetMd5();
                    }
                    item.ShareCode = (item.Id + 1000).ToString();
                    DataContextFactory.GetDataContext().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }


        /// <summary>
        /// 经纪人父级名称
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        public string GetParentAgentName(int parentAgent)
        {
            string name = null;
            try
            {
                var item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == parentAgent);
                if (item != null)
                {
                    name = item.AgentName;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return name;
        }

        /// <summary>
        /// 获取顶级经纪人Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int GetAgentTopParent(int parentId)
        {
            try
            {
                var query = DataContextFactory.GetDataContext().Database.SqlQuery<string>("select getAgentTopParent({0})", parentId).FirstOrDefault();
                return Convert.ToInt32(query);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return -1;
        }

        /// <summary>
        /// 获取顶级代理人信息   陈亮   2017-08-03  
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent GetTopAgent(int agentId)
        {
            try
            {
                var sql = @"SELECT 
                                top.*
                            FROM 
                                bx_agent AS son INNER JOIN bx_agent AS top
                                ON son.topagentid = top.id
                            WHERE 
                                son.id = ?agentId";

                var param = new MySqlParameter[]
                {
                    new MySqlParameter
                    {
                        ParameterName="agentId",
                        Value=agentId,
                        MySqlDbType=MySqlDbType.Int32
                    }
                };
                return DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent>(sql, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 修改经纪人 isUsed 修改经纪人的角色id
        /// </summary>
        /// <param name="agentId">经纪人id</param>
        /// <param name="isUsed">审核状态 0:待审核； 1:可用； 2:禁用</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool EditAgentAndManagerUserRoleId(int agentId, int isUsed, int roleId)
        {
            var result = false;
            try
            {
                using (var scope = new TransactionScope())
                {
                    var agentItem = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == agentId);
                    agentItem.IsUsed = isUsed;
                    agentItem.ManagerRoleId = roleId;
                    var user = DataContextFactory.GetDataContext().manageruser.FirstOrDefault(x => x.Name == agentItem.AgentAccount);
                    if (user != null)
                    {
                        user.ManagerRoleId = roleId;
                    }
                    DataContextFactory.GetDataContext().SaveChanges();
                    scope.Complete();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 经纪人列表信息
        /// </summary>
        /// <param name="topAgentId">顶级经纪人id</param>
        /// <param name="isUsed">审核状态 0:待审核； 1:可用； 2:禁用</param>
        /// <param name="search">搜索参数 名字或电话</param>
        /// <param name="pageSize">每页显示的条数</param>
        /// <param name="pageNum">当前页</param>
        /// <returns></returns>
        public List<bx_agent> QueryAgentInfo(int topAgentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum)
        {
            totalNum = 0;
            try
            {
                var resultDefault = (from a in DataContextFactory.GetDataContext().bx_agent
                                     where a.Id != topAgentId && a.TopAgentId == topAgentId && a.IsUsed != 3
                                     select new
                                     {
                                         Id = a.Id,
                                         IsUsed = a.IsUsed,
                                         AgentName = a.AgentName,
                                         Mobile = a.Mobile,
                                         CreateTime = a.CreateTime,
                                         AgentAccount = a.AgentAccount,
                                         AgentPassWord = a.AgentPassWord,
                                         ManagerRoleId = a.ManagerRoleId
                                     }).ToList();
                var result = resultDefault.Select(x => new bx_agent()
                {
                    Id = x.Id,
                    IsUsed = x.IsUsed,
                    AgentName = x.AgentName,
                    Mobile = x.Mobile,
                    CreateTime = x.CreateTime,
                    AgentAccount = x.AgentAccount,
                    AgentPassWord = x.AgentPassWord,
                    ManagerRoleId = x.ManagerRoleId
                }).ToList();
                if (isUsed.HasValue)
                {
                    result = result.Where(x => x.IsUsed == isUsed).ToList();
                }
                if (!string.IsNullOrEmpty(search))
                {
                    result = result.Where(x => (x.AgentName != null && x.AgentName.Contains(search))
                        || (x.Mobile != null && x.Mobile.Contains(search))).ToList();
                }
                totalNum = result.Count();

                return result.OrderByDescending(x => x.CreateTime).Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        public List<bx_agent> QueryAgentInfo(int topAgentId, int agentId, int? isUsed, string search, int pageSize, int pageNum, out int totalNum)
        {
            totalNum = 0;
            try
            {
                var result = db.bx_agent.Where(a => ((agentId == 0 || (a.Id != agentId && a.ParentAgent == agentId)) && (agentId != 0 || (a.Id != a.TopAgentId && a.TopAgentId == topAgentId)) && a.IsUsed != 3 && (string.IsNullOrEmpty(search) || ((!string.IsNullOrEmpty(a.AgentName) && a.AgentName.Contains(search)) || (!string.IsNullOrEmpty(a.Mobile) && a.Mobile.Contains(search)))) && (!isUsed.HasValue || a.IsUsed == isUsed)));

                totalNum = result.ToList().Count;
                return result.OrderByDescending(x => x.CreateTime).Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 加入企业
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ShareCode"></param>
        /// <returns></returns>
        public bool CopyAgentInfoAdd(int id, int ShareCode)
        {
            var result = false;
            try
            {
                var parentItem = GetAgent(ShareCode - 1000);
                var agentItem = GetAgent(id);
                if (agentItem.ParentAgent == parentItem.Id)
                {
                    return false;
                }
                bx_agent item = new bx_agent();
                #region
                item.IsUsed = 0;
                item.AgentName = agentItem.AgentName;
                item.AgentAccount = agentItem.AgentAccount;
                item.AgentPassWord = agentItem.AgentPassWord;
                item.IsDaiLi = agentItem.IsDaiLi;
                item.AgentType = agentItem.AgentType;
                item.Mobile = agentItem.Mobile;
                item.Region = agentItem.Region;
                item.ParentAgent = parentItem.Id;
                //item.OpenId = agentItem.OpenId;
                item.IsBigAgent = agentItem.IsBigAgent;
                item.FlagId = agentItem.FlagId;
                item.ParentRate = agentItem.ParentRate;
                item.AgentRate = agentItem.AgentRate;
                item.ReviewRate = agentItem.ReviewRate;
                item.PayType = agentItem.PayType;
                item.AgentGetPay = agentItem.AgentGetPay;
                item.CommissionType = agentItem.CommissionType;
                item.ParentShareCode = agentItem.ParentShareCode;
                item.IsGenJin = agentItem.IsGenJin;
                item.IsShow = agentItem.IsShow;
                item.IsShowCalc = agentItem.IsShowCalc;
                item.SecretKey = agentItem.SecretKey;
                item.IsLiPei = agentItem.IsLiPei;
                item.MessagePayType = agentItem.MessagePayType;
                item.CreateTime = DateTime.Now;
                #endregion
                DataContextFactory.GetDataContext().bx_agent.Add(item);
                if (DataContextFactory.GetDataContext().SaveChanges() > 0)
                {
                    result = true;
                    item.OpenId = item.Id.ToString().GetMd5();
                    item.ShareCode = (item.Id + 1000).ToString();
                    DataContextFactory.GetDataContext().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = false;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 修改 代理人为顶级代理人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sonAgentIds">这里的代理人不包含自己</param>
        /// <returns></returns>
        public int EditAgentChangeTopAgent(int id, List<string> sonAgentIds)
        {
            var result = -1;
            try
            {
                var agentItem = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == id);
                if (agentItem != null)
                {
                    var user = DataContextFactory.GetDataContext().manageruser.FirstOrDefault(x => x.Name == agentItem.AgentAccount);
                    if (user != null)
                    {
                        #region
                        agentItem.IsDaiLi = 1;
                        agentItem.ParentAgent = 0;
                        agentItem.TopAgentId = agentItem.Id;
                        agentItem.agent_level = 1;
                        agentItem.platform = 7;//*****这里要是增加了平台的话还的修改*****

                        if (DataContextFactory.GetDataContext().SaveChanges() > 0)
                        {
                            #region 创建相应的角色信息     角色跟经纪人关系
                            var role = new ManagerRoleRepository().AddManagerRole(agentItem.Id, agentItem.AgentAccount, agentItem.zhen_bang_type);
                            var roleId = role.Where(x => x.role_type == 3).Select(x => x.id).FirstOrDefault();
                            if (user != null)
                            {
                                new ManagerUserRepository().EditManagerUserRoleId(user.ManagerUserId, roleId);
                                new ManagerUserRepository().EditAgentRoleId(agentItem.Id, roleId);
                            }
                            //角色跟模块关系
                            new ManagerRoleModuleRelationRepository().AddRoleModuleRelation(role, agentItem.AgentAccount, 0, 1);
                            #endregion 创建相应的角色信息     角色跟经纪人关系

                            #region 修改下级代理人
                            if (sonAgentIds.Count > 0)
                            {
                                var roleIdDefault = role.Where(x => x.role_type == 0).Select(x => x.id).FirstOrDefault();

                                var arrayAgent = Array.ConvertAll(sonAgentIds.ToArray(), s => int.Parse(s));

                                var childAgent = DataContextFactory.GetDataContext().bx_agent.Where(x => arrayAgent.Contains(x.Id)).ToList();
                                var agentAccountList = childAgent.Select(o => o.AgentAccount);
                                var managerUserList = DataContextFactory.GetDataContext().manageruser.Where(m => agentAccountList.Contains(m.Name)).ToList();
                                childAgent.ForEach(agent =>
                                {
                                    agent.ManagerRoleId = roleIdDefault;
                                    agent.TopAgentId = id;
                                    agent.agent_level = agent.agent_level - 1;
                                    var userDefalut = managerUserList.FirstOrDefault(u => u.Name == agent.AgentAccount);
                                    if (userDefalut != null)
                                    {
                                        userDefalut.ManagerRoleId = roleIdDefault;
                                    }
                                });

                                if (DataContextFactory.GetDataContext().SaveChanges() > 0)
                                {
                                    result = 1;
                                }
                                else
                                {
                                    result = 0;
                                }
                            }
                            else
                            {
                                result = 1;
                            }
                            #endregion 修改下级代理人
                        }
                        else
                        {
                            result = 0;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result = -2;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 获取可分发的人
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="listSonAgent"></param>
        /// <returns></returns>
        public List<bx_agent> GetAgentDistributedInfo(int parentId, List<int> listSonAgent)
        {
            try
            {
                var result = (from d in DataContextFactory.GetDataContext().bx_agent_distributed
                              join a in DataContextFactory.GetDataContext().bx_agent on d.AgentId equals a.Id
                              where d.ParentAgentId == parentId && !d.Deteled && d.AgentType == 0 && a.IsUsed == 1
                              select a
                           ).ToList<bx_agent>();
                if (result.Count <= 0)
                {
                    //var childAgent = GetSonId(parentId).Split(',');
                    //var output = Array.ConvertAll<string, int>(childAgent, delegate(string s) { return int.Parse(s); }).ToList().Where(x => x != parentId).ToList();
                    var output = listSonAgent.Where(x => x != parentId).ToList();
                    result = (from a in DataContextFactory.GetDataContext().bx_agent
                              where output.Contains(a.Id) && a.IsUsed == 1
                              select a
                                ).ToList<bx_agent>();
                }
                logError.Info(result.Count);
                return result;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }


        public int GetAgentIsSue(int agentId)
        {
            try
            {
                var item = DataContextFactory.GetDataContext().bx_agent_distributed.FirstOrDefault(x => x.AgentId == agentId && !x.Deteled && x.AgentType == 1);
                if (item != null)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return 0;
        }

        #region 集团报表初始化业务,目前已经不用了
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="topAgentId"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public int UpdateAgenrReport(string topAgentId, DateTime startTime, DateTime endTime)
        //{
        //    try
        //    {
        //        TimeSpan ts = endTime - startTime;
        //        var days = ts.Days + 1;
        //        //得到顶级经纪人
        //        var sql = string.Format(@"select * from bx_agent where regtype=0 and isdaili=1 and parentAgent=0 and id in ({0})", topAgentId);
        //        var topAgentInfo = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent>(sql).ToList();
        //        int defaultRegion = -1;
        //        int quoteSuccessNum = 0;
        //        int quoteFailNum = 0;
        //        int notQuoteNum = 0;
        //        #region
        //        topAgentInfo.ForEach(t =>
        //        {
        //            for (int i = 0; i < days; i++)
        //            {
        //                var time = Convert.ToDateTime(startTime.AddDays(i).ToString("yyyy-MM-dd"));
        //                var isCheckSql = DataContextFactory.GetDataContext().bx_agent_report.FirstOrDefault(x => x.topAgentId == t.Id && x.time == time);
        //                if (isCheckSql == null)
        //                {
        //                    #region
        //                    bx_agent_report item = new bx_agent_report();
        //                    item.topAgentId = t.Id;
        //                    item.time = time;
        //                    item.quoteAllNum = GetQuoteCalculation(t.Id, out quoteSuccessNum, out quoteFailNum, out notQuoteNum, time);
        //                    item.quoteSuccessNum = quoteSuccessNum;
        //                    item.quoteFailNum = quoteFailNum;
        //                    item.notQuoteNum = notQuoteNum;
        //                    item.underwritingSuccess = 0;
        //                    item.underwritingFail = 0;
        //                    item.singleVolume = 0;
        //                    item.region = Int32.TryParse(t.Region, out defaultRegion) ? GetRegion(defaultRegion) : t.Region;
        //                    item.createTime = DateTime.Now;
        //                    DataContextFactory.GetDataContext().bx_agent_report.Add(item);
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region
        //                    isCheckSql.topAgentId = t.Id;
        //                    isCheckSql.time = time;
        //                    isCheckSql.quoteAllNum = GetQuoteCalculation(t.Id, out quoteSuccessNum, out quoteFailNum, out notQuoteNum, time);
        //                    isCheckSql.quoteSuccessNum = quoteSuccessNum;
        //                    isCheckSql.quoteFailNum = quoteFailNum;
        //                    isCheckSql.notQuoteNum = notQuoteNum;
        //                    isCheckSql.underwritingSuccess = 0;
        //                    isCheckSql.underwritingFail = 0;
        //                    isCheckSql.singleVolume = 0;
        //                    isCheckSql.region = Int32.TryParse(t.Region, out defaultRegion) ? GetRegion(defaultRegion) : t.Region;
        //                    isCheckSql.createTime = DateTime.Now;
        //                    DataContextFactory.GetDataContext().SaveChanges();
        //                    #endregion
        //                }
        //            }
        //        });
        //        #endregion
        //        DataContextFactory.GetDataContext().SaveChanges();
        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //    }
        //    return 0;
        //} 
        #endregion


        public bool UpdateCallMoblie(int topagent, int agent, string mobile, int isUsed)
        {
            string sqlSelect = "select isUsed from  tx_call_phonenum_setting where agentId=" + agent;
            string sqlAdd = string.Format(";insert into tx_call_phonenum_setting (topagentId,agentId,mobile,isUsed,createtime,updatetime) values ({0},{1},{2},{3},{4},{5})", topagent, agent, mobile, isUsed, "NOW()", "NOW()");
            string sqlUpdate1 = string.Format("update tx_call_phonenum_setting set isUsed={3},mobile='{2}',updatetime=NOW() where topagentId={0} and agentId={1}", topagent, agent, mobile, isUsed, "NOW()", "NOW()");
            string sqlUpdate2 = string.Format("update tx_call_phonenum_setting  set isUsed=0 where topagentId={0} and isUsed=1;", topagent);
            Object obj = _dbHelper.ExecuteScalar(sqlSelect);
            if (obj != null)
            {

                if (isUsed == 1)
                {
                    _dbHelper.ExecuteNonQuery(sqlUpdate2 + sqlUpdate1);
                }
                else
                {
                    _dbHelper.ExecuteNonQuery(sqlUpdate1);

                }




            }
            else
            {
                _dbHelper.ExecuteNonQuery(sqlAdd);
            }


            return true;

        }



        /// <summary>
        /// 更新agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public bool UpdateAgent(bx_agent agent)
        {
            AttachIfNot(agent);

            DataContextFactory.GetDataContext().Entry(agent).State = EntityState.Modified;//System.Data.EntityState.Modified;
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }
        public bool UpdateIsRequoteByAgentId(int agentId, int isRequote)
        {
            string strSql = "update bx_agent_setting set isRequote=?isRequote where agent_id=?agent_id";
            var param = new MySqlParameter[]
                            {
                    new MySqlParameter
                    {
                        ParameterName="agentId",
                        Value=agentId,
                        MySqlDbType=MySqlDbType.Int32
                    },new MySqlParameter
                    {
                        ParameterName="isRequote",
                        Value=isRequote,
                        MySqlDbType=MySqlDbType.Int32
                    }
                            };
            return _dbHelper.ExecuteNonQuery(strSql, param) > 0;

        }
        private void AttachIfNot(bx_agent agent)
        {
            if (!DataContextFactory.GetDataContext().bx_agent.Local.Contains(agent))
            {
                DataContextFactory.GetDataContext().bx_agent.Attach(agent);
            }
        }

        public string GetRegion(int region)
        {
            var sql = string.Format(@"SELECT city_name FROM bx_city WHERE id={0}", region);
            return DataContextFactory.GetDataContext().Database.SqlQuery<string>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 得到报价的 试算量(报价总量)、报价成功量、报价失败量
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="quoteSuccessNum"></param>
        /// <param name="quoteFailNum"></param>
        /// <param name="notQuoteNum"></param>
        /// <param name="sTime"></param>
        /// <param name="agentIds">格式：'102','103','168'</param>
        /// <returns></returns>
        public int GetQuoteCalculation(int agentId, out int quoteSuccessNum, out int quoteFailNum, out int notQuoteNum, DateTime sTime, string agentIds)
        {
            quoteSuccessNum = 0;
            quoteFailNum = 0;
            notQuoteNum = 0;
            try
            {
                var startTime = Convert.ToDateTime(sTime.ToString("yyyy-MM-dd 00:00:00"));
                var endTime = Convert.ToDateTime(sTime.ToString("yyyy-MM-dd 23:59:59"));
                var childAgent = agentIds;// GetSonId(agentId);

                var allSql = string.Format(@"select count(*) as num from bx_userinfo where agent in ({0}) and UpdateTime >='{1}' and UpdateTime<='{2}'", childAgent, startTime, endTime);
                var successSql = string.Format(@"select count(*) as num from bx_userinfo where agent in ({0}) and QuoteStatus>0 and UpdateTime >='{1}' and UpdateTime<='{2}'", childAgent, startTime, endTime);
                var failSql = string.Format(@"select count(*) as num from bx_userinfo where agent in ({0}) and QuoteStatus=0 and UpdateTime >='{1}' and UpdateTime<='{2}'", childAgent, startTime, endTime);
                var notSql = string.Format(@"select count(*) as num from bx_userinfo where agent in ({0}) and QuoteStatus=-1 and UpdateTime >='{1}' and UpdateTime<='{2}'", childAgent, startTime, endTime);
                quoteSuccessNum = DataContextFactory.GetDataContext().Database.SqlQuery<int>(successSql).FirstOrDefault();
                quoteFailNum = DataContextFactory.GetDataContext().Database.SqlQuery<int>(failSql).FirstOrDefault();
                notQuoteNum = DataContextFactory.GetDataContext().Database.SqlQuery<int>(notSql).FirstOrDefault();
                return DataContextFactory.GetDataContext().Database.SqlQuery<int>(allSql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Error("错误信息:" + ex.Message + "/n" + ex.Source + "/n" + ex.StackTrace + "/n" + ex.InnerException);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public string GetToken(int agentid, string uniqueIdentifier)
        {
            try
            {
                //取bx_agent_token对应的值
                string sqlAgentToken = string.Format("select * from bx_agent_token where agentid={0} and uniqueIdentifier='{1}' limit 1", agentid, uniqueIdentifier);
                bx_agent_token agent_token = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent_token>(sqlAgentToken).FirstOrDefault();
                //取bx_agent对象
                string sqlAgent = string.Format("select * from bx_agent where id={0} limit 1", agentid);
                bx_agent agent = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent>(sqlAgent).FirstOrDefault();
                //看bx_agent是否可用
                int used = agent != null ? agent.IsUsed ?? 0 : 0;
                if (agent_token == null || agent == null)
                {
                    return string.Empty;
                }
                return agent_token.token + "," + used;
            }
            catch (Exception ex)
            {
                logError.Error("错误信息:" + ex.Message + "/n" + ex.Source + "/n" + ex.StackTrace + "/n" + ex.InnerException);
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取某个agent所属的 某个顶级代理人下面的所有经纪人
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<string> GetAllChildrenByAgentId(int agentId)
        {
            var querySql = "select  getAgentOfTopParentOfAllChildren(@childagentid)";
            List<MySqlParameter> mainParameters = new List<MySqlParameter>
            {
                new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "@childagentid",
                    Value = agentId
                }
            };
            var returnstr = DataContextFactory.GetDataContext().Database.SqlQuery<string>(querySql, mainParameters.ToArray()).FirstOrDefault();

            List<string> agentlist = returnstr.ToString().Split(',').ToList();

            return agentlist;
        }
        public IList<bx_agent> FindAgentChildrenList(List<string> agentlist, string blurname)
        {
            var querySql = string.Empty;
            if (!string.IsNullOrEmpty(blurname))
            {
                agentlist.Remove(agentlist.Find(string.IsNullOrEmpty));
                querySql = "select * from  bx_agent  where agentname like concat('%',@blurname,'%') and id in (" + string.Join(",", agentlist) + ")";
            }
            else
            {
                querySql = "select * from  bx_agent  where id in (" + string.Join(",", agentlist) + ")";
            }

            List<MySqlParameter> mainParameters = new List<MySqlParameter>
            {
                new MySqlParameter()
                {
                    MySqlDbType = MySqlDbType.String,
                    ParameterName = "@blurname",
                    Value = blurname
                }
            };
            var list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent>(querySql, mainParameters.ToArray()).ToList();
            return list;
        }
        /// <summary>
        /// 添加经纪人信息
        /// </summary>
        public int AddAgentInfo(string agentName, int sourcce, out bx_agent agentItem)
        {
            int result = 0;
            agentItem = new bx_agent();
            try
            {
                //var item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.AgentName == agentName);
                //if (item == null)
                //{
                bx_agent agent = new bx_agent();
                agent.IsBigAgent = sourcce;
                agent.IsUsed = 1;
                agent.AgentName = agentName;
                agent.IsDaiLi = 1;
                agent.AgentType = 0;
                agent.BatchRenewalTotalCount = 50;
                agent.BatchRenewalFrequency = 10;
                agent.CreateTime = DateTime.Now;
                agent.ParentAgent = 0;
                agent.ManagerRoleId = 0;
                DataContextFactory.GetDataContext().bx_agent.Add(agent);
                DataContextFactory.GetDataContext().SaveChanges();
                agent.OpenId = agent.Id.ToString().GetMd5();
                if (agent.IsDaiLi == 1)
                {
                    agent.SecretKey = agent.OpenId.Substring(0, 11);
                    agent.BatchRenewalTotalCount = 1000;
                }
                agent.ShareCode = (agent.Id + 1000).ToString();
                DataContextFactory.GetDataContext().SaveChanges();
                result = 1;
                agentItem = agent;
                //}
                //else
                //{
                //    result = 2;
                //}
            }
            catch (Exception ex)
            {
                result = 0;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 软删除顶级代理人
        /// 将对应的顶级代理人的IsUsed设置为2，没有处理manageruser表中的数据
        /// 这里没有修改下级代理人、客户数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        private bool SoftDeleteTopAgentId(int topAgentId)
        {
            var sql = "UPDATE bx_agent SET IsUsed=3 WHERE agent_level=1 AND id=?id";

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="id",
                    MySqlDbType=MySqlDbType.Int32,
                    Value=topAgentId
                }
            };

            return _dbHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 删除代理人信息 （只删除没有下级代理人的信息）
        /// 新增顶级的逻辑删除功能
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <returns>0:报错；1:成功；2:失败；3:删除顶级代理人时，顶级代理人已经是删除状态</returns>
        public int DelAgentInfo(int agentId, string agentName, int deleteUserId, string deleteAccount, int deletePaltform)
        {
            var result = 0;
            try
            {
                //判断是否是顶级
                var isTopAgentSql = "select Id,AgentAccount,ParentAgent,IsUsed from bx_agent where id=" + agentId;
                var minAgent = _dbHelper.ExecuteObject<MinAgent>(isTopAgentSql);
                if (minAgent != null && minAgent.ParentAgent == 0)
                {
                    if (minAgent.IsUsed == 3)
                        return 3;
                    //顶级逻辑删除
                    return SoftDeleteTopAgentId(agentId) ? 1 : 2;
                }

                //判断是否有下级代理人
                string isHasChlidSql = "SELECT (CASE WHEN EXISTS(SELECT id  FROM bx_agent  WHERE parentagent = " + agentId + ") THEN 1 ELSE 0 END) AS result";
                var isHasChlid = _dbHelper.ExecuteScalar<int>(isHasChlidSql);

                if (isHasChlid == 1)
                {//有下级代理人
                    return 2;
                }
                //判断代理人下是否有Ukey
                string isHasUkeySql = "select count(1) from bx_agent_ukey where agent_id=" + agentId;
                int ukeyCount = _dbHelper.ExecuteScalar<int>(isHasUkeySql);
                if (ukeyCount > 0)
                {
                    return 3;
                }

                var agentAccount = minAgent.AgentAccount;

                var deleteAgentId = ConfigurationManager.AppSettings["crmMultipleDelete"];

                //获取agentId的客户数据得到buid
                UserInfoRepository userInfoRepository = new UserInfoRepository();
                List<long> listBuid = userInfoRepository.GetListBuId(agentId.ToString());

                var connStr = _dbHelper.ConnectionString;
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlTransaction trans = conn.BeginTransaction();
                    MySqlCommand cmd = conn.CreateCommand();
                    try
                    {
                        //删除代理人
                        var delAgent = string.Format("DELETE FROM bx_agent WHERE id={0}", agentId);
                        db.Database.ExecuteSqlCommand(delAgent);

                        //添加删除代理人日志
                        int addCount = AddDeleteAgentLog(agentId, agentName, deleteUserId, deleteAccount, deletePaltform);

                        //删除manageruser
                        if (!string.IsNullOrEmpty(agentAccount))
                        {
                            string delManagerUser = string.Format("DELETE FROM MANAGERUSER WHERE Name='{0}'", agentAccount);
                            db.Database.ExecuteSqlCommand(delManagerUser);
                        }

                        //删除bx_sms_account
                        var delSmsAccount = string.Format("DELETE FROM bx_sms_account WHERE AGENT_ID={0}", agentId);
                        db.Database.ExecuteSqlCommand(delSmsAccount);

                        //删除客户数据（放入我们的账户下面）   插入步骤表
                        if (listBuid.Count() > 0)
                        {
                            var buids = string.Join(",", listBuid);
                            //删除客户数据（放入我们的账户下面）
                            var updateUserInfo = string.Format("update bx_userinfo set agent={0} where id in ({1})", deleteAgentId, buids);
                            db.Database.ExecuteSqlCommand(updateUserInfo);

                            //插入步骤表
                            var insertStep = GenerateInsertStepSql(agentId, buids);
                            db.Database.ExecuteSqlCommand(insertStep);

                            //更新分配表的数据为删除状态
                            var deleteDistributed = string.Format("update bx_agent_distributed set deteled=1 where agentId={0}", agentId);
                            db.Database.ExecuteSqlCommand(deleteDistributed);
                        }

                        //提交修改
                        trans.Commit();
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        private string GenerateInsertStepSql(int agent, string buIds)
        {
            //if (listBuid.Count == 0)
            //    return null;
            //StringBuilder sb = new StringBuilder();


            //sb.Append("INSERT INTO bx_crm_steps (json_content,agent_id,create_time,TYPE,b_uid) VALUES ");
            //foreach (var item in listBuid)
            //{
            //    sb.Append(" ('{\"OldAgentId\":" + agent + "}'," + agent + ",now(),5," + item + "),");
            //}
            //sb.Remove(sb.Length - 1, 1);


            //return sb.ToString();

            return "INSERT INTO bx_crm_steps (json_content,agent_id,create_time,TYPE,b_uid) SELECT '{\"OldAgentId\":" + agent + "}'," + agent + ",NOW(),5,id FROM bx_userinfo WHERE id IN(" + buIds + ")";
        }

        public List<bx_agent> FindList()
        {
            var items = DataContextFactory.GetDataContext().bx_agent.ToList();
            return items;
        }

        public bool HasAgent(string mobile, string shareCode)
        {
            return DataContextFactory.GetDataContext().bx_agent.Where(x => x.ParentShareCode == shareCode && x.Mobile == mobile).Any();
        }

        public List<DefeatAnalysis> GetDefeatAnalytics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime)
        {
            string sql = @"use bihu_analytics;SELECT SUM(count) as Count, DataInTime
                            FROM Defeat_analytics
                            WHERE agentid IN (" +
                         string.Join(",", agentIds) + ")AND DATAINTIME BETWEEN '" + startTime + "' AND '" + endTime + "' GROUP BY DataInTime ";

            logInfo.InfoFormat("获取业务统计请求sql ：{0}", sql);
            var defeatanalysis = _sqlhelper.ExecuteDataTable(sql).ToList<DefeatAnalysis>().ToList();
            return defeatanalysis;
        }

        public List<DefeatAnalysis> GetReasonAnalytics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime)
        {
            string sql = @"use bihu_analytics;select * from(SELECT DefeatReason ,DefeatReasonid,sum(Count) as Count
                            FROM Defeat_analytics
                            WHERE agentid IN (" +
                         string.Join(",", agentIds) + ")AND DATAINTIME BETWEEN '" + startTime + "' AND '" + endTime + "' GROUP BY DefeatReasonId) t order by Count desc";

            logInfo.InfoFormat("获取业务统计请求sql ：{0}", sql);
            var defeatanalysis = _sqlhelper.ExecuteDataTable(sql).ToList<DefeatAnalysis>().ToList();
            return defeatanalysis;
        }

        /// <summary>
        /// 获取业务员战败数据统计
        /// </summary>
        /// <param name="request"></param>
        /// <param name="agentIds"></param>
        /// <param name="defeatAnalysis"></param>
        /// <returns></returns>
        public string GetAgentAnalytics(AgentAnalyticsRequest request, IList<string> agentIds, ref string defeatAnalysis)
        {
            //查询已启用的战败状态
            string topAgentId = GetTopAgentId((int.Parse(agentIds.FirstOrDefault())));
            string sqlUsedDefeat = "SELECT id as DefeatReasonID,DefeatReason FROM BX_DEFEATREASONSETTING DS  WHERE DS.AGENTID IN (" + topAgentId + ") AND DS.DELETED=0";
            var defeatAnalysisList = _sqlhelper.ExecuteDataTable(sqlUsedDefeat).ToList<DefeatAnalysis>().ToList();
            int orderBy;
            if (request.OrderBy.ToLower() != "defeatcount")
                if (!int.TryParse(request.OrderBy.ToLower().Replace("reason_", ""), out orderBy) || orderBy > defeatAnalysisList.Count)
                    request.OrderBy = "agentId";
            List<object> list = new List<object>();
            for (int i = 0; i < defeatAnalysisList.Count; i++)
            {
                var key = "reason_" + i;
                var value = defeatAnalysisList[i].DefeatReason;
                list.Add(new
                {
                    key,
                    value
                });
            }
            defeatAnalysis = JsonConvert.SerializeObject(list);
            string format = @"select bx_agent.agentname,bx_agent.id as agentid,c.* from bx_agent left join (
                                select agentid as cagentid,sum({9}) as 'defeatcount',{0}
                                    from (select a.agentname, agentid, {1}
	                                    from (select sum(Count) as count, agentid, agentname, defeatreasonid, defeatreason
		                                    from bihu_analytics.defeat_analytics 
                                        where dataintime between '{7}' and '{8}' 
		                                    group by agentid, defeatreasonid
		                                    ) a
	                                    ) b
                                    group by agentid
                                )c on c.cagentid=bx_agent.id where bx_agent.id in ({2}) and	bx_agent.agentname like '%{10}%' order by {3} {4} limit {5},{6}";
            string sql = string.Format(format,
                string.Join(",", defeatAnalysisList.ToArray(i => string.Format("sum({0}) as '{0}'", "reason_" + defeatAnalysisList.IndexOf(i)))),
                string.Join(",", defeatAnalysisList.ToArray(i => string.Format("case a.defeatreasonid when {0} then count end  as '{1}'", i.DefeatReasonId, "reason_" + defeatAnalysisList.IndexOf(i)))),
                string.Join(",", agentIds),
                request.OrderBy,
                request.IsDesc ? "desc" : "",
                (request.CurPage - 1) * request.PageSize,
                request.PageSize,
                request.StartTime,
                request.EndTime,
                string.Join("+", defeatAnalysisList.ToArray(i => string.Format("ifnull({0},0)", "reason_" + defeatAnalysisList.IndexOf(i))))
                , request.SearchTxt
                );

            logInfo.InfoFormat("获取业务员战败数据统计请求sql ：{0}", sql);
            var dt = _sqlhelper.ExecuteDataTable(sql);
            dt.Columns.Remove("cagentid");
            var json = JsonConvert.SerializeObject(dt);
            var strJson = json.Replace(":null", ":0");
            return strJson;
        }
        public BusinessStatisticsViewModel GetBusinessStatistics(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime)
        {
            string sql = @"SELECT SUM(QUOTECARCOUNT)AS QUOTECARCOUNT,SUM(QUOTECOUNT) AS QUOTECOUNT,SUM(SMSSENDCOUNT) AS SMSSENDCOUNT,SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT,
                                    SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT,SUM(SINGLECOUNT) AS SINGLECOUNT,SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount,SUM(BatchRenewalCount)AS BatchRenewalCount
                                   FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS WHERE AGENTID IN (" +
                         string.Join(",", agentIds) + ")" + "AND DATAINTIME BETWEEN '" + startTime + "' AND '" + endTime + "'";
            logInfo.InfoFormat("获取业务统计请求sql ：{0}", sql);
            var bs = _sqlhelper.ExecuteDataTable(sql).ToEntity<BusinessStatistics>();

            var viewModel = new BusinessStatisticsViewModel
            {
                AppointmentCount = bs.AppointmentCount,
                DefeatCount = bs.DefeatCount,
                QuoteCarCount = bs.QuoteCarCount,
                QuoteCount = bs.QuoteCount,
                ReturnVisitCount = bs.ReturnVisitCount,
                SingleCount = bs.SingleCount,
                BatchRenewalCount = bs.BatchRenewalCount,
                SmsSendCount = bs.SmsSendCount,
                OrderCount = bs.OrderCount,
                thisNewDate = DateTime.Now.ToString("yyyy-MM-dd HH") + ":00:00"
            };
            return viewModel;
        }
        public List<int> GetDelSonListByDb(int currentAgent)
        {
            var listAgents = new List<int>();
            try
            {
                var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "curAgent",
                        Value = currentAgent
                    }
                };
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId=?curAgent");
                sqlBuilder.Append(" UNION");
                sqlBuilder.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1)");
                sqlBuilder.Append(" UNION");
                sqlBuilder.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM bx_agent WHERE parentagent IN (SELECT id FROM bx_agent WHERE parentagent=?curAgent AND isused>=1) )");
                sqlBuilder.Append(" UNION");
                sqlBuilder.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM bx_agent WHERE parentagent IN (");
                sqlBuilder.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                sqlBuilder.Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1)))");
                //查询列表
                IList<int> reList = _dbHelper.ExecuteDataSet(sqlBuilder.ToString(), parameters.ToArray()).ToList<int>();
                listAgents = reList.ToList();
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("当前代理人：" + currentAgent + "；发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<int>();
        }
        public List<int> GetAllSonListByDb(int currentAgent, bool hasSelf = true)
        {
            var listAgents = new List<int>();
            try
            {
                var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "curAgent",
                        Value = currentAgent
                    }
                };
                #region SQL语句
                var strSql = new StringBuilder();
                var Agent = GetAgent(currentAgent);
                if (hasSelf && Agent.RegType != 1)
                {
                    strSql.Append("select SQL_CACHE  ?curAgent ")
                        .Append(" union ");
                }
                if (Agent.RegType == 1)
                {
                    strSql.Append(" select id from  bx_agent where group_id=?curAgent and isused in(1,2)")
                    .Append(" union")
                    .Append(@" select id from bx_agent
                                                where parentagent in (select id from bx_agent where group_id=?curAgent and isused in(1,2)) ")
                    .Append(" union")
                    .Append(@" select id from bx_agent where parentagent in (
                                                select id from bx_agent where parentagent in (
                                                select id from  bx_agent where group_id=?curAgent and isused in(1,2)) ) ");

                    //strSql.Append(" SELECT id FROM  bx_agent WHERE group_id=?curAgent AND isused>=1");
                    //strSql.Append(" UNION ");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId=?curAgent");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT id FROM bx_agent");
                    //strSql.Append(" WHERE parentagent IN (SELECT id FROM bx_agent WHERE group_id=?curAgent AND isused>=1) ");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN (SELECT id FROM bx_agent");
                    //strSql.Append(" WHERE parentagent IN (SELECT id FROM bx_agent WHERE group_id=?curAgent AND isused>=1) )");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE group_id=?curAgent AND isused>=1) ) ");
                    //strSql.Append(" UNION ");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM  bx_agent WHERE group_id=?curAgent AND isused>=1) ) )");
                }
                else
                {
                    strSql.Append(" select id from  bx_agent where parentagent=?curAgent and isused in(1,2)")
                        .Append(" union")
                        .Append(@" select id from bx_agent
                                                where parentagent in (select id from bx_agent where parentagent=?curAgent and isused in(1,2)) ")
                        .Append(" union")
                        .Append(@" select id from bx_agent where parentagent in (
                                                select id from bx_agent where parentagent in (
                                                select id from  bx_agent where parentagent=?curAgent and isused in(1,2)) ) ");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId=?curAgent");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1)");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (SELECT id FROM bx_agent WHERE parentagent=?curAgent AND isused>=1) ");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM bx_agent WHERE parentagent IN (SELECT id FROM bx_agent WHERE parentagent=?curAgent AND isused>=1) )");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1) ) ");
                    //strSql.Append(" UNION");
                    //strSql.Append(" SELECT agentId AS id FROM bx_delete_agent_log WHERE delete_userId IN(SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (");
                    //strSql.Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused>=1)))");
                }

                #endregion
                //查询列表
                IList<int> reList = _dbHelper.ExecuteDataSet(strSql.ToString(), parameters.ToArray()).ToList<int>();
                listAgents = reList.ToList();
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<int>() { currentAgent };
        }
        public List<int> GetSonListByDb(int currentAgent, bool hasSelf = true)
        {
            var listAgents = new List<int>();
            try
            {
                var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "curAgent",
                        Value = currentAgent
                    }
                };
                #region SQL语句
                var strSql = new StringBuilder();
                var Agent = GetAgent(currentAgent);
                if (hasSelf && Agent.RegType != 1)
                {
                    strSql.Append("select SQL_CACHE  ?curAgent ")
                        .Append(" union ");
                }
                if (Agent.RegType == 1)
                {
                    strSql.Append(" select id from  bx_agent where group_id=?curAgent and isused in(1,2)")
                    .Append(" union")
                    .Append(@" select id from bx_agent
                            where parentagent in (select id from bx_agent where group_id=?curAgent and isused in(1,2)) ")
                    .Append(" union")
                    .Append(@" select id from bx_agent where parentagent in (
                            select id from bx_agent where parentagent in (
                            select id from  bx_agent where group_id=?curAgent and isused in(1,2)) ) ");
                }
                else
                {
                    strSql.Append(" select id from  bx_agent where parentagent=?curAgent and isused in(1,2)")
                        .Append(" union")
                        .Append(@" select id from bx_agent
                            where parentagent in (select id from bx_agent where parentagent=?curAgent and isused in(1,2)) ")
                        .Append(" union")
                        .Append(@" select id from bx_agent where parentagent in (
                            select id from bx_agent where parentagent in (
                            select id from  bx_agent where parentagent=?curAgent and isused in(1,2)) ) ");
                }
                //#第五级代理
                //union  all
                //select id from  bx_agent
                //where  parentagent in (
                //select id from bx_agent 
                //where  parentagent in (
                //select id from bx_agent
                //where  parentagent in (
                //select id from  bx_agent where  parentagent=@curAgent
                //) )
                //)
                #endregion
                //查询列表
                IList<int> reList = _dbHelper.ExecuteDataSet(strSql.ToString(), parameters.ToArray()).ToList<int>();
                listAgents = reList.ToList();
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<int>() { currentAgent };
        }
        public List<AgentData> GetTrendMap(IEnumerable<string> agentIds, DateTime startTime, DateTime endTime)
        {
            string sql = @"SELECT SUM(QUOTECARCOUNT)AS QUOTECARCOUNT,SUM(QUOTECOUNT) AS QUOTECOUNT,SUM(SMSSENDCOUNT) AS SMSSENDCOUNT,SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT,
                                    SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT,SUM(SINGLECOUNT) AS SINGLECOUNT,SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount,sum(BatchRenewalCount) as BatchRenewalCount ,DATAINTIME
                                   FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS WHERE AGENTID IN (" +
                         string.Join(",", agentIds) + ")" + "AND DATAINTIME BETWEEN '" + startTime + "' AND '" + endTime + "' GROUP BY DATAINTIME";
            logInfo.InfoFormat("获取趋势图请求sql ：{0}", sql);
            var listAgent = _sqlhelper.ExecuteDataTable(sql).ToList<AgentData>().ToList();
            return listAgent;
        }

        public IList<string> GetUsedSons(int agentId)
        {
            var para = new MySqlParameter
            {
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "agentId",
                Value = agentId
            };
            string sql1 = @"SELECT SQL_CACHE  @agentId AS ID
                            UNION
                            SELECT ID
                            FROM BX_AGENT
                            WHERE isused = 1
	                            AND PARENTAGENT = @agentId
                            UNION
                        SELECT ID
                        FROM BX_AGENT
                        WHERE isused = 1
	                        AND PARENTAGENT IN (SELECT ID
		                        FROM BX_AGENT
		                        WHERE PARENTAGENT = @agentId)
                        UNION
                        SELECT ID
                        FROM BX_AGENT
                        WHERE isused = 1
	                        AND PARENTAGENT IN (SELECT ID
		                        FROM BX_AGENT
		                        WHERE PARENTAGENT IN (SELECT ID
			                        FROM BX_AGENT
			                        WHERE PARENTAGENT = @agentId))";
            //查询所有已启用的代理人
            IList<string> reList = _dbHelper.ExecuteDataSet(sql1, para).ToList<string>();
            return reList;
        }


        public List<AgentData> GetAgentDataByPage(int agentId, DateTime startTime, DateTime endTime,
            bool isDesc, string orderBy, int curPage, int pageSize, string searchTxt, bool isByLevel, ref int totalCount)
        {
            var agent = GetAgent(agentId);
            IList<string> reList = new List<string>();
            IList<int> reList1 = new List<int>();
            //查询所有已启用的代理人
            if (agent.RegType == 1)
            {
                reList = GetAgentIdAndNameByGroupId(agentId.ToString()).Select(x => x.Id.ToString()).ToList<string>();
                reList1 = GetSonListByDb(agentId).ToList();
            }
            else
            {
                reList = GetUsedSons(agentId);
            }

            if (reList.Count == 0) return null;

            var agentIdsIsUsed = string.Join(",", reList);

            if (string.IsNullOrWhiteSpace(searchTxt))
                totalCount = reList.Count;
            else
                using (EntityContext _context = new EntityContext())
                {
                    var list = reList.ToList().ConvertAll(int.Parse);
                    totalCount = _context.bx_agent.Count(x => x.AgentName.Contains(searchTxt) && list.Contains(x.Id));
                }
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@agentIds", MySqlDbType.VarString){Value =agentIdsIsUsed },
                new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime},
                new MySqlParameter("@startIndex", MySqlDbType.Int32){Value =(curPage - 1) * pageSize},
                new MySqlParameter("@pageSize", MySqlDbType.Int32){Value =pageSize}
            };
            string sql = "";
            if (!isByLevel)
            {
                sql = string.Format(@"SELECT bx_agent.ID AS AGENTID, bx_agent.AGENTNAME,bx_agent.AGENT_LEVEL as AGENTLEVEL,a.QUOTECARCOUNT,a.QUOTECOUNT,a.SMSSENDCOUNT,a.RETURNVISITCOUNT,a.APPOINTMENTCOUNT,a.SINGLECOUNT,a.DEFEATCOUNT ,a.ordercount ,a.BatchRenewalCount 
                                        FROM bx_agent LEFT JOIN (SELECT AGENTID, AGENTNAME, SUM(QUOTECARCOUNT) AS QUOTECARCOUNT, SUM(QUOTECOUNT) AS QUOTECOUNT, SUM(SMSSENDCOUNT) AS SMSSENDCOUNT
                                    	, SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT, SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT, SUM(SINGLECOUNT) AS SINGLECOUNT, SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount ,sum(BatchRenewalCount) as BatchRenewalCount 
                                        FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS
                                        where
                                    		    DATAINTIME BETWEEN @startTime AND @endTime AND AGENTID IN ({2}) 
                                    	GROUP BY AGENTID
                                    	) a ON a.AGENTID = bx_agent.id WHERE ID IN ({2}) {3}
                                        ORDER BY {0} {1} 
                                        LIMIT @startIndex, @pageSize
                                                ", orderBy, isDesc ? "DESC" : "", agentIdsIsUsed, string.IsNullOrWhiteSpace(searchTxt) ? "" : "and bx_agent.AGENTNAME like '%" + searchTxt + "%'");
            }
            else
            {
                //    sql = string.Format(@"SELECT bx_agent.ID AS AGENTID, bx_agent.AGENTNAME,bx_agent.AGENT_LEVEL as AGENTLEVEL,a.QUOTECARCOUNT,a.QUOTECOUNT,a.SMSSENDCOUNT,a.RETURNVISITCOUNT,a.APPOINTMENTCOUNT,a.SINGLECOUNT,a.DEFEATCOUNT ,a.ordercount ,a.BatchRenewalCount 
                //                            FROM bx_agent LEFT JOIN (SELECT AGENTID, AGENTNAME, SUM(QUOTECARCOUNT) AS QUOTECARCOUNT, SUM(QUOTECOUNT) AS QUOTECOUNT, SUM(SMSSENDCOUNT) AS SMSSENDCOUNT
                //                        	, SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT, SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT, SUM(SINGLECOUNT) AS SINGLECOUNT, SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount ,sum(BatchRenewalCount) as BatchRenewalCount 
                //                            FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS
                //                            where
                //                        		    DATAINTIME BETWEEN @startTime AND @endTime AND AGENTID IN ({2}) 
                //                        	GROUP BY AGENTID
                //                        	) a ON a.AGENTID = bx_agent.id WHERE ID IN ({2}) {3}
                //                            ORDER BY {0} {1} ", orderBy, isDesc ? "DESC" : "", agentIdsIsUsed, string.IsNullOrWhiteSpace(searchTxt) ? "" : "and bx_agent.AGENTNAME like '%" + searchTxt + "%'");
                //}
                sql = string.Format(@"select t2.*,t3.AgentName,t3.AGENT_LEVEL as AGENTLEVEL from(SELECT Path as AgentId,SUM(QUOTECARCOUNT) AS QUOTECARCOUNT, SUM(QUOTECOUNT) AS QUOTECOUNT, SUM(SMSSENDCOUNT) AS SMSSENDCOUNT
                                    , SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT, SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT, SUM(SINGLECOUNT) AS SINGLECOUNT, SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount ,sum(BatchRenewalCount) as BatchRenewalCount from(
                                    select * from(
                                    select ID,ID as Path from bx_agent t where t.ID in({2})
                                    UNION 
                                    select ID,ParentAgent as Path from bx_agent t where t.ParentAgent in(select ID from bx_agent t where t.ID in({2}))
                                    UNION
                                    select ID,TopAgentId as Path from bx_agent t where t.ParentAgent in(select ID from bx_agent t where t.ParentAgent in(select ID from bx_agent t where t.ID in({2})
                                    ))) t
                                    LEFT JOIN
                                    (SELECT AGENTID,SUM(QUOTECARCOUNT) AS QUOTECARCOUNT, SUM(QUOTECOUNT) AS QUOTECOUNT, SUM(SMSSENDCOUNT) AS SMSSENDCOUNT
                                    , SUM(RETURNVISITCOUNT) AS RETURNVISITCOUNT, SUM(APPOINTMENTCOUNT) AS APPOINTMENTCOUNT, SUM(SINGLECOUNT) AS SINGLECOUNT, SUM(DEFEATCOUNT) AS DEFEATCOUNT,sum(ordercount) as ordercount ,sum(BatchRenewalCount) as BatchRenewalCount 
                                    FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS
                                    where
                                    DataInTime BETWEEN @startTime AND @endTime AND
                                     AGENTID IN ({4})
                                    GROUP BY AGENTID
                                    ) a on t.ID=a.AGENTID) t1 GROUP BY t1.Path) t2 left join bx_agent t3 ON t2.AgentId=t3.Id {3}
                                    order by {0} {1} LIMIT @startIndex, @pageSize", orderBy, isDesc ? "DESC" : "", agentIdsIsUsed, string.IsNullOrWhiteSpace(searchTxt) ? "" : "where t3.AgentName like '%" + searchTxt + "%'", agent.RegType == 1 ? string.Join(",", reList1) : agentIdsIsUsed);
            }
            logInfo.InfoFormat("获取业务员数据请求sql ：{0},agentIds:{1},startTime:{2},endTime:{3},startIndex:{4},pageSize:{5}", sql, string.Join(",", agentIdsIsUsed), startTime, endTime, (curPage - 1) * pageSize, pageSize);
            var listAgent = _sqlhelper.ExecuteDataTable(sql, parameters).ToList<AgentData>().ToList();
            return listAgent;
        }
        public List<AgentData> GetSingleAgentData(int agentId, DateTime startTime, DateTime endTime)
        {
            string sql = string.Format(@"SELECT AGENTID,AGENTNAME,sum(QUOTECARCOUNT) as QUOTECARCOUNT,sum(QUOTECOUNT) as QUOTECOUNT,sum(SMSSENDCOUNT) as SMSSENDCOUNT,sum(RETURNVISITCOUNT) as RETURNVISITCOUNT,
                                    sum(APPOINTMENTCOUNT) as APPOINTMENTCOUNT,sum(SINGLECOUNT) as SINGLECOUNT,sum(DEFEATCOUNT) as DEFEATCOUNT,sum(ordercount) as  ordercount
                                   FROM BIHU_ANALYTICS.BUSINESS_ANALYTICS WHERE AGENTID= {0} AND DATAINTIME BETWEEN '{1}' AND '{2}'",
                agentId, startTime, endTime);
            var listAgent = _sqlhelper.ExecuteDataTable(sql).ToList<AgentData>().ToList();
            return listAgent;
        }

        public List<SfAgentData> GetAgentData4SfH5ByPage(int groupId, DateTime startTime, DateTime endTime, bool isDesc, string orderBy, int pageIndex, int pageSize, string serachText, ref int totalCount)
        {
            var topAgentList = GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => x.Id).ToList();
            if (topAgentList.Count == 0) return null;

            var agentIdsIsUsed = string.Join(",", topAgentList);

            if (string.IsNullOrWhiteSpace(serachText))
            {
                totalCount = topAgentList.Count;
            }
            else
            {
                using (EntityContext _context = new EntityContext())
                {
                    totalCount = _context.bx_agent.Count(x => topAgentList.Contains(x.Id) && x.AgentName.Contains(serachText));
                }
            }
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime},
                new MySqlParameter("@startIndex", MySqlDbType.Int32){Value =(pageIndex - 1) * pageSize},
                new MySqlParameter("@pageSize", MySqlDbType.Int32){Value =pageSize}
            };
            var sql = string.Format(@"SELECT t.id AgentId,t.AgentName,
	                                    CASE t.AgentType
		                                    WHEN 0 THEN
			                                    '4S店'
		                                    WHEN 1 THEN
			                                    '修理厂'
		                                    WHEN 2 THEN
			                                    '专业代理'
		                                    WHEN 3 THEN
			                                    '互联网公司'
		                                    WHEN 4 THEN
			                                    '其他'
		                                    WHEN 5 THEN
			                                    '保险公司'
	                                    END AgentType,
	                                    IFNULL(t1.InsureCount, 0) InsureCount,IFNULL(t2.RenewalCount, 0) RenewalCount,IFNULL(FORMAT(t1.InsureCount / t2.RenewalCount,2),0.00) Ratio
                                    FROM bx_agent t
                                    LEFT JOIN (
	                                    SELECT topagentid,sum(renbaoinsurecount + pinganinsurecount + taipingyanginsurecount + guoshoucaiinsurecount + othersourceinsurecount) AS InsureCount
	                                    FROM bihu_analytics.tj_insuredistributionanalysis t
	                                    WHERE DATE_FORMAT(t.insuretime_year_month_day,'%Y-%m-%d %H:%i:%s')>=@startTime AND DATE_FORMAT(t.insuretime_year_month_day,'%Y-%m-%d %H:%i:%s')<@endTime AND topagentid IN ({0})
	                                    GROUP BY topagentid
                                    ) t1 ON t.Id = t1.topagentid
                                    LEFT JOIN (
                                        SELECT TopAgentId,RenewalCount
							          FROM bihu_analytics.tj_renewalcount_record
							          WHERE RenewalTime>=@startTime AND RenewalTime<@endTime and TopAgentId IN ({0})
                                    ) t2 ON t.Id = t2.TopAgentId
                                    WHERE t.id IN ({0}) {3} order by {1} {2} LIMIT @startIndex, @pageSize", agentIdsIsUsed, orderBy, isDesc ? "DESC" : "", string.IsNullOrWhiteSpace(serachText) ? "" : "and t.AgentName like '%" + serachText + "%'");
            var datalist = _sqlhelper.ExecuteDataTable(sql, parameters).ToList<SfAgentData>().ToList();
            return datalist;
        }

        public SfH5AverageViewModel GetAverageData(int groupId, DateTime startTime, DateTime endTime)
        {
            var topAgentList = GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => x.Id).ToList();
            if (topAgentList.Count == 0) return new SfH5AverageViewModel();
            var agentIdsIsUsed = string.Join(",", topAgentList);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
            };
            var sql = string.Format(@"select FORMAT(InsureCount/{1},1) AverageInsureCount,FORMAT(RenewalCount/{1},1) AverageRenewalCount,FORMAT(InsureCount/RenewalCount,2) AverageRatio from(
                                    select
                                    IFNULL((SELECT sum(renbaoinsurecount) AS InsureCount
	                                    FROM bihu_analytics.tj_insuredistributionanalysis t
	                                    WHERE DATE_FORMAT(t.insuretime_year_month_day,'%Y-%m-%d %H:%i:%s')>=@startTime AND DATE_FORMAT(t.insuretime_year_month_day,'%Y-%m-%d %H:%i:%s')<@endTime AND topagentid IN ({0})
                                    ),0) InsureCount,
                                    IFNULL((SELECT SUM(RenewalCount) FROM bihu_analytics.tj_renewalcount_record WHERE RenewalTime>=@startTime AND RenewalTime<@endTime AND TopAgentId IN ({0})
		                            ),0) RenewalCount) t", agentIdsIsUsed, topAgentList.Count);
            var data = _sqlhelper.ExecuteDataTable(sql, parameters).ToEntity<SfH5AverageViewModel>();
            return data;
        }

        public List<AgentData> GetAgentData(IEnumerable<string> agentIds)
        {
            string sql = @"SELECT AgentId,AgentName,QuoteCarCount,QuoteCount,SmsSendCount,ReturnVisitCount,
                                    AppointmentCount,SingleCount,DefeatCount,BatchRenewalCount,DataInTime
                                   FROM bihu_analytics.BUSINESS_ANALYTICS WHERE agentid in (" +
                                  string.Join(",", agentIds) + ")";

            var listAgent = _sqlhelper.ExecuteDataTable(sql).ToList<AgentData>().ToList();
            return listAgent;
        }

        /// <summary>
        /// 获取顶级经纪人Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int GetTopAgentIdByAgentId(int agentId)
        {
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int32)
                };
                parameters[0].Value = agentId;
                var query = DataContextFactory.GetDataContext().Database.SqlQuery<string>("select TopAgentId from bx_agent where id=@agentId", parameters).FirstOrDefault();
                return Convert.ToInt32(query);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return -1;
        }

        /// <summary>
        /// 获取角色名称
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public string GetRoleNameByAgentId(int agentId)
        {
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int32)
                };
                parameters[0].Value = agentId;
                return DataContextFactory.GetDataContext().Database.SqlQuery<string>(" select role_name from manager_role_db where id= ( SELECT ManagerRoleId FROM bx_agent where id=@agentId) and role_status=0", parameters).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return "";
        }

        public List<int> GetAllTopAgentId()
        {
            return DataContextFactory.GetDataContext().bx_agent.Where(o => o.IsDaiLi == 1 && o.IsUsed == 1).Select(o => o.Id).ToList();
        }

        /// <summary>
        /// 根据手机号获取代理人
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topParentAgent"></param>
        /// <returns></returns>
        public bool GetAgentByPhoneTopAgent(string mobile, int parentAgent)
        {
            List<int> listAgent = new List<int>();
            listAgent = GetAllAgentByPhone(mobile);
            //如果为空，返回false
            if (!listAgent.Any()) return false;
            //遍历list做比较
            var parentAgentTopagent = GetTopAgentIdList(parentAgent.ToString());
            var agentids = string.Join(",", listAgent);


            List<int> allAgentList = GetTopAgentIdList(agentids);
            return allAgentList.Any(c => c.Equals(parentAgentTopagent.FirstOrDefault()));



        }
        /// <summary>
        /// 根据mobile获取所有的代理
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public List<int> GetAllAgentByPhone(string mobile)
        {
            var item = new List<int>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.Mobile == mobile).Select(s => s.Id).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        public IEnumerable<AgentGroupVM> GetAgentGroups()
        {
            var _dbContext = DataContextFactory.GetDataContext();
            var topAgentGroup = _dbContext.bx_agent.GroupBy(x => new { x.TopAgentId }).Select(x => new AgentGroupVM { AgentIdKey = x.Key.TopAgentId, AgentIdsValue = x.Select(a => a.Id) }).ToList();
            var tempOne = _dbContext.bx_agent.Where(x => x.agent_level == 3).GroupBy(x => new { x.ParentAgent }).Select(x => new AgentGroupVM { AgentIdKey = x.Key.ParentAgent, AgentIdsValue = x.Select(a => a.Id) }).ToList();
            var tempTwo = _dbContext.bx_agent.Where(x => x.agent_level == 2).Select(x => x.Id).ToList();
            var tempIds = tempTwo.Except(tempOne.Select(x => x.AgentIdKey));
            List<AgentGroupVM> tempThree = new List<AgentGroupVM>();
            if (tempIds.Any())
            {
                tempThree = _dbContext.bx_agent.Where(x => tempIds.Contains(x.Id)).Select(x => new AgentGroupVM { AgentIdKey = x.Id }).ToList();
            }
            tempThree.ForEach(x => x.AgentIdsValue = new List<int>());
            var parentAgentGroup = tempOne.Union(tempThree).ToList();
            var level3AgentGroup = _dbContext.bx_agent.Where(x => x.agent_level == 3).Select(x => new AgentGroupVM { AgentIdKey = x.Id }).ToList();
            level3AgentGroup.ForEach(x => x.AgentIdsValue = new List<int> { x.AgentIdKey });
            parentAgentGroup.ForEach(x => x.AgentIdsValue = x.AgentIdsValue.Union(new List<int> { x.AgentIdKey }));

            return topAgentGroup.Union(parentAgentGroup).Union(level3AgentGroup);


        }

        public AgentGroupVM GetSpecifiedAgentGroupToRedis(int currentAgentId)
        {
            AgentGroupVM agentGroupVm = new AgentGroupVM();
            var _dbContext = DataContextFactory.GetDataContext();

            var agentIds = _dbContext.bx_agent.Where(x => x.ParentAgent == currentAgentId).Select(x => x.Id);
            agentGroupVm.AgentIdKey = currentAgentId;
            agentGroupVm.AgentIdsValue = agentIds;
            return agentGroupVm;
        }

        public int GetAgentEffectiveCallDuration(int agentId)
        {
            int effectiveCallDuration = 0;
            var agent = DataContextFactory.GetDataContext().bx_agent.Where(x => x.Id == agentId).SingleOrDefault();
            if (agent != null)
            {
                effectiveCallDuration = agent.EffectiveCallDuration;
            }
            return effectiveCallDuration;

        }

        public bool UpdateAgentEffectiveCallDuration(int agentId, int effectiveCallDuration)
        {
            var _dbContext = DataContextFactory.GetDataContext();
            var agent = _dbContext.bx_agent.Where(x => x.Id == agentId).SingleOrDefault();
            if (agent != null)
            {
                agent.EffectiveCallDuration = effectiveCallDuration;
            }
            return _dbContext.SaveChanges() >= 0;
        }

        /// <summary>
        /// 注册顶级代理人时判断手机号是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="parentShareCode"></param>
        /// <returns></returns>
        public bool IsExistMobileForTopAgent(string mobile, int topAgentId)
        {
            string sql = string.Empty;
            if (topAgentId > 0)
            {
                sql = "SELECT 1 FROM bx_agent WHERE IsUsed in (0,1,2) AND mobile=?mobile AND topAgentId=?topAgentId LIMIT 1";
            }
            else
            {
                sql = "SELECT 1 FROM bx_agent WHERE IsUsed in (0,1,2) AND mobile=?mobile LIMIT 1";
            }
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="mobile",
                    Value=mobile,
                    MySqlDbType=MySqlDbType.VarChar
                },
                new MySqlParameter
                {
                    ParameterName="topAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.VarChar
                }
            };
            var result = _dbHelper.ExecuteScalar<string>(sql, param);
            return !string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="oldRoleId"></param>
        /// <param name="newRoleId"></param>
        /// <returns></returns>
        public bool UpdateRoleId(int oldRoleId, int newRoleId)
        {
            var sql = " UPDATE bx_agent SET ManagerRoleId=?newRoleId WHERE ManagerRoleId=?oldRoleId";
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
        public List<AgentIdAndAgentName> GetByTopAgentId2(int topAgentId)
        {
            var sql = @"
                        SELECT                    bx_agent.id               AS AgentId,
                          bx_agent.agentname   AS AgentName,
                          bx_agent.AgentAccount     AS AgentAccount,
                          manager_role_db.role_type AS RoleType,
                           bx_agent.IsUsed
                        FROM bx_agent
                          INNER JOIN manager_role_db
                            ON bx_agent.ManagerRoleId = manager_role_db.id                           
                        WHERE topagentid = ?topAgentId
                            AND isused in(1,2)
                        ORDER BY bx_agent.CreateTime";
            //sql += " union ";
            // sql += "(SELECT AgentId ,CONCAT(agent_name,'（已删）') AS AgentName,'' AS AgentAccount, 0 AS RoleType,3 AS IsUsed FROM bx_delete_agent_log WHERE delete_userid IN(SELECT id FROM bx_agent WHERE topagentid=?topAgentId))";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="topAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sql, param).ToList();
        }

        public List<AgentIdAndAgentName> GetByTopAgentId(int topAgentId)
        {
            var sql = @"
                        SELECT
                          bx_agent.id               AS AgentId,
                          bx_agent.agentname        AS AgentName,
                          bx_agent.AgentAccount     AS AgentAccount,
                          manager_role_db.role_type AS RoleType
                        FROM bx_agent
                          INNER JOIN manager_role_db
                            ON bx_agent.ManagerRoleId = manager_role_db.id
                        WHERE topagentid = ?topAgentId
                            AND isused = 1
                        ORDER BY CreateTime;";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="topAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sql, param).ToList();
        }
        public List<AgentIdAndAgentName> GetByParentAgentId2(int parentAgentId)
        {
            var sql = @"
                    SELECT
                        bx_agent.id               AS AgentId,
                        bx_agent.agentname        AS AgentName,
                        bx_agent.AgentAccount     AS AgentAccount,
                        manager_role_db.role_type AS RoleType,
                       bx_agent.IsUsed
                    FROM bx_agent
                        INNER JOIN manager_role_db
                        ON bx_agent.ManagerRoleId = manager_role_db.id
                    WHERE (parentagent = ?parentAgent
                            OR bx_agent.id = ?parentAgent)
                        AND isused in (1,2)
                    ORDER BY CreateTime
                    ";
            //sql += " union ";
            //sql += " (SELECT AgentId ,CONCAT(agent_name,'（已删）') AS AgentName,'' AS AgentAccount, 0 AS RoleType,3 AS IsUsed FROM bx_delete_agent_log WHERE delete_userid IN(SELECT id FROM bx_agent WHERE parentagent=?parentAgent OR bx_agent.id = ?parentAgent)) ";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="parentAgent",
                    Value=parentAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sql, param).ToList();
        }
        public List<AgentIdAndAgentName> GetByParentAgentId(int parentAgentId)
        {
            var sql = @"
                    SELECT
                        bx_agent.id               AS AgentId,
                        bx_agent.agentname        AS AgentName,
                        bx_agent.AgentAccount     AS AgentAccount,
                        manager_role_db.role_type AS RoleType
                    FROM bx_agent
                        INNER JOIN manager_role_db
                        ON bx_agent.ManagerRoleId = manager_role_db.id
                    WHERE (parentagent = ?parentAgent
                            OR bx_agent.id = ?parentAgent)
                        AND isused = 1
                    ORDER BY CreateTime;
                    ";

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="parentAgent",
                    Value=parentAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentIdAndAgentName>(sql, param).ToList();
        }

        /// <summary>
        /// 除了当前代理人以外其他用户使用这个账号的数量 
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public int SameAccountCount(int agentId, string account)
        {
            int count = db.bx_agent.Where(t => t.AgentAccount == account && t.Id != agentId).ToList().Count;
            return count;
        }

        /// <summary>
        /// 查询没有助理角色的顶级代理人 zky 2017-01-01 /crm
        /// </summary>
        /// <returns></returns>
        public IList<bx_agent> GetNoHelperTopAgent()
        {
            string sql = "select * from bx_agent where ParentAgent=0 and  IsDaiLi=1 and agent_level=1 and id not in(SELECT top_agent_id from manager_role_db where role_type = 5)";
            return db.Database.SqlQuery<bx_agent>(sql).ToList();
        }

        public string GetUkeyName(int ukeyId)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(i => i.id == ukeyId).Select(l => l.config_name).FirstOrDefault();
        }
        /// <summary>
        /// 获取source
        /// </summary>
        /// <param name="source">source值</param>
        /// <param name="sourceType"> 0:传递新source  1：传递老source</param>
        /// <returns></returns>
        public bx_companyrelation GetSource(long source, int sourceType)
        {
            var _dbContext = DataContextFactory.GetDataContext();
            var companyRelationModel = new bx_companyrelation();
            if (sourceType == 0)
            {
                companyRelationModel = _dbContext.bx_companyrelation.SingleOrDefault(x => x.source == source);
            }
            else
            {
                companyRelationModel = _dbContext.bx_companyrelation.SingleOrDefault(x => x.flags == source);
            }
            return companyRelationModel;
        }
        /// <summary>
        /// 业务员列表批量审核 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds">批量更新的代理id</param>
        /// <param name="messagePayType">短信扣费方式</param>
        /// <param name="usedStatus">启用状态</param>
        /// <param name="isShowRate">是否展示费率</param>
        /// <param name="isSubmit">是否可核保</param>
        /// <returns></returns>
        public bool AgentBatchAudit(List<int> agentIds, int messagePayType, int usedStatus, int isShowRate, int isSubmit, int zhenBangType)
        {
            bool result;
            string sql = string.Empty;
            if (zhenBangType > 0)//振邦账号批量审核不处理费率
            {
                sql = @"UPDATE bx_agent
                            SET 
                                isused =?isused, 
                                MessagePayType =?MessagePayType,
                                IsSubmit =?IsSubmit
                            WHERE
                                id IN (" + agentIds.Join(",") + ")";
            }
            else
            {
                sql = @"UPDATE bx_agent
                            SET 
                                isused =?isused, 
                                MessagePayType =?MessagePayType,
                                IsShow =?IsShow, 
                                IsSubmit =?IsSubmit
                            WHERE
                                id IN (" + agentIds.Join(",") + ")";
            }

            MySqlParameter[] param = new MySqlParameter[] {
                new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "isused",
                        Value = usedStatus
                    },
                new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "MessagePayType",
                        Value = messagePayType
                    },
                new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "IsShow",
                        Value = isShowRate
                    },
                new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "IsSubmit",
                        Value = isSubmit
                    }
                //new MySqlParameter
                //    {
                //        MySqlDbType = MySqlDbType.Int32,
                //        ParameterName = "agentIds",
                //        Value = agentIds
                //    }
            };

            try
            {
                _dbHelper.ExecuteNonQuery(sql, param);
                result = true;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                result = false;
            }
            return result;

        }

        /// <summary>
        /// 根据代理人Id获取代理人账号 zky 2017-08-31 /crm
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        public List<string> GetAgentAccountByAgentId(List<int> agentIds)
        {
            return db.bx_agent.Where(t => agentIds.Contains(t.Id)).Select(t => t.AgentAccount).ToList();
        }

        /// <summary>
        /// 根据条件查询  zky 2017-09-21 /crm
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IList<bx_agent> GetList(Expression<Func<bx_agent, bool>> where)
        {
            return db.bx_agent.Where(where).ToList();
        }

        public List<AgentNameViewModel> FindAgentList(string agentIds)
        {
            var listAgents = new List<AgentNameViewModel>();
            try
            {
                string strSql = string.Format("select id as AgentId,AgentName from bx_agent where id in ({0})", agentIds);
                listAgents = DataContextFactory.GetDataContext().Database.SqlQuery<AgentNameViewModel>(strSql).ToList();
                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<AgentNameViewModel>();
        }

        public List<int> GetAllAgentIds()
        {
            string getAllAgentIds = "select id from bx_agent where isused=1 order by CreateTime";
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(getAllAgentIds).ToList();
        }

        /// <summary>
        /// 获取集团账号下的机构列表 zky 2017-11-13 /crm、统计
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="authenState"></param>
        /// <param name="groupId"></param>
        /// <param name="needPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<OrgListDto> GetOrgList(string orgName, int authenState, int groupId, bool needPage, int orgId, int pageIndex, int pageCount, out int total)
        {
            IList<OrgListDto> result = new List<OrgListDto>();
            string sqlWhere = string.Empty;


            if (authenState == 0)
            {
                sqlWhere += " and (c.authen_state is null or c.authen_state=0)";
            }
            else if (authenState == 1)
            {
                sqlWhere += " and c.authen_state=1";
            }

            if (groupId > 0)
            {
                sqlWhere += " and a.group_id=?groupId";
            }
            if (orgId > 0)
            {
                sqlWhere += " and a.id=?orgId";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(orgName))
                {
                    sqlWhere += " and a.agentName like concat('%',?orgName,'%')";
                }
            }

            string sql = @"SELECT
	                        a.id AS AgentId,
	                        a.AgentName,
	                        a.Mobile,
                            a.AgentAccount as Account,
                            a.AgentPassWord as Password,
                            a.charge_person as ChargePerson,
	                        b.city_name as CityName,
	                        IF(c.authen_state=1,'已认证','未认证') as AuthenState,
	                        IF((select count(1) from bx_camera_config d where a.id=d.park_id)>0,'有','无') as Camera,
                            IF(c.id is null,0,c.id) as AuthenId
                        FROM
	                        bx_agent a
                        INNER JOIN bx_city b ON a.region = b.id
                        LEFT JOIN bx_group_authen c ON a.id = c.agentId
                        WHERE
	                         a.isDaiLi=1 {0}";
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter{ParameterName="orgName",Value=orgName,MySqlDbType=MySqlDbType.VarChar },
                new MySqlParameter{ParameterName="authenState",Value=authenState,MySqlDbType=MySqlDbType.Int32 },
                new MySqlParameter{ParameterName="groupId",Value=groupId,MySqlDbType=MySqlDbType.Int32 },
                new MySqlParameter{ParameterName="orgId",Value=orgId,MySqlDbType=MySqlDbType.Int32 },
            };

            string executeSql = string.Format(sql, sqlWhere);
            var query = db.Database.SqlQuery<OrgListDto>(executeSql, param);
            total = query.Count();

            if (!needPage)//不需要分页
            {
                result = query.ToList();
            }
            else
            {
                result = query.Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            }
            return result;
        }

        public List<bx_agent> GetAgentIdAndNameByGroupId(string groupIds)
        {
            try
            {

                var selectSql = string.Format(@"select a.id,a.agentname from bx_agent as a where a.group_id in({0}) and a.isused=1", groupIds);
                //List<MySqlParameter> ps = new List<MySqlParameter> { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "group_id", Value = groupId } };
                return _sqlhelper.ExecuteDataTable(selectSql).ToList<bx_agent>().ToList();
            }
            catch (Exception ex)
            {

                logError.ErrorFormat("获取集团下顶级代理人异常：{0}", ex);
            }
            return null;
        }

        public int AddDeleteAgentLog(int agentId, string agentName, int deleteUserId, string deleteAccount, int deletePaltform)
        {
            string sql = @"INSERT into bx_delete_agent_log
                        (agentId, agent_name, delete_userId, delete_account, delete_date, delete_platform)
                        VALUES(?agentId, ?agentName, ?deleteUserId, ?deleteAccount, now(), ?deletePaltform)";

            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter{ParameterName="agentId",MySqlDbType=MySqlDbType.Int32,Value=agentId },
                new MySqlParameter{ParameterName="agentName",MySqlDbType=MySqlDbType.VarChar,Value=agentName },
                new MySqlParameter{ParameterName="deleteUserId",MySqlDbType=MySqlDbType.Int32,Value=deleteUserId },
                new MySqlParameter{ParameterName="deleteAccount",MySqlDbType=MySqlDbType.VarChar,Value=deleteAccount },
                new MySqlParameter{ParameterName="deletePaltform",MySqlDbType=MySqlDbType.Int32,Value=deletePaltform },
            };
            return db.Database.ExecuteSqlCommand(sql, param);
        }

        public IList<OrgListDto> AgentNameIdList(int groupId)
        {
            var list = db.bx_agent.Where(t => t.group_id == groupId).Select(t => new OrgListDto
            {
                AgentId = t.Id,
                AgentName = t.AgentName
            }).ToList();
            return list;
        }

        /// <summary>
        /// 判断代理人是否在顶级下 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="shareCode"></param>
        /// <returns></returns>
        public bool IsTopAgentSonShareCode(int agentId, int shareCode)
        {
            string sql = @"SELECT COUNT(id) FROM bx_agent WHERE TopAgentId=
                        (
                        SELECT TopAgentId FROM bx_agent WHERE id=?id 
                        )
                        AND sharecode=?shareCode";
            var param = new MySqlParameter[] {
                new MySqlParameter {
                    MySqlDbType=MySqlDbType.Int32,
                    ParameterName="id",
                    Value=agentId
                },
                new MySqlParameter { MySqlDbType=MySqlDbType.Int32,
                    ParameterName="shareCode",
                    Value=shareCode},
            };

            return _dbHelper.ExecuteScalar<int>(sql, param) > 0;

        }

        /// <summary>
        /// 判断邀请码是否是三级代理的 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="shareCode"></param>
        /// <returns></returns>
        public bool IsThreeShareCode(int shareCode)
        {
            string sql = "SELECT COUNT(id) FROM bx_agent WHERE ShareCode=?shareCode AND parentAgent!=TopAgentid and parentagent!=0";
            return _dbHelper.ExecuteScalar<int>(sql, new MySqlParameter
            {
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "shareCode",
                Value = shareCode
            }) > 0;
        }

        /// <summary>
        /// 判断代理人是否是三级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool IsThreeAgent(int agentId)
        {
            string sql = "SELECT COUNT(*) FROM bx_agent WHERE id=?id AND agent_level=3";
            return _dbHelper.ExecuteScalar<int>(sql, new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "id",
                Value = agentId
            }) > 0;
        }

        /// <summary>
        /// 判断代理人是否是二级并且没有三级代理 zky 2017-11-20 /crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool IsTwoAgentAndNoSon(int agentId)
        {
            string sql = @"SELECT  COUNT(Id) FROM bx_agent WHERE  id=?parentagent and  parentagent=TopAgentid AND 
                        (
                            SELECT COUNT(id) FROM bx_agent WHERE parentagent = ?parentagent
                        ) = 0";
            return _dbHelper.ExecuteScalar<int>(sql, new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "parentagent",
                Value = agentId
            }) > 0;
        }

        /// <summary>
        /// 获取业务员列表 zky 2017-12-8 /crm(如果查询条件修改请修改下面 GetAgetListCount 条件)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Mobile"></param>
        /// <param name="StateDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="IsUsed"></param>
        /// <param name="ParentAgentName"></param>
        /// <param name="ParentAgentId"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="AgentId"></param>
        /// <param name="AuthenState"></param>
        /// <param name="ZhenBangType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IList<CustomerModel> GetAgentList(string Name, string Mobile, string StateDateTime, string EndDateTime, int IsUsed, string ParentAgentName, string ParentAgentId, int TopAgentId, int AgentId, int AuthenState, int QueryZBType, int OnlySite, int AgentZBType, int PageIndex, int PageSize, int TestState, out int RecordCount)
        {
            string sqlwhere = string.Empty;
            #region 查询条件
            if (!string.IsNullOrEmpty(Name) && Regex.IsMatch(Name, "^%|%$", RegexOptions.IgnorePatternWhitespace))
            {
                TopAgentId = AgentId = 0;
            }

            if (TopAgentId == AgentId)//顶级账号
            {
                sqlwhere = @"where 1=1 and bx_agent.IsUsed != 3  and bx_agent.TopAgentId=?TopAgentId";
            }
            else//非顶级
            {
                sqlwhere = @"where 1=1 and bx_agent.IsUsed != 3  and (bx_agent.id=?AgentId or bx_agent.ParentAgent=?AgentId)";
            }

            if (IsUsed != -1)
            {
                sqlwhere += " and bx_agent.IsUsed=?IsUsed";
            }
            if (!string.IsNullOrEmpty(ParentAgentName))
            {
                sqlwhere += " and (SELECT max(AgentName) FROM bx_agent as m WHERE m.id= bx_agent.ParentAgent )  like concat('%',?ParentAgentName,'%') ";
            }
            if (!string.IsNullOrEmpty(ParentAgentId))
            {
                sqlwhere += " and bx_agent.ParentAgent in (?ParentAgentId)";
            }
            if (!string.IsNullOrEmpty(Name))
            {
                sqlwhere += " and bx_agent.AgentName  like concat('%',?Name,'%')";
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                sqlwhere += " and bx_agent.Mobile=?Mobile";
            }
            if (!string.IsNullOrEmpty(StateDateTime))
            {
                StateDateTime = Convert.ToDateTime(StateDateTime).ToString("yyyy-MM-dd");
                sqlwhere += " and bx_agent.CreateTime>=?StateDateTime";
            }
            if (!string.IsNullOrEmpty(EndDateTime))
            {
                EndDateTime = Convert.ToDateTime(EndDateTime).AddDays(1).ToString();
                sqlwhere += " and bx_agent.CreateTime<?EndDateTime";
            }

            if (AgentZBType > 0)//振邦账号列表查询需要添加 类型限制
            {
                if (OnlySite > 0)
                {
                    sqlwhere += " and bx_agent.zhen_bang_type=2";
                }
                else
                {
                    sqlwhere += " and bx_agent.zhen_bang_type in (1,3,4)";
                }
            }
            if (QueryZBType > 0)
            {
                sqlwhere += " and bx_agent.zhen_bang_type=?ZhenBangType";
            }

            //增城人保的认证状态 区分振邦的认证状态
            //modify by qidakang zcTopAgentId是以,分割的字符串，跟TopAgentId.ToString()不能用==，只能使用Contains
            if (zcTopAgentId.Contains(TopAgentId.ToString()))
            {
                if (AuthenState == -1)
                {
                    sqlwhere += " and authen.authen_state is null";
                }
                else if (AuthenState == 0) //modify qidakang 2018-10-10 未认证包含null和0
                {
                    sqlwhere += " and (authen.authen_state is null or authen.authen_state=0)";
                }
                else if ( AuthenState == 1 || AuthenState == 2)
                {
                    sqlwhere += " and authen.authen_state=?AuthenState";
                }
            }
            else
            {
                if (AuthenState == 0)
                {
                    sqlwhere += " and (authen.authen_state is null or authen.authen_state=0)";
                }
                else if (AuthenState == 1)
                {
                    sqlwhere += " and authen.authen_state=1";
                }
            }

            //add by qidakang  2018-04-09 10:32:00
            if (TestState == 1)//当是1时候通过，
            {
                sqlwhere += " and authen.TestState=?TestState";
            }
            else if (TestState == 0)//0和-1未通过,TestState=-1时候，查询所有数据
            {
                sqlwhere += " and (authen.TestState is null or authen.TestState=0 or authen.TestState=-1)";
            }


            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Name",
                    Value = Name
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "StateDateTime",
                    Value = StateDateTime
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "EndDateTime",
                    Value = EndDateTime
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "ParentAgentName",
                    Value = ParentAgentName
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "IsUsed",
                    Value = IsUsed
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ParentAgentId",
                    Value = ParentAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "AgentId",
                    Value = AgentId
                },
                 new MySqlParameter
                 {
                     MySqlDbType=MySqlDbType.Int32,
                     ParameterName="ZhenBangType",
                     Value=QueryZBType
                 },
                 new MySqlParameter
                 {
                     MySqlDbType=MySqlDbType.Int32,
                     ParameterName="AuthenState",
                     Value=AuthenState
                 },
                 new MySqlParameter
                 {
                      MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TestState",
                    Value = TestState
                 }
            };
            #endregion
            #region 查询sql
            string sql = @"
                SELECT 	bx_agent.Id, 
                bx_agent.AgentName, 
                bx_agent.Mobile, 
                bx_agent.OpenId, 
                bx_agent.ShareCode, 
                bx_agent.CreateTime, 
                bx_agent.IsBigAgent,
                bx_agent.FlagId, 
                bx_agent.ParentAgent, 
                bx_agent.ParentRate, 
                bx_agent.AgentRate, 
                bx_agent.ReviewRate,
                bx_agent.IsShow,
                bx_agent.IsShowCalc,  
                bx_agent.MessagePayType,   
                bx_agent.Agent_Level,
                bx_agent.IsSubmit,
                (SELECT max(AgentName) FROM bx_agent as m WHERE m.id= bx_agent.ParentAgent )AS ParentAgentName,
                bx_agent.PayType, 
                bx_agent.AgentGetPay, 
                bx_agent.CommissionType, 
                (SELECT max(ShareCode) FROM bx_agent as m WHERE m.id= bx_agent.ParentAgent )AS ParentShareCode,
                bx_agent.IsUsed,
                bx_agent.AgentAccount,
                bx_agent.SecretKey,
                bx_activity_oauthuserinfo.nickname as NickName,
                bx_agent.AgentType,
                bx_agent.IsDaiLi,
                tx_call_phonenum_setting.isUsed as IsGrabOrder,
                bx_agent.charge_person as ChargePerson,
                user.PwdMd5 is not null  as IsHasPwd,
                manager_department.id as department_id,
                manager_department.department_name, 
                bx_agent.zhen_bang_type as ZhenBangType,
	            IF(authen.authen_state=null,-1,authen.authen_state) as AuthenState,
                IF(authen.id is null,0,authen.id) as AuthenId,
                IF(authen.cardholder is null,'',authen.cardholder) as RealName,
                IF(authen.TestState is null,-1,authen.TestState) as TestState
                FROM 
                bx_agent   
                LEFT JOIN bx_activity_oauthuserinfo ON bx_agent.OpenId=bx_activity_oauthuserinfo.openid 
                LEFT JOIN manageruser as user on user.Name=bx_agent.AgentAccount
                LEFT JOIN manager_department  ON user.department_id=manager_department.id
                LEFT JOIN bx_group_authen as authen on bx_agent.id=authen.agentId
                LEFT JOIN  tx_call_phonenum_setting on  bx_agent.id=tx_call_phonenum_setting.agentId
                {2}
                GROUP BY bx_agent.Id
                order by bx_agent.createtime desc  
                 LIMIT {0},{1}";

            const string countSql = @"
                SELECT 	count(distinct bx_agent.Id)as num
                FROM 
                bx_agent
                LEFT JOIN bx_activity_oauthuserinfo ON bx_agent.OpenId=bx_activity_oauthuserinfo.openid
                LEFT JOIN bx_group_authen as authen on bx_agent.id=authen.agentId
                {0}";
            #endregion

            RecordCount = int.Parse(_dbHelper.ExecuteScalar(string.Format(countSql, sqlwhere), parameters.ToArray()).ToString());
            var list = _dbHelper.ExecuteDataSet(string.Format(sql, (PageIndex - 1) * PageSize, PageSize, sqlwhere), parameters.ToArray()).ToList<CustomerModel>();

            //增城人保的认证状态 区分振邦的认证状态
            if (!zcTopAgentId.Contains(TopAgentId.ToString()))
            {
                foreach (var item in list)
                {
                    item.AuthenState = item.AuthenState == 1 ? 1 : 0;  //列表返回所有的认证状态，振邦只有已认证和未认证

                }
            }
            return list;
        }

        public int GetChildAgentCountByTopAgentId(int topAgentId)
        {
            return db.bx_agent.Where(o => o.TopAgentId == topAgentId && o.IsUsed == 1).Count();
        }

        /// <summary>
        /// 查询业务员总数量接口 zky 2017-12-26 /crm(查询条件和上面获取业务员列表 GetAgentList 一致)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Mobile"></param>
        /// <param name="StateDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="IsUsed"></param>
        /// <param name="ParentAgentName"></param>
        /// <param name="ParentAgentId"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="AgentId"></param>
        /// <param name="AuthenState"></param>
        /// <param name="ZhenBangType"></param>
        /// <returns></returns>
        public int GetAgetListCount(string Name, string Mobile, string StateDateTime, string EndDateTime, int IsUsed, string ParentAgentName, string ParentAgentId, int TopAgentId, int AgentId, int AuthenState, int QueryZBType, int OnlySite, int AgentZBType)
        {
            string sqlwhere = string.Empty;
            #region 查询条件
            if (!string.IsNullOrEmpty(Name) && Regex.IsMatch(Name, "^%|%$", RegexOptions.IgnorePatternWhitespace))
            {
                TopAgentId = AgentId = 0;
            }

            if (TopAgentId == AgentId)//顶级账号
            {
                sqlwhere = @"where 1=1 and bx_agent.IsUsed != 3  and bx_agent.TopAgentId=?TopAgentId";
            }
            else//非顶级
            {
                sqlwhere = @"where 1=1 and bx_agent.IsUsed != 3  and (bx_agent.id=?AgentId or bx_agent.ParentAgent=?AgentId)";
            }

            if (IsUsed != -1)
            {
                sqlwhere += " and bx_agent.IsUsed=?IsUsed";
            }
            if (!string.IsNullOrEmpty(ParentAgentName))
            {
                sqlwhere += " and (SELECT max(AgentName) FROM bx_agent as m WHERE m.id= bx_agent.ParentAgent )  like concat('%',?ParentAgentName,'%') ";
            }
            if (!string.IsNullOrEmpty(ParentAgentId))
            {
                sqlwhere += " and bx_agent.ParentAgent in (?ParentAgentId)";
            }
            if (!string.IsNullOrEmpty(Name))
            {
                sqlwhere += " and bx_agent.AgentName  like concat('%',?Name,'%')";
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                sqlwhere += " and bx_agent.Mobile=?Mobile";
            }
            if (!string.IsNullOrEmpty(StateDateTime))
            {
                StateDateTime = Convert.ToDateTime(StateDateTime).ToString("yyyy-MM-dd");
                sqlwhere += " and bx_agent.CreateTime>=?StateDateTime";
            }
            if (!string.IsNullOrEmpty(EndDateTime))
            {
                EndDateTime = Convert.ToDateTime(EndDateTime).AddDays(1).ToString();
                sqlwhere += " and bx_agent.CreateTime<?EndDateTime";
            }

            if (AgentZBType > 0)//振邦账号列表查询需要添加 类型限制
            {
                if (OnlySite > 0)
                {
                    sqlwhere += " and bx_agent.zhen_bang_type=2";
                }
                else
                {
                    sqlwhere += " and bx_agent.zhen_bang_type in (1,3,4)";
                }
            }
            if (QueryZBType > 0)
            {
                sqlwhere += " and bx_agent.zhen_bang_type=?ZhenBangType";
            }

            //增城人保的认证状态 区分振邦的认证状态
            //modify by qidakang zcTopAgentId是以,分割的字符串，跟TopAgentId.ToString()不能用==，只能使用Contains
            if (zcTopAgentId.Contains(TopAgentId.ToString() ))
            {
                if (AuthenState == -1)
                {
                    sqlwhere += "and authen.authen_state is null";
                }
                else if(AuthenState == 0)//modify qidakang 2018-10-10 未认证包含null和0
                {
                    sqlwhere += " and (authen.authen_state is null or authen.authen_state=0)";
                }
                else if ( AuthenState == 1 || AuthenState == 2)
                {
                    sqlwhere += " and authen.authen_state=?AuthenState";
                }
            }
            else
            {
                if (AuthenState == 0)
                {
                    sqlwhere += " and (authen.authen_state is null or authen.authen_state=0)";
                }
                else if (AuthenState == 1)
                {
                    sqlwhere += " and authen.authen_state=1";
                }
            }

            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Name",
                    Value = Name
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "StateDateTime",
                    Value = StateDateTime
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "EndDateTime",
                    Value = EndDateTime
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "ParentAgentName",
                    Value = ParentAgentName
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "IsUsed",
                    Value = IsUsed
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ParentAgentId",
                    Value = ParentAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "AgentId",
                    Value = AgentId
                },
                 new MySqlParameter
                 {
                     MySqlDbType=MySqlDbType.Int32,
                     ParameterName="ZhenBangType",
                     Value=QueryZBType
                 },
                 new MySqlParameter
                 {
                     MySqlDbType=MySqlDbType.Int32,
                     ParameterName="AuthenState",
                     Value=AuthenState
                 }
            };
            #endregion
            const string countSql = @"
                SELECT 	count(distinct bx_agent.Id)as num
                FROM 
                bx_agent
                LEFT JOIN bx_activity_oauthuserinfo ON bx_agent.OpenId=bx_activity_oauthuserinfo.openid
                LEFT JOIN bx_group_authen as authen on bx_agent.id=authen.agentId
                {0}";


            int totalCount = int.Parse(_dbHelper.ExecuteScalar(string.Format(countSql, sqlwhere), parameters.ToArray()).ToString());
            return totalCount;
        }

        /// <summary>
        /// 获取代理人配置信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent_setting GetAgentSettingModel(int agentId)
        {
            return db.bx_agent_setting.Where(t => t.agent_id == agentId).FirstOrDefault();
        }

        /// <summary>
        /// 获取tx_agent表中的顶级信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public tx_agent GetBusinessModel(int topAgentId)
        {
            return db.tx_agent.Where(o => o.AgentId == topAgentId).FirstOrDefault();
        }

        public List<bx_agent> GetAgentsByAgentIdAndModelTypeAndSearchType(int agentId, int modelType, int searchType)
        {
            string sqlSelect = string.Empty;
            if (modelType == 1)
            {
                if (searchType == 1)
                {

                    sqlSelect = string.Format("select aa.Id,aa.AgentName from bx_agent as aa where aa.ParentAgent in( select Id from bx_agent as a where a.group_id={0} and a.zhen_bang_type=1 ) and aa.zhen_bang_type=4", agentId);
                }
                else if (searchType == 2)
                {
                    sqlSelect = string.Format("select aa.Id,aa.AgentName from bx_agent as aa where aa.ParentAgent in( select Id from bx_agent as a where a.group_id={0} and a.zhen_bang_type=1 ) and aa.zhen_bang_type=2", agentId);
                }
                else if (searchType == 3 || searchType == 4)
                {
                    sqlSelect = string.Format("select Id,AgentName from bx_agent as a where a.group_id={0} and a.zhen_bang_type=1", agentId);
                }


            }
            else if (modelType == 2)
            {
                sqlSelect = string.Format("select a.Id,a.AgentName from bx_agent as a where a.group_id={0} and a.zhen_bang_type=1", agentId);

            }
            else if (modelType == 3)
            {
                sqlSelect = string.Format("select a.id,a.AgentName  from bx_agent as a where a.TopAgentId={0}", agentId);

            }
            return _dbHelper.ExecuteDataSet(CommandType.Text, sqlSelect, null).ToList<bx_agent>().ToList(); ;
        }

        public List<bx_agent_config> GetAgentConfigs(int agentId, int modelType)
        {
            var selectSql = string.Empty;
            if (modelType == 1 || modelType == 2)
            {
                selectSql = string.Format(@"select ac.ukey_id,ac.config_name from bx_agent_config as ac where  ac.agent_id in(select a.id from bx_agent as a where a.group_id={0})", agentId);
            }
            else
            {
                selectSql = string.Format(@"select ac.ukey_id,ac.config_name from bx_agent_config as ac where ac.agent_id={0}", agentId);
            }
            return _dbHelper.ExecuteDataSet(CommandType.Text, selectSql, null).ToList<bx_agent_config>().ToList();
        }

        /// <summary>
        /// 根据代理人id获取代理人配置信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent_setting GetAgentSettingModelByAgentId(int agentId)
        {
            return db.bx_agent_setting.Where(t => t.agent_id == agentId).FirstOrDefault();
        }

        /// <summary>
        /// 添加代理人配置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddAgentSetting(bx_agent_setting entity)
        {
            db.bx_agent_setting.Attach(entity);
            db.Entry(entity).State = EntityState.Added;
            return db.SaveChanges() > 0;
        }



        public tx_agent GetTxAgent(int agentId)
        {
            return db.tx_agent.Where(t => t.AgentId == agentId).FirstOrDefault();
        }



        /// <summary>
        /// 获取父级下第一个子级的代理ID
        /// </summary>
        /// <param name="parentAgentId"></param>
        /// <returns></returns>
        public List<int> GetByParentId(int parentAgentId)
        {
            var sql = @"
                    SELECT
                        bx_agent.id               AS AgentId
                    FROM bx_agent
                    WHERE parentagent = ?parentAgent
                        AND isused = 1
                    ORDER BY CreateTime;
                    ";

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="parentAgent",
                    Value=parentAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, param).ToList();
        }

        /// <summary>
        /// 更新代理人账号的启用状态
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        public bool UpdateAgentIsUsed(string agentIds, int isUsed)
        {
            string sql = string.Format("update bx_agent set IsUsed={0} where id in ({1})", isUsed, agentIds);
            return _dbHelper.ExecuteNonQuery(sql) > 0;
        }
        /// <summary>
        /// 验证该顶级Id是否在该集团下
        /// </summary>
        /// <param name="TopAgentId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public bool AgentInGroup(int TopAgentId, int GroupId)
        {
            string sql = string.Format("select count(1) from bx_agent where group_id={0} and Id={1} and agent_level=1 and IsUsed=1", GroupId, TopAgentId);
            return _dbHelper.ExecuteScalar<int>(sql) > 0;
        }





        public int VerificationThirdAccount(int agentId)
        {


            string strSql = string.Format("SELECT bx_agent.ParentAgent FROM bx_agent WHERE  bx_agent.Id={0} AND bx_agent.IsUsed=1 AND bx_agent.agent_level=3 ", agentId);
            //logInfo.Info("VerificationThirdAccount="+strSql+":"+agentId);
            return _dbHelper.ExecuteScalar<int>(strSql);
        }


        public List<ChannelModel> GetListChannel(long ChildAgent)
        {
            var result = new List<ChannelModel>();
            try
            {
                string sql = string.Format("select " +
                           "bac.id as Id,ba.Mobile,ba.AgentName AS NAME,date_format(bac.create_time, '%Y-%m-%d %H:%i') AS CreateTime," +
                           "bac.ukey_id as UkeyId,bac.`source` as Source,bac.agent_id as AgentId,bac.agent_id as OwnerId," +
                           "bac.config_name as ConfigName,bac.is_used AS IsUsed,bx_city.city_name as CityName,bx_city.id as CityId," +
                           "bauk.agent_id AS UkeyOwnerAgentId,IFNULL(bauk.InsuranceUserName,'') AS  InsuranceUserName " +
                           "from bx_agent_config  as bac LEFT JOIN bx_agent_ukey AS bauk ON bauk.id = bac.ukey_id " +
                           "left join  bx_agent as ba on bac.agent_id = ba.Id left join bx_city on bx_city.id = bac.city_id " +
                           "where bac.agent_id ={0} and bac.is_used in (0,1) order by  bac.update_time desc;", ChildAgent);
                result = DataContextFactory.GetDataContext().Database.SqlQuery<ChannelModel>(sql).ToList();

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        public int UpdateChannelIsUesd(AgentUKeyRequest request)
        {
            try
            {
                var model = DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.id == request.ChannelId && x.agent_id == request.ChildAgent).FirstOrDefault();
                if (request.IsUsed == 1)
                {
                    var oldModel = DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == request.ChildAgent && x.city_id == model.city_id && x.source == model.source && x.id != model.id && x.is_used == 1).FirstOrDefault();
                    if (oldModel != null)
                    {
                        return 2;
                    }
                    else
                    {
                        if (model != null)
                        {
                            model.is_used = request.IsUsed == 1 ? 1 : 0;
                            return DataContextFactory.GetDataContext().SaveChanges();
                        }
                    }
                }
                else
                {
                    if (model != null)
                    {
                        model.is_used = request.IsUsed == 1 ? 1 : 0;
                        return DataContextFactory.GetDataContext().SaveChanges();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                logError.Info("更改渠道状态发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -10003;
            }

        }

        /// <summary>
        /// 放在where后面，用于筛选数据是属于当前代理人和他下级代理人的数据
        /// </summary>
        /// <param name="RoleType">角色类型：3系统管理员，4：管理员</param>
        /// <param name="ChildAgent">当前代理人</param>
        /// <param name="Agent">顶级代理人</param>
        /// <returns></returns>
        public string GetAgentWhere(int RoleType, int ChildAgent, int Agent)
        {
            if (RoleType == 4 || RoleType == 3)
            {
                return " AND bx_agent.TopAgentId=" + Agent;
            }
            else
            {
                return string.Format(" AND (bx_agent.ParentAgent={0} OR bx_agent.Id={0}) ", ChildAgent);
            }
        }

        /// <summary>
        /// 查询的时候根据agent集合来查询。
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public string GetConditionSql(Tuple<List<int>, string> tuple)
        {
            string builder = string.Empty;
            List<int> listAgent = tuple.Item1;
            string sqlAgent = tuple.Item2;
            // 只有代理人数量小于2000时才用in，否则用关联bx_agent.
            if (string.IsNullOrEmpty(sqlAgent) && !(listAgent.Count == 1 && listAgent[0] == -1))
            {
                if (listAgent.Any())
                {
                    var agent = string.Join("','", listAgent);
                    builder = string.Format(" AND ui.agent in ('{0}') ", agent);
                }
                else
                {
                    // 某些搜索添加回导致agen为空，这时bx_userinfo将不会走agent的索引，导致全表扫描，所以设置agent='-1'
                    builder = " AND ui.agent ='-1' ";
                }
            }
            return builder;
        }

        public List<string> GetSonLevelAgent(int agentId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT Id FROM bx_agent WHERE Id =(SELECT parentagent FROM bx_agent WHERE Id=@agentId)  AND agent_level>1 ");
            sqlBuilder.Append("UNION ");
            sqlBuilder.Append("SELECT Id FROM bx_agent WHERE Id=@agentId ");
            sqlBuilder.Append("UNION ");
            sqlBuilder.Append("SELECT Id FROM bx_agent WHERE parentagent=@agentId");
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="agentId",
                    Value=agentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(sqlBuilder.ToString(), param).ToList().Select(a => a.ToString()).ToList();
        }

        public List<MobileDefeatAnalyticsVM> GetDefeatAnalytics4Mobile(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName, out int totalCount)
        {
            totalCount = 0;
            var agent = DataContextFactory.GetDataContext().bx_sf_agent.FirstOrDefault(x => x.Id == agentId);
            var agentIds = agent.TopAgentIds;
            if (!string.IsNullOrEmpty(agentIds))
            {
                totalCount = agentIds.Split(',').Length;
                var listIds = new List<string>(agentIds.Split(',')).ConvertAll(i => int.Parse(i));
                if (!string.IsNullOrEmpty(searchText))
                {
                    var agentList = DataContextFactory.GetDataContext().bx_agent.Where(x => listIds.Contains(x.Id) && x.AgentName.Contains(searchText)).Select(x => new { x.Id }).ToList().Select(a => a.Id).ToList<int>();
                    totalCount = agentList.Count;
                    listIds = agentList;
                }
                listIds.Sort();
                listIds = listIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (listIds.Count > 0)
                {
                    var sql = string.Format(@"select a.Id TopAgentId,a.AgentName,t.DefeatCount from bx_agent a LEFT JOIN(
                                            select a.TopAgentId,COUNT(DISTINCT t.BuId) DefeatCount from bx_defeatreasonhistory t LEFT JOIN
                                            bx_agent a on t.AgentId=a.Id
                                            {5}
                                            {4}
                                            where a.TopAgentId in({0}) and t.Deleted=0 and t.CreateTime>='{1}' and t.CreateTime<'{2}' {6} {3}
                                            GROUP BY a.TopAgentId) t
                                            ON a.Id=t.TopAgentId where a.Id in({0})", string.Join(",", listIds), startTime, endTime,
                                            string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'",
                                            string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on u.CategoryInfoId = c.Id",
                                            agent.is_view_all_data == 0 ? "LEFT JOIN bx_userinfo u ON t.BuId=u.Id" : "",
                                            agent.is_view_all_data == 0 ? "and u.LastYearSource=2" : "");
                    var result = _dbHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<MobileDefeatAnalyticsVM>().ToList();
                    return result;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public List<DefeatAnalysis> GetReasonAnalytics4Mobile(int agentId, DateTime startTime, DateTime endTime, string categoryName, int isViewAllData)
        {
            string sql = string.Format(@"select t.DefeatReasonId,t.Count,t.DefeatReason from(select d.DefeatReasonId,count(d.DefeatReasonId) Count,s.DefeatReason,d.BuId,d.CreateTime from bx_defeatreasonhistory d LEFT JOIN bx_agent t on d.AgentId=t.Id
                                        LEFT JOIN bx_defeatreasonsetting s on d.DefeatReasonId=s.Id {5} {4}
                                        where d.CreateTime>='{1}' and d.CreateTime<'{2}' and d.Deleted=0 and t.TopAgentId={0} {6} {3}
                                        group by d.DefeatReasonId ORDER BY Count DESC) t GROUP BY t.BuId ORDER BY t.CreateTime DESC", agentId, startTime, endTime,
                                        string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'",
                                        string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on u.CategoryInfoId = c.Id",
                                        isViewAllData == 0 ? "LEFT JOIN bx_userinfo u ON d.BuId=u.Id" : "",
                                        isViewAllData == 0 ? "and u.LastYearSource=2" : "");

            logInfo.InfoFormat("获取业务统计请求sql ：{0}", sql);
            var defeatanalysis = _sqlhelper.ExecuteDataTable(sql).ToList<DefeatAnalysis>().ToList();
            return defeatanalysis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentAgent">当前代理人</param>
        /// <param name="flag">
        /// 1:二级-->顶级、二级-->三级
        /// 2:二级-->顶级、二级-->三级、二级-->同级二级、二级-->同级二级下的三级
        /// 3:二级-->三级
        /// </param>
        /// <param name="hasSelf"></param>
        /// <returns></returns>
        public List<int> GetJuniorAgentIdList(int currentAgent, int flag, bool hasSelf = true)
        {
            var listAgents = new List<int>();
            var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "curAgent",
                        Value = currentAgent
                    }
                };

            var strSql = new StringBuilder();
            var Agent = GetAgent(currentAgent);
            if (Agent.agent_level == 1)
            {
                return listAgents;
            }
            if (hasSelf)
            {
                strSql.Append(" select SQL_CACHE  ?curAgent ")
                    .Append(" union ");
            }

            if (flag != 3)
            {
                //顶级代理人
                strSql.Append(" SELECT topagentid AS id FROM bx_agent WHERE id=?curAgent AND isused in(1,2)").Append(" UNION ");
            }
            //二级代理人，同顶级的其他二级代理人
            if (Agent.agent_level == 2 && flag == 2)
            {
                strSql.Append(" SELECT id FROM bx_agent WHERE parentagent IN (SELECT topagentid FROM bx_agent WHERE id=?curAgent AND isused in(1,2)) AND agent_level=2 ")
                  .Append(" UNION ")
                  .Append(" SELECT id FROM bx_agent WHERE parentagent IN (SELECT id FROM bx_agent WHERE parentagent IN (SELECT topagentid FROM bx_agent WHERE id=?curAgent AND isused in(1,2)) AND agent_level=2 ) ")
                  .Append(" UNION ");
            }

            //三级代理人
            if (Agent.agent_level == 3)
            {
                strSql.Append(" select id from bx_agent where parentagent in (select parentagent from bx_agent where id=?curAgent and isused in(1,2)) ")
                  .Append(" UNION ");
            }
            strSql.Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused in(1,2) ")
                  .Append(" UNION ")
                  .Append(" SELECT id FROM bx_agent WHERE parentagent IN (SELECT id FROM bx_agent WHERE parentagent=?curAgent AND isused in(1,2)) ")
                  .Append(" UNION ")
                  .Append(" SELECT id FROM bx_agent WHERE parentagent IN ( ")
                  .Append(" SELECT id FROM bx_agent WHERE parentagent IN ( ")
                  .Append(" SELECT id FROM  bx_agent WHERE parentagent=?curAgent AND isused in(1,2)) ) ");
            //LogHelper.Info("GetJuniorAgentIdList=" + strSql.ToString());
            listAgents = DataContextFactory.GetDataContext().Database.SqlQuery<int>(strSql.ToString(), parameters.ToArray()).ToList();
            return listAgents;
        }
        public List<int> GetOtherAgentList(int AgentId, int TopAgentId)
        {
            var listAgents = new List<int>();
            string sql = "SELECT Id FROM bx_agent WHERE TopAgentId=@TopAgentId AND agent_level=2 AND isused in(1,2) AND id<>@AgentId UNION ";
            sql += " SELECT Id FROM bx_agent WHERE isused in(1,2) AND parentagent  IN (SELECT Id FROM bx_agent WHERE TopAgentId=@TopAgentId AND agent_level=2 AND isused in(1,2) AND id<>@AgentId)";

            var parameters = new List<MySqlParameter>(){
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "TopAgentId",
                        Value = TopAgentId
                    },
                    new MySqlParameter
                    {
                        MySqlDbType = MySqlDbType.Int32,
                        ParameterName = "AgentId",
                        Value = AgentId
                    },
                };
            //logInfo.Info("GetOtherAgentList=" + sql + ";AgentId=" + AgentId + ";TopAgentId=" + TopAgentId);
            listAgents = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, parameters.ToArray()).ToList();
            return listAgents;
        }
        /// <summary>
        /// 判断是几级代理人
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent VerifySecond(int agentId, int agentLevel)
        {
            bx_agent model = DataContextFactory.GetDataContext().bx_agent.Where(a => a.Id == agentId && a.agent_level == agentLevel && a.IsUsed == 1).FirstOrDefault();
            return model;
        }
        public bx_agent VerifyLevel(int agentId)
        {
            bx_agent model = DataContextFactory.GetDataContext().bx_agent.Where(a => a.Id == agentId && a.IsUsed == 1).FirstOrDefault();
            return model;
        }
        /// <summary>
        /// 根据顶级获取下级所有管理员Id
        /// </summary>
        /// <param name="topagentid"></param>
        /// <returns></returns>
        public List<int> GetManagerId(int topagentid)
        {
            List<string> list = new List<string>();
            var para = new MySqlParameter
            {
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "topagentid",
                Value = topagentid
            };
            string sql1 = @"SELECT bx_agent.Id FROM bx_agent LEFT JOIN manager_role_db ON bx_agent.ManagerRoleId=manager_role_db.id 
                            WHERE bx_agent.TopAgentId=@topagentid AND bx_agent.IsUsed=1 AND manager_role_db.role_type =4";
            //查询所有已启用的代理人
            IList<int> listint = _dbHelper.ExecuteDataSet(sql1, para).ToList<int>();
            return listint.ToList();
        }
        public IList<bx_agent> GetList(List<long> ids)
        {
            List<int> idss = ids.ConvertAll<int>(a => (int)a);
            return new EntityContext().bx_agent.Where(a => idss.Contains(a.Id)).ToList();
        }
        /// <summary>
        /// 判断代理人中是否有禁用的
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        public bool IsUsedByAgentId(string agentIds)
        {
            try
            {
                string sql = "SELECT COUNT(1) FROM bx_agent WHERE id IN (" + agentIds + ") AND isused=2";
                object obj = _dbHelper.ExecuteScalar(sql);
                return int.Parse(obj.ToString()) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error("IsUsedByAgentId_AgentIds=" + agentIds + "发生异常：" + ex);
            }
            return false;
        }
        /// <summary>
        /// 判断代理人中是否有禁用的
        /// </summary>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        public List<string> GetUsedDisableByAgentId(string agentIds)
        {
            try
            {
                string sql = "SELECT AgentName FROM bx_agent WHERE id IN (" + agentIds + ") AND isused=2";
                return DataContextFactory.GetDataContext().Database.SqlQuery<string>(sql).ToList<string>();
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetUsedDisableByAgentId_AgentIds=" + agentIds + "发生异常：" + ex);
            }
            return null;
        }

        /// <summary>
        /// 往Signal平台推消息
        /// </summary>
        /// <param name="childAgent">被禁用/删除的代理人ID</param>
        /// <param name="isUsed">1：修改账号密码，2：禁用，3：删除</param>
        public void PushSignal(int childAgent, int isUsed)
        {
            var sendModel = new AgentUsedViewModel() { AgentId = childAgent, IsUsed = isUsed };
            string url = string.Format("{0}/api/Message/SendAgentUsedMessage", _messageCenterHost);
            string data = sendModel.ToJson();
            logInfo.Info(string.Format("消息发送SendAgentUsedMessage请求串: url:{0}/api/Message/SendAgentUsedMessage ; data:{1}", _messageCenterHost, data));
            //post消息发送
            string resultMessage = HttpWebAsk.HttpClientPostAsync(data, url);
            logInfo.Info(string.Format("消息发送SendAgentUsedMessage返回值:{0}", resultMessage));
        }

        /// <summary>
        /// 获取短信设置
        /// </summary>
        /// <param name="agentId">经济人ID</param>
        /// <returns></returns>
        public ShortMsgSettingResponse GetShortMsgSetting(int agentId)
        {
            ShortMsgSettingResponse response = new ShortMsgSettingResponse();
            try
            {
                //查询语句
                //根据agentId从bx_agent表中获取phone_is_wechat（手机号是否与微信同号）
                //从bx_agent_setting表中获取StoreName（门店名称）、DisclaimerTips（免责提示）
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("select a.id as AgentId,");
                sbSql.Append("a.phone_is_wechat as PhoneIsWechat,");
                sbSql.Append("s.store_name as StoreName ,");
                sbSql.Append("s.disclaimer_tips as DisclaimerTips ");
                sbSql.Append("from bx_agent as a ");
                sbSql.Append("inner join bx_agent_setting as s ");
                sbSql.Append("on s.agent_id=a.id ");
                sbSql.Append("where a.id=@agentId");

                var para = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                };
                response = DataContextFactory.GetDataContext().Database.SqlQuery<ShortMsgSettingResponse>(sbSql.ToString(), para).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response = null;
                throw ex;
            }
            return response;
        }

        /// <summary>
        /// 设置短信设置
        /// </summary>
        /// <param name="request">短信设置数据模型</param>
        /// <returns></returns>
        public bool SetShortMsgSetting(ShortMsgSettingRequest request)
        {
            bool setResult = false;
            try
            {
                //更新SQL语句
                //根据agentId更新bx_agent表中phone_is_wechat（手机号是否与微信同号）
                //更新bx_agent_setting表中StoreName（门店名称）、DisclaimerTips（免责提示）
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("update bx_agent as a ");
                sbSql.Append("inner join bx_agent_setting as s ");
                sbSql.Append("on s.agent_id=a.Id ");
                sbSql.Append("set a.phone_is_wechat=@phoneIsWechat,");
                sbSql.Append("s.store_name=@storeName,");
                sbSql.Append("s.disclaimer_tips=@disclaimerTips,");
                sbSql.Append("s.modify_time=CURTIME() ");
                sbSql.Append("where a.id=@agentId");

                MySqlParameter[] sqlParams = new MySqlParameter[4];
                sqlParams[0] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "phoneIsWechat",
                    Value = request.PhoneIsWechat
                };
                sqlParams[1] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "storeName",
                    Value = request.StoreName
                };
                sqlParams[2] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "disclaimerTips",
                    Value = request.DisclaimerTips
                };
                sqlParams[3] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = request.AgentId
                };

                int executeRows = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sbSql.ToString(), sqlParams);
                setResult = executeRows == 2 ? true : false;
            }
            catch (Exception ex)
            {
                setResult = false;
                throw ex;
            }

            return setResult;
        }
        public List<bx_agent> GetAgentById(List<int> agentIds)
        {
            try
            {
                return DataContextFactory.GetDataContext().bx_agent.Where(a => agentIds.Contains(a.Id)).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("发送异常：" + ex);
            }
            return new List<bx_agent>();
        }

        public List<int> GetChildAgentIdByTopAgentIds(List<int> topAgentIds)
        {
            var result = db.bx_agent.Where(x => topAgentIds.Contains(x.TopAgentId) && x.IsUsed == 1).Select(x => x.Id).ToList();
            return result;
        }
    }
}
