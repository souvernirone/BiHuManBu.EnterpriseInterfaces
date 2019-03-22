using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CustomerBusinessRepository : ICustomerBusinessRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("INFO");
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        /// <summary>
        /// 根据客户AgentID 获取 下级业务员列表信息
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <returns></returns>
        public List<CTopLevelAgentViewModel> GetBusinessAgentList(int currentAgentId)
        {
            List<CTopLevelAgentViewModel> modellist = null;
            try
            {
                var AgentQuery = from a in DataContextFactory.GetDataContext().bx_agent_distributed.Where(x => !x.Deteled && x.ParentAgentId == currentAgentId && x.AgentType == 0)
                                 join b in DataContextFactory.GetDataContext().bx_agent.Where(x => x.AgentAccount != null && x.AgentAccount != "")
                                 on a.AgentId equals b.Id into temp1
                                 from t1 in temp1.DefaultIfEmpty()
                                 join d in DataContextFactory.GetDataContext().manager_role_db
                                 on t1.ManagerRoleId equals d.id
                                 select new CTopLevelAgentViewModel
                                 {
                                     UpdateTime = a.UpdateTime,
                                     AgentId = a.AgentId,
                                     AgentName = t1.AgentName,
                                     Id = a.Id,
                                     IsNotifIedInList = 1,
                                     Mobile = t1.Mobile,
                                     RoleId = d.id,
                                     RoleName = d.role_name,
                                     IsUsed = t1.IsUsed.Value
                                 };

                modellist = AgentQuery.ToList();
            }
            catch (Exception ex)
            {
                //获取业务员列表赋值出错
            }
            // }
            return modellist;
        }

        /// <summary>
        /// 根据AgentID 从数据库中查询业务成员信息
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <returns></returns>
        public List<int> GetAgentIds(int currentAgentId)
        {
            //MySqlParameter ps = new MySqlParameter() { MySqlDbType = MySqlDbType.Int32, Value = currentAgentId, ParameterName = "AgentId" };
            //var idStr = DataContextFactory.GetDataContext().Database.SqlQuery(typeof(string), "select queryChildrenAgentID(?AgentId)", ps).Cast<string>().First();
            string sql = string.Format(@"select `queryChildrenAgentID`({0})", currentAgentId);
            var idStr = DataContextFactory.GetDataContext().Database.SqlQuery<string>(sql).First();
            var idArray = idStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return idArray.Select(x => int.Parse(x)).ToList();
        }

        public List<AgentDistributedViewModel> GetDistributAgentList(List<int> agentIds)
        {
            return DataContextFactory.GetDataContext().bx_agent.Where(x => agentIds.Contains(x.Id)).Select(x => new AgentDistributedViewModel { Id = x.Id, OpenId = x.OpenId }).ToList();
        }

        public bool UpdateIsDistributeByBatchId(long bathcId)
        {
            var singleOrDefault = DataContextFactory.GetDataContext().bx_batchrenewal.SingleOrDefault(x => x.Id == bathcId);
            if (singleOrDefault != null)
                singleOrDefault.IsDistributed = true;
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        public bool IsAllDistributed(long batchId)
        {
            var originalBuIds = DataContextFactory.GetDataContext().bx_batchrenewal_item.Where(x => x.BatchId == batchId).Select(x => x.BUId);
            return DataContextFactory.GetDataContext().bx_userinfo.Count(x => originalBuIds.Contains(x.Id) && x.IsDistributed == 0) > 0 ? false : true;
        }

        public Boolean BulkUpdateByList(List<UpdateUserInfoModel> userInfoModels)
        {
            string dateTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder builder = new StringBuilder();
            // 根据生成多条sql，要事务执行
            var groupAgent = userInfoModels.GroupBy(o => o.Agent);
            foreach (var item in groupAgent)
            {
                var temp = item.FirstOrDefault();
                var sql = string.Format("update bx_userinfo set Agent='{0}',OpenId='{1}',IsDistributed={2},DistributedTime='{3}',agent_id={5},UpdateTime='{6}',top_agent_id={7},IsCamera={8},CameraTime='{9}' where id in ({4});",
                    temp.Agent
                    , temp.OpenId
                    , temp.IsDistributed
                    , temp.DistributedTime
                    , string.Join(",", item.Select(o => o.Id))
                    , int.Parse(temp.Agent)
                    ,dateTimeStr
                    ,temp.TopAgentId
                    ,temp.IsCamera
                    ,temp.CameraTime);
                    
                builder.Append(sql);
            }

            var finalSql = builder.ToString();
            try
            {               
                using (TransactionScope trans = new TransactionScope())
                {
                    var count = _dbHelper.ExecuteNonQuery(finalSql);
                    trans.Complete();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public int UpdateIstest(string buids)
        {
            string sql = string.Format(@"UPDATE bx_userinfo SET IsTest=1  WHERE id IN({0});", buids);
            return _dbHelper.ExecuteNonQuery(sql);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateBratch"></param>
        /// <param name="noBuidList">不需要更新批续的buid集合</param>
        /// <returns></returns>
        public int UpdatebratchItem(Dictionary<long, long> updateBratch, List<long> noBuidList)
        {
            string sql = "";
            foreach (var item in updateBratch)
            {
                if (noBuidList.Contains(item.Key))
                {
                    continue;
                }
                sql += string.Format(@"UPDATE bx_batchrenewal_item  SET BUId = {0}  WHERE BUId ={1};", item.Key, item.Value);
            }
            if (string.IsNullOrEmpty(sql))
            {
                return 1;
            }
            return _dbHelper.ExecuteNonQuery(sql);
        }
        public List<long> GetBatchRenewalItemBuidList(Dictionary<long, long> updateBratch)
        {
            List<long> list = new List<long>();
            try
            {
                List<long> buids = updateBratch.Keys.ToList();
                if (buids==null||buids.Count==0)
                {
                    return list;
                }
                string sql = "SELECT BUId FROM bx_batchrenewal_item WHERE BUId IN ("+string.Join(",",buids)+") AND IsNew = 1 AND IsDelete = 0";
                list = DataContextFactory.GetDataContext().Database.SqlQuery<long>(sql).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetBatchRenewalItemBuidList获取要分配数据的批续数据，发生异常：" + ex);
            }
            return list;
        }
        public int UpdateDistributeRecycle(string buids, int toAgent, string openid, int isDistributed)
        {
            string sql = string.Format("UPDATE bx_userinfo SET OpenId ='{0}' , Agent={1},IsDistributed={3},agent_id={4},UpdateTime=now()  WHERE Id  in ({2}) and IsDistributed in (2,3); ", openid,
                toAgent, buids, isDistributed, toAgent);
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);

        }


        public bool SaveGroupTransferRecord(int fromAgentId, List<UpdateUserInfoModel> updateUserInfoModelList, List<long> buids, int stepType)
        {
            string updateTransferRecordSql = string.Format(@"update bx_transferrecord set deleted=1 where BuId in({0}) and stepType={1}", string.Join(",", buids), stepType);

            MySqlHelper.ExecuteNonQuery(DbConfig, updateTransferRecordSql, null);
            var saveTransferRecords = new List<SaveTransferRecord>();
            if (updateUserInfoModelList != null)
            {
                foreach (var item in updateUserInfoModelList)
                {
                    var saveTransferRecord = new SaveTransferRecord
                    {
                        BuId = item.Id,
                        CreateAgentId = fromAgentId,
                        CreateTime = DateTime.Now,
                        FromAgentId = fromAgentId,
                        StepType = stepType,
                        ToAgentId = Convert.ToInt32(item.Agent),
                        UpdateTime = DateTime.Now
                    };
                    saveTransferRecords.Add(saveTransferRecord);

                }
                return Convert.ToInt32(MySqlHelper.InsertByList(saveTransferRecords, DbConfig, "bx_transferrecord")) != 0;
            }
            else
            {
                foreach (var item in buids)
                {
                    var saveTransferRecord = new SaveTransferRecord();
                    saveTransferRecord.BuId = item;
                    saveTransferRecord.CreateAgentId = fromAgentId;
                    saveTransferRecord.CreateTime = DateTime.Now;
                    saveTransferRecord.FromAgentId = fromAgentId;
                    saveTransferRecord.StepType = stepType;
                    saveTransferRecord.ToAgentId = 0;
                    saveTransferRecord.UpdateTime = DateTime.Now;
                    saveTransferRecords.Add(saveTransferRecord);
                }
                return Convert.ToInt32(MySqlHelper.InsertByList(saveTransferRecords, DbConfig, "bx_transferrecord")) != 0;
            }
        }

        /// <summary>
        /// 通过Buid从库中查找车牌号
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public string GetLicenseNoByBuid(long buid)
        {
            string LicenseNo = "";
            var originalBuIds = DataContextFactory.GetDataContext().bx_userinfo.SingleOrDefault(x => x.Id == buid);
            LicenseNo = originalBuIds.LicenseNo;
            return LicenseNo;
        }

        public List<BxUserinfoViewModel> GetUserinfosForBuids(List<long> buidsList)
        {
            string sql =
                string.Format(
                    @"SELECT 	Id, UserId, UserName, LicenseNo, Mobile, OpenId, CityCode, RenewalIdNo, EngineNo, CarVIN, Source, LastYearSource, MoldName, RegisterDate, ApproxDate, Address, VehicleId, StandardName, NeedEngineNo, 
	CreateTime, UpdateTime, ProcessStep, QuoteStatus, 
	OrderStatus, IsOrder, IsReView, JiSuanType, IsService, ServiceTime, Agent, IdCard, IsLastYear, LicenseOwner, IsTest, InsuredName,InsuredMobile, InsuredIdCard, 
	InsuredAddress, IsInputBxData, IsRiskVehicle, IsPeopleQuote, HongBaoId, HongBaoAmount, Datasource, 
	ApproxPeopleName, ApproxPeopleId, ApproxCreateDate, IsInstalment, IsClosing, IsSingleSubmit, RenewalType, RenewalStatus, InsuredIdType, IsDistributed, OwnerIdCardType, Email,InsuredCertiStartdate,InsuredCertiEnddate,InsuredEmail,HolderIdCard,HolderName,HolderMobile,HolderAddress,HolderIdType,HolderCertiStartdate,HolderCertiEnddate,HolderEmail 
	,CarMoldId,CategoryInfoId,OwnerCertiStartdate,OwnerCertiEnddate,OwnerCertiAddress,DistributedTime FROM 
	bx_userinfo where  Id in ({0})", string.Join(",", buidsList));
            return DataContextFactory.GetDataContext().Database.SqlQuery<BxUserinfoViewModel>(sql).ToList();
            //return   db.bx_userinfo.WhereIn(c=>c.Id, buidsList.ToArray()).ToList();
        }
        public IEnumerable<BxUserinfoRenewalViewModel> GetBxUserinfoRenewal(List<long> buidsList)
        {
            //using (var db = new EntityContext())
            //{
            //    return db.bx_userinfo_renewal_index.WhereIn(c => c.b_uid, buidsList.ToArray()).ToList();
            //}
            string sql = string.Format(@"SELECT bx_userinfo_renewal_index.b_uid,LastForceEndDate,LastBizEndDate FROM bx_userinfo_renewal_index 
            INNER JOIN bx_car_renewal ON bx_userinfo_renewal_index.car_renewal_id=bx_car_renewal.Id
            WHERE b_uid IN ({0}) 
            ", string.Join(",", buidsList));
            return _dbHelper.ExecuteDataSet(sql).ToList<BxUserinfoRenewalViewModel>();
        }

        public List<DistributeUserinfoDto> GetUserinfosForagents(List<string> agentList)
        {
            return DataContextFactory.GetDataContext().bx_userinfo.Where(o => agentList.Contains(o.Agent) && o.IsTest == 0).Select(o => new DistributeUserinfoDto
            {
                Id = o.Id,
                LicenseNo = o.LicenseNo,
                Agent = o.Agent
            }).ToList();
        }

        public int InsertBxMessage(bx_message bxMessages)
        {
            if (bxMessages == null)
            {
                return 0;
            }
            DataContextFactory.GetDataContext().bx_message.Add(bxMessages);
            DataContextFactory.GetDataContext().SaveChanges();
            return bxMessages.Id;
            //string sql = @"INSERT INTO bx_message ( Title, Body, Msg_Type, Agent_Id, Create_Time, Msg_Status, Msg_Level, Send_Time, Create_Agent_Id) VALUES ";
            //foreach (bx_message bxMessage in bxMessages)
            //{
            //    sql += @" ('"+ bxMessage.Title + "','" + bxMessage .Body+"',"+bxMessage.Msg_Type+","+bxMessage.Agent_Id+",'"+bxMessage.Create_Time+ "',"+bxMessage.Msg_Status+","+bxMessage.Msg_Level+",'"+bxMessage.Send_Time+"',"+bxMessage.Create_Agent_Id+"),";
            //}
            //sql = sql.Substring(0, sql.Length - 1);
            //sql += " ;SELECT  LAST_INSERT_ID();";
            //return _dbHelper.ExecuteNonQuery(sql);
        }

        public List<BxMessageViewModel> GetBxMessages(int agentId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.String,
                    ParameterName = "agentId",
                    Value = agentId
                }
            };

            string sql = string.Format(@"SELECT 	Id, Title, Body, Msg_Type, Agent_Id, Create_Time, Update_Time, Msg_Status, Msg_Level, 
	Send_Time, Create_Agent_Id, Url, License_No, Buid
	 
	FROM bx_message WHERE Msg_Type=6 AND Agent_Id IN ({0}) and Msg_Status=0
	", agentId);
            return DataContextFactory.GetDataContext().Database.SqlQuery<BxMessageViewModel>(sql).ToList();
        }


        public int UpdateBxMessage(string messageIds)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.String,
                    ParameterName = "messageIds",
                    Value = messageIds
                }

            };
            string sql = string.Format(@"UPDATE bx_message 
	                                    SET
	                                    Msg_Status=1
	                                    WHERE
	                                    Id  IN ({0});", messageIds);
            // ReSharper disable once CoVariantArrayConversion
            //return db.Database.SqlQuery<int>(sql, parameters.ToArray()).FirstOrDefault();
            return _dbHelper.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 判断是否存在没有续保完成的数据（续保中、排队中）
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetUnfinishBatchRefreshRenewal(int topAgentId, int childAgent)
        {
            List<bx_batchrefreshrenewal> list = new List<bx_batchrefreshrenewal>();
            list = DataContextFactory.GetDataContext().bx_batchrefreshrenewal.Where(a =>
               a.operate_agent == childAgent && a.topagentid == topAgentId && a.is_deleted == 0
               && (a.refrenewalstatus == 5 || a.refrenewalstatus == 6)).ToList();
            //判断该数据在该天是否是再次续保
            List<bx_batchrefreshrenewal> list2 = DataContextFactory.GetDataContext().bx_batchrefreshrenewal.Where(a =>
                a.operate_agent == childAgent && a.topagentid == topAgentId && a.is_deleted == 0 && (a.refreshtimes < a.sendtimes)).ToList();
            if (list2 != null && list2.Count > 0)
            {
                list.AddRange(list2);
            }
            return list;
        }
        public int GetTodayBatchRefRenewal(int topAgentId, int childAgent)
        {
            string sql = "SELECT SUM(IF(is_deleted=0,sendtimes,sendtimes-1)) FROM bx_batchrefreshrenewal WHERE (is_deleted=0 OR sendtimes>1 ) AND operate_agent=@ChildAgent AND topagentid=@TopAgentId AND updatetime>=CURDATE()";
            MySqlParameter[] sqlParams ={
                                           new MySqlParameter(){
                                                  ParameterName="@ChildAgent",
                                                  MySqlDbType = MySqlDbType.Int32,
                                                  Value=childAgent
                                              },
                                              new MySqlParameter(){
                                                  ParameterName="@TopAgentId",
                                                  MySqlDbType = MySqlDbType.Int32,
                                                  Value=topAgentId
                                              }
                                       };
            int? maxSum = 0;
            maxSum = DataContextFactory.GetDataContext().Database.SqlQuery<int>(sql, sqlParams).FirstOrDefault();
            return maxSum.HasValue ? maxSum.Value : 0;
        }

        public List<bx_batchrefreshrenewal> GetTodayBatchRefRenewalList(int topAgentId, int childAgent)
        {
            try
            {
                string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE operate_agent=" + childAgent + " AND topagentid=" + topAgentId + " AND createtime>=CURDATE()";

                return DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
            }
            return null;
        }
        /// <summary>
        /// 判断是否存在数据，如果存在，则更新
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<long> GetAndUpdateBatchRefreshRenewal(List<long> buids, int topAgentId, int childAgent, string date)
        {
            List<long> tempBuids = null;
            // List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().bx_batchrefreshrenewal.Where(a => 
            //   buids.Contains(a.buid) && a.operate_agent == childAgent && a.topagentid == topAgentId && a.is_deleted == 0).ToList();
            string sql2 = "SELECT * FROM bx_batchrefreshrenewal WHERE buid IN(" + string.Join(",", buids) + ") and operate_agent=" + childAgent + " AND topagentid=" + topAgentId + " AND is_deleted=0 AND createtime>=CURDATE();";
            List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql2).ToList();

            if (list != null && list.Count > 0)
            {
                tempBuids = list.Select(a => (long)a.buid).ToList();
                var tempIds = string.Join(",", list.Select(a => (long)a.id).ToList());
                string strBuids = string.Join(",", tempIds);
                string sql = "UPDATE `bx_batchrefreshrenewal` SET  `refrenewalstatus`=6 ,`updatetime` = '" + date + "' , `sendtimes` = sendtimes+1  ";
                sql += " WHERE `id` in(" + tempIds + ")";
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
            }
            return tempBuids;
        }

        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByAllBuid(List<long> buids, int topAgentId, int childAgent, string currdate)
        {
            string sql2 = "SELECT * FROM bx_batchrefreshrenewal WHERE buid IN(" + string.Join(",", buids) + ") and operate_agent=" + childAgent + " AND topagentid=" + topAgentId + " AND is_deleted=0 AND updatetime='" + currdate + "' order by id ";
            List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql2).ToList();
            return list;
        }

        public bx_batchrefreshrenewal BatchRefreshRenewalDetail(int topAgentId, int operateAgent, long Buid)
        {
            string sql2 = "SELECT * FROM bx_batchrefreshrenewal WHERE buid=" + Buid + " and operate_agent=" + operateAgent + " AND topagentid=" + topAgentId + " AND is_deleted=0 " + "AND updatetime>=CURDATE();";
            bx_batchrefreshrenewal model = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql2).FirstOrDefault();
            //bx_batchrefreshrenewal model = DataContextFactory.GetDataContext().bx_batchrefreshrenewal.Where(a =>a.buid==Buid&& a.operate_agent == operateAgent && a.topagentid == topAgentId && a.updatetime >= DateTime.Now.Date&&a.is_deleted==0).FirstOrDefault();
            if (model == null || model.id <= 0)
            {
                return null;
            }
            return model;

        }
        /// <summary>
        /// 将批量刷新续保数据添加到BatchRefreshRenewal表
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgentId">顶级代理人</param>
        /// <param name="agentId">当前代理人</param>
        /// <returns></returns>
        public bool AddBatchRefreshRenewal(List<bx_userinfo> userList, int topAgentId, int childAgent, string date)
        {
            //生成insert sql脚本
            string sql = CreateInsertSql(userList, topAgentId, childAgent, date);
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql) > 0;
        }
        private string CreateInsertSql(List<bx_userinfo> userList, int topAgentId, int childAgent, string date)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            foreach (bx_userinfo model in userList)
            {
                sqlBuilder.Append("INSERT INTO `bx_batchrefreshrenewal` ( `buid`, `agentid`, `topagentid`, `createtime`, `updatetime`, `refreshtimes`, `sendtimes`, `refrenewalstatus`, `is_deleted`,`licenseno`,`citycode`,`renewalcartype`,`openid`,`operate_agent`)");
                sqlBuilder.Append("VALUES( " + model.Id + ", " + long.Parse(model.Agent) + ", " + topAgentId + ", '" + date + "', '" + date + "', 0, 1, 6, 0,'" + model.LicenseNo + "','" + model.CityCode + "'," + model.RenewalCarType + ",'" + model.OpenId + "'," + childAgent + ");");
            }
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 获取刷新续保状态
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<BatchRefreshRenewalModel> GetBatchRefreshRenewalList(List<long> buids, int topAgentId, int childAgent)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            //sqlBuilder.Append("SELECT a.id AS buid,a.licenseno,b.refrenewalstatus FROM bx_userinfo a ");
            //sqlBuilder.Append(" INNER JOIN bx_batchrefreshrenewal b ON a.id=b.buid WHERE b.is_deleted=0 ");
            sqlBuilder.Append("SELECT b.buid,b.licenseno,b.refrenewalstatus FROM bx_batchrefreshrenewal b WHERE b.is_deleted=0 ");
            if (buids.Count > 0 && !buids.Contains(0))
            {
                sqlBuilder.Append(" AND b.buid IN (" + string.Join(",", buids) + ")");
            }
            sqlBuilder.Append(" AND b.operate_agent=@ChildAgent AND b.topagentid=@TopAgentId AND updatetime>=CURDATE() AND b.updatetime=(SELECT MAX(updatetime) FROM bx_batchrefreshrenewal WHERE operate_agent=@ChildAgent AND topagentid=@TopAgentId ORDER BY updatetime DESC)  ");
            MySqlParameter[] sqlParams ={
                                           new MySqlParameter()
                                           {
                                                  ParameterName="@ChildAgent",
                                                  MySqlDbType = MySqlDbType.Int32,
                                                  Value=childAgent
                                            },
                                           new MySqlParameter()
                                           {
                                               ParameterName="@TopAgentId",
                                               MySqlDbType = MySqlDbType.Int32,
                                               Value=topAgentId
                                           }
                                       };
            return DataContextFactory.GetDataContext().Database.SqlQuery<BatchRefreshRenewalModel>(sqlBuilder.ToString(), sqlParams).ToList();
        }

        /// <summary>
        /// 如果【排队中】的数据，则删除
        /// 如果不存在，则直接返回
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool DeleteLineUpRenewal(int buid, int topAgentId, int operateAgent)
        {
            // bx_batchrefreshrenewal model = DataContextFactory.GetDataContext().bx_batchrefreshrenewal.Where(a => a.buid == buid && a.is_deleted == 0 && a.refrenewalstatus == 6).FirstOrDefault();
            //如果存在【排队中】数据，则删除
            //if (model != null && model.id > 0)
            //{
            //    model.is_deleted = 1;                
            //    return DataContextFactory.GetDataContext().SaveChanges() > 0;
            //}
            //如果不存在，则证明进入到【续保中】    
            try
            {
                string sql = "UPDATE bx_batchrefreshrenewal SET is_deleted=1 WHERE buid=" + buid + " AND is_deleted = 0 AND refrenewalstatus =6 AND operate_agent=" + topAgentId + " AND topagentid=" + operateAgent;
                return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql) > 0;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常" + ex);
            }
            return false;
        }

        /// <summary>
        /// 获取前20代理人的第一条数据
        /// </summary>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByAgentList()
        {
            //1.查询数据
            //string sql = "select id,a.agentid from ( select * from bx_batchrefreshrenewal where is_deleted=0 AND refrenewalstatus=6 order by updatetime ) a group by a.agentid limit 20 ;";
            string sql = "select a.* from ( select * from bx_batchrefreshrenewal where is_deleted=0 AND refrenewalstatus=6 order by updatetime ) a group by a.agentid limit 20 ;";
            List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            //将数据更新为续保中
            foreach (var model in list)
            {
                model.refrenewalstatus = 5;
            }
            DataContextFactory.GetDataContext().SaveChanges();
            return list;
        }

        /// <summary>
        /// 获取排队中数据的时间最早的5条记录
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalLimit(int rows)
        {
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE is_deleted=0 AND refrenewalstatus=6 ORDER BY updatetime LIMIT " + rows;
            List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            foreach (var model in list)
            {
                model.refrenewalstatus = 5;
            }
            DataContextFactory.GetDataContext().SaveChanges();
            return list;
        }

        public List<bx_batchrefreshrenewal> GetBatchRefreshRenewalByTimes()
        {
            string sql = "SELECT * FROM bx_batchrefreshrenewal WHERE sendtimes>refreshtimes AND is_deleted=0 LIMIT 5";
            List<bx_batchrefreshrenewal> list = DataContextFactory.GetDataContext().Database.SqlQuery<bx_batchrefreshrenewal>(sql).ToList();
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            foreach (var model in list)
            {
                model.refrenewalstatus = 5;
            }
            DataContextFactory.GetDataContext().SaveChanges();
            return list;
        }
        /// <summary>
        /// 更新该表中续保状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateBatchRefreshRenewalStatus(bx_batchrefreshrenewal model)
        {
            //更新表中状态
            DataContextFactory.GetDataContext().Entry<bx_batchrefreshrenewal>(model).State = System.Data.Entity.EntityState.Modified;
            DataContextFactory.GetDataContext().SaveChanges();
            return true;
        }

        public List<DistributeTimeTagModel> GetDistributedTimeTag(int agentId)
        {
            try
            {
                return (DataContextFactory.GetDataContext().bx_distributedtime.Where(x => (x.AgentId == agentId) && !x.Deleted).Select(x => new DistributeTimeTagModel
                       {
                           Id = x.Id,
                           DistributedStartTime = x.DistributedStartTime,
                           DistributedEndTime = x.DistributedEndTime,

                       })).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("agentId="+agentId+";发生异常：" + ex);
            }
            return new List<DistributeTimeTagModel>();
        }
        public List<CustomerInfoesVM> CustomerInfoes(string mobile, int agentId)
        {

            string selectSql = @"select uri.client_name  as CustomerName ,u.Id as BuId FROM   bx_userinfo  as u
left join bx_userinfo_renewal_info as uri
on u.id = uri.b_uid
where u.agent =?agentId1 AND uri.client_mobile =?client_mobile
UNION
select uri.client_name,u.Id FROM   bx_userinfo as u
left join bx_userinfo_renewal_info as uri
on u.id = uri.b_uid
where u.agent =?agentId2 and
 uri.client_mobile_other = ?client_mobile_other";
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "agentId1",
                    Value = agentId
                },
                   new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "agentId2",
                    Value = agentId
                },
                        new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "client_mobile",
                    Value = mobile
                },
                                      new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "client_mobile_other",
                    Value = mobile
                }

            };
            var result = DataContextFactory.GetDataContext().Database.SqlQuery<CustomerInfoesVM>(selectSql, parameters.ToArray()).ToList();
            return result;
        }

       
    }
}
