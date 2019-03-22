using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using System.Threading.Tasks;
using System.Data;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;
using System.Data.Entity;
using System.Collections;

namespace BiHuManBu.ExternalInterfaces.Repository
{

    public class StatisticsRepository : IStatisticsRepository
    {
        readonly MySqlHelper _mySqlHelper;
        readonly string dataBaseName = string.Empty;
        private ILog logMsg;
        public StatisticsRepository(IAgentRepository agentRepository)
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
            dataBaseName = ConfigurationManager.AppSettings["ChangeDataBaseName"] == "0" ? "bihumanbu_qa" : "bihumanbu";
            logMsg = LogManager.GetLogger("MSG");
        }
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
                logMsg.InfoFormat("写入成功，日期：{0}", endDate.AddDays(-1));
                //Task.Run(() => CarRenewalStatistics(ps, agentIds));
                //Task.Run(() => AppointmentStatisticsDetail(ps,agentIds));
                //Task.Run(() => AnswerCallTimesStatisticsDetail(ps,agentIds));
            }
            return true;
        }
        void CarRenewalStatistics(List<MySqlParameter> ps)
        {
            #region 统计昨天回访的数据
            var getCustomerRenewalSql = string.Format(@"USE  {0};SELECT 
u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,
CASE WHEN  crw2.ParentStatus=9 AND crw2.singletime IS NOT NULL THEN crw2.singletime ELSE  crw2.create_time END AS StatusCreateTime,
CASE WHEN  crw2.status=20 THEN '预约到店' WHEN crw2.ParentStatus=9 THEN '已出单' WHEN crw2.ParentStatus IN(4,16) THEN '战败'  WHEN crw2.ParentStatus=10001 THEN '跟进中' END AS CustomerStatusName,
crw2.BizEndTime  AS BizEndDate,
DATE_FORMAT(crw2.BizEndTime,'%Y') AS BizEndDateYear,
crw2.ParentStatus AS CustomerStatus,
u.CategoryInfoId as CategoryInfoId,
DATE_FORMAT(CASE WHEN  crw2.ParentStatus=9 AND crw2.singletime IS NOT NULL THEN crw2.singletime ELSE  crw2.create_time END,'%Y-%m')  as StatusCreateTime_Year_Month,
NOW() AS CreateTime,
NOW() AS UpdateTime
FROM  (SELECT crw.* FROM bx_consumer_review crw INNER JOIN  (SELECT MAX(id) AS id FROM bx_consumer_review AS crw1 WHERE crw1.status!=0 AND 
 crw1.create_time >=?dateInTimeStart AND crw1.create_time<?dateInTimeEnd AND crw1.IsDeleted=0 AND crw1.BizEndTime  >'1900-01-01' GROUP BY crw1.b_uid) AS m ON crw.id=m.id 
)AS crw2 
left JOIN  bx_userinfo AS u 
ON crw2.b_uid=u.id
LEFT JOIN bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  bx_agent AS a 
ON a.Id=u.Agent where u.IsTest=0", dataBaseName);
            #endregion
            #region 统计昨天未回访的数据
            var getCustomerNotRenewalSql = string.Format(@"USE  {0};
SELECT 
u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName, 
u.UpdateTime AS StatusCreateTime,
CASE WHEN 
	bi2.BizEndDate is null THEN cr.LastBizEndDate
	WHEN cr.LastBizEndDate is null THEN bi2.BizEndDate
	WHEN DATE_FORMAT(bi2.BizEndDate,'%Y')>DATE_FORMAT(cr.LastBizEndDate,'%Y')THEN bi2.BizEndDate 
	ELSE cr.LastBizEndDate  
END 
AS BizEndDate,	
DATE_FORMAT(
CASE WHEN 
	bi2.BizEndDate is null THEN cr.LastBizEndDate
	WHEN cr.LastBizEndDate is null THEN bi2.BizEndDate
	WHEN DATE_FORMAT(bi2.BizEndDate,'%Y')>DATE_FORMAT(cr.LastBizEndDate,'%Y')THEN bi2.BizEndDate 
	ELSE cr.LastBizEndDate  
END ,'%Y'
)   
AS BizEndDateYear,
'未跟进'  AS CustomerStatusName,
u.CategoryInfoId as CategoryInfoId,
  DATE_FORMAT(  u.UpdateTime,'%Y-%m') as  StatusCreateTime_Year_Month,
-1 AS CustomerStatus,
NOW() AS CreateTime,
NOW() AS UpdateTime
FROM (select u1.Id,u1.Agent,u1.CarVIN,u1.EngineNo,u1.CategoryInfoId,u1.UpdateTime,u1.LicenseNo from bx_userinfo as u1  where u1.UpdateTime>=?dateInTimeStart AND u1.UpdateTime<?dateInTimeEnd AND u1.IsReView=0 and u1.IsTest=0 )  AS u
left join bx_userinfo_renewal_index b
on u.Id=b.b_uid
LEFT JOIN bx_car_renewal AS cr
ON b.car_renewal_id=cr.Id
LEFT JOIN bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  bx_agent AS a 
ON u.Agent=a.Id
LEFT JOIN 
 bx_batchrenewal_item AS bi2  ON u.id=bi2.buid AND  bi2.isnew=1 AND bi2.isdelete=0
WHERE cr.LastBizEndDate>'1900-01-01' or bi2.BizEndDate>'1900-01-01'", dataBaseName);
            #endregion
            #region 获得上年的出单跟战败（用于保留数据）
            var getLastTimeDataSql = @"USE bihu_analytics; SELECT id FROM tj_reviewdetail_record t WHERE t.CustomerStatus IN (4, 9, 16) AND t.Deleted = 0 AND YEAR (t.StatusCreateTime) < YEAR (NOW()) AND BuId IN({0})";
            #endregion

            List<CarRenewalStatisticsDetailTable> carRenewalStatisticsRenewalDetailTable = new List<CarRenewalStatisticsDetailTable>();
            List<CarRenewalStatisticsDetailTable> carRenewalStatisticsNotRenewalDetailTable = new List<CarRenewalStatisticsDetailTable>();
            carRenewalStatisticsRenewalDetailTable = _mySqlHelper.ExecuteDataTable(CommandType.Text, getCustomerRenewalSql, ps.ToArray()).ToList<CarRenewalStatisticsDetailTable>().ToList().ToList();
            carRenewalStatisticsNotRenewalDetailTable = _mySqlHelper.ExecuteDataTable(CommandType.Text, getCustomerNotRenewalSql, ps.ToArray()).ToList<CarRenewalStatisticsDetailTable>().ToList().GroupBy(x => new { x.BuId, x.BizEndDateYear }).Select(x => x.OrderByDescending(s => s.BizEndDate).FirstOrDefault()).ToList();
            var carRenewalStatisticsDetailTable = carRenewalStatisticsRenewalDetailTable.Union(carRenewalStatisticsNotRenewalDetailTable).ToList();
            if (carRenewalStatisticsDetailTable.Any())
            {
                var updateIds = carRenewalStatisticsDetailTable.Select(x => x.BuId).ToList();
                if (updateIds.Any())
                {
                    //从昨日回访数据中取出出单跟战败的buId;
                    var insureAndDefeatBuIds = carRenewalStatisticsDetailTable.Where(x => (new int?[] { 4, 9, 16 }.Contains(x.CustomerStatus))).Select(x => x.BuId).ToList();
                    List<int> getLastTimeData = new List<int>();
                    if (insureAndDefeatBuIds.Any())
                    {
                        //根据昨日的出单及战败buId，查询该buId往年的出单跟战败数据(用于保留往年数据)
                        getLastTimeData = _mySqlHelper.ExecuteDataTable(CommandType.Text, string.Format(getLastTimeDataSql, string.Join(",", insureAndDefeatBuIds)), null).ToList<int>().ToList();
                    }
                    var str = string.Empty;
                    if (getLastTimeData.Any())
                    {
                        str = " and id not in(" + string.Join(",", getLastTimeData) + ")";
                    }
                    var deleteSql = @"USE bihu_analytics; delete from tj_reviewdetail_record WHERE BuId IN ({0})" + str;
                    var newDeleteSql = string.Empty;
                    var deleteCount = 0;
                    if (updateIds.Count <= 10000)
                    {
                        newDeleteSql = string.Format(deleteSql, string.Join(",", updateIds));
                        deleteCount = _mySqlHelper.ExecuteNonQuery(CommandType.Text, newDeleteSql);
                    }
                    else
                    {
                        for (int i = 0; i <= updateIds.Count / 10000; i++)
                        {
                            newDeleteSql = string.Format(deleteSql, string.Join(",", updateIds.Skip(i * 10000).Take(10000)));
                            deleteCount += _mySqlHelper.ExecuteNonQuery(CommandType.Text, newDeleteSql);
                        }
                    }
                    logMsg.InfoFormat("数据条数：{0}，数据中的出单跟战败条数：{1}，数据中包含的往年出单跟战败条数：{2}，删除条数：{3}", updateIds.Count, insureAndDefeatBuIds.Count, getLastTimeData.Count, deleteCount);
                }
                

                if (carRenewalStatisticsDetailTable.Count <= 10000)
                {
                    StringBuilder insertCarRenewalStatisticsDetailStringBuilder = new StringBuilder(@" USE bihu_analytics;INSERT  INTO tj_reviewdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,BizEndDate,CustomerName,CustomerStatusName,CategoryName,CustomerStatus,StatusCreateTime,CreateTime,UpdateTime,Deleted,CategoryInfoId,BizEndDateYear,StatusCreateTime_YearAndMonth)  VALUES ");
                    foreach (var item in carRenewalStatisticsDetailTable)
                    {

                        insertCarRenewalStatisticsDetailStringBuilder.Append(string.Format(" ({0},{1},'{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}','{14}','{15}',0,{16},{17},'{18}'),", item.BuId, item.AgentId, item.AgentName, item.ParentAgentId, item.TopAgentId, item.LicenseNo, item.CarVIN, string.IsNullOrEmpty(item.EngineNo) ? "" : item.EngineNo.Replace("'", "").Replace("\\", ""), item.BizEndDate, item.CustomerName, item.CustomerStatusName, item.CategoryName, item.CustomerStatus, item.StatusCreateTime, DateTime.Now, DateTime.Now, item.CategoryInfoId, item.BizEndDateYear, item.StatusCreateTime_Year_Month));
                    }
                    var insertCarRenewalStatisticsDetailSql = insertCarRenewalStatisticsDetailStringBuilder.ToString();
                    var insertCount = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertCarRenewalStatisticsDetailSql.Substring(0, insertCarRenewalStatisticsDetailSql.LastIndexOf(",")));
                }
                else
                {
                    for (int i = 0; i <= carRenewalStatisticsDetailTable.Count / 10000; i++)
                    {
                        StringBuilder insertCarRenewalStatisticsDetailStringBuilder = new StringBuilder(@" USE bihu_analytics;INSERT  INTO tj_reviewdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,BizEndDate,CustomerName,CustomerStatusName,CategoryName,CustomerStatus,StatusCreateTime,CreateTime,UpdateTime,Deleted,CategoryInfoId,BizEndDateYear,StatusCreateTime_YearAndMonth)  VALUES ");
                        foreach (var item in carRenewalStatisticsDetailTable.Skip(i * 10000).Take(10000))
                        {
                            insertCarRenewalStatisticsDetailStringBuilder.Append(string.Format(" ({0},{1},'{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12},'{13}','{14}','{15}',0,{16},{17},'{18}'),", item.BuId, item.AgentId, item.AgentName, item.ParentAgentId, item.TopAgentId, item.LicenseNo, item.CarVIN, string.IsNullOrEmpty(item.EngineNo) ? "" : item.EngineNo.Replace("'", "").Replace("\\", ""), item.BizEndDate, item.CustomerName, item.CustomerStatusName, item.CategoryName, item.CustomerStatus, item.StatusCreateTime, DateTime.Now, DateTime.Now, item.CategoryInfoId, item.BizEndDateYear, item.StatusCreateTime_Year_Month));
                        }
                        var insertCarRenewalStatisticsDetailSql = insertCarRenewalStatisticsDetailStringBuilder.ToString();
                        var insertCount = _mySqlHelper.ExecuteNonQuery(CommandType.Text, insertCarRenewalStatisticsDetailSql.Substring(0, insertCarRenewalStatisticsDetailSql.LastIndexOf(",")));
                    }

                }


            }


        }
        private List<List<MySqlParameter>> GetListMySqlParameterList(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            List<List<MySqlParameter>> ListMySqlParameterList = new List<List<MySqlParameter>>() { new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(9) } },new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(9) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(10) } },new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(10) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(11) } },new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(11) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(12) } }, new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(12) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(14) } }, new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(14) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(15) } },new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(15) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(16) } }, new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(16) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(17) } },new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(17) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeStart.AddHours(18) } }, new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeStart", Value = dateTimeStart.AddHours(18) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "dateInTimeEnd", Value = dateTimeEnd} }
            };
            return ListMySqlParameterList;
        }
        void AppointmentStatisticsDetail(List<MySqlParameter> ps)
        {
            #region 插入昨天预约数量
            var insertAppointmentStatisticsDetailSql = string.Format(@"USE  bihu_analytics; INSERT  INTO tj_appointmentdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,CustomerName,CategoryName,StatusCreateTime,BizEndDate,CustomerStatusName,CreateTime,CategoryInfoId,BizEndDateYear,StatusCreateTime_YearAndMonth) 
SELECT u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0   or cs.CategoryInfo is null  THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,crw.create_time AS StatusCreateTime,
crw.BizEndTime   AS BizEndDate,
'预约到店' AS CustomerStatusName ,
NOW() AS CreateTime,
u.CategoryInfoId as CategoryInfoId,
YEAR(crw.BizEndTime)   as BizEndDateYear,
DATE_FORMAT(crw.create_time,'%Y-%m')   as StatusCreateTime_YearAndMonth


FROM {0}.bx_consumer_review AS crw 
Inner JOIN {1}.bx_userinfo AS u
ON crw.b_uid=u.id
LEFT JOIN {2}.bx_userinfo_renewal_info AS urio
ON u.Id=urio.b_uid
LEFT JOIN {3}.bx_customercategories AS cs
ON u.CategoryInfoId=cs.Id
LEFT JOIN  {4}.bx_agent AS a 
ON u.Agent=a.Id

WHERE crw.create_time>=?dateInTimeStart  AND crw.create_time<=?dateInTimeEnd    AND crw.BizEndTime>'1900-01-01' AND crw.status=20", dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName);
            #endregion
            var insertCount = _mySqlHelper.ExecuteNonQuery(insertAppointmentStatisticsDetailSql, ps.ToArray());
        }
        void AnswerCallTimesStatisticsDetail(List<MySqlParameter> ps)
        {

            #region 插入昨天有效通话数
            var insertAnswerCallTimsDetailSql = string.Format(@" USE  bihu_analytics;INSERT  INTO tj_answercalltimesdetail_record (buId,AgentId,AgentName,ParentAgentId,TopAgentId,LicenseNo,CarVIN,EngineNo,CustomerName,CategoryName,StatusCreateTime,BizEndDate,CustomerStatusName,CreateTime,CategoryInfoId,BizEndDateYear,StatusCreateTime_YearAndMonth) 
SELECT u.Id AS buId,u.Agent AS AgentId,a.agentName AS AgentName, CASE WHEN a.ParentAgent=0 THEN u.Agent ELSE a.ParentAgent END AS ParentAgentId,a.TopAgentId AS TopAgentId,u.LicenseNo AS LicenseNo,
u.CarVIN AS CarVIN ,u.EngineNo AS EngineNo,urio.client_name AS CustomerName,CASE WHEN u.CategoryInfoId=0  or cs.CategoryInfo is null  THEN '未分类' ELSE cs.CategoryInfo END AS CategoryName,rh.createtime AS StatusCreateTime,
CASE WHEN YEAR(bi2.BizEndDate) > YEAR(IFNULL(cr.LastBizEndDate,'1900-01-01')) THEN bi2.BizEndDate ELSE IFNULL(cr.LastBizEndDate,'1900-01-01') END   AS BizEndDate,


      CASE
WHEN u.IsReView = 0 THEN '未跟进'
WHEN u.IsReView = 9 THEN '已出单'
WHEN u.IsReView IN(4, 16) THEN '战败'
WHEN u.IsReView not in(0,9,4,16,20) THEN '跟进中'
WHEN u.IsReView = 20 THEN '预约到店' END
   AS CustomerStatusName,
NOW() AS CreateTime,
u.CategoryInfoId as CategoryInfoId,

CASE WHEN YEAR(bi2.BizEndDate) > YEAR(IFNULL(cr.LastBizEndDate,'1900-01-01')) THEN YEAR(bi2.BizEndDate) ELSE YEAR(IFNULL(cr.LastBizEndDate,'1900-01-01')) END as BizEndDateYear,
CASE WHEN YEAR(bi2.BizEndDate) > YEAR(IFNULL(cr.LastBizEndDate,'1900-01-01'))THEN DATE_FORMAT(bi2.BizEndDate, '%Y-%m') ELSE DATE_FORMAT(IFNULL(cr.LastBizEndDate,'1900-01-01'), '%Y-%m') END as StatusCreateTime_YearAndMonth

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
ON u.Agent = a.Id
LEFT JOIN
 {5}.bx_batchrenewal_item AS bi2  ON u.id=bi2.buid AND  bi2.isnew=1
WHERE rh.createtime >=?dateInTimeStart AND rh.createtime <=?dateInTimeEnd AND rh.AnswerState = 1  and (bi2.BizEndDate>'1900-01-01' or cr.LastBizEndDate>'1900-01-01')", dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName, dataBaseName);
            #endregion
            var insertCount = _mySqlHelper.ExecuteNonQuery(insertAnswerCallTimsDetailSql, ps.ToArray());

        }
        #region 续保统计
        public List<FollowUpAllocationVM> GetFollowUpAllocationResult(DateTime bizEndDate, List<int> agentIds)
        {
            string getFollowUpAllocationResultSql = string.Format(@"SELECT CategoryName,
SUM(CASE WHEN CustomerStatus= -1 THEN 1 ELSE 0 END)AS NotFollowUpCount,
 SUM(CASE WHEN CustomerStatus IN(10001, 20) THEN 1 ELSE 0 END ) AS FollowUpCount,
     SUM(CASE WHEN CustomerStatus IN(4, 16) THEN 1 ELSE 0 END ) AS DefeatCount,
      SUM(CASE WHEN CustomerStatus = 9 THEN 1 ELSE 0 END) AS OutOrderCount,
Count(id) AS TaskCount
FROM bihu_analytics.tj_reviewdetail_record
WHERE BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND Deleted=0 GROUP BY CategoryName", string.Join(",", agentIds));
            var newBizEndDate = bizEndDate.AddMonths(1);
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDateStart", Value = Convert.ToDateTime(bizEndDate.Year + "-" + bizEndDate.Month + "-01") }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDateEnd", Value = Convert.ToDateTime(newBizEndDate.Year + "-" + newBizEndDate.Month + "-01") } };
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, getFollowUpAllocationResultSql, ps.ToArray()).ToList<FollowUpAllocationVM>().ToList();
        }

        public List<FollowUpAllocationResultAboutAgentVmTemp> GetFollowUpAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds)
        {
            var newBizEndDate = bizEndDate.AddMonths(1);
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDateStart", Value = Convert.ToDateTime(bizEndDate.Year + "-" + bizEndDate.Month + "-01") }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDateEnd", Value = Convert.ToDateTime(newBizEndDate.Year + "-" + newBizEndDate.Month + "-01") } };
            StringBuilder getFollowUpAllocationResultAboutAgentStringBuilderl = new StringBuilder(string.Format(@"SELECT CategoryName,AgentName,AgentId,
SUM(CASE WHEN CustomerStatus= -1 THEN 1 ELSE 0 END)AS NotFollowUpCount,
 SUM(CASE WHEN CustomerStatus IN(10001, 20) THEN 1 ELSE 0 END ) AS FollowUpCount,
     SUM(CASE WHEN CustomerStatus IN(4, 16) THEN 1 ELSE 0 END ) AS DefeatCount,
      SUM(CASE WHEN CustomerStatus = 9 THEN 1 ELSE 0 END) AS OutOrderCount,
Count(id) AS TaskCount
FROM bihu_analytics.tj_reviewdetail_record
WHERE BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd  AND AgentId IN({0}) AND Deleted=0 ", string.Join(",", agentIds)));
            if (!string.IsNullOrWhiteSpace(customerCategory))
            {
                getFollowUpAllocationResultAboutAgentStringBuilderl.Append(" AND CategoryName=?CategoryName ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
            }
            getFollowUpAllocationResultAboutAgentStringBuilderl.Append(" GROUP BY CategoryName,AgentId,AgentName");
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, getFollowUpAllocationResultAboutAgentStringBuilderl.ToString(), ps.ToArray()).ToList<FollowUpAllocationResultAboutAgentVmTemp>().ToList();
        }
        public List<OutOrderOrDefeatAllocationDetailsTemp> GetOutOrderAllocationResult(DateTime bizEndDate, string statusIdStr, List<int> agentIds)
        {

            return GetOutOrderOrDefeatAllocationResult(bizEndDate, statusIdStr, agentIds);
        }

        public List<OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp> GetOutOrderAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusIdStr)
        {
            return GetOutOrderOrDefeatAllocationResultAboutAgent(bizEndDate, customerCategory, agentIds, statusIdStr);
        }
        public List<OutOrderOrDefeatAllocationDetailsTemp> GetDefeatAllocationResult(DateTime bizEndDate, string statusStr, List<int> agentIds)
        {
            return GetOutOrderOrDefeatAllocationResult(bizEndDate, statusStr, agentIds);
        }

        public List<OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp> GetDefeatAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusStr)
        {
            return GetOutOrderOrDefeatAllocationResultAboutAgent(bizEndDate, customerCategory, agentIds, statusStr);
        }
        public List<StatisticsCellDetail> GetFollowUpAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, bool isSingleStatusSearch, bool isSingleCategorySearch, int pageIndex, int pageSize)
        {
            StringBuilder strWhere = new StringBuilder();
            if (isSingleCategorySearch)
            {
                strWhere.Append(string.Format(" AND CategoryName='{0}'", categoryName));
            }
            if (isSingleStatusSearch)
            {
                strWhere.Append(string.Format(" AND CustomerStatus IN({0})", statusIdStr));

            }
            var sql = "";
            if (searchAgentId == -1)
            {
                sql = string.Format(@"select LicenseNo,CarVIN,BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,CreateTime 
                                    from (
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND AgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND ParentAgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND TopAgentId={3} AND Deleted=0 {2}) t order by id limit {4},{5}", bizEndDate.Year + "-" + bizEndDate.Month + "-01", bizEndDate.AddMonths(1).Year + "-" + bizEndDate.AddMonths(1).Month + "-01", strWhere.ToString(), agentId, (pageIndex - 1) * pageSize, pageSize);
            }
            else
            {
                sql = string.Format(@"SELECT LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND AgentId={3} AND Deleted=0 {2}
                                    order by id limit {4},{5}", bizEndDate.Year + "-" + bizEndDate.Month + "-01", bizEndDate.AddMonths(1).Year + "-" + bizEndDate.AddMonths(1).Month + "-01", strWhere.ToString(), searchAgentId, (pageIndex - 1) * pageSize, pageSize);
            }

            return _mySqlHelper.ExecuteDataSet(CommandType.Text, sql, null).ToList<StatisticsCellDetail>().ToList();
        }

        public List<StatisticsCellDetail> GetOutOrderOrDefeatAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, bool isAboutMonth, int pageIndex, int pageSize)
        {
            StringBuilder strWhere = new StringBuilder();
            if (isSingleCategorySearch)
            {
                strWhere.Append(string.Format(" AND CategoryName='{0}'", categoryName));

            }
            if (isAboutMonth)
            {
                if (isSingleMonthSearch)
                {
                    strWhere.Append(string.Format(" AND StatusCreateTime>='{0}' AND StatusCreateTime<'{1}'", GetNewTime(bizEndDate, month, "-") + "-01", Convert.ToDateTime(GetNewTime(bizEndDate, month, "-") + "-01").AddMonths(1).ToString("yyyy-MM-dd")));
                }
                else
                {
                    strWhere.Append(string.Format(" AND StatusCreateTime>='{0}' AND StatusCreateTime<'{1}'", bizEndDate.AddMonths(-3).Year + "-" + bizEndDate.AddMonths(-3).Month + "-01", bizEndDate.AddMonths(1).Year + "-" + bizEndDate.AddMonths(1).Month + "-01"));

                }
                strWhere.Append(string.Format(" AND CustomerStatus IN({0})", statusIdStr));
            }
            var sql = "";
            if (searchAgentId == -1)
            {
                sql = string.Format(@"select LicenseNo,CarVIN,BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,CreateTime 
                                    from (
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND AgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND ParentAgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND TopAgentId={3} AND Deleted=0 {2}) t order by id limit {4},{5}", bizEndDate.Year + "-" + bizEndDate.Month + "-01", bizEndDate.AddMonths(1).Year + "-" + bizEndDate.AddMonths(1).Month + "-01", strWhere.ToString(), agentId, (pageIndex - 1) * pageSize, pageSize);
            }
            else
            {
                sql = string.Format(@"SELECT LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE BizEndDate>='{0}' AND BizEndDate<'{1}' AND AgentId={3} AND Deleted=0 {2} order by id limit {4},{5}", bizEndDate.Year + "-" + bizEndDate.Month + "-01", bizEndDate.AddMonths(1).Year + "-" + bizEndDate.AddMonths(1).Month + "-01", strWhere.ToString(), searchAgentId, (pageIndex - 1) * pageSize, pageSize);
            }
            var aa = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<StatisticsCellDetail>().ToList();
            return aa;
        }


        #endregion

        #region 工作统计
        public List<OutOrderStatisticsVMTemp> GetOutOrderStatisticsResult(DateTime reviewTime, List<int> agentIds, string statusIdStr)
        {
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusCreateStartTime", Value = reviewTime.Year + "-" + reviewTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusCreateEndTime", Value = reviewTime.AddMonths(1).Year + "-" + reviewTime.AddMonths(1).Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStartTime", Value = reviewTime.Year + "-" + reviewTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEndTime", Value = reviewTime.AddMonths(4).Year + "-" + reviewTime.AddMonths(4).Month + "-01" } };
            string getOutOrderStatisticsResultSql = string.Format(@"SELECT CategoryName,Count(id) AS DataCountInMonth ,MONTH(BizEndDate) AS TimeInMonth FROM bihu_analytics.tj_reviewdetail_record  WHERE AgentId in({0}) AND  StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime AND  BizEndDate>=?BizEndDateStartTime AND BizEndDate <?BizEndDateEndTime AND  CustomerStatus IN({1}) AND Deleted=0   GROUP BY CategoryName,MONTH(BizEndDate)  Order By BizEndDate", string.Join(",", agentIds), statusIdStr);

            return _mySqlHelper.ExecuteDataTable(CommandType.Text, getOutOrderStatisticsResultSql, ps.ToArray()).ToList<OutOrderStatisticsVMTemp>().ToList();

        }

        public List<OutOrderStatisticsResultAboutAgentVMTemp> GetOutOrderStatisticsResultAboutAgent(DateTime reviewTime, string customerCategory, List<int> agentIds, string statusIdStr)
        {
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusCreateStartTime", Value = reviewTime.Year + "-" + reviewTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusCreateEndTime", Value = reviewTime.AddMonths(1).Year + "-" + reviewTime.AddMonths(1).Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStartTime", Value = reviewTime.Year + "-" + reviewTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEndTime", Value = reviewTime.AddMonths(4).Year + "-" + reviewTime.AddMonths(4).Month + "-01" } };

            StringBuilder getOutOrderStatisticsResultAboutAgentStringBuilder = new StringBuilder(string.Format(@"SELECT AgentName,AgentId,CategoryName,Count(id) AS OutOrderCountInMonth,MONTH(BizEndDate)  TimeInMonth FROM bihu_analytics.tj_reviewdetail_record  WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime  AND  BizEndDate>=?BizEndDateStartTime AND BizEndDate <?BizEndDateEndTime AND AgentId IN({0}) AND CustomerStatus IN({1}) AND Deleted=0", string.Join(",", agentIds), statusIdStr));
            if (!string.IsNullOrWhiteSpace(customerCategory))
            {
                getOutOrderStatisticsResultAboutAgentStringBuilder.Append(" AND CategoryName=?CategoryName ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
            }
            getOutOrderStatisticsResultAboutAgentStringBuilder.Append("  GROUP BY MONTH(BizEndDate),CategoryName,AgentName,AgentId  order by BizEndDate   ");
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, getOutOrderStatisticsResultAboutAgentStringBuilder.ToString(), ps.ToArray()).ToList<OutOrderStatisticsResultAboutAgentVMTemp>().ToList();
        }
        public List<WorkStatistics_FollowUpVM> GetFollowUpStatisticsResult(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds)
        {
            var agentIdStr = string.Join(",", agentIds);
            List<WorkStatistics_FollowUpVM> workStatistics_FollowUpVMList = new List<WorkStatistics_FollowUpVM>();
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StatusCreateStartTime", Value = reviewStartTime }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StatusCreateEndTime", Value = reviewEndTime }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = reviewStartTime.Year + "-" + reviewStartTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = reviewStartTime.AddMonths(4).Year + "-" + reviewStartTime.AddMonths(4).Month + "-01" } };
            string getAnswerCallTimesSql = string.Empty;
            string getAppointmentSql = string.Empty;
            string getDefeatSql = string.Empty;
            string getOutOrderSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(customerCategory))
            {
                getAnswerCallTimesSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_answercalltimesdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CategoryName=?CategoryName   GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);

                getAppointmentSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_appointmentdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CategoryName=?CategoryName    GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);

                getDefeatSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CategoryName=?CategoryName AND CustomerStatus in(4,16) AND Deleted=0   GROUP BY DATE_FORMAT(BizEndDate,'%m') order by  BizEndDate", agentIdStr);

                getOutOrderSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CustomerStatus=9 AND CategoryName=?CategoryName AND Deleted=0   GROUP BY DATE_FORMAT(BizEndDate,'%m') order by  BizEndDate", agentIdStr);

                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
            }
            else
            {
                getAnswerCallTimesSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_answercalltimesdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND  BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd AND AgentId in({0})    GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);

                getAppointmentSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_appointmentdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND  BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd AND AgentId in({0})   GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);

                getDefeatSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND  BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CustomerStatus IN(4,16)  AND Deleted=0    GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);

                getOutOrderSql = string.Format(@"SELECT MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND  BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND AgentId in({0}) AND CustomerStatus=9  AND Deleted=0    GROUP BY DATE_FORMAT(BizEndDate,'%m') order by BizEndDate", agentIdStr);
            }
            using (var _dbContext = new EntityContext())
            {

                var AnswerCallTimesData = _mySqlHelper.ExecuteDataTable(CommandType.Text, getAnswerCallTimesSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonth>().ToList();
                workStatistics_FollowUpVMList.Add(new WorkStatistics_FollowUpVM { StatisticsCategory = "有效通话数", DataCountInStatisticsCategory = AnswerCallTimesData.Sum(a => a.DataCountInMonth), WorkStatistics_FollowUpAboutMonthList = AnswerCallTimesData });

                var AppointmentData = _mySqlHelper.ExecuteDataTable(CommandType.Text, getAppointmentSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonth>().ToList();

                workStatistics_FollowUpVMList.Add(new WorkStatistics_FollowUpVM { StatisticsCategory = "预约数", DataCountInStatisticsCategory = AppointmentData.Sum(a => a.DataCountInMonth), WorkStatistics_FollowUpAboutMonthList = AppointmentData });


                var DefeatData = _mySqlHelper.ExecuteDataTable(CommandType.Text, getDefeatSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonth>().ToList();

                workStatistics_FollowUpVMList.Add(new WorkStatistics_FollowUpVM { StatisticsCategory = "战败数", DataCountInStatisticsCategory = DefeatData.Sum(a => a.DataCountInMonth), WorkStatistics_FollowUpAboutMonthList = DefeatData });

                var OutOrderData = _mySqlHelper.ExecuteDataTable(CommandType.Text, getOutOrderSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonth>().ToList();

                workStatistics_FollowUpVMList.Add(new WorkStatistics_FollowUpVM { StatisticsCategory = "出单数", DataCountInStatisticsCategory = OutOrderData.Sum(a => a.DataCountInMonth), WorkStatistics_FollowUpAboutMonthList = OutOrderData });
            }
            return workStatistics_FollowUpVMList;
        }

        public WorkStatistics_FollowUpStatisticsResultAboutAgentVM GetFollowUpStatisticsResultAboutAgent(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds)
        {
            var agentIdStr = string.Join(",", agentIds);

            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StatusCreateStartTime", Value = reviewStartTime }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StatusCreateEndTime", Value = reviewEndTime }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = reviewStartTime.Year + "-" + reviewStartTime.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = reviewStartTime.AddMonths(4).Year + "-" + reviewStartTime.AddMonths(4).Month + "-01" } };
            string getAgentAnswerCallTimesSql = string.Empty;
            string getAgentAppointmentSql = string.Empty;
            string getAgentDefeatSql = string.Empty;
            string getAgentOutOrderSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(customerCategory))
            {
                getAgentAnswerCallTimesSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_answercalltimesdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd  AND CategoryName=?CategoryName ";

                getAgentAppointmentSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_appointmentdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd  AND CategoryName=?CategoryName";

                getAgentDefeatSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd  AND CategoryName=?CategoryName AND CustomerStatus in(4,16) AND Deleted=0  ";

                getAgentOutOrderSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd  AND CustomerStatus=9 AND CategoryName=?CategoryName AND Deleted=0 ";

                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
            }
            else
            {
                getAgentAnswerCallTimesSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_answercalltimesdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd ";

                getAgentAppointmentSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_appointmentdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd ";

                getAgentDefeatSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd  AND CustomerStatus in(4,16) AND Deleted=0 ";

                getAgentOutOrderSql = @"SELECT AgentName,AgentId, MONTH(BizEndDate) as TimeInMonth,COUNT(id) as DataCountInMonth FROM bihu_analytics.tj_reviewdetail_record WHERE StatusCreateTime>=?StatusCreateStartTime AND StatusCreateTime<=?StatusCreateEndTime AND BizEndDate>=?BizEndDateStart AND  BizEndDate<?BizEndDateEnd AND CustomerStatus=9 AND Deleted=0 ";
            }
            getAgentAnswerCallTimesSql += string.Format(" AND AgentId IN({0})   GROUP BY MONTH(BizEndDate),AgentId  order by BizEndDate", agentIdStr);
            getAgentAppointmentSql += string.Format(" AND AgentId IN({0})     GROUP BY MONTH(BizEndDate),AgentId order by BizEndDate", agentIdStr);
            getAgentDefeatSql += string.Format(" AND AgentId IN({0})   GROUP BY MONTH(BizEndDate),AgentId  order by BizEndDate", agentIdStr);
            getAgentOutOrderSql += string.Format(" AND AgentId IN({0})    GROUP BY MONTH(BizEndDate),AgentId order by BizEndDate", agentIdStr);
            using (var _dbContext = new EntityContext())
            {

                #region 优化代码
                //                List<List<WorkStatistics_FollowUpAgentAboutMonthTemp>> workStatistics_FollowUpAgentAboutMonthTempList = new List<List<WorkStatistics_FollowUpAgentAboutMonthTemp>>();
                //                var agentAnswerCallTimesData=    Task.Run(() => {
                //                    return
                //_dbContext.Database.SqlQuery<WorkStatistics_FollowUpAgentAboutMonthTemp>(getAgentAnswerCallTimesSql, ps.ToArray()).ToList();
                //                });
                //            var    agentAppointmentData= Task.Run(() => {
                //                    return
                //_dbContext.Database.SqlQuery<WorkStatistics_FollowUpAgentAboutMonthTemp>(getAgentAppointmentSql, ps.ToArray()).ToList();
                //                });
                //                var agentDefeatData =    Task.Run(() => {
                //                    return
                //_dbContext.Database.SqlQuery<WorkStatistics_FollowUpAgentAboutMonthTemp>(getAgentDefeatSql, ps.ToArray()).ToList();
                //                });
                //                var agentOutOrderData=Task.Run(() => {
                //                    return
                //_dbContext.Database.SqlQuery<WorkStatistics_FollowUpAgentAboutMonthTemp>(getAgentOutOrderSql, ps.ToArray()).ToList();
                //                });
                //                Task.WaitAll(agentAnswerCallTimesData, agentAppointmentData, agentDefeatData, agentOutOrderData);
                //                workStatistics_FollowUpAgentAboutMonthTempList.Add(agentAnswerCallTimesData.Result);
                //                workStatistics_FollowUpAgentAboutMonthTempList.Add(agentAppointmentData.Result);
                //                workStatistics_FollowUpAgentAboutMonthTempList.Add(agentDefeatData.Result);
                //                workStatistics_FollowUpAgentAboutMonthTempList.Add(agentOutOrderData.Result);
                #endregion
                var agentAnswerCallTimesData = GetWorkTypeDataInWorkStatistics(_mySqlHelper.ExecuteDataTable(CommandType.Text, getAgentAnswerCallTimesSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonthTemp>().ToList(), agentIds, reviewStartTime);
                var agentAppointmentData = GetWorkTypeDataInWorkStatistics(_mySqlHelper.ExecuteDataTable(CommandType.Text, getAgentAppointmentSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonthTemp>().ToList(), agentIds, reviewStartTime);
                var agentDefeatData = GetWorkTypeDataInWorkStatistics(_mySqlHelper.ExecuteDataTable(CommandType.Text, getAgentDefeatSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonthTemp>().ToList(), agentIds, reviewStartTime);
                var agentOutOrderData = GetWorkTypeDataInWorkStatistics(_mySqlHelper.ExecuteDataTable(CommandType.Text, getAgentOutOrderSql, ps.ToArray()).ToList<WorkStatistics_FollowUpAgentAboutMonthTemp>().ToList(), agentIds, reviewStartTime);
                WorkStatistics_FollowUpStatisticsResultAboutAgentVM workStatistics_FollowUpStatisticsResultAboutAgentVM = new WorkStatistics_FollowUpStatisticsResultAboutAgentVM
                {
                    AgentAnswerCallTimesResult = agentAnswerCallTimesData,
                    AgentAppointmentResult = agentAppointmentData,
                    AgentDefeatResult = agentDefeatData,
                    AgentOutOrderResult = agentOutOrderData
                };
                #region 注释代码
                //foreach (var item in agentIds)
                //{

                //    var agentAnswerCallTimesModel = agentAnswerCallTimesData.FirstOrDefault(x => x.AgentId == item);
                //    var agentAppointmentModel = agentAppointmentData.FirstOrDefault(x => x.AgentId == item);
                //    var agentDefeatModel = agentDefeatData.FirstOrDefault(x => x.AgentId == item);
                //    var agentOutOrderModel = agentOutOrderData.FirstOrDefault(x => x.AgentId == item);
                //    WorkStatistics_FollowUpStatisticsResultAboutAgentVM workStatistics_FollowUpStatisticsResultAboutAgentVM = new WorkStatistics_FollowUpStatisticsResultAboutAgentVM()
                //    {
                //        AgentId = item,
                //        AgentName = agentAnswerCallTimesModel.AgentName,
                //        WorkStatistics_FollowUpVM_AboutWorkTypeList = new List<WorkStatistics_FollowUpVM> { new WorkStatistics_FollowUpVM { StatisticsCategory = "有效通话数", DataCountInStatisticsCategory = agentAnswerCallTimesModel.TotalCount, WorkStatistics_FollowUpAboutMonthList = agentAnswerCallTimesModel.WorkStatistics_FollowUpAgentAboutMonthList }, new WorkStatistics_FollowUpVM { StatisticsCategory = "预约数", DataCountInStatisticsCategory = agentAppointmentModel.TotalCount, WorkStatistics_FollowUpAboutMonthList = agentAppointmentModel.WorkStatistics_FollowUpAgentAboutMonthList }, new WorkStatistics_FollowUpVM { StatisticsCategory = "战败数", DataCountInStatisticsCategory = agentAppointmentModel.TotalCount, WorkStatistics_FollowUpAboutMonthList = agentAppointmentModel.WorkStatistics_FollowUpAgentAboutMonthList }, new WorkStatistics_FollowUpVM { StatisticsCategory = "出单数", DataCountInStatisticsCategory = agentAppointmentModel.TotalCount, WorkStatistics_FollowUpAboutMonthList = agentAppointmentModel.WorkStatistics_FollowUpAgentAboutMonthList } }
                //    };
                //    workStatistics_FollowUpStatisticsResultAboutAgentVMList.Add(workStatistics_FollowUpStatisticsResultAboutAgentVM);

                //}
                #endregion

                return workStatistics_FollowUpStatisticsResultAboutAgentVM;
            }
        }
        public List<StatisticsCellDetail> GetOutOrderStatisticsResultDetail(DateTime reviewTime, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, int pageIndex, int pageSize)
        {
            StringBuilder strWhere = new StringBuilder();
            if (isSingleCategorySearch)
            {
                strWhere.Append(string.Format(" AND CategoryName='{0}'", categoryName));
            }
            if (isSingleMonthSearch)
            {
                strWhere.Append(string.Format(" AND BizEndDate>='{0}' AND BizEndDate<'{1}' ", GetNewTime(reviewTime, month, "+") + "-01", Convert.ToDateTime(GetNewTime(reviewTime, month, "+") + "-01").AddMonths(1).ToString("yyyy-MM-dd")));
            }
            else
            {
                strWhere.Append(string.Format(" AND BizEndDate>='{0}' AND BizEndDate<'{1}'", reviewTime.Year + "-" + reviewTime.Month + "-01", reviewTime.AddMonths(4).Year + "-" + reviewTime.AddMonths(4).Month + "-01"));
            }
            strWhere.Append(string.Format(" AND CustomerStatus IN({0})", statusIdStr));

            var sql = "";
            if (searchAgentId == -1)
            {
                sql = string.Format(@"select LicenseNo,CarVIN,BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,CreateTime 
                                    from (
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND ParentAgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND TopAgentId={3} AND Deleted=0 {2}) t order by id limit {4},{5}", reviewTime.Year + "-" + reviewTime.Month + "-01", Convert.ToDateTime(reviewTime.Year + "-" + reviewTime.Month + "-01").AddMonths(1).ToString("yyyy-MM-dd"), strWhere.ToString(), agentId, (pageIndex - 1) * pageSize, pageSize);
            }
            else
            {
                sql = string.Format(@"SELECT LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} AND Deleted=0 {2} order by id limit {4},{5}", reviewTime.Year + "-" + reviewTime.Month + "-01", Convert.ToDateTime(reviewTime.Year + "-" + reviewTime.Month + "-01").AddMonths(1).ToString("yyyy-MM-dd"), strWhere.ToString(), searchAgentId, (pageIndex - 1) * pageSize, pageSize);
            }

            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<StatisticsCellDetail>().ToList();
        }
        public List<StatisticsCellDetail> GetAnswerCallTimesResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            return GetAnswerCallTimesOrAppointmentResultDetail(reviewStartTime, reviewEndTime, agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, "bihu_analytics.tj_answercalltimesdetail_record", pageIndex, pageSize);


        }
        public List<StatisticsCellDetail> GetAppointmentResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            return GetAnswerCallTimesOrAppointmentResultDetail(reviewStartTime, reviewEndTime, agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, "bihu_analytics.tj_appointmentdetail_record", pageIndex, pageSize);
        }
        public List<StatisticsCellDetail> GetOutOrderOrDefeatStatisticsResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string statusIdStr, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            StringBuilder strWhere = new StringBuilder();
            if (isSingleCategorySearch)
            {
                strWhere.Append(string.Format(" AND CategoryName='{0}'", categoryName));
            }
            if (isSingleMonthSearch)
            {
                strWhere.Append(string.Format(" AND BizEndDate>='{0}' AND BizEndDate<'{1}' ", GetNewTime(reviewStartTime, month, "+") + "-01", Convert.ToDateTime(GetNewTime(reviewStartTime, month, "+") + "-01").AddMonths(1).ToString("yyyy-MM-dd")));
            }
            else
            {
                strWhere.Append(string.Format(" AND BizEndDate>='{0}' AND BizEndDate<'{1}'", reviewStartTime.Year + "-" + reviewStartTime.Month + "-01", reviewStartTime.AddMonths(4).Year + "-" + reviewStartTime.AddMonths(4).Month + "-01"));
            }
            strWhere.Append(string.Format(" AND CustomerStatus IN({0})", statusIdStr));
            var sql = "";
            if (searchAgentId == -1)
            {
                sql = string.Format(@"select LicenseNo,CarVIN,BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,CreateTime 
                                    from (
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND ParentAgentId={3} AND Deleted=0 {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND TopAgentId={3} AND Deleted=0 {2}) t order by id limit {4},{5}", reviewStartTime.ToString("yyyy-MM-dd"), reviewEndTime.ToString("yyyy-MM-dd"), strWhere.ToString(), agentId, (pageIndex - 1) * pageSize, pageSize);
            }
            else
            {
                sql = string.Format(@"SELECT LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM bihu_analytics.tj_reviewdetail_record
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} AND Deleted=0 {2} order by id limit {4},{5}", reviewStartTime.ToString("yyyy-MM-dd"), reviewEndTime.ToString("yyyy-MM-dd"), strWhere.ToString(), searchAgentId, (pageIndex - 1) * pageSize, pageSize);
            }
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<StatisticsCellDetail>().ToList();

        }
        private List<OutOrderOrDefeatAllocationDetailsTemp> GetOutOrderOrDefeatAllocationResult(DateTime bizEndDate, string statusStr, List<int> agentIds)
        {
            string getOutOrderOrDefeatAllocationResultsql = string.Format(@"SELECT CategoryName,DATE_FORMAT(StatusCreateTime,'%c')As  TimeInMonth ,
COUNT(id) AS CountInMonth
FROM bihu_analytics.tj_reviewdetail_record
WHERE BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND CustomerStatus in({0}) AND AgentId in({1}) AND  StatusCreateTime>=?StatusStartCreateTime AND  StatusCreateTime<?StatusEndCreateTime AND Deleted=0 GROUP BY StatusCreateTime_YearAndMonth,CategoryName", statusStr, string.Join(", ", agentIds));
            var newBizEndDate = bizEndDate.AddMonths(1);
            var newStatusStartCreateTime = bizEndDate.AddMonths(-3);
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = bizEndDate.Year + "-" + bizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = newBizEndDate.Year + "-" + newBizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusStartCreateTime", Value = newStatusStartCreateTime.ToString("yyyy-MM-dd") }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusEndCreateTime", Value = newBizEndDate.ToString("yyyy-MM-dd") } };
            string getCategoryTaskCount = string.Format("SELECT COUNT(Id) AS CategoryTaskCount,CategoryName  FROM bihu_analytics.tj_reviewdetail_record WHERE BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND  AgentId in({0}) AND Deleted=0  GROUP BY CategoryName", string.Join(", ", agentIds));
            List<MySqlParameter> ps1 = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = bizEndDate.Year + "-" + bizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = newBizEndDate.Year + "-" + newBizEndDate.Month + "-01" } };
            var categoryTaskCountVmLIst = _mySqlHelper.ExecuteDataTable(CommandType.Text, getCategoryTaskCount, ps1.ToArray()).ToList<CategoryTaskCountVm>();
            var outOrderOrDefeatAllocationDetailsTempList = _mySqlHelper.ExecuteDataTable(CommandType.Text, getOutOrderOrDefeatAllocationResultsql, ps.ToArray()).ToList<OutOrderOrDefeatAllocationDetailsTemp>().ToList();
            return (from a in categoryTaskCountVmLIst
                    join b in outOrderOrDefeatAllocationDetailsTempList
                    on a.CategoryName equals b.CategoryName
                    select new OutOrderOrDefeatAllocationDetailsTemp
                    {
                        CategoryName = a.CategoryName,
                        CountInMonth = b.CountInMonth,
                        TaskCount = a.CategoryTaskCount,
                        TimeInMonth = b.TimeInMonth
                    }).ToList();
        }
        private List<OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp> GetOutOrderOrDefeatAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusStr)
        {
            var newBizEndDate = bizEndDate.AddMonths(1);
            var newStatusStartCreateTime = bizEndDate.AddMonths(-3);
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = bizEndDate.Year + "-" + bizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = newBizEndDate.Year + "-" + newBizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusStartCreateTime", Value = newStatusStartCreateTime.ToString("yyyy-MM-dd") }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "StatusEndCreateTime", Value = newBizEndDate.ToString("yyyy-MM-dd") } };
            StringBuilder getOutOrderOrDefeatAllocationResultAboutAgentStringBuilder = new StringBuilder(string.Format(@"SELECT CategoryName,AgentId,AgentName,DATE_FORMAT(StatusCreateTime,'%c')As  TimeInMonth,
COUNT(id) AS CountInMonth
FROM bihu_analytics.tj_reviewdetail_record
WHERE BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd AND CustomerStatus in({0})  AND StatusCreateTime>=?StatusStartCreateTime AND  StatusCreateTime<?StatusEndCreateTime  AND AgentId IN({1}) AND Deleted=0", statusStr, string.Join(",", agentIds)));


            StringBuilder getAgentCategoryTaskCountStringBuilder = new StringBuilder(string.Format(@"SELECT AgentName,AgentId,COUNT(Id) AS CategoryTaskCount,CategoryName  FROM  bihu_analytics.tj_reviewdetail_record WHERE  BizEndDate>=?BizEndDateStart AND BizEndDate<?BizEndDateEnd And AgentId IN({0}) AND Deleted=0", string.Join(",", agentIds)));
            List<MySqlParameter> ps1 = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateStart", Value = bizEndDate.Year + "-" + bizEndDate.Month + "-01" }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "BizEndDateEnd", Value = newBizEndDate.Year + "-" + newBizEndDate.Month + "-01" } };
            if (!string.IsNullOrWhiteSpace(customerCategory))
            {
                getAgentCategoryTaskCountStringBuilder.Append(" AND CategoryName=?CategoryName  ");
                getOutOrderOrDefeatAllocationResultAboutAgentStringBuilder.Append("  AND CategoryName=?CategoryName ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
                ps1.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CategoryName", Value = customerCategory });
            }
            getOutOrderOrDefeatAllocationResultAboutAgentStringBuilder.Append(" GROUP BY CategoryName,AgentName,AgentId,StatusCreateTime_YearAndMonth");
            getAgentCategoryTaskCountStringBuilder.Append(" GROUP BY CategoryName,AgentName,AgentId");
            var agentCategoryTaskCountVMList = _mySqlHelper.ExecuteDataTable(CommandType.Text, getAgentCategoryTaskCountStringBuilder.ToString(), ps1.ToArray()).ToList<AgentCategoryTaskCountVM>().ToList();
            var outOrderOrDefeatAllocationResultAboutAgentDetailsTempList = _mySqlHelper.ExecuteDataTable(CommandType.Text, getOutOrderOrDefeatAllocationResultAboutAgentStringBuilder.ToString(), ps.ToArray()).ToList<OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp>().ToList();
            return (from a in agentCategoryTaskCountVMList
                    join b in outOrderOrDefeatAllocationResultAboutAgentDetailsTempList
                    on new { a.AgentId, a.CategoryName } equals new { b.AgentId, b.CategoryName }


                    select new OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp { AgentId = a.AgentId, AgentName = a.AgentName, CategoryName = a.CategoryName, CountInMonth = b.CountInMonth, TimeInMonth = b.TimeInMonth, TaskCount = a.CategoryTaskCount }).ToList();
        }

        private List<WorkStatistics_FollowUpAboutAgent> GetWorkTypeDataInWorkStatistics(List<WorkStatistics_FollowUpAgentAboutMonthTemp> dataTemp, List<int> agentIds, DateTime reviewStartTime)
        {
            var data = dataTemp.GroupBy(x => new { x.AgentId, x.AgentName }).Select(x => new WorkStatistics_FollowUpAboutAgent { AgentId = x.Key.AgentId, AgentName = x.Key.AgentName, TotalCount = x.Sum(a => a.DataCountInMonth), WorkStatistics_FollowUpAgentAboutMonthList = x.Where(b => b.AgentId == x.Key.AgentId).Select(c => new WorkStatistics_FollowUpAgentAboutMonth { DataCountInMonth = c.DataCountInMonth, TimeInMonth = c.TimeInMonth }).ToList() }).ToList();
            #region 注释代码
            //var exceptAgentIds = data.Select(x => x.AgentId).Except(agentIds).ToList();
            //var _theFirstMonth = reviewStartTime.Month.ToString();
            //var _theSecondMonth = reviewStartTime.AddMonths(1).Month.ToString();
            //var _theThirdMonth = reviewStartTime.AddMonths(2).Month.ToString();
            //foreach (var item in exceptAgentIds)
            //{
            //    WorkStatistics_FollowUpAboutAgent workStatistics_FollowUpAboutAgent = new WorkStatistics_FollowUpAboutAgent { AgentId = item, AgentName ="", TotalCount = 0, WorkStatistics_FollowUpAgentAboutMonthList = new List<WorkStatistics_FollowUpAgentAboutMonth> { new WorkStatistics_FollowUpAgentAboutMonth { TimeInMonth = _theFirstMonth, DataCountInMonth = 0 }, new WorkStatistics_FollowUpAgentAboutMonth { TimeInMonth = _theSecondMonth, DataCountInMonth = 0 }, new WorkStatistics_FollowUpAgentAboutMonth { TimeInMonth = _theThirdMonth, DataCountInMonth = 0 } } };
            //    data.Add(workStatistics_FollowUpAboutAgent);
            //}
            #endregion

            return data;
        }

        private List<StatisticsCellDetail> GetAnswerCallTimesOrAppointmentResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, string tableName, int pageIndex, int pageSize)
        {
            StringBuilder strWhere = new StringBuilder();
            if (isSingleCategorySearch)
            {
                strWhere.Append(string.Format(" AND CategoryName='{0}'", categoryName));
            }
            if (isSingleMonthSearch)
            {
                strWhere.Append(string.Format(" AND BizEndDate>='{0}' AND BizEndDate<'{1}'", GetNewTime(reviewStartTime, month, "+") + "-01", Convert.ToDateTime(GetNewTime(reviewStartTime, month, "+") + "-01").AddMonths(1).ToString("yyyy-MM-dd")));
            }
            else
            {
                strWhere.Append(string.Format("AND BizEndDate>='{0}' AND BizEndDate<='{1}'", reviewStartTime.Year + "-" + reviewStartTime.Month + "-01", reviewStartTime.AddMonths(4).Year + "-" + reviewStartTime.AddMonths(4).Month + "-01"));
            }
            var sql = "";
            if (searchAgentId == -1)
            {
                sql = string.Format(@"select LicenseNo,CarVIN,BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,CreateTime 
                                    from (
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM {6}
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM {6}
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND ParentAgentId={3} {2}
                                    UNION
                                    SELECT id,LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM {6}
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND TopAgentId={3} {2}) t order by id limit {4},{5}", reviewStartTime.ToString("yyyy-MM-dd"), reviewEndTime.ToString("yyyy-MM-dd"), strWhere.ToString(), agentId, (pageIndex - 1) * pageSize, pageSize, tableName);
            }
            else
            {
                sql = string.Format(@"SELECT LicenseNo,CarVIN,DATE_FORMAT(BizEndDate, '%Y-%m-%d') AS BizEndDate,CustomerName,CustomerStatusName,CategoryName,AgentName,
	                                    DATE_FORMAT(
		                                    StatusCreateTime,
		                                    '%Y-%m-%d'
	                                    ) AS CreateTime
                                    FROM {6}
                                    WHERE StatusCreateTime>='{0}' AND StatusCreateTime<'{1}' AND AgentId={3} {2} order by id limit {4},{5}", reviewStartTime.ToString("yyyy-MM-dd"), reviewEndTime.ToString("yyyy-MM-dd"), strWhere.ToString(), searchAgentId, (pageIndex - 1) * pageSize, pageSize, tableName);
            }
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<StatisticsCellDetail>().ToList();

        }

        private string GetNewTime(DateTime time, string month, string op)
        {
            string newTime = string.Empty;
            string newYear = string.Empty;
            var newMonth = month.Length < 2 ? "0" + month : month;
            if (op == "+")
            {
                newYear = time.Month > int.Parse(month) ? time.AddYears(1).Year.ToString() : time.Year.ToString();
            }
            else
            {
                newYear = int.Parse(month) > time.Month ? time.AddYears(-1).Year.ToString() : time.Year.ToString();
            }
            newTime = newYear + "-" + newMonth;
            return newTime;
        }
        #region 到店统计

        #endregion
        #endregion


        #endregion

        #region 深圳人保表单
        #region 进场分析

        public EntryPeriodAnalysisVM AnalysisEntryPeriod(string entryDate_day, string entryDate_month, string entryDate_year, string topAgentIds)
        {

            try
            {

                var entryPeriodAnalysis_selectSql_day = _getAnalysisEntryPeriodSql("cameratime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ea.topagentid as TopAgentId,
                //ea.nine_eleven_cameracount as NineToElevenCameraCount,
                //ea.eleven_thirteen_cameracount as ElevenToThirteenCameraCount,
                //ea.thirteen_seventeen_cameracount as ThirteenToSeventeenCameraCount,
                //ea.otherhour_cameracount as OtherHourCameraCount
                //from bihu_analytics.tj_entrytimeintervalanalysis as ea where  ea.cameratime_year_month_day=?cameratime_year_month_day and ea.topagentid in ({0})", topAgentIds);
                #endregion
                var entryPeriodAnalysis_selectSql_month = _getAnalysisEntryPeriodSql("cameratime_year_month", topAgentIds);
                #region 注释代码
                //                    string.Format(@"select 
                //ea.topagentid as TopAgentId ,
                //sum(ea.nine_eleven_cameracount) as NineToElevenCameraCount,
                //sum(ea.eleven_thirteen_cameracount) as ElevenToThirteenCameraCount,
                //sum(ea.thirteen_seventeen_cameracount) as ThirteenToSeventeenCameraCount,
                //sum(ea.otherhour_cameracount) as OtherHourCameraCount
                //from bihu_analytics.tj_entrytimeintervalanalysis as ea where ea.cameratime_year_month=?cameratime_year_month and ea.topagentid in({0})
                //group by ea.topagentid", topAgentIds);
                #endregion
                var entryPeriodAnalysis_selectSql_year = _getAnalysisEntryPeriodSql("cameratime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ea.topagentid as TopAgentId ,
                //sum(ea.nine_eleven_cameracount) as NineToElevenCameraCount,
                //sum(ea.nine_eleven_cameracount) as ElevenToThirteenCameraCount,
                //sum(ea.thirteen_seventeen_cameracount) as ThirteenToSeventeenCameraCount,
                //sum(ea.otherhour_cameracount) as OtherHourCameraCount
                //from bihu_analytics.tj_entrytimeintervalanalysis as ea where ea.cameratime_year=?cameratime_year and ea.topagentid in({0})
                //group by ea.topagentid", topAgentIds);
                #endregion

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month_day", Value = entryDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month", Value = entryDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year", Value = entryDate_year } };

                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryPeriodAnalysis_selectSql_day, ps_selectSql_day.ToArray()).ToList<EntryPeriodAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryPeriodAnalysis_selectSql_month, ps_selectSql_month.ToArray()).ToList<EntryPeriodAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryPeriodAnalysis_selectSql_year, ps_selectSql_year.ToArray()).ToList<EntryPeriodAnalysisVM_PropertyClass>().ToList();
                return new EntryPeriodAnalysisVM() { EntryPeriodDayAnalysisVM = result_selectSql_day, EntryPeriodMonthAnalysisVM = result_selectSql_month, EntryPeriodYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();

            }
        }
        private string _getAnalysisEntryPeriodSql(string selectDateType, string topAgentIds)
        {

            return string.Format(@"select 
ea.topagentid as TopAgentId ,
sum(ea.nine_eleven_cameracount) as NineToElevenCameraCount,
sum(ea.eleven_thirteen_cameracount) as ElevenToThirteenCameraCount,
sum(ea.thirteen_seventeen_cameracount) as ThirteenToSeventeenCameraCount,
sum(ea.otherhour_cameracount) as OtherHourCameraCount
from bihu_analytics.tj_entrytimeintervalanalysis as ea where ea.{0}=?{1} and ea.topagentid in({2})
group by ea.topagentid", selectDateType, selectDateType, topAgentIds);

        }
        public EntryProportionAnalysisVM AnalysisEntryProportion(string entryDate_day, string entryDate_month, string entryDate_year, string topAgentIds)
        {
            try
            {
                var entryProportionAnalysis_select_day = _getAnalysisEntryProportion("cameratime_year_month_day", topAgentIds);
                #region  注释代码
                //                string.Format(@"select 
                //ea.topagentid as TopAgentId,
                //ea.renbaocameracount as RenBaoCameraCount,
                //ea.pingancameracount as PingAnCameraCount,
                //ea.taipingyangcameracount as TaiPingYangCameraCount,
                //ea.guoshoucaicameracount as GuoShouCaiCameraCount,
                //ea.othersourcecameracount as OtherSourceCameraCount
                //from  bihu_analytics.tj_entrytimeintervalanalysis as ea  where ea.cameratime_year_month_day=?cameratime_year_month_day and ea.topagentid in({0}) group by  ", topAgentIds);
                #endregion

                var entryProportionAnalysis_select_month = _getAnalysisEntryProportion("cameratime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ea.topagentid as TopAgentId,
                //sum(ea.renbaocameracount) as RenBaoCameraCount,
                //sum(ea.pingancameracount) as PingAnCameraCount,
                //sum(ea.taipingyangcameracount) as TaiPingYangCameraCount,
                //sum(ea.guoshoucaicameracount) as GuoShouCaiCameraCount,
                //sum(ea.othersourcecameracount) as OtherSourceCameraCount
                //from  bihu_analytics.tj_entrytimeintervalanalysis as ea  where ea.cameratime_year_month=?cameratime_year_month and ea.topagentid in({0})
                //group by ea.topagentid", topAgentIds);
                #endregion

                var entryProportionAnalysis_select_year = _getAnalysisEntryProportion("cameratime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ea.topagentid as TopAgentId,
                //sum(ea.renbaocameracount) as RenBaoCameraCount,
                //sum(ea.pingancameracount) as PingAnCameraCount,
                //sum(ea.taipingyangcameracount) as TaiPingYangCameraCount ,
                //sum(ea.guoshoucaicameracount) as GuoShouCaiCameraCount,
                //sum(ea.othersourcecameracount) as OtherSourceCameraCount
                //from  bihu_analytics.tj_entrytimeintervalanalysis as ea  where ea.cameratime_year=?cameratime_year and ea.topagentid in({0})
                //group by ea.topagentid", topAgentIds);
                #endregion


                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month_day", Value = entryDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month", Value = entryDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year", Value = entryDate_year } };


                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryProportionAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<EntryProportionAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryProportionAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<EntryProportionAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryProportionAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<EntryProportionAnalysisVM_PropertyClass>().ToList();

                return new EntryProportionAnalysisVM() { EntryProportionDayAnalysisVM = result_selectSql_day, EntryProportionMonthAnalysisVM = result_selectSql_month, EntryProportionYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string _getAnalysisEntryProportion(string selectDateType, string topAgentIds)
        {

            return string.Format(@"select 
ea.topagentid as TopAgentId,
sum(ea.renbaocameracount) as RenBaoCameraCount,
sum(ea.pingancameracount) as PingAnCameraCount,
sum(ea.taipingyangcameracount) as TaiPingYangCameraCount,
sum(ea.guoshoucaicameracount) as GuoShouCaiCameraCount,
sum(ea.othersourcecameracount) as OtherSourceCameraCount
from  bihu_analytics.tj_entrytimeintervalanalysis as ea  where ea.{0}=?{1} and ea.topagentid in({2})
group by ea.topagentid", selectDateType, selectDateType, topAgentIds);
        }
        public EntryFollowUpAnalysisVM AnalysisEntryFollowUp(string entryDate_day, string entryDate_month, string entryDate_year, string topAgentIds)
        {
            try
            {
                var entryFollowUpAnalysis_select_day = _getAnalysisEntryFollowUp("tj_entryfollowupanalysisaboutday", "cameratime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //t.topagentid as TopAgentId,
                //t.cameracount as CameraCount ,
                //t.nowquotecount as NowQuoteCount ,
                //t.nowinsurecount as NowInsureCount ,
                //t.notnowinsurecount as NotNowInsureCount ,
                //ifnull(CEIL(t.nowquotecount/t.cameracount)*100,0) as NowQuoteRate ,
                //ifnull(CEIL(t.nowinsurecount/t.cameracount)*100,0) as NowInsureRate
                //from  bihu_analytics.tj_entryfollowupanalysisaboutday as t  where t.cameratime_year_month_day=?cameratime_year_month_day and t.topagentid in({0})", topAgentIds);
                #endregion

                var entryFollowUpAnalysis_select_month = _getAnalysisEntryFollowUp("tj_entryfollowupanalysisaboutmonth", "cameratime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //t.topagentid as TopAgentId,
                //t.cameracount as CameraCount,
                //t.nowquotecount as NowQuoteCount ,
                //t.nowinsurecount as NowInsureCount,
                //t.notnowinsurecount as NotNowInsureCount,
                //ifnull(CEIL(t.nowquotecount/t.CameraCount)*100 ,0)as NowQuoteRate ,
                //ifnull(CEIL(t.nowinsurecount/t.CameraCount)*100 ,0)as NowInsureRate
                //from bihu_analytics.tj_entryfollowupanalysisaboutmonth as t
                //where t.cameratime_year_month=?cameratime_year_month and t.topagentid in({0})
                //group by t.topagentid", topAgentIds);
                #endregion

                var entryFollowUpAnalysis_select_year = _getAnalysisEntryFollowUp("tj_entryfollowupanalysisaboutyear", "cameratime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //t.topagentid as TopAgentId,
                //t.cameracount as CameraCount,
                //t.nowquotecount as NowQuoteCount,
                //t.nowinsurecount as NowInsureCount,
                //ifnull(CEIL(t.nowquotecount/t.CameraCount)*100 ,0)as NowQuoteRate ,
                //ifnull(CEIL(t.nowinsurecount/t.CameraCount)*100 ,0)as NowInsureRate
                //from bihu_analytics.tj_entryfollowupanalysisaboutyear as t
                //where t.cameratime_year=?cameratime_year and t.topagentid in({0})
                //group by t.topagentid", topAgentIds);
                #endregion


                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month_day", Value = entryDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year_month", Value = entryDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "cameratime_year", Value = entryDate_year } };

                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryFollowUpAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<EntryFollowUpAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryFollowUpAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<EntryFollowUpAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, entryFollowUpAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<EntryFollowUpAnalysisVM_PropertyClass>().ToList();
                return new EntryFollowUpAnalysisVM { EntryFollowUpDayAnalysisVM = result_selectSql_day, EntryFollowUpMonthAnalysisVM = result_selectSql_month, EntryFollowUpYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();

            }
        }

        private string _getAnalysisEntryFollowUp(string tableName, string selectDateType, string topAgentIds)
        {
            return string.Format(@"select 
t.topagentid as TopAgentId,
t.cameracount as CameraCount,
t.nowquotecount as NowQuoteCount ,
t.nowinsurecount as NowInsureCount,
t.notnowinsurecount as NotNowInsureCount,
ifnull(CONVERT(t.nowquotecount/t.CameraCount,DECIMAL(10, 2))*100 ,0)as NowQuoteRate ,
ifnull(CONVERT(t.nowinsurecount/t.CameraCount,DECIMAL(10, 2))*100 ,0)as NowInsureRate
from bihu_analytics.{0} as t
where t.{1}=?{2} and t.topagentid in({3})
group by t.topagentid", tableName, selectDateType, selectDateType, topAgentIds);
        }
        #endregion
        #region 报价分析
        public QuoteTimesAnalysisTempVM AnalysisQuoteTimes(string quoteDate_day, string quoteDate_month, string quoteDate_year, string topAgentIds)
        {
            try
            {
                var minAnalysisDate_select = @"select min(quotetime_year_month_day) from bihu_analytics.tj_quoteanalysis as qa ";
                var minAnalysisDateObj = _mySqlHelper.ExecuteScalar(CommandType.Text, minAnalysisDate_select, null);
                var minAnalysisDate = minAnalysisDateObj == DBNull.Value ? Convert.ToDateTime(quoteDate_day) : Convert.ToDateTime(minAnalysisDateObj);
                var nowAnalysisDate = Convert.ToDateTime(quoteDate_day);
                var diffDay = (nowAnalysisDate - minAnalysisDate).Days;
                var diffWeek = diffDay / 7.0;
                var weekSequentialCycle = diffWeek >= 5 ? 5 : Math.Floor(diffWeek);

                var diffMonth = (nowAnalysisDate.Year - minAnalysisDate.Year) * 12 + (nowAnalysisDate.Month - minAnalysisDate.Month);
                var monthSequentialCycle = diffMonth >= 3 ? 3 : diffMonth;
                List<string> weekSequentialList = new List<string>();
                List<string> monthSequentialList = new List<string>();
                if (weekSequentialCycle <= 0)
                {
                    weekSequentialList.Add("'" + quoteDate_day + "'");
                    weekSequentialCycle += 1;
                }
                else
                {

                    for (int i = 1; i <= weekSequentialCycle; i++)
                    {
                        weekSequentialList.Add("'" + nowAnalysisDate.AddDays(-i * 7).ToString("yyyy-MM-dd") + "'");
                    }
                }

                if (monthSequentialCycle <= 0)
                {
                    monthSequentialList.Add("'" + quoteDate_month + "'");
                    monthSequentialCycle += 1;
                }
                else
                {
                    for (int i = 1; i <= monthSequentialCycle; i++)
                    {
                        monthSequentialList.Add("'" + nowAnalysisDate.AddMonths(-i).ToString("yyyy-MM") + "'");
                    }

                }
                var weekSequential = string.Join(",", weekSequentialList);
                var monthSequential = string.Join(",", monthSequentialList);
                var quoteTimesAnalysis_select_day = _getAnalysisQuoteTimesAboutDayOrMonthSql("quotetime_year_month_day", weekSequential, weekSequentialCycle, topAgentIds);
                #region 注释代码
                //                string.Format(@" select 
                //qa.topagentid as TopAgentId,
                //qa.renbaoquotecount as RenBaoQuoteTimes ,
                //qa.pinganquotecount as PingAnQuoteTimes ,
                //qa.taipingyangquotecount as TaiPingYangQuoteTimes ,
                //qa.guoshoucaiquotecount as GuoShouCaiQuoteTimes,
                //qa.othersourcequotecount as OtherSourceQuoteTimes ,
                //(select 
                //sum(qa1.othersourcequotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month_day in({0})  and qa1.topagentid=qa.topagentid )/{1} as RenBaoAgoQuoteTimes,
                //(select 
                //sum(qa1.pinganquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month_day in({2}) and qa1.topagentid=qa.topagentid)/{3} as PingAnAgoQuoteTimes,
                //(select 
                //sum(qa1.taipingyangquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month_day in({4}) and qa1.topagentid=qa.topagentid)/{5} as TaiPingYangAgoQuoteTimes,
                //(select 
                //sum(qa1.guoshoucaiquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month_day in({5}) and qa1.topagentid=qa.topagentid)/{7} as GuoShouCaiAgoQuoteTimes,
                //(select 
                //sum(qa1.othersourcequotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month_day in({8}) and qa1.topagentid=qa.topagentid)/{9} as OtherSourceAgoQuoteTimes
                //from bihu_analytics.tj_quoteanalysis as qa
                //where qa.quotetime_year_month_day=?quotetime_year_month_day and qa.topagentid in({10})", weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, topAgentIds);

                #endregion

                var quoteTimesAnalysis_select_month = _getAnalysisQuoteTimesAboutDayOrMonthSql("quotetime_year_month", monthSequential, monthSequentialCycle, topAgentIds);
                #region 注释代码
                //                string.Format(@" select 
                //qa.topagentid as TopAgentId,
                //sum(qa.renbaoquotecount) as RenBaoQuoteTimes,
                //sum(qa.pinganquotecount) as PingAnQuoteTimes,
                //sum(qa.taipingyangquotecount) as TaiPingYangQuoteTimes,
                //sum(qa.guoshoucaiquotecount) as GuoShouCaiQuoteTimes,
                //sum(qa.othersourcequotecount) as OtherSourceQuoteTimes,
                //(select 
                //sum(qa1.renbaoquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month in({0}) and qa1.topagentid=qa.topagentid)/{1} as RenBaoAgoQuoteTimes,
                //(select 
                //sum(qa1.pinganquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month in({2}) and qa1.topagentid=qa.topagentid )/{3} as PingAnAgoQuoteTimes,
                //(select 
                //sum(qa1.taipingyangquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month in({4}) and qa1.topagentid=qa.topagentid)/{5} as TaiPingYangAgoQuoteTimes,
                //(select 
                //sum(qa1.guoshoucaiquotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month in({6}) and qa1.topagentid=qa.topagentid )/{7} as GuoShouCaiAgoQuoteTimes,
                //(select 
                //sum(qa1.othersourcequotecount)
                //from bihu_analytics.tj_quoteanalysis as qa1 where qa1.quotetime_year_month in({8}) and qa1.topagentid=qa.topagentid )/{9} as OtherSourceAgoQuoteTimes
                //from bihu_analytics.tj_quoteanalysis as qa
                //where qa.quotetime_year_month=?quotetime_year_month and qa.topagentid in({10})
                //group by qa.topagentid", monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, topAgentIds);
                #endregion

                var quoteTimesAnalysis_select_year = string.Format(@" select 
qa.topagentid as TopAgentId,
sum(qa.renbaoquotecount) as RenBaoQuoteTimes,
sum(qa.pinganquotecount) as PingAnQuoteTimes,
sum(qa.taipingyangquotecount) as TaiPingYangQuoteTimes,
sum(qa.guoshoucaiquotecount) as GuoShouCaiQuoteTimes ,
sum(qa.othersourcequotecount) as OtherSourceQuoteTimes
from bihu_analytics.tj_quoteanalysis as qa
where qa.quotetime_year=?quotetime_year and qa.topagentid in({0})
group by qa.topagentid", topAgentIds);

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year_month_day", Value = quoteDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year_month", Value = quoteDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year", Value = quoteDate_year } };


                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteTimesAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<QuoteTimesAnalysisTempVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteTimesAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<QuoteTimesAnalysisTempVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteTimesAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<QuoteTimesAnalysisTempVM_PropertyClass>().ToList();
                return new QuoteTimesAnalysisTempVM { QuoteTimesDayAnalysisTempVM = result_selectSql_day, QuoteTimesMonthAnalysisTempVM = result_selectSql_month, QuoteTimesYearAnalysisTempVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string _getAnalysisQuoteTimesAboutDayOrMonthSql(string selectDateType, string sequential, double sequentialCycle, string topAgentIds)
        {

            return string.Format(@" select 
qa.topagentid as TopAgentId,
sum(qa.renbaoquotecount) as RenBaoQuoteTimes,
sum(qa.pinganquotecount) as PingAnQuoteTimes,
sum(qa.taipingyangquotecount) as TaiPingYangQuoteTimes,
sum(qa.guoshoucaiquotecount) as GuoShouCaiQuoteTimes,
sum(qa.othersourcequotecount) as OtherSourceQuoteTimes,
(select 
sum(qa1.renbaoquotecount)
from bihu_analytics.tj_quoteanalysis as qa1 where qa1.{0} in({1}) and qa1.topagentid=qa.topagentid)/{2} as RenBaoAgoQuoteTimes,
(select 
sum(qa1.pinganquotecount)
from bihu_analytics.tj_quoteanalysis as qa1 where qa1.{3} in({4}) and qa1.topagentid=qa.topagentid )/{5} as PingAnAgoQuoteTimes,
(select 
sum(qa1.taipingyangquotecount)
from bihu_analytics.tj_quoteanalysis as qa1 where qa1.{6} in({7}) and qa1.topagentid=qa.topagentid)/{8} as TaiPingYangAgoQuoteTimes,
(select 
sum(qa1.guoshoucaiquotecount)
from bihu_analytics.tj_quoteanalysis as qa1 where qa1.{9} in({10}) and qa1.topagentid=qa.topagentid )/{11} as GuoShouCaiAgoQuoteTimes,
(select 
sum(qa1.othersourcequotecount)
from bihu_analytics.tj_quoteanalysis as qa1 where qa1.{12} in({13}) and qa1.topagentid=qa.topagentid )/{14} as OtherSourceAgoQuoteTimes
from bihu_analytics.tj_quoteanalysis as qa
where qa.{15}=?{16} and qa.topagentid in({17})
group by qa.topagentid", selectDateType, sequential, sequentialCycle, selectDateType, sequential, sequentialCycle, selectDateType, sequential, sequentialCycle, selectDateType, sequential, sequentialCycle, selectDateType, sequential, sequentialCycle, selectDateType, selectDateType, topAgentIds);
        }
        public QuoteActionAnalysisVM AnalysisQuoteAction(string quoteDate_day, string quoteDate_month, string quoteDate_year, string topAgentIds)
        {
            try
            {
                var quoteActionAnalysis_select_day = _getAnalysisQuoteActionSql("quotetime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //qa.topagentid as TopAgentId,
                //qa.nine_eleven_quotecount as NineToElevenQuoteTimes,
                //qa.eleven_thirteen_quotecount as ElevenToThirteenQuoteTimes,
                //qa.thirteen_seventeen_quotecount as ThirteenToSeventeenQuoteTimes,
                //qa.otherhour_quotecount as OtherHourQuoteTimes
                //from bihu_analytics.tj_quoteanalysis as qa where  qa.quotetime_year_month_day=?quotetime_year_month_day and qa.topagentid in ({0})", topAgentIds);
                #endregion

                var quoteActionAnalysis_select_month = _getAnalysisQuoteActionSql("quotetime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //qa.topagentid as TopAgentId ,
                //sum(qa.nine_eleven_quotecount) as NineToElevenQuoteTimes,
                //sum(qa.eleven_thirteen_quotecount) as ElevenToThirteenQuoteTimes,
                //sum(qa.thirteen_seventeen_quotecount) as ThirteenToSeventeenQuoteTimes,
                //sum(qa.otherhour_quotecount) as OtherHourQuoteTimes
                //from bihu_analytics.tj_quoteanalysis as qa where qa.quotetime_year_month=?quotetime_year_month and qa.topagentid in({0})
                //group by qa.topagentid", topAgentIds);
                #endregion

                var quoteActionAnalysis_select_year = _getAnalysisQuoteActionSql("quotetime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //qa.topagentid as TopAgentId ,
                //sum(qa.nine_eleven_quotecount) as NineToElevenQuoteTimes,
                //sum(qa.eleven_thirteen_quotecount) as ElevenToThirteenQuoteTimes,
                //sum(qa.thirteen_seventeen_quotecount) as ThirteenToSeventeenQuoteTimes,
                //sum(qa.otherhour_quotecount) as OtherHourQuoteTimes
                //from bihu_analytics.tj_quoteanalysis as qa where qa.quotetime_year=?quotetime_year and qa.topagentid in({0})
                //group by qa.topagentid", topAgentIds);
                #endregion


                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year_month_day", Value = quoteDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year_month", Value = quoteDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "quotetime_year", Value = quoteDate_year } };
                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteActionAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<QuoteActionAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteActionAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<QuoteActionAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, quoteActionAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<QuoteActionAnalysisVM_PropertyClass>().ToList();
                return new QuoteActionAnalysisVM { QuoteActionDayAnalysisVM = result_selectSql_day, QuoteActionMonthAnalysisVM = result_selectSql_month, QuoteActionYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
        private string _getAnalysisQuoteActionSql(string selectDateType, string topAgentIds)
        {
            return string.Format(@"select 
qa.topagentid as TopAgentId ,
sum(qa.nine_eleven_quotecount) as NineToElevenQuoteTimes,
sum(qa.eleven_thirteen_quotecount) as ElevenToThirteenQuoteTimes,
sum(qa.thirteen_seventeen_quotecount) as ThirteenToSeventeenQuoteTimes,
sum(qa.otherhour_quotecount) as OtherHourQuoteTimes
from bihu_analytics.tj_quoteanalysis as qa where qa.{0}=?{1} and qa.topagentid in({2})
group by qa.topagentid", selectDateType, selectDateType, topAgentIds);
        }
        #endregion
        #region 投保分析

        public InsureDistributionAnalysisTempVM AnalysisInsureDistribution(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            try
            {

                var minAnalysisDate_select = @"select min(iib.insuretime_year_month_day) from bihu_analytics.tj_insuredistributionanalysis as iib ";
                var minAnalysisDateObj = _mySqlHelper.ExecuteScalar(CommandType.Text, minAnalysisDate_select, null);
                var minAnalysisDate = minAnalysisDateObj == DBNull.Value ? Convert.ToDateTime(insureDate_day) : Convert.ToDateTime(minAnalysisDateObj);
                var nowAnalysisDate = Convert.ToDateTime(insureDate_day);
                var diffDay = (nowAnalysisDate - minAnalysisDate).Days;
                var diffWeek = diffDay / 7.0;
                var weekSequentialCycle = diffWeek >= 5 ? 5 : Math.Floor(diffWeek);

                var diffMonth = (nowAnalysisDate.Year - minAnalysisDate.Year) * 12 + (nowAnalysisDate.Month - minAnalysisDate.Month);
                var monthSequentialCycle = diffMonth >= 3 ? 3 : diffMonth;
                List<string> weekSequentialList = new List<string>();
                List<string> monthSequentialList = new List<string>();
                if (weekSequentialCycle <= 0)
                {
                    weekSequentialList.Add("'" + insureDate_day + "'");
                    weekSequentialCycle += 1;
                }
                else
                {

                    for (int i = 1; i <= weekSequentialCycle; i++)
                    {
                        weekSequentialList.Add("'" + nowAnalysisDate.AddDays(-i * 7).ToString("yyyy-MM-dd") + "'");
                    }
                }

                if (monthSequentialCycle <= 0)
                {
                    monthSequentialList.Add("'" + insureDate_month + "'");
                    monthSequentialCycle += 1;
                }
                else
                {
                    for (int i = 1; i <= monthSequentialCycle; i++)
                    {
                        monthSequentialList.Add("'" + nowAnalysisDate.AddMonths(-i).ToString("yyyy-MM") + "'");
                    }

                }
                var weekSequential = string.Join(",", weekSequentialList);
                var monthSequential = string.Join(",", monthSequentialList);

                var InsureDistributionAnalysis_select_day_isLastNewCar = _getAnalysisInsureDistributionAboutDayOrMonthSql("insuretime_year_month_day", weekSequential, weekSequentialCycle, 1, topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iib.topagentid as TopAgentId,
                //iib.renbaoinsurecount as RenBaoTimes ,
                //iib.pinganinsurecount as PingAnTimes ,
                //iib.taipingyanginsurecount as TaiPingYangTimes ,
                //iib.guoshoucaiinsurecount as GuoShouCaiTimes,
                //iib.othersourceinsurecount as OtherSourceTimes ,
                //(select 
                //sum(iib1.renbaoinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({0})  and iib1.topagentid=iib.topagentid  and iib1.islastnewcar=1 )/{1} as RenBaoAgoTimes,
                //(select 
                //sum(iib1.pinganinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({2}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{3} as PingAnAgoTimes,
                //(select 
                //sum(iib1.taipingyanginsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({4}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{5} as TaiPingYangAgoTimes,
                //(select 
                //sum(iib1.guoshoucaiinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({5}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{7} as GuoShouCaiAgoTimes,
                //(select 
                //sum(iib1.othersourceinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({8}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{9} as OtherSourceAgoTimes
                //from bihu_analytics.tj_insuredistributionanalysis as iib
                //where iib.insuretime_year_month_day=?insuretime_year_month_day and iib.topagentid in({10}) and iib.islastnewcar=1", weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, weekSequential, weekSequentialCycle, topAgentIds);
                #endregion

                var InsureDistributionAnalysis_select_month_isLastNewCar = _getAnalysisInsureDistributionAboutDayOrMonthSql("insuretime_year_month", monthSequential, monthSequentialCycle, 1, topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iib.topagentid as TopAgentId,
                //iib.renbaoinsurecount as RenBaoTimes ,
                //iib.pinganinsurecount as PingAnTimes ,
                //iib.taipingyanginsurecount as TaiPingYangTimes ,
                //iib.guoshoucaiinsurecount as GuoShouCaiTimes,
                //iib.othersourceinsurecount as OtherSourceTimes ,
                //(select 
                //sum(iib1.renbaoinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({0})  and iib1.topagentid=iib.topagentid  and iib1.islastnewcar=1 )/{1} as RenBaoAgoTimes,
                //(select 
                //sum(iib1.pinganinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({2}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{3} as PingAnAgoTimes,
                //(select 
                //sum(iib1.taipingyanginsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({4}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{5} as TaiPingYangAgoTimes,
                //(select 
                //sum(iib1.guoshoucaiinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({5}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{7} as GuoShouCaiAgoTimes,
                //(select 
                //sum(iib1.othersourceinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({8}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=1)/{9} as OtherSourceAgoTimes
                //from bihu_analytics.tj_insuredistributionanalysis as iib
                //where iib.insuretime_year_month=?insuretime_year_month and iib.topagentid in({10}) and iib.islastnewcar=1", monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, topAgentIds);
                #endregion

                var InsureDistributionAnalysis_select_year_isLastNewCar = _getAnalysisInsureDistributionAboutYear(1, topAgentIds);
                #region 注释代码
                //                string.Format(@" select 
                ////iib.topagentid as TopAgentId,
                ////sum(iib.renbaoinsurecount) as RenBaoTimes,
                ////sum(iib.pinganinsurecount) as PingAnTimes,
                ////sum(iib.taipingyanginsurecount) as TaiPingYangTimes,
                ////sum(iib.guoshoucaiinsurecount) as GuoShouCaiTimes ,
                ////sum(iib.othersourceinsurecount) as OtherSourceTimes
                ////from bihu_analytics.tj_insuredistributionanalysis as iib
                ////where iib.insuretime_year=?insuretime_year and iib.topagentid in({0}) and iib.islastnewcar=1
                ////group by iib.topagentid", topAgentIds);
                #endregion



                var InsureDistributionAnalysis_select_day_isNotLastNewCar = _getAnalysisInsureDistributionAboutDayOrMonthSql("insuretime_year_month_day", weekSequential, weekSequentialCycle, 0, topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iib.topagentid as TopAgentId,
                //iib.renbaoinsurecount as RenBaoTimes ,
                //iib.pinganinsurecount as PingAnTimes ,
                //iib.taipingyanginsurecount as TaiPingYangTimes ,
                //iib.guoshoucaiinsurecount as GuoShouCaiTimes,
                //iib.othersourceinsurecount as OtherSourceTimes ,
                //(select 
                //sum(iib1.renbaoinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({0})  and iib1.topagentid=iib.topagentid  and iib1.islastnewcar=0 )/{1} as RenBaoAgoTimes,
                //(select 
                //sum(iib1.pinganinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({2}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{3} as PingAnAgoTimes,
                //(select 
                //sum(iib1.taipingyanginsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({4}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{5} as TaiPingYangAgoTimes,
                //(select 
                //sum(iib1.guoshoucaiinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({5}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{7} as GuoShouCaiAgoTimes,
                //(select 
                //sum(iib1.othersourceinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month_day in({8}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{9} as OtherSourceAgoTimes
                //from bihu_analytics.tj_insuredistributionanalysis as iib
                //where iib.insuretime_year_month_day=?insuretime_year_month_day and iib.topagentid in({10}) and iib.islastnewcar=0", monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, topAgentIds);
                #endregion



                var InsureDistributionAnalysis_select_month_isNotLastNewCar = _getAnalysisInsureDistributionAboutDayOrMonthSql("insuretime_year_month", monthSequential, monthSequentialCycle, 0, topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iib.topagentid as TopAgentId,
                //iib.renbaoinsurecount as RenBaoTimes ,
                //iib.pinganinsurecount as PingAnTimes ,
                //iib.taipingyanginsurecount as TaiPingYangTimes ,
                //iib.guoshoucaiinsurecount as GuoShouCaiTimes,
                //iib.othersourceinsurecount as OtherSourceTimes ,
                //(select 
                //sum(iib1.renbaoinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({0})  and iib1.topagentid=iib.topagentid  and iib1.islastnewcar=0 )/{1} as RenBaoAgoTimes,
                //(select 
                //sum(iib1.pinganinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({2}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{3} as PingAnAgoTimes,
                //(select 
                //sum(iib1.taipingyanginsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({4}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{5} as TaiPingYangAgoTimes,
                //(select 
                //sum(iib1.guoshoucaiinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({5}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{7} as GuoShouCaiAgoTimes,
                //(select 
                //sum(iib1.othersourceinsurecount)
                //from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.insuretime_year_month in({8}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar=0)/{9} as OtherSourceAgoTimes
                //from bihu_analytics.tj_insuredistributionanalysis as iib
                //where iib.insuretime_year_month=?insuretime_year_month and iib.topagentid in({10}) and iib.islastnewcar=1", monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, monthSequential, monthSequentialCycle, topAgentIds);
                #endregion


                var InsureDistributionAnalysis_select_year_isNotLastNewCar = _getAnalysisInsureDistributionAboutYear(0, topAgentIds);
                #region 注释代码
                //                string.Format(@" select 
                //iib.topagentid as TopAgentId,
                //sum(iib.renbaoinsurecount) as RenBaoTimes,
                //sum(iib.pinganinsurecount) as PingAnTimes,
                //sum(iib.taipingyanginsurecount) as TaiPingYangTimes,
                //sum(iib.guoshoucaiinsurecount) as GuoShouCaiTimes ,
                //sum(iib.othersourceinsurecount) as OtherSourceTimes
                //from bihu_analytics.tj_insuredistributionanalysis as iib
                //where iib.insuretime_year=?insuretime_year and iib.topagentid in({0}) and iib.islastnewcar=0
                //group by iib.topagentid", topAgentIds);
                #endregion


                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };


                var result_selectSql_day_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_day_isLastNewCar, ps_selectSql_day.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_month_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_month_isLastNewCar, ps_selectSql_month.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_year_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_year_isLastNewCar, ps_selectSql_year.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();

                var result_selectSql_day_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_day_isNotLastNewCar, ps_selectSql_day.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_month_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_month_isNotLastNewCar, ps_selectSql_month.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_year_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, InsureDistributionAnalysis_select_year_isNotLastNewCar, ps_selectSql_year.ToArray()).ToList<InsureDistributionAnalysisTempVM_PropertyClass_PropertyClass>().ToList();
                InsureDistributionAnalysisTempVM result = new InsureDistributionAnalysisTempVM() { InsureDistributionLastNewCarAnalysisTempVM = new InsureDistributionAnalysisTempVM_PropertyClass { InsureDistributionDayAnalysisTempVM_PropertyClass = result_selectSql_day_isLastNewCar, InsureDistributionMonthAnalysisTempVM_PropertyClass = result_selectSql_month_isLastNewCar, InsureDistributionYearAnalysisTempVM_PropertyClass = result_selectSql_year_isLastNewCar }, InsureDistributionNotLastNewCarAnalysisTempVM = new InsureDistributionAnalysisTempVM_PropertyClass { InsureDistributionDayAnalysisTempVM_PropertyClass = result_selectSql_day_isNotLastNewCar, InsureDistributionMonthAnalysisTempVM_PropertyClass = result_selectSql_month_isNotLastNewCar, InsureDistributionYearAnalysisTempVM_PropertyClass = result_selectSql_year_isNotLastNewCar } };
                //List<List<InsureDistributionAnalysisTempVM>> insureDistributionAnalysis_isLastNewCarList = new List<List<InsureDistributionAnalysisTempVM>> { result_selectSql_day_isLastNewCar, result_selectSql_month_isLastNewCar, result_selectSql_year_isLastNewCar };
                //List<List<InsureDistributionAnalysisTempVM>> insureDistributionAnalysis_isNotLastNewCarList = new List<List<InsureDistributionAnalysisTempVM>> { result_selectSql_day_isNotLastNewCar, result_selectSql_month_isNotLastNewCar, result_selectSql_year_isNotLastNewCar };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }


        }
        private string _getAnalysisInsureDistributionAboutDayOrMonthSql(string selectDateType, string sequential, double sequentialCycle, int isLastNewCar, string topAgentIds)
        {
            return string.Format(@"select 
iib.topagentid as TopAgentId,
sum(iib.renbaoinsurecount) as RenBaoTimes ,
sum(iib.pinganinsurecount) as PingAnTimes ,
sum(iib.taipingyanginsurecount) as TaiPingYangTimes ,
sum(iib.guoshoucaiinsurecount) as GuoShouCaiTimes,
sum(iib.othersourceinsurecount) as OtherSourceTimes ,
(select 
sum(iib1.renbaoinsurecount)
from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.{0} in({1})  and iib1.topagentid=iib.topagentid  and iib1.islastnewcar={2} )/{3} as RenBaoAgoTimes,
(select 
sum(iib1.pinganinsurecount)
from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.{4} in({5}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar={6})/{7} as PingAnAgoTimes,
(select 
sum(iib1.taipingyanginsurecount)
from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.{8} in({9}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar={10})/{11} as TaiPingYangAgoTimes,
(select 
sum(iib1.guoshoucaiinsurecount)
from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.{12} in({13}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar={14})/{15} as GuoShouCaiAgoTimes,
(select 
sum(iib1.othersourceinsurecount)
from bihu_analytics.tj_insuredistributionanalysis as iib1 where iib1.{16} in({17}) and iib1.topagentid=iib.topagentid and iib1.islastnewcar={18})/{19} as OtherSourceAgoTimes
from bihu_analytics.tj_insuredistributionanalysis as iib
where iib.{20}=?{21} and iib.topagentid in({22}) and iib.islastnewcar={23} group by iib.topagentid", selectDateType, sequential, isLastNewCar, sequentialCycle, selectDateType, sequential, isLastNewCar, sequentialCycle, selectDateType, sequential, isLastNewCar, sequentialCycle, selectDateType, sequential, isLastNewCar, sequentialCycle, selectDateType, sequential, isLastNewCar, sequentialCycle, selectDateType, selectDateType, topAgentIds, isLastNewCar);

        }
        private string _getAnalysisInsureDistributionAboutYear(int isLastYearNewCar, string topAgentIds)
        {

            return string.Format(@" select 
iib.topagentid as TopAgentId,
sum(iib.renbaoinsurecount) as RenBaoTimes,
sum(iib.pinganinsurecount) as PingAnTimes,
sum(iib.taipingyanginsurecount) as TaiPingYangTimes,
sum(iib.guoshoucaiinsurecount) as GuoShouCaiTimes ,
sum(iib.othersourceinsurecount) as OtherSourceTimes
from bihu_analytics.tj_insuredistributionanalysis as iib
where iib.insuretime_year=?insuretime_year and iib.topagentid in({0}) and iib.islastnewcar={1}
group by iib.topagentid", topAgentIds, isLastYearNewCar);
        }

        public InsureBizAvgAnalysisVM AnalysisInsureBizAvg(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            try
            {
                var insureBizAvgAnalysis_select_day = _getAnalysisInsureBizAvgSql("insuretime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ibaa.topagentid as TopAgentId,
                //ibaa.renbaobizavg as RenBaoBizAvg,
                //ibaa.pinganbizavg as PingAnBizAvg,
                //ibaa.taipingyangbizavg as TaiPingYangBizAvg ,
                //ibaa.guoshoucaibizavg as GuoShouCaiBizAvg ,
                //ibaa.othersourcebizavg as OtherSourceBizAvg
                //from bihu_analytics.tj_insurebizavganalysis as ibaa
                //where ibaa.insuretime_year_month_day=?insuretime_year_month_day and ibaa.topagentid in({0})", topAgentIds);
                #endregion

                var insureBizAvgAnalysis_select_month = _getAnalysisInsureBizAvgSql("insuretime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ibaa.topagentid as TopAgentId,
                //sum(ibaa.renbaobizavg) as RenBaoBizAvg,
                //sum(ibaa.pinganbizavg) as PingAnBizAvg,
                //sum(ibaa.taipingyangbizavg) as TaiPingYangBizAvg ,
                //sum(ibaa.guoshoucaibizavg) as GuoShouCaiBizAvg,
                //sum(ibaa.othersourcebizavg) as OtherSourceBizAvg
                //from bihu_analytics.tj_insurebizavganalysis as ibaa
                //where ibaa.insuretime_year_month=?insuretime_year_month and ibaa.topagentid in({0})
                //group by ibaa.topagentid", topAgentIds);
                #endregion

                var insureBizAvgAnalysis_select_year = _getAnalysisInsureBizAvgSql("insuretime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ibaa.topagentid as TopAgentId ,
                //sum(ibaa.renbaobizavg) as RenBaoBizAvg,
                //sum(ibaa.pinganbizavg) as PingAnBizAvg,
                //sum(ibaa.taipingyangbizavg) as TaiPingYangBizAvg ,
                //sum(ibaa.guoshoucaibizavg) as GuoShouCaiBizAvg,
                //sum(ibaa.othersourcebizavg) as OtherSourceBizAvg
                //from bihu_analytics.tj_insurebizavganalysis as ibaa
                //where ibaa.insuretime_year=?insuretime_year and ibaa.topagentid in({0})
                //group by ibaa.topagentid", topAgentIds);
                #endregion

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };

                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureBizAvgAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<InsureBizAvgAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureBizAvgAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<InsureBizAvgAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureBizAvgAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<InsureBizAvgAnalysisVM_PropertyClass>().ToList();
                return new InsureBizAvgAnalysisVM { InsureBizAvgDayAnalysisVM = result_selectSql_day, InsureBizAvgMonthAnalysisVM = result_selectSql_month, InsureBizAvgYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string _getAnalysisInsureBizAvgSql(string selectDateType, string topAgentIds)
        {
            return string.Format(@"select 
ibaa.topagentid as TopAgentId,
ROUND(SUM(ibaa.renbaobizavg)/SUM(t.renbaoinsuranceGroup1+t.renbaoinsuranceGroup2+t.renbaoinsuranceGroup3+t.renbaoohterinsurancegroup)) AS RenBaoBizAvg,
ROUND(SUM(ibaa.pinganbizavg)/SUM(t.pinganinsuranceGroup1+t.pinganinsuranceGroup2+t.pinganinsuranceGroup3+t.pinganohterinsurancegroup)) AS PingAnBizAvg,
ROUND(SUM(ibaa.taipingyangbizavg)/SUM(t.taipingyanginsuranceGroup1+t.taipingyanginsuranceGroup2+t.taipingyanginsuranceGroup3+t.taipingyangohterinsurancegroup)) AS TaiPingYangBizAvg,
ROUND(SUM(ibaa.guoshoucaibizavg)/SUM(t.guoshoucaiinsuranceGroup1+t.guoshoucaiinsuranceGroup2+t.guoshoucaiinsuranceGroup3+t.guoshoucaiohterinsurancegroup)) AS GuoShouCaiBizAvg,
ROUND(SUM(ibaa.othersourcebizavg)/SUM(t.othersourceinsuranceGroup1+t.othersourceinsuranceGroup2+t.othersourceinsuranceGroup3+t.othersourceohterinsurancegroup)) AS OtherSourceBizAvg
from bihu_analytics.tj_insurebizavganalysis as ibaa LEFT JOIN bihu_analytics.tj_insureriskanalysis t
on ibaa.topagentid=t.topagentid and ibaa.insuretime_year_month_day=t.insuretime_year_month_day
where ibaa.{0}=?{1} and ibaa.topagentid in({2})
group by ibaa.topagentid", selectDateType, selectDateType, topAgentIds);
        }

        public InsureRiskAnalysisVM AnalysisInsureRisk(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            try
            {

                var insureRiskAnalysis_select_day = _getAnalysisInsureRiskSql("insuretime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ira.topagentid as TopAgentId,

                //ira.renbaoinsuranceGroup1 as RenBaoInsuranceGroup1  ,
                //ira.renbaoinsuranceGroup2 as  RenBaoInsuranceGroup2,
                //ira.renbaoinsuranceGroup3 as  RenBaoInsuranceGroup3,
                //ira.renbaodanjiaoqiang as   RenBaoInsuranceDanJiaoQiang  ,
                //ira.renbaoohterinsurancegroup as RenBaoOtherInsuranceGroup ,

                //ira.pinganinsuranceGroup1 as  PingAnInsuranceGroup1,
                //ira.pinganinsuranceGroup2 as PingAnInsuranceGroup2 ,
                //ira.pinganinsuranceGroup3 as PingAnInsuranceGroup3 ,
                //ira.pingandanjiaoqiang as PingAnInsuranceDanJiaoQiang ,
                //ira.pinganohterinsurancegroup as  PingAnOtherInsuranceGroup,

                //ira.taipingyanginsuranceGroup1 as TaiPingYangInsuranceGroup1 ,
                //ira.taipingyanginsuranceGroup2 as  TaiPingYangInsuranceGroup2,
                //ira.taipingyanginsuranceGroup3 as  TaiPingYangInsuranceGroup3,
                //ira.taipingyangdanjiaoqiang as  TaiPingYangInsuranceDanJiaoQiang,
                //ira.taipingyangohterinsurancegroup as  TaiPingYangOtherInsuranceGroup,

                //ira.guoshoucaiinsuranceGroup1 as  GuoShouCaiInsuranceGroup1,
                //ira.guoshoucaiinsuranceGroup2 as GuoShouCaiInsuranceGroup2 ,
                //ira.guoshoucaiinsuranceGroup3 as  GuoShouCaiInsuranceGroup3,
                //ira.guoshoucaidanjiaoqiang as GuoShouCaiInsuranceDanJiaoQiang ,
                //ira.guoshoucaiohterinsurancegroup as  GuoShouCaiOtherInsuranceGroup,


                //ira.othersourceinsuranceGroup1 as OtherSourceInsuranceGroup1 ,
                //ira.othersourceinsuranceGroup2 as  OtherSourceInsuranceGroup2,
                //ira.othersourceinsuranceGroup3 as  OtherSourceInsuranceGroup3,
                //ira.othersourcedanjiaoqiang as OtherSourceInsuranceDanJiaoQiang ,
                //ira.othersourceohterinsurancegroup as  OtherSourceOtherInsuranceGroup

                //from bihu_analytics.tj_insureriskanalysis as ira
                //where ira.insuretime_year_month_day=?insuretime_year_month_day and  ira.topagentid in({0}) ", topAgentIds);
                #endregion

                var insureRiskAnalysis_select_month = _getAnalysisInsureRiskSql("insuretime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ira.topagentid as TopAgentId ,

                //sum(ira.renbaoinsuranceGroup1) as RenBaoInsuranceGroup1 ,
                //sum(ira.renbaoinsuranceGroup2) as RenBaoInsuranceGroup2 ,
                //sum(ira.renbaoinsuranceGroup3) as RenBaoInsuranceGroup3 ,
                //sum(ira.renbaodanjiaoqiang) as   RenBaoInsuranceDanJiaoQiang  ,
                //sum(ira.renbaoohterinsurancegroup)  as  RenBaoOtherInsuranceGroup,

                //sum(ira.pinganinsuranceGroup1) as  PingAnInsuranceGroup1,
                //sum(ira.pinganinsuranceGroup2) as  PingAnInsuranceGroup2,
                //sum(ira.pinganinsuranceGroup3) as PingAnInsuranceGroup3 ,
                //sum(ira.pingandanjiaoqiang) as  PingAnInsuranceDanJiaoQiang,
                //sum(ira.pinganohterinsurancegroup) as PingAnOtherInsuranceGroup ,

                //sum(ira.taipingyanginsuranceGroup1) as TaiPingYangInsuranceGroup1 ,
                //sum(ira.taipingyanginsuranceGroup2) as  TaiPingYangInsuranceGroup2,
                //sum(ira.taipingyanginsuranceGroup3) as  TaiPingYangInsuranceGroup3,
                //sum(ira.taipingyangdanjiaoqiang) as  TaiPingYangInsuranceDanJiaoQiang,
                //sum(ira.taipingyangohterinsurancegroup) as  TaiPingYangOtherInsuranceGroup,

                //sum(ira.guoshoucaiinsuranceGroup1) as GuoShouCaiInsuranceGroup1 ,
                //sum(ira.guoshoucaiinsuranceGroup2) as GuoShouCaiInsuranceGroup2 ,
                //sum(ira.guoshoucaiinsuranceGroup3) as GuoShouCaiInsuranceGroup3 ,
                //sum(ira.guoshoucaidanjiaoqiang) as  GuoShouCaiInsuranceDanJiaoQiang,
                //sum(ira.guoshoucaiohterinsurancegroup) as GuoShouCaiOtherInsuranceGroup ,


                //sum(ira.othersourceinsuranceGroup1) as  OtherSourceInsuranceGroup1,
                //sum(ira.othersourceinsuranceGroup2) as  OtherSourceInsuranceGroup2,
                //sum(ira.othersourceinsuranceGroup3) as  OtherSourceInsuranceGroup3,
                //sum(ira.othersourcedanjiaoqiang) as  OtherSourceInsuranceDanJiaoQiang,
                //sum(ira.othersourceohterinsurancegroup) as OtherSourceOtherInsuranceGroup 

                //from bihu_analytics.tj_insureriskanalysis as ira
                //where  ira.insuretime_year_month=?insuretime_year_month and  ira.topagentid in({0})
                //group by ira.topagentid", topAgentIds);
                #endregion

                var insureRiskAnalysis_select_year = _getAnalysisInsureRiskSql("insuretime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //ira.topagentid as TopAgentId ,

                //sum(ira.renbaoinsuranceGroup1) as RenBaoInsuranceGroup1 ,
                //sum(ira.renbaoinsuranceGroup2) as RenBaoInsuranceGroup2 ,
                //sum(ira.renbaoinsuranceGroup3) as RenBaoInsuranceGroup3 ,
                //sum(ira.renbaodanjiaoqiang) as   RenBaoInsuranceDanJiaoQiang  ,
                //sum(ira.renbaoohterinsurancegroup)  as  RenBaoOtherInsuranceGroup,

                //sum(ira.pinganinsuranceGroup1) as  PingAnInsuranceGroup1,
                //sum(ira.pinganinsuranceGroup2) as  PingAnInsuranceGroup2,
                //sum(ira.pinganinsuranceGroup3) as PingAnInsuranceGroup3 ,
                //sum(ira.pingandanjiaoqiang) as  PingAnInsuranceDanJiaoQiang,
                //sum(ira.pinganohterinsurancegroup) as PingAnOtherInsuranceGroup ,

                //sum(ira.taipingyanginsuranceGroup1) as TaiPingYangInsuranceGroup1 ,
                //sum(ira.taipingyanginsuranceGroup2) as  TaiPingYangInsuranceGroup2,
                //sum(ira.taipingyanginsuranceGroup3) as  TaiPingYangInsuranceGroup3,
                //sum(ira.taipingyangdanjiaoqiang) as  TaiPingYangInsuranceDanJiaoQiang,
                //sum(ira.taipingyangohterinsurancegroup) as  TaiPingYangOtherInsuranceGroup,

                //sum(ira.guoshoucaiinsuranceGroup1) as GuoShouCaiInsuranceGroup1 ,
                //sum(ira.guoshoucaiinsuranceGroup2) as GuoShouCaiInsuranceGroup2 ,
                //sum(ira.guoshoucaiinsuranceGroup3) as GuoShouCaiInsuranceGroup3 ,
                //sum(ira.guoshoucaidanjiaoqiang) as  GuoShouCaiInsuranceDanJiaoQiang,
                //sum(ira.guoshoucaiohterinsurancegroup) as GuoShouCaiOtherInsuranceGroup ,


                //sum(ira.othersourceinsuranceGroup1) as  OtherSourceInsuranceGroup1,
                //sum(ira.othersourceinsuranceGroup2) as  OtherSourceInsuranceGroup2,
                //sum(ira.othersourceinsuranceGroup3) as  OtherSourceInsuranceGroup3,
                //sum(ira.othersourcedanjiaoqiang) as  OtherSourceInsuranceDanJiaoQiang,
                //sum(ira.othersourceohterinsurancegroup) as OtherSourceOtherInsuranceGroup 

                //from bihu_analytics.tj_insureriskanalysis as ira
                //where ira.insuretime_year=?insuretime_year and  ira.topagentid in({0})
                //group by ira.topagentid", topAgentIds);
                #endregion

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };

                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureRiskAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<InsureRiskAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureRiskAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<InsureRiskAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureRiskAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<InsureRiskAnalysisVM_PropertyClass>().ToList();
                return new InsureRiskAnalysisVM { InsureRiskDayAnalysisVM = result_selectSql_day, InsureRiskMonthAnalysisVM = result_selectSql_month, InsureRiskYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string _getAnalysisInsureRiskSql(string selectDateType, string topAgentIds)
        {
            return string.Format(@"select 
ira.topagentid as TopAgentId ,

sum(ira.renbaoinsuranceGroup1) as RenBaoInsuranceGroup1 ,
sum(ira.renbaoinsuranceGroup2) as RenBaoInsuranceGroup2 ,
sum(ira.renbaoinsuranceGroup3) as RenBaoInsuranceGroup3 ,
sum(ira.renbaodanjiaoqiang) as   RenBaoInsuranceDanJiaoQiang  ,
sum(ira.renbaoohterinsurancegroup)  as  RenBaoOtherInsuranceGroup,

sum(ira.pinganinsuranceGroup1) as  PingAnInsuranceGroup1,
sum(ira.pinganinsuranceGroup2) as  PingAnInsuranceGroup2,
sum(ira.pinganinsuranceGroup3) as PingAnInsuranceGroup3 ,
sum(ira.pingandanjiaoqiang) as  PingAnInsuranceDanJiaoQiang,
sum(ira.pinganohterinsurancegroup) as PingAnOtherInsuranceGroup ,

sum(ira.taipingyanginsuranceGroup1) as TaiPingYangInsuranceGroup1 ,
sum(ira.taipingyanginsuranceGroup2) as  TaiPingYangInsuranceGroup2,
sum(ira.taipingyanginsuranceGroup3) as  TaiPingYangInsuranceGroup3,
sum(ira.taipingyangdanjiaoqiang) as  TaiPingYangInsuranceDanJiaoQiang,
sum(ira.taipingyangohterinsurancegroup) as  TaiPingYangOtherInsuranceGroup,

sum(ira.guoshoucaiinsuranceGroup1) as GuoShouCaiInsuranceGroup1 ,
sum(ira.guoshoucaiinsuranceGroup2) as GuoShouCaiInsuranceGroup2 ,
sum(ira.guoshoucaiinsuranceGroup3) as GuoShouCaiInsuranceGroup3 ,
sum(ira.guoshoucaidanjiaoqiang) as  GuoShouCaiInsuranceDanJiaoQiang,
sum(ira.guoshoucaiohterinsurancegroup) as GuoShouCaiOtherInsuranceGroup ,


sum(ira.othersourceinsuranceGroup1) as  OtherSourceInsuranceGroup1,
sum(ira.othersourceinsuranceGroup2) as  OtherSourceInsuranceGroup2,
sum(ira.othersourceinsuranceGroup3) as  OtherSourceInsuranceGroup3,
sum(ira.othersourcedanjiaoqiang) as  OtherSourceInsuranceDanJiaoQiang,
sum(ira.othersourceohterinsurancegroup) as OtherSourceOtherInsuranceGroup 

from bihu_analytics.tj_insureriskanalysis as ira
where  ira.{0}=?{1} and  ira.topagentid in({2})
group by ira.topagentid", selectDateType, selectDateType, topAgentIds);
        }
        public InsureAdvanceAnalysisVM AnalysisInsureAdvance(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            try
            {


                var insureAdvanceAnalysis_select_day = _getAnalysisInsureAdvanceSql("insuretime_year_month_day", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iaa.topagentid as TopAgentId,
                //iaa.renbao_0to30 as RenBaoAdvance_0to30,
                //iaa.renbao_31to60 as RenBaoAdvance_31to60,
                //iaa.renbao_61to90 as RenBaoAdvance_61to90 ,
                //iaa.pingan_0to30 as PingAnAdvance_0to30,
                //iaa.pingan_31to60 as PingAnAdvance_31to60,
                //iaa.pingan_61to90 as PingAnAdvance_61to90,
                //iaa.taipingyang_0to30 as TaiPingYangAdvance_0to30,
                //iaa.taipingyang_0to30 as TaiPingYangAdvance_31to60,
                //iaa.taipingyang_0to30 as TaiPingYangAdvance_61to90,
                //iaa.goushoucai_0to30 as GouShouCaiAdvance_0to30,
                //iaa.goushoucai_31to60 as GouShouCaiAdvance_31to60,
                //iaa.goushoucai_61to90 as GouShouCaiAdvance_61to90,
                //iaa.othersource_0to30 as OtherSourceAdvance_0to30,
                //iaa.othersource_31to60 as OtherSourceAdvance_31to60,
                //iaa.othersource_61to90 as OtherSourceAdvance_61to90
                //from bihu_analytics.tj_insureadvanceanalysis as iaa
                //where iaa.insuretime_year_month_day=?insuretime_year_month_day and iaa.topagentid in({0})
                //", topAgentIds);
                #endregion

                var insureAdvanceAnalysis_select_month = _getAnalysisInsureAdvanceSql("insuretime_year_month", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iaa.topagentid as TopAgentId,
                //sum(iaa.renbao_0to30) as RenBaoAdvance_0to30 ,
                //sum(iaa.renbao_31to60) as RenBaoAdvance_31to60,
                //sum(iaa.renbao_61to90) as RenBaoAdvance_61to90,
                //sum(iaa.pingan_0to30) as PingAnAdvance_0to30,
                //sum(iaa.pingan_31to60) as PingAnAdvance_31to60,
                //sum(iaa.pingan_61to90) as PingAnAdvance_61to90,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_0to30,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_31to60,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_61to90,
                //sum(iaa.goushoucai_0to30) as GouShouCaiAdvance_0to30,
                //sum(iaa.goushoucai_31to60) as GouShouCaiAdvance_31to60,
                //sum(iaa.goushoucai_61to90) as GouShouCaiAdvance_61to90,
                //sum(iaa.othersource_0to30) as OtherSourceAdvance_0to30,
                //sum(iaa.othersource_31to60) as OtherSourceAdvance_31to60,
                //sum(iaa.othersource_61to90) as OtherSourceAdvance_61to90
                //from bihu_analytics.tj_insureadvanceanalysis as iaa
                //where iaa.insuretime_year_month=?insuretime_year_month and iaa.topagentid in({0})
                //group by iaa.topagentid", topAgentIds);
                #endregion

                var insureAdvanceAnalysis_select_year = _getAnalysisInsureAdvanceSql("insuretime_year", topAgentIds);
                #region 注释代码
                //                string.Format(@"select 
                //iaa.topagentid as TopAgentId,
                //sum(iaa.renbao_0to30) as RenBaoAdvance_0to30 ,
                //sum(iaa.renbao_31to60) as RenBaoAdvance_31to60,
                //sum(iaa.renbao_61to90) as RenBaoAdvance_61to90,
                //sum(iaa.pingan_0to30) as PingAnAdvance_0to30,
                //sum(iaa.pingan_31to60) as PingAnAdvance_31to60,
                //sum(iaa.pingan_61to90) as PingAnAdvance_61to90,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_0to30,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_31to60,
                //sum(iaa.taipingyang_0to30) as TaiPingYangAdvance_61to90,
                //sum(iaa.goushoucai_0to30) as GouShouCaiAdvance_0to30,
                //sum(iaa.goushoucai_31to60) as GouShouCaiAdvance_31to60,
                //sum(iaa.goushoucai_61to90) as GouShouCaiAdvance_61to90,
                //sum(iaa.othersource_0to30) as OtherSourceAdvance_0to30,
                //sum(iaa.othersource_31to60) as OtherSourceAdvance_31to60,
                //sum(iaa.othersource_61to90) as OtherSourceAdvance_61to90
                //from bihu_analytics.tj_insureadvanceanalysis as iaa
                //where iaa.insuretime_year=?insuretime_year and iaa.topagentid in({0})
                //group by iaa.topagentid", topAgentIds);
                #endregion

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };


                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureAdvanceAnalysis_select_day, ps_selectSql_day.ToArray()).ToList<InsureAdvanceAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureAdvanceAnalysis_select_month, ps_selectSql_month.ToArray()).ToList<InsureAdvanceAnalysisVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, insureAdvanceAnalysis_select_year, ps_selectSql_year.ToArray()).ToList<InsureAdvanceAnalysisVM_PropertyClass>().ToList();
                return new InsureAdvanceAnalysisVM { InsureAdvanceDayAnalysisVM = result_selectSql_day, InsureAdvanceMonthAnalysisVM = result_selectSql_month, InsureAdvanceYearAnalysisVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string _getAnalysisInsureAdvanceSql(string selectDateType, string topAgentIds)
        {

            return string.Format(@"select 
iaa.topagentid as TopAgentId,
sum(iaa.renbao_0to30) as RenBaoAdvance_0to30 ,
sum(iaa.renbao_31to60) as RenBaoAdvance_31to60,
sum(iaa.renbao_61to90) as RenBaoAdvance_61to90,
sum(iaa.pingan_0to30) as PingAnAdvance_0to30,
sum(iaa.pingan_31to60) as PingAnAdvance_31to60,
sum(iaa.pingan_61to90) as PingAnAdvance_61to90,
sum(iaa.taipingyang_0to30) AS TaiPingYangAdvance_0to30,
sum(iaa.taipingyang_31to60) AS TaiPingYangAdvance_31to60,
sum(iaa.taipingyang_61to90) AS TaiPingYangAdvance_61to90,
sum(iaa.goushoucai_0to30) as GouShouCaiAdvance_0to30,
sum(iaa.goushoucai_31to60) as GouShouCaiAdvance_31to60,
sum(iaa.goushoucai_61to90) as GouShouCaiAdvance_61to90,
sum(iaa.othersource_0to30) as OtherSourceAdvance_0to30,
sum(iaa.othersource_31to60) as OtherSourceAdvance_31to60,
sum(iaa.othersource_61to90) as OtherSourceAdvance_61to90
from bihu_analytics.tj_insureadvanceanalysis as iaa
where iaa.{0}=?{1} and iaa.topagentid in({2})
group by iaa.topagentid", selectDateType, selectDateType, topAgentIds);
        }

        public FlowDirectionAnalysisVM AnalysisFlowDirectionFromRenBao(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            try
            {

                var flowDirectionFromRenBaoAnalysis_selectDay_isLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 1, "insuretime_year_month_day");


                var flowDirectionFromRenBaoAnalysis_selectMonth_isLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 1, "insuretime_year_month");

                var flowDirectionFromRenBaoAnalysis_selectYear_isLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 1, "insuretime_year");



                var flowDirectionFromRenBaoAnalysis_selectDay_isNotLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 0, "insuretime_year_month_day");

                var flowDirectionFromRenBaoAnalysis_selectMonth_isNotLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 0, "insuretime_year_month");

                var flowDirectionFromRenBaoAnalysis_selectYear_isNotLastNewCar = GetAnalysisFlowDirectionFromRenBaoSql(topAgentIds, 0, "insuretime_year");

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };


                var result_selectSql_day_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectDay_isLastNewCar, ps_selectSql_day.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_month_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectMonth_isLastNewCar, ps_selectSql_month.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_year_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectYear_isLastNewCar, ps_selectSql_year.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();

                var result_selectSql_day_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectDay_isNotLastNewCar, ps_selectSql_day.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_month_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectMonth_isNotLastNewCar, ps_selectSql_month.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
                var result_selectSql_year_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionFromRenBaoAnalysis_selectYear_isNotLastNewCar, ps_selectSql_year.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();


                FlowDirectionAnalysisVM result = new FlowDirectionAnalysisVM() { FlowDirectionLastNewCarAnalysisVM = new FlowDirectionAnalysisVM_PropertyClass { FlowDirectionDayAnalysisVM_PropertyClass = result_selectSql_day_isLastNewCar, FlowDirectionMonthAnalysisVM_PropertyClass = result_selectSql_month_isLastNewCar, FlowDirectionYearAnalysisVM_PropertyClass = result_selectSql_year_isLastNewCar }, FlowDirectionNotLastNewCarAnalysisVM = new FlowDirectionAnalysisVM_PropertyClass { FlowDirectionDayAnalysisVM_PropertyClass = result_selectSql_day_isNotLastNewCar, FlowDirectionMonthAnalysisVM_PropertyClass = result_selectSql_month_isNotLastNewCar, FlowDirectionYearAnalysisVM_PropertyClass = result_selectSql_year_isNotLastNewCar } };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        private string GetAnalysisFlowDirectionFromRenBaoSql(string topAgentIds, int islastnewcar, string selectDateType)
        {

            return string.Format(@"select 
t.topagentid as TopAgentId,
sum(t.renbaocurrentyearinsurecount) as RenBaoInsureCount,
sum(t.pingancurrentyearinsurecount) as PinganInsureCount,
sum(t.taipingyangcurrentyearinsurecount) as TaipingyangInsureCount,
sum(t.guoshoucaicurrentyearinsurecount) as GuoShouCaiInsureCount,
sum(t.othersourcecurrentyearinsurecount) as OtherSourceInsureCount
from bihu_analytics.tj_flowdirectionfromrenbaoanalysis as t
where t.islastnewcar = {0} and t.topagentid IN({1}) and t.{2}=?{3}
group by t.topagentid
", islastnewcar, topAgentIds, selectDateType, selectDateType);
        }

        public FlowDirectionAnalysisVM AnalysisFlowDirectionToRenBao(string insureDate_day, string insureDate_month, string insureDate_year, string topAgentIds)
        {
            var flowDirectionToRenBaoAnalysis_selectDay_isLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 1, "insuretime_year_month_day");


            var flowDirectionToRenBaoAnalysis_selectMonth_isLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 1, "insuretime_year_month");

            var flowDirectionToRenBaoAnalysis_selectYear_isLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 1, "insuretime_year");



            var flowDirectionToRenBaoAnalysis_selectDay_isNotLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 0, "insuretime_year_month_day");

            var flowDirectionToRenBaoAnalysis_selectMonth_isNotLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 0, "insuretime_year_month");

            var flowDirectionToRenBaoAnalysis_selectYear_isNotLastNewCar = GetAnalysisFlowDirectionToRenBaoSql(topAgentIds, 0, "insuretime_year");

            List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month_day", Value = insureDate_day } };

            List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year_month", Value = insureDate_month } };

            List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "insuretime_year", Value = insureDate_year } };

            var result_selectSql_day_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectDay_isLastNewCar, ps_selectSql_day.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
            var result_selectSql_month_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectMonth_isLastNewCar, ps_selectSql_month.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
            var result_selectSql_year_isLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectYear_isLastNewCar, ps_selectSql_year.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();

            var result_selectSql_day_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectDay_isNotLastNewCar, ps_selectSql_day.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
            var result_selectSql_month_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectMonth_isNotLastNewCar, ps_selectSql_month.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();
            var result_selectSql_year_isNotLastNewCar = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowDirectionToRenBaoAnalysis_selectYear_isNotLastNewCar, ps_selectSql_year.ToArray()).ToList<FlowDirectionAnalysisVM_PropertyClass_PropertyClass>().ToList();


            FlowDirectionAnalysisVM result = new FlowDirectionAnalysisVM() { FlowDirectionLastNewCarAnalysisVM = new FlowDirectionAnalysisVM_PropertyClass { FlowDirectionDayAnalysisVM_PropertyClass = result_selectSql_day_isLastNewCar, FlowDirectionMonthAnalysisVM_PropertyClass = result_selectSql_month_isLastNewCar, FlowDirectionYearAnalysisVM_PropertyClass = result_selectSql_year_isLastNewCar }, FlowDirectionNotLastNewCarAnalysisVM = new FlowDirectionAnalysisVM_PropertyClass { FlowDirectionDayAnalysisVM_PropertyClass = result_selectSql_day_isNotLastNewCar, FlowDirectionMonthAnalysisVM_PropertyClass = result_selectSql_month_isNotLastNewCar, FlowDirectionYearAnalysisVM_PropertyClass = result_selectSql_year_isNotLastNewCar } };
            return result;
        }
        private string GetAnalysisFlowDirectionToRenBaoSql(string topAgentIds, int islastnewcar, string selectDateType)
        {

            return string.Format(@"select 
t.topagentid as TopAgentId,
sum(t.renbaolastyearinsurecount) as RenBaoInsureCount,
sum(t.pinganlastyearinsurecount) as PinganInsureCount,
sum(t.taipingyanglastyearinsurecount) as TaipingyangInsureCount,
sum(t.guoshoucailastyearinsurecount) as GuoShouCaiInsureCount,
sum(t.othersourcelastyearinsurecount) as OtherSourceInsureCount
from bihu_analytics.tj_flowdirectiontorenbaoanalysis  as t
where t.islastnewcar = {0} and t.topagentid IN({1}) and t.{2}=?{3}
group by t.topagentid
", islastnewcar, topAgentIds, selectDateType, selectDateType);
        }
        public FlowMonitorVM MonitorFlow(string analysisDate_day, string analysisDate_month, string analysisDate_year, string topAgentIds)
        {
            try
            {


                var flowMonitor_selectDay = GetMonitorFlowSql(topAgentIds, "analysisdate_year_month_day");
                var flowMonitor_selectMonth = GetMonitorFlowSql(topAgentIds, "analysisdate_year_month");
                var flowMonitor_selectYear = GetMonitorFlowSql(topAgentIds, "analysisdate_year");

                List<MySqlParameter> ps_selectSql_day = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "analysisdate_year_month_day", Value = analysisDate_day } };

                List<MySqlParameter> ps_selectSql_month = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "analysisdate_year_month", Value = analysisDate_month } };

                List<MySqlParameter> ps_selectSql_year = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "analysisdate_year", Value = analysisDate_year } };


                var result_selectSql_day = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowMonitor_selectDay, ps_selectSql_day.ToArray()).ToList<FlowMonitorVM_PropertyClass>().ToList();
                var result_selectSql_month = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowMonitor_selectMonth, ps_selectSql_month.ToArray()).ToList<FlowMonitorVM_PropertyClass>().ToList();
                var result_selectSql_year = _mySqlHelper.ExecuteDataTable(CommandType.Text, flowMonitor_selectYear, ps_selectSql_year.ToArray()).ToList<FlowMonitorVM_PropertyClass>().ToList();


                return new FlowMonitorVM { FlowMonitorDayVM = result_selectSql_day, FlowMonitorMonthVM = result_selectSql_month, FlowMonitorYearVM = result_selectSql_year };
            }
            catch (Exception ex)
            {
                throw new Exception();
            }


        }
        private string GetMonitorFlowSql(string topAgentIds, string selectDateType)
        {
            return string.Format(@" select 
 t.topagentid as TopAgentId,
 sum(t.importdatacount) as ImportDataCount ,
 sum(t.datainterchangecount) as DataInterchangeCount,
sum(t.quotecount) as QuoteCount,
sum(t.conclusionCount) as ConclusionCount
 from bihu_analytics.tj_flowmonitor as t
 where t.topagentid in({0}) and t.{1}=?{2}
 group by t.topagentid", topAgentIds, selectDateType, selectDateType);

        }

        #endregion
        #endregion

        public List<CarDealerReportData> GetCarDealerReport(DateTime startTime, DateTime endTime, int agentId, string categoryName, out int carDealerCount)
        {
            carDealerCount = 0;
            var agent = DataContextFactory.GetDataContext().bx_sf_agent.FirstOrDefault(x => x.Id == agentId);
            var agentIds = agent.TopAgentIds;
            if (string.IsNullOrEmpty(agentIds))
            {
                return null;
            }
            carDealerCount = agentIds.Split(',').Length;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(categoryName))
            {
                sql = string.Format(@"select t.QuoteCount,t1.EntryCount,t.EntryTime from
                                    (
                                    select sum(t.quote_car_count) QuoteCount,DATE_FORMAT(t.data_in_time,'%Y-%m-%d') EntryTime from bihu_analytics.tj_sf_dailywork t
                                    where t.top_agent_id in({0}) and t.data_in_time>='{1}' AND t.data_in_time<'{2}' {3}
                                    group BY t.data_in_time
                                    ) t
                                    LEFT JOIN
                                    (
                                    select COUNT(e.Id) EntryCount,DATE_FORMAT(e.CameraTime,'%Y-%m-%d') EntryTime from bihu_analytics.tj_entrydetails e
                                    where e.TopAgentId in({0}) and e.CameraTime>='{1}' AND e.CameraTime<'{2}' {4}
                                    group by DATE_FORMAT(e.CameraTime,'%Y-%m-%d')
                                    ) t1
                                    on t.EntryTime=t1.EntryTime", agentIds, startTime, endTime, agent.is_view_all_data == 0 ? "and t.last_year_source=2" : "", agent.is_view_all_data == 0 ? "AND e.LastYearSource=2" : "");
            }
            else
            {
                sql = string.Format(@"select t.QuoteCount,t1.EntryCount,t.EntryTime from
                                    (
                                    select sum(t.category_quote_car_count) QuoteCount,DATE_FORMAT(t.data_in_time,'%Y-%m-%d') EntryTime from bihu_analytics.tj_sf_dailywork t
                                    where t.top_agent_id in({0}) and t.data_in_time>='{1}' AND t.data_in_time<'{2}' {3}
                                    group BY t.data_in_time
                                    ) t
                                    LEFT JOIN
                                    (
                                    select COUNT(e.Id) EntryCount,DATE_FORMAT(e.CameraTime,'%Y-%m-%d') EntryTime from bihu_analytics.tj_entrydetails e
                                    LEFT JOIN bx_userinfo u on e.BuId=u.Id
                                    LEFT JOIN bx_customercategories c on u.CategoryInfoId=c.Id
                                    where e.TopAgentId in({0}) and e.CameraTime>='{1}' AND e.CameraTime<'{2}' {4}
                                    and c.CategoryInfo='在修不在保'
                                    group by DATE_FORMAT(e.CameraTime,'%Y-%m-%d')
                                    ) t1
                                    on t.EntryTime=t1.EntryTime", agentIds, startTime, endTime, agent.is_view_all_data == 0 ? "and t.last_year_source=2" : "", agent.is_view_all_data == 0 ? "AND e.LastYearSource=2" : "");
            }
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<CarDealerReportData>().ToList();
            return result;
        }

        public List<CarDealerReportDetails> GetCarDealerReportDetailsByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName, out int totalCount)
        {
            totalCount = 0;
            var agent = DataContextFactory.GetDataContext().bx_sf_agent.FirstOrDefault(x => x.Id == agentId);
            var agentIds = agent.TopAgentIds;
            if (!string.IsNullOrEmpty(agentIds))
            {
                totalCount = agentIds.Split(',').Length;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var listIds = new List<string>(agentIds.Split(',')).ConvertAll(i => int.Parse(i));
                    totalCount = DataContextFactory.GetDataContext().bx_agent.Count(x => listIds.Contains(x.Id) && x.AgentName.Contains(searchText));
                }
                var sql = string.Format(@"select a.Id TopAgentId,a.AgentName,IFNULL(t.EntryCount,0) EntryCount,IFNULL(t.QuoteCount,0) QuoteCount,IFNULL(t.RenewalPeriodCount,0) RenewalPeriodCount,IFNULL(t.DistributedCount,0) DistributedCount,IFNULL(t.ReviewCount,0) ReviewCount from bx_agent a
                                        left join
                                        (select e.TopAgentId,COUNT(e.Id) EntryCount,IFNULL(SUM(CASE WHEN IsQuoteInTheDay=1 THEN 1 ELSE 0 END),0) QuoteCount,
                                        SUM(CASE WHEN DATEDIFF(LastBizEndDate,CameraTime)>=0 AND DATEDIFF(LastBizEndDate,CameraTime)<=90 THEN 1
                                            WHEN DATEDIFF(LastForceEndDate,CameraTime)>=0 AND DATEDIFF(LastForceEndDate,CameraTime)<=90 THEN 1
                                            ELSE 0 END) RenewalPeriodCount,
                                        IFNULL(SUM(CASE WHEN IsDistributed>0 THEN 1 ELSE 0 END),0) DistributedCount,
                                        IFNULL(SUM(CASE WHEN IsReView>0 THEN 1 ELSE 0 END),0) ReviewCount
                                        from bihu_analytics.tj_entrydetails e {6}
                                        where e.TopAgentId in({0}) and e.CameraTime>='{1}' AND e.CameraTime<'{2}' {8} {7}
                                        group by e.TopAgentId) t on a.Id=t.TopAgentId where a.Id in({0}) {3} order by a.Id limit {4},{5}", agentIds, startTime, endTime, string.IsNullOrEmpty(searchText) ? "" : " and a.AgentName like '%" + searchText + "%'",
                                        (pageIndex - 1) * pageSize, pageSize, string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on e.CategoryInfoId=c.Id",
                                        string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'", agent.is_view_all_data == 0 ? "AND e.LastYearSource=2" : "");
                var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<CarDealerReportDetails>().ToList();
                return result;
            }
            else
            {
                return null;
            }
        }

        public List<EntryDetailsViewModel> GetEntryDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName, out int totalCount)
        {
            var countSql = string.Format(@"select count(id) from bihu_analytics.tj_entrydetails where TopAgentId={0} and CameraTime>='{1}' AND CameraTime<'{2}' {3}", topAgentId, startTime, endTime, isViewAllData == 0 ? "AND LastYearSource=2" : "");
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, countSql, null));
            var sql = string.Format(@"select e.Id,e.LicenseNo,
                                    CASE WHEN DATEDIFF(LastBizEndDate,CameraTime)>=0 AND DATEDIFF(LastBizEndDate,CameraTime)<=90 THEN '是'
                                        WHEN DATEDIFF(LastForceEndDate,CameraTime)>=0 AND DATEDIFF(LastForceEndDate,CameraTime)<=90 THEN '是'
                                        ELSE '否' END InRenewalPeriod,
                                    CASE e.LastYearSource WHEN 2 THEN '人保' ELSE '其它' END LastYearSource,
                                    CASE e.IsQuoteInTheDay WHEN 1 THEN '是' ELSE '否' END IsQuote,
                                    DATE_FORMAT(e.CameraTime,'%Y-%m-%d %H:%i:%s') CameraTime,
                                    CASE e.IsDistributed WHEN 0 THEN '否' ELSE '是' END IsDistributed,
                                    CASE e.IsReView WHEN 0 THEN '否' ELSE '是' END IsReView
                                    from bihu_analytics.tj_entrydetails e {5} where e.TopAgentId={0} and e.CameraTime>='{1}' AND e.CameraTime<'{2}' {7} {6}
                                    ORDER BY CameraTime DESC LIMIT {3},{4}", topAgentId, startTime, endTime, (pageIndex - 1) * pageSize, pageSize,
                                    string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on e.CategoryInfoId=c.Id",
                                    string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'",
                                    isViewAllData == 0 ? "AND e.LastYearSource=2" : "");
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<EntryDetailsViewModel>().ToList();
            return result;
        }

        public List<DailyWorkViewModel> GetDailyWorkByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName, out int totalCount)
        {
            totalCount = 0;
            var agent = DataContextFactory.GetDataContext().bx_sf_agent.FirstOrDefault(x => x.Id == agentId);
            var agentIds = agent.TopAgentIds;
            if (!string.IsNullOrEmpty(agentIds))
            {
                totalCount = agentIds.Split(',').Length;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var listIds = new List<string>(agentIds.Split(',')).ConvertAll(i => int.Parse(i));
                    totalCount = DataContextFactory.GetDataContext().bx_agent.Count(x => listIds.Contains(x.Id) && x.AgentName.Contains(searchText));
                }
                var sql = "";
                if (string.IsNullOrEmpty(categoryName))
                {
                    sql = string.Format(@"select a.Id TopAgentId,a.AgentName,IFNULL(sum(t.quote_car_count),0) QuoteCount,
	                                    IFNULL(sum(t.review_count),0) ReviewCount,IFNULL(sum(t.call_count),0) CallCount
	                                    from bx_agent a
	                                    LEFT JOIN bihu_analytics.tj_sf_dailywork t on a.Id=t.top_agent_id and t.data_in_time>='{1}'
	                                    and t.data_in_time<'{2}' {6}
	                                    where a.Id in({0}) {3}
                                        group by a.Id limit {4},{5}", agentIds, startTime, endTime,
                                        string.IsNullOrEmpty(searchText) ? "" : " and a.AgentName like '%" + searchText + "%'",
                                        (pageIndex - 1) * pageSize, pageSize, agent.is_view_all_data == 0 ? "and t.last_year_source=2" : "");
                }
                else
                {
                    sql = string.Format(@"select t.top_agent_id TopAgentId,a.AgentName,IFNULL(t.category_quote_car_count,0) QuoteCount,
                                        IFNULL(t.category_review_count,0) ReviewCount,IFNULL(t.category_call_count,0) CallCount
                                        from bx_agent a
                                        LEFT JOIN bihu_analytics.tj_sf_dailywork t on a.Id=t.top_agent_id and t.data_in_time>='{1}'
	                                    and t.data_in_time<'{2}' {6}
                                        where a.Id in({0}) {3} limit {4},{5}", agentIds, startTime, endTime,
                                        string.IsNullOrEmpty(searchText) ? "" : " and a.AgentName like '%" + searchText + "%'",
                                        (pageIndex - 1) * pageSize, pageSize, agent.is_view_all_data == 0 ? "and t.last_year_source=2" : "");
                }
                var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<DailyWorkViewModel>().ToList();
                return result;
            }
            else
            {
                return null;
            }
        }

        public List<CustomerType> GetPremiumWithCustomerType(DateTime startTime, DateTime endTime, int agentId)
        {
            var sql = string.Format(@"SELECT Customer,IFNULL(Premium,0) Premium,IFNULL(OrderCount,0) OrderCount FROM(
                                    SELECT 1 Customer,SUM(NewCarPremium) Premium,SUM(NewCarTaiCi) OrderCount FROM dz_permium_customer t
                                    WHERE t.AgentID={0} AND t.DateofPayment>='{1}' AND t.DateofPayment<'{2}'
                                    UNION ALL
                                    SELECT 2 Customer,SUM(SecondNewCarPremium) Premium,SUM(SecondNewCarTaiCi) OrderCount FROM dz_permium_customer t
                                    WHERE t.AgentID={0} AND t.DateofPayment>='{1}' AND t.DateofPayment<'{2}'
                                    UNION ALL
                                    SELECT 3 Customer,SUM(OtherPremium) Premium,SUM(OtherTaiCi) OrderCount FROM dz_permium_customer t
                                    WHERE t.AgentID={0} AND t.DateofPayment>='{1}' AND t.DateofPayment<'{2}') t", agentId, startTime, endTime);
            var dt = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null);
            dt.Columns.Add("Ratio");
            var sum = Convert.ToDecimal(dt.Compute("sum(Premium)", "true").ToString());
            foreach (DataRow dr in dt.Rows)
            {
                dr["Ratio"] = Math.Round((Convert.ToDecimal(dr["Premium"]) / (sum > 0 ? sum : 1)), 4) * 100;
            }
            return dt.ToList<CustomerType>().ToList();
        }

        public List<InsuranceCompany> GetPremiumWithCompany(DateTime startTime, DateTime endTime, int agentId)
        {
            var sql = string.Format(@"SELECT t.CompanyId Company,SUM(t.Total) Premium FROM dz_premium_company t WHERE t.AgentID={0}
                                    AND t.DateofPayment>='{1}' AND t.DateofPayment<'{2}' GROUP BY t.CompanyId", agentId, startTime, endTime);
            var dt = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null);
            dt.Columns.Add("Ratio");
            if (dt.Rows.Count > 0)
            {
                var sum = Convert.ToDecimal(dt.Compute("sum(Premium)", "true").ToString());
                foreach (DataRow dr in dt.Rows)
                {
                    dr["Ratio"] = Math.Round((Convert.ToDecimal(dr["Premium"]) / (sum > 0 ? sum : 1)), 4) * 100;
                }
            }
            return dt.ToList<InsuranceCompany>().ToList();

        }
        public List<InsuranceType> GetPremiumWithInsuranceType(DateTime startTime, DateTime endTime, int agentId)
        {
            var sql = string.Format(@"SELECT Insurance,IFNULL(Premium,0) Premium FROM (SELECT 1 Insurance,SUM(t.BizTotal) Premium
                                    FROM
	                                    dz_premium_company t
                                    WHERE
	                                    t.AgentID={0}
                                    AND t.DateofPayment>='{1}'
                                    AND t.DateofPayment<'{2}'
                                    UNION ALL
                                    SELECT
	                                    2 Insurance,SUM(t.ForceTotal) Premium
                                    FROM
	                                    dz_premium_company t
                                    WHERE
	                                    t.AgentID={0}
                                    AND t.DateofPayment>='{1}'
                                    AND t.DateofPayment<'{2}') t", agentId, startTime, endTime);
            var dt = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null);
            dt.Columns.Add("Ratio");
            var sum = Convert.ToDecimal(dt.Compute("sum(Premium)", "true").ToString());
            foreach (DataRow dr in dt.Rows)
            {
                dr["Ratio"] = Math.Round((Convert.ToDecimal(dr["Premium"]) / (sum > 0 ? sum : 1)), 4) * 100;
            }
            return dt.ToList<InsuranceType>().ToList();
        }

        public List<EntryOverViewModel> GetEntryOverView(int agentId, DateTime startTime, DateTime endTime, int viewType,int agentLevel)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"SELECT SUM(entry_count) EntryCount,
	                              SUM(renewal_period_count) RenewalPeriodCount,
	                              SUM(distributed_count) DistributedCount,
	                              (SUM(entry_count) - SUM(distributed_count)) UndistributeCount,
	                              SUM(quote_count) QuoteCount,
	                              (SUM(entry_count) - SUM(quote_count)) NotQuoteCount,
	                              SUM(review_count) ReviewCount,
	                              (SUM(entry_count) - SUM(review_count)) NotReviewCount{4}
                                  FROM
	                                bihu_analytics.tj_entry_overview t INNER JOIN bx_agent a ON t.agent_id = a.Id
                                  WHERE ");
            if (agentLevel == 1)
            {
                sbSql.Append("t.top_agent_id = {2} ");
            }
            else if (agentLevel == 2)
            {
                sbSql.Append("(t.parent_agent_id = {2} or t.agent_id = {2}) ");
            }
            else
            {
                sbSql.Append("t.agent_id = {2} ");
            }
            sbSql.Append("AND data_in_time >= '{0}' AND data_in_time < '{1}' {3}");
            var sql = string.Format(sbSql.ToString(), startTime, endTime, agentId, viewType == 2 ? "GROUP BY data_in_time" : "",
                                    viewType == 2 ? ",DATE_FORMAT(data_in_time,'%Y-%m-%d') DataInTime" : ",'' DataInTime");
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<EntryOverViewModel>().ToList();
        }

        public List<CustomerAnalysisViewModel> CustomerAnalysisOverView(List<string> agentIds, int topAgentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT a.Id AgentId,a.AgentName,a.ParentAgent ParentAgentId,a.TopAgentId,t1.CustomerCount,t1.DistributedCount,
                                    t1.CustomerCount-t1.DistributedCount UndistributeCount,
                                    t1.ReviewCount,
                                    t1.CustomerCount-t1.ReviewCount NotReviewCount,
                                    t1.QuoteCount,
                                    t1.CustomerCount-t1.QuoteCount NotQuoteCount,t1.OrderCount,
                                    t1.FailedCount FROM bx_agent a LEFT JOIN (SELECT tt.Agent,COUNT(1) CustomerCount,
			                                 SUM(CASE tt.IsDistributed WHEN 0 THEN 0 ELSE 1 END) DistributedCount,
			                                 SUM(CASE tt.IsReView WHEN 0 THEN 0 ELSE 1 END) ReviewCount,
			                                 SUM(CASE tt.QuoteStatus WHEN -1 THEN 0 WHEN 0 THEN 0 ELSE 1 END) QuoteCount,
			                                 SUM(CASE tt.IsReView WHEN 9 THEN 1 ELSE 0 END) OrderCount,
			                                 SUM(CASE tt.IsReView WHEN 4 THEN 1 ELSE 0 END) FailedCount FROM(
                                    SELECT ui.IsDistributed,ui.IsReView,QuoteStatus,ui.OrderStatus,ui.Agent,
                                                IF(
                                                    ui.renewalstatus=1,
                                                    IF(
                                                        YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                                                        cr.LastForceEndDate,
                                                        IF(YEAR(bx_batchrenewal_item.BizEndDate)>(IF(ISNULL(cr.LastBizEndDate),0,YEAR(cr.LastBizEndDate))),
                                                         bx_batchrenewal_item.ForceEndDate,
                                                         cr.LastForceEndDate
                                                        )
                                                    ),
                                                    bx_batchrenewal_item.ForceEndDate
                                                )
                                                 AS LastForceEndDate,
                                                IF(
                                                    ui.renewalstatus=1,
                                                    IF(
                                                        YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                                                        cr.LastBizEndDate,
                                                        IF(
                        
                                                    YEAR(bx_batchrenewal_item.BizEndDate)
                                                    > (
	                                                    IF(
		                                                    ISNULL(cr.LastBizEndDate),
		                                                    0,
		                                                    YEAR(cr.LastBizEndDate)
	                                                    )
                                                    )
                                                    ,
                                                            bx_batchrenewal_item.BizEndDate,
                                                            cr.LastBizEndDate
                                                        )
                                                    ),
                                                    bx_batchrenewal_item.BizEndDate
                                                )
                                                 AS LastBizEndDate
                                                FROM
                                                    bx_car_renewal cr
                                                        RIGHT JOIN
                                                    bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                                                        RIGHT JOIN
                                                    bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                                                        LEFT JOIN
                                                    bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId AND bx_batchrenewal_item.IsNew = 1 AND bx_batchrenewal_item.IsDelete = 0
                                                WHERE ui.IsTest=0
                                                 AND ui.agent IN ('{0}')
                                    AND (LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}' OR ForceEndDate BETWEEN '{1}' AND '{2}' OR BizEndDate BETWEEN '{1}' AND '{2}')) tt
                                    WHERE LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}' GROUP BY tt.Agent) t1 ON a.Id=CONVERT(t1.Agent,SIGNED)
                                    WHERE a.TopAgentId={3}", string.Join("','", agentIds), startTime, endTime, topAgentId);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<CustomerAnalysisViewModel>().ToList();
        }

        public List<DefeatAnalytics> DefeatAnalysis(List<string> agentIds, int topAgentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT a.Id AgentId,a.TopAgentId,a.ParentAgent ParentAgentId,DefeatReasonId,DefeatReasonContent FROM bx_agent a LEFT JOIN (
                                    SELECT
	                                    IF(@grp=t2.Id,@rank:=@rank+1,@rank:=1) rank,
	                                    @grp:=t2.Id,
	                                    t2.*
                                    FROM (SELECT t1.*,c.DefeatReasonId,c.DefeatReasonContent FROM (SELECT ui.Id,ui.agent,
								            IF(
										            ui.renewalstatus=1,
										            IF(
												            YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
												            cr.LastForceEndDate,
												            IF(YEAR(bx_batchrenewal_item.BizEndDate)>(IF(ISNULL(cr.LastBizEndDate),0,YEAR(cr.LastBizEndDate))),
												                bx_batchrenewal_item.ForceEndDate,
												                cr.LastForceEndDate
												            )
										            ),
										            bx_batchrenewal_item.ForceEndDate
								            )
								                AS LastForceEndDate,
								            IF(
										            ui.renewalstatus=1,
										            IF(
												            YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
												            cr.LastBizEndDate,
												            IF(

										            YEAR(bx_batchrenewal_item.BizEndDate)
										            > (
											            IF(
												            ISNULL(cr.LastBizEndDate),
												            0,
												            YEAR(cr.LastBizEndDate)
											            )
										            )
										            ,
														            bx_batchrenewal_item.BizEndDate,
														            cr.LastBizEndDate
												            )
										            ),
										            bx_batchrenewal_item.BizEndDate
								            )
								                AS LastBizEndDate
								            FROM
										            bx_car_renewal cr
												            RIGHT JOIN
										            bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
												            RIGHT JOIN
										            bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
												            LEFT JOIN
										            bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId AND bx_batchrenewal_item.IsNew = 1 AND bx_batchrenewal_item.IsDelete = 0
								            WHERE ui.IsTest=0 AND ui.IsReView=4
								                AND ui.agent IN ('{0}')
		                                    AND (LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}' OR ForceEndDate BETWEEN '{1}' AND '{2}' OR BizEndDate BETWEEN '{1}' AND '{2}')) t1
		                                    LEFT JOIN bx_consumer_review c ON t1.Id=c.b_uid AND c.status=4
		                                    WHERE LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}' ORDER BY t1.Id DESC,c.id DESC) t2,(SELECT @grp:=0,@rank:=0) init) t3 ON a.Id=CONVERT(t3.agent,SIGNED) WHERE a.TopAgentId={3} AND t3.rank=1", string.Join("','", agentIds),
                                            startTime, endTime, topAgentId);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<DefeatAnalytics>().ToList();
        }

        public List<GroupEntryViewModel> GetEntryDataByPage(List<int> agentIds, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT a.Id AgentId,a.AgentName,SUM(entry_count) EntryCount,
	                              SUM(renewal_period_count) RenewalPeriodCount,
	                              SUM(distributed_count) DistributedCount,
	                              (SUM(entry_count) - SUM(distributed_count)) UndistributeCount,
	                              SUM(quote_count) QuoteCount,
	                              (SUM(entry_count) - SUM(quote_count)) NotQuoteCount,
	                              SUM(review_count) ReviewCount,
	                              (SUM(entry_count) - SUM(review_count)) NotReviewCount
                                  FROM
	                                bx_agent a LEFT JOIN bihu_analytics.tj_entry_overview t ON a.Id=t.top_agent_id AND data_in_time>='{1}' AND data_in_time<'{2}'
                                  WHERE a.Id in ({0}) GROUP BY a.Id", string.Join(",", agentIds),
                                  startTime, endTime);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<GroupEntryViewModel>().ToList();
        }

        public List<CustomerAnalysisViewModel> CustomerAnalysisByGroup(List<int> childsAgentIds, List<int> topAgentIds, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT a.TopAgentId AgentId,COUNT(t.Agent) CustomerCount,
			                                 SUM(CASE IFNULL(t.IsDistributed,0) WHEN 0 THEN 0 ELSE 1 END) DistributedCount,
			                                 SUM(CASE IFNULL(t.IsReView,0) WHEN 0 THEN 0 ELSE 1 END) ReviewCount,
			                                 SUM(CASE IFNULL(t.QuoteStatus,0) WHEN -1 THEN 0 WHEN 0 THEN 0 ELSE 1 END) QuoteCount,
			                                 SUM(CASE IFNULL(t.IsReView,0) WHEN 9 THEN 1 ELSE 0 END) OrderCount,
			                                 SUM(CASE IFNULL(t.IsReView,0) WHEN 4 THEN 1 ELSE 0 END) FailedCount FROM bx_agent a LEFT JOIN (
                                             SELECT * FROM (
                                                 SELECT ui.IsDistributed,ui.IsReView,QuoteStatus,ui.OrderStatus,ui.Agent,IF(
                                                    ui.renewalstatus=1,
                                                    IF(
                                                        YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                                                        cr.LastForceEndDate,
                                                        IF(YEAR(bx_batchrenewal_item.BizEndDate)>(IF(ISNULL(cr.LastBizEndDate),0,YEAR(cr.LastBizEndDate))),
                                                         bx_batchrenewal_item.ForceEndDate,
                                                         cr.LastForceEndDate
                                                        )
                                                    ),
                                                    bx_batchrenewal_item.ForceEndDate
                                                )
                                                 AS LastForceEndDate,
                                                IF(
                                                    ui.renewalstatus=1,
                                                    IF(
                                                        YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                                                        cr.LastBizEndDate,
                                                        IF(
                        
                                                    YEAR(bx_batchrenewal_item.BizEndDate)
                                                    > (
	                                                    IF(
		                                                    ISNULL(cr.LastBizEndDate),
		                                                    0,
		                                                    YEAR(cr.LastBizEndDate)
	                                                    )
                                                    )
                                                    ,
                                                            bx_batchrenewal_item.BizEndDate,
                                                            cr.LastBizEndDate
                                                        )
                                                    ),
                                                    bx_batchrenewal_item.BizEndDate
                                                )
                                                 AS LastBizEndDate
                                                FROM
                                                    bx_car_renewal cr
                                                        RIGHT JOIN
                                                    bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                                                        RIGHT JOIN
                                                    bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                                                        LEFT JOIN
                                                    bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId AND bx_batchrenewal_item.IsNew = 1 AND bx_batchrenewal_item.IsDelete = 0
                                                WHERE ui.IsTest=0
                                                 AND ui.agent IN ('{0}')
                                    AND (LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}' OR ForceEndDate BETWEEN '{1}' AND '{2}' OR BizEndDate BETWEEN '{1}' AND '{2}')) tt
                                    WHERE LastForceEndDate BETWEEN '{1}' AND '{2}' OR LastBizEndDate BETWEEN '{1}' AND '{2}') t
                                    ON a.Id=CONVERT(t.Agent,SIGNED) WHERE a.TopAgentId in({3})
                                    GROUP BY a.TopAgentId", string.Join("','", childsAgentIds), startTime, endTime, string.Join(",", topAgentIds));
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql.ToString(), null).ToList<CustomerAnalysisViewModel>().ToList();
        }
    }

}
