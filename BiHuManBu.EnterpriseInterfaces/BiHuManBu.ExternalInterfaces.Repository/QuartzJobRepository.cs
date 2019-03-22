using System.Reflection;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.Data;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using Newtonsoft.Json;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuartzJobRepository : IQuartzJobRepository
    {
        readonly MySqlHelper _mySqlHelper;
        readonly string dataBaseName = string.Empty;
        private readonly DateTime _startAnalyticsDay = DateTime.Parse(ConfigurationManager.AppSettings["StartAnalyticsDay"]);
        private readonly int _agentId = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateTopAgent"]);
        private readonly int _num = int.Parse(ConfigurationManager.AppSettings["num"]);
        private EntityContext _dbContext = new EntityContext();
        ILog log = LogManager.GetLogger("ErrorAppender");
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");


        public QuartzJobRepository()
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
            dataBaseName = ConfigurationManager.AppSettings["ChangeDataBaseName"] == "0" ? "bihumanbu_qa" : "bihumanbu";
        }
        #region 回放统计
        public List<CheckedResult_QuartzJob> ConsumerReviewRemindJob()
        {
            using (var _dbContext = new EntityContext())
            {

              
                string sql = @"select  cr.xgAccount as Account , cr.ParentStatus as Status,u.id as BuId,u.agent as AgentId,u.licenseno as Licenseno, TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'),DATE_FORMAT(cr.next_review_date, '%Y-%m-%d %H:%i:%S'))  as RemindMinute  from  bx_userinfo  as u
inner join bx_consumer_review  as cr
on u.id=cr.b_uid  
where   
  cr.next_review_date  <=DATE_FORMAT( DATE_ADD( NOW() , INTERVAL 11 MINUTE), '%Y-%m-%d %H:%i') 
 AND cr.next_review_date>=DATE_FORMAT( NOW(), '%Y-%m-%d %H:%i') 
AND cr.ParentStatus in(10001,20) AND cr.IsDeleted=0
  ";
                return _dbContext.Database.SqlQuery<CheckedResult_QuartzJob>(sql).ToList();
            }
        }
        #endregion

        #region  批量发送短信
        /// 曹晨旭
        /// <summary>
        /// 批量发送短信
        /// </summary>
        public List<BulkSendSms_QuartzJob> BulkSendSmsJob()
        {
            List<BulkSendSms_QuartzJob> sendRequestVmList = new List<BulkSendSms_QuartzJob>();
            using (var _dbContent = new EntityContext())
            {
                string findNeedBulkSendSql = @"select  sbh.Id,sbh.AgentId,sbh.Content as SmsContent,a.TopAgentId from bx_sms_batch_history as sbh left join  bx_agent as a on sbh.AgentId= a.Id   where sbh.status=0 and sbh.isdelete=0 and TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i'),DATE_FORMAT(sbh.sendtime, '%Y-%m-%d %H:%i'))<=5 and  TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i'),DATE_FORMAT(sbh.sendtime, '%Y-%m-%d %H:%i'))>=0";
                sendRequestVmList = _dbContent.Database.SqlQuery<BulkSendSms_QuartzJob>(findNeedBulkSendSql).ToList();
                if (sendRequestVmList.Any())
                {
                    var updateBulkSendStatus = string.Format(@"update bx_sms_batch_history set status=1,SendedCount=WaitToSendCount,WaitToSendCount=0 where id in({0})",
                        string.Join(",", sendRequestVmList.Select(x => x.Id)));
                    var updateAccountContent = string.Format(@"update bx_sms_account_content set sendStatus=1 where batchid in ({0})",
                        string.Join(",", sendRequestVmList.Select(x => x.Id)));
                    _dbContent.Database.ExecuteSqlCommand(updateBulkSendStatus);
                    _dbContent.Database.ExecuteSqlCommand(updateAccountContent);
                }
                return sendRequestVmList;
            }
        }
        #endregion

        #region  任务修复批更新状态
        /// ddl
        /// <summary>
        /// 任务修复批更新状态
        /// </summary>
        public List<long> TaskUpdateBatchRenewalItemStatus()
        {
            List<long> batchRenwalItemIdList = new List<long>();
            using (var _dbContext = new EntityContext())
            {
                batchRenwalItemIdList = _dbContext.Database.SqlQuery<long>("SELECT	b.id FROM	bx_batchrenewal a	LEFT JOIN bx_batchrenewal_item b ON b.BatchId = a.id WHERE a.ItemTaskStatus = 1 AND b.ItemStatus = 3 AND a.IsDelete = 0	AND b.IsDelete = 0 and DATE_ADD(b.UpdateTime,INTERVAL 30 MINUTE) <NOW()").ToList();
                if (batchRenwalItemIdList.Any())
                {
                    _dbContext.Database.ExecuteSqlCommand(string.Format("UPDATE bx_batchrenewal_item SET ItemStatus = -1 WHERE id IN ({0})", string.Join(",", batchRenwalItemIdList)));
                }
                return batchRenwalItemIdList;
            }
        }
        #endregion

        #region  批处理更新状态
        /// 曹晨旭
        /// <summary>
        /// 批处理更新状态
        /// </summary>
        public bool BatchRenewalStatusJob()
        {
            using (_dbContext)
            {
                #region 批量更新信息
                bool isUpdateSuccessbool = false;
                string sql = string.Empty;
                sql = string.Format(@" SELECT batch.Id  AS Id,
        CASE WHEN IFNULL(SUM(item.TreateSuccessedCount),0)<=0 AND IFNULL(SUM(item.TreateFailedCount),0)<=0 AND IFNULL(SUM(item.UntreatedCount),0)<=0 THEN 0 ELSE COUNT(*) END  AS TotalCount,
        CASE WHEN SUM(item.ItemTaskStatus)<=0 THEN 2 ELSE 1 END AS ItemTaskStatus,
        IFNULL(SUM(item.TreateSuccessedCount),0) AS TreateSuccessedCount,
        IFNULL(SUM(item.TreateFailedCount),0) AS TreateFailedCount,
        IFNULL(SUM(item.UntreatedCount),0) AS UntreatedCount 
        FROM  bx_batchrenewal batch
        LEFT JOIN 
        (
        SELECT 
        BatchId,
         CASE WHEN ItemStatus=-1 OR ItemStatus=3 OR ItemStatus=0 THEN 1 ELSE 0 END AS ItemTaskStatus,
         CASE WHEN ItemStatus=1 OR ItemStatus=4 THEN 1 ELSE 0 END AS TreateSuccessedCount,
         CASE WHEN ItemStatus=2  THEN 1 ELSE 0 END AS TreateFailedCount,
         CASE WHEN ItemStatus=-1 OR ItemStatus=3 OR ItemStatus=0 THEN 1 ELSE 0 END AS UntreatedCount 
         FROM bx_batchrenewal_item   
        WHERE  IsDelete=0
        ) AS item ON item.BatchId=batch.Id
        WHERE batch.UntreatedCount>0 AND batch.IsDelete=0
        GROUP BY batch.Id   ");
                //判断
                var countResult = _dbContext.Database.SqlQuery<UpdateBatchRenewal_QuartzJob>(sql).ToList();
                if (countResult.Count() == 0)
                {
                    return true;
                }
                string dbConnectionString = ConfigurationManager.ConnectionStrings["zb"].ConnectionString;
                isUpdateSuccessbool = Convert.ToInt32(BulkUpdateByList<UpdateBatchRenewal_QuartzJob>(countResult, dbConnectionString, "bx_batchrenewal", "Id")) == countResult.Count();
                #endregion
                return isUpdateSuccessbool;
            }
        }
        #region   曹晨旭   这两个方法是 从 MySqlHelper 拿出来的
        public object BulkUpdateByList<T>(IEnumerable<T> enumerable, string connectionString, string tableName, string updateByFiledName) where T : class
        {
            int count = 0;
            if (enumerable == null || enumerable.Count() <= 0) throw new Exception("List无任何数据");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("添加失败！请先设置插入的表名");
            var sql = PrepareSql<T>(enumerable, updateByFiledName, tableName);
            //using (MySqlConnection con = new MySqlConnection(connectionString))
            //{

            //    con.Open();
            //    using (MySqlCommand cmd = new MySqlCommand(sql, con))
            //    {
            //        count = cmd.ExecuteNonQuery();
            //    }
            //}
            count = _dbContext.Database.ExecuteSqlCommand(sql);
            return count;
        }
        private static string PrepareSql<T>(IEnumerable<T> tEnumerable, string updateByFiledName, string updateTableName) where T : class
        {

            var sbUpdateSql = new StringBuilder("Update " + updateTableName + " set");
            var sbWhereSql = new StringBuilder(" Where " + updateByFiledName + " In(");
            var t = typeof(T);
            var properties = t.GetProperties().ToList();
            var updateByFiledIndex = -1;
            var index = -1;
            for (var i = 0; i < properties.Count; i++)
            {
                if (properties[i].Name != updateByFiledName) continue;
                updateByFiledIndex = i;
                break;
            }
            properties.RemoveAt(updateByFiledIndex);
            foreach (var propertie in properties)
            {
                index++;
                sbUpdateSql.Append(" " + propertie.Name + "=case " + updateByFiledName + " ");

                foreach (var item in tEnumerable)
                {
                    if (index == 0)
                    {
                        sbWhereSql.Append("'" +
                                                  item.GetType().GetProperty(updateByFiledName).GetValue(item, null) + "',");
                    }
                    sbUpdateSql.Append(" when " + item.GetType().GetProperty(updateByFiledName).GetValue(item, null) + " Then '" + item.GetType().GetProperty(propertie.Name).GetValue(item, null) + "'");
                }
                sbUpdateSql.Append(" End ,");
            }
            return sbUpdateSql.ToString().Substring(0, sbUpdateSql.ToString().Length - 1) + " " +
                   sbWhereSql.ToString().Substring(0, sbWhereSql.ToString().Length - 1) + ")";

        }
        #endregion
        #endregion

        #region  更新用户信息
        /// 曹晨旭
        /// <summary>
        /// 更新用户信息
        /// </summary>
        public bool UpdateUserInfoJob()
        {
            //设置不启动
            int isTimeSetting = 0;
            //引用数据
            using (_dbContext)
            {
                //续保集合
                List<long> userIdsList = new List<long>();
                #region 1.判断小于4点时执行 2.查询今天内的数据
                var toDayStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                var toDayEndTime = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00");

                #region 时间配置
                var sqlTimeSetting = string.Format(@"  SELECT TimeScope FROM  bx_renewal_time_setting");
                //主键编号集合
                List<string> timeSetting = _dbContext.Database.SqlQuery<string>(sqlTimeSetting).ToList();

                //动态根据时间执行
                for (int i = 0; i < timeSetting.Count; i++)
                {
                    string firsttime = "";
                    string sencondtime = "";
                    firsttime = timeSetting[i].Split('-')[0].ToString();
                    sencondtime = timeSetting[i].Split('-')[1].ToString();
                    if (DateTime.Parse(DateTime.Now.ToShortDateString() + " " + firsttime) <= DateTime.Now & DateTime.Now <= DateTime.Parse(DateTime.Now.ToShortDateString() + " " + sencondtime))
                    {
                        isTimeSetting++;
                    }
                }
                #endregion
                //该时间段执行 如果服务启动在续保时间段内直接执行
                //quartz的XML配置和数据库相关的时间段
                if (isTimeSetting > 0)
                {
                    var sqlDate = string.Format(@"  SELECT Buid FROM  bx_batchrenewal_item WHERE ItemStatus=-1 AND IsDelete=0 AND  IsNew=1 AND  CreateTime   Between '{0}' and '{1}' ", toDayStartTime, toDayEndTime);
                    //主键编号集合
                    userIdsList = _dbContext.Database.SqlQuery<long>(sqlDate).ToList();
                    #region 批量更新信息
                    bool isUpdateSuccess = false;
                    bool isUpdateZeroSuccess = false;
                    string sql = string.Empty;
                    //string sqlUpdateZero = string.Empty;
                    if (userIdsList.Count() <= 0)
                    {
                        return false;
                    }
                    //修改数据三个字段
                    sql = string.Format(@"  Update  bx_userinfo Set UpdateTime=now(), LastYearSource=-1,RenewalStatus=-1,NeedEngineNo=1 Where Id In ({0}) ", string.Join(",", userIdsList));
                    //sqlUpdateZero = string.Format(@"  Update  bx_batchrenewal_item Set ItemStatus=0 Where Buid In ({0}) AND ItemStatus>-1 ", string.Join(",", userIdsList));
                    isUpdateSuccess = _dbContext.Database.ExecuteSqlCommand(sql) > 0 ? true : false;
                    return isUpdateSuccess;
                    #region 去掉修改bx_batchrenewal_item状态为0的操作，避免影响之前批续进度，但是中心在处理完后，会根据buid更新对应的所有数据
                    //修改为0
                    //isUpdateZeroSuccess = _dbContext.Database.ExecuteSqlCommand(sqlUpdateZero) > 0 ? true : false;
                    //return isUpdateZeroSuccess;
                        #endregion
                    #endregion
                }
                else
                {
                    return true;
                }
                #endregion
            }
        }
        #endregion

        #region  从指定的时间开始统计   业务统计
        /// 曹晨旭
        /// <summary>
        /// 从指定的时间开始统计   业务统计   初始化
        /// </summary>
        public void Onload_BusinessStatisticsJob()
        {
            Utils utils = new Utils();
            //指定的时间
            var dataInTimeList = utils.GetDatainTimeList();
            if (_agentId > 0)
            {
                utils.DeleteAnalyticsYuntong(_agentId);
            }
            for (var date = _startAnalyticsDay; date < DateTime.Today; date = date.AddDays(1))
            {
                if (_agentId > 0)
                {
                    utils.InsertEachDayAnalyticsYunTong(date, date.AddDays(1), _agentId);
                }
                //已统计则跳过
                if (dataInTimeList.Exists(i => i.DataInTime == date)) continue;
                utils.InsertEachDayAnalytics(date, date.AddDays(1));
            }
        }
        /// 曹晨旭
        /// <summary>
        /// 从指定的时间开始统计   业务统计   执行、、、、、、、、、、、、、、、、、、、、、、、、、、、
        /// </summary>
        public void Execute_BusinessStatisticsJob(DateTime startTime, DateTime endTime)
        {
            //定时任务到整点执行
            Utils utils = new Utils();
            // 添加今天的统计数据
            utils.InsertTodayAnalytics(startTime, endTime);
        }
        #endregion

        #region  战败的业务统计
        /// 曹晨旭
        /// <summary>
        /// 战败的业务统计      初始化
        /// </summary>
        public void Onload_DefeatStatisticsJob()
        {
            Utils utils = new Utils();
            //指定的时间
            var dataInTimeList = utils.GetDefeatDatainTimeList();
            for (var date = _startAnalyticsDay; date < DateTime.Today; date = date.AddDays(1))
            {
                //已统计则跳过
                if (dataInTimeList.Exists(i => Convert.ToDateTime(i.DataInTime) == date)) continue;
                utils.InsertEachDayDefeatAnalytics(date, date.AddDays(1));
            }
        }
        /// 曹晨旭
        /// <summary>
        /// 战败的业务统计    执行、、、、、、、、、、、、、、、、、、、、、、、、、、、
        /// </summary>
        public void Execute_DefeatStatisticsJob(DateTime startTime, DateTime endTime)
        {
            //定时任务到整点执行
            Utils utils = new Utils();
            // 添加今天的统计数据
            utils.InsertTodayDefeatAnalytics(startTime, endTime);
        }
        #endregion

        #region 运营后台消息发送

        public bool Execute_MesageServiceJob()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //查满足条件的消息Id
            string sqlMsg = "SELECT * FROM bx_message WHERE Msg_Type=0 AND MsgStatus='2' AND Send_Time < CURTIME() ORDER BY id DESC";
            try
            {
                bx_message msg = new bx_message();
                DataContextFactory.GetDataContext().Database.SqlQuery<bx_message>(sqlMsg).FirstOrDefault();

                if (msg == null)
                { //**执行失败
                    log.Info("时间为：" + DateTime.Now + "，未查询到数据");
                }
                //查该消息是否已发送过
                string sqlCount = string.Format("SELECT COUNT(1) FROM bx_msgindex WHERE MsgId={0} AND ReadStatus=0", msg.Id);
                int count;
                count = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sqlCount).FirstOrDefault();

                if (count > 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "已执行过");
                    return false;
                }
                //更新消息状态
                string sqlUpdate = string.Format("UPDATE bx_message SET MsgStatus='1' WHERE id={0}", msg.Id);
                count = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sqlUpdate);
                if (count == 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "更新失败");
                    return false;
                }
                List<ChannelScope> list = new List<ChannelScope>();
                if (string.IsNullOrEmpty(msg.ChannelAndScope))
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "配置无需发送消息");
                    return false;
                }
                list = msg.ChannelAndScope.FromJson<List<ChannelScope>>();
                #region 批量发送短信
                var smsObj = list.FirstOrDefault(l => l.Channel == 3);
                if (smsObj != null)
                {
                    //批量发送短信
                    List<bx_agent> agentlists = new List<bx_agent>();
                    string istest = ConfigurationManager.AppSettings["IsTest"];
                    string sqlAgents = string.Format("SELECT * FROM bx_agent WHERE ParentAgent=0 {0}", istest.Equals("1") ? " and id=102" : string.Empty);
                    agentlists = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent>(sqlAgents).ToList();
                    if (agentlists.Any())
                    {
                        string smsurl = string.Format("{0}/SubmitSms", ConfigurationManager.AppSettings["SmsCenter"]);
                        string smsdata = string.Empty;
                        string smsresult = string.Empty;
                        int x = 0;
                        foreach (var agent in agentlists.Where(l => l.Mobile.Length == 11))
                        {
                            smsdata = string.Format("account={0}&password={1}&mobile={2}&smscontent={3}&businessType={4}", "102-bihu", "16857e4cc146f05cbdf9e7198ba1dffd", agent.Mobile, msg.Url, 3);
                            HttpWebAsk.Post(smsurl, smsdata, out smsresult);
                            x++;
                        }
                        //记录最后一条发短信返回值
                        log.Info("发送" + x + "次短信，调用接口：Url:" + smsurl + "，请求：" + smsdata + "，最后一次返回值：" + smsresult);
                    }
                }
                #endregion
                #region 往crm推消息
                var crmObj = list.FirstOrDefault(l => l.Channel != 3);
                if (crmObj != null)
                {
                    //如果消息记录没在msgindex表中存在，则进行插入操作
                    string msgurl = string.Format("{0}/api/Message/SetMsgAgent", ConfigurationManager.AppSettings["SystemCrmUrl"]);
                    string tmpData =
                        string.Format("MsgId={0}&Agent={1}", msg.Id, msg.Create_Agent_Id);
                    var postData = new
                    {
                        MsgId = msg.Id,
                        Agent = msg.Create_Agent_Id,
                        SecCode = tmpData.GetMd5()
                    };
                    string msgjson = postData.ToJson();
                    var msgresult = HttpWebAsk.HttpClientPostAsync(msgjson, msgurl);
                    log.Info("发送生成消息接口：Url:" + msgurl + "，请求：" + msgjson + "，返回值：" + msgresult);
                }
                return true;

                #endregion
            }
            catch (Exception ex)
            {
                log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return false;
        }

        #endregion

        #region 续保工作统计
        public bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {

            for (var date = dataInTimeStart; dataInTimeStart < dataInTimeEnd; dataInTimeStart = dataInTimeStart.AddDays(1))
            {
                var endDate = dataInTimeStart.AddDays(1);
                List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = endDate } };
                //并行任务
                CarRenewalStatistics(ps);
                AppointmentStatisticsDetail(ps);
                AnswerCallTimesStatisticsDetail(ps);
                //Task.Run(() => CarRenewalStatistics(ps, agentIds));
                //Task.Run(() => AppointmentStatisticsDetail(ps,agentIds));
                //Task.Run(() => AnswerCallTimesStatisticsDetail(ps,agentIds));
            }
            return true;
        }
        void CarRenewalStatistics(List<MySqlParameter> ps)
        {
            #region 统计昨天回访的数据
            var getCustomerRenewalSql = string.Format(@"USE  {0};SELECT * FROM 
(SELECT 
u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,
CASE WHEN  crw2.status=9 AND crw2.singletime IS NOT NULL THEN crw2.singletime ELSE  crw2.create_time END AS StatusCreateTime,
CASE WHEN crw2.status=9 THEN '已出单' WHEN crw2.status IN(4,16) THEN '战败'  WHEN crw2.status=5 THEN '忙碌中待联系' WHEN crw2.status=17 THEN '已报价考虑中（普通）' WHEN crw2.status=13 THEN '已报价考虑中（重点）' WHEN  crw2.status=20 THEN '预约到店' WHEN crw2.status=14 THEN '其他' END AS CustomerStatusName,
crw2.BizEndTime  AS BizEndDate,
DATE_FORMAT(crw2.BizEndTime,'%Y')  AS BizEndDateYear,
crw2.status AS CustomerStatus,
NOW() AS CreateTime,
NOW() AS UpdateTime
FROM  (SELECT crw.* FROM bx_consumer_review AS crw WHERE id IN (SELECT MAX(id) FROM bx_consumer_review AS crw1 WHERE crw1.status!=0 AND  crw1.create_time >=?dateInTimeStart AND crw1.create_time<=?dateInTimeEnd AND crw1.IsDeleted=0 AND crw1.BizEndTime  IS NOT NULL GROUP BY crw1.b_uid))AS crw2  
left JOIN  bx_userinfo AS u 
ON crw2.b_uid=u.id
LEFT JOIN bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  bx_agent AS a 
ON u.Agent=a.Id) AS temp", dataBaseName);
            #endregion
            #region 统计昨天未回访的数据
            var getCustomerNotRenewalSql = string.Format(@"USE  {0};SELECT * FROM (
SELECT 
u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName, u.UpdateTime AS StatusCreateTime,
CASE WHEN DATE_FORMAT(bi2.BizEndDate,'%Y')>DATE_FORMAT(cr.LastBizEndDate,'%Y')THEN bi2.BizEndDate ELSE cr.LastBizEndDate  END   AS BizEndDate,
CASE WHEN DATE_FORMAT(bi2.BizEndDate,'%Y')>DATE_FORMAT(cr.LastBizEndDate,'%Y')THEN  DATE_FORMAT(bi2.BizEndDate,'%Y') ELSE DATE_FORMAT(cr.LastBizEndDate,'%Y')  END   AS BizEndDateYear,
'未跟进'  AS CustomerStatusName,
-1 AS CustomerStatus,
NOW() AS CreateTime,
NOW() AS UpdateTime
FROM (select u1.Id,u1.Agent,u1.CarVIN,u1.EngineNo,u1.CategoryInfoId,u1.UpdateTime,u1.LicenseNo from bx_userinfo as u1  where u1.UpdateTime>=?dateInTimeStart AND u1.UpdateTime<=?dateInTimeEnd AND u1.IsReView=0  )  AS u
LEFT JOIN bx_car_renewal AS cr
ON u.LicenseNo=cr.LicenseNo
LEFT JOIN bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  bx_agent AS a 
ON u.Agent=a.Id
LEFT JOIN 
 bx_batchrenewal_item AS bi2  ON u.id=bi2.buid AND  bi2.isnew=1 AND bi2.isdelete=0) AS temp
WHERE temp.BizEndDate IS NOT NULL AND DATE_FORMAT(temp.BizEndDate,'%Y')!='1900' AND DATE_FORMAT(temp.BizEndDate,'%Y')!='0001'", dataBaseName);
            #endregion
            #region 获得前天得数据
            var getLastTimeDataSql = @"USE bihu_analytics; SELECT Id,BuId,CustomerStatus,DATE_FORMAT(BizEndDate,'%Y') AS BizEndDateYear  FROM tj_reviewdetail_record WHERE  Deleted = 0 AND BuId IN({0})";
            #endregion
            #region 更新前天的数据

            #endregion
            #region 插入昨天数据

            #endregion

            var carRenewalStatisticsRenewalDetailTable = _mySqlHelper.ExecuteDataTable(CommandType.Text, getCustomerRenewalSql, ps.ToArray()).ToList<CarRenewalStatisticsDetailTable>();
            var carRenewalStatisticsNotRenewalDetailTable = _mySqlHelper.ExecuteDataTable(CommandType.Text, getCustomerNotRenewalSql, ps.ToArray()).ToList<CarRenewalStatisticsDetailTable>().ToList().GroupBy(x => new { x.BuId, x.BizEndDateYear }).Select(x => x.OrderByDescending(s => s.BizEndDate).FirstOrDefault());
            var carRenewalStatisticsDetailTable = carRenewalStatisticsRenewalDetailTable.Union(carRenewalStatisticsNotRenewalDetailTable).ToList();
            if (carRenewalStatisticsDetailTable.Any())
            {
                var getLastTimeData = _mySqlHelper.ExecuteDataTable(CommandType.Text, string.Format(getLastTimeDataSql, string.Join(",", carRenewalStatisticsDetailTable.Select(x => x.BuId))), null).ToList<CarRenewalStatisticsDetailTable>().ToList();

                List<long> updateIds = new List<long>();

                if (getLastTimeData.Any())
                {
                    var updateModel = (from a in carRenewalStatisticsDetailTable
                                       join b in getLastTimeData
                                       on new { BuId = a.BuId, BizEndDateYear = a.BizEndDateYear } equals new { BuId = b.BuId, BizEndDateYear = b.BizEndDateYear }
                                       select new { Id = b.Id }).ToList();
                    updateIds = updateModel.Select(x => x.Id).ToList();
                }
                if (updateIds.Any())
                {
                    if (updateIds.Count <= 10000)
                    {
                        var updateSql = @"USE bihu_analytics; UPDATE  tj_reviewdetail_record SET Deleted=1 WHERE id IN ({0})";
                        updateSql = string.Format(updateSql, string.Join(",", updateIds));
                        _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSql);
                    }
                    else
                    {
                        for (int i = 0; i <= updateIds.Count / 10000; i++)
                        {
                            var updateSql = @"USE bihu_analytics; UPDATE  tj_reviewdetail_record SET Deleted=1 WHERE id IN ({0})";
                            updateSql = string.Format(updateSql, string.Join(",", updateIds.Skip(i * 10000).Take(10000)));
                            _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSql);
                        }
                    }
                }


                if (carRenewalStatisticsDetailTable.Count <= 10000)
                {
                    StringBuilder insertCarRenewalStatisticsDetailStringBuilder = new StringBuilder(@" USE bihu_analytics;INSERT  INTO tj_reviewdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,BizEndDate,CustomerName,CustomerStatusName,CategoryName,CustomerStatus,StatusCreateTime,CreateTime,UpdateTime,Deleted)  VALUES ");
                    foreach (var item in carRenewalStatisticsDetailTable)
                    {
                        insertCarRenewalStatisticsDetailStringBuilder.Append(string.Format(" ({0},{1},'{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}','{14}','{15}',0),", item.BuId, item.AgentId, item.AgentName, item.ParentAgentId, item.TopAgentId, item.LicenseNo, item.CarVIN, item.EngineNo, item.BizEndDate, item.CustomerName, item.CustomerStatusName, item.CategoryName, item.CustomerStatus, item.StatusCreateTime, DateTime.Now, DateTime.Now));
                    }
                    var insertCarRenewalStatisticsDetailSql = insertCarRenewalStatisticsDetailStringBuilder.ToString();
                    var insertCount = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertCarRenewalStatisticsDetailSql.Substring(0, insertCarRenewalStatisticsDetailSql.LastIndexOf(",")));
                }
                else
                {
                    for (int i = 0; i <= carRenewalStatisticsDetailTable.Count / 10000; i++)
                    {
                        StringBuilder insertCarRenewalStatisticsDetailStringBuilder = new StringBuilder(@" USE bihu_analytics;INSERT  INTO tj_reviewdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,BizEndDate,CustomerName,CustomerStatusName,CategoryName,CustomerStatus,StatusCreateTime,CreateTime,UpdateTime,Deleted)  VALUES ");
                        foreach (var item in carRenewalStatisticsDetailTable.Skip(i * 10000).Take(10000))
                        {
                            insertCarRenewalStatisticsDetailStringBuilder.Append(string.Format(" ({0},{1},'{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}','{14}','{15}',0),", item.BuId, item.AgentId, item.AgentName, item.ParentAgentId, item.TopAgentId, item.LicenseNo, item.CarVIN, item.EngineNo, item.BizEndDate, item.CustomerName, item.CustomerStatusName, item.CategoryName, item.CustomerStatus, item.StatusCreateTime, DateTime.Now, DateTime.Now));
                        }
                        var insertCarRenewalStatisticsDetailSql = insertCarRenewalStatisticsDetailStringBuilder.ToString();
                        var insertCount = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertCarRenewalStatisticsDetailSql.Substring(0, insertCarRenewalStatisticsDetailSql.LastIndexOf(",")));
                    }

                }


            }


        }
        void AppointmentStatisticsDetail(List<MySqlParameter> ps)
        {
            #region 插入昨天预约数量
            var insertAppointmentStatisticsDetailSql = string.Format(@"USE  bihu_analytics; INSERT  INTO tj_appointmentdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,CustomerName,CategoryName,StatusCreateTime,BizEndDate,CustomerStatusName,CreateTime) 
SELECT * FROM (SELECT u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0   or cs.CategoryInfo is null  THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,crw.create_time AS StatusCreateTime,
crw.BizEndTime   AS BizEndDate,
'预约到店' AS CustomerStatusName ,
NOW() AS CreateTime
FROM {0}.bx_consumer_review AS crw 
Inner JOIN {1}.bx_userinfo AS u
ON crw.b_uid=u.id
LEFT JOIN {2}.bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN {3}.bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  {4}.bx_agent AS a 
ON u.Agent=a.Id

WHERE crw.create_time>=?dateInTimeStart  AND crw.create_time<=?dateInTimeEnd    AND crw.BizEndTime IS NOT NULL AND crw.status=20 ) AS temp", dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName);
            #endregion
            var insertCount = _mySqlHelper.ExecuteNonQuery(insertAppointmentStatisticsDetailSql, ps.ToArray());
        }
        void AnswerCallTimesStatisticsDetail(List<MySqlParameter> ps)
        {

            #region 插入昨天有效通话数
            var insertAnswerCallTimsDetailSql = string.Format(@" USE  bihu_analytics;INSERT  INTO tj_answercalltimesdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,CustomerName,CategoryName,StatusCreateTime,BizEndDate,CustomerStatusName,CreateTime) 
SELECT * FROM (SELECT u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null  THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,rh.createtime AS StatusCreateTime,
CASE WHEN DATE_FORMAT(bi2.BizEndDate, '%Y') > DATE_FORMAT(cr.LastBizEndDate, '%Y')THEN bi2.BizEndDate ELSE cr.LastBizEndDate END   AS BizEndDate,
      CASE
WHEN u.IsReView = 0 THEN '未跟进'
WHEN u.IsReView = 9 THEN '已出单'
WHEN u.IsReView IN(4, 16) THEN '战败'
WHEN u.IsReView = 5 THEN '忙碌中待联系'
WHEN u.IsReView = 17 THEN '已报价考虑中（普通）'
WHEN u.IsReView = 13 THEN '已报价考虑中（重点）'
WHEN u.IsReView = 20 THEN '预约到店'
WHEN u.IsReView = 14 THEN '其他' END
   AS CustomerStatusName,
NOW() AS CreateTime
  FROM bihu_analytics.record_history AS rh
 Inner JOIN  {0}.bx_userinfo AS u
ON rh.BuId = u.id

LEFT JOIN  {1}.bx_car_renewal AS cr
ON u.LicenseNo = cr.LicenseNo
LEFT JOIN  {2}.bx_userinfo_renewal_info AS urio
ON u.Id = urio.b_uid
LEFT JOIN  {3}.bx_customercategories AS cs
ON u.CategoryInfoId = cs.Id
LEFT JOIN   {4}.bx_agent AS a
ON u.Agent =a.Id
LEFT JOIN
 {5}.bx_batchrenewal_item AS bi2  ON u.id=bi2.buid AND  bi2.isnew=1
WHERE rh.createtime >=?dateInTimeStart AND rh.createtime <=?dateInTimeEnd AND rh.AnswerState = 1 ) AS temp
WHERE temp.BizEndDate IS NOT NULL AND DATE_FORMAT(temp.BizEndDate,'%Y')!='1900' AND DATE_FORMAT(temp.BizEndDate,'%Y')!='0001'", dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName);
            #endregion
            var insertCount = _mySqlHelper.ExecuteNonQuery(insertAnswerCallTimsDetailSql, ps.ToArray());

        }

        #endregion
        /// <summary>
        /// 更新过期账号为禁用 zky 2017-12-12
        /// </summary>
        /// <returns></returns>
        public int UpdateExpireIsUsed()
        {
            string agentSql = "update bx_agent set isUsed=2 where accountType=0 and endDate<now() and isUsed=1";
            string tokenSql = "DELETE from bx_agent_token where agentId in (select id from bx_agent where accountType = 0 and endDate<now() and isUsed=1)";
            int token = _dbContext.Database.ExecuteSqlCommand(tokenSql);
            return _dbContext.Database.ExecuteSqlCommand(agentSql);
        }
        #region 深圳人保报表统计

        public bool InitReportsDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {

            for (var date = dataInTimeStart; dataInTimeStart < dataInTimeEnd; dataInTimeStart = dataInTimeStart.AddDays(1))
            {
                var endDate = dataInTimeStart.AddDays(1);
                var dataInTimeStartStr = dataInTimeStart.ToString("yyyy-MM-dd");
                string deleteSql = string.Format(@"DELETE  FROM bihu_analytics.tj_entryfollowupanalysisaboutday where cameratime_year_month_day='{0}';
DELETE  FROM bihu_analytics.tj_entryfollowupanalysisaboutmonth where cameraTime_year_month='{1}';
DELETE  FROM  bihu_analytics.tj_entryfollowupanalysisaboutyear where cameraTime_year='{2}';
DELETE FROM bihu_analytics.tj_entrytimeintervalanalysis where cameratime_year_month_day='{3}';
DELETE FROM bihu_analytics.tj_quoteanalysis where quotetime_year_month_day='{4}';
DELETE FROM bihu_analytics.tj_flowdirectionfromrenbaoanalysis where insuretime_year_month_day='{5}';
DELETE FROM bihu_analytics.tj_flowdirectiontorenbaoanalysis where insuretime_year_month_day='{6}';
DELETE FROM bihu_analytics.tj_flowmonitor where analysisdate_year_month_day='{7}';
", dataInTimeStartStr, dataInTimeStart.ToString("yyyy-MM"), dataInTimeStart.ToString("yyyy"), dataInTimeStartStr, dataInTimeStartStr, dataInTimeStartStr, dataInTimeStartStr, dataInTimeStartStr);
                _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSql, null);

                #region 进场分析
                InitEntryTimeIntervalAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                InitEntryFollowUpAnalysisAboutDay(dataInTimeStart, endDate, topAgentIds);
                InitEntryFollowUpAnalysisAboutMonth(dataInTimeStart, endDate, topAgentIds);
                InitEntryFollowUpAnalysisAboutYear(dataInTimeStart, endDate, topAgentIds);
                #endregion
                #region 报价分析
                InitQuoteAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                #endregion

                #region 流向分析
                InitFlowDirectionFromRenBaoAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                InitFlowDirectionToRenBaoAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                #endregion
                #region 流量监控
                InitFlowMonitorDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                #endregion

            }

            return true;
        }

        public int TaskGetRenewalIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, Dictionary<DateTime, Dictionary<long, long>> dicRenewal)
        {
            string deleteSql = string.Format(@"DELETE FROM bihu_analytics.tj_renewalcount_record where RenewalTime BETWEEN '{0}' AND '{1}';", dataInTimeStart, dataInTimeEnd);
            _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSql, null);
            var insertSql = new StringBuilder("USE BIHU_ANALYTICS;");
            foreach (var item in dicRenewal)
            {
                var agentAndCount = (Dictionary<long, long>)item.Value;
                foreach (var item1 in agentAndCount)
                {
                    insertSql.Append(string.Format(@"insert into tj_renewalcount_record(TopAgentId,RenewalCount,RenewalTime,CreateTime) values({0},{1},'{2}','{3}');", item1.Key, item1.Value, item.Key.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }
            int insertCount = _dbContext.Database.ExecuteSqlCommand(insertSql.ToString());
            return insertCount;
        }

        public bool InitReportsAboutInsureDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {

            for (var date = dataInTimeStart; dataInTimeStart < dataInTimeEnd; dataInTimeStart = dataInTimeStart.AddDays(1))
            {
                var endDate = dataInTimeStart.AddDays(1);
                var dataInTimeStartStr = dataInTimeStart.ToString("yyyy-MM-dd");
                string deleteSql = string.Format(@"
DELETE FROM bihu_analytics.tj_insureadvanceanalysis where insuretime_year_month_day='{0}';
DELETE FROM bihu_analytics.tj_insurebizavganalysis where insuretime_year_month_day='{1}';
DELETE FROM bihu_analytics.tj_insuredistributionanalysis where insuretime_year_month_day='{2}';
DELETE FROM bihu_analytics.tj_insureriskanalysis where insuretime_year_month_day='{3}';
", dataInTimeStartStr, dataInTimeStartStr, dataInTimeStartStr, dataInTimeStartStr);
                
                _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSql, null);
                #region 投保分析
                InitInsureDistributionAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                InitInsureBizAvgAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                InitInsureRiskAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                InitInsureAdvanceAnalysisDataIntoDB(dataInTimeStart, endDate, topAgentIds);
                #endregion
            }
            return true;
        }
        #region 进场分析基础数据入库
        /// <summary>
        /// 进场分析_进场时段分析基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitEntryTimeIntervalAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var firstTimeIntervalStart = dataInTimeStart.AddHours(9);
            var firstTimeIntervalEnd = dataInTimeStart.AddHours(11);
            var secondTimeIntervalEnd = dataInTimeStart.AddHours(13);
            var thirdTimeIntervalEnd = dataInTimeStart.AddHours(17);

//            var insertSql = string.Format(@"insert into bihu_analytics.tj_entrytimeintervalanalysis(topagentid,cameratime_year_month_day,cameratime_year_month,cameratime_year,nine_eleven_cameracount,eleven_thirteen_cameracount,thirteen_seventeen_cameracount,otherhour_cameracount,renbaocameracount,pingancameracount,taipingyangcameracount,guoshoucaicameracount,othersourcecameracount)select 
//a.topagentid as topagentid,
//date(u.CameraTime) as cameratime_year_month_day,
//date_format(u.CameraTime,'%Y-%m') as cameratime_year_month,
//date_format(u.CameraTime,'%Y') as cameratime_year,
//sum(case when u.cameratime>=?firstTimeIntervalStart  and u.cameratime<?firstTimeIntervalEnd then 1 else 0 end) as nine_eleven_cameracount ,
//sum(case when u.cameratime>=?secondTimeIntervalStart and u.cameratime<?secondTimeIntervalEnd then 1 else 0 end) as eleven_thirteen_cameracount,
//sum(case when u.cameratime>=?thirdTimeIntervalStart and u.cameratime<?thirdTimeIntervalEnd then 1 else 0 end) as thirteen_seventeen_cameracount ,
//sum(case when (u.cameratime>= ?otherFirstTimeIntervalStart and u.cameratime<?otherFirstTimeIntervalEnd) or (u.cameratime>=?otherSecondTimeIntervalStart and u.cameratime<?otherSecondTimeIntervalEnd) then 1 else 0 end) as otherhour_cameracount,
//sum(case when u.lastyearsource=2 then 1 else 0 end) as renbaocameracount,
//sum(case when u.lastyearsource=0 then 1 else 0 end) as pingancameracount,
//sum(case when u.lastyearsource=1 then 1 else 0 end) as taipingyangcameracount,
//sum(case when u.lastyearsource=3 then 1 else 0 end) as guoshoucaicameracount,
//sum(case when u.lastyearsource not in(0,1,2,3)  then 1 else 0 end) as othersourcecameracount
//from  {0}.bx_userinfo as u 
//left join {1}.bx_agent as a
//on u.agent=convert(a.id,char)
//where  a.TopAgentId in({2}) and u.IsCamera and u.CameraTime BETWEEN ?allDayStart and ?allDayEnd 
//group by a.topagentid", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            //logError.Error("进场分析_进场时段分析基础数据入库Sql:" + insertSql);
            var ps = new List<MySqlParameter>() {
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "firstTimeIntervalStart", Value = firstTimeIntervalStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "firstTimeIntervalEnd", Value = firstTimeIntervalEnd },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "secondTimeIntervalStart", Value = firstTimeIntervalEnd }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "secondTimeIntervalEnd", Value = secondTimeIntervalEnd },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "thirdTimeIntervalStart", Value = secondTimeIntervalEnd }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "thirdTimeIntervalEnd", Value = thirdTimeIntervalEnd },
                       new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "otherFirstTimeIntervalStart", Value = dataInTimeStart },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "otherFirstTimeIntervalEnd", Value = firstTimeIntervalStart },
                 new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "otherSecondTimeIntervalStart", Value = thirdTimeIntervalEnd },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "otherSecondTimeIntervalEnd", Value = dataInTimeEnd },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "allDayStart", Value = dataInTimeStart },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "allDayEnd", Value = dataInTimeEnd } };
            var selectSql = string.Format(@"select 
a.topagentid as topagentid,
date_format(u.CameraTime,'%Y-%m-%d') as cameratime_year_month_day,
date_format(u.CameraTime,'%Y-%m') as cameratime_year_month,
date_format(u.CameraTime,'%Y') as cameratime_year,
sum(case when u.cameratime>=?firstTimeIntervalStart  and u.cameratime<?firstTimeIntervalEnd then 1 else 0 end) as nine_eleven_cameracount ,
sum(case when u.cameratime>=?secondTimeIntervalStart and u.cameratime<?secondTimeIntervalEnd then 1 else 0 end) as eleven_thirteen_cameracount,
sum(case when u.cameratime>=?thirdTimeIntervalStart and u.cameratime<?thirdTimeIntervalEnd then 1 else 0 end) as thirteen_seventeen_cameracount ,
sum(case when (u.cameratime>= ?otherFirstTimeIntervalStart and u.cameratime<?otherFirstTimeIntervalEnd) or (u.cameratime>=?otherSecondTimeIntervalStart and u.cameratime<?otherSecondTimeIntervalEnd) then 1 else 0 end) as otherhour_cameracount,
sum(case when u.lastyearsource=2 then 1 else 0 end) as renbaocameracount,
sum(case when u.lastyearsource=0 then 1 else 0 end) as pingancameracount,
sum(case when u.lastyearsource=1 then 1 else 0 end) as taipingyangcameracount,
sum(case when u.lastyearsource=3 then 1 else 0 end) as guoshoucaicameracount,
sum(case when u.lastyearsource not in(0,1,2,3)  then 1 else 0 end) as othersourcecameracount
from  {0}.bx_userinfo as u 
left join {1}.bx_agent as a
on u.agent=convert(a.id,char)
where  a.TopAgentId in({2}) and u.IsCamera and u.CameraTime BETWEEN ?allDayStart and ?allDayEnd 
group by a.topagentid", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            try
            {
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_entrytimeintervalanalysis";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logError.ErrorFormat("进场时段分析基础数据入库错误信息:{0}", ex);
            }
            return 0;
        }
        /// <summary>
        /// 进场分析_进场跟进分析天基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitEntryFollowUpAnalysisAboutDay(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {

            var minAnalysisDate = Convert.ToDateTime(ConfigurationManager.AppSettings["minAnalysisDate"]);

            var topAgentIdsStr = string.Join(",", topAgentIds);
            string cameraDaySql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
