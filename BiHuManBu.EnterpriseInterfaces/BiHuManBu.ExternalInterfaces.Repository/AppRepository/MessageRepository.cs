using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models;
using log4net;
using MySql.Data.MySqlClient;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;

using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class MessageRepository : AppIRepository.IMessageRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        #region bx_message
        public int Add(bx_message bxMessage)
        {
            int workOrderId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_message.Add(bxMessage);
                DataContextFactory.GetDataContext().SaveChanges();
                workOrderId = t.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderId = 0;
            }
            return workOrderId;
        }

        public int Update(bx_message bxMessage)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_message.AddOrUpdate(bxMessage);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public bx_message Find(int msgId)
        {
            bx_message bxMessage = new bx_message();
            try
            {
                bxMessage = DataContextFactory.GetDataContext().bx_message.FirstOrDefault(i => i.Id == msgId && i.Msg_Status == 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMessage;
        }
        /// <summary>
        /// 获取所有未读的msg表记录，未排序，消息列表用
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<bx_message> FindNoReadList(int agentId, out int total)
        {
            List<bx_message> list = new List<bx_message>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_message.Where(i => i.Agent_Id == agentId && i.Msg_Status == 0 && i.Msg_Type != 2 && i.Msg_Type != 1 && i.Send_Time.Value <= DateTime.Now).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }

        public List<TableMessage> FindNoReadAllList(int agentId, out int total)
        {
            List<TableMessage> list = new List<TableMessage>();
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int64)
                };
                parameters[0].Value = agentId;
                string strSql = @"SELECT CONCAT('Msg_',Id) AS StrId,Id,Msg_Level,Msg_Status,Msg_Type,Send_Time,Title,Body,
                                update_time,url,
                                agent_id,(SELECT AgentName FROM bx_agent WHERE id=agent_id) AS AgentName,
                                create_agent_id,(SELECT AgentName FROM bx_agent WHERE id=agent_id) AS CreateAgentName,
                                create_Time,(select licenseno from bx_userinfo where id=buid) AS licenseno,'' AS last_force_end_date,'' AS Last_biz_end_date,'' AS next_force_start_date,'' AS next_biz_start_date,
                                (select LastYearSource from bx_userinfo where id=buid) AS source,0 AS days,buid,
                                (select Agent from bx_userinfo where id=buid) AS OwnerAgent FROM bx_message 
                                WHERE Agent_Id=@agentId
                                AND Msg_Status=0 AND msg_type!=2 AND msg_type!=1 AND Send_Time <= NOW()
                                UNION ALL 
                                SELECT CONCAT('Xb_',Id) AS StrId,id,1,bx_notice_xb.stauts,1,create_time,'到期通知',CONCAT(license_no,'车险',day_num,'天后到期') AS Body,
                                '' AS update_time,'' AS url,
                                agent_id,(SELECT AgentName FROM bx_agent WHERE id=agent_id) AS AgentName,
                                agent_id AS create_agent_id,(SELECT AgentName FROM bx_agent WHERE id=agent_id) AS CreateAgentName,
                                create_time,license_no as licenseno,last_force_end_date,Last_biz_end_date,next_force_start_date,next_biz_start_date,
                                source,days,b_uid,
                                (select Agent from bx_userinfo where id=b_uid) AS OwnerAgent FROM bx_notice_xb 
                                WHERE agent_id=@agentId AND bx_notice_xb.stauts=0
                                UNION ALL 
                                SELECT CONCAT('Rw_',Id) AS StrId,id,1 AS Msg_Level,read_status,2 AS Msg_Type,create_time,'回访通知',CONCAT(date_format(next_review_date,'%m月%d日 %H:%I'),'需回访',(select LicenseNo from bx_userinfo where id=b_uid)) AS Body,
                                '' AS update_time,'' AS url,
                                operatorid,(SELECT AgentName FROM bx_agent WHERE id=operatorid) AS AgentName,
                                operatorid AS create_agent_id,(SELECT AgentName FROM bx_agent WHERE id=operatorid) AS CreateAgentName,
                                create_time,(select licenseno from bx_userinfo where id=b_uid) AS licenseno,'' AS last_force_end_date,'' AS Last_biz_end_date,'' AS next_force_start_date,'' AS next_biz_start_date,
                                (select LastYearSource from bx_userinfo where id=b_uid) AS source,0 AS days,b_uid,
                                (select Agent from bx_userinfo where id=b_uid) AS OwnerAgent FROM bx_consumer_review
                                WHERE operatorid=@agentId AND read_status=0 AND next_review_date IS NOT NULL AND next_review_date!=NULL
                                AND next_review_date <= NOW()";
                list = DataContextFactory.GetDataContext().Database.SqlQuery<TableMessage>(strSql, parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }

        /// <summary>
        /// 保存代理人和信鸽推送应用关系
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int SaveAgent_XGAccount_RelationShip(bx_agent_xgaccount_relationship agentXGAccountRelationShip)
        {
            var xgAccount = DataContextFactory.GetDataContext().bx_agent_xgaccount_relationship.FirstOrDefault(x => x.AgentId == agentXGAccountRelationShip.AgentId && x.DeviceType == agentXGAccountRelationShip.DeviceType);
            if (xgAccount == null)
            {
                DataContextFactory.GetDataContext().bx_agent_xgaccount_relationship.Add(agentXGAccountRelationShip);
            }
            else
            {
                xgAccount.UpdateTime = DateTime.Now;
            }
            return DataContextFactory.GetDataContext().SaveChanges();

        }
        #endregion

        #region bx_system_message

        public int UpdateMessageStatus(long id, int readStatus)
        {
            int count = -1;
            var msgIndexModel = DataContextFactory.GetDataContext().bx_msgindex.SingleOrDefault(x => x.Id == id);
            if (!new List<int> { 2, 3, 6, 7 }.Contains(msgIndexModel.ReadStatus))
            {
                msgIndexModel.ReadStatus += readStatus;

                count = DataContextFactory.GetDataContext().SaveChanges();

            }
            return count;
            #endregion
        }

        public List<MessageHistory> GetMessageHistory(int agentId, int readStatus, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            try
            {
                string strreadstatus = string.Empty;
                if (readStatus == 1)
                {
                    strreadstatus = " AND bx_msgindex.ReadStatus&2=2";
                }
                else if (readStatus == 2)
                {
                    strreadstatus = " AND bx_msgindex.ReadStatus&2=0";
                }
                //查询总数
                var strcount = new StringBuilder();
                strcount.Append(
                    "SELECT COUNT(1) FROM bx_msgindex LEFT JOIN bx_message ON bx_message.Id=bx_msgindex.MsgId")  //")//
                    .Append(" LEFT JOIN bx_userinfo ON bx_message.Buid=bx_userinfo.Id")
                    .Append(string.Format(
                        " WHERE bx_msgindex.Deleted&4=0 AND bx_msgindex.AgentId={0} {1} AND bx_message.Msg_Type in (2,8,9,10) ", agentId, strreadstatus));
                totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(strcount.ToString()).FirstOrDefault();
                //查询列表
                var strlist = new StringBuilder();
                strlist.Append("SELECT IFNULL(bx_userinfo.Id,0) AS BuId,")
                    .Append("bx_msgindex.Id AS Id,")
                    .Append("bx_message.Title,")
                    .Append("bx_message.Body,")
                    .Append("bx_msgindex.AgentId,")
                    .Append("bx_msgindex.ReadStatus,")
                    .Append("bx_msgindex.SendTime,")
                     .Append("bx_message.Id AS MsgId,")
                      .Append("bx_message.Msg_Type AS MsgType,")
                    .Append(" IFNULL(bx_userinfo.IsDistributed,0) IsDistributed FROM bx_msgindex LEFT JOIN bx_message ON bx_message.Id=bx_msgindex.MsgId")
                    .Append(" LEFT JOIN bx_userinfo ON bx_message.Buid=bx_userinfo.Id")
                    .Append(string.Format(
                    " WHERE bx_msgindex.Deleted&4=0 {0} AND bx_msgindex.AgentId={1} AND bx_message.Msg_Type in (2,8,9,10) ", strreadstatus, agentId))
                    .Append(string.Format(" ORDER BY bx_msgindex.Id DESC LIMIT {0},{1}", (pageIndex - 1) * pageSize, pageSize));
                List<MessageHistory> list =
                    DataContextFactory.GetDataContext().Database.SqlQuery<MessageHistory>(strlist.ToString()).ToList();
                return list;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<MessageHistory>();
        }
        public int DeleteMessage(long id)
        {
            var msgIndexModel = DataContextFactory.GetDataContext().bx_msgindex.SingleOrDefault(x => x.Id == id);
            msgIndexModel.Deleted = 7;
            return DataContextFactory.GetDataContext().SaveChanges();
        }

        public bool CheckXgAccount(int agentId, int deviceType)
        {
            return DataContextFactory.GetDataContext().bx_agent_xgaccount_relationship.Any(x => x.AgentId == agentId && x.DeviceType == deviceType && !x.Deleted);
        }

        public List<SettedAgent> GetSettedAgents(int agentId, int pageIndex, int pageSize, out int totalCount)
        {
            string strWhere = string.Format(@" FROM bx_agent_distributed LEFT JOIN bx_agent ON bx_agent.Id=bx_agent_distributed.AgentId
WHERE bx_agent_distributed.ParentAgentId={0} AND bx_agent_distributed.Deteled=0 AND bx_agent_distributed.AgentType=0 AND bx_agent.IsUsed=1 limit {1},{2}", agentId, (pageIndex - 1) * pageSize, pageSize);
            string strCount = string.Format("SELECT count(1) {0}", strWhere);
            string strSql = string.Format("SELECT bx_agent_distributed.Id,bx_agent.AgentName,bx_agent.Id as AgentId {0}", strWhere);
            totalCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(strCount).FirstOrDefault();
            return DataContextFactory.GetDataContext().Database.SqlQuery<SettedAgent>(strSql).ToList();
        }

        public long GetUsableSmsCount(int agentId, int topAgentId, out string agentAccount)
        {
            agentAccount = string.Empty;
            long count = 0;
            int usedAgentId = agentId;
            var agentModel = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == usedAgentId);
            agentAccount = agentModel.AgentName;
            if (agentModel.MessagePayType == 0)
            {
                usedAgentId = topAgentId;
                agentAccount = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == usedAgentId).AgentName;
            }
            var sms_accountModel = DataContextFactory.GetDataContext().bx_sms_account.FirstOrDefault(x => x.agent_id == usedAgentId);
            if (sms_accountModel != null)
            {
                if (sms_accountModel.status == 0)
                {
                    count = -1;
                }
                else
                {
                    count = sms_accountModel.avail_times.HasValue ? sms_accountModel.avail_times.Value : 0;
                    agentAccount = agentAccount;
                }
            }
            return count;
        }
    }
}

