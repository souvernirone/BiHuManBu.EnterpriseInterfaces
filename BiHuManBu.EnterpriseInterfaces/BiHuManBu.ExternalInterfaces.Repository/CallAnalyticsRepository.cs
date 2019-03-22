using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Data;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CallAnalyticsRepository : ICallAnalyticsRepository
    {
        readonly MySqlHelper _mySqlHelper;
        readonly IAgentRepository _agentRepository;
        public CallAnalyticsRepository(IAgentRepository _agentRepository)
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
            this._agentRepository = _agentRepository;
        }

        public List<RecordListViewModel> GetRecordList(SearchtRecordListWhereViewModel searchtRecordListWhereViewModel, out int totalCount,List<string> listSonAgent)
        {
            //var agentIds = _agentRepository.GetSonsList(searchtRecordListWhereViewModel.AgentId);

            List<MySqlParameter> ps = new List<MySqlParameter>();
            StringBuilder getRecordListSql = new StringBuilder("SELECT * FROM(select Id,LicenseNo,CustomerName,Mobile, AnswerState ,CallStartTime, CallEndTime,CallDuration,AgentName,AgentId,RecordFileKey,RecordFileUploadStatus,CreateTime from bihu_analytics.record_history {0}");
            StringBuilder getRecordListCountSql = new StringBuilder("select count(1) from bihu_analytics.record_history {0}");
            StringBuilder whereSql = new StringBuilder(string.Format(" where agentId in({0}) and TIMESTAMPDIFF(MINUTE,CallEndTime,now())>=10 ", string.Join(",", listSonAgent)));
            if (!string.IsNullOrWhiteSpace(searchtRecordListWhereViewModel.AgentName))
            {
                whereSql.Append("  and AgentName=?AgentName ");

                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "AgentName", Value = searchtRecordListWhereViewModel.AgentName });
            }
            if (searchtRecordListWhereViewModel.AnswerState != -1)
            {
                whereSql.Append("  and AnswerState=?AnswerState ");

                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "AnswerState", Value = searchtRecordListWhereViewModel.AnswerState });
            }
            if (searchtRecordListWhereViewModel.CallStartTime.HasValue)
            {
                whereSql.Append("  and CallStartTime>=?CallStartTime ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "CallStartTime", Value = searchtRecordListWhereViewModel.CallStartTime.Value });
            }
            if (searchtRecordListWhereViewModel.CallEndTime.HasValue)
            {
                whereSql.Append("  and CallStartTime<=?CallEndTime ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "CallEndTime", Value = searchtRecordListWhereViewModel.CallEndTime });
            }
            if (!string.IsNullOrWhiteSpace(searchtRecordListWhereViewModel.CustomerName))
            {
                whereSql.Append("  and CustomerName=?CustomerName ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "CustomerName", Value = searchtRecordListWhereViewModel.CustomerName });
            }
            if (!string.IsNullOrWhiteSpace(searchtRecordListWhereViewModel.LicenseNo))
            {
                whereSql.Append("  and LicenseNo like ?LicenseNo ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "LicenseNo", Value = searchtRecordListWhereViewModel.LicenseNo.ToUpper() + "%" });
            }
            if (searchtRecordListWhereViewModel.MinCallDuration != -1)
            {
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "MinCallDuration", Value = searchtRecordListWhereViewModel.MinCallDuration });
                if (searchtRecordListWhereViewModel.MaxCallDuration != -1)
                {
                    whereSql.Append("  and CallDuration>=?MinCallDuration and CallDuration<=?MaxCallDuration ");
                    ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "MaxCallDuration", Value = searchtRecordListWhereViewModel.MaxCallDuration });
                }
                else
                {
                    whereSql.Append("  and CallDuration>=?MinCallDuration  ");
                }
            }

            if (!string.IsNullOrWhiteSpace(searchtRecordListWhereViewModel.Mobile))
            {
                whereSql.Append("  and Mobile=?Mobile ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "Mobile", Value = searchtRecordListWhereViewModel.Mobile });
            }
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(string.Format(getRecordListCountSql.ToString(), whereSql.ToString()), ps.ToArray()));
            whereSql.Append(") t order by CreateTime desc");
            if (searchtRecordListWhereViewModel.IsPaging)
            {
                whereSql.Append(" limit ?pageIndex,?pageSize");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageIndex", Value = (searchtRecordListWhereViewModel.PageIndex - 1) * searchtRecordListWhereViewModel.PageSize });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageSize", Value = searchtRecordListWhereViewModel.PageSize });
            }
            return _mySqlHelper.ExecuteDataTable(string.Format(getRecordListSql.ToString(), whereSql.ToString()), ps.ToArray()).ToList<RecordListViewModel>().ToList();


        }

        public dynamic SaveRecord(SaveRecordViewModel saveRecordViewModel)
        {
            dynamic result = new System.Dynamic.ExpandoObject();
            //var getRecordSql = "select count(1) from record_history where RecordFileKey=?RecordFileKey";
            if (GetRecordHistory(saveRecordViewModel.RecordFileKey) != null)
            {
                result.IsSuccess = false;
                result.Message = "数据重复";
            }
            else
            {
                var saveRecordSql = @"INSERT INTO bihu_analytics.record_history (AgentId,AgentName,ParentAgentId,TopAgentId,Licenseno,CustomerName,Mobile,AnswerState,CallDuration,CallStartTime,CallEndTime,RecordFileKey,CreateTime,UpdateTime,BuId)VALUES(?AgentId,?AgentName,?ParentAgentId,?TopAgentId,?Licenseno,?CustomerName,?Mobile,?AnswerState,?CallDuration,?CallStartTime,?CallEndTime,?RecordFileKey,?CreateTime,?UpdateTime,?BuId)";
                List<MySqlParameter> ps = new List<MySqlParameter>() {
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int32,ParameterName= "AgentId" , Value= saveRecordViewModel .AgentId},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "AgentName" , Value= saveRecordViewModel .AgentName},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int32,ParameterName= "ParentAgentId" , Value= saveRecordViewModel .ParentAgentId},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int32,ParameterName= "TopAgentId" , Value= saveRecordViewModel .TopAgentId},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "Licenseno" , Value= saveRecordViewModel .Licenseno.ToUpper()},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "CustomerName" , Value= saveRecordViewModel .CustomerName},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "Mobile" , Value= saveRecordViewModel .Mobile},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int32,ParameterName= "AnswerState" , Value= saveRecordViewModel .AnswerState},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int32,ParameterName= "CallDuration" , Value= saveRecordViewModel .CallDuration},
                new MySqlParameter {  MySqlDbType=MySqlDbType.DateTime,ParameterName= "CallStartTime" , Value= saveRecordViewModel .CallStartTime},
                new MySqlParameter {  MySqlDbType=MySqlDbType.DateTime,ParameterName= "CallEndTime" , Value= saveRecordViewModel .CallEndTime},
                new MySqlParameter {  MySqlDbType=MySqlDbType.VarChar,ParameterName= "RecordFileKey" , Value= saveRecordViewModel .RecordFileKey},
                new MySqlParameter {  MySqlDbType=MySqlDbType.DateTime,ParameterName= "CreateTime" , Value= saveRecordViewModel .CreateTime},
                new MySqlParameter {  MySqlDbType=MySqlDbType.DateTime,ParameterName= "UpdateTime" , Value= saveRecordViewModel .UpdateTime},
                new MySqlParameter {  MySqlDbType=MySqlDbType.Int64,ParameterName= "BuId" , Value= saveRecordViewModel .BuId},
            };
                result.IsSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, saveRecordSql, ps.ToArray()) > 0;
                if (result.IsSuccess)
                {
                    result.Message = "添加成功";
                }
                else
                {
                    result.Message = "添加失败";
                }
            }
            return result;
        }
        public dynamic UpdateRecord(UpdateRecordViewModel updateRecordViewModel)
        {
            dynamic result = new System.Dynamic.ExpandoObject();
            result.IsSuccess = false;
            string updateRecordFileUploadStatusAndFailLogSql = string.Empty;
            if (GetRecordHistory(updateRecordViewModel.RecordFileKey) != null)
            {

                List<MySqlParameter> ps = new List<MySqlParameter>();
                updateRecordFileUploadStatusAndFailLogSql = @"update bihu_analytics.record_history set RecordFileUploadStatus=?RecordFileUploadStatus,RecordFileFailReason=?RecordFileFailReason, updatetime=now()  where RecordFileKey=?RecordFileKey";
                ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "RecordFileUploadStatus", Value = updateRecordViewModel.RecordFileUploadStatus ? 1 : 0 }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RecordFileKey", Value = updateRecordViewModel.RecordFileKey }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RecordFileFailReason", Value = updateRecordViewModel.RecordFileFailReason } };
                result.IsSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateRecordFileUploadStatusAndFailLogSql, ps.ToArray()) > 0;
                result.Message = result.IsSuccess ? "更新成功" : "更新失败";
            }
            else {
                result.Message = "无此条通话记录";
            }
       
          
            return result;
        }
        public bool UpdateRecordForUser(string recordFileKey, int answerState)
        {
            var updateRecordForUser = @"update bihu_analytics.record_history set AnswerState=?AnswerState,UpdateTime=Now()   where RecordFileKey=?RecordFileKey";
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RecordFileKey", Value = recordFileKey }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "AnswerState", Value = answerState } };
            return Convert.ToInt32(_mySqlHelper.ExecuteNonQuery(CommandType.Text, updateRecordForUser, ps.ToArray())) > 0;
        }

        public OverviewOfDataViewModel GetOverviewOfData(int agentId, int topAgentId,int effectiveCallDuration, DateTime startTime, DateTime endTime,List<string> listSonAgent)
        {
            //var agentIds = _agentRepository.GetSonsList(agentId);
    
            var getOverviewOfDataSql = string.Format("select count(id) as CallTimes,sum(case when AnswerState=1 then 1 else 0 end) as AnswerCallTimes,sum(case when AnswerState=1 then CallDuration else 0 end ) as CallTotalTime,sum(case when CallDuration>=?callDuration And AnswerState=1  then 1 else 0  end) as EffectiveCalls,sum(case when CallDuration>=?callDuration And AnswerState=1  then CallDuration else 0  end) as EffectiveDuration from bihu_analytics.record_history where  agentId in ({0}) and createtime>?startTime and createtime<?endTime", string.Join(",", listSonAgent));
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "startTime", Value = startTime }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "endTime", Value = endTime }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "callDuration", Value = effectiveCallDuration } };
            return _mySqlHelper.ExecuteDataTable(getOverviewOfDataSql, ps.ToArray()).ToList<OverviewOfDataViewModel>().FirstOrDefault();
        }

        public List<TimePassAnalysisViewModel> GetTimePassAnalysis(int agentId, DateTime startTime, DateTime endTime, List<string> listSonAgent)
        {
            //var agentIds = _agentRepository.GetSonsList(agentId);
            var getTimePassAnalysisSql = string.Format(@"select count(id) as CallDurationTimes, CallTimeType  from  (select id,case when CallDuration between 0 and 60  then 0
when CallDuration between 60 and 120  then  1
when CallDuration between 120 and 300 then  2
when CallDuration >300 then 3 end as CallTimeType
                from bihu_analytics.record_history where  agentId in ({0}) and createtime>?startTime and createtime<?endTime) as t group by t.CallTimeType", string.Join(",", listSonAgent));
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "startTime", Value = startTime }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "endTime", Value = endTime } };
            return _mySqlHelper.ExecuteDataTable(getTimePassAnalysisSql, ps.ToArray()).ToList<TimePassAnalysisViewModel>().ToList();
        }

        public List<SalesmanStatisticsViewModel> GetSalesmanStatistics(int agentId,int topAgentId,int effectiveCallDuration, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, bool isExport, out int totalCount, out List<string> callDataInDates, List<string> listSonAgent)
        {
            StringBuilder sb = new StringBuilder(string.Format("select AgentId,AgentName,count(id) as AgentCallTimes,sum(case when AnswerState=1 then 1 else 0 end) as AnswerCallTimes,sum(case when AnswerState=1 then CallDuration else 0 end) as  AgentCallTime,sum(case when CallDuration between 0 and 60 then 1 else 0 end) as Zero_One_CallTimes, sum(case when CallDuration between 60 and 120 then 1 else 0 end) as One_Two_CallTimes,sum(case when CallDuration between 120 and 300 then 1 else 0 end) as Two_Five_CallTimes,sum(case when CallDuration>300 then 1 else 0 end) as TanFive_CallTimes,sum(case when CallDuration>=?callDuration And AnswerState=1 then 1 else 0  end) as EffectiveCalls,sum(case when CallDuration>=?callDuration And AnswerState=1 then CallDuration else 0  end) as EffectiveDuration,null as CreateTime from bihu_analytics.record_history where agentId in ({0}) and createtime>?startTime and createtime<?endTime group by agentId ", string.Join(",", listSonAgent)));
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "agentId", Value = string.Join(",", listSonAgent) }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "startTime", Value = startTime }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "endTime", Value = endTime },new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "callDuration", Value = effectiveCallDuration } };
            totalCount = 0;
            callDataInDates = new List<string>();
            if (!isExport)
            {
                totalCount = _mySqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray()).ToList<SalesmanStatisticsViewModel>().Count;
                sb.Append(" order by createtime desc limit ?pageIndex,?pageSize");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageIndex", Value = (pageIndex - 1) * pageSize });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageSize", Value = pageSize });
            }
            if (isExport && DateTime.Compare(startTime.AddDays(1), endTime) != 0)
            {
                //通话统计数据日期集合
                callDataInDates = _mySqlHelper.ExecuteDataTable(string.Format("select DATE_FORMAT(createtime,'%Y-%m-%d') as CreateTime from bihu_analytics.record_history where agentId in ({0}) and createtime>?startTime and createtime<?endTime group by DATE_FORMAT(createtime,'%Y-%m-%d') ", string.Join(",", listSonAgent)), ps.ToArray()).ToList<string>().ToList();
                //通话统计数据日期数量
                totalCount = callDataInDates.Count;
                sb.Append(string.Format(" union all select AgentId,AgentName,count(id) as AgentCallTimes,sum(case when AnswerState=1 then 1 else 0 end) as AnswerCallTimes,sum(case when AnswerState=1 then CallDuration else 0 end) as  AgentCallTime,sum(case when CallDuration between 0 and 60 then 1 else 0 end) as Zero_One_CallTimes, sum(case when CallDuration between 60 and 120 then 1 else 0 end) as One_Two_CallTimes,sum(case when CallDuration between 120 and 300 then 1 else 0 end) as Two_Five_CallTimes,sum(case when CallDuration>300 then 1 else 0 end) as TanFive_CallTimes,sum(case when CallDuration>=?callDuration And AnswerState=1 then 1 else 0  end) as EffectiveCalls,sum(case when CallDuration>=?callDuration And AnswerState=1 then CallDuration else 0  end) as EffectiveDuration,DATE_FORMAT(createtime,'%Y-%m-%d') CreateTime from bihu_analytics.record_history where agentId in ({0}) and createtime>?startTime and createtime<?endTime group by agentId,DATE_FORMAT(createtime,'%Y-%m-%d')", string.Join(",", listSonAgent)));
            }
            return _mySqlHelper.ExecuteDataTable(sb.ToString(), ps.ToArray()).ToList<SalesmanStatisticsViewModel>().ToList();
        }
        private RecordListViewModel GetRecordHistory(string recordFileKey)
        {
            var getRecordSql = "select RecordFileUploadStatus from bihu_analytics.record_history where RecordFileKey=?RecordFileKey";
            return _mySqlHelper.ExecuteDataTable(getRecordSql, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RecordFileKey", Value = recordFileKey }).ToList<RecordListViewModel>().FirstOrDefault();
        }
    }
}