DATE_FORMAT(u.CameraTime,'%Y-%m-%d') AS cameraTime_year_month_day,
COUNT(DISTINCT(u.Id)) AS cameracount
FROM  {0}.bx_userinfo AS u 
LEFT JOIN {1}.bx_agent AS a  
ON u.Agent=CONVERT(a.Id,CHAR)
WHERE  u.IsCamera AND a.TopAgentId IN({2})  and u.CameraTime>=?cameraTimeStart1 and u.CameraTime<?cameraTimeEnd1
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            string quoteDaySql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT(qh.b_uid)) AS quotecount,
DATE_FORMAT(qh.createtime,'%Y-%m-%d') AS quoteTime_year_month_day
 FROM {0}.bx_userinfo AS u
LEFT JOIN bihustatistics.bx_quote_history AS qh
ON u.Id=qh.b_uid
LEFT JOIN {1}.bx_agent AS a
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart2 and u.CameraTime<?cameraTimeEnd2  AND a.TopAgentId IN({2}) AND qh.createtime>=?createtimeStart and qh.createtime<?createtimeEnd
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            string insureDaySql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT r.ower_id,r.licenseNo,r.CarVIN) AS insurecount,
DATE_FORMAT(r.signing_date,'%Y-%m-%d') AS insuretime_year_month_day
FROM {0}.bx_userinfo AS u
LEFT JOIN {1}.dz_reconciliation AS r
ON r.create_people_id=convert(  u.Agent,SIGNED) AND u.LicenseNo=r.licenseNo AND u.CarVIN=r.CarVIN
LEFT JOIN {2}.bx_agent AS a 
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart3 and u.CameraTime<?cameraTimeEnd3 AND a.TopAgentId IN({3}) AND r.signing_date>?InputDateStart and r.signing_date<?InputDateEnd
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, dataBaseName, topAgentIdsStr);
            var notinsureDay = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT r.ower_id,r.licenseNo,r.CarVIN) AS notinsurecount,
