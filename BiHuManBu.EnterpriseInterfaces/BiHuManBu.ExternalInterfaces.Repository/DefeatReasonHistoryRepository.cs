using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class DefeatReasonHistoryRepository : IDefeatReasonHistoryRepository
    {
        readonly MySqlHelper _mySqlHelper;
        private readonly ILog logInfo;
        public DefeatReasonHistoryRepository()
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
            logInfo = LogManager.GetLogger("INFO");
        }

        public bool AddToDefeatReasonHistory(bx_defeatreasonhistory defeatReasonHistory)
        {
            if (defeatReasonHistory.Deleted)
            {
                DeleteDefeatReasonHistory(defeatReasonHistory.BuId);
            }
            defeatReasonHistory.Deleted = false;
            DataContextFactory.GetDataContext().bx_defeatreasonhistory.Add(defeatReasonHistory);
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }
        public void DeleteDefeatReasonHistory(long buId)
        {

            var defeatreasonhistoryModel = DataContextFactory.GetDataContext().bx_defeatreasonhistory.Where(x => x.BuId == buId && x.Deleted == false).OrderByDescending(x => x.Id).FirstOrDefault();
            if (defeatreasonhistoryModel != null)
            {
                defeatreasonhistoryModel.Deleted = true;
            }

        }
        public int GetDefeatReasonHistoryCount(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, List<int> agentIds)
        {
            List<string> list = new List<string>();
            foreach (var i in agentIds)
            {
                string j = i.ToString();
                list.Add("'" + j + "'");
            }

            StringBuilder sb = new StringBuilder(@"select count(1) 
                                                from (  select  
                                                drh.id as Id,drh.LicenseNo as LicenseNo, a.AgentName as AgentName,
                                                drh.LicenseOwner as LicenseOwner,drh.MoldName as MoldName,  drs.DefeatReason as DefeatReason,
                                                drh.CreateTime as CreateTime,drh.DefeatReasonId,drh.BuId as BuId,cc.CategoryInfo as CustomerCategory ,cc.id as CategoryInfoId
                                                from bx_defeatreasonhistory as drh
                                                inner join bx_defeatreasonsetting as drs on drh.DefeatReasonId=drs.Id
                                                inner join bx_userinfo as ui on drh.buid=ui.id
                                                left join bx_customercategories as cc on ui.CategoryInfoId=cc.id 
                                                left join bx_agent as a on ui.Agent=a.id where drh.deleted=0 and  drh.AgentId in({0}) ) as  temp 
                                                inner join  bx_userinfo as bu  on 
                                                temp.BuId=bu.id  where  bu.agent in ({1})");
            List<MySqlParameter> ps = new List<MySqlParameter>() { };

            //var query = (from defeatreasonhistory in DataContextFactory.GetDataContext().bx_defeatreasonhistory
            //             join defeatreasonsetting in DataContextFactory.GetDataContext().bx_defeatreasonsetting
            //             on defeatreasonhistory.DefeatReasonId equals defeatreasonsetting.Id
            //             where !defeatreasonhistory.Deleted && agentIds.Contains(defeatreasonhistory.AgentId)
            //             select new DefeatReasonDataViewModel
            //             {
            //                 Id = defeatreasonhistory.Id,
            //                 LicenseNo = defeatreasonhistory.LicenseNo,
            //                 AgentName = defeatreasonhistory.AgentName,
            //                 LicenseOwner = defeatreasonhistory.LicenseOwner,
            //                 MoldName = defeatreasonhistory.MoldName,
            //                 DefeatReason = defeatreasonsetting.DefeatReason,
            //                 CreateTime = defeatreasonhistory.CreateTime,
            //                 DefeatReasonId = defeatreasonhistory.DefeatReasonId,
            //                 BuId = defeatreasonhistory.BuId
            //             });
            if (seachDefeatReasonHistoryCondition.CustomerCategoryId != -1)
            {
                sb.Append(" and temp.CategoryInfoId=?CategoryInfoId");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "CategoryInfoId", Value = seachDefeatReasonHistoryCondition.CustomerCategoryId });
            }

            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.LicenseNo))
            {
                sb.Append(" and temp.LicenseNo=?LicenseNo");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "LicenseNo", Value = seachDefeatReasonHistoryCondition.LicenseNo.ToUpper() });
                //query = query.Where(x => x.LicenseNo == seachDefeatReasonHistoryCondition.LicenseNo.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.AgentName))
            {
                sb.Append(" and temp.AgentName=?AgentName");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "AgentName", Value = seachDefeatReasonHistoryCondition.AgentName });
                //query = query.Where(x => x.AgentName == seachDefeatReasonHistoryCondition.AgentName);
            }

            if (seachDefeatReasonHistoryCondition.StartTime.HasValue)
            {
                sb.Append(" and temp.CreateTime>=?StartTime");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StartTime", Value = seachDefeatReasonHistoryCondition.StartTime });
                //query = query.Where(x => x.CreateTime >= seachDefeatReasonHistoryCondition.StartTime);
            }
            if (seachDefeatReasonHistoryCondition.EndTime.HasValue)
            {
                var newEndTime = seachDefeatReasonHistoryCondition.EndTime.Value.AddDays(1);
                sb.Append(" and temp.CreateTime<=?EndTime");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "EndTime", Value = newEndTime });
                //query = query.Where(x => x.CreateTime <= newEndTime);
            }
            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.DefeatReasonContent))
            {
                sb.Append(" and temp.DefeatReason=?DefeatReason");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "DefeatReason", Value = seachDefeatReasonHistoryCondition.DefeatReasonContent });
                //query = query.Where(x => x.DefeatReason == seachDefeatReasonHistoryCondition.DefeatReasonContent);
            }
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(string.Format(sb.ToString(), string.Join(",", agentIds), string.Join(",", list)), ps.ToArray()).FirstOrDefault();
        }
        public List<DefeatReasonDataViewModel> GetDefeatReasonHistory(SeachDefeatReasonHistoryCondition seachDefeatReasonHistoryCondition, List<int> agentIds, out int totalCount)
        {
            List<string> list = new List<string>();
            foreach (var i in agentIds)
            {
                string j = i.ToString();
                list.Add("'" + j + "'");
            }

            StringBuilder sb = new StringBuilder(@"select 
                                                temp.Id,temp.LicenseNo, temp.AgentName,
                                                temp.LicenseOwner,temp.MoldName,  temp.DefeatReason,
                                                temp.CreateTime,temp.DefeatReasonId,temp.BuId,temp.CustomerCategory
                                                from (  select  
                                                drh.id as Id,drh.LicenseNo as LicenseNo, a.AgentName as AgentName,
                                                drh.LicenseOwner as LicenseOwner,drh.MoldName as MoldName,  drs.DefeatReason as DefeatReason,
                                                drh.CreateTime as CreateTime,drh.DefeatReasonId,drh.BuId as BuId,cc.CategoryInfo as CustomerCategory ,cc.id as CategoryInfoId
                                                from bx_defeatreasonhistory as drh
                                                inner join bx_defeatreasonsetting as drs on drh.DefeatReasonId=drs.Id
                                                inner join bx_userinfo as ui on drh.buid=ui.id
                                                left join bx_customercategories as cc on ui.CategoryInfoId=cc.id 
                                                left join bx_agent as a on ui.Agent=a.id where drh.deleted=0 and  drh.AgentId in({0}) ) as  temp 
                                                inner join  bx_userinfo as bu  on 
                                                temp.BuId=bu.id  where  bu.agent in ({1}) ");
            List<MySqlParameter> bs = new List<MySqlParameter>() { };

            //var query = (from defeatreasonhistory in DataContextFactory.GetDataContext().bx_defeatreasonhistory
            //             join defeatreasonsetting in DataContextFactory.GetDataContext().bx_defeatreasonsetting
            //             on defeatreasonhistory.DefeatReasonId equals defeatreasonsetting.Id
            //             where !defeatreasonhistory.Deleted && agentIds.Contains(defeatreasonhistory.AgentId)
            //             select new DefeatReasonDataViewModel
            //             {
            //                 Id = defeatreasonhistory.Id,
            //                 LicenseNo = defeatreasonhistory.LicenseNo,
            //                 AgentName = defeatreasonhistory.AgentName,
            //                 LicenseOwner = defeatreasonhistory.LicenseOwner,
            //                 MoldName = defeatreasonhistory.MoldName,
            //                 DefeatReason = defeatreasonsetting.DefeatReason,
            //                 CreateTime = defeatreasonhistory.CreateTime,
            //                 DefeatReasonId = defeatreasonhistory.DefeatReasonId,
            //                 BuId = defeatreasonhistory.BuId
            //             });
            if (seachDefeatReasonHistoryCondition.CustomerCategoryId != -1)
            {
                sb.Append(" and temp.CategoryInfoId=?CategoryInfoId");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "CategoryInfoId", Value = seachDefeatReasonHistoryCondition.CustomerCategoryId });
            }

            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.LicenseNo))
            {
                sb.Append(" and temp.LicenseNo=?LicenseNo");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "LicenseNo", Value = seachDefeatReasonHistoryCondition.LicenseNo.ToUpper() });
                //query = query.Where(x => x.LicenseNo == seachDefeatReasonHistoryCondition.LicenseNo.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.AgentName))
            {
                sb.Append(" and temp.AgentName=?AgentName");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "AgentName", Value = seachDefeatReasonHistoryCondition.AgentName });
                //query = query.Where(x => x.AgentName == seachDefeatReasonHistoryCondition.AgentName);
            }

            if (seachDefeatReasonHistoryCondition.StartTime.HasValue)
            {
                sb.Append(" and temp.CreateTime>=?StartTime");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StartTime", Value = seachDefeatReasonHistoryCondition.StartTime });
                //query = query.Where(x => x.CreateTime >= seachDefeatReasonHistoryCondition.StartTime);
            }
            if (seachDefeatReasonHistoryCondition.EndTime.HasValue)
            {
                var newEndTime = seachDefeatReasonHistoryCondition.EndTime.Value.AddDays(1);
                sb.Append(" and temp.CreateTime<=?EndTime");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "EndTime", Value = newEndTime });
                //query = query.Where(x => x.CreateTime <= newEndTime);
            }
            if (!string.IsNullOrWhiteSpace(seachDefeatReasonHistoryCondition.DefeatReasonContent))
            {
                sb.Append(" and temp.DefeatReason=?DefeatReason");
                bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "DefeatReason", Value = seachDefeatReasonHistoryCondition.DefeatReasonContent });
                //query = query.Where(x => x.DefeatReason == seachDefeatReasonHistoryCondition.DefeatReasonContent);
            }
            int elsepage = 0;
            if (seachDefeatReasonHistoryCondition.CurPage > seachDefeatReasonHistoryCondition.ShowPageNum / 2 + 1)
            {
                elsepage = seachDefeatReasonHistoryCondition.ShowPageNum / 2 - 1;
            }
            else
            {
                elsepage = seachDefeatReasonHistoryCondition.ShowPageNum - seachDefeatReasonHistoryCondition.CurPage;
            }
            logInfo.Info("获取数量：" + string.Format("SELECT COUNT(1) FROM ( " + sb.ToString() + " LIMIT {2},{3}) AS aa", string.Join(",", agentIds), string.Join(",", list), (seachDefeatReasonHistoryCondition.CurPage * seachDefeatReasonHistoryCondition.PageSize).ToString(), (seachDefeatReasonHistoryCondition.PageSize * elsepage).ToString()));
            totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(string.Format("SELECT COUNT(1) FROM ( " + sb.ToString() + " LIMIT {2},{3}) AS aa", string.Join(",", agentIds), string.Join(",", list), (seachDefeatReasonHistoryCondition.CurPage * seachDefeatReasonHistoryCondition.PageSize).ToString(), (seachDefeatReasonHistoryCondition.PageSize * elsepage).ToString()), bs.ToArray()).FirstOrDefault();
            //totalCount = query.Count();
            sb.Append(" order by Id desc  limit ?pageIndex,?pageSize");
            bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageIndex", Value = (seachDefeatReasonHistoryCondition.CurPage - 1) * seachDefeatReasonHistoryCondition.PageSize });
            bs.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageSize", Value = seachDefeatReasonHistoryCondition.PageSize });

            logInfo.Info("获取战败记录：" + string.Format(string.Format(sb.ToString(), string.Join(",", list), string.Join(",", list))));
            return DataContextFactory.GetDataContext().Database.SqlQuery<DefeatReasonDataViewModel>(string.Format(sb.ToString(), string.Join(",", agentIds), string.Join(",", list)), bs.ToArray()).ToList();
            //return query.OrderByDescending(x => x.Id).Skip((seachDefeatReasonHistoryCondition.CurPage - 1) * seachDefeatReasonHistoryCondition.PageSize).Take(seachDefeatReasonHistoryCondition.PageSize).ToList();
        }

        public List<DefeatReasonMobileDetails> GetDefeatAnalyticsDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName, out int totalCount)
        {
            var countSql = string.Format(@"select count(distinct buid) from bx_defeatreasonhistory d LEFT JOIN bx_agent t on d.AgentId=t.Id
                                    LEFT JOIN bx_defeatreasonsetting s on d.DefeatReasonId=s.Id {5} {4}
                                    where d.CreateTime>='{1}' and d.CreateTime<'{2}' and d.Deleted=0 and t.TopAgentId={0} {6} {3}", topAgentId, startTime, endTime,
                                    string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'",
                                    string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on u.CategoryInfoId = c.Id",
                                    isViewAllData == 0 ? "LEFT JOIN bx_userinfo u on d.BuId=u.Id" : "",
                                    isViewAllData == 0 ? "and u.LastYearSource=2" : "");

            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, countSql, null));

            var sql = string.Format(@"select t.LicenseNo,t.DefeatReason from (select d.LicenseNo,s.DefeatReason,d.BuId from bx_defeatreasonhistory d LEFT JOIN bx_agent t on d.AgentId=t.Id
                                    LEFT JOIN bx_defeatreasonsetting s on d.DefeatReasonId=s.Id {7} {6}
                                    where d.CreateTime>='{1}' and d.CreateTime<'{2}' and d.Deleted=0 and t.TopAgentId={0} {8} {5} ORDER BY d.CreateTime DESC) t group by BuId limit {3},{4}",
                                    topAgentId, startTime, endTime, (pageIndex - 1) * pageSize, pageSize,
                                    string.IsNullOrEmpty(categoryName) ? "" : " and c.CategoryInfo='在修不在保'",
                                    string.IsNullOrEmpty(categoryName) ? "" : " LEFT JOIN bx_customercategories c on u.CategoryInfoId = c.Id",
                                    isViewAllData == 0 ? "LEFT JOIN bx_userinfo u on d.BuId=u.Id" : "",
                                    isViewAllData == 0 ? "and u.LastYearSource=2" : "");
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<DefeatReasonMobileDetails>().ToList();
            return result;
        }
    }
}
