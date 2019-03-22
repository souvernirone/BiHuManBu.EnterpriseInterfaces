using BiHuManBu.ExternalInterfaces.Models.Model;
using MySql.Data.MySqlClient;
using StatisticalModuleJobs.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using StatisticalModuleJobs.DbHelper;

namespace StatisticalModuleJobs
{
    public class Utils
    {
        /// <summary>
        /// mysql 连接字符串
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// 一次插入处理的条数
        /// </summary>
        private readonly int _num;

        /// <summary>
        ///更新时间
        /// </summary>
        private readonly StatisticalModuleJobs.DbHelper.MySqlHelper _sqlhelper;
        private delegate void Log(string message, string fileName);
        private readonly Log _logError = Logging.LogError;
        private readonly Log _logInfo = Logging.LogInfo;
        public Utils()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["zb"].ConnectionString;
            _sqlhelper = new StatisticalModuleJobs.DbHelper.MySqlHelper(_connectionString);

            _num = int.Parse(ConfigurationManager.AppSettings["num"]);
        }

        /// <summary>
        /// 添加每一天的统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayAnalytics(DateTime startTime, DateTime endTime)
        {
            _logInfo("添加统计数据开始，统计时间区间:" + startTime + " - " + endTime, "BusinessStatisticsJob.txt");

            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
            var agentDataList = _sqlhelper.ExecuteDataTable(SeleteSql, parameters).ToList<AgentData>().ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _sqlhelper.ExecuteNonQuery(insertSql);
                    _logInfo("插入成功的条数:" + result, "BusinessStatisticsJob.txt");
                }
            }
            _logInfo("添加统计数据结束,代理人数：" + agentDataList.Count, "BusinessStatisticsJob.txt");
        }


        /// <summary>
        /// 添加每一天的统计数据 运通账号
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayAnalyticsYunTong(DateTime startTime, DateTime endTime, int TopAgent)
        {
            _logInfo("添加统计数据开始，统计时间区间:" + startTime + " - " + endTime, "BusinessStatisticsJob.txt");

            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
            var agentDataList = _sqlhelper.ExecuteDataTable(SeleteSql, parameters).ToList<AgentData>().ToList();
            //保留TopAgent为赋值的数据
            agentDataList = agentDataList.Where(x => x.TopAgentId == TopAgent).ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _sqlhelper.ExecuteNonQuery(insertSql);
                    _logInfo("插入成功的条数:" + result, "BusinessStatisticsJob.txt");
                }
            }
            _logInfo("添加统计数据结束,代理人数：" + agentDataList.Count, "BusinessStatisticsJob.txt");
        }

        /// <summary>
        /// 添加每一天的战败分析统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertEachDayDefeatAnalytics(DateTime startTime, DateTime endTime)
        {

            _logInfo("添加统计数据开始，统计时间区间:" + startTime + " - " + endTime, "DefeatAnalysisJob.txt");

            //查询代理人统计数据
            MySqlParameter[] parameters =
            {
                new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
            };
            var agentDataList = _sqlhelper.ExecuteDataTable(SeleteDefeatAnalysis, parameters).ToList<DefeatAnalysis>().ToList();
            if (agentDataList.Count > 0)
            {
                //插入数据，一次插入100条
                for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                {
                    string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                    int result = _sqlhelper.ExecuteNonQuery(insertSql);
                    _logInfo("插入成功的条数:" + result, "DefeatAnalysisJob.txt");
                }
            }
            _logInfo("添加统计数据结束,代理人数：" + agentDataList.Count, "DefeatAnalysisJob.txt");
        }

        /// <summary>
        /// 添加当天的统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertTodayAnalytics(DateTime startTime, DateTime endTime)
        {
            _logInfo("添加统计数据开始，统计时间区间:" + startTime + " - " + endTime, "BusinessStatisticsJob.txt");
            var agentDataList = new List<AgentData>();
            var conn = new MySqlConnection(_connectionString); conn.Open();
            var transaction = conn.BeginTransaction();
            try
            {
                //查询代理人统计数据
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };

                agentDataList = _sqlhelper.ExecuteDataTable(SeleteSql, parameters).ToList<AgentData>().ToList();
                //删除今天的统计数据
                DeleteAnalytics(DateTime.Today);
                if (agentDataList.Count > 0)
                {
                    //插入数据，一次插入100条
                    for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                    {
                        string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                        int result = _sqlhelper.ExecuteNonQuery(insertSql);
                        _logInfo("插入成功的条数:" + result, "BusinessStatisticsJob.txt");
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, "Error.txt");
            }
            _logInfo("添加统计数据结束,代理人数：" + agentDataList.Count, "BusinessStatisticsJob.txt");
        }

        /// <summary>
        /// 添加当天的战败分析统计数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public void InsertTodayDefeatAnalytics(DateTime startTime, DateTime endTime)
        {
            _logInfo("添加统计数据开始，统计时间区间:" + startTime + " - " + endTime, "DefeatAnalysisJob.txt");
            var agentDataList = new List<DefeatAnalysis>();
            var conn = new MySqlConnection(_connectionString); conn.Open();
            var transaction = conn.BeginTransaction();
            try
            {
                //查询代理人统计数据
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@startTime", MySqlDbType.DateTime){Value =startTime},
                    new MySqlParameter("@endTime", MySqlDbType.DateTime){Value =endTime}
                };
                agentDataList = _sqlhelper.ExecuteDataTable(SeleteDefeatAnalysis, parameters).ToList<DefeatAnalysis>().ToList();
                //删除今天的统计数据
                DeleteDefeatAnalytics(DateTime.Today);
                if (agentDataList.Count > 0)
                {
                    //插入数据，一次插入100条
                    for (var i = 1; i <= agentDataList.Count / _num + 1; i++)
                    {
                        string insertSql = BuildInsertSql(agentDataList.Take(_num * i).Skip(_num * (i - 1)).ToList());
                        int result = _sqlhelper.ExecuteNonQuery(insertSql);
                        _logInfo("插入成功的条数:" + result, "DefeatAnalysisJob.txt");
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, "Error.txt");
            }
            _logInfo("添加统计数据结束,代理人数：" + agentDataList.Count, "DefeatAnalysisJob.txt");
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
            return _sqlhelper.ExecuteDataTable(SqlDatainTime).ToList<AgentData>().ToList();
        }

        /// <summary>
        /// 查询所有已统计的日期
        /// </summary>
        /// <returns></returns>
        public List<DefeatAnalysis> GetDefeatDatainTimeList()
        {
            return _sqlhelper.ExecuteDataTable(SqlDefeatDatainTime).ToList<DefeatAnalysis>().ToList();
        }

        /// <summary>
        /// 删除某一天的统计数据
        /// </summary>
        /// <param name="dataInTime"></param>
        public void DeleteAnalytics(DateTime dataInTime)
        {
            var parameter = new MySqlParameter("@dataInTime", MySqlDbType.DateTime) { Value = dataInTime };
            int result = _sqlhelper.ExecuteNonQuery(SqlDeleteByDataInTime, parameter);
            _logInfo("删除今天的记录，删除行数：" + result + ",统计日期:" + dataInTime, "BusinessStatisticsJob.txt");
        }

        /// <summary>
        /// 删除某一天的统计数据 运通
        /// </summary>
        /// <param name="dataInTime"></param>
        public void DeleteAnalyticsYuntong(int TopAgentId)
        {
            var parameter = new MySqlParameter("@topAgentId", MySqlDbType.Int32) { Value = TopAgentId };
            int result = _sqlhelper.ExecuteNonQuery(SqlDeleteByReFresh, parameter);
            _logInfo("删除今天的记录，删除行数：" + result + ",删除的顶级账号数据:" + TopAgentId, "BusinessStatisticsJob.txt");

        }
        /// <summary>
        /// 删除某一天的战败分析统计数据
        /// </summary>
        /// <param name="dataInTime"></param>
        private void DeleteDefeatAnalytics(DateTime dataInTime)
        {
            var parameter = new MySqlParameter("@dataInTime", MySqlDbType.DateTime) { Value = dataInTime };
            int result = _sqlhelper.ExecuteNonQuery(SqlDeleteDefeatByDataInTime, parameter);
            _logInfo("删除今天的记录，删除行数：" + result + ",统计日期:" + dataInTime, "DefeatAnalysisJob.txt");
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

        //        /// <summary>
        //        /// 批量续保车数量
        //        /// </summary>
        //        private const string BatchRenewalCount = @" SELECT COUNT(DISTINCT  bx_userinfo.Id) AS BatchRenewalCount,bx_userinfo.AGENT AS AGENTID FROM bx_car_renewal 
        //                                    LEFT JOIN bx_userinfo_renewal_index ON bx_userinfo_renewal_index.car_renewal_id=bx_car_renewal.Id 
        //                                    LEFT JOIN  bx_userinfo ON bx_userinfo_renewal_index.b_uid=bx_userinfo.id 
        //                                    WHERE bx_car_renewal.CreateTime  BETWEEN @STARTTIME  AND @ENDTIME GROUP BY  AGENTID";

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
        //private const string SeleteDefeatAnalysis = @"SELECT COUNT(1) AS COUNT,	ifnull(rep.notreapetcount, 0) AS notreapetcount, DH.AGENTID, AGENTNAME, DEFEATREASON, DEFEATREASONID,now() as CREATETIME,@starttime as DataInTime 
        //                                                FROM BX_DEFEATREASONHISTORY DH LEFT JOIN BX_DEFEATREASONSETTING DS ON DH.DEFEATREASONID = DS.ID
        //                                                LEFT JOIN (
        //                                                 SELECT
        //                                                  agentid,
        //                                                  count(id) AS notreapetcount
        //                                                 FROM
        //                                                  BX_DEFEATREASONHISTORY a
        //                                                 WHERE
        //                                                  id IN (
        //                                                   SELECT
        //                                                    min(ID) AS id
        //                                                   FROM
        //                                                    BX_DEFEATREASONHISTORY
        //                                                   WHERE
        //                                                    CreateTime > @startTime
        //                                                   GROUP BY
        //                                                    BUID
        //                                                  )
        //                                                ) rep ON DH.AgentId = rep.agentid
        //                                                WHERE DEFEATREASON IS NOT NULL AND dh.CREATETIME BETWEEN @startTime AND @ENDTIME and ds.deleted=0 
        //                                                and Dh.id in (SELECT
        //                                             MAX(ID) as id
        //                                            FROM
        //                                             BX_DEFEATREASONHISTORY
        //                                            GROUP BY
        //                                             BUID,agentid)
        //                                                GROUP BY AGENTID, DEFEATREASONID";


        //        /// <summary>
        //        /// 查询战败分析
        //        /// </summary>
        //        private const string SeleteDefeatAnalysis = @"SELECT
        //	                                                        COUNT(1) AS COUNT,
        //	                                                       ui.Agent as agentid,
        //	                                                       	bx_agent.AgentName,
        //	                                                        DEFEATREASON,
        //	                                                        DH.DEFEATREASONID,
        //	                                                        now() AS CREATETIME,
        //	                                                        @startTime AS DataInTime
        //                                                        FROM
        //	                                                        BX_DEFEATREASONHISTORY DH
        //                                                        LEFT JOIN BX_DEFEATREASONSETTING DS ON DH.DEFEATREASONID = DS.ID
        //                                                       left join bx_userinfo ui on DH.BuId = UI.ID
        //                                                        LEFT JOIN bx_agent ON bx_agent.Id=ui.Agent         
        //                                                        WHERE
        //	                                                        DEFEATREASON IS NOT NULL
        //                                                        AND dh.CREATETIME BETWEEN @startTime
        //                                                        AND @endTime
        //                                                        AND ds.deleted = 0
        //                                                        AND Dh.id IN (
        //	                                                        SELECT
        //		                                                        MAX(ID) AS id
        //	                                                        FROM
        //		                                                        BX_DEFEATREASONHISTORY
        //	                                                        GROUP BY
        //		                                                        BUID
        //                                                        )
        //                                                        GROUP BY
        //	                                                        AGENTID,
        //	                                                        DEFEATREASONID";


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
                                    IFNULL(A.QUOTECARCOUNT, 0) AS QUOTECARCOUNT, 
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
                                     LEFT JOIN({0}) A ON BX.ID = A.AGENT
                                     LEFT JOIN({1}) B ON BX.ID = B.AGENT_ID
                                     LEFT JOIN({2}) C ON BX.ID = C.OPERATORID
                                     LEFT JOIN({3}) D ON BX.ID = D.AGENT
                                     LEFT JOIN({4}) E ON BX.ID = E.AGENTID
                                     LEFT JOIN({5}) F ON BX.ID = F.AGENT
                                     LEFT JOIN({6}) G ON BX.ID = G.AGENTID 
                                     LEFT JOIN({7}) H ON BX.ID = H.AGENTID 
                                     LEFT JOIN({8}) I ON BX.ID = I.Agent 
                                 WHERE  
                                    IFNULL(A.QUOTECARCOUNT,0)+
                                    IFNULL(B.SMSSENDCOUNT,0)+
                                    IFNULL(C.RETURNVISITCOUNT,0) +
                                    IFNULL(D.APPOINTMENTCOUNT,0) +
                                    IFNULL(E.SINGLECOUNT,0) +
                                    IFNULL(F.DEFEATCOUNT,0)+
                                    IFNULL(G.QUOTECOUNT,0) +
                                    IFNULL(H.ORDERCOUNT,0)+
                                    IFNULL(I.BatchRenewalCount,0)>0 ",
                                    QuoteCarCount,
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