DATE_FORMAT(u.CameraTime,'%Y-%m-%d') AS joinday,
DATE_FORMAT(r.signing_date,'%Y-%m-%d') AS insuretime_year_month_day
FROM {0}.bx_userinfo AS u
LEFT JOIN {1}.dz_reconciliation AS r
ON u.Agent=CONVERT( r.create_people_id,CHAR) AND u.LicenseNo=r.licenseNo AND u.CarVIN=r.CarVIN
LEFT JOIN {2}.bx_agent AS a 
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart4 and u.CameraTime<?cameraTimeEnd4 AND a.TopAgentId IN({3}) AND (r.signing_date<?InputDateStart1 or r.signing_date>?InputDateEnd1) and r.signing_date>=?minInputDate
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, dataBaseName, topAgentIdsStr);

//            var insertSql = string.Format(@"insert into bihu_analytics.tj_entryfollowupanalysisaboutday(topagentId,cameraTime_year_month_day,cameracount,nowquotecount,nowinsurecount,notnowinsurecount)SELECT 
//temp1.topagentId,
//temp1.cameraTime_year_month_day,
//IFNULL(temp1.cameracount,0) AS cameracount,
//IFNULL(temp2.quotecount,0) AS nowquotecount,
//IFNULL(temp3.insurecount ,0) AS nowinsurecount,
//IFNULL(temp4.notinsurecount,0) AS notnowinsurecount
//FROM 
//({0})AS temp1
//LEFT JOIN
//({1}) AS temp2
//ON temp1.topagentId=temp2.topagentId AND temp1.CameraTime_year_month_day=temp2.quoteTime_year_month_day
//LEFT JOIN 
//({2}) AS temp3
//ON temp1.topagentId=temp3.topagentId AND temp1.CameraTime_year_month_day=temp3.insuretime_year_month_day
//LEFT JOIN (
//{3}
//)AS temp4
//ON temp1.topagentId=temp4.topagentId AND temp1.CameraTime_year_month_day=temp4.joinday", cameraDaySql, quoteDaySql, insureDaySql, notinsureDay);
            var selectSql = string.Format(@"SELECT 
temp1.topagentId,
temp1.cameraTime_year_month_day,
IFNULL(temp1.cameracount,0) AS cameracount,
IFNULL(temp2.quotecount,0) AS nowquotecount,
IFNULL(temp3.insurecount ,0) AS nowinsurecount,
IFNULL(temp4.notinsurecount,0) AS notnowinsurecount
FROM 
({0})AS temp1
LEFT JOIN
({1}) AS temp2
ON temp1.topagentId=temp2.topagentId AND temp1.CameraTime_year_month_day=temp2.quoteTime_year_month_day
LEFT JOIN 
({2}) AS temp3
ON temp1.topagentId=temp3.topagentId AND temp1.CameraTime_year_month_day=temp3.insuretime_year_month_day
LEFT JOIN (
{3}
)AS temp4
ON temp1.topagentId=temp4.topagentId AND temp1.CameraTime_year_month_day=temp4.joinday", cameraDaySql, quoteDaySql, insureDaySql, notinsureDay);
            //logError.Error("进场分析_进场跟进分析天基础数据入库Sql:" + insertSql);
            try
            {
                var ps = new List<MySqlParameter> { new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName= "cameraTimeStart1",Value= dataInTimeStart } , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd1", Value = dataInTimeEnd }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart2", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd2", Value = dataInTimeEnd }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeStart", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeEnd", Value = dataInTimeEnd }
            , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart3", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd3", Value = dataInTimeEnd }
                , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateStart", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateEnd", Value = dataInTimeEnd }
              , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart4", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd4", Value = dataInTimeEnd }
                , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateStart1", Value = dataInTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateEnd1", Value = dataInTimeEnd },
            new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="minInputDate",Value=minAnalysisDate} };
                var selectReuslt = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectReuslt.TableName = "bihu_analytics.tj_entryfollowupanalysisaboutday";
                var isSuccess = _mySqlHelper.BulkInsert(selectReuslt);

                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logError.ErrorFormat("进场跟进分析天基础数据入库错误信息:{0}", ex);
            }
            return 0;
        }
        /// <summary>
        /// 进场分析_进场跟进分析月基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始时间</param>
        /// <param name="dataInTimeEnd">统计结束时间</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitEntryFollowUpAnalysisAboutMonth(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var deleteSql = string.Format("delete from bihu_analytics.tj_entryfollowupanalysisaboutmonth  where cameraTime_year_month='{0}'", dataInTimeStart.ToString("yyyy-MM"));

            var IsDelete = _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSql, null);
            DateTime d1 = new DateTime(dataInTimeStart.Year, dataInTimeStart.Month, 1);
            if (dataInTimeStart.Year <= 2017)
            {
                d1 = Convert.ToDateTime(ConfigurationManager.AppSettings["minAnalysisDate"]);
            }
            DateTime d2 = new DateTime(d1.AddMonths(1).Year, d1.AddMonths(1).Month, 1);
            var topAgentIdsStr = string.Join(",", topAgentIds);
            var cameraMonthSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
DATE_FORMAT(u.CameraTime,'%Y-%m') AS cameraTime_year_month,
COUNT(DISTINCT(u.Id)) AS cameracount
FROM  {0}.bx_userinfo AS u 
LEFT JOIN {1}.bx_agent AS a  
ON u.Agent=CONVERT(a.Id,CHAR)
WHERE  u.IsCamera AND a.TopAgentId IN({2})  and u.CameraTime>=?cameraTimeStart1 and u.CameraTime<?cameraTimeEnd1
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            var quoteMonthSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT(qh.b_uid)) AS quotecount,
DATE_FORMAT(qh.createtime,'%Y-%m') AS quoteTime_year_month
 FROM {0}.bx_userinfo AS u
