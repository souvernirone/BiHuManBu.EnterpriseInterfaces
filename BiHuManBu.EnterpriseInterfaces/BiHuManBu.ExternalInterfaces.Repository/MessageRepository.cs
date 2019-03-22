using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using log4net;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private ILog logError;
        private EntityContext db = DataContextFactory.GetDataContext();
        //private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        //private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        public MessageRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        #region 新消息系统
        /// <summary>
        /// 添加bx_msgIndex,由于添加的量很大，所以直接写sql添加
        /// </summary>
        /// <param name="insertSql"></param>
        /// <returns></returns>
        public bool Add(string insertSql)
        {
            return db.Database.ExecuteSqlCommand(insertSql) > 0;
        }

        public long AddMsgIdx(bx_msgindex model)
        {
            long modelId = 0;
            try
            {
                var t = db.bx_msgindex.Add(model);
                db.SaveChanges();
                modelId = t.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                modelId = 0;
            }
            return modelId;
        }

        public async Task<bool> AddMsgIndexListAsync(List<bx_msgindex> list)
        {
            var content = db;
            content.bx_msgindex.AddRange(list);
            return (await content.SaveChangesAsync() > 0);
        }

        /// <summary>
        /// 未读消息总数
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public int MsgCount(int agentId, int method)
        {
            int totalCount = 0;
            try
            {
                totalCount = db.bx_msgindex.Count(i => i.AgentId == agentId && (i.ReadStatus & method) == 0 && (i.Method & method) > 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return totalCount;
        }

        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="pageSize"></param>
        /// <param name="curPage"></param>
        /// <returns></returns>
        public List<bx_msgindex> MsgList(int agentId, int msgMethod, int pageSize, int curPage, out int total)
        {
            List<bx_msgindex> list = new List<bx_msgindex>();
            try
            {
                list = db.bx_msgindex.Where(i => i.AgentId == agentId && (i.Method & msgMethod) > 0).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list.OrderBy(o => o.ReadStatus & msgMethod).ThenByDescending(o => o.Id).Take(pageSize * curPage).Skip(pageSize * (curPage - 1)).ToList();
        }

        /// <summary>
        /// 判断msgid是否存在
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public bool IsExist(int msgId)
        {
            return db.bx_msgindex.Any(o => o.MsgId == msgId);
        }

        /// <summary>
        /// 根据关联Id取消息详情
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public bx_message GetMsgDetail(long indexId)
        {
            try
            {
                var msg = from message in db.bx_message
                          join msgIndex in db.bx_msgindex
                          on message.Id equals msgIndex.MsgId
                          where message.MsgStatus == "1" && msgIndex.Id == indexId
                          select message;
                return msg.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new bx_message();
            }
        }
        public int GetMsgId(long indexId)
        {
            int msgId = 0;
            try
            {
                msgId = db.bx_msgindex.Where(i => i.Id == indexId).Select(s => s.MsgId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return msgId;
        }
        /// <summary>
        /// 根据关联Id取消息详情
        /// </summary>
        /// <param name="childagent"></param>
        /// <returns></returns>
        public bx_message MsgLastDetail(long childagent, int msgMethod)
        {
            bx_message bxMessage = new bx_message();
            try
            {
                int msgId = GetLastMsgId(childagent, msgMethod);//取信息id
                if (msgId != 0)
                    bxMessage = db.bx_message.FirstOrDefault(i => i.Id == msgId && i.MsgStatus.Equals("1"));
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMessage;
        }
        public int GetLastMsgId(long childagent, int msgMethod)
        {
            int msgId = 0;
            try
            {
                //msgId = db.bx_msgindex.Where(i => i.AgentId == childagent && (i.ReadStatus & msgMethod) == 0 && (i.Method & msgMethod) > 0).OrderByDescending(o => o.SendTime).Select(s => s.MsgId).FirstOrDefault();
                var message = db.bx_msgindex.Where(i => i.AgentId == childagent && (i.Deleted & msgMethod) == 0 && (i.Method & msgMethod) > 0).OrderByDescending(t => t.SendTime).FirstOrDefault();
                if (message==null)
                {
                    return msgId;
                }
                if (message.ReadStatus == 0)
                {
                    msgId = message.MsgId;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return msgId;
        }
        /// <summary>
        /// 根据关系Id 取 消息关系表的记录
        /// </summary>
        /// <param name="indexId"></param>
        /// <returns></returns>
        public bx_msgindex GetMsgIndex(long indexId, int msgMethod)
        {
            bx_msgindex bxMsgIndex = new bx_msgindex();
            try
            {
                bxMsgIndex = db.bx_msgindex.FirstOrDefault(i => i.Id == indexId && (i.Method & msgMethod) > 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMsgIndex;
        }
        /// <summary>
        /// 根据agentid和消息id 取 消息关系表的记录
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public bx_msgindex GetMsgIndex(long agentId, int msgId, int msgMethod)
        {
            bx_msgindex bxMsgIndex = new bx_msgindex();
            try
            {
                bxMsgIndex = db.bx_msgindex.FirstOrDefault(i => i.MsgId == msgId && i.AgentId == agentId && (i.Method & msgMethod) > 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMsgIndex;
        }
        /// <summary>
        /// 更新消息关系表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateMsgIndex(bx_msgindex model)
        {
            int count = 0;
            try
            {
                db.bx_msgindex.AddOrUpdate(model);
                count = db.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public int UpdateChildAgentMsg(long childAgent, int msgMethod)
        {
            int count = 0;
            try
            {
                var querySql = string.Format("update bx_msgindex set ReadStatus=ReadStatus|{0},ReadTime='{1}' where AgentId={2} And ReadStatus&{3}=0 And Method&{4}>0", msgMethod, DateTime.Now, childAgent, msgMethod, msgMethod);
                return db.Database.ExecuteSqlCommand(querySql);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public List<bx_agent_xgaccount_relationship> GetXgAccounts(int[] agentIds, int deviceType = 4)
        {
            if (deviceType == 4)
            {
                return db.bx_agent_xgaccount_relationship.Where(x => agentIds.Contains(x.AgentId) && !x.Deleted&&new List<int>() { 1,2}.Contains( x.DeviceType)).ToList();
            }
            else
            {
                return db.bx_agent_xgaccount_relationship.Where(x => agentIds.Contains(x.AgentId) && !x.Deleted && new List<int>() { 3, 4 }.Contains(x.DeviceType)).ToList();
            }
        }

        public bx_agent_xgaccount_relationship GetXgAccount(int agentId)
        {
            return db.bx_agent_xgaccount_relationship.FirstOrDefault(x => x.AgentId == agentId && !x.Deleted);
        }

        #endregion

        #region 迁移过来的消息模块

        #region bx_message
        public int Add(bx_message bxMessage)
        {
            int workOrderId = 0;
            try
            {
                var t = db.bx_message.Add(bxMessage);
                db.SaveChanges();
                workOrderId = t.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderId = 0;
            }
            return workOrderId;
        }

        public async Task<bool> AddListAsync(List<bx_message> listMsg)
        {
            if (listMsg.Count == 0)
                return true;
            var content = db;
            foreach (var item in listMsg)
            {
                content.bx_message.Add(item);
            }
            return (await content.SaveChangesAsync() > 0);
        }

        public int Update(bx_message bxMessage)
        {
            int count = 0;
            try
            {
                db.bx_message.AddOrUpdate(bxMessage);
                count = db.SaveChanges();
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
                bxMessage = db.bx_message.FirstOrDefault(i => i.Id == msgId && i.Msg_Status == 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMessage;
        }
        public bx_message FindById(int msgId)
        {
            bx_message bxMessage = new bx_message();
            try
            {
                bxMessage = db.bx_message.FirstOrDefault(i => i.Id == msgId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxMessage;
        }

        public List<bx_message> FindMsgList(int agentId, int pageSize, int curPage, out int total)
        {
            List<bx_message> list = new List<bx_message>();
            try
            {
                list = db.bx_message.Where(i => i.Agent_Id == agentId && i.Msg_Status == 0).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list.OrderByDescending(o => o.Msg_Status).ThenByDescending(o => o.Update_Time).Take(pageSize * curPage).Skip(pageSize * (curPage - 1)).ToList();
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
                list = db.bx_message.Where(i => i.Agent_Id == agentId && i.Msg_Status == 0 && i.Msg_Type != 2 && i.Msg_Type != 1 && i.Send_Time.Value <= DateTime.Now).ToList();
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
                list = db.Database.SqlQuery<TableMessage>(strSql, parameters.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }
        #region 3表关联查询注释
        #endregion

        #endregion

        #region bx_system_message
        public int AddSysMessage(bx_system_message bxSystemMessage)
        {
            int workOrderId = 0;
            try
            {
                var t = db.bx_system_message.Add(bxSystemMessage);
                db.SaveChanges();
                workOrderId = t.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderId = 0;
            }
            return workOrderId;
        }

        public int UpdateSysMessage(bx_system_message bxSystemMessage)
        {
            int count = 0;
            try
            {
                db.bx_system_message.AddOrUpdate(bxSystemMessage);
                count = db.SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public List<bx_system_message> FindSysMessage()
        {
            List<bx_system_message> list = new List<bx_system_message>();
            try
            {
                list = db.bx_system_message.OrderByDescending(o => o.Create_Time).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        #endregion
        #endregion
    }
}
