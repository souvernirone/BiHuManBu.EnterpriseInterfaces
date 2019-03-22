using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using System.Data;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SmsBulkSendManageRepository : ISmsBulkSendManageRepository
    {
        readonly IAgentRepository _agentRepository;
        readonly MySqlHelper _mySqlHelper;
        public SmsBulkSendManageRepository(IAgentRepository _agentRepository)
        {
            this._agentRepository = _agentRepository;
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zb"].ConnectionString);
        }
        public List<TargetUsersMobileResult> GetTargetUsersMobile(GetTargetUsersRequest getTargetUsersRequest, string agentIdsStr, out int totalCount)
        {

            List<MySqlParameter> ps = new List<MySqlParameter>();
            var getResultSql = new StringBuilder(@"SELECT {0} FROM  bx_userinfo AS  u
LEFT JOIN bx_userinfo_renewal_index AS urix
ON u.Id=urix.b_uid
LEFT JOIN bx_car_renewal AS cr
ON urix.car_renewal_id=cr.Id
LEFT JOIN bx_userinfo_renewal_info AS  urio
ON u.Id=urio.b_uid
LEFT JOIN bx_batchrenewal_item AS bi
ON u.Id=bi.BUId
where u.Agent IN({1}) AND  ((urio.client_mobile IS NOT NULL and  urio.client_mobile!=''   AND urio.client_mobile!='(NULL)') OR (urio.client_mobile_other   IS   NOT NULL AND  urio.client_mobile_other!='' And     urio.client_mobile_other!='(NULL)'))
");
            if (getTargetUsersRequest.BizEndDate_Start.HasValue)
            {
                getResultSql.Append("  and IF(bi.IsNew=1 AND bi.BizEndDate>cr.LastBizEndDate,bi.BizEndDate,cr.LastBizEndDate) BETWEEN ?BizEndDate_Start And ?BizEndDate_End ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDate_Start", Value = getTargetUsersRequest.BizEndDate_Start.Value });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "BizEndDate_End", Value = getTargetUsersRequest.BizEndDate_End.Value });

            }
            if (getTargetUsersRequest.ForceEndDate_Start.HasValue)
            {
                getResultSql.Append("  and IF(bi.IsNew=1 AND bi.BizEndDate>cr.LastBizEndDate,cr.LastForceEndDate,cr.LastForceEndDate) BETWEEN ?ForceEndDate_Start And ?ForceEndDate_End ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "ForceEndDate_Start", Value = getTargetUsersRequest.ForceEndDate_Start.Value });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "ForceEndDate_End", Value = getTargetUsersRequest.ForceEndDate_End.Value });
            }
            if (!string.IsNullOrWhiteSpace(getTargetUsersRequest.RegisterDate_Start))
            {
                getResultSql.Append("  and  u.RegisterDate BETWEEN ?RegisterDate_Start And ?RegisterDate_End");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RegisterDate_Start", Value = getTargetUsersRequest.RegisterDate_Start });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "RegisterDate_End", Value = getTargetUsersRequest.RegisterDate_End });
            }
            var obj = _mySqlHelper.ExecuteScalar(string.Format(getResultSql.ToString(), @" sum( case when  (urio.client_mobile IS NOT NULL and  urio.client_mobile!=''   AND urio.client_mobile!='(NULL)'  and urio.client_mobile_other   IS   NOT NULL AND  urio.client_mobile_other!='' And     urio.client_mobile_other!='(NULL)')  then 2 
             when  ((urio.client_mobile IS  NULL or  urio.client_mobile=''   or urio.client_mobile!='(NULL)')  and (urio.client_mobile_other   IS    NULL or  urio.client_mobile_other='' or     urio.client_mobile_other='(NULL)' )) then 1 else 0 end ) as totalCount ", agentIdsStr), ps.ToArray());
            if (DBNull.Value != obj)
            {
                totalCount = Convert.ToInt32(obj);
            }
            else
            {
                totalCount = 0;
            }

            var result = _mySqlHelper.ExecuteDataTable(string.Format(getResultSql.ToString(), @"  urio.client_mobile as Mobile1,urio.client_mobile_other as Mobile2 ", agentIdsStr), ps.ToArray()).ToList<TargetUsersMobileResult>().ToList();
            return result;





        }
        public int AddSmsBulkSendRecord(dynamic addSmsBulkSendRecordRequest)
        {
            string addToSmsBatchHistorySql = string.Empty;

            addToSmsBatchHistorySql = @"INSERT INTO bx_sms_batch_history(CreateTime,sendtime,Content,CustomerCount,Status,SendedCount,WaitToSendCount,FailedCount,AgentId) VALUES(NOW(),?sendtime,?Content,?CustomerCount,?Status,?SendedCount,?WaitToSendCount,?FailedCount,?AgentId);select @@IDENTITY";
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "sendtime", Value = addSmsBulkSendRecordRequest.SendTime }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "Content", Value = addSmsBulkSendRecordRequest.SmsContent }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "CustomerCount", Value = addSmsBulkSendRecordRequest.CustomerCount }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "Status", Value = addSmsBulkSendRecordRequest.Status }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "SendedCount", Value = addSmsBulkSendRecordRequest.SendedCount }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "WaitToSendCount", Value = addSmsBulkSendRecordRequest.WaitToSendCount }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "FailedCount", Value = addSmsBulkSendRecordRequest.FailedCount }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "AgentId", Value = addSmsBulkSendRecordRequest.AgentId } };

            var batchId = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, addToSmsBatchHistorySql, ps.ToArray()));
            StringBuilder sb = new StringBuilder("INSERT INTO bx_sms_account_content(agent_id,sent_mobile,content,create_time,agent_name,sent_type,business_type,SendStatus,BatchId,license_no)VALUES ");
            foreach (var mobile in addSmsBulkSendRecordRequest.MobileList)
            {
                sb.Append(string.Format(" ({0},'{1}','{2}','{3}','{4}',0,7,{5},{6},''), ", addSmsBulkSendRecordRequest.AgentId, mobile, addSmsBulkSendRecordRequest.SmsContent, addSmsBulkSendRecordRequest.SendTime , addSmsBulkSendRecordRequest.AgentName, addSmsBulkSendRecordRequest.Status, batchId));
            }

            _mySqlHelper.ExecuteNonQuery(CommandType.Text, sb.ToString().Substring(0, sb.ToString().Length - 2), null);
            return batchId;
        }

        public List<SmsBulkSendRecordViewModel> GetSmsBulkSendRecord(GetSmsBulkSendRecordRequest getSmsBulkSendRecordRequest, List<int> agentIds,out int totalCount)
        {
           
            StringBuilder getSmsBulkSendRecordSql = new StringBuilder(@"SELECT {0} FROM bx_sms_batch_history WHERE AgentId IN({1}) AND IsDelete=0 ");
            List<MySqlParameter> ps = new List<MySqlParameter>();
            if (getSmsBulkSendRecordRequest.CreateStartTime.HasValue)
            {
                getSmsBulkSendRecordSql.Append(" And  CreateTime >=?CreateStartTime And CreateTime<=?CreateEndTime ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "CreateStartTime", Value = getSmsBulkSendRecordRequest.CreateStartTime.Value });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "CreateEndTime", Value = getSmsBulkSendRecordRequest.CreateEndTime.Value });

            }
            if (getSmsBulkSendRecordRequest.SendStartTime.HasValue)
            {
                getSmsBulkSendRecordSql.Append(" And sendtime >=?SendStartTime And sendtime<=?SendEndTime ");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "SendStartTime", Value = getSmsBulkSendRecordRequest.SendStartTime.Value });
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "SendEndTime", Value = getSmsBulkSendRecordRequest.SendEndTime.Value });
            }
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, string.Format(getSmsBulkSendRecordSql.ToString(), " count(1) ", string.Join(",", agentIds)), ps.ToArray()));
            if (getSmsBulkSendRecordRequest.IsDesc == -1)
            {
                getSmsBulkSendRecordSql.Append(string.Format(" order by CreateTime desc  limit {0},{1}", (getSmsBulkSendRecordRequest.PageIndex - 1) * getSmsBulkSendRecordRequest.PageSize, getSmsBulkSendRecordRequest.PageSize));
            }
            else if (getSmsBulkSendRecordRequest.IsDesc == 1)
            {
                getSmsBulkSendRecordSql.Append(string.Format(" order by sendtime  limit {0},{1}", (getSmsBulkSendRecordRequest.PageIndex - 1) * getSmsBulkSendRecordRequest.PageSize, getSmsBulkSendRecordRequest.PageSize));
            }
            else
            {
                getSmsBulkSendRecordSql.Append(string.Format(" order by sendtime desc limit {0},{1}", (getSmsBulkSendRecordRequest.PageIndex - 1) * getSmsBulkSendRecordRequest.PageSize, getSmsBulkSendRecordRequest.PageSize));
            }
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, string.Format(getSmsBulkSendRecordSql.ToString(), " Id, date_format(CreateTime,'%Y-%m-%d %H:%i') as CreateTime , Content as SmsContent, CustomerCount as CustomerTotalCount,Status, SendedCount,WaitToSendCount,FailedCount,date_format(sendtime,'%Y-%m-%d %H:%i') as SendTime ", string.Join(",", agentIds)), ps.ToArray()).ToList<SmsBulkSendRecordViewModel>().ToList();
        }
        public bool DeleteSmsBulkSendRecord(int id)
        {
            var deleteSmsBulkSendRecordSql = string.Format(@"Update bx_sms_batch_history set IsDelete=1 WHERE id={0})
", id);
            var deleteSmsAccountContentSQL = string.Format(@"update bx_sms_account_content set isDelete=1 where id={0} ", id);
            _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSmsAccountContentSQL, null);
            return _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSmsBulkSendRecordSql, null) > 0;
        }
        public bool UpdateSmsBulkSendRecord(int id, int agentId,  DateTime sendTime, int status, List<string> mobileList, string smsContent, string agentName)
        {

            var updateSendTimeSql = @"update bx_sms_batch_history  set sendtime =?sendtime,status=?status,CustomerCount=?CustomerCount,SendedCount=?SendedCount,WaitToSendCount=?WaitToSendCount,Content=?Content where Id=?Id";
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "sendtime", Value = sendTime }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "Id", Value = id }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "status", Value = status }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "CustomerCount", Value = mobileList.Count }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "SendedCount", Value = status == 1 ? mobileList.Count : 0 }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "WaitToSendCount", Value = status == 1 ? 0 : mobileList.Count }, new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "Content", Value = smsContent } };
            var isSuccess = Convert.ToInt32(_mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSendTimeSql, ps.ToArray())) > 0;
            UpdateSmsAccountContent(id, mobileList, smsContent, status, agentId, agentName,sendTime);
            return isSuccess;
        }
        public bool CancelSend(int id)
        {
            var cancelSendSql = CreateUpdateStatusSql();

            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "Id", Value = id }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "status", Value = 2 } };
            UpdateAccountContent(id, 2);
            return Convert.ToInt32(_mySqlHelper.ExecuteNonQuery(CommandType.Text, cancelSendSql, ps.ToArray())) > 0;
        }
        public SendMobilesAndContentResult GetSendMobilesAndContentById(int id)
        {
            var getAccountContentSql = string.Format(@"select sent_mobile as Mobile,content as SmsContent from bx_sms_account_content where batchid={0} and isdelete=0 ", id);
            var tempResult = _mySqlHelper.ExecuteDataTable(CommandType.Text, getAccountContentSql, null).ToList<SendMobilesAndContentTempResult>().ToList();
            return new SendMobilesAndContentResult { SmsContent = tempResult.First().SmsContent, MobileList = tempResult.Select(x => x.Mobile).ToList() };
        }
        public bool UpdateStatus(int id)
        {
            var updateStatusSql = CreateUpdateStatusSql();
            List<MySqlParameter> ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "Id", Value = id }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "status", Value = 1 } };
            UpdateAccountContent(id, 1);

            return Convert.ToInt32(_mySqlHelper.ExecuteNonQuery(CommandType.Text, updateStatusSql, ps.ToArray())) > 0;
        }
        public BulkSendResult GetBulkSendRecordById(int id)
        {
            var getBulkSendAgentId = string.Format("select AgentId,CustomerCount,Content,Status from bx_sms_batch_history where id in({0})", id);
            return _mySqlHelper.ExecuteDataRow(CommandType.Text, getBulkSendAgentId, null).ToT<BulkSendResult>();
        }
        public bool UpdateSmsAccountUseCount(int agentId, int calculatedCount, int operationType)
        {
            string updateSmsAccountUseCount = string.Empty;
  
            if (operationType == 1)
            {
                updateSmsAccountUseCount = string.Format(@"update bx_sms_account set avail_times =avail_times+{0} where agent_id ={1}", calculatedCount, agentId);
            }
            else
            {
                updateSmsAccountUseCount = string.Format(@"update bx_sms_account set avail_times=avail_times-{0} where agent_id={1}", calculatedCount, agentId);
            }
            return Convert.ToInt32(_mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSmsAccountUseCount, null)) > 0;
        }
        public int GetAvailCount(int agentId)
        {
            //System.Dynamic.ExpandoObject result = new System.Dynamic.ExpandoObject();
            var getAvailCountSql = string.Format(@" select avail_times from bx_sms_account where agent_id={0}", agentId);
            //result= _mySqlHelper.ExecuteDataRow(CommandType.Text, getAvailCountSql, null).ToT<dynamic>();
            return Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, getAvailCountSql, null));
        }
        public bool ChangeSmsBulkRedcordToFail(int id)
        {
            bool isSuccess = false;
            var changeSmsBulkRedcordToFail = string.Format(@"update bx_sms_batch_history  set status=3,FailedCount=SendedCount,SendedCount=0 where id={0}", id);

            var changeSmsAccountContent = string.Format(@"update  bx_sms_account_content set sendStatus=3 where batchid={0}", id);
            isSuccess=Convert.ToInt32( _mySqlHelper.ExecuteNonQuery(CommandType.Text, changeSmsBulkRedcordToFail, null))>0;
            isSuccess=Convert.ToInt32( _mySqlHelper.ExecuteNonQuery(CommandType.Text, changeSmsAccountContent, null))>0;
            return isSuccess;
        }
        private string CreateUpdateStatusSql()
        {
            return @"update bx_sms_batch_history set status=?status where id=?id";
        }

        private void UpdateSmsAccountContent(int id, List<string> mobileList, string smsContent, int status, int agentId, string agentName,DateTime sendTime)
        {
            var mobiles = GetSendMobilesAndContentById(id).MobileList;
            var intersectMobiles = mobileList.Intersect(mobiles).ToList();
            var needDeleteMobiles = mobiles.Except(intersectMobiles).ToList();
            var needAddMobiles = mobileList.Except(intersectMobiles).ToList();
            string deleteSmsAccountContentSql = string.Empty;

            StringBuilder addSmsAccountContentSql = new StringBuilder(@"insert into bx_sms_account_content(agent_id,sent_mobile,content,create_time,agent_name,sent_type,business_type,sendStatus,batchid,license_no) values ");
            if (intersectMobiles.Count() > 0)
            {
                UpdateAccountContent(id, status, smsContent, intersectMobiles.ToList(),sendTime);
            }
            if (needDeleteMobiles.Count() > 0)
            {

                if (needDeleteMobiles.Count() == mobiles.Count)
                {
                    deleteSmsAccountContentSql = string.Format(@"update bx_sms_account_content set isDelete=1 where batchid={0}", id);
                }
                else
                {
                    for (int i = 0; i < needDeleteMobiles.Count; i++)
                    {
                        needDeleteMobiles[i] = "'" + needDeleteMobiles[i] + "'";
                    }
                    deleteSmsAccountContentSql = string.Format(@"update bx_sms_account_content set isDelete=1 where batchid={0} and sent_mobile in({1})", id, string.Join(",", needDeleteMobiles));
                }

                _mySqlHelper.ExecuteNonQuery(CommandType.Text, deleteSmsAccountContentSql, null);
            }
            if (needAddMobiles.Count() > 0)
            {
                foreach (var mobile in needAddMobiles)
                {
                    addSmsAccountContentSql.Append(string.Format(" ({0},'{1}','{2}','{3}','{4}',0,7,{5},{6},''), ", agentId, mobile, smsContent,sendTime, agentName, status, id));
                }
                _mySqlHelper.ExecuteNonQuery(CommandType.Text, addSmsAccountContentSql.ToString().Substring(0, addSmsAccountContentSql.ToString().Length - 2), null);
            }
        }
        private void UpdateAccountContent(int id, int status, string smsContent = "", List<string> mobileList = null,DateTime? sendTime=null)
        {
            string setValueSql =string.Format(" sendstatus={0} ", status);
            if (!string.IsNullOrWhiteSpace(smsContent))
            {
                setValueSql += string.Format(",content='{0}' ", smsContent);
            }
            if (sendTime.HasValue) {
                setValueSql += string.Format(",create_time='{0}' ", sendTime.Value);
            }
            string updateSql = string.Empty;
            if (mobileList == null)
            {
                updateSql = string.Format(@"update bx_sms_account_content set {0} where batchid={1} )", setValueSql, id);
            }
            else
            {
                for (int i = 0; i < mobileList.Count; i++)
                {
                    mobileList[i] = "'" + mobileList[i] + "'";
                }

                updateSql = string.Format(@"update bx_sms_account_content set {0} where batchid={1} and sent_mobile in ({2})", setValueSql, id, string.Join(",", mobileList));
            }

            _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSql, null);
        }
    }
}