LEFT JOIN bihustatistics.bx_quote_history AS qh
ON u.Id=qh.b_uid
LEFT JOIN {1}.bx_agent AS a
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart2 and u.CameraTime<?cameraTimeEnd2  AND a.TopAgentId IN({2}) AND qh.createtime>=?createtimeStart and qh.createtime<?createtimeEnd
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            var insureMonthSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT r.ower_id,r.licenseNo,r.CarVIN) AS insurecount,
DATE_FORMAT(r.signing_date,'%Y-%m') AS insuretime_year_month
FROM {0}.bx_userinfo AS u
LEFT JOIN {1}.dz_reconciliation AS r
ON r.create_people_id=convert(  u.Agent,SIGNED) AND u.LicenseNo=r.licenseNo AND u.CarVIN=r.CarVIN
LEFT JOIN {2}.bx_agent AS a 
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart3 and u.CameraTime<?cameraTimeEnd3 AND a.TopAgentId IN({3}) AND r.signing_date>?InputDateStart and r.signing_date<?InputDateEnd
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, dataBaseName, topAgentIdsStr);
            var notinsureMonthSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT r.ower_id,r.licenseNo,r.CarVIN) AS notinsurecount,
DATE_FORMAT(u.CameraTime,'%Y-%m') AS joinmonth,
DATE_FORMAT(r.signing_date,'%Y-%m') AS insuretime_year_month
FROM {0}.bx_userinfo AS u
LEFT JOIN {1}.dz_reconciliation AS r
ON r.create_people_id=convert(  u.Agent,SIGNED) AND u.LicenseNo=r.licenseNo AND u.CarVIN=r.CarVIN
LEFT JOIN {2}.bx_agent AS a 
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera and u.CameraTime>=?cameraTimeStart4 and u.CameraTime<?cameraTimeEnd4 AND a.TopAgentId IN({3}) AND (r.signing_date<?InputDateStart1 or r.signing_date>?InputDateEnd1) and r.signing_date>=?minAnalysisDate
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, dataBaseName, topAgentIdsStr);
//            var insertSql = string.Format(@"insert into bihu_analytics.tj_entryfollowupanalysisaboutmonth(topagentId,cameraTime_year_month,cameracount,nowquotecount,nowinsurecount,notnowinsurecount)SELECT 
//temp1.topagentId,
//temp1.cameraTime_year_month,
//IFNULL(temp1.cameracount,0) AS cameracount,
//IFNULL(temp2.quotecount,0) AS nowquotecount,
//IFNULL(temp3.insurecount ,0) AS nowinsurecount,
//IFNULL(temp4.notinsurecount,0) AS notnowinsurecount
//FROM 
//({0})AS temp1
//LEFT JOIN
//({1}) AS temp2
//ON temp1.topagentId=temp2.topagentId AND temp1.cameraTime_year_month=temp2.quoteTime_year_month
//LEFT JOIN 
//({2}) AS temp3
//ON temp1.topagentId=temp3.topagentId AND temp1.cameraTime_year_month=temp3.insuretime_year_month
//LEFT JOIN (
//{3}
//)AS temp4
//ON temp1.topagentId=temp4.topagentId AND temp1.cameraTime_year_month=temp4.joinmonth", cameraMonthSql, quoteMonthSql, insureMonthSql, notinsureMonthSql);
//            logError.Error("进场分析_进场跟进分析月基础数据入库Sql:" + insertSql);
            var selectSql = string.Format(@"SELECT 
temp1.topagentId,
temp1.cameraTime_year_month,
IFNULL(temp1.cameracount,0) AS cameracount,
IFNULL(temp2.quotecount,0) AS nowquotecount,
IFNULL(temp3.insurecount ,0) AS nowinsurecount,
IFNULL(temp4.notinsurecount,0) AS notnowinsurecount
FROM 
({0})AS temp1
LEFT JOIN
({1}) AS temp2
ON temp1.topagentId=temp2.topagentId AND temp1.cameraTime_year_month=temp2.quoteTime_year_month
LEFT JOIN 
({2}) AS temp3
ON temp1.topagentId=temp3.topagentId AND temp1.cameraTime_year_month=temp3.insuretime_year_month
LEFT JOIN (
{3}
)AS temp4
ON temp1.topagentId=temp4.topagentId AND temp1.cameraTime_year_month=temp4.joinmonth", cameraMonthSql, quoteMonthSql, insureMonthSql, notinsureMonthSql);
            try
            {
                var ps = new List<MySqlParameter> {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName= "cameraTimeStart1",Value= d1 } , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd1", Value = d2 },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart2", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd2", Value = d2 },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeStart", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeEnd", Value = d2 }
            , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart3", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd3", Value = d2 }
                , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateStart", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateEnd", Value = d2 }
              , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart4", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd4", Value = d2 }
                , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateStart1", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateEnd1", Value = d2 }
            ,new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="minAnalysisDate",Value=d1} };
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_entryfollowupanalysisaboutmonth";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logError.ErrorFormat("进场跟进分析月基础数据入库错误信息:{0}", ex);
            }
            return 0;
        }
        /// <summary>
        /// 进场分析_进场跟进分析年基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始时间</param>
        /// <param name="dataInTimeEnd">统计结束时间</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitEntryFollowUpAnalysisAboutYear(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var deleteSql = string.Format("delete from bihu_analytics.tj_entryfollowupanalysisaboutyear  where cameraTime_year='{0}'", dataInTimeStart.Year);
            var IsDelete = _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSql, null);
            //TODO:首次以上线时间为年初始值
            DateTime d1 = new DateTime(dataInTimeStart.Year, 1, 1);
            if (dataInTimeStart.Year <= 2017)
            {
                d1 = Convert.ToDateTime(ConfigurationManager.AppSettings["minAnalysisDate"]);
            }
            DateTime d2 = new DateTime(d1.AddYears(1).Year, 1, 1);
            var topAgentIdsStr = string.Join(",", topAgentIds);
            var cameraYearSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
DATE_FORMAT(u.CameraTime,'%Y') AS cameraTime_year,
COUNT(DISTINCT(u.Id)) AS cameracount
FROM  {0}.bx_userinfo AS u 
LEFT JOIN {1}.bx_agent AS a  
ON u.Agent=CONVERT(a.Id,CHAR)
WHERE u.IsCamera AND a.TopAgentId IN({2}) and u.CameraTime>=?cameraTimeStart1 and u.CameraTime<?cameraTimeEnd1
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            var quoteYearSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT(qh.b_uid)) AS quotecount,
DATE_FORMAT(qh.createtime,'%Y') AS quoteTime_year
 FROM {0}.bx_userinfo AS u
LEFT JOIN bihustatistics.bx_quote_history AS qh
ON u.Id=qh.b_uid
LEFT JOIN {1}.bx_agent AS a
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera AND a.TopAgentId IN({2}) AND u.CameraTime>=?cameraTimeStart2 and u.CameraTime<?cameraTimeEnd2 and qh.createtime>=?createtimeStart and qh.createtime<?createtimeEnd
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
            var insureYearSql = string.Format(@"SELECT 
a.TopAgentId AS topagentId,
COUNT(DISTINCT r.ower_id,r.licenseNo,r.CarVIN) AS insurecount,
DATE_FORMAT(r.signing_date,'%Y') AS insuretime_year
FROM {0}.bx_userinfo AS u
LEFT JOIN {1}.dz_reconciliation AS r
ON r.create_people_id=convert(  u.Agent,SIGNED) AND u.LicenseNo=r.licenseNo AND u.CarVIN=r.CarVIN
LEFT JOIN {2}.bx_agent AS a 
ON u.Agent=CONVERT(a.id,CHAR)
WHERE  u.IsCamera AND a.TopAgentId IN({3}) AND u.CameraTime>=?cameraTimeStart3 and u.CameraTime<?cameraTimeEnd3  and r.signing_date>=?InputDateStart and r.signing_date<?InputDateEnd 
GROUP BY a.TopAgentId", dataBaseName, dataBaseName, dataBaseName, topAgentIdsStr);
//            var insertSql = string.Format(@"insert into bihu_analytics.tj_entryfollowupanalysisaboutyear(topagentId,cameraTime_year,cameracount,nowquotecount,nowinsurecount,notnowinsurecount)SELECT 
//temp1.topagentId,
//temp1.cameraTime_year,
//IFNULL(temp1.cameracount,0) AS cameracount,
//IFNULL(temp2.quotecount,0) AS nowquotecount,
//IFNULL(temp3.insurecount ,0) AS nowinsurecount,
//0 as notnowinsurecount
//FROM 
//({0})AS temp1
//LEFT JOIN
//({1}) AS temp2
//ON temp1.topagentId=temp2.topagentId AND temp1.cameraTime_year=temp2.quoteTime_year
//LEFT JOIN 
//({2}) AS temp3
//ON temp1.topagentId=temp3.topagentId AND temp1.cameraTime_year=temp3.insuretime_year", cameraYearSql, quoteYearSql, insureYearSql);
//            logError.Error("进场分析_进场跟进分析年基础数据入库Sql:" + insertSql);
            var selectSql = string.Format(@"SELECT 
temp1.topagentId,
temp1.cameraTime_year,
IFNULL(temp1.cameracount,0) AS cameracount,
IFNULL(temp2.quotecount,0) AS nowquotecount,
IFNULL(temp3.insurecount ,0) AS nowinsurecount,
0 as notnowinsurecount
FROM 
({0})AS temp1
LEFT JOIN
({1}) AS temp2
ON temp1.topagentId=temp2.topagentId AND temp1.cameraTime_year=temp2.quoteTime_year
LEFT JOIN 
({2}) AS temp3
ON temp1.topagentId=temp3.topagentId AND temp1.cameraTime_year=temp3.insuretime_year", cameraYearSql, quoteYearSql, insureYearSql);
            try
            {
                var ps = new List<MySqlParameter> {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName= "cameraTimeStart1",Value= d1 } , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd1", Value = d2 },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart2", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd2", Value = d2 },
                new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeStart", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "createtimeEnd", Value = d2 }
            , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeStart3", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "cameraTimeEnd3", Value = d2 }
                , new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateStart", Value = d1 }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "InputDateEnd", Value = d2 } };
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_entryfollowupanalysisaboutyear";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logError.ErrorFormat("进场跟进分析年基础数据入库错误信息:{0}", ex);
            }
            return 0;
        }
        #endregion
        #region 报价分析基础数据入库
        /// <summary>
        /// 报价分析基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitQuoteAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var firstTimeIntervalStart = dataInTimeStart.AddHours(9);
            var firstTimeIntervalEnd = dataInTimeStart.AddHours(11);
            var secondTimeIntervalEnd = dataInTimeStart.AddHours(13);
            var thirdTimeIntervalEnd = dataInTimeStart.AddHours(17);
//            var insertSql = string.Format(@"
//insert into bihu_analytics.tj_quoteanalysis(topagentid,quotetime_year_month_day,quotetime_year_month, quotetime_year
//,renbaoquotecount,pinganquotecount,taipingyangquotecount,guoshoucaiquotecount,othersourcequotecount,nine_eleven_quotecount,eleven_thirteen_quotecount,thirteen_seventeen_quotecount,otherhour_quotecount)select 
//temp.topagentid,
//date(temp.qoutetime) as quotetime_year_month_day,
//date_format(temp.qoutetime,'%Y-%m') as quotetime_year_month,
//date_format(temp.qoutetime,'%Y') as quotetime_year,
//sum(case when temp.source=2 then 1 else 0 end) as renbaoquotecount,
//sum(case when temp.source=0 then 1 else 0 end) as pinganquotecount,
//sum(case when temp.source=1 then 1 else 0 end) as taipingyangquotecount,
//sum(case when temp.source=3 then 1 else 0 end) as guoshoucaiquotecount,
//sum(case when temp.source not in(0,1,2,3) then 1 else 0 end) as othersourcequotecount,
//sum(case when temp.qoutetime>=?qoutetimeStart1 and temp.qoutetime<?qoutetimeEnd1 then 1 else 0 end) as nine_eleven_quotecount ,
//sum(case when temp.qoutetime>=?qoutetimeStart2 and temp.qoutetime<?qoutetimeEnd2 then 1 else 0 end) as eleven_thirteen_quotecount,
//sum(case when temp.qoutetime>=?qoutetimeStart3 and temp.qoutetime<?qoutetimeEnd3 then 1 else 0 end) as thirteen_seventeen_quotecount ,
//sum(case when (temp.qoutetime>=?qoutetimeStart4 and temp.qoutetime<?qoutetimeEnd4) or (temp.qoutetime>=?qoutetimeStart5 and temp.qoutetime<?qoutetimeEnd5) then 1  else 0 end) as otherhour_quotecount
// from (
//select a.topagentid as topagentid,qh.b_uid as buid ,qh.createtime as qoutetime,
//qh.source as source
//from bihustatistics.bx_quote_history as qh
//left join {0}.bx_agent as a
//on qh.agent=a.id 
//where qh.createtime between ?createtimeStart and ?createtimeEnd and a.topAgentId in({1})
//group by a.topagentid,qh.b_uid,qh.source) temp
//group by temp.topagentid", dataBaseName, string.Join(",", topAgentIds));
            var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeStart1",Value=firstTimeIntervalStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeEnd1",Value=firstTimeIntervalEnd},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeStart2",Value=firstTimeIntervalEnd},
                     new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeEnd2",Value=secondTimeIntervalEnd},
                      new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeStart3",Value=secondTimeIntervalEnd},
                        new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeEnd3",Value=thirdTimeIntervalEnd},
                            new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeStart4",Value=dataInTimeStart},
                               new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeEnd4",Value=firstTimeIntervalStart},
                                   new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeStart5",Value=thirdTimeIntervalEnd},
                               new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="qoutetimeEnd5",Value=dataInTimeEnd},
                                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="createtimeStart",Value=dataInTimeStart},
                               new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="createtimeEnd",Value=dataInTimeEnd},

            };
            var selectSql = string.Format(@"select 
temp.topagentid,
date_format(temp.qoutetime,'%Y-%m-%d') as quotetime_year_month_day,
date_format(temp.qoutetime,'%Y-%m') as quotetime_year_month,
date_format(temp.qoutetime,'%Y') as quotetime_year,
sum(case when temp.source=2 then 1 else 0 end) as renbaoquotecount,
sum(case when temp.source=0 then 1 else 0 end) as pinganquotecount,
sum(case when temp.source=1 then 1 else 0 end) as taipingyangquotecount,
sum(case when temp.source=3 then 1 else 0 end) as guoshoucaiquotecount,
sum(case when temp.source not in(0,1,2,3) then 1 else 0 end) as othersourcequotecount,
sum(case when temp.qoutetime>=?qoutetimeStart1 and temp.qoutetime<?qoutetimeEnd1 then 1 else 0 end) as nine_eleven_quotecount ,
sum(case when temp.qoutetime>=?qoutetimeStart2 and temp.qoutetime<?qoutetimeEnd2 then 1 else 0 end) as eleven_thirteen_quotecount,
sum(case when temp.qoutetime>=?qoutetimeStart3 and temp.qoutetime<?qoutetimeEnd3 then 1 else 0 end) as thirteen_seventeen_quotecount ,
sum(case when (temp.qoutetime>=?qoutetimeStart4 and temp.qoutetime<?qoutetimeEnd4) or (temp.qoutetime>=?qoutetimeStart5 and temp.qoutetime<?qoutetimeEnd5) then 1  else 0 end) as otherhour_quotecount
 from (
select a.topagentid as topagentid,qh.b_uid as buid ,qh.createtime as qoutetime,
qh.source as source
from bihustatistics.bx_quote_history as qh
left join {0}.bx_agent as a
on qh.agent=a.id 
where qh.createtime between ?createtimeStart and ?createtimeEnd and a.topAgentId in({1})
group by a.topagentid,qh.b_uid,qh.source) temp
group by temp.topagentid", dataBaseName, string.Join(",", topAgentIds));
            try
            {
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_quoteanalysis";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);

                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                logError.ErrorFormat("报价分析基础数据入库错误信息:{0}", ex);
            }
            return 0;
        }
        #endregion
        #region 投保分析基础数据入库
        /// <summary>
        /// 投保分析_投保分布分析基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitInsureDistributionAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
