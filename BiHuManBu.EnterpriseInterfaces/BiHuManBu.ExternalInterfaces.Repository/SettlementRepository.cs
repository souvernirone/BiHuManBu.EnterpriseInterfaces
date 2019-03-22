using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SettlementRepository : ISettlementRepository
    {
        readonly MySqlHelper _mySqlHelper;
        public SettlementRepository()
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zb"].ConnectionString);
        }
        public bool AddUnSettlementRange(List<UnSettlementRequestVM> unSettlementListVM)
        {
            var data = ToDataTable(unSettlementListVM, "js_unsettlement");
            return _mySqlHelper.BulkInsert(data) > 0;
        }

        public Tuple<bool, string> CreateSettlement(List<int> ids, int settleType)
        {
            var selectSql = string.Empty;


            if (settleType != 5)
            {
                if (settleType == 1)
                {

                    selectSql = string.Format(@"select min(swingcarddate) as SwingCardStartDate , max(swingcarddate) as SwingCardEndDate, DataInAgentName,DataInAgentId,ParentAgentName,ParentAgentId,sum(Price) as SettlePrice ,{0} as SettleType,CompanyName from js_unsettlement where Id in ({1}) group by DataInAgentId", settleType, string.Join(",", ids));
                }
                else
                {
                    selectSql = string.Format(@"select min(swingcarddate) as SwingCardStartDate , max(swingcarddate) as SwingCardEndDate, ParentAgentName,ParentAgentId,sum(Price) as SettlePrice ,{0} as SettleType,CompanyName from js_unsettlement where Id in ({1}) group by ParentAgentId", settleType, string.Join(",", ids));
                }
            }
            else
            {

                selectSql = string.Format(@"select min(swingcarddate) as SwingCardStartDate , max(swingcarddate) as SwingCardEndDate,CompanyId,ChannelName,ChannelId,sum(Price) as SettlePrice ,{0} as SettleType,CompanyName,TopAgentId as ParentAgentId,DataInAgentId   from js_unsettlement where id in ({1}) group by ChannelId", settleType, string.Join(",", ids));
            }

            var selectResult = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectSql, null).ToT<SettlementInsertedModel>();
            if (selectResult == null)
            {
                return new Tuple<bool, string>(false, "无任何可添加的待结算单");
            }
            var ps = new List<MySqlParameter>()
            {
                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="CreateTime",Value=DateTime.Now},
                  new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="UpdateTime",Value=DateTime.Now},
                   new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="SwingCardStartDate",Value=selectResult.SwingCardStartDate},
                  new MySqlParameter{ MySqlDbType=MySqlDbType.DateTime,ParameterName="SwingCardEndDate",Value=selectResult.SwingCardEndDate},

                 new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="DataInAgentId",Value=selectResult.DataInAgentId},
                  new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="ParentAgentId",Value=selectResult.ParentAgentId},

                      new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="DataInAgentName",Value=selectResult.DataInAgentName},
                  new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="ParentAgentName",Value=selectResult.ParentAgentName},

                      new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="ChannelId",Value=selectResult.ChannelId},
                  new MySqlParameter { MySqlDbType=MySqlDbType.Double,ParameterName="SettlePrice",Value=selectResult.SettlePrice},

                      new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="SettleType",Value=settleType},


                        new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="CompanyId",Value=selectResult.CompanyId},
                  new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="ChannelName",Value=selectResult.ChannelName},


                                 new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="ReconciliationCount",Value=ids.Count},
                  new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="SettleStatus",Value=0},


                                 new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="ReconciliationStatus",Value=0},
                  new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="ReceiptStatus",Value=0},
                    new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="BackPriceStatus",Value=0},
                          new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="CompanyName",Value=selectResult.CompanyName}



            };
            var insertSql = @"insert into js_settlement(CreateTime,UpdateTime,SwingCardStartDate,SwingCardEndDate,DataInAgentId,ParentAgentId,DataInAgentName,ParentAgentName,ChannelId,SettlePrice,SettleType,CompanyId,ChannelName,ReconciliationCount,SettleStatus,ReconciliationStatus,ReceiptStatus,BackPriceStatus,CompanyName)values(?CreateTime,?UpdateTime,?SwingCardStartDate,?SwingCardEndDate,?DataInAgentId,?ParentAgentId,?DataInAgentName,?ParentAgentName,?ChannelId,?SettlePrice,?SettleType,?CompanyId,?ChannelName,?ReconciliationCount,?SettleStatus,?ReconciliationStatus,?ReceiptStatus,?BackPriceStatus,?CompanyName);select @@IDENTITY;";
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {

                    var batchId = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, insertSql, ps.ToArray()));
                    var updateSql = string.Format("update js_unsettlement set batchId={0} where id in({1})", batchId, string.Join(",", ids));
                    var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSql, null) > 0;
                    scope.Complete();
                    return new Tuple<bool, string>(isSuccess, "成功创建结算单");
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }

        }

        public List<SettlementResponseVM> GetSettlementListAboutCompany(List<int> channelIds, List<int> agentIds, SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice, out int settleCount)
        {
            var selectDataSql = new StringBuilder(@"SELECT Id,DATE_FORMAT(CreateTime,'%Y-%m-%d %H:%i') as CreateTime,SwingCardStartDate,SwingCardEndDate,CompanyId,ChannelId,CompanyName,ChannelName,SettlePrice,ReconciliationCount,ReconciliationStatus,ReceiptStatus,BackPriceStatus FROM js_settlement");

            var selectWhereSql = new StringBuilder(string.Format(" where  ChannelId in({0})  and ParentAgentId in({1}) and Deleted=0", string.Join(",", channelIds), string.Join(",", agentIds)));
            var selectCountAndPriceSql = new StringBuilder("select ifnull(count(1),0) as Count,ifnull(sum(CASE WHEN settlestatus=0 THEN settleprice ELSE 0 END),0) as Price,ifnull(sum(CASE WHEN settlestatus=0 THEN 1 ELSE 0 END),0) as settleCount from js_settlement");
            var ps = new List<MySqlParameter>();
            if (settlementListRequestVM.CreateStartTime.HasValue)
            {
                selectWhereSql.Append("  and CreateTime>=?CreateStartTime");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "CreateStartTime",
                    Value = settlementListRequestVM.CreateStartTime.Value
                });
            }

            if (settlementListRequestVM.CreateEndTime.HasValue)
            {
                selectWhereSql.Append("  and CreateTime<?CreateEndTime");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "CreateEndTime",
                    Value = settlementListRequestVM.CreateEndTime.Value.AddDays(1)
                });
            }
            if (settlementListRequestVM.SwingCardStartDate.HasValue)
            {
                selectWhereSql.Append("  and SwingCardStartDate>=?SwingCardStartDate");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "SwingCardStartDate",
                    Value = settlementListRequestVM.SwingCardStartDate.Value
                });
            }

            if (settlementListRequestVM.SwingCardEndDate.HasValue)
            {
                selectWhereSql.Append("  and SwingCardEndDate<?SwingCardEndDate");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "SwingCardEndDate",
                    Value = settlementListRequestVM.SwingCardEndDate.Value.AddDays(1)
                });
            }

            if (settlementListRequestVM.ReconciliationStatus != -1)
            {
                selectWhereSql.Append("  and ReconciliationStatus=?ReconciliationStatus");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ReconciliationStatus",
                    Value = settlementListRequestVM.ReconciliationStatus
                });
            }
            if (settlementListRequestVM.ReceiptStatus != -1)
            {
                selectWhereSql.Append("  and ReceiptStatus=?ReceiptStatus");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ReceiptStatus",
                    Value = settlementListRequestVM.ReceiptStatus
                });
            }

            if (settlementListRequestVM.BackPriceStatus != -1)
            {
                selectWhereSql.Append("  and BackPriceStatus=?BackPriceStatus");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "BackPriceStatus",
                    Value = settlementListRequestVM.BackPriceStatus
                });
            }
            var pageIndex = (settlementListRequestVM.PageIndex - 1) * settlementListRequestVM.PageSize;
            selectDataSql.Append(selectWhereSql.ToString()).Append(string.Format(" order by UpdateTime desc limit {0},{1}", pageIndex, settlementListRequestVM.PageSize));
            selectCountAndPriceSql.Append(selectWhereSql.ToString());
            var dataRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectCountAndPriceSql.ToString(), ps.ToArray());
            totalCount = Convert.ToInt32(dataRow["Count"]);
            totalPrice = Convert.ToDouble(dataRow["Price"]);
            settleCount = Convert.ToInt32(dataRow["settleCount"]);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, selectDataSql.ToString(), ps.ToArray()).ToList<SettlementResponseVM>().ToList();

        }
        public List<SettlementResponseVM> GetSettlementListNotAboutCompany(List<int> agentIds, SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice, out int settleCount)
        {
            var selectDataSql = new StringBuilder(@"SELECT Id,DATE_FORMAT(CreateTime,'%Y-%m-%d %H:%i') as CreateTime,SwingCardStartDate,SwingCardEndDate,DataInAgentName,CompanyName,ChannelId,ParentAgentName,SettleStatus,SettlePrice,SettleDate FROM js_settlement ");
            var selectWhereSql = new StringBuilder("  where Deleted=0 and SettleType=?SettleType");
            var selectCountAndPriceSql = new StringBuilder("select ifnull(count(1),0) as Count,ifnull(sum(CASE WHEN settlestatus=0 THEN settleprice ELSE 0 END),0) as Price,ifnull(sum(case when settlestatus =0 then 1 else 0 end  ),0) as settleCount from js_settlement");
            var ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "SettleType", Value = settlementListRequestVM.SettleType } };
            if (settlementListRequestVM.SettleType == 1)
            {
                selectWhereSql.Append(string.Format(" and  DataInAgentId IN({0}) ", string.Join(",", agentIds)));
            }
            else
            {
                selectWhereSql.Append(string.Format(" and  ParentAgentId IN({0}) ", string.Join(",", agentIds)));
            }
            if (settlementListRequestVM.CreateStartTime.HasValue)
            {
                selectWhereSql.Append("  and CreateTime>=?CreateStartTime");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "CreateStartTime",
                    Value = settlementListRequestVM.CreateStartTime.Value
                });
            }

            if (settlementListRequestVM.CreateEndTime.HasValue)
            {
                selectWhereSql.Append("  and CreateTime<?CreateEndTime");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "CreateEndTime",
                    Value = settlementListRequestVM.CreateEndTime.Value.AddDays(1)
                });
            }
            if (settlementListRequestVM.SwingCardStartDate.HasValue)
            {
                selectWhereSql.Append("  and SwingCardStartDate>=?SwingCardStartDate");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "SwingCardStartDate",
                    Value = settlementListRequestVM.SwingCardStartDate.Value
                });
            }

            if (settlementListRequestVM.SwingCardEndDate.HasValue)
            {
                selectWhereSql.Append("  and SwingCardEndDate<?SwingCardEndDate");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "SwingCardEndDate",
                    Value = settlementListRequestVM.SwingCardEndDate.Value.AddDays(1)
                });
            }

            if (settlementListRequestVM.SettleStatus != -1)
            {
                selectWhereSql.Append("  and SettleStatus=?SettleStatus");
                ps.Add(new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "SettleStatus",
                    Value = settlementListRequestVM.SettleStatus
                });
            }
            var pageIndex = (settlementListRequestVM.PageIndex - 1) * settlementListRequestVM.PageSize;
            selectDataSql.Append(selectWhereSql.ToString()).Append(string.Format(" order by UpdateTime desc limit {0},{1}", pageIndex, settlementListRequestVM.PageSize));
            selectCountAndPriceSql.Append(selectWhereSql.ToString());
            var dataRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectCountAndPriceSql.ToString(), ps.ToArray());
            totalCount = Convert.ToInt32(dataRow["Count"]);
            totalPrice = Convert.ToDouble(dataRow["Price"]);
            settleCount = Convert.ToInt32(dataRow["settleCount"]);
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, selectDataSql.ToString(), ps.ToArray()).ToList<SettlementResponseVM>().ToList();

        }
        public List<UnSettlementResponseVM> GetUnSettlementListAboutCompany(List<int> channelIds, int pageIndex, int pageSize, List<int> agentIds, out int totalCount, out double totalPrice, out int reconciliationCount, DateTime? swingCardStartDate = default(DateTime?), DateTime? swingCardEndDate = default(DateTime?))
        {
            try
            {

                var selectDataSql = new StringBuilder(@"select  Id, DATE_FORMAT(SwingCardDate,'%Y-%m-%d') as SwingCardDate,Licenseno, ReconciliationNum,UserName,InsuranceType,CompanyId, Price,CompanyName,case when  InsuranceType=1 then '交强险' else '商业险' end as InsuranceName,ChannelName,InsurancePrice ,TaxPrice,DataInAgentName,ParentAgentName,DATE_FORMAT(CatchSingleTime,'%Y-%m-%d %H:%i') as CatchSingleTime,GuId as Guid  from js_unsettlement  ");
                var selectCountAndPriceSql = new StringBuilder(@"select ifnull(count(1),0) as Count,ifnull(sum(Price),0) as Price  from js_unsettlement");
                var selectReconciliationCountSql = new StringBuilder(@"select count(t.guid) from (select guid from js_unsettlement ");
                var selectWhereSql = new StringBuilder(string.Format("  where ChannelId in ({0}) and TopAgentId IN({1}) and batchid = -1 and AgentType_ZB=?AgentType_ZB", string.Join(",", channelIds), string.Join(",", agentIds)));
                var ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "AgentType_ZB", Value = 5 } };
                if (swingCardStartDate.HasValue)
                {

                    selectWhereSql.Append("  and SwingCardDate>=?swingCardStartDate");

                    ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "swingCardStartDate", Value = swingCardStartDate.Value });
                }

                if (swingCardEndDate.HasValue)
                {
                    selectWhereSql.Append(" and SwingCardDate<?swingCardEndDate");
                    ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "swingCardEndDate", Value = swingCardEndDate.Value.AddDays(1) });
                }
                pageIndex = (pageIndex - 1) * pageSize;
                selectDataSql.Append(selectWhereSql.ToString()).Append(string.Format(" order by CatchSingleTime desc limit {0},{1}", pageIndex, pageSize));
                selectCountAndPriceSql.Append(selectWhereSql.ToString());
                selectReconciliationCountSql.Append(selectWhereSql.ToString()).Append(" group by GuId ) t");
                var dataRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectCountAndPriceSql.ToString(), ps.ToArray());
                totalCount = Convert.ToInt32(dataRow["Count"]);
                totalPrice = Convert.ToDouble(dataRow["Price"]);
                reconciliationCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, selectReconciliationCountSql.ToString(), ps.ToArray()));
                return _mySqlHelper.ExecuteDataTable(CommandType.Text, selectDataSql.ToString(), ps.ToArray()).ToList<UnSettlementResponseVM>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
        public List<UnSettlementResponseVM> GetUnSettlementListNotAboutCompany(int searchAgentType, List<int> agentIds, int pageIndex, int pageSize, out int totalCount, out double totalPrice, out int reconciliationCount, DateTime? swingCardStartDate = default(DateTime?), DateTime? swingCardEndDate = default(DateTime?))
        {
            try
            {

                var selectDataSql = new StringBuilder(@"select Id,DATE_FORMAT(SwingCardDate,'%Y-%m-%d') as SwingCardDate,Licenseno,ReconciliationNum,UserName,InsuranceType,CompanyName,case when  InsuranceType=1 then '交强险' else '商业险' end as InsuranceName,CompanyId,ChannelName, InsurancePrice, TaxPrice,DataInAgentName,ParentAgentName,Price,DATE_FORMAT(CatchSingleTime,'%Y-%m-%d %H:%i') as CatchSingleTime ,GuId as Guid  from js_unsettlement ");
                var selectCountAndPriceSql = new StringBuilder(@"select ifnull(count(1),0) as Count,ifnull(sum(Price),0) as Price  from js_unsettlement ");

                var selectReconciliationCountSql = new StringBuilder(@"select count(t.guid) from (select guid from js_unsettlement ");
                var selectWhereSql = new StringBuilder(@" where  BatchId=-1 ");
                if (searchAgentType == 1)
                {
                    selectWhereSql.Append(string.Format(" and DataInAgentId in({0})  and AgentType_ZB=?agenttype_zb", string.Join(",", agentIds)));
                }
                else
                {
                    selectWhereSql.Append(string.Format(" and ParentAgentId in({0}) and AgentType_ZB=?agenttype_zb", string.Join(",", agentIds)));
                }

                var ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "agenttype_zb", Value = searchAgentType } };
                if (swingCardStartDate.HasValue)
                {
                    selectWhereSql.Append("  and SwingCardDate>=?swingCardStartDate");
                    ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "swingCardStartDate", Value = swingCardStartDate.Value });
                }
                if (swingCardEndDate.HasValue)
                {
                    selectWhereSql.Append("  and SwingCardDate<?swingCardEndDate");
                    ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "swingCardEndDate", Value = swingCardEndDate.Value.AddDays(1) });
                }
                pageIndex = (pageIndex - 1) * pageSize;
                selectCountAndPriceSql.Append(selectWhereSql.ToString());
                selectReconciliationCountSql.Append(selectWhereSql.ToString()).Append(" group by GuId ) t");
                var dataRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectCountAndPriceSql.ToString(), ps.ToArray());

                totalCount = Convert.ToInt32(dataRow["Count"]);
                totalPrice = Convert.ToDouble(dataRow["Price"]);


                selectDataSql.Append(selectWhereSql.ToString()).Append(string.Format(" order by CatchSingleTime desc limit {0},{1}", pageIndex, pageSize));
                reconciliationCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, selectReconciliationCountSql.ToString(), ps.ToArray())); ;
                return _mySqlHelper.ExecuteDataTable(CommandType.Text, selectDataSql.ToString(), ps.ToArray()).ToList<UnSettlementResponseVM>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public bool RollBackSettlementList(int batchId, List<int> SettleIds, out int rollbackCount)
        {

            var updateSettlementSql = string.Empty;
            var updateUnSettlementSql = string.Empty;
            var selectSettelCountSql = string.Format(@"select  ReconciliationCount from js_settlement where id={0}", batchId);
            var settelCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, selectSettelCountSql, null));

            var selectGuidSql = string.Format(@"select guid from js_unsettlement where id in({0})", string.Join(",", SettleIds));
            var guids = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectGuidSql, null).ToList<string>().ToList();

            var selectPriceAndCountSql = string.Format(@"select ifnull(sum(Price),0) as rollbackprice,count(id) as rollbackcount   from js_unsettlement as unstemp1 where unstemp1.guid in('{0}') and BatchId={1};", string.Join(",", guids), batchId);
            var settleRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectPriceAndCountSql, null);
            var rollbackPrice = Convert.ToDouble(settleRow["rollbackprice"]);
            rollbackCount = Convert.ToInt32(settleRow["rollbackcount"]);
            var deleted = rollbackCount == settelCount ? 1 : 0;
            updateSettlementSql = string.Format(@"update js_settlement set SettlePrice=SettlePrice-{0},ReconciliationCount=ReconciliationCount-{1},Deleted={2},UpdateTime=now() where id ={3}", rollbackPrice, rollbackCount, deleted, batchId);
            updateUnSettlementSql = string.Format(@"update js_unsettlement set BatchId=-1,UpdateTime=now() where  guid In('{0}') and batchid={1}", string.Join(",", guids), batchId);
            using (TransactionScope scope = new TransactionScope())
            {
                var isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSettlementSql, null) > 0;
                isSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateUnSettlementSql, null) > 0;
                scope.Complete();
                return isSuccess;
            }

        }

        public bool UpdateStaus(SettlementUpdateStatusVM settlementUpdateStatusVM, out List<string> guids)
        {
            var updateSettlementSql = string.Empty;
            var addReceiptOrbackPriceSql = string.Empty;

            List<MySqlParameter> ps = new List<MySqlParameter>();
            var idsStr = string.Join(",", settlementUpdateStatusVM.Ids);
            var selectGuidsSql = new StringBuilder(string.Format(@"select GuId as Guid from js_unsettlement where BatchId in({0})", idsStr));
            if (settlementUpdateStatusVM.UpdateStatusType == 1)
            {
                updateSettlementSql = string.Format(@"update js_settlement set SettleStatus=1,SettleDate='{0}',UpdateTime=now() where id in({1}) and SettleStatus=0 ", settlementUpdateStatusVM.SettlementStatus.SettleTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), idsStr);



            }
            else if (settlementUpdateStatusVM.UpdateStatusType == 2)
            {
                updateSettlementSql = string.Format(@"update js_settlement set ReconciliationStatus=1,UpdateTime=now() where id in({0})", idsStr);

            }
            else if (settlementUpdateStatusVM.UpdateStatusType == 3)
            {
                updateSettlementSql = string.Format(@"update js_settlement set ReceiptStatus=1,UpdateTime=now() where id in({0})", idsStr);
                addReceiptOrbackPriceSql = string.Format(@"insert into js_receiptinfo(ReceiptTitle,ReceiptNum,ReceiptPrice,ReceiptDate,CreateTime,UpdateTime,BatchId) values(?ReceiptTitle,?ReceiptNum,?ReceiptPrice,?ReceiptDate,?CreateTime,?UpdateTime,?BatchId)");

                ps.AddRange(new List<MySqlParameter> {
                    new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="ReceiptTitle",Value=settlementUpdateStatusVM.ReceiptStatus.ReceiptTitle},
                    new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="ReceiptNum",Value=settlementUpdateStatusVM.ReceiptStatus.ReceiptNum},
                    new MySqlParameter { MySqlDbType=MySqlDbType.Double,ParameterName="ReceiptPrice",Value=settlementUpdateStatusVM.ReceiptStatus.ReceiptPrice},
                         new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="ReceiptDate",Value=settlementUpdateStatusVM.ReceiptStatus.ReceiptDate},
                                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="CreateTime",Value=DateTime.Now},
                                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="UpdateTime",Value=DateTime.Now},
                                     new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="BatchId",Value=settlementUpdateStatusVM.Ids[0]}

                });
            }
            else if (settlementUpdateStatusVM.UpdateStatusType == 4)
            {
                updateSettlementSql = string.Format(@"update js_settlement set BackPriceStatus=1,UpdateTime=now() where id in({0})", idsStr);
                addReceiptOrbackPriceSql = string.Format(@"insert into js_backpriceinfo(Payer,BackPrice,BackPriceDate,CreateTime,UpdateTime,BatchId) values(?Payer,?BackPrice,?BackPriceDate,?CreateTime,?UpdateTime,?BatchId)");

                ps.AddRange(new List<MySqlParameter> {
                    new MySqlParameter { MySqlDbType=MySqlDbType.VarChar,ParameterName="Payer",Value=settlementUpdateStatusVM.BackPriceStatus.Payer},

                    new MySqlParameter { MySqlDbType=MySqlDbType.Double,ParameterName="BackPrice",Value=settlementUpdateStatusVM.BackPriceStatus.BackPrice},
                         new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="BackPriceDate",Value=settlementUpdateStatusVM.BackPriceStatus.BackPriceDate},
                                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="CreateTime",Value=DateTime.Now},
                                new MySqlParameter { MySqlDbType=MySqlDbType.DateTime,ParameterName="UpdateTime",Value=DateTime.Now},
                                     new MySqlParameter { MySqlDbType=MySqlDbType.Int32,ParameterName="BatchId",Value=settlementUpdateStatusVM.Ids[0]}

                });
            }





            using (TransactionScope scope = new TransactionScope())
            {
                var isSuccuss = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSettlementSql, null) >= 0;
                guids = _mySqlHelper.ExecuteDataTable(CommandType.Text, selectGuidsSql.ToString(), null).ToList<string>().ToList();
                if (!string.IsNullOrWhiteSpace(addReceiptOrbackPriceSql))
                {
                    isSuccuss = _mySqlHelper.ExecuteNonQuery(CommandType.Text, addReceiptOrbackPriceSql, ps.ToArray()) > 0;


                }

                scope.Complete();
                return isSuccuss;
            }

        }
        public Tuple<bool, string> AddToSettlementList(int batchId, List<int> unSettleIds)
        {
            var selectUnsettleSql = string.Format(@"select ifnull(sum(Price),0) as Price,ifnull( count(id),0)as Count,min(SwingCardDate)  as  SwingCardMinDate,max(SwingCardDate) as SwingCardMaxDate from js_unsettlement where id in({0})", string.Join(",", unSettleIds));


            var dataUnsettleRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectUnsettleSql, null);
            var unSettleCount = Convert.ToInt32(dataUnsettleRow["Count"]);
            if (unSettleCount <= 0)
            {
                return new Tuple<bool, string>(false, "无任何可添加的待结算单");
            }
            var settlePriceAdded = Convert.ToDouble(dataUnsettleRow["Price"]);
            var swingCardMinDate = Convert.ToDateTime(dataUnsettleRow["SwingCardMinDate"]);
            var swingCardMaxDate = Convert.ToDateTime(dataUnsettleRow["SwingCardMaxDate"]);
            var selectSettleSql = string.Format(@"select SwingCardStartDate,SwingCardEndDate from js_settlement where id={0}", batchId);
            var dataSettleRow = _mySqlHelper.ExecuteDataRow(CommandType.Text, selectSettleSql, null);
            var swingCardStartDate = Convert.ToDateTime(dataSettleRow["SwingCardStartDate"]);
            var SwingCardEndDate = Convert.ToDateTime(dataSettleRow["SwingCardEndDate"]);
            if (swingCardMinDate == swingCardMaxDate)
            {
                if (swingCardMinDate < swingCardStartDate)
                {
                    swingCardStartDate = swingCardMinDate;
                }
                if (swingCardMinDate > SwingCardEndDate)
                {
                    SwingCardEndDate = swingCardMinDate;
                }
            }
            else
            {
                if (swingCardMinDate < swingCardStartDate)
                {
                    swingCardStartDate = swingCardMinDate;
                }
                if (swingCardMaxDate > SwingCardEndDate)
                {
                    SwingCardEndDate = swingCardMaxDate;
                }
            }
            var ps = new List<MySqlParameter> { new MySqlParameter { MySqlDbType = MySqlDbType.Double, ParameterName = "SettlePrice", Value = settlePriceAdded }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "ReconciliationCount", Value = unSettleCount }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "SwingCardStartDate", Value = swingCardStartDate }, new MySqlParameter { MySqlDbType = MySqlDbType.DateTime, ParameterName = "SwingCardEndDate", Value = SwingCardEndDate }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "Id", Value = batchId } };
            var updateSettelSql = @"update  js_settlement set SettlePrice=SettlePrice+?SettlePrice,ReconciliationCount=ReconciliationCount+?ReconciliationCount,SwingCardStartDate=?SwingCardStartDate, SwingCardEndDate=?SwingCardEndDate where id =?Id";
            var updateUnSettelSql = string.Format(@"update js_unsettlement set BatchId={0} where id in({1})", batchId, string.Join(",", unSettleIds));
            using (TransactionScope scope = new TransactionScope())
            {
                var updateSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateSettelSql, ps.ToArray()) > 0;
                updateSuccess = _mySqlHelper.ExecuteNonQuery(CommandType.Text, updateUnSettelSql, null) > 0;
                scope.Complete();
                return new Tuple<bool, string>(true, "成功添加到结算单");
            }

        }

        private DataTable ToDataTable<T>(IEnumerable<T> collection, string tableName)
        {

            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.TableName = tableName;
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        public bool CheckSameData(List<int> ids, int settleType, int batchId)
        {
            var selectCountSql = string.Empty;
            if (batchId == -1)
            {
                if (settleType == 1)
                {
                    selectCountSql = string.Format(@"select count(DISTINCT( DataInAgentId)) from js_unsettlement where  id in({0})", string.Join(",", ids));
                }
                else if (settleType == 2 || settleType == 3 || settleType == 4)
                {
                    selectCountSql = string.Format(@"select count(DISTINCT( ParentAgentId)) from  js_unsettlement where  id in({0})", string.Join(",", ids));
                }
                else
                {
                    selectCountSql = string.Format(@"select count(DISTINCT(ChannelId)) from  js_unsettlement where  id in({0})", string.Join(",", ids));
                }
            }
            else
            {
                if (settleType == 1)
                {
                    selectCountSql = string.Format(@"SELECT count(1) FROM js_unsettlement AS us WHERE us.id IN({0}) AND  NOT EXISTS  (SELECT  DataInAgentId FROM js_settlement AS s WHERE  id={1} AND us.DataInAgentId=s.DataInAgentId);", string.Join(",", ids), batchId);
                }
                else if (settleType == 2 || settleType == 3 || settleType == 4)
                {
                    selectCountSql = string.Format(@"SELECT count(1) FROM js_unsettlement AS us WHERE us.id IN({0}) AND  NOT EXISTS  (SELECT  ParentAgentId FROM js_settlement AS s WHERE  id={1} AND us.ParentAgentId=s.ParentAgentId);", string.Join(",", ids), batchId);
                }
                else
                {
                    selectCountSql = string.Format(@"SELECT count(1) FROM js_unsettlement AS us WHERE us.id IN({0}) AND  NOT EXISTS  (SELECT  ChannelId FROM js_settlement AS s WHERE  id={1} AND us.ChannelId=s.ChannelId);", string.Join(",", ids), batchId);
                }
            }
            try
            {


                var isHas = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, selectCountSql, null)) > 1 ? false : true;
                return isHas;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        /// <summary>
        /// 获取结算单明细
        /// </summary>
        /// <param name="batchId">批次编号</param>
        /// <returns></returns>

        public List<UnSettlementResponseVM> GetSettleListDetail(int batchId, string licenseNo, string reconciliationNum, out int totalCount, out int reconciliationCount, int pageIndex = 1, int pageSize = 10)
        {

            var selectDataSql = new StringBuilder(@"select Id,DATE_FORMAT(SwingCardDate,'%Y-%m-%d') as SwingCardDate,Licenseno,ReconciliationNum,UserName,InsuranceType,ChannelId,ChannelName,InsurancePrice,Price,SettleRate,case when  InsuranceType=1 then '交强险' else '商业险' end as InsuranceName,TaxPrice, InsurancePrice,DataInAgentName,ParentAgentName,DATE_FORMAT(CatchSingleTime,'%Y-%m-%d %H:%i') as CatchSingleTime,CompanyId,CompanyName,Guid from js_unsettlement");
            var selectCountSql = new StringBuilder(@"select count(1) from js_unsettlement ");
            var selectWhereSql = new StringBuilder(@"  where BatchId=?batchId");

            var selectReconciliationCountSql = new StringBuilder(@"select count(t.guid) from (select guid from js_unsettlement ");
            var ps = new List<MySqlParameter>() { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "batchId", Value = batchId } };
            if (licenseNo.ToUpper() != "-1")
            {
                selectWhereSql.Append(" and Licenseno=?Licenseno");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "Licenseno", Value = licenseNo });
            }
            if (reconciliationNum.ToUpper() != "-1")
            {
                selectWhereSql.Append(" and ReconciliationNum=?ReconciliationNum");
                ps.Add(new MySqlParameter { MySqlDbType = MySqlDbType.VarChar, ParameterName = "ReconciliationNum", Value = reconciliationNum });
            }
            selectReconciliationCountSql.Append(selectWhereSql.ToString()).Append(" group by GuId) t");
            selectDataSql.Append(selectWhereSql.ToString()).Append(" order by id limit ?pageIndex,?pageSize");
            selectCountSql.Append(selectWhereSql.ToString());
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text,
         selectCountSql.ToString(), ps.ToArray()));
            reconciliationCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, selectReconciliationCountSql.ToString(), ps.ToArray()));
            ps.AddRange(new List<MySqlParameter> { new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageIndex", Value = (pageIndex - 1) * pageSize }, new MySqlParameter { MySqlDbType = MySqlDbType.Int32, ParameterName = "pageSize", Value = pageSize } });
            return _mySqlHelper.ExecuteDataTable(CommandType.Text, selectDataSql.ToString(), ps.ToArray()).ToList<UnSettlementResponseVM>().ToList();
        }
    }
}
