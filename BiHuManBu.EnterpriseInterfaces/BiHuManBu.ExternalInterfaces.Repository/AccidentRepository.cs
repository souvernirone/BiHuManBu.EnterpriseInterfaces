using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{


    public class AccidentRepository : IAccidentRepository
    {
        private readonly MySqlHelper _mySqlHelper;

        private EntityContext db = DataContextFactory.GetDataContext();
        private ILog logInfo = LogManager.GetLogger("INFO");
        private string imageServer = ConfigurationManager.AppSettings["TXImageServer"];

        public AccidentRepository()
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
        }
        /// <summary>
        /// 获取线索管理列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="followState">线索状态</param>
        /// <param name="timeType">时间类型</param>
        /// <param name="carInfo">车牌、车架号</param>
        /// <param name="agentId">用户id</param>
        /// <returns></returns>
        public List<AccidentClueModel> GetClueList(int pageIndex, int pageSize, out int totalCount, int followState, int timeType, string carInfo, int agentId, int topAgentId, int roleType)
        {
            try
            {
                var sql = new StringBuilder();
                sql.Append(@"   SELECT tp.*,company.name AS CompanyName,IFNULL((SELECT ReciveCaragentid FROM tx_cluefollowuprecord foll WHERE foll.clueid = tp.id AND foll.state = 2 ORDER BY foll.id DESC LIMIT 1), 0) AS ReciveCaragentid,(SELECT COUNT(*) FROM tx_noticemessage notice WHERE  notice.culeid=tp.id AND notice.mesaagetype = 1 LIMIT 1) AS TimeoutNoticeCount,sms.smscontent AS SmsContent,(SELECT InsureInfo FROM tx_clueinsureinfo WHERE clueid = tp.id AND deleted=0 ORDER BY createTime DESC LIMIT 1) AS InsureInfo FROM (  SELECT clue.* FROM tx_clues_agent_relationship relation
	             LEFT JOIN tx_clues clue ON relation.ClueId = clue.id 
	             WHERE 1=1  ");
                /*4s店和主管、系统管理员*/
                if (agentId == topAgentId || roleType == 7 || roleType == 3)
                {
                    agentId = topAgentId;
                    sql.Append(" AND (relation.TopAgentId=?agentId) ");
                }
                else
                    sql.Append(" AND (relation.AgentId =?agentId) ");
                /*状态为跟进中状态 打电话、发短信息、上门接车*/
                if (string.IsNullOrWhiteSpace(carInfo))
                {
                    if (followState == 1)
                    {
                        sql.Append(" AND clue.followupstate in(1,2,5,6,7)  ");
                    }
                    else if (followState == 3)
                    {
                        sql.Append(" AND clue.followupstate in(3,8,9)  ");
                    }
                    else
                    {
                        sql.Append(" AND clue.followupstate =?followupstate  ");
                    }

                    switch (timeType)
                    {
                        /*今日*/
                        case 1:
                            sql.Append(" AND TO_DAYS(clue.CreateTime) = TO_DAYS(NOW()) ");
                            break;
                        /*昨天*/
                        case 2:
                            sql.Append(" AND TO_DAYS(NOW()) - TO_DAYS(clue.CreateTime) = 1 ");
                            break;
                        /*本周*/
                        case 3:
                            sql.Append(" AND YEARWEEK(DATE_FORMAT(clue.CreateTime,'%Y-%m-%d')- INTERVAL 1 DAY) = YEARWEEK(NOW()) ");
                            break;
                        /*本月*/
                        case 4:
                            sql.Append(" AND DATE_FORMAT(clue.CreateTime, '%Y%m') = DATE_FORMAT( CURDATE(), '%Y%m') ");
                            break;
                    }
                }
                else
                {
                    /*车牌、车架号检索*/
                    sql.Append(" AND (clue.licenseno LIKE ?carInfo or clue.MoldName LIKE ?carInfo)  ");
                }
                sql.Append(" GROUP BY clue.id ) AS tp LEFT JOIN bx_companyrelation company ON company.source = tp.source  LEFT JOIN tx_sms sms ON sms.id = tp.smsid   ");
                List<MySqlParameter> paramList = new List<MySqlParameter>();
                paramList.Add(new MySqlParameter("?agentId", MySqlDbType.Int32) { Value = agentId });
                paramList.Add(new MySqlParameter("?followupstate", MySqlDbType.Int32) { Value = followState });
                paramList.Add(new MySqlParameter("?carInfo", MySqlDbType.VarChar) { Value = '%' + carInfo + '%' });
                var totalSql = string.Format("SELECT COUNT(*) FROM ({0}) AS t", sql.ToString());
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(totalSql, paramList.ToArray()).FirstOrDefault();
                /*app端在根据车牌、车架号查询时，要求展示全部数据*/
                if (!string.IsNullOrWhiteSpace(carInfo) && totalCount > 0)
                    pageSize = totalCount;
                sql.Append(" ORDER BY CreateTime DESC   LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize + " ");
                var data = DataContextFactory.GetDataContext().Database.SqlQuery<CustomClueDataModel>(sql.ToString(), paramList.ToArray()).ToList();
                var result = new List<AccidentClueModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        TimeSpan span = DateTime.Now - item.CreateTime;
                        var model = new AccidentClueModel
                        {
                            CarVIN = item.CarVIN,
                            CaseType = item.casetype,
                            ClueId = item.id,
                            DangerArea = item.dangerarea,
                            FollowupState = item.followupstate,
                            LicenseNo = item.licenseno,
                            ReportCasePeople = item.ReportCasePeople,
                            SmsRecivedTime = item.smsrecivedtime,
                            SourceName = item.CompanyName,
                            /*根据讨论，用线索表的创建时间代替短信接收时间，避免和短信表级联*/
                            UpdateTime = (item.CreateTime == null ? "" : item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            TimeDifference = span.TotalSeconds,
                            Mobile = item.mobile,
                            ReciveCaragentid = item.ReciveCaragentid,
                            TimeoutNoticeCount = item.TimeoutNoticeCount,
                            SmsContent = item.SmsContent,
                            InsureInfo = AnalysisOfDangerousSpecies(item.InsureInfo),
                            ChosedModelName = item.ChosedModelName,
                            ClueFromType = item.ClueFromType,
                            AcceptedTime = item.AcceptedTime
                        };
                        var many = string.Empty;
                        var drive = string.Empty;
                        many = item.IsMany == 1 ? "多方事故" : "单方事故";
                        drive = item.IsDrivering == 2 ? "可以正常行驶" : "无法正常行驶";
                        model.DangerDec = many + "," + drive;
                        if (item.AcceptedTime != null)
                        {
                            try
                            {
                                TimeSpan accepted = DateTime.Now - item.AcceptedTime;
                                model.AcceptTimeDifference = accepted.TotalSeconds;
                            }
                            catch
                            {
                            }
                        }
                        result.Add(model);
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                totalCount = 0;
                logInfo.Info("线索列表出错：" + ex);
                return new List<AccidentClueModel>();
            }
        }

        private string AnalysisOfDangerousSpecies(string dangerousSpecies)
        {
            var message = "正在查询保单信息...";
            if (string.IsNullOrWhiteSpace(dangerousSpecies))
            {
                return message;
            }
            try
            {
                var data = JsonHelper.DeSerialize<GetReInfoNewViewModel>(dangerousSpecies);
                var result = string.Empty;
                if (data != null)
                {
                    if (data.UserInfo != null)
                    {
                        if (!string.IsNullOrWhiteSpace(data.UserInfo.BusinessExpireDate))
                        {
                            try
                            {
                                if (Convert.ToDateTime(data.UserInfo.BusinessExpireDate) > DateTime.Now)
                                {
                                    result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "商业险";
                                }
                            }
                            catch
                            {

                            }
                        }

                        if (!string.IsNullOrWhiteSpace(data.UserInfo.ForceExpireDate))
                        {
                            try
                            {
                                if (Convert.ToDateTime(data.UserInfo.ForceExpireDate) > DateTime.Now)
                                {
                                    result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "交强险";
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    if (data.SaveQuote != null)
                    {
                        if (data.SaveQuote.CheSun > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "机动车损失保险";
                        }
                        if (data.SaveQuote.SanZhe > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "第三者责任险";
                        }
                        if (data.SaveQuote.SiJi > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "司机座位险";
                        }
                        if (data.SaveQuote.ChengKe > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "乘客座位险";
                        }
                        if (data.SaveQuote.DaoQiang > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "盗抢险";
                        }
                        if (data.SaveQuote.HuaHen > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "划痕险";
                        }
                        if (data.SaveQuote.BoLi > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "玻璃单独破碎险";
                        }
                        if (data.SaveQuote.ZiRan > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "自燃损失险";
                        }
                        if (data.SaveQuote.SheShui > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "涉水行驶损失险";
                        }
                        if (data.SaveQuote.HcJingShenSunShi > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "精神损失险";
                        }
                        if (data.SaveQuote.HcSanFangTeYue > 0)
                        {
                            result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "车损无法找到第三方险";
                        }
                        if (!string.IsNullOrWhiteSpace(data.SaveQuote.HcXiuLiChang))
                        {
                            try
                            {
                                var hc = Convert.ToInt32(data.SaveQuote.HcXiuLiChang);
                                if (hc > 0)
                                    result = result + (string.IsNullOrWhiteSpace(result) ? "" : ",") + "指定修理厂险";
                            }
                            catch
                            {
                            }
                        }
                    }
                    return string.IsNullOrWhiteSpace(result) ? "未查询到险种信息" : result;
                }
            }
            catch (Exception ex)
            {
                return message;
            }
            return message;
        }

        public List<AccidentClueModel> GetClueList(AccidentListRequest accidentListRequest, out int totalCount)
        {
            var sql = "";
            var countSql = "";
            var strWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(accidentListRequest.LicenseNo))
            {
                strWhere.Append(" and a.licenseno like '%" + accidentListRequest.LicenseNo + "%'");
            }
            if (accidentListRequest.CaseType >= 0)
            {
                strWhere.Append(" and a.casetype=" + accidentListRequest.CaseType);
            }
            if (accidentListRequest.Source > -2)
            {
                if (accidentListRequest.Source == -1)
                {
                    strWhere.Append(" and a.source not in (0,1,2,3)");
                }
                else
                {
                    strWhere.Append(" and a.source=" + accidentListRequest.Source);
                }
            }
            if (!string.IsNullOrEmpty(accidentListRequest.ReportCasePeople))
            {
                strWhere.Append(" and a.ReportCasePeople like '%" + accidentListRequest.ReportCasePeople + "%'");
            }
            if (!string.IsNullOrEmpty(accidentListRequest.LastFollowAgent))
            {
                strWhere.Append(" and c.AgentName like '%" + accidentListRequest.LastFollowAgent + "%'");
            }
            if (accidentListRequest.State != 0)
            {
                if (accidentListRequest.State == -2)
                {
                    strWhere.Append(" and a.followupstate in (1,2,5,6,7)");
                }
                else
                {
                    strWhere.Append(" and a.followupstate=" + accidentListRequest.State);
                }
            }
            if (accidentListRequest.RoleType == 7 || accidentListRequest.RoleType == 3)
            {
                sql = string.Format(@"SELECT a.id ClueId,a.licenseno LicenseNo,
                                    CASE WHEN a.ClueFromType=1 THEN DATE_FORMAT(s.createtime,'%Y-%m-%d %H:%i:%s')
                                    ELSE DATE_FORMAT(a.AcceptedTime,'%Y-%m-%d %H:%i:%s')
                                    END
                                    SmsRecivedTime,
                                    a.dangerarea DangerArea,com.alias SourceName,a.casetype CaseType,a.followupstate FollowupState,c.AgentName LastFollowAgent,DATE_FORMAT(b.CreateTime,'%Y-%m-%d %H:%i:%s') LastFollowTime,ReportCasePeople,CarVIN,
                                    CASE WHEN a.ClueFromType=1 THEN s.SmsContent
                                    ELSE a.accidentremark
                                    END
                                    SmsContent,
                                    case b.state when 5 then b.smscontent else b.remark end Remark,a.ReportCaseNum,b.remark LastFollowContent,ClueFromType,a.MaintainAmount,d.AuditedState
                                    FROM tx_clues a LEFT JOIN bx_companyrelation com on a.source=com.source LEFT JOIN tx_sms s on a.smsid=s.id
                                    LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id
                                    LEFT JOIN bx_agent c on b.fromagentid=c.Id
                                    LEFT JOIN tx_settlement_detail d on a.id=d.OrderId
                                    WHERE a.agentid={0} and a.createtime>='{1}' and a.createtime<'{2}' {3} order by a.id desc limit {4},{5}", accidentListRequest.TopAgentId,
                                    accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString(),
                                    (accidentListRequest.PageIndex - 1) * accidentListRequest.PageSize, accidentListRequest.PageSize);
                countSql = string.Format(@"SELECT count(a.id) FROM tx_clues a LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id
                                        LEFT JOIN bx_agent c on b.toagentid=c.Id
                                        where a.agentid={0} and a.createtime>='{1}' and a.createtime<'{2}' {3}", accidentListRequest.TopAgentId,
                                    accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString());
            }
            else
            {
                sql = string.Format(@"SELECT a.id ClueId,a.licenseno LicenseNo,
                                    CASE WHEN a.ClueFromType=1 THEN DATE_FORMAT(s.createtime,'%Y-%m-%d %H:%i:%s')
                                    ELSE DATE_FORMAT(a.AcceptedTime,'%Y-%m-%d %H:%i:%s')
                                    END
                                    SmsRecivedTime,
                                    a.dangerarea DangerArea,com.alias SourceName,a.casetype CaseType,a.followupstate FollowupState,c.AgentName LastFollowAgent,DATE_FORMAT(b.CreateTime,'%Y-%m-%d %H:%i:%s') LastFollowTime,ReportCasePeople,CarVIN,
                                    CASE WHEN a.ClueFromType=1 THEN s.SmsContent
                                    ELSE a.accidentremark
                                    END
                                    SmsContent,
                                    case b.state when 5 then b.smscontent else b.remark end Remark,a.ReportCaseNum,b.remark LastFollowContent,ClueFromType,a.MaintainAmount,d.AuditedState
                                    from tx_clues_agent_relationship t LEFT JOIN tx_clues a on t.ClueId=a.id LEFT JOIN bx_companyrelation com on a.source=com.source LEFT JOIN tx_sms s on a.smsid=s.id
                                    LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id
                                    LEFT JOIN bx_agent c on b.fromagentid=c.Id
                                    LEFT JOIN tx_settlement_detail d on a.id=d.OrderId
                                    WHERE t.AgentId={0} and a.createtime>='{1}' and a.createtime<'{2}' {3} order by a.id desc limit {4},{5}", accidentListRequest.AgentId,
                                    accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString(),
                                    (accidentListRequest.PageIndex - 1) * accidentListRequest.PageSize, accidentListRequest.PageSize);
                countSql = string.Format(@"SELECT count(a.id) from tx_clues_agent_relationship t LEFT JOIN tx_clues a on t.ClueId=a.id 
                                        LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id
                                        LEFT JOIN bx_agent c on b.toagentid=c.Id
                                        where t.AgentId={0} and a.createtime>='{1}' and a.createtime<'{2}' {3}", accidentListRequest.AgentId,
                                        accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString());
            }
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<AccidentClueModel>().ToList();
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, countSql, null));
            return result;
        }

        public Dictionary<int, int> GetCluesCountWithState(AccidentListRequest accidentListRequest)
        {
            var countSql = "";
            var strWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(accidentListRequest.LicenseNo))
            {
                strWhere.Append(" and a.licenseno like '%" + accidentListRequest.LicenseNo + "%'");
            }
            if (accidentListRequest.CaseType >= 0)
            {
                strWhere.Append(" and a.casetype=" + accidentListRequest.CaseType);
            }
            if (accidentListRequest.Source > -2)
            {
                if (accidentListRequest.Source == -1)
                {
                    strWhere.Append(" and a.source not in (0,1,2,3)");
                }
                else
                {
                    strWhere.Append(" and a.source=" + accidentListRequest.Source);
                }
            }
            if (!string.IsNullOrEmpty(accidentListRequest.ReportCasePeople))
            {
                strWhere.Append(" and a.ReportCasePeople like '%" + accidentListRequest.ReportCasePeople + "%'");
            }
            if (!string.IsNullOrEmpty(accidentListRequest.LastFollowAgent))
            {
                strWhere.Append(" and c.AgentName like '%" + accidentListRequest.LastFollowAgent + "%'");
            }
            if (accidentListRequest.State != 0)
            {
                if (accidentListRequest.State == -2)
                {
                    strWhere.Append(" and a.followupstate in (1,2,5,6,7)");
                }
                else
                {
                    strWhere.Append(" and a.followupstate=" + accidentListRequest.State);
                }
            }
            if (accidentListRequest.RoleType == 7 || accidentListRequest.RoleType == 3)
            {
                countSql = string.Format(@"SELECT a.followupstate,count(a.id) cnt FROM tx_clues a {4}
                                        where a.agentid={0} and a.createtime>='{1}' and a.createtime<'{2}' {3} GROUP BY a.followupstate", accidentListRequest.TopAgentId,
                                        accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString(),
                                        string.IsNullOrEmpty(accidentListRequest.LastFollowAgent) ? "" : "LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id LEFT JOIN bx_agent c on b.toagentid = c.Id ");
            }
            else
            {
                countSql = string.Format(@"SELECT a.followupstate,count(a.id) cnt
                                    from tx_clues_agent_relationship t LEFT JOIN tx_clues a on t.ClueId=a.id {4}
                                    where t.AgentId={0} and a.createtime>='{1}' and a.createtime<'{2}' {3} GROUP BY a.followupstate", accidentListRequest.AgentId,
                                    accidentListRequest.SMSStartTime, accidentListRequest.SMSEndTime.AddDays(1), strWhere.ToString(),
                                    string.IsNullOrEmpty(accidentListRequest.LastFollowAgent) ? "" : "LEFT JOIN tx_cluefollowuprecord b on a.last_follow_id=b.id LEFT JOIN bx_agent c on b.toagentid = c.Id ");
            }
            var dt = _mySqlHelper.ExecuteDataTable(countSql);
            return dt.Rows.Cast<DataRow>().ToDictionary(x => Convert.ToInt32(x[0]), x => Convert.ToInt32(x[1]));
        }



        public int SaveSMS(tx_sms model)
        {
            string sql = string.Format("insert ignore into tx_sms(agentid,mobile,smscontent,createtime,updatetime,casetime )values ({0},'{1}','{2}','{3}','{4}','{5}');SELECT @@Identity;", model.agentid, model.mobile, model.smscontent, model.createtime, model.updatetime, model.casetime);
            return Convert.ToInt32(_mySqlHelper.ExecuteScalar(sql));
            //try
            //{
            //    db.tx_sms.Add(model);
            //    if (db.SaveChanges() > 0)
            //    {
            //        return model.id;
            //    }
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    return 0;
            //}

        }



        public bool SMSIsExist(string mobile, string smsContent)
        {

            string sql = string.Format("SELECT * FROM tx_sms  WHERE  mobile=?mobile   AND TO_DAYS(createtime) = TO_DAYS(NOW())  AND smscontent=?smscontent", mobile, smsContent);

            List<MySqlParameter> paramList = new List<MySqlParameter>();
            paramList.Add(new MySqlParameter("?mobile", MySqlDbType.VarChar) { Value = mobile });
            paramList.Add(new MySqlParameter("?smscontent", MySqlDbType.VarChar) { Value = smsContent });
            return _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, paramList.ToArray()) != null;
        }




        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="timeType"></param>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public AccidentClueTotalModel GetTotalCount(int timeType, int agentId, int topAgentId, int roleType)
        {
            try
            {
                var sql = new StringBuilder();
                sql.Append(@"   SELECT 
                        IFNULL(SUM(CASE WHEN followupstate=-1  THEN 1 ELSE 0 END),0) AS UntreatedTotalCount,
                        IFNULL(SUM(CASE WHEN followupstate IN(1,2,5,6,7)   THEN 1 ELSE 0 END),0) AS FollowUpTotalCount,
                        IFNULL(SUM(CASE WHEN followupstate =3  THEN 1 ELSE 0 END),0) AS VehicleToStoreTotalCount,
                        IFNULL(SUM(CASE WHEN followupstate =4  THEN 1 ELSE 0 END),0) AS LossTotalCount
                        FROM (
                       SELECT clue.* FROM tx_clues_agent_relationship  relation 
                       LEFT JOIN tx_clues clue ON relation.ClueId = clue.id 
                       WHERE clue.source BETWEEN 0 AND 3 ");
                /*4s店和主管、系统管理员*/
                if (agentId == topAgentId || roleType == 7 || roleType == 3)
                {
                    agentId = topAgentId;
                    sql.Append(" AND (relation.TopAgentId=?agentId) ");
                }
                else
                    sql.Append(" AND (relation.AgentId =?agentId) ");
                switch (timeType)
                {
                    /*今日*/
                    case 1:
                        sql.Append(" AND TO_DAYS(clue.CreateTime) = TO_DAYS(NOW()) ");
                        break;
                    /*昨天*/
                    case 2:
                        sql.Append(" AND TO_DAYS(NOW()) - TO_DAYS(clue.CreateTime) = 1 ");
                        break;
                    /*本周*/
                    case 3:
                        sql.Append(" AND YEARWEEK(DATE_FORMAT(clue.CreateTime,'%Y-%m-%d')- INTERVAL 1 DAY) = YEARWEEK(NOW()) ");
                        break;
                    /*本月*/
                    case 4:
                        sql.Append(" AND DATE_FORMAT(clue.CreateTime, '%Y%m') = DATE_FORMAT( CURDATE(), '%Y%m') ");
                        break;
                }
                sql.Append(" GROUP BY clue.id ) AS t  ");
                List<MySqlParameter> paramList = new List<MySqlParameter>();
                paramList.Add(new MySqlParameter("?agentId", MySqlDbType.Int32) { Value = agentId });
                var model = DataContextFactory.GetDataContext().Database.SqlQuery<AccidentClueTotalModel>(sql.ToString(), paramList.ToArray()).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                return new AccidentClueTotalModel();
            }
        }

        public ClueStatisticalViewModel GetClueStatisticalViewModel(int agentId, DateTime startTime, DateTime endTime, int roleType)
        {
            var sql = "";
            if (roleType == 7 || roleType == 3)
            {
                sql = string.Format(@"SELECT SUM(CASE WHEN followupstate = -1 THEN 1 ELSE 0 END) UnhandleCount,
                                    SUM(CASE WHEN followupstate in (1,2,5,6,7) THEN 1 ELSE 0 END) FollowUpCount,
                                    SUM(CASE WHEN followupstate=3 THEN 1 ELSE 0 END) ReachDealersCount,
                                    SUM(CASE WHEN followupstate=4 THEN 1 ELSE 0 END) LossCount,
                                    SUM(CASE WHEN followupstate=8 THEN 1 ELSE 0 END) MaintainCount,
                                    SUM(CASE WHEN followupstate=9 OR followupstate=10 THEN 1 ELSE 0 END) HandOverCount
                                    from tx_clues t
                                    WHERE t.agentid={0} AND t.CreateTime>='{1}' AND t.CreateTime<'{2}'", agentId, startTime, endTime);
            }
            else
            {
                sql = string.Format(@"SELECT SUM(CASE WHEN followupstate=-1 THEN 1 ELSE 0 END) UnhandleCount,
                                    SUM(CASE WHEN followupstate in(1,2,5,6,7) THEN 1 ELSE 0 END) FollowUpCount,
                                    SUM(CASE WHEN followupstate=3 THEN 1 ELSE 0 END) ReachDealersCount,
                                    SUM(CASE WHEN followupstate=4 THEN 1 ELSE 0 END) LossCount,
                                    SUM(CASE WHEN followupstate=8 THEN 1 ELSE 0 END) MaintainCount,
                                    SUM(CASE WHEN followupstate=9 OR followupstate=10 THEN 1 ELSE 0 END) HandOverCount
                                    from tx_clues_agent_relationship r INNER JOIN tx_clues t ON r.ClueId=t.id
                                    WHERE r.AgentId={0} AND t.CreateTime>='{1}' AND t.CreateTime<'{2}'", agentId, startTime, endTime);
            }
            var result = _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, null).ToT<ClueStatisticalViewModel>();
            return result;
        }

        public List<ClueStatisticalWithCompany> GetClueStatisticalWithCompany(int agentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT CompanyId,CaseType,CASE CaseType WHEN 1 THEN '送修' WHEN 2 THEN '返修' WHEN 3 THEN '三者车' ELSE '未知' END CaseName,SUM(t.TotalCount) TotalCount,SUM(ReachDealersCount) ReachDealersCount FROM(
                                    SELECT CompanyId,CaseType,COUNT(Id) TotalCount,
                                    SUM(CASE WHEN State=3 THEN 1 ELSE 0 END) ReachDealersCount FROM(
                                    SELECT t.Id,CASE WHEN t.source in(0,1,2,3) THEN t.source ELSE -1 END CompanyId,t.casetype CaseType,
                                    t.followupstate State
                                    FROM tx_clues t WHERE t.agentid={0} AND t.CreateTime>='{1}' AND t.CreateTime<'{2}') t1
                                    GROUP BY t1.CompanyId,t1.CaseType) t GROUP BY t.CompanyId,t.CaseType", agentId, startTime, endTime);
            var dt = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null);
            var query = from c in dt.AsEnumerable()
                        group c by new
                        {
                            CompanyId = c.Field<Int64>("CompanyId")
                        } into s
                        select new ClueStatisticalWithCompany
                        {
                            CompanyId = (int)s.Select(p => p.Field<Int64>("CompanyId")).First(),
                            clueStatisticalWithStates = (from s1 in s
                                                         group s1 by new
                                                         {
                                                             CaseType = s1.Field<int>("CaseType")
                                                         } into s2
                                                         select new ClueStatisticalWithState
                                                         {
                                                             TotalCount = (int)s2.Select(p => p.Field<decimal>("TotalCount")).First(),
                                                             ReachDealersCount = (int)s2.Select(p => p.Field<decimal>("ReachDealersCount")).First(),
                                                             CaseType = s2.Select(p => p.Field<int>("CaseType")).First(),
                                                             CaseName = s2.Select(p => p.Field<string>("CaseName")).First()
                                                         }).OrderByDescending(x => x.CaseType).ToList()
                        };
            return query.ToList();
        }

        public List<ClueResponsivity> GetClueResponsivity(int agentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT t.fromagentid,a.AgentName,CASE WHEN MINUTEDIFF<=3 THEN 3 WHEN MINUTEDIFF>3 AND MINUTEDIFF<=5 THEN 5 ELSE 6 END MINUTEDIFF FROM tx_cluefollowuprecord t
                                    INNER JOIN
                                    (SELECT MIN(a.CreateTime) CreateTime,a.clueid,TIMESTAMPDIFF(MINUTE,c.CreateTime,a.CreateTime) MINUTEDIFF FROM tx_cluefollowuprecord a INNER JOIN tx_clues c ON a.clueid=c.id WHERE a.state=6
                                    AND a.Deleted=0 AND c.agentid={0} AND c.createtime>='{1}' and c.createtime<'{2}'
                                    GROUP BY a.clueid) b ON t.CreateTime=b.CreateTime AND t.clueid=b.clueid
                                    INNER JOIN bx_agent a
                                    ON t.fromagentid=a.Id", agentId, startTime, endTime);
            var dt = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null);
            var query = from c in dt.AsEnumerable()
                        group c by new
                        {
                            fromagentid = c.Field<int>("fromagentid")
                        } into s
                        select new ClueResponsivity
                        {
                            AgentId = s.Select(p => p.Field<int>("fromagentid")).First(),
                            AgentName = s.Select(p => p.Field<string>("AgentName")).First(),
                            TotalCount = s.Count(),
                            OneToThreeMinutesCount = s.Count(p => p.Field<Int64>("MINUTEDIFF") == 3),
                            ThreeToFiveMinutesCount = s.Count(p => p.Field<Int64>("MINUTEDIFF") == 5),
                            OverFiveMinutesCount = s.Count(p => p.Field<Int64>("MINUTEDIFF") == 6)
                        };
            var result = query.ToList();
            return result;
        }

        public List<LossStatistical> GetLossStatistical(int agentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT r.reason LossOfReason,COUNT(*) LossCount from tx_clues a LEFT JOIN tx_cluefollowuprecord b ON a.last_follow_id=b.id
                                    LEFT JOIN tx_loss_reason r ON b.loss_reason_id=r.id
                                    WHERE a.followupstate=4
                                    and a.agentid={0}
                                    and a.createtime>='{1}'
                                    and a.createtime<'{2}'
                                    and a.Deleted=0 and b.Deleted=0
                                    GROUP BY b.loss_reason_id,r.reason", agentId, startTime, endTime);
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<LossStatistical>().ToList();
            return result;
        }

        public AccidentClueModel GetClusDetails(int clueId)
        {
            var sql = string.Format(@"select c.id ClueId,c.licenseno LicenseNo,MoldName,casetype CaseType,accidentremark AccidentRemark,followupstate FollowupState,ReportCaseNum,
                                    ReportCasePeople,c.mobile Mobile,
                                    CASE WHEN c.ClueFromType=1 THEN DATE_FORMAT(s.createtime,'%Y-%m-%d %H:%i:%s')
                                    ELSE DATE_FORMAT(c.AcceptedTime,'%Y-%m-%d %H:%i:%s')
                                    END
                                    SmsRecivedTime,dangerarea DangerArea,SourceName,s.SmsContent,c.MaintainAmount,ClueFromType,c.OrderNum,d.AuditedState,c.ReceiveCarAddress,IsMany,IsDrivering,
                                    (SELECT COUNT(1) FROM tx_noticemessage WHERE culeid={0} AND mesaagetype=1) AS TimeoutNoticeCount
                                    from tx_clues c left join tx_sms s on c.smsid=s.id LEFT JOIN tx_settlement_detail d on c.OrderNum=d.OrderNum where c.id={0}", clueId);
            var result = _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, null).ToT<AccidentClueModel>();
            return result;
        }

        public List<AccidentFollowRecord> GetFollowUpRecords(int clueId)
        {
            var sql = string.Format(@"SELECT c.AgentName,b.Mobile,f.role_name RoleName,a.SmsContent,a.State,DATE_FORMAT(a.CreateTime,'%Y-%m-%d %H:%i:%s') CreateTime,DATE_FORMAT(a.NextFollowUpTime,'%Y-%m-%d %H:%i:%s') NextFollowUpTime,a.ReciveCarAea,d.AgentName ReciveCarAgent,ArrivalType,ReciveCarAgentId,
                                    e.reason LossReason,Remark,DATE_FORMAT(a.expect_arrival_time,'%Y-%m-%d %H:%i:%s') ExpectArrivalTime,DATE_FORMAT(b.ExpectedFinishedTime,'%Y-%m-%d %H:%i:%s') ExpectedFinishedTime
                                    FROM tx_cluefollowuprecord a 
                                    INNER JOIN tx_clues b ON a.clueid=b.id
                                    INNER JOIN bx_agent c ON a.fromagentid=c.Id
                                    INNER JOIN manager_role_db f ON c.ManagerRoleId=f.Id
                                    LEFT JOIN bx_agent d ON a.ReciveCaragentid=d.Id
                                    LEFT JOIN tx_loss_reason e ON a.loss_reason_id=e.id
                                    WHERE a.clueid={0} AND a.Deleted=0 AND a.state in(1,2,3,4,5,6,7,8,9) order by a.id desc", clueId);
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<AccidentFollowRecord>().ToList();
            return result;
        }

        public AccidentClueModel GetClusDetail(int clueId)
        {
            var sql = string.Format(@" SELECT tx_clueinsureinfo.InsureInfo,(SELECT COUNT(*) FROM tx_noticemessage notice WHERE  notice.culeid={0} AND notice.mesaagetype = 1 LIMIT 1) AS TimeoutNoticeCount,tx_cluefollowuprecord.ReciveCaragentid,tx_sms.smscontent SmsContent,tx_clues.id ClueId,tx_clues.UpdateTime,tx_clues.CreateTime ClueTime,tx_clues.sourcename SourceName,tx_clues.licenseno LicenseNo,tx_clues.MoldName,tx_clues.casetype CaseType,tx_clues.accidentremark AccidentRemark,tx_clues.followupstate FollowupState,tx_clues.ReportCaseNum,
                                    tx_clues.ReportCasePeople,tx_clues.mobile Mobile,DATE_FORMAT(tx_clues.smsrecivedtime,'%Y-%m-%d %H:%i:%s') SmsRecivedTime,tx_clues.dangerarea DangerArea,tx_clues.source Source FROM tx_clues
INNER JOIN   tx_sms ON   tx_clues.smsid=tx_sms.id    
  LEFT JOIN tx_cluefollowuprecord   ON tx_clues.id=tx_cluefollowuprecord.clueid
  LEFT JOIN tx_clueinsureinfo  ON   tx_clueinsureinfo.clueid=tx_clues.id
 WHERE tx_clues.id={0}     ORDER BY tx_cluefollowuprecord.CreateTime  DESC LIMIT 1", clueId);
            var result = _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, null).ToT<AccidentClueModel>();
            //  var result = db.Database.SqlQuery<AccidentClueModel>(sql).FirstOrDefault();
            return result;
        }


        public List<string> GetClusImage(int clueId)
        {
            string sql = string.Format("SELECT Url FROM  tx_clue_image WHERE tx_clue_image.ClueId={0} AND tx_clue_image.ImageType=1 AND Deleted=1", clueId);

            List<string> imageList = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<string>().ToList();
            var imageListWithServer = new List<string>();
            foreach (var image in imageList)
            {
                imageListWithServer.Add(imageServer + image);
            }
            return imageListWithServer;
        }


        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public List<AccidentSMSTempModel> GetTempList(int topAgentId)
        {
            try
            {
                var sql = " SELECT id AS TemplateId, agentid AS TopAgentId,SmsTemplateName,SmsTemplateContent FROM tx_smstemplatesetting WHERE agentid = ?agentId AND deleted = 0 ";
                List<MySqlParameter> paramList = new List<MySqlParameter>();
                paramList.Add(new MySqlParameter("?agentId", MySqlDbType.Int32) { Value = topAgentId });
                return DataContextFactory.GetDataContext().Database.SqlQuery<AccidentSMSTempModel>(sql.ToString(), paramList.ToArray()).ToList();
            }
            catch (Exception)
            {
                return new List<AccidentSMSTempModel>();
            }
        }

        public int HasNextState(int id)
        {
            string sql = string.Format("select nextstate from tx_cluefollowuprecord where id={0} and Deleted=0", id);
            object obj = _mySqlHelper.ExecuteScalar(CommandType.Text, sql, null);
            logInfo.Info(sql);
            //string nextstate =obj==null?"":obj.ToString();
            //var result = db.tx_cluefollowuprecord.SingleOrDefault(x => x.id == id && x.Deleted == 0);
            if (obj == null) throw new Exception(string.Format("无此跟进记录，记录Id为{0}", id));
            else return Convert.ToInt32(obj);
        }


        public int CluesState(int cluesId)
        {
            string sql = string.Format("select followupstate from tx_clues where id={0} and Deleted=0", cluesId);
            object obj = _mySqlHelper.ExecuteScalar(CommandType.Text, sql, null);
            logInfo.Info(sql);
            //string nextstate =obj==null?"":obj.ToString();
            //var result = db.tx_cluefollowuprecord.SingleOrDefault(x => x.id == id && x.Deleted == 0);
            if (obj == null) throw new Exception(string.Format("无此线索，线索Id为{0}", cluesId));
            else return Convert.ToInt32(obj);

        }



        public List<ClueState> GetFollowUpStates(int agentId)
        {
            var result = db.tx_state.Where(x => (x.agentid == 0 || x.agentid == agentId) && x.deleted == 0 && x.state != 8 && x.state != 9).Select(x => new ClueState { StateId = x.id, State = x.state, StateInfo = x.stateinfo }).ToList();
            return result;
        }

        public List<ClueLossReason> GetLossReasons(int agentId)
        {
            var result = db.tx_loss_reason.Where(x => (x.agent_id == 0 || x.agent_id == agentId) && x.is_delete == 0).Select(x => new ClueLossReason { ReasonId = x.id, Reason = x.reason }).ToList();
            return result;
        }

        public List<RecivesCarPeople> GetRecivesCarPeoples(int topAgentId)
        {
            var sql = string.Format(@"SELECT a.Id AgentId,a.AgentName FROM bx_agent a INNER JOIN manager_role_function_relation b
                                    ON a.ManagerRoleId=b.role_id AND b.function_code='dtd_connection'
                                    WHERE a.TopAgentId={0}", topAgentId);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<RecivesCarPeople>().ToList();
        }

        public int UpdateClue(int state, int clueId, int followId)
        {
            var clue = db.tx_clues.Where(x => x.id == clueId).FirstOrDefault();
            if (clue != null)
            {
                clue.followupstate = state;
                clue.last_follow_id = followId;
                clue.UpdateTime = DateTime.Now;
                return db.SaveChanges();
            }
            return -1;
        }

        public string GetReInfo(int clueId)
        {
            return db.tx_clueinsureinfo.Where(x => x.clueid == clueId).Select(x => x.InsureInfo).FirstOrDefault();
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public manager_role_db GetRoleModelById(int roleId)
        {
            return db.manager_role_db.Where(o => o.id == roleId).FirstOrDefault();
        }

        public List<int> GetManagerIdByTopAgentId(int topAgentId)
        {
            var sql = string.Format(@"SELECT a.Id AgentId FROM bx_agent a INNER JOIN manager_role_db b
                                    ON a.ManagerRoleId=b.id AND b.role_type=7
                                    WHERE a.TopAgentId={0}", topAgentId);
            return db.Database.SqlQuery<int>(sql).ToList();
        }


        /// <summary>
        /// 添加或修改推修手机运行状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int MobileServiceAddOrUpdate(tx_mobileservicestatus model)
        {
            try
            {
                if (model == null) return 0;
                var exitModel = db.tx_mobileservicestatus.Where(o => o.PhoneNumber == model.PhoneNumber && o.CustKey == model.CustKey).FirstOrDefault();
                if (exitModel == null || exitModel.Id < 1)
                {
                    db.tx_mobileservicestatus.Add(model);
                    if (db.SaveChanges() > 0)
                        return model.Id;
                    else
                        return 0;
                }
                else
                {
                    exitModel.IsAvailable = model.IsAvailable;
                    exitModel.NetWorkType = model.NetWorkType;
                    exitModel.IsConnectSupply = model.IsConnectSupply;
                    exitModel.BatteryCapacity = model.BatteryCapacity;
                    exitModel.UpdateTime = DateTime.Now;
                    return db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 根据顶级id更新门店地址信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public int InsertAddressModel(int topAgentId, string address)
        {
            var seach = db.tx_storeaddress.Where(x => x.TopAgentId == topAgentId).FirstOrDefault();
            if (seach != null && seach.Id > 0)
            {
                seach.Address = address;
                seach.UpdateTime = DateTime.Now;
                return db.SaveChanges();
            }
            else
            {
                var model = new tx_storeaddress
                {
                    Address = address,
                    TopAgentId = topAgentId,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                var result = db.tx_storeaddress.Add(model);
                db.SaveChanges();
                return result.Id;
            }
        }

        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public tx_storeaddress GetAddressModel(int topAgentId)
        {
            var model = new tx_storeaddress();
            if (topAgentId < 1)
            {
                return model;
            }
            return db.tx_storeaddress.Where(x => x.TopAgentId == topAgentId).FirstOrDefault();
        }

		public ClueOrderStatistical ClueOrderStatistical(int topAgentId, DateTime startTime, DateTime endTime)
        {
            var sql = string.Format(@"SELECT
                                        COUNT(1) TotalCount,
	                                    IFNULL(SUM(CASE t.AcceptState WHEN 1 THEN 1 ELSE 0 END),0) SuccessCount,
	                                    IFNULL(SUM(CASE t.AcceptState WHEN 2 THEN 1 ELSE 0 END),0) FailCount
                                    FROM

                                        tx_pushed_detail t
                                    WHERE

                                        t.ToAgentId = {0}
                                    AND t.CreateTime >= '{1}'
                                    AND t.CreateTime < '{2}'
                                    AND(t.AcceptState = 1 OR t.AcceptState = 2)", topAgentId, startTime, endTime);
            return db.Database.SqlQuery<ClueOrderStatistical>(sql).FirstOrDefault();
        }

        public List<FunctionCodeModel> GetFunctionCodeByAgentId(int agentId)
        {
            try
            {
                var sql = string.Format(@"SELECT a.Id AgentId,b.function_code AS 'FunctionCode' FROM bx_agent a INNER JOIN manager_role_function_relation b
    ON a.ManagerRoleId=b.role_id
    WHERE a.Id={0}", agentId);
                return db.Database.SqlQuery<FunctionCodeModel>(sql).ToList();
            }
            catch
            {
                return new List<FunctionCodeModel>();
            }
        }
    }
}