//            var insertSql = string.Format(@"insert into bihu_analytics.tj_insuredistributionanalysis(topagentid,islastnewcar,insuretime_year_month_day,insuretime_year_month,insuretime_year,renbaoinsurecount,pinganinsurecount,taipingyanginsurecount,guoshoucaiinsurecount,othersourceinsurecount)select 
//temp.topagentid,
//temp.islastnewcar,
//date(temp.inputdate) as  insuretime_year_month_day,
//date_format(temp.inputdate,'%Y-%m') as  insuretime_year_month,
//date_format(temp.inputdate,'%Y') as  insuretime_year,
//sum(case when temp.company_id=2 then 1 else 0 end) as renbaoinsurecount,
//sum(case when temp.company_id=0 then 1 else 0 end) as pinganinsurecount,
//sum(case when temp.company_id=1 then 1 else 0 end) as taipingyanginsurecount,
//sum(case when temp.company_id=3 then 1 else 0 end) as guoshoucaiinsurecount,
//sum(case when temp.company_id not in(0,1,2,3) then 1 else 0 end) as othersourceinsurecount
//from (
//select 
//r.ower_id as topagentid,
//r.company_id as company_id,
// case when (year(now())-year(r.issuing_date))=1 or (now())-year(r.issuing_date))=0 then 1 else 0 end islastnewcar,
//r.signing_date as inputdate
//from {0}.dz_reconciliation as r
//where r.signing_date>=?inputdateStart and r.signing_date<?inputdateEnd  and r.ower_id in({1}) 
//group by r.ower_id,r.licenseno,r.CarVIN) as temp
//group by temp.topagentid,temp.islastnewcar", dataBaseName, string.Join(",", topAgentIds));
            var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateStart",Value=dataInTimeStart},
                   new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateEnd",Value=dataInTimeEnd},


            };
            var selectSql = string.Format(@"select 
temp.topagentid,
temp.islastnewcar,
date_format(temp.inputdate,'%Y-%m-%d') as  insuretime_year_month_day,
date_format(temp.inputdate,'%Y-%m') as  insuretime_year_month,
date_format(temp.inputdate,'%Y') as  insuretime_year,
sum(case when temp.company_id=2 then 1 else 0 end) as renbaoinsurecount,
sum(case when temp.company_id=0 then 1 else 0 end) as pinganinsurecount,
sum(case when temp.company_id=1 then 1 else 0 end) as taipingyanginsurecount,
sum(case when temp.company_id=3 then 1 else 0 end) as guoshoucaiinsurecount,
sum(case when temp.company_id not in(0,1,2,3) then 1 else 0 end) as othersourceinsurecount
from (
select 
r.ower_id as topagentid,
r.company_id as company_id,
 case when (year(now())-year(r.issuing_date))=1 or (year(now())-year(r.issuing_date))=0 then 1 else 0 end islastnewcar,
r.card_date as inputdate
from {0}.dz_reconciliation as r
where r.create_date>=?inputdateStart and r.create_date<?inputdateEnd  and r.ower_id in({1})

group by r.ower_id,r.licenseno,r.CarVIN,date_format(r.card_date,'%Y-%m-%d')) as temp
group by temp.topagentid,temp.islastnewcar,date_format(temp.inputdate,'%Y-%m-%d')", dataBaseName, string.Join(",", topAgentIds));
            try
            {

                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_insuredistributionanalysis";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }
        /// <summary>
        /// 投保分析_投保结构分析_商业险均单分析基础数据入库(由于每日抓单数据包含多个刷卡日的数据，2018-12-13修改为数据库字段存入总保费)
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitInsureBizAvgAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var ps = new List<MySqlParameter>
            {
                new MySqlParameter {MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateStart" ,Value=dataInTimeStart},
                   new MySqlParameter {MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateEnd" ,Value=dataInTimeEnd}
            };
            //var selectSql = string.Format(@"SELECT 
            //                r.ower_id AS topagentid,
            //                DATE_FORMAT(r.signing_date,'%Y-%m-%d') AS  insuretime_year_month_day,
            //                DATE_FORMAT(r.signing_date,'%Y-%m') AS  insuretime_year_month,
            //                DATE_FORMAT(r.signing_date,'%Y') AS  insuretime_year,
            //                SUM(CASE WHEN r.company_id=2 THEN r.insurance_price ELSE 0 END),
            //                IFNULL(ROUND(SUM(CASE WHEN r.company_id=2 THEN r.insurance_price ELSE 0 END)/SUM(CASE WHEN r.company_id=2 THEN 1 ELSE 0 END)),0) AS renbaobizavg,
            //                IFNULL(ROUND(SUM(CASE WHEN r.company_id=0 THEN r.insurance_price ELSE 0 END)/SUM(CASE WHEN r.company_id=0 THEN 1 ELSE 0 END)),0 )AS pinganbizavg,
            //                IFNULL(ROUND(SUM(CASE WHEN r.company_id=1 THEN r.insurance_price ELSE 0 END)/SUM(CASE WHEN r.company_id=1 THEN 1 ELSE 0 END)),0) AS taipingyangbizavg,
            //                IFNULL(ROUND(SUM(CASE WHEN r.company_id=3 THEN r.insurance_price ELSE 0 END)/SUM(CASE WHEN r.company_id=3 THEN 1 ELSE 0 END)),0) AS guoshoucaibizavg,
            //                IFNULL(ROUND(SUM(CASE WHEN r.company_id NOT IN(0,1,2,3) THEN r.insurance_price ELSE 0 END)/SUM(CASE WHEN r.company_id NOT IN(0,1,2,3) THEN r.insurance_price ELSE 0 END)),0)  AS othersourcebizavg
            //                FROM {0}.dz_reconciliation AS r
            //                WHERE r.insurance_type=2 AND r.create_date>=?inputdateStart AND r.create_date<?inputdateEnd  AND r.ower_id IN({1})
            //                GROUP BY r.ower_id,DATE_FORMAT(r.card_date,'%Y-%m-%d')", dataBaseName, string.Join(",", topAgentIds));
            var selectSql = string.Format(@"SELECT 
                            r.ower_id AS topagentid,
                            DATE_FORMAT(r.card_date,'%Y-%m-%d') AS  insuretime_year_month_day,
                            DATE_FORMAT(r.card_date,'%Y-%m') AS  insuretime_year_month,
                            DATE_FORMAT(r.card_date,'%Y') AS  insuretime_year,
                            IFNULL(SUM(CASE WHEN r.company_id=2 THEN r.insurance_price ELSE 0 END),0) AS renbaobizavg,
                            IFNULL(SUM(CASE WHEN r.company_id=0 THEN r.insurance_price ELSE 0 END),0 )AS pinganbizavg,
                            IFNULL(SUM(CASE WHEN r.company_id=1 THEN r.insurance_price ELSE 0 END),0) AS taipingyangbizavg,
                            IFNULL(SUM(CASE WHEN r.company_id=3 THEN r.insurance_price ELSE 0 END),0) AS guoshoucaibizavg,
                            IFNULL(SUM(CASE WHEN r.company_id NOT IN(0,1,2,3) THEN r.insurance_price ELSE 0 END),0)  AS othersourcebizavg
                            FROM {0}.dz_reconciliation AS r
                            WHERE r.insurance_type=2 AND r.create_date>=?inputdateStart AND r.create_date<?inputdateEnd  AND r.ower_id IN({1})
                            GROUP BY r.ower_id,DATE_FORMAT(r.card_date,'%Y-%m-%d')", dataBaseName, string.Join(",", topAgentIds));
            var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
            selectResult.TableName = "bihu_analytics.tj_insurebizavganalysis";

            var isSuccess = _mySqlHelper.BulkInsert(selectResult);
            return isSuccess;

        }
        /// <summary>
        /// 投保分析_投保结构分析_险别分析基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitInsureRiskAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
//            var insertSql = string.Format(@"insert into bihu_analytics.tj_insureriskanalysis(topagentid,insuretime_year_month_day,insuretime_year_month,insuretime_year,renbaodanjiaoqiang,renbaoinsuranceGroup1,renbaoinsuranceGroup2,renbaoinsuranceGroup3,renbaoohterinsurancegroup,pingandanjiaoqiang,pinganinsuranceGroup1,pinganinsuranceGroup2,pinganinsuranceGroup3,pinganohterinsurancegroup,taipingyangdanjiaoqiang,taipingyanginsuranceGroup1,taipingyanginsuranceGroup2,taipingyanginsuranceGroup3,taipingyangohterinsurancegroup,guoshoucaidanjiaoqiang,guoshoucaiinsuranceGroup1,guoshoucaiinsuranceGroup2,guoshoucaiinsuranceGroup3,guoshoucaiohterinsurancegroup,othersourcedanjiaoqiang,othersourceinsuranceGroup1,othersourceinsuranceGroup2,othersourceinsuranceGroup3,othersourceohterinsurancegroup)select temp.topagentid,temp.insuretime_year_month_day,temp.insuretime_year_month,temp.insuretime_year,temp.renbaodanjiaoqiang,temp.renbaoinsuranceGroup1,temp.renbaoinsuranceGroup2,temp.renbaoinsuranceGroup3,temp.renbaoohterinsurancegroup,temp.pingandanjiaoqiang,temp.pinganinsuranceGroup1,temp.pinganinsuranceGroup2,temp.pinganinsuranceGroup3,temp.pinganohterinsurancegroup,temp.taipingyangdanjiaoqiang,temp.taipingyanginsuranceGroup1,temp.taipingyanginsuranceGroup2,temp.taipingyanginsuranceGroup3,temp.taipingyangohterinsurancegroup,temp.guoshoucaidanjiaoqiang,temp.guoshoucaiinsuranceGroup1,temp.guoshoucaiinsuranceGroup2,temp.guoshoucaiinsuranceGroup3,temp.guoshoucaiohterinsurancegroup,temp.othersourcedanjiaoqiang,temp.othersourceinsuranceGroup1,temp.othersourceinsuranceGroup2,temp.othersourceinsuranceGroup3,temp.othersourceohterinsurancegroup  from(select
//r.ower_id as topagentid,
//date(r.signing_date) as  insuretime_year_month_day,
//date_format(r.signing_date,'%Y-%m') as  insuretime_year_month,
//date_format(r.signing_date,'%Y') as  insuretime_year,
//@insurancesum:=case when bdxz.SanZheBaoFei>0 then 32 else 0 end +case when bdxz.CheSunBaoFei>0 then 8 else 0 end +case when bdxz.SiJiBaoFei>0 then 4 else 0 end +case when bdxz.ChengKeBaoFei>0 then 2 else 0 end +case when bdxz.DaoQiangBaoFei>0 then 1 else 0 end ,
//sum( case when r.company_id=2 and r.bao_dan_type=1 then 1 else 0 end) as renbaodanjiaoqiang,
//sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum=47 then 1 else 0 end) as renbaoinsuranceGroup1,
//sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=40 and @insurancesum<47 then 1 else 0 end ) as renbaoinsuranceGroup2,
//sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=37 and @insurancesum<40 then 1 else 0 end ) as renbaoinsuranceGroup3,
//sum(case when r.company_id=2 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum<32)) then 1 else 0 end ) as  renbaoohterinsurancegroup,

//sum( case when r.company_id=0 and r.bao_dan_type=1 then 1 else 0 end) as pingandanjiaoqiang,
//sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum=47 then 1 else 0 end) as pinganinsuranceGroup1,
//sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=40 and @insurancesum<47 then 1 else 0 end ) as pinganinsuranceGroup2,
//sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=37 and @insurancesum<40 then 1 else 0 end ) as pinganinsuranceGroup3,
//sum(case when r.company_id=0 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum<32)) then 1 else 0 end ) as  pinganohterinsurancegroup,

//sum( case when r.company_id=1 and r.bao_dan_type=1 then 1 else 0 end) as taipingyangdanjiaoqiang,
//sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum=47 then 1 else 0 end) as taipingyanginsuranceGroup1,
//sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=40 and @insurancesum<47 then 1 else 0 end ) as taipingyanginsuranceGroup2,
//sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=37 and @insurancesum<40 then 1 else 0 end ) as taipingyanginsuranceGroup3,
//sum(case when r.company_id=1 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum<32)) then 1 else 0 end ) as  taipingyangohterinsurancegroup,

//sum( case when r.company_id=3 and r.bao_dan_type=1 then 1 else 0 end) as guoshoucaidanjiaoqiang,
//sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum=47 then 1 else 0 end) as guoshoucaiinsuranceGroup1,
//sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=40 and @insurancesum<47 then 1 else 0 end ) as guoshoucaiinsuranceGroup2,
//sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=37 and @insurancesum<40 then 1 else 0 end ) as guoshoucaiinsuranceGroup3,
//sum(case when r.company_id=3 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum<32)) then 1 else 0 end ) as  guoshoucaiohterinsurancegroup,

//sum( case when r.company_id not in(0,1,2,3) and r.bao_dan_type=1 then 1 else 0 end) as othersourcedanjiaoqiang,
//sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum=47 then 1 else 0 end) as othersourceinsuranceGroup1,
//sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=40 and @insurancesum<47 then 1 else 0 end ) as othersourceinsuranceGroup2,
//sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum>=37 and @insurancesum<40 then 1 else 0 end ) as othersourceinsuranceGroup3,
//sum(case when r.company_id not in(0,1,2,3) and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and @insurancesum<32)) then 1 else 0 end ) as  othersourceohterinsurancegroup
//from {0}.dz_reconciliation as r
//left join {1}.dz_baodanxianzhong as bdxz
//on r.baodanxinxi_id=bdxz.BaoDanXinXiId
//where r.signing_date>=?InputDateStart and r.signing_date<?InputDateEnd  and r.ower_id in({2})
//group by r.ower_id)as temp", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="InputDateStart",Value=dataInTimeStart},
                     new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="InputDateEnd",Value=dataInTimeEnd}
            };
            var selectSql = string.Format(@"SELECT r.topagentid,r.insuretime_year_month_day,r.insuretime_year_month,r.insuretime_year,
sum( case when r.company_id=2 and r.bao_dan_type=1 then 1 else 0 end) as renbaodanjiaoqiang,
sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum=47 then 1 else 0 end) as renbaoinsuranceGroup1,
sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=40 and insurancesum<47 then 1 else 0 end ) as renbaoinsuranceGroup2,
sum(case when r.company_id=2 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=32 and insurancesum<40 then 1 else 0 end ) as renbaoinsuranceGroup3,
sum(case when r.company_id=2 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and insurancesum<32)) then 1 else 0 end ) as  renbaoohterinsurancegroup,

sum( case when r.company_id=0 and r.bao_dan_type=1 then 1 else 0 end) as pingandanjiaoqiang,
sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum=47 then 1 else 0 end) as pinganinsuranceGroup1,
sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=40 and insurancesum<47 then 1 else 0 end ) as pinganinsuranceGroup2,
sum(case when r.company_id=0 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=32 and insurancesum<40 then 1 else 0 end ) as pinganinsuranceGroup3,
sum(case when r.company_id=0 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and insurancesum<32)) then 1 else 0 end ) as  pinganohterinsurancegroup,

sum( case when r.company_id=1 and r.bao_dan_type=1 then 1 else 0 end) as taipingyangdanjiaoqiang,
sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum=47 then 1 else 0 end) as taipingyanginsuranceGroup1,
sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=40 and insurancesum<47 then 1 else 0 end ) as taipingyanginsuranceGroup2,
sum(case when r.company_id=1 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=32 and insurancesum<40 then 1 else 0 end ) as taipingyanginsuranceGroup3,
sum(case when r.company_id=1 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and insurancesum<32)) then 1 else 0 end ) as  taipingyangohterinsurancegroup,

sum( case when r.company_id=3 and r.bao_dan_type=1 then 1 else 0 end) as guoshoucaidanjiaoqiang,
sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum=47 then 1 else 0 end) as guoshoucaiinsuranceGroup1,
sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=40 and insurancesum<47 then 1 else 0 end ) as guoshoucaiinsuranceGroup2,
sum(case when r.company_id=3 and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=32 and insurancesum<40 then 1 else 0 end ) as guoshoucaiinsuranceGroup3,
sum(case when r.company_id=3 and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and insurancesum<32)) then 1 else 0 end ) as  guoshoucaiohterinsurancegroup,

sum( case when r.company_id not in(0,1,2,3) and r.bao_dan_type=1 then 1 else 0 end) as othersourcedanjiaoqiang,
sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum=47 then 1 else 0 end) as othersourceinsuranceGroup1,
sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=40 and insurancesum<47 then 1 else 0 end ) as othersourceinsuranceGroup2,
sum(case when r.company_id not in(0,1,2,3) and r.bao_dan_type=0 and r.insurance_type=2 and insurancesum>=32 and insurancesum<40 then 1 else 0 end ) as othersourceinsuranceGroup3,
sum(case when r.company_id not in(0,1,2,3) and (r.bao_dan_type=2 or (r.bao_dan_type=0 and r.insurance_type=2 and insurancesum<32)) then 1 else 0 end ) as  othersourceohterinsurancegroup
 from
(
select
r.ower_id as topagentid,
date_format(r.card_date,'%Y-%m-%d') as  insuretime_year_month_day,
date_format(r.card_date,'%Y-%m') as  insuretime_year_month,
YEAR(r.card_date) as  insuretime_year,r.insurance_type,r.company_id,r.bao_dan_type,
@insurancesum:=case when bdxz.SanZheBaoFei>0 then 32 else 0 end +case when bdxz.CheSunBaoFei>0 then 8 else 0 end +case when bdxz.SiJiBaoFei>0 then 4 else 0 end +case when bdxz.ChengKeBaoFei>0 then 2 else 0 end +case when bdxz.DaoQiangBaoFei>0 then 1 else 0 end as insurancesum
from {0}.dz_reconciliation as r
left join {1}.dz_baodanxianzhong as bdxz
on r.baodanxinxi_id=bdxz.BaoDanXinXiId
where r.create_date>=?InputDateStart and r.create_date<?InputDateEnd  and r.ower_id in({2})) r
group by r.topagentid,r.insuretime_year_month_day", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
            selectResult.TableName = "bihu_analytics.tj_insureriskanalysis";

            var isSuccess = _mySqlHelper.BulkInsert(selectResult);
            //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
            return isSuccess;
        }
        /// <summary>
        /// 投保分析_投保结构分析_提前续保分析基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计开始日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>

        private int InitInsureAdvanceAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
