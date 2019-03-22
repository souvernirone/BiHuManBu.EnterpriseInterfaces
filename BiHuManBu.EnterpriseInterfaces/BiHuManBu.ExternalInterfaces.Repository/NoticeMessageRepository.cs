using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class NoticeMessageRepository: INoticeMessageRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");

        /// <summary>
        /// V 2.2.0及以前
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentId"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<AccidentNoticeMessageModel> GetMessageList(int pageIndex, int pageSize, int agentId, out int totalCount)
        {
            totalCount = 0;
            try
            {
                var sql = @"SELECT SUM((SELECT COUNT(*) FROM tx_noticemessage WHERE  reciveaentId=?reciveaentId AND mesaagetype not in (1,7,8,9))) + SUM((SELECT COUNT(*) FROM (SELECT culeid FROM tx_noticemessage
                WHERE  reciveaentId = ?reciveaentId AND mesaagetype = 1
                GROUP BY culeid ) t)) AS TotalCount ";
                var param = new MySqlParameter[]
               {
                    new MySqlParameter
                    {
                        ParameterName="?reciveaentId",
                        Value=agentId,
                        MySqlDbType=MySqlDbType.Int32
                    }
               };
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();



                var listSql = string.Format(@"	SELECT * FROM (
                    SELECT mess1.*,
                    CASE WHEN mess1.mesaagetype = 0 THEN
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE mess1.culeid = foll.clueid AND foll.state <> -1) > 0 ELSE 
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE mess1.culeid = foll.clueid AND UNIX_TIMESTAMP(foll.CreateTime) > UNIX_TIMESTAMP(mess1.createTime)) > 0	
                    END AS IsHandle,
                    0 AS TimeoutNoticeCount	
                    FROM tx_noticemessage mess1
                    WHERE  mess1.reciveaentId = ?reciveaentId  AND mess1.mesaagetype not in (1,7,8,9) 
                    UNION ALL
                    SELECT nt.*,
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE nt.culeid = foll.clueid AND foll.state <> -1) > 0 AS IsHandle,
                    (SELECT COUNT(id) FROM tx_noticemessage t2 WHERE t2.culeid = nt.culeid  AND t2.mesaagetype = 1 AND nt.mesaagetype = 1) AS TimeoutNoticeCount
                    FROM tx_noticemessage nt 
                    INNER JOIN (
                    SELECT mess2.culeid,MAX(mess2.id) AS id
                    FROM tx_noticemessage mess2
                    WHERE  mess2.reciveaentId = ?reciveaentId AND mess2.mesaagetype = 1 
                    GROUP BY mess2.culeid) AS  grp 
                    ON nt.id = grp.id) AS t
                    ORDER BY t.createtime  DESC
            LIMIT {0},{1} ", (pageIndex - 1) * pageSize, pageSize);
                List<NoticeMessageDataModel> data = DataContextFactory.GetDataContext().Database.SqlQuery<NoticeMessageDataModel>(listSql, param.ToArray()).ToList();
                var result = new List<AccidentNoticeMessageModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        TimeSpan span = DateTime.Now - item.createTime;
                        var model = new AccidentNoticeMessageModel
                        {
                            Content = item.content,
                            CreateTime = item.createTime == null ? "" : item.createTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CuleId = item.culeid,
                            MesaageType = item.mesaagetype,
                            MessageId = item.id,
                            OperateAgentId = item.operateagentId,
                            ReciveAgentId = item.reciveaentId,
                            TimeDifference = span.TotalSeconds,
                            Title = item.title,
                            CuleState = item.culestate,
                            IsHandle = item.IsHandle,
                            TimeoutNoticeCount = item.TimeoutNoticeCount
                        };
                        result.Add(model);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logError.Info("获取推修消息列表发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new List<AccidentNoticeMessageModel>();
            }
        }

        /// <summary>
        /// V 2.3.0
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentId"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<AccidentNoticeMessageModel> GetMessageListV23(int pageIndex, int pageSize, int agentId, out int totalCount)
        {
            totalCount = 0;
            try
            {
                var sql = @"SELECT SUM((SELECT COUNT(*) FROM tx_noticemessage WHERE  reciveaentId=?reciveaentId AND mesaagetype <> 1 AND mesaagetype <> 7)) + SUM((SELECT COUNT(*) FROM (SELECT culeid FROM tx_noticemessage
                WHERE  reciveaentId = ?reciveaentId AND mesaagetype = 1
                GROUP BY culeid ) t)) AS TotalCount ";
                var param = new MySqlParameter[]
               {
                    new MySqlParameter
                    {
                        ParameterName="?reciveaentId",
                        Value=agentId,
                        MySqlDbType=MySqlDbType.Int32
                    }
               };
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();



                var listSql = string.Format(@"	SELECT * FROM (
                    SELECT mess1.*,
                    CASE WHEN mess1.mesaagetype = 0 THEN
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE mess1.culeid = foll.clueid AND foll.state <> -1) > 0 ELSE 
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE mess1.culeid = foll.clueid AND UNIX_TIMESTAMP(foll.CreateTime) > UNIX_TIMESTAMP(mess1.createTime)) > 0	
                    END AS IsHandle,
                    0 AS TimeoutNoticeCount	
                    FROM tx_noticemessage mess1
                    WHERE  mess1.reciveaentId = ?reciveaentId  AND mess1.mesaagetype <> 1 AND mess1.mesaagetype <> 7 
                    UNION ALL
                    SELECT nt.*,
                    (SELECT COUNT(id) FROM tx_cluefollowuprecord foll WHERE nt.culeid = foll.clueid AND foll.state <> -1) > 0 AS IsHandle,
                    (SELECT COUNT(id) FROM tx_noticemessage t2 WHERE t2.culeid = nt.culeid  AND t2.mesaagetype = 1 AND nt.mesaagetype = 1) AS TimeoutNoticeCount
                    FROM tx_noticemessage nt 
                    INNER JOIN (
                    SELECT mess2.culeid,MAX(mess2.id) AS id
                    FROM tx_noticemessage mess2
                    WHERE  mess2.reciveaentId = ?reciveaentId AND mess2.mesaagetype = 1 
                    GROUP BY mess2.culeid) AS  grp 
                    ON nt.id = grp.id) AS t
                    ORDER BY t.createtime  DESC
            LIMIT {0},{1} ", (pageIndex - 1) * pageSize, pageSize);
                List<NoticeMessageDataModel> data = DataContextFactory.GetDataContext().Database.SqlQuery<NoticeMessageDataModel>(listSql, param.ToArray()).ToList();
                var result = new List<AccidentNoticeMessageModel>();
                var clueData = new List<tx_clues>();
                if (data != null)
                {
                    try
                    {
                        var robbing = data.Where(o => o.mesaagetype == 9).ToList();
                        if (robbing != null && robbing.Count > 0)
                        {
                            var ids = robbing.Select(o => o.culeid).ToList();
                            clueData = DataContextFactory.GetDataContext().tx_clues.Where(o => ids.Contains(o.id)).ToList();
                        }
                    }
                    catch
                    {
                    }
                    foreach (var item in data)
                    {
                        TimeSpan span = DateTime.Now - item.createTime;
                        var model = new AccidentNoticeMessageModel
                        {
                            Content = item.content,
                            CreateTime = item.createTime == null ? "" : item.createTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CuleId = item.culeid,
                            MesaageType = item.mesaagetype,
                            MessageId = item.id,
                            OperateAgentId = item.operateagentId,
                            ReciveAgentId = item.reciveaentId,
                            TimeDifference = span.TotalSeconds,
                            Title = item.title,
                            CuleState = item.culestate,
                            IsHandle = item.IsHandle,
                            TimeoutNoticeCount = item.TimeoutNoticeCount
                        };
                        try
                        {
                            if (clueData != null && clueData.Count > 0)
                            {
                                var clueModel = clueData.Where(o => o.id == item.culeid).FirstOrDefault();
                                if (clueModel != null && clueModel.id > 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(clueModel.licenseno) && clueModel.licenseno.Length >= 4)
                                        model.Content = model.Content.Replace(clueModel.licenseno, clueModel.licenseno.Substring(0, 4) + "***");
                                    if (!string.IsNullOrWhiteSpace(clueModel.CarOwner) && clueModel.CarOwner.Length >= 1)
                                        model.Content = model.Content.Replace(clueModel.CarOwner, clueModel.CarOwner.Substring(0, 1) + "**");
                                }
                            }
                        }
                        catch
                        {
                        }
                        result.Add(model);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logError.Info("获取推修消息列表发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new List<AccidentNoticeMessageModel>();
            }
        }


        /// <summary>
        /// 抢单信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<RobbingModel> GetRobbingMessageList(int agentId)
        {

            var sql = string.Format(@"select tn.culeid,cu.sourcename as 'SourceName',cu.IsMany,cu.IsDrivering,cb.Name AS 'BrandName',
cu.licenseno as 'LicenseNo',cu.CarOwner,cu.OrderNum,cu.source as 'SourceId' from tx_noticemessage tn 
LEFT JOIN tx_clues cu on cu.id = tn.culeid
LEFT JOIN tx_carbrands cb ON cu.ChosedModelId = cb.BrandId
where tn.reciveaentId = {0} AND tn.mesaagetype = 7 AND cu.followupstate IN(21,23)  AND tn.culeid NOT IN (SELECT culeid FROM tx_noticemessage WHERE reciveaentId = {0} AND mesaagetype IN(8,9)) ORDER BY tn.createTime desc ", agentId);
            var data = DataContextFactory.GetDataContext().Database.SqlQuery<RobbingModel>(sql).ToList();
            if (data != null)
            {
                foreach (var item in data)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.LicenseNo) && item.LicenseNo.Length >= 4)
                            item.LicenseNo = item.LicenseNo.Substring(0, 4) + "***";
                        if (!string.IsNullOrWhiteSpace(item.CarOwner) && item.CarOwner.Length >= 1)
                            item.CarOwner = item.CarOwner.Substring(0, 1) + "**";
                    }
                    catch (Exception)
                    {
                    }
                    var many = string.Empty;
                    var drive = string.Empty;
                    many = item.IsMany == 1 ? "多方事故" : "单方事故";
                    drive = item.IsDrivering == 1 ? "可以正常行驶" : "无法正常行驶";
                    item.Tip = many + "," + drive;
                }
            }

            return data;
        }
    }
}