//            var insertSql = string.Format(@"insert into bihu_analytics.tj_insureadvanceanalysis(topagentid,insuretime_year_month_day,insuretime_year_month,insuretime_year,renbao_0to30,renbao_31to60,renbao_61to90,pingan_0to30,pingan_31to60,pingan_61to90,taipingyang_0to30,taipingyang_31to60,taipingyang_61to90,goushoucai_0to30,goushoucai_31to60,goushoucai_61to90,othersource_0to30,othersource_31to60,othersource_61to90)
//select 
//temp.topagentid as topagentid,
//date(temp.inputdate) as  insuretime_year_month_day,
//date_format(temp.inputdate,'%Y-%m') as  insuretime_year_month,
//date_format(temp.inputdate,'%Y') as  insuretime_year,
//sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=2 then 1 else 0  end ) as renbao_0to30,
//sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=2 then 1 else 0  end ) as renbao_31to60,
//sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=2 then 1 else 0  end ) as renbao_61to90,
//sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=0 then 1 else 0  end ) as pingan_0to30,
//sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=0 then 1 else 0  end ) as pingan_31to60,
//sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=0 then 1 else 0  end ) as pingan_61to90,
//sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=1 then 1 else 0  end ) as taipingyang_0to30,
//sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=1 then 1 else 0  end ) as taipingyang_31to60,
//sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=1 then 1 else 0  end ) as taipingyang_61to90,
//sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=3 then 1 else 0  end ) as goushoucai_0to30,
//sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=3 then 1 else 0  end ) as goushoucai_31to60,
//sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=3 then 1 else 0  end ) as goushoucai_61to90,
//sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_0to30,
//sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_31to60,
//sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_61to90
//from (
//select 
//r.ower_id as topagentid,
//r.company_id as company_id,
//case when r.insurance_type=1 then  datediff(bxx.forcestartdate,r.signing_date) else  datediff(bxx.bizstartdate,r.signing_date) end  as  diffdays,
//r.signing_date as inputdate
//from  {0}.dz_reconciliation as r
//left join  {1}.dz_baodanxinxi as bxx
//on  r.baodanxinxi_id=bxx.id
//where r.signing_date>=?inputdateStart and r.signing_date<?inputdateEnd and r.ower_id in({2})
//group by r.ower_id,r.licenseno,r.carvin) as temp
//group by temp.topagentid", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateStart",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateEnd",Value=dataInTimeEnd}

            };
            var selectSql = string.Format(@"select 
temp.topagentid as topagentid,
date_format(temp.inputdate,'%Y-%m-%d') as  insuretime_year_month_day,
date_format(temp.inputdate,'%Y-%m') as  insuretime_year_month,
date_format(temp.inputdate,'%Y') as  insuretime_year,
sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=2 then 1 else 0  end ) as renbao_0to30,
sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=2 then 1 else 0  end ) as renbao_31to60,
sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=2 then 1 else 0  end ) as renbao_61to90,
sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=0 then 1 else 0  end ) as pingan_0to30,
sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=0 then 1 else 0  end ) as pingan_31to60,
sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=0 then 1 else 0  end ) as pingan_61to90,
sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=1 then 1 else 0  end ) as taipingyang_0to30,
sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=1 then 1 else 0  end ) as taipingyang_31to60,
sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=1 then 1 else 0  end ) as taipingyang_61to90,
sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id=3 then 1 else 0  end ) as goushoucai_0to30,
sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id=3 then 1 else 0  end ) as goushoucai_31to60,
sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id=3 then 1 else 0  end ) as goushoucai_61to90,
sum(case when temp.diffdays>=0 and temp.diffdays<=30 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_0to30,
sum(case when temp.diffdays>=31 and temp.diffdays<=60 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_31to60,
sum(case when temp.diffdays>=61 and temp.diffdays<=90 and temp.company_id not in(0,1,2,3) then 1 else 0  end ) as othersource_61to90
from (
select 
r.ower_id as topagentid,
r.company_id as company_id,
case when r.insurance_type=1 then  datediff(bxx.forcestartdate,r.signing_date) else  datediff(bxx.bizstartdate,r.signing_date) end  as  diffdays,
r.card_date as inputdate
from  {0}.dz_reconciliation as r
left join  {1}.dz_baodanxinxi as bxx
on  r.baodanxinxi_id=bxx.id
where r.create_date>=?inputdateStart and r.create_date<?inputdateEnd and r.ower_id in({2})
group by r.ower_id,r.licenseno,r.carvin,date_format(r.card_date,'%Y-%m-%d')) as temp
group by temp.topagentid,date_format(temp.inputdate,'%Y-%m-%d')", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
            var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
            selectResult.TableName = "bihu_analytics.tj_insureadvanceanalysis";

            var isSuccess = _mySqlHelper.BulkInsert(selectResult);
            //var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
            return isSuccess;
        }
        #endregion
        #region 流向分析基础数据入库
        /// <summary>
        /// 流向分析_去年人保本年转化分析
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计开始日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        private int InitFlowDirectionFromRenBaoAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            try
            {

//                var insertSql = string.Format(@"insert into bihu_analytics.tj_flowdirectionfromrenbaoanalysis(topagentid,insuretime_year_month_day,insuretime_year_month,insuretime_year,islastnewcar,renbaocurrentyearinsurecount,pingancurrentyearinsurecount,taipingyangcurrentyearinsurecount,guoshoucaicurrentyearinsurecount,othersourcecurrentyearinsurecount)
//select 
//temp.TopAgentId as topagentid,
//date(temp.InputDate) as   insuretime_year_month_day,
//date_format(temp.InputDate,'%Y-%m') as  insuretime_year_month,
//date_format(temp.InputDate,'%Y') as  insuretime_year,
//temp.islastnewcar,
//sum(case when temp.company_id=2 then 1 else 0 end ) as renbaocurrentyearinsurecount,
//sum(case when temp.company_id=0 then 1 else 0 end) as pingancurrentyearinsurecount,
//sum(case when temp.company_id=1 then 1 else 0 end) as taipingyangcurrentyearinsurecount,
//sum(case when temp.company_id=3 then 1 else 0 end) as guoshoucaicurrentyearinsurecount,
//sum(case when temp.company_id not in(0,1,2,3) then 1 else 0 end) as othersourcecurrentyearinsurecount
//from (
//select 
//r.ower_id as TopAgentId,
//r.signing_date,
//r.company_id,
//case when (year(now())-year(u.registerdate))=1 or (year(now()) - year(u.registerdate))=0 then 1 else 0 end islastnewcar
//from {0}.bx_userinfo as u 
//left join {1}.dz_reconciliation as r
//on r.create_people_id=convert(  u.Agent,SIGNED) and u.LicenseNo=r.licenseNo and u.CarVIN=u.CarVIN
//where u.LastYearSource=2 and   r.ower_id in({2}) and   r.signing_date>=?inputdateStart and r.signing_date<?inputdateEnd GROUP BY r.guid
// ORDER BY r.guid,r.insurance_type) as temp
//group by temp.TopAgentId ,temp.islastnewcar", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
                var selectSql = string.Format(@"select 
temp.TopAgentId as topagentid,
date_format(temp.InputDate,'%Y-%m-%d') as   insuretime_year_month_day,
date_format(temp.InputDate,'%Y-%m') as  insuretime_year_month,
date_format(temp.InputDate,'%Y') as  insuretime_year,
temp.islastnewcar,
sum(case when temp.company_id=2 then 1 else 0 end ) as renbaocurrentyearinsurecount,
sum(case when temp.company_id=0 then 1 else 0 end) as pingancurrentyearinsurecount,
sum(case when temp.company_id=1 then 1 else 0 end) as taipingyangcurrentyearinsurecount,
sum(case when temp.company_id=3 then 1 else 0 end) as guoshoucaicurrentyearinsurecount,
sum(case when temp.company_id not in(0,1,2,3) then 1 else 0 end) as othersourcecurrentyearinsurecount
from (
select 
r.ower_id as TopAgentId,
r.card_date as InputDate,
r.company_id,
case when (year(now())-year(u.registerdate))=1 or (year(now()) - year(u.registerdate))=0 then 1 else 0 end islastnewcar
from {0}.bx_userinfo as u 
left join {1}.dz_reconciliation as r
on r.create_people_id=u.Agent and u.LicenseNo=r.licenseNo and u.CarVIN=u.CarVIN
where u.LastYearSource=2 and   r.ower_id in({2}) and   r.create_date>=?inputdateStart and r.create_date<?inputdateEnd GROUP BY r.guid
 ORDER BY r.guid,r.insurance_type) as temp
group by temp.TopAgentId ,temp.islastnewcar,date_format(temp.InputDate,'%Y-%m-%d')", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
                //logError.Error("流向分析_去年人保本年转化分析执行sql:" + insertSql);
                var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateStart",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateEnd",Value=dataInTimeEnd}

            };
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_flowdirectionfromrenbaoanalysis";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var IsSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {

                throw new Exception();
            }
        }
        private int InitFlowDirectionToRenBaoAnalysisDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            try
            {


//                var insertSql = string.Format(@"insert into bihu_analytics.tj_flowdirectiontorenbaoanalysis(topagentid,insuretime_year_month_day,insuretime_year_month,insuretime_year,islastnewcar,renbaolastyearinsurecount,pinganlastyearinsurecount,taipingyanglastyearinsurecount,guoshoucailastyearinsurecount,othersourcelastyearinsurecount)
//select
//temp.topagentid,
//date(temp.InputDate) as  insuretime_year_month_day,
//date_format(temp.InputDate,'%Y-%m') as  insuretime_year_month,
//date_format(temp.InputDate,'%Y') as  insuretime_year,
//temp.islastnewcar,
//sum(case when temp.LastYearSource=2 then 1 else 0 end ) as renbaolastyearinsurecount,
//sum(case when temp.LastYearSource=0 then 1 else 0 end) as pinganlastyearinsurecount,
//sum(case when temp.LastYearSource=1 then 1 else 0 end) as taipingyanglastyearinsurecount,
//sum(case when temp.LastYearSource=3 then 1 else 0 end) as guoshoucailastyearinsurecount,
//sum(case when temp.LastYearSource not in(0,1,2,3)   OR temp.LastYearSource IS NULL then 1 else 0 end) as othersourcelastyearinsurecount
//from (
//select 
//r.ower_id as topagentid,
//r.signing_date,
//case when (year(now())-year(u.registerdate))=1  or (year(now()) - year(u.registerdate))=0  then 1 else 0 end islastnewcar,
//u.LastYearSource
//from  {0}.dz_reconciliation as r
//left join {1}.bx_userinfo as u
//on r.create_people_id=convert(  u.Agent,SIGNED) and r.licenseNo=u.LicenseNo and r.CarVIN=u.CarVIN
//where r.ower_id in({2}) and    r.company_id=2 and r.signing_date>=?inputdateStart and  r.signing_date<?inputdateEnd GROUP BY r.guid
// ORDER BY r.guid,r.insurance_type ) as temp
//group by temp.topagentid,temp.islastnewcar", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
//                logError.Error("流向分析_本年转入人保上年承保执行sql:" + insertSql);
                var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateStart",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="inputdateEnd",Value=dataInTimeEnd}


            };
                var selectSql = string.Format(@"select
temp.topagentid,
date_format(temp.InputDate,'%Y-%m-%d') as  insuretime_year_month_day,
date_format(temp.InputDate,'%Y-%m') as  insuretime_year_month,
date_format(temp.InputDate,'%Y') as  insuretime_year,
temp.islastnewcar,
sum(case when temp.LastYearSource=2 then 1 else 0 end ) as renbaolastyearinsurecount,
sum(case when temp.LastYearSource=0 then 1 else 0 end) as pinganlastyearinsurecount,
sum(case when temp.LastYearSource=1 then 1 else 0 end) as taipingyanglastyearinsurecount,
sum(case when temp.LastYearSource=3 then 1 else 0 end) as guoshoucailastyearinsurecount,
sum(case when temp.LastYearSource not in(0,1,2,3)   OR temp.LastYearSource IS NULL then 1 else 0 end) as othersourcelastyearinsurecount
from (
select 
r.ower_id as topagentid,
r.card_date as InputDate,
case when (year(now())-year(u.registerdate))=1  or (year(now()) - year(u.registerdate))=0  then 1 else 0 end islastnewcar,
u.LastYearSource
from  {0}.dz_reconciliation as r
left join {1}.bx_userinfo as u
on r.create_people_id=u.Agent and r.licenseNo=u.LicenseNo and r.CarVIN=u.CarVIN
where r.ower_id in({2}) and    r.company_id=2 and r.create_date>=?inputdateStart and  r.create_date<?inputdateEnd GROUP BY r.guid
 ORDER BY r.guid,r.insurance_type ) as temp
group by temp.topagentid,temp.islastnewcar,date_format(temp.InputDate,'%Y-%m-%d')", dataBaseName, dataBaseName, string.Join(",", topAgentIds));
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_flowdirectiontorenbaoanalysis";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var IsSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion
        #region 流量监控基础数据入库

        private int InitFlowMonitorDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            try
            {

                var topAgentIdsStr = string.Join(",", topAgentIds);
                var agentSelect = string.Format(@"select a.Id as id from {0}.bx_agent as a  where a.Id in({1})", dataBaseName, topAgentIdsStr);
                var importDataCountSql = string.Format(@"select 
b.topagentid,
count(bi.Id) as ImportDataCount
from {0}.bx_batchrenewal as b
left join {1}.bx_batchrenewal_item as bi
on b.Id=bi.BatchId
where b.CreateTime>=?CreateTimeStart and b.CreateTime<?CreateTimeEnd and b.TopAgentId in ({2}) and  bi.IsNew=1
group by b.TopAgentId", dataBaseName, dataBaseName, topAgentIdsStr);
                //                var dataInterchangeCountSql = string.Format(@"select 
                //a.topagentid,
                //sum(case when u.LatestRenewalTime>=?LatestRenewalTimeStart1 and u.LatestRenewalTime<?LatestRenewalTimeEnd1 then 1 else 0 end +case when u.LatestQuoteTime>?LatestQuoteTimeStart1 and u.LatestQuoteTime<?LatestQuoteTimeEnd1 then 1 else 0 end) as datainterchangecount
                //from {0}.bx_userinfo as u
                //left join {1}.bx_agent as a 
                //on u.agent=convert (a.id,char)
                //where a.topagentid in({2}) and (u.LatestRenewalTime>=?LatestRenewalTimeStart2 and u.LatestRenewalTime<?LatestRenewalTimeEnd2) or (u.LatestQuoteTime>=?LatestQuoteTimeStart2 and u.LatestQuoteTime<?LatestQuoteTimeEnd2)
                //group by a.topagentid", dataBaseName, dataBaseName, topAgentIdsStr);



                var dataInterchangeCountSql = string.Format(@"SELECT topagentid,SUM(datainterchangecount)datainterchangecount  FROM (
SELECT 
a.topagentid,
count(u.id) AS datainterchangecount
FROM {0}.bx_userinfo AS u FORCE INDEX  (idx_agent_istest_Uptime_RType_IsDistributed_RStatus)
LEFT JOIN {1}.bx_agent AS a 
ON u.agent=CONVERT (a.id,CHAR) 
WHERE a.topagentid IN({2}) 
AND u.LatestRenewalTime>=?LatestRenewalStartTime  AND u.LatestRenewalTime<?LatestRenewalEndTime
group by a.topagentid

UNION ALL
SELECT 
a.topagentid,
count(u.id)AS datainterchangecount
FROM {3}.bx_userinfo AS u FORCE INDEX  (idx_agent_istest_Uptime_RType_IsDistributed_RStatus)
LEFT JOIN {4}.bx_agent AS a 
ON u.agent=CONVERT (a.id,CHAR) 
WHERE a.topagentid IN({5}) 
AND u.LatestQuoteTime>=?LatestQuoteStartTime AND u.LatestQuoteTime<?LatestQuoteEndTime group by a.topagentid  ) as a
group by a.topagentid", dataBaseName, dataBaseName, topAgentIdsStr, dataBaseName, dataBaseName, topAgentIdsStr);


                //                var quotecountSql = string.Format(@"select
                //temp.topagentid,
                //count(temp.buid) as quotecount
                //from (
                //select 
                //a.topagentid as topagentid,
                //qh.b_uid as buid
                //from  bihustatistics.bx_quote_history as qh
                //left  join {0}.bx_agent as a
                //on qh.agent=convert( a.id,char)
                //where qh.createtime >=?createtimeStart1 and qh.createtime <?createtimeEnd1 and a.TopAgentId in({1})
                //group by a.TopAgentId,qh.source,qh.b_uid) as temp
                //group by temp.topagentid", dataBaseName, topAgentIdsStr);



                var quotecountSql = string.Format(@"SELECT
temp.topagentid,
COUNT(temp.buid) AS quotecount
FROM (
SELECT a.TopAgentId,qh.source,qh.b_uid AS buid FROM (
SELECT 
mmm.b_uid AS b_uid,mmm.source ,mmm.agent
FROM  bihustatistics.bx_quote_history AS mmm WHERE mmm.createtime >=?createtimeStart1 AND
 mmm.createtime <?createtimeEnd1)  AS  qh
LEFT  JOIN {0}.bx_agent AS a
ON qh.agent=CONVERT( a.id,CHAR)
WHERE  a.TopAgentId 
 IN({1})
GROUP BY a.TopAgentId,qh.source,qh.b_uid
) AS temp
GROUP BY temp.topagentid", dataBaseName, topAgentIdsStr);

                var conclusionCountSql = string.Format(@"select 
temp.topagentid as topagentid,
count(temp.guid) as conclusionCount
from (
select 
r.ower_id as topagentid,
r.guid
from  {0}.dz_reconciliation as r
where r.signing_date>=?InputDateStart and r.signing_date<?InputDateEnd and r.ower_id in({1})
group by r.guid,r.ower_id) as temp
 group by temp.topagentid", dataBaseName, topAgentIdsStr);

//                var insertSql = string.Format(@"insert into bihu_analytics.tj_flowmonitor(topagentid,analysisdate_year_month_day,analysisdate_year_month,analysisdate_year,importdatacount,datainterchangecount,quotecount,conclusionCount)
//select 
//temp1.id as topagentid,
//DATE_FORMAT('{0}','%Y-%m-%d') as analysisdate_year_month_day,
//DATE_FORMAT('{1}','%Y-%m') as analysisdate_year_month,
//DATE_FORMAT('{2}','%Y') as analysisdate_year,
//IFNULL(temp2.ImportDataCount,0) as importdatacount,
//IFNULL(temp3.datainterchangecount,0) as datainterchangecount,
//IFNULL(temp4.quotecount,0)as quotecount,
//IFNULL(temp5.conclusionCount,0) as conclusionCount
//from (
//{3}
//) as temp1
//left join (
//{4}
//) as temp2
//on temp1.id=temp2.topagentid
//left join (
//{5}
//) as temp3
//on temp1.id=temp3.topagentid
//left join (
//{6}
//) as temp4
//on temp1.id=temp4.topagentid
//left join (
//{7}
//) as temp5
// on temp1.id=temp5.topagentid
// ", dataInTimeStart, dataInTimeStart, dataInTimeStart, agentSelect, importDataCountSql, dataInterchangeCountSql, quotecountSql, conclusionCountSql);
                var ps = new List<MySqlParameter>
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="CreateTimeStart",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="CreateTimeEnd",Value=dataInTimeEnd},
                     new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="LatestRenewalStartTime",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="LatestRenewalEndTime",Value=dataInTimeEnd},
                             new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="LatestQuoteStartTime",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="LatestQuoteEndTime",Value=dataInTimeEnd},
                                            new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="createtimeStart1",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="createtimeEnd1",Value=dataInTimeEnd},
                                        new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="InputDateStart",Value=dataInTimeStart},
                    new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="InputDateEnd",Value=dataInTimeEnd}

            };
                var selectSql = string.Format(@"select 
temp1.id as topagentid,
DATE_FORMAT('{0}','%Y-%m-%d') as analysisdate_year_month_day,
DATE_FORMAT('{1}','%Y-%m') as analysisdate_year_month,
DATE_FORMAT('{2}','%Y') as analysisdate_year,
IFNULL(temp2.ImportDataCount,0) as importdatacount,
IFNULL(temp3.datainterchangecount,0) as datainterchangecount,
IFNULL(temp4.quotecount,0)as quotecount,
IFNULL(temp5.conclusionCount,0) as conclusionCount
from (
{3}
) as temp1
left join (
{4}
) as temp2
on temp1.id=temp2.topagentid
left join (
{5}
) as temp3
on temp1.id=temp3.topagentid
left join (
{6}
) as temp4
on temp1.id=temp4.topagentid
left join (
{7}
) as temp5
 on temp1.id=temp5.topagentid", dataInTimeStart, dataInTimeStart, dataInTimeStart, agentSelect, importDataCountSql, dataInterchangeCountSql, quotecountSql, conclusionCountSql);
                //logError.Error("流量监控执行Sql:" + selectSql+"；参数为："+JsonConvert.SerializeObject(ps));
                var selectResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectSql, ps.ToArray());
                selectResult.TableName = "bihu_analytics.tj_flowmonitor";
                var isSuccess = _mySqlHelper.BulkInsert(selectResult);
                //var IsSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertSql, ps.ToArray()) > 0 ? 1 : 0;
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion

        #region 进店数据详情
        public int InitEntryDetails(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var sql = string.Format(@"delete from bihu_analytics.tj_entrydetails where CreateTime>='{1}' and CreateTime<'{2}';
                                    INSERT INTO bihu_analytics.tj_entrydetails(BuId,AgentId,TopAgentId,LicenseNo,CategoryInfoId,IsQuoteInTheDay,IsRenBaoQuote,LastYearSource,CameraTime,LastBizEndDate,LastForceEndDate,CreateTime,IsDistributed,IsReView)
                                    SELECT u.Id BuId,a.Id AgentId,a.TopAgentId,u.LicenseNo,u.CategoryInfoId,CASE WHEN q1.b_uid IS NOT NULL THEN 1 ELSE 0 END IsQuoteInTheDay,
                                    CASE WHEN q2.b_uid IS NOT NULL THEN 1 ELSE 0 END IsRenBaoQuote,
                                    u.LastYearSource,u.CameraTime,r.LastBizEndDate,r.LastForceEndDate,'{3}',u.IsDistributed,CASE WHEN cv.id IS NOT NULL THEN 1 ELSE 0 END IsReView
                                    from bx_userinfo u
                                    LEFT JOIN bx_agent a
                                    on u.Agent=CONVERT(a.Id,CHAR)
                                    LEFT JOIN (SELECT DISTINCT(b_uid) id from bx_consumer_review c where c.create_time>='{1}' AND c.create_time<'{2}') cv ON u.Id=cv.id
                                    LEFT JOIN
                                    (select MAX(q.id),q.b_uid from bihustatistics.bx_quote_history q LEFT JOIN bx_agent a ON q.agent=a.Id
                                    where a.TopAgentId in({0}) AND q.createtime>='{1}' and q.createtime<'{2}' 
                                    GROUP BY q.b_uid) q1
                                    on u.Id=q1.b_uid
                                    LEFT JOIN
                                    (select MAX(q.id),q.b_uid from bihustatistics.bx_quote_history q LEFT JOIN bx_agent a ON q.agent=a.Id
                                    where a.TopAgentId in({0}) AND q.source=2 AND q.createtime>='{1}' and q.createtime<'{2}' 
                                    GROUP BY q.b_uid) q2
                                    on u.Id=q2.b_uid
                                    left join bx_userinfo_renewal_index b
                                    on u.Id=b.b_uid
                                    LEFT JOIN bx_car_renewal AS r
                                    ON b.car_renewal_id=r.Id
                                    where a.TopAgentId in({0}) and
                                    u.IsCamera and u.CameraTime>='{1}' and u.CameraTime<'{2}' and u.IsTest=0", string.Join(",", topAgentIds), dataInTimeStart, dataInTimeEnd, dataInTimeStart.ToString("yyyy-MM-dd"));
            return _dbContext.Database.ExecuteSqlCommand(sql);
        }
        #endregion
        #endregion

        #region 深分统计日常工作
        public int InitDailyWork(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds)
        {
            var sql = string.Format(@"delete from bihu_analytics.tj_sf_dailywork where data_in_time>='{1}' and data_in_time<'{2}';
                                    INSERT into bihu_analytics.tj_sf_dailywork(top_agent_id,quote_car_count,category_quote_car_count,review_count,category_review_count,call_count,category_call_count,create_time,data_in_time,last_year_source)
                                    select t.Id,t1.QuoteCount,t2.CategoryQuoteCount,t3.ReviewCount,t4.CategoryReviewCount,t5.CallCount,t6.CategoryCallCount,now(),'{3}',t.LastYearSource
                                    from (SELECT a.Id,0 LastYearSource from bx_agent a where a.id in({0}) UNION SELECT b.Id,2 LastYearSource from bx_agent b where b.id in({0})) t LEFT JOIN
                                    (
                                    /* 当天报价台次 */
                                    select TopAgentId,COUNT(DISTINCT b_uid) QuoteCount,LastYearSource from(
                                    select a.TopAgentId,t.b_uid,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource
                                    from bihustatistics.bx_quote_history t 
                                    LEFT JOIN bx_agent a ON t.agent=a.id  
                                    LEFT JOIN bx_userinfo u ON t.b_uid=u.Id
                                    WHERE a.TopAgentId in({0}) 
                                    and t.createtime>='{1}' and t.createtime<'{2}') t group by TopAgentId,LastYearSource
                                    ) t1
                                    on t.Id=t1.TopAgentId and t.LastYearSource=t1.LastYearSource
                                    LEFT JOIN
                                    (
                                    /* 当天在修不在保报价台次 */
                                    select TopAgentId,COUNT(DISTINCT b_uid) CategoryQuoteCount,LastYearSource from(
                                    select a.TopAgentId,t.b_uid,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource
                                    from bihustatistics.bx_quote_history t
                                    LEFT JOIN bx_userinfo u on t.b_uid=u.Id
                                    LEFT JOIN bx_customercategories c on u.CategoryInfoId=c.Id
                                    LEFT JOIN bx_agent a ON t.agent=a.id                
                                    WHERE a.TopAgentId in({0}) 
                                    and t.createtime>='{1}' and t.createtime<'{2}' 
                                    and c.CategoryInfo='在修不在保') t group by TopAgentId,LastYearSource
                                    ) t2
                                    ON t.Id=t2.TopAgentId and t.LastYearSource=t2.LastYearSource
                                    LEFT JOIN
                                    (
                                    /* 当天回访数 */
                                    select TopAgentId,count(id) ReviewCount,LastYearSource from(
                                    select c.id,a.TopAgentId,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource from bx_consumer_review c
                                    LEFT JOIN
                                    bx_userinfo u
                                    ON c.b_uid=u.Id
                                    LEFT JOIN bx_agent a
                                    ON convert(u.Agent,SIGNED)=a.Id
                                    where a.TopAgentId in({0}) AND c.create_time>='{1}' and c.create_time<'{2}') t
                                    GROUP BY TopAgentId,LastYearSource
                                    ) t3
                                    on t.Id=t3.TopAgentId and t.LastYearSource=t3.LastYearSource
                                    LEFT JOIN
                                    (
                                    /* 当天在修不在保回访数 */
                                    select TopAgentId,count(id) CategoryReviewCount,LastYearSource from(
                                    select c.id,a.TopAgentId,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource from bx_consumer_review c
                                    LEFT JOIN
                                    bx_userinfo u
                                    ON c.b_uid=u.Id
                                    LEFT JOIN bx_agent a
                                    ON convert(u.Agent,SIGNED)=a.Id
                                    LEFT JOIN bx_customercategories c1
                                    on u.CategoryInfoId=c1.Id
                                    where a.TopAgentId in({0}) AND c.create_time>='{1}' and c.create_time<'{2}' and c1.CategoryInfo='在修不在保') t
                                    GROUP BY TopAgentId,LastYearSource
                                    ) t4
                                    on t.Id=t4.TopAgentId and t.LastYearSource=t4.LastYearSource
                                    LEFT JOIN
                                    (
                                    /* 当天呼叫数 */
                                    select TopAgentId,count(Id) CallCount,LastYearSource from(
                                    select TopAgentId,t.Id,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource from bihu_analytics.record_history t
                                    LEFT JOIN
                                    bx_userinfo u
                                    ON t.BuId=u.Id
                                    where TopAgentId in({0}) and t.AnswerState=1 
                                    and t.CreateTime>='{1}' and t.CreateTime<'{2}') t GROUP BY TopAgentId,LastYearSource
                                    ) t5
                                    on t.Id=t5.TopAgentId and t.LastYearSource=t5.LastYearSource
                                    LEFT JOIN
                                    (
                                    /* 当天在修不在保呼叫数 */
                                    select TopAgentId,count(Id) CategoryCallCount,LastYearSource from(
                                    select TopAgentId,t.Id,CASE WHEN u.LastYearSource=2 THEN 2 ELSE 0 END LastYearSource from bihu_analytics.record_history t
                                    LEFT JOIN bx_userinfo u on t.BuId=u.Id
                                    LEFT JOIN bx_customercategories c
                                    on u.CategoryInfoId=c.Id
                                    where TopAgentId in({0}) and t.AnswerState=1 
                                    and t.CreateTime>='{1}' and t.CreateTime<'{2}' and c.CategoryInfo='在修不在保') t GROUP BY TopAgentId,LastYearSource
                                    ) t6
                                    on t.Id=t6.TopAgentId and t.LastYearSource=t6.LastYearSource", string.Join(",", topAgentIds), dataInTimeStart, dataInTimeEnd, dataInTimeStart.ToString("yyyy-MM-dd"));


            return _dbContext.Database.ExecuteSqlCommand(sql);
        }
        #endregion

        public int EntryStatistics(DateTime statisticsStartTime, DateTime statisticsEndTime, int topAgentId)
        {
            var sql = string.Format(@"DELETE FROM bihu_analytics.tj_entry_overview WHERE data_in_time>='{0}' AND data_in_time<'{1}' {3};
                                    INSERT INTO bihu_analytics.tj_entry_overview(agent_id,parent_agent_id,top_agent_id,entry_count,distributed_count,review_count,quote_count,renewal_period_count,create_time,data_in_time)
                                    SELECT t.Agent agent_id,IFNULL(a.ParentAgent,0) parent_agent_id,IFNULL(a.TopAgentId,0) top_agent_id,t.EntryCount entry_count,
                                    t.DistributedCount distributed_count,t.ReviewCount review_count,t.QuoteCount quote_count,t.RenewalPeriodCount renewal_period_count,
                                    NOW() create_time,'{0}' data_in_time FROM (
                                    SELECT
                                        Agent,
                                        COUNT(1) EntryCount,
                                        SUM(CASE IsDistributed WHEN 0 THEN 0 ELSE 1 END) DistributedCount,
                                        SUM(CASE IsReView WHEN 0 THEN 0 ELSE 1 END) ReviewCount,
                                        SUM(CASE QuoteStatus WHEN -1 THEN 0 WHEN 0 THEN 0 ELSE 1 END) QuoteCount,
                                        SUM(CASE WHEN DATEDIFF(LastBizEndDate,NOW())>=0 AND DATEDIFF(LastBizEndDate,NOW())<=90 THEN 1
	                                         WHEN DATEDIFF(LastForceEndDate,NOW())>=0 AND DATEDIFF(LastForceEndDate,NOW())<=90 THEN 1
	                                         ELSE 0 END) RenewalPeriodCount FROM(
                                    SELECT 
	                                    IF(@grp=t1.Id,@rank:=@rank+1,@rank:=1) rank,
	                                    @grp:=t1.Id,
	                                    t1.*
                                    FROM(
                                    SELECT u.Id,u.Agent,u.IsReView,u.IsDistributed,u.QuoteStatus,c.LastBizEndDate,c.LastForceEndDate,u.CameraTime,IFNULL(d.create_time,u.CameraTime) create_time FROM
                                        bx_userinfo u LEFT JOIN bx_userinfo_renewal_index q ON u.Id=q.b_uid LEFT JOIN bx_car_renewal c ON q.car_renewal_id=c.Id
                                        LEFT JOIN bx_consumer_review d ON u.Id=d.b_uid AND d.`status` IN(4,9)
                                        WHERE
                                        u.IsCamera = 1
                                        AND u.CameraTime >= '{0}'
                                        AND u.CameraTime < '{1}'
                                        AND u.IsTest=0 ORDER BY u.Id DESC,d.id ASC) t1,(SELECT @grp:=0,@rank:=0) init) t2
                                        WHERE t2.rank=1 AND DATE_FORMAT(CameraTime,'%Y-%m-%d')<=DATE_FORMAT(create_time,'%Y-%m-%d')
                                    GROUP BY t2.Agent) t LEFT JOIN bx_agent a ON t.Agent=a.Id {2}", statisticsStartTime, statisticsEndTime,
                                    topAgentId == 0 ? "" : "WHERE a.TopAgentId=" + topAgentId, topAgentId == 0 ? "" : "AND top_agent_id=" + topAgentId);
            return _dbContext.Database.ExecuteSqlCommand(sql);
        }

    }
    public class Utils
    {
        /// <summary>
        /// 一次插入处理的条数
        /// </summary>
        private readonly int _num;
        private EntityContext _dbContext;

        /// <summary>
        ///更新时间
        /// </summary>
        private delegate void Log(string message, string fileName);
        public Utils()
        {
            _dbContext = new EntityContext();
            _num = int.Parse(ConfigurationManager.AppSettings["num"]);
        }

        /// <summary>
        /// 添加每一天的统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayAnalytics(DateTime startTime, DateTime endTime)
        {
            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
            var agentDataList = _dbContext.Database.SqlQuery<AgentData>(SeleteSql, parameters).ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _dbContext.Database.ExecuteSqlCommand(insertSql);
                }
            }
        }


        /// <summary>
        /// 添加每一天的统计数据 运通账号
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayAnalyticsYunTong(DateTime startTime, DateTime endTime, int TopAgent)
        {
            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
            var agentDataList = _dbContext.Database.SqlQuery<AgentData>(SeleteSql, parameters).ToList();
            //保留TopAgent为赋值的数据
            agentDataList = agentDataList.Where(x => x.TopAgentId == TopAgent).ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _dbContext.Database.ExecuteSqlCommand(insertSql);
                }
            }
        }

        /// <summary>
        /// 添加每一天的战败分析统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayDefeatAnalytics(DateTime startTime, DateTime endTime)
        {
            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
            };
            var agentDataList = _dbContext.Database.SqlQuery<DefeatAnalysis>(SeleteDefeatAnalysis, parameters).ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _dbContext.Database.ExecuteSqlCommand(insertSql);
                }
            }
        }

        /// <summary>
        /// 添加当天的统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertTodayAnalytics(DateTime startTime, DateTime endTime)
        {
            var agentDataList = new List<AgentData>();
            //查询代理人统计数据
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };

            agentDataList = _dbContext.Database.SqlQuery<AgentData>(SeleteSql, parameters).ToList();
            //删除今天的统计数据
            DeleteAnalytics(startTime);
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _dbContext.Database.ExecuteSqlCommand(insertSql);
                }
            }
        }

        /// <summary>
        /// 添加当天的战败分析统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertTodayDefeatAnalytics(DateTime startTime, DateTime endTime)
        {
            var agentDataList = new List<DefeatAnalysis>();
            //查询代理人统计数据
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
            agentDataList = _dbContext.Database.SqlQuery<DefeatAnalysis>(SeleteDefeatAnalysis, parameters).ToList();
            //删除今天的统计数据
            DeleteDefeatAnalytics(startTime);
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _dbContext.Database.ExecuteSqlCommand(insertSql);
                }
            }
        }

        /// <summary>
        /// 拼接插入统计表的sql
        /// </summary>
        /// <param name="data">需要插入的数据</param>
        /// <returns></returns>
        private string BuildInsertSql(IEnumerable<DefeatAnalysis> data)
        {
            var sql = new StringBuilder("USE BIHU_ANALYTICS;INSERT INTO Defeat_analytics(ID,AGENTID,AGENTNAME,COUNT,DefeatReason,DefeatReasonId,CreateTime,DataInTime) VALUES ");
            var tailerList = new List<string>();
            foreach (var item in data)
            {
                tailerList.Add(string.Format("({0},{1},'{2}',{3},'{4}',{5},'{6}','{7}')", 0, item.AgentId, item.AgentName, item.Count, item.DefeatReason, item.DefeatReasonId, item.CreateTime, item.DataInTime));
            }
            sql.Append(string.Join(",", tailerList));
            return sql.ToString();
        }


        /// <summary>
        /// 拼接插入统计表的sql
        /// </summary>
        /// <param name="data">需要插入的数据</param>
        /// <returns></returns>
        private string BuildInsertSql(IEnumerable<AgentData> data)
        {
            var sql = new StringBuilder("USE BIHU_ANALYTICS;INSERT INTO BUSINESS_ANALYTICS (ID,AGENTID,AGENTNAME,QUOTECARCOUNT,QUOTECOUNT,SMSSENDCOUNT,RETURNVISITCOUNT,APPOINTMENTCOUNT,SINGLECOUNT,DEFEATCOUNT,CREATETIME,UPDATETIME,`DELETE`,PARENTAGENTID,TOPAGENTID,DATAINTIME,ORDERCOUNT,BatchRenewalCount) VALUES ");
            var tailerList = new List<string>();
            foreach (var item in data)
            {
                tailerList.Add(string.Format("({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},{17})", 0, item.AgentId, item.AgentName, item.QuoteCarCount, item.QuoteCount, item.SmsSendCount, item.ReturnVisitCount, item.AppointmentCount, item.SingleCount, item.DefeatCount, item.CreateTime, item.UpdateTime, item.Delete ? 0 : 1, item.ParentAgentId, item.TopAgentId, item.DataInTime, item.OrderCount, item.BatchRenewalCount));
            }
            sql.Append(string.Join(",", tailerList));
            return sql.ToString();
        }

        /// <summary>
        /// 查询所有已统计的日期
        /// </summary>
        /// <returns></returns>
        public List<AgentData> GetDatainTimeList()
        {
            return _dbContext.Database.SqlQuery<AgentData>(SqlDatainTime).ToList();

        }

        /// <summary>
        /// 查询所有已统计的日期
        /// </summary>
        /// <returns></returns>
        public List<DefeatAnalysis> GetDefeatDatainTimeList()
        {
            return _dbContext.Database.SqlQuery<DefeatAnalysis>(SqlDefeatDatainTime).ToList();
        }

        /// <summary>
        /// 删除某一天的统计数据
        /// </summary>
        /// <param name="dataInTime"></param>
        public void DeleteAnalytics(DateTime dataInTime)
        {
            var parameter = new MySqlParameter("@dataInTime", MySqlDbType.DateTime) { Value = dataInTime };
            int result = _dbContext.Database.ExecuteSqlCommand(SqlDeleteByDataInTime, parameter);
        }

        /// <summary>
        /// 删除某一天的统计数据 运通
        /// </summary>
        /// <param name="dataInTime"></param>
        public void DeleteAnalyticsYuntong(int TopAgentId)
        {
            var parameter = new MySqlParameter("@topAgentId", MySqlDbType.Int32) { Value = TopAgentId };
            int result = _dbContext.Database.ExecuteSqlCommand(SqlDeleteByReFresh, parameter);

        }
        /// <summary>
        /// 删除某一天的战败分析统计数据
        /// </summary>
        /// <param name="dataInTime"></param>
        private void DeleteDefeatAnalytics(DateTime dataInTime)
        {
            var parameter = new MySqlParameter("@dataInTime", MySqlDbType.DateTime) { Value = dataInTime };
            int result = _dbContext.Database.ExecuteSqlCommand(SqlDeleteDefeatByDataInTime, parameter);
        }
        #region  sql语句
        /// <summary>
        /// 删除某一天的统计数据
        /// </summary>
        private const string SqlDeleteByDataInTime = "USE BIHU_ANALYTICS;DELETE FROM BUSINESS_ANALYTICS WHERE DATAINTIME = @DATAINTIME";

        /// <summary>
        /// 删除某顶级账号，刷数据使用
        /// </summary>
        private const string SqlDeleteByReFresh = "USE BIHU_ANALYTICS;DELETE FROM BUSINESS_ANALYTICS WHERE TopAgentId = @TopAgentId";


        private const string SqlDeleteDefeatByDataInTime = "USE BIHU_ANALYTICS;DELETE FROM Defeat_ANALYTICS WHERE DATAINTIME = @DATAINTIME";
        /// <summary>
        /// 出单量/件（从对账列表中抓取）（一辆车同一天计算为一件）
        /// </summary>
        private const string SingleCount = @"SELECT AGENT_ID AS AGENTID, COUNT(1) AS SINGLECOUNT
                                                    FROM (SELECT AGENT_ID, GUID, COUNT(1)
	                                                    FROM DZ_RECONCILIATION
	                                                    WHERE LENGTH(GUID) = 36
		                                                    AND AGENT_ID > 0
		                                                    AND CREATE_DATE BETWEEN @STARTTIME AND @ENDTIME
	                                                    GROUP BY AGENT_ID, GUID
	                                                    ) T
                                                   GROUP BY AGENT_ID";
        /// <summary>
        /// 出单量/件（已出保单列表中抓取数据）（一辆车同一天计算为一件）
        /// </summary>
        private const string OrderCount = @" SELECT
	                                            COUNT(DISTINCT buid, source) AS ORDERCOUNT,
	                                            BRU.AGENT AS AGENTID
                                            FROM
	                                            BX_CAR_ORDER AS BC
                                            Left Join bx_order_userinfo AS BRU
                                            ON BC.id=BRU.OrderId
                                            WHERE
	                                            BC.ORDER_STATUS =- 2
                                            AND CREATE_TIME  BETWEEN @STARTTIME
                                            AND @ENDTIME
                                            GROUP BY
	                                           BRU.AGENT";

        /// <summary>
        /// 战败次数（一辆车同一天计算为一件）
        /// </summary>
        private const string DefeatCount = @"SELECT UI.AGENT, COUNT(DISTINCT UI.ID) AS DEFEATCOUNT
                                            FROM BX_CONSUMER_REVIEW CR LEFT JOIN BX_USERINFO UI ON CR.B_UID = UI.ID
                                            WHERE CR.STATUS IN (4, 16)
	                                            AND CREATE_TIME BETWEEN  @STARTTIME AND @ENDTIME
                                            GROUP BY UI.AGENT";


        /// <summary>
        /// 回访次数
        /// </summary>
        private const string ReturnVisitCount = @"SELECT OPERATORID, COUNT(*) AS RETURNVISITCOUNT
		                                                        FROM BX_CONSUMER_REVIEW WHERE 
                                                    CREATE_TIME BETWEEN  @STARTTIME AND @ENDTIME  
                                                    GROUP BY OPERATORID";

        /// <summary>
        /// 预约到店（一辆车同一天计算为一件）
        /// </summary>
        private const string AppointmentCount = @"SELECT UI.AGENT, COUNT(DISTINCT UI.ID) AS APPOINTMENTCOUNT
                                                        FROM BX_CONSUMER_REVIEW CR LEFT JOIN BX_USERINFO UI ON CR.B_UID = UI.ID
                                                        WHERE CR.STATUS = 20
	                                                        AND CREATE_TIME BETWEEN @STARTTIME AND @ENDTIME
                                                        GROUP BY UI.AGENT";

        /// <summary>
        /// 报价车辆数（一辆车同一天计算为一件）
        /// </summary>
        private const string QuoteCarCount = @"SELECT AGENT, COUNT(1) AS QUOTECARCOUNT
                            FROM (SELECT AGENT, COUNT(DISTINCT B_UID) AS QUOTECARCOUNT
	                            FROM BIHUSTATISTICS.BX_QUOTE_HISTORY
	                            WHERE CREATETIME BETWEEN @STARTTIME AND @ENDTIME
	                            GROUP BY AGENT, B_UID
	                            ) A
                            GROUP BY AGENT";

        /// <summary>
        /// 报价次数
        /// </summary>
        private const string QuoteCount = @"SELECT AGENTID, COUNT(QUOTECARCOUNT) AS QUOTECARCOUNT, SUM(QUOTECOUNT) AS QUOTECOUNT
                                            FROM (SELECT AGENT AS AGENTID, B_UID, COUNT(DISTINCT B_UID) AS QUOTECARCOUNT, COUNT(1) AS QUOTECOUNT
	                                            FROM BIHUSTATISTICS.BX_QUOTE_HISTORY WHERE 
                                                CREATETIME BETWEEN @STARTTIME AND @ENDTIME
                                                GROUP BY AGENT, B_UID
	                                            ) A
                                            GROUP BY AGENTID";

        /// <summary>
        /// 短信发送量
        /// </summary>
        private const string SmsSendCount = @"SELECT AGENT_ID,COUNT(1) AS SMSSENDCOUNT
	                                FROM BX_SMS_ACCOUNT_CONTENT
	                                WHERE CREATE_TIME BETWEEN @STARTTIME AND @ENDTIME
	                                GROUP BY AGENT_ID";


        /// <summary>
        /// 批量续保车数量
        /// </summary>
        private const string BatchRenewalCount = @" SELECT COUNT(DISTINCT LicenseNo) AS BatchRenewalCount,Agent  FROM bx_userinfo  WHERE UpdateTime BETWEEN  @STARTTIME  AND @ENDTIME  AND RenewalStatus>-1 AND RenewalType IN (1,2,3,4,6,7,8)  GROUP BY Agent";

        /// <summary>
        /// 查询所有已统计的日期
        /// </summary>
        private const string SqlDatainTime = "USE bihu_analytics;SELECT DISTINCT(DATE(DATAINTIME))AS DATAINTIME FROM BUSINESS_ANALYTICS";

        /// <summary>
        /// 查询所有已统计的日期
        /// </summary>
        private const string SqlDefeatDatainTime = "USE bihu_analytics;SELECT DISTINCT(DATE(DATAINTIME))AS DATAINTIME FROM Defeat_ANALYTICS";

        /// <summary>
        /// 查询战败分析
        /// </summary>
        private const string SeleteDefeatAnalysis = @"SELECT COUNT(1) Count,agentid,MAX(AgentName) AgentName,MAX(DEFEATREASON) DEFEATREASON,DEFEATREASONID,NOW() AS CREATETIME,DATE_FORMAT(@startTime,'%Y-%m-%d') AS DataInTime
                                                    FROM(
                                                    SELECT 
                                                    IF(@grp=t.BuId,@rank:=@rank+1,@rank:=1) rank,
                                                    @grp:=t.BuId,
                                                    t.*
                                                    FROM(
                                                    SELECT
	                                                    ui.Agent AS agentid,
	                                                    bx_agent.AgentName,
	                                                    DEFEATREASON,
	                                                    DH.DEFEATREASONID,
                                                        DH.BuId,
                                                        DH.Id
                                                    FROM
	                                                    BX_DEFEATREASONHISTORY DH
                                                    LEFT JOIN BX_DEFEATREASONSETTING DS ON DH.DEFEATREASONID=DS.ID
                                                    LEFT JOIN bx_userinfo ui ON DH.BuId=UI.ID
                                                    LEFT JOIN bx_agent ON bx_agent.Id=ui.Agent
                                                    WHERE
	                                                    DEFEATREASON IS NOT NULL
                                                    AND dh.CREATETIME BETWEEN @startTime
                                                    AND @endTime
                                                    AND ds.deleted=0
                                                    ORDER BY DH.BuId DESC,DH.Id DESC) t,(SELECT @grp:=0,@rank:=0) init) t
                                                    WHERE rank=1
                                                    GROUP BY agentid,DEFEATREASONID";

        /// <summary>
        /// 查询统计数据
        /// </summary>
        private static string SeleteSql
        {
            get
            {
                var sql = string.Format(
                                @"SELECT 
                                    0 AS ID, 
                                    IFNULL(BX.ID, 0) AS AGENTID, 
                                    BX.AGENTNAME,
                                    IFNULL(BX.AGENTNAME, NULL),
                                    IFNULL(G.QUOTECARCOUNT, 0) AS QUOTECARCOUNT, 
                                    IFNULL(B.SMSSENDCOUNT, 0) AS SMSSENDCOUNT, 
                                    IFNULL(C.RETURNVISITCOUNT, 0) AS RETURNVISITCOUNT,
                                    IFNULL(D.APPOINTMENTCOUNT, 0) AS APPOINTMENTCOUNT,
                                    IFNULL(E.SINGLECOUNT, 0) AS SINGLECOUNT , 
                                    IFNULL(F.DEFEATCOUNT, 0) AS DEFEATCOUNT,
                                    IFNULL(G.QUOTECOUNT, 0) AS QUOTECOUNT, 
                                    IFNULL(H.ORDERCOUNT, 0) AS ORDERCOUNT, 
                                    IFNULL(I.BatchRenewalCount, 0) AS BatchRenewalCount, 
                                    NOW() AS CREATETIME,
                                    NOW() AS UPDATETIME, 
                                    0 AS `DELETE`, 
                                    BX.PARENTAGENT AS PARENTAGENTID, 
                                    BX.TOPAGENTID AS TOPAGENTID,
                                    DATE(@STARTTIME) AS DATAINTIME
                                 FROM (SELECT ID,AGENTNAME,PARENTAGENT,TOPAGENTID FROM BX_AGENT) AS BX
                                     LEFT JOIN({0}) B ON BX.ID = B.AGENT_ID
                                     LEFT JOIN({1}) C ON BX.ID = C.OPERATORID
                                     LEFT JOIN({2}) D ON BX.ID = D.AGENT
                                     LEFT JOIN({3}) E ON BX.ID = E.AGENTID
                                     LEFT JOIN({4}) F ON BX.ID = F.AGENT
                                     LEFT JOIN({5}) G ON BX.ID = G.AGENTID 
                                     LEFT JOIN({6}) H ON BX.ID = H.AGENTID 
                                     LEFT JOIN({7}) I ON BX.ID = I.Agent 
                                 WHERE  
                                    IFNULL(G.QUOTECARCOUNT,0)+
                                    IFNULL(B.SMSSENDCOUNT,0)+
                                    IFNULL(C.RETURNVISITCOUNT,0) +
                                    IFNULL(D.APPOINTMENTCOUNT,0) +
                                    IFNULL(E.SINGLECOUNT,0) +
                                    IFNULL(F.DEFEATCOUNT,0)+
                                    IFNULL(G.QUOTECOUNT,0) +
                                    IFNULL(H.ORDERCOUNT,0)+
                                    IFNULL(I.BatchRenewalCount,0)>0 ",
                                    SmsSendCount,
                                    ReturnVisitCount,
                                    AppointmentCount,
                                    SingleCount,
                                    DefeatCount,
                                    QuoteCount,
                                    OrderCount,
                                    BatchRenewalCount);
                return sql;
            }
        }
        #endregion
    }
}
