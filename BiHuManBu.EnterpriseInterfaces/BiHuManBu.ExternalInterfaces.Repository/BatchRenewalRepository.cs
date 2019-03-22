using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Collections.Generic;
using System.Text;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using System.Data.Entity.Validation;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using log4net;
using System.Data;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization.Json;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class BatchRenewalRepository : IBatchRenewalRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        private ILog logError;
        private ILog logInfo;
        private EntityContext db = DataContextFactory.GetDataContext();
        public bool ExcuteOldBatchrenewalData()
        {
            var time = Convert.ToDateTime("2017-3-21 23:59:59");
            var m = (from b in DataContextFactory.GetDataContext().bx_batchrenewal
                     join a in DataContextFactory.GetDataContext().bx_agent
                     on b.AgentId equals a.Id
                     where b.CreateTime <= time && b.ItemTaskStatus != 2
                     select new
                     {
                         AgentId = b.AgentId,
                         TotalCount = a.BatchRenewalTotalCount
                     }).DistinctBy(x => x.AgentId).ToList();
            foreach (var item in m)
            {
                var n = (from a in DataContextFactory.GetDataContext().bx_batchrenewal_item
                         join c in DataContextFactory.GetDataContext().bx_batchrenewal
                         on a.BatchId equals c.Id
                         join b in DataContextFactory.GetDataContext().bx_userinfo
                         on a.BUId equals b.Id
                         where a.ItemStatus == 0 && b.RenewalStatus == -1 && c.AgentId == item.AgentId && c.ItemTaskStatus != 2 && c.CreateTime <= time
                         orderby a.Id
                         select a).Skip(0).Take(item.TotalCount);
                if (!n.Any())
                {
                    continue;
                }
                foreach (var i in n)
                {
                    i.ItemStatus = -1;
                }

            }
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;

        }

        public bool ResettinStatus(long id)
        {
            DataContextFactory.GetDataContext().bx_batchrenewal_item.SingleOrDefault().ItemStatus = -1;
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        /// <summary>
        /// 根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="buId">userInfoId</param>
        /// <param name="itemStatus">状态</param>
        /// <returns>修改影响行数</returns>
        public int UpdateBatchRenewalItem(int buId, int itemStatus)
        {

            var sql = string.Format("UPDATE bx_batchrenewal_item SET ItemStatus={1},HistoryItemStatus={1},UpdateTime=NOW() WHERE BUId ={0}  AND IsDelete=0  ", buId, itemStatus); ;
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
        }


        /// <summary>
        /// 根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="buId">userInfoId</param>
        /// <param name="itemStatus">状态</param>
        /// <returns>修改影响行数</returns>
        public int UpdateItemStatus(int buId, int itemStatus)
        {
            var sql = string.Format("UPDATE bx_batchrenewal_item SET ItemStatus={1},HistoryItemStatus={1},UpdateTime=NOW() WHERE BUId ={0}  AND IsDelete=0  ", buId, itemStatus); ;
            return DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
        }

        /// <summary>
        /// 软删除数据，将buids对应的IsDelete改成1
        /// </summary>
        /// <param name="buids">要删除的buids</param>
        /// <returns></returns>
        public bool SoftDelete(string buids)
        {
            var sql = "UPDATE bx_batchrenewal_item SET IsDelete=1 WHERE BUId IN (" + buids + ") ";
            DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        /// <summary>
        /// 恢复删除的数据
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool RevokeDelete(int buid)
        {
            var sql = "UPDATE bx_batchrenewal_item SET IsDelete=0 WHERE BUId =?buid";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="buid",
                    Value=buid,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql, param);
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        public bx_batchrenewal_item GetItemByBuId(long buId)
        {
            return DataContextFactory.GetDataContext().bx_batchrenewal_item.Where(x => x.BUId == buId && x.IsDelete == 0 && x.IsNew == 1).FirstOrDefault();
        }
        /// <summary>
        /// 获取所有人
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        public List<int> GetSonsList(int currentAgent)
        {

            var listAgents = new List<int>();
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@curAgent", MySqlDbType.String)
                };
                parameters[0].Value = currentAgent;
                #region SQL语句

                var strSql = new StringBuilder();
                strSql.Append(@"select @curAgent 
union all
select id from  bx_agent where  parentagent=@curAgent
union  all
select id from bx_agent
where  parentagent in (
select id from  bx_agent where  parentagent=@curAgent
) 
union all
select id from bx_agent 
where  parentagent in (
select id from bx_agent
where  parentagent in (
select id from  bx_agent where  parentagent=@curAgent
) )
union  all
select id from  bx_agent
where  parentagent in (
select id from bx_agent 
where  parentagent in (
select id from bx_agent
where  parentagent in (
select id from  bx_agent where  parentagent=@curAgent
) )
)
");
                #endregion
                using (var _dbContext = new EntityContext())
                {
                    //查询列表
                    listAgents = _dbContext.Database.SqlQuery<int>(strSql.ToString(), parameters.ToArray()).ToList();
                }

                return listAgents;
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<int>();
        }
        public IList<CheckBackModel> CheckUserInfo(List<BatchRenewalItemViewModel> batchRenewalItemModelList, string agentId, int renewalCarType, int topagentId)
        {
            int NewagentId = 0;
            if (Convert.ToInt32(_dbHelper.ExecuteScalar("SELECT repeat_quote FROM bx_agent WHERE id=" + topagentId + "")) == 0)
            {
                NewagentId = topagentId;
            }
            else
            {
                NewagentId = int.Parse(agentId);
            }
            var agentIds = GetSonsList(NewagentId);
            //修改
            var querySql = "";
            var licenseNoListNew = batchRenewalItemModelList.Where(x => !string.IsNullOrWhiteSpace(x.LicenseNo)).Select(x => x.LicenseNo).Distinct().ToList();
            var vinNoListNew = batchRenewalItemModelList.Where(x => !string.IsNullOrWhiteSpace(x.VinNo)).Select(x => x.VinNo).Distinct().ToList();
            var carlicenseNo = licenseNoListNew.Select(item => "'" + item + "'").ToList();

            var carVin = vinNoListNew.Select(item => "'" + item + "'").ToList();

            //公共查询SQL
            var selsql = @"select bx_userinfo.Id as BuId,bx_userinfo.carvin as CarVin,bx_userinfo.Agent as Agent,bx_userinfo.EngineNo as EngineNo,bx_userinfo.LicenseNo as LicenseNo,bx_userinfo.IsDistributed as IsDistributed,bx_userinfo.RenewalStatus as RenewalStatus,bx_userinfo.NeedEngineNo as NeedEngineNo,bx_userinfo.LastYearSource as LastYearSource,bx_userinfo.DistributedTime as DistributedTime,bx_userinfo.OpenId as OpenId,bx_userinfo.UpdateTime as UpdateTime,bx_userinfo.CategoryInfoId AS  CategoryInfoId,case when
bx_userinfo.LastYearSource>-1 and bx_userinfo.NeedEngineNo=0 then 1
when bx_userinfo.LastYearSource<=-1 and bx_userinfo.NeedEngineNo=0 then 4 
when bx_userinfo.LastYearSource<=-1 and bx_userinfo.NeedEngineNo!=0 then 2 else 2 end as Status,bx_car_renewal.LastBizEndDate as LastBizEndDate ,bx_car_renewal.LastForceEndDate as LastForceEndDate ,bx_userinfo.RenewalType AS RenewalType,bx_userinfo_renewal_info.remark as Remark,bx_userinfo_renewal_info.intention_remark AS Intention_Remark,bx_userinfo_renewal_info.client_mobile as client_mobile,bx_userinfo_renewal_info.client_mobile_other as client_mobile_other,bx_userinfo_renewal_info.client_name  as client_name,bx_userinfo.RegisterDate AS RegisterDate,bx_userinfo.MoldName AS MoldName,bx_agent.AgentName from bx_userinfo
  LEFT JOIN bx_userinfo_renewal_index ON bx_userinfo.id=bx_userinfo_renewal_index.b_uid 
LEFT JOIN bx_car_renewal ON bx_userinfo_renewal_index.car_renewal_id=bx_car_renewal.Id 
LEFT JOIN bx_userinfo_renewal_info ON bx_userinfo_renewal_info.b_uid=bx_userinfo.id 
LEFT JOIN bx_agent ON bx_agent.Id=bx_userinfo.Agent ";
            //条件sql
            string checkModelSql = "";
            //车牌号与车架号同时存在
            if (carlicenseNo.Any() && carVin.Any())
            {
                checkModelSql = @"
where  (bx_userinfo.LicenseNo in ({0}) OR bx_userinfo.carvin in ({1}) )  and bx_userinfo.Agent in ({2})  And bx_userinfo.IsTest=0  ";
            }
            else if (carlicenseNo.Any() && !carVin.Any())
            {
                checkModelSql = @"
where  bx_userinfo.LicenseNo in ({0}) and bx_userinfo.Agent in ({1})  And bx_userinfo.IsTest=0  ";
            }
            else
            {
                checkModelSql = @"
where bx_userinfo.carvin in ({0}) and bx_userinfo.Agent in ({1})  And bx_userinfo.IsTest=0  ";
            }
            //增加品牌型号查询
            checkModelSql += @" AND bx_userinfo.RenewalCarType=" + renewalCarType + "";

            try
            {
                if (carlicenseNo.Any() && carVin.Any())
                {
                    querySql = string.Format(selsql + checkModelSql, string.Join(",", carlicenseNo), string.Join(",", carVin), string.Join(",", agentIds));

                    var checkBackModels = _dbHelper.ExecuteDataTable(CommandType.Text, querySql, null).ToList<CheckBackModel>();
                    return checkBackModels;
                }
                else if (carlicenseNo.Any() && !carVin.Any())
                {
                    querySql = string.Format(selsql + checkModelSql, string.Join(",", carlicenseNo), string.Join(",", agentIds));
                    var checkBackModels = _dbHelper.ExecuteDataTable(CommandType.Text, querySql, null).ToList<CheckBackModel>();

                    return checkBackModels;
                }
                else
                {
                    querySql = string.Format(selsql + checkModelSql, string.Join(",", carVin), string.Join(",", agentIds));
                    var checkBackModels = _dbHelper.ExecuteDataTable(CommandType.Text, querySql, null).ToList<CheckBackModel>();

                    return checkBackModels;
                }
            }
            catch (Exception)
            {

                LogHelper.Error("筛选老数据异常SQL:" + querySql);
                return null;
            }


        }
        #region 修改EF-sql
        public long InsertBatchRenewal(List<bx_batchrenewal> model)
        {
            //BiHuManBu.ExternalInterfaces.Infrastructure.Helper.Extends.
            return Convert.ToInt64(InsertByListToEf<bx_batchrenewal>(model, DbConfig, "bx_batchrenewal"));
        }
        #endregion
        public bool BulkInsertBatchRenewaErrorlItem(List<bx_batchrenewal_erroritem> batchRenewalErrorItemList)
        {
            return Convert.ToInt32(InsertByListToEf<bx_batchrenewal_erroritem>(batchRenewalErrorItemList, DbConfig, "bx_batchrenewal_erroritem")) != 0;
        }
        /// <summary>
        /// 获取续保时间段
        /// </summary>
        /// <returns></returns>
        public List<string> GetTimeSetting()
        {
            using (var _dbContext = new EntityContext())
            {
                var timeSettingInfo = _dbContext.bx_batchrenewal_time_setting.Select(x => x.TimeScope).ToList();
                return timeSettingInfo;
            }
        }

        public List<BatchRenewalSource> GetCacheBatchRenewalSource(int cityId)
        {
            return null;
            // return _batchRenewalRepository.GetCacheBatchRenewalSource(cityId);

        }
        public string GetFileNameByBatchId(long batchId)
        {
            using (var _dbContext = new EntityContext())
            {
                return _dbContext.bx_batchrenewal.Where(x => x.Id == batchId).Select(x => x.FileName).FirstOrDefault();
                //_dbContext.bx_batchrenewal.SingleOrDefault(x => x.Id == batchId).FileName;
            }
        }
        public IList<DownLoadExcel> GetBatchRenewalTable(long batchRenewalId)
        {
            string downLoadExcelSql = string.Format(@"
                                    SELECT tp.LicenseNo,tp.Name,tp.CustomerName,tp.Mobile1,tp.CategoryInfo,tp.Mobile2,tp.Remark,tp.MoldName,tp.EngineNo,tp.VinNo,
        tp.SalesManName,tp.SalesManAccount,tp.RegisterDate,tp.ItemStatus,tp.InsuredPeopleName,
        tp.IdCard,tp.IsTest,IF(YEAR(IFNULL(tp.ForceRisksEndTime,'1900-01-01')) <= 1970,'',tp.ForceRisksEndTime) AS 'ForceRisksEndTime',
IF(YEAR(IFNULL(tp.BusinessRisksEndTime,'1900-01-01')) <= 1970,'',tp.BusinessRisksEndTime) AS 'BusinessRisksEndTime',IF(tp.IsBatch=1,tp.ItemSource,tp.RenewalSource) AS 'PreSource' FROM (
        SELECT IF(@bd=b_Uid,@ct:=@ct+1,@ct:=1) AS rank, @bd:=b_Uid,tpdata.* FROM (
        SELECT
        uri.b_Uid,uri.create_time,
        CASE WHEN userinfo.LicenseNo=userinfo.CarVIN THEN '' ELSE userinfo.LicenseNo END AS 'LicenseNo',
        userinfo.LicenseOwner AS 'Name',
        item.CustomerName AS 'CustomerName',
        uri.client_mobile AS 'Mobile1',
        bc.CategoryInfo AS 'CategoryInfo',
        uri.client_mobile_other AS 'Mobile2',
        uri.remark  AS 'Remark',
        (CASE  WHEN userinfo.EngineNo='' THEN item.EngineNo  WHEN  userinfo.EngineNo IS NULL THEN item.EngineNo ELSE userinfo.EngineNo END ) AS 'EngineNo',
        (CASE  WHEN userinfo.CarVIN='' THEN item.VinNo  WHEN  userinfo.CarVIN IS NULL THEN item.VinNo ELSE userinfo.CarVIN END ) AS  'VinNo',
        item.SalesManName AS 'SalesManName',
        item.SalesManAccount AS 'SalesManAccount',
        CASE WHEN ABS(TIMESTAMPDIFF(YEAR, userinfo.RegisterDate,NOW()))>100 AND userinfo.RegisterDate!='' THEN '' ELSE DATE_FORMAT( userinfo.RegisterDate,'%Y-%m-%d') END AS 'RegisterDate',
        item.ItemStatus AS 'ItemStatus',
        CASE WHEN item.ItemStatus = 1 THEN rwal.InsuredName ELSE '' END AS 'InsuredPeopleName',
        CASE WHEN item.ItemStatus = 1 THEN CONCAT('\t',rwal.InsuredIdCard) ELSE '' END  AS 'IdCard',
        userinfo.IsTest AS 'IsTest',
        DATE_FORMAT(IF(item.ItemStatus=1,
          IF(YEAR(item.ForceEndDate)<=1970,rwal.LastForceEndDate,
          IF(YEAR(item.ForceEndDate) > (IF(ISNULL(rwal.LastForceEndDate),0,YEAR(rwal.LastForceEndDate))),
            item.ForceEndDate,rwal.LastForceEndDate)),item.ForceEndDate),'%Y-%m-%d') AS 'ForceRisksEndTime',

         DATE_FORMAT(IF(item.ItemStatus=1,
            IF(YEAR(item.BizEndDate)<=1970,rwal.LastBizEndDate,
            IF(YEAR(item.BizEndDate) > (IF(ISNULL(rwal.LastBizEndDate),0,YEAR(rwal.LastBizEndDate))),
              item.BizEndDate,rwal.LastBizEndDate)),item.BizEndDate),'%Y-%m-%d') AS 'BusinessRisksEndTime',
           
         IF(item.ItemStatus=1,
            IF(YEAR(item.BizEndDate)<=1970,0,
            IF(YEAR(item.BizEndDate) > (IF(ISNULL(rwal.LastBizEndDate),0,YEAR(rwal.LastBizEndDate))),
            1,0)),1) AS IsBatch,

        IF(YEAR(item.BizEndDate)<=1970,userinfo.MoldName,
           IF(YEAR(item.BizEndDate) > (IF(ISNULL(rwal.LastBizEndDate),0,YEAR(rwal.LastBizEndDate))),
              item.MoldName,userinfo.MoldName))   AS 'MoldName', 
            cpy.Name AS 'ItemSource',
            brt.Name AS 'RenewalSource'
        FROM bx_batchrenewal_item  AS item
        LEFT JOIN bx_userinfo AS userinfo  ON item.BUId=userinfo.id 
        LEFT JOIN bx_userinfo_renewal_index AS idx ON userinfo.id=idx.b_uid
        LEFT JOIN bx_car_renewal AS rwal ON idx.car_renewal_id=rwal.Id 
        LEFT JOIN bx_companyrelation AS cpy ON cpy.source=item.LastYearSource
        LEFT JOIN bx_companyrelation AS brt ON brt.source=rwal.LastYearSource
        LEFT JOIN bx_userinfo_renewal_info AS uri ON userinfo.id=uri.b_uid
        LEFT JOIN bx_customercategories AS bc ON bc.id= userinfo.CategoryInfoId
        WHERE item.BatchId={0}
        ORDER BY uri.b_Uid,uri.create_time DESC ) AS tpdata,(SELECT @bd:=0,@ct:=0) AS cn) AS tp
        WHERE tp.rank = 1 ", batchRenewalId);

            return _dbHelper.ExecuteDataSet(CommandType.Text, downLoadExcelSql, null).ToList<DownLoadExcel>();
        }

        public int InitUserInfo(List<InitUserInfoModel> updateUserinfoModelList)
        {
            return Convert.ToInt32(BulkUpdateByListToEf<InitUserInfoModel>(updateUserinfoModelList, DbConfig, "bx_userinfo", "Id"));
        }
        public int InitUserInfoNew(List<initUserInfoModelNew> updateUserinfoModelList)
        {
            return Convert.ToInt32(BulkUpdateByListToEf<initUserInfoModelNew>(updateUserinfoModelList, DbConfig, "bx_userinfo", "Id"));
        }
        public int BulkUpdateUserInfo(List<UpdateUserInfoTimeModel> updateUserInfoModel)
        {
            return Convert.ToInt32(BulkUpdateByListToEf(updateUserInfoModel, DbConfig, "bx_userinfo", "Id"));
        }



        /// <summary>
        /// 根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid">userInfoId</param>
        /// <param name="ItemStatus">状态</param>
        /// <returns>修改影响行数</returns>
        public bool UpdateItemStatus(List<long> ids)
        {
            string buidStr = string.Join(",", ids);
            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format("UPDATE  bx_batchrenewal_item SET ItemStatus=0 WHERE  BUID  IN ({0})   AND IsDelete=0   ", string.Join(",", ids));
                return _dbContext.Database.ExecuteSqlCommand(sql) > 0;
            }
        }



        /// <summary>
        /// 批量加入MYSQL数据库
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static object InsertByListToEf<T>(List<T> list, string connectionString, string tableName)
        {
            //CommittableTransaction committran = new CommittableTransaction();  
            int count = 0;
            if (list == null || list.Count <= 0) throw new Exception("List无任何数据");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("添加失败！请先设置插入的表名");
            // 构建INSERT语句
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert into " + tableName + "(");
            Type t = typeof(T);
            foreach (var item in t.GetProperties())
            {
                sb.Append(item.Name + ",");
            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append(") VALUES ");
            foreach (var item in list)
            {

                Type type = item.GetType();
                sb.Append("(");
                foreach (var pi in type.GetProperties())
                {
                    if (type.GetProperty(pi.Name).GetValue(item, null) != null)
                    {

                        if (type.GetProperty(pi.Name).GetValue(item, null).ToString() == "False")
                        {
                            sb.Append("" + 0 + ",");
                        }
                        else if (type.GetProperty(pi.Name).GetValue(item, null).ToString() == "True")
                        {
                            sb.Append("" + 1 + ",");
                        }
                        else
                        {
                            sb.Append("'" + type.GetProperty(pi.Name).GetValue(item, null) + "',");
                        }
                    }
                    else
                    {
                        sb.Append("'" + type.GetProperty(pi.Name).GetValue(item, null) + "',");
                    }

                }
                sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append("),");

            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append(";");
            sb.Append("select @@IDENTITY");

            using (var _dbContext = new EntityContext())
            {
                var id = _dbContext.Database.SqlQuery<long>(sb.ToString()).FirstOrDefault();
                return id;
            }

        }


        public int BulkInsertUserInfo(List<UserInfoModel> userInfoes)
        {

            return Convert.ToInt32(InsertByListToEf<UserInfoModel>(userInfoes, DbConfig, "bx_userinfo"));
        }

        public int BulkInsertBatchRenewalItem(List<BatchRenewalItemModel> batchRenewalItems)
        {

            return Convert.ToInt32(InsertByListToEf<BatchRenewalItemModel>(batchRenewalItems, DbConfig, "bx_batchrenewal_item"));
        }

        #region Sql转Ef
        public static object BulkUpdateByListToEf<T>(IEnumerable<T> enumerable, string connectionString, string tableName, string updateByFiledName) where T : class
        {
            if (enumerable == null || !enumerable.Any()) throw new Exception("List无任何数据");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("添加失败！请先设置插入的表名");
            var sql = PrepareSql(enumerable, updateByFiledName, tableName);
            using (var dbContext = new EntityContext())
            {
                //Count
                var count = dbContext.Database.ExecuteSqlCommand(sql);
                return count;
            }
        }
        #endregion


        private static string PrepareSql<T>(IEnumerable<T> tEnumerable, string updateByFiledName, string updateTableName) where T : class
        {

            var sbUpdateSql = new StringBuilder("Update " + updateTableName + " set");
            var sbWhereSql = new StringBuilder(" Where " + updateByFiledName + " In(");
            var t = typeof(T);
            var properties = t.GetProperties().ToList();
            var updateByFiledIndex = -1;
            var index = -1;
            for (var i = 0; i < properties.Count; i++)
            {
                if (properties[i].Name != updateByFiledName) continue;
                updateByFiledIndex = i;
                break;
            }
            properties.RemoveAt(updateByFiledIndex);
            foreach (var propertie in properties)
            {
                index++;
                sbUpdateSql.Append(" " + propertie.Name + "=case " + updateByFiledName + " ");

                foreach (var item in tEnumerable)
                {
                    if (index == 0)
                    {
                        sbWhereSql.Append("'" + item.GetType().GetProperty(updateByFiledName).GetValue(item, null) + "',");
                    }
                    sbUpdateSql.Append(" when " + item.GetType().GetProperty(updateByFiledName).GetValue(item, null) + " Then '" + item.GetType().GetProperty(propertie.Name).GetValue(item, null) + "'");
                }
                sbUpdateSql.Append(" End ,");
            }
            return sbUpdateSql.ToString().Substring(0, sbUpdateSql.ToString().Length - 1) + " " +
                   sbWhereSql.ToString().Substring(0, sbWhereSql.ToString().Length - 1) + ")";

        }



        /// <summary>
        /// 根据Buid修改批量续保子表状态
        /// </summary>
        /// <param name="Buid">userInfoId</param>
        /// <param name="ItemStatus">状态</param>
        /// <returns>修改影响行数</returns>
        public bool UpdateIsNew(List<long> ids)
        {
            string buidStr = string.Join(",", ids);
            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format("UPDATE  bx_batchrenewal_item SET IsNew=0 WHERE  BUID  IN ({0})", string.Join(",", ids));
                return _dbContext.Database.ExecuteSqlCommand(sql) > 0;
            }
        }

        public int BatchInsertQuotereqCarinfo(List<BatchQuotereqCarinfoModel> batchQuotereqCarinfoModels)
        {
            var addbatchQuotereqCarinfoModels = batchQuotereqCarinfoModels.Select(x => new NewBatchQuotereqCarinfoModel { b_uid = x.b_uid, is_lastnewcar = x.is_lastnewcar, create_time = x.create_time, update_time = x.update_time }).ToList();
            return Convert.ToInt32(InsertByListToEf<NewBatchQuotereqCarinfoModel>(addbatchQuotereqCarinfoModels, DbConfig, "bx_quotereq_carinfo"));
        }

        public int BatchUpdateQuotereqCarinfo(List<BatchQuotereqCarinfoModel> batchQuotereqCarinfoModels)
        {
            var updateMolde = batchQuotereqCarinfoModels.Select(x => new BatchUpdateQuotereqCarinfoModel { b_uid = x.b_uid, is_lastnewcar = x.is_lastnewcar, update_time = x.update_time }).ToList();
            return Convert.ToInt32(BulkUpdateByListToEf<BatchUpdateQuotereqCarinfoModel>(updateMolde, DbConfig, "bx_quotereq_carinfo", "b_uid"));
        }

        public List<long> CheckQuotereqCarinfo(List<long> buids)
        {
            using (var _dbContext = new EntityContext())
            {
                return _dbContext.Database.SqlQuery<long>(string.Format("Select b_uid from bx_quotereq_carinfo where b_uid in ({0})", string.Join(",", buids))).ToList<long>();
                //var aa = _dbContext.bx_quotereq_carinfo.Where(x => buids.Contains(x.b_uid)).Select(x => x.b_uid).ToString();
                //return _dbContext.bx_quotereq_carinfo.Where(x => buids.Contains(x.b_uid)).Select(x => x.b_uid).ToList();

            }
        }


        public List<long> CheckUserRenewalInfo(List<long> buids)
        {

            string buidStr = string.Join(",", buids);
            using (var _dbContext = new EntityContext())
            {
                return _dbContext.Database.SqlQuery<long>(string.Format("select b_uid from bx_userinfo_renewal_info where b_uid in ({0})", string.Join(",", buids))).ToList<long>();
            }
            // return BiHuManBu.StoreFront.Infrastructure.DbHelper.MySqlHelper.ExecuteDataTable(DbConfig, string.Format("select b_uid from bx_userinfo_renewal_info where b_uid in ({0})", string.Join(",", buids)), null).ToList<long>().ToList();
        }


        public int BatchUpdateUserRenewalInfo(List<UserRenewalInfoModel> userRenewalInfoModels)
        {
            return Convert.ToInt32(BulkUpdateByListToEf<UserRenewalInfoModel>(userRenewalInfoModels, DbConfig, "bx_userinfo_renewal_info", "b_uid"));
        }

        public int BulkInsertUserRenewalInfo(List<UserRenewalInfoModel> userRenewalInfoModels)
        {
            return Convert.ToInt32(InsertByListToEf<UserRenewalInfoModel>(userRenewalInfoModels, DbConfig, "bx_userinfo_renewal_info"));
        }
        public bool UpdateBatchRenewal(long batchRenewalId)
        {
            using (var _dbContext = new EntityContext())
            {

                var batchRenewal = _dbContext.bx_batchrenewal.Where(x => x.Id == batchRenewalId).SingleOrDefault();
                batchRenewal.IsCompleted = true;
                return _dbContext.SaveChanges() > 0 ? true : false;
            }
        }


        public class ChannelPattern
        {
            public int ChannelType;
            public List<int> SelectedSources;
        }
        public bool AnewBatchRenewal(long batchRenewalId, int operateType, ChannelPatternModel channelPatternmodel)
        {
            using (var _dbContext = new EntityContext())
            {
                var batchRenewal = _dbContext.bx_batchrenewal.Where(x => x.Id == batchRenewalId).FirstOrDefault();
                if (batchRenewal == null) return false;
                //1.重查  2.修改
                if (operateType == 1)
                {
                    batchRenewal.ChannelPattern = JsonConvert.SerializeObject(channelPatternmodel);
                    batchRenewal.ItemTaskStatus = 1;
                    batchRenewal.IsAnewBatchRenewal = 1;
                    var batchRenewalListItem = _dbContext.bx_batchrenewal_item.Where(x => x.BatchId == batchRenewalId & x.ItemStatus == 2).Select(x => x.BUId).ToList();
                    var batchRenewalBuidItems = _dbContext.bx_batchrenewal_item.Where(x => batchRenewalListItem.Contains(x.BUId));
                    foreach (var item in batchRenewalBuidItems)
                    {
                        item.ItemStatus = 0;
                    }
                    var batchRenewalItem = _dbContext.bx_batchrenewal_item.Where(x => x.BatchId == batchRenewalId & x.ItemStatus == 2);
                    foreach (var item in batchRenewalItem)
                    {
                        item.ItemStatus = -1;
                    }
                }
                else
                {
                    batchRenewal.ChannelPattern = JsonConvert.SerializeObject(channelPatternmodel);
                }
                return _dbContext.SaveChanges() >= 0 ? true : false;

                // #region 重新修改
                //int cityId = batchRenewal.CityId;
                //int topAgentId = batchRenewal.TopAgentId;
                //string batchRenewalData = batchRenewal.ChannelPattern;
                //ChannelPattern channelPattern = new ChannelPattern();
                //List<BatchRenewalSource> sourceAllList = GetBatchRenewalSource(cityId, topAgentId.ToString());
                //List<int> sourceAllId = sourceAllList.Select(x => x.Id).ToList();
                //if (!string.IsNullOrEmpty(batchRenewalData))
                //{
                //    channelPattern = (ChannelPattern)convertObject(batchRenewalData);
                //    List<int> sourceNowList = channelPattern.SelectedSources;

                //    List<int> channelPatternSourceList = sourceAllId.Except(sourceNowList).ToList();
                //    ChannelPattern chPattern = new ChannelPattern();
                //    chPattern.ChannelType = 0;
                //    chPattern.SelectedSources = channelPatternSourceList;
                //    batchRenewal.ChannelPattern = JsonConvert.SerializeObject(chPattern);
                //}
                //else
                //{
                //    ChannelPattern chPattern = new ChannelPattern();
                //    chPattern.ChannelType = 1;
                //    chPattern.SelectedSources = sourceAllId;
                //    batchRenewal.ChannelPattern = JsonConvert.SerializeObject(chPattern);
                //}
                //#endregion
                //batchRenewal.IsAnewBatchRenewal = 1;
                //batchRenewal.ItemTaskStatus = 1;




            }
        }

        /// <summary>
        /// 反序列化 字符串到对象
        /// </summary>
        /// <param name="obj">泛型对象</param>
        /// <param name="str">要转换为对象的字符串</param>
        /// <returns>反序列化出来的对象</returns>
        public static T Desrialize<T>(T obj, string str)
        {
            try
            {
                obj = default(T);
                IFormatter formatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(str);
                MemoryStream stream = new MemoryStream(buffer);
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }

        public ChannelPattern convertObject(string json)
        {
            MemoryStream stream2 = new MemoryStream();
            DataContractJsonSerializer ser2 = new DataContractJsonSerializer(typeof(ChannelPattern));
            StreamWriter wr = new StreamWriter(stream2);
            wr.Write(json);
            wr.Flush();
            stream2.Position = 0;
            Object obj = ser2.ReadObject(stream2);
            ChannelPattern list = (ChannelPattern)obj;
            return list;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool DeteleRenewalData(List<int> BatchrenewalIdList)
        {
            using (var _dbContext = new EntityContext())
            {
                string delSql = string.Format("Delete from  bx_batchrenewal  where ID = {0}", BatchrenewalIdList[0]);
                return _dbContext.Database.ExecuteSqlCommand(delSql) > 0 ? true : false;
            }
        }
        /// <summary>
        /// 删除模板-预留传递泛型也许会批量修改
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool DeleteBatchRenewal(List<int> BatchrenewalIdList)
        {
            string message = string.Empty;
            //删除语句
            int isSuccess = 0;
            string delSql_1 = string.Format("update  bx_batchrenewal set IsDelete=1   where Id = {0}", BatchrenewalIdList[0]);
            isSuccess = _dbHelper.ExecuteNonQuery(CommandType.Text, delSql_1, null);
            if (isSuccess > 0)
            {
                //删除正确关联记录
                string delSql_2 = string.Format("update   bx_batchrenewal_item set IsDelete=1   where BatchId ={0}", BatchrenewalIdList[0]);
                _dbHelper.ExecuteNonQuery(CommandType.Text, delSql_2, null);

                //删除错误关联记录
                string delSql_3 = string.Format("Delete from  bx_batchrenewal_erroritem  where BatchId = {0}", BatchrenewalIdList[0]);
                _dbHelper.ExecuteNonQuery(CommandType.Text, delSql_3, null);
            }
            //判断
            if (isSuccess > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 根据Buid删除bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bool DeleteBatchRenewalItem(int buId)
        {
            //删除
            string delSql = string.Format("update   bx_batchrenewal_item set IsDelete=1 , IsNew=0  where BUId ={0}", buId);
            var isSuccess = _dbHelper.ExecuteNonQuery(CommandType.Text, delSql, null);

            //判断
            if (isSuccess > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 根据Buid恢复bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bool RevertBatchRenewalItem(int buId)
        {
            int isSuccess = 0;
            try
            {
                //查看指定buId所有可用批次Item的Id
                var buidListSql = string.Format("SELECT bx_batchrenewal_item.Id FROM bx_batchrenewal_item INNER JOIN bx_batchrenewal ON bx_batchrenewal_item.BatchId = bx_batchrenewal.Id WHERE	bx_batchrenewal_item.BUId ={0} AND bx_batchrenewal.IsDelete = 0", buId);

                var newbuids = DataContextFactory.GetDataContext().Database.SqlQuery<long>(buidListSql).ToList();
                if (newbuids.Any())
                {
                    var sb = new StringBuilder();
                    //更新可用批次ID最大的记录为最新
                    sb.Append(string.Format("update   bx_batchrenewal_item set IsDelete=0 , IsNew=1  where ID IN ({0}); ", newbuids.Max()));
                    //更新未删除批次的Item为未删除
                    sb.Append(string.Format("UPDATE  bx_batchrenewal_item  SET IsDelete=0  WHERE  ID IN ({0})  ", string.Join(",", newbuids)));
                    isSuccess = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sb.ToString());
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("根据Buid恢复bx_batchrenewal_item表出错：buid:" + buId + "--异常消息:" + ex.Message + "--错误对象名称:" + ex.Source + "--堆栈信息:" + ex.StackTrace);
            }
            //判断
            if (isSuccess > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 根据buid集合恢复bx_batchrenewal_item表
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public bool RevertBatchRenewalItemByBuIdList(List<long> buId)
        {
            int isSuccess = 0;
            try
            {
                if (buId == null || buId.Count == 0) return false;
                var ids = string.Join(",", buId);
                //查看指定buId所有可用批次Item的Id
                var buidListSql = string.Format("SELECT bx_batchrenewal_item.Id FROM bx_batchrenewal_item INNER JOIN bx_batchrenewal ON bx_batchrenewal_item.BatchId = bx_batchrenewal.Id WHERE	bx_batchrenewal_item.BUId in({0}) AND bx_batchrenewal.IsDelete = 0", ids);

                var newbuids = DataContextFactory.GetDataContext().Database.SqlQuery<long>(buidListSql).ToList();
                if (newbuids.Any())
                {
                    var sb = new StringBuilder();
                    //更新可用批次ID最大的记录为最新
                    sb.Append(string.Format("update   bx_batchrenewal_item set IsDelete=0 , IsNew=1  where ID IN ({0}); ", newbuids.Max()));
                    //更新未删除批次的Item为未删除
                    sb.Append(string.Format("UPDATE  bx_batchrenewal_item  SET IsDelete=0  WHERE  ID IN ({0})  ", string.Join(",", newbuids)));
                    isSuccess = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sb.ToString());
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("根据Buid恢复bx_batchrenewal_item表出错：buid:" + buId + "--异常消息:" + ex.Message + "--错误对象名称:" + ex.Source + "--堆栈信息:" + ex.StackTrace);
            }
            //判断
            if (isSuccess > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取批量续保选择续保城市接口
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<BatchRenewalSource> GetBatchRenewalSource(int cityId, string agentId)
        {
            using (var _dbContext = new EntityContext())
            {
                //查询所有数据
                //string getSourceSql = string.Format(" SELECT DISTINCT bx_agent_ukey.source AS Id,bcom.name AS SourceName ,MAX(CASE WHEN bx_agent_ukey.agent_id={1}  THEN 1  ELSE 0  END )  AS  isuse FROM (SELECT * FROM  bx_city_source_irc WHERE irc_code =ANY(SELECT irc_code FROM bx_city_source_irc WHERE citycode={0})) AS a LEFT JOIN  bx_agent_ukey ON a.citycode=bx_agent_ukey.city_id AND bx_agent_ukey.isusedeploy=1 INNER JOIN  bx_companyrelation bcom ON  bx_agent_ukey.source=bcom.source GROUP BY bcom.name,bx_agent_ukey.source   ", cityId, agentId);


                string getSourceSql = string.Format("  select DISTINCT Id,SourceName,max(isuse) isuse,case when isRenewal is null then 0 else isRenewal end as IsRenewal from (  SELECT  DISTINCT bx_agent_ukey.source AS Id,bcom.name AS SourceName ,MAX(CASE WHEN   bx_agent_ukey.agent_id={1} and  bx_agent_ukey.isRenewal in(0,1)  THEN 1  ELSE 0  END )  AS  isuse,bx_agent_ukey.isRenewal  FROM (SELECT * FROM  bx_city_source_irc WHERE irc_code =ANY(SELECT  irc_code FROM bx_city_source_irc WHERE citycode={0})) AS a LEFT JOIN  bx_agent_ukey ON a.citycode=bx_agent_ukey.city_id AND  bx_agent_ukey.isusedeploy=1 INNER JOIN  bx_companyrelation bcom ON  bx_agent_ukey.source=bcom.source  GROUP BY  bcom.name ,bx_agent_ukey.source UNION SELECT DISTINCT bx_agent_ukey.source AS id, bcom.name , MAX(CASE WHEN bx_agent_ukey.agent_id={1} and  bx_agent_ukey.isRenewal in(0,1) THEN 1  ELSE 0  END )  AS  isuse,bx_agent_ukey.isRenewal FROM bx_agent_ukey INNER JOIN  bx_companyrelation bcom ON  bx_agent_ukey.source=bcom.source  WHERE  bx_agent_ukey.city_id={0} GROUP BY bcom.name,bx_agent_ukey.source ) a group by Id,SourceName ", cityId, agentId);


                return _dbHelper.ExecuteDataTable(CommandType.Text, string.Format(getSourceSql)).ToList<BatchRenewalSource>();
                //  return _dbHelper.ExecuteDataTable(CommandType.Text, string.Format(sql, sb.ToString()), ps.ToArray()).ToList<BatchRenewalViewModel>();
            }
        }

        /// <summary>
        /// 获取剩余时间
        /// </summary>
        /// <returns></returns>
        public string GetBatchRenewalQueueTime(int BatchId)
        {
            using (var _dbcontext = new EntityContext())
            {
                int sumBatchCount = 0;
                //获取当前批次的数量
                var nowDataCount = _dbcontext.bx_batchrenewal.Where(x => x.Id == BatchId).Select(x => x.TotalCount).FirstOrDefault();
                //得到全部未执行的数量
                var selCompletedCount = _dbcontext.bx_batchrenewal_item.Where(x => x.ItemStatus == -1 && x.IsDelete == 0).Count();
                //相减
                sumBatchCount = selCompletedCount - Convert.ToInt32(nowDataCount);
                int residueTime = sumBatchCount / 60;
                if (residueTime <= 59)
                {
                    if (residueTime <= 30 && residueTime >= 1)
                    {
                        return "正在排队,预计30min后启动";
                    }
                    else if (residueTime <= 59 && residueTime >= 30)
                    {
                        return "正在排队,预计" + residueTime + "min后启动";
                    }
                    else
                    {
                        return "即将启动";
                    }
                }
                else
                {
                    float fTime = (float)residueTime / 60;
                    return "正在排队,预计" + decimal.Round(decimal.Parse(fTime.ToString()), 1) + " h后启动";
                }
            }
        }

        public int GetAgentBatchRenewalCount(int agentId)
        {

            using (var _dbContext = new EntityContext())
            {
                int excuteCount = 0;
                try
                {
                    var singleOrDefault = _dbContext.bx_agent.SingleOrDefault(x => x.Id == agentId);
                    if (singleOrDefault != null)
                        excuteCount = singleOrDefault.BatchRenewalTotalCount;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("获取业务员执行总数失败：业务员Id:" + agentId + "--异常消息:" + ex.Message + "--错误对象名称:" + ex.Source + "--堆栈信息:" + ex.StackTrace);
                }
                return excuteCount;
            }
        }

        public List<bx_batchrenewal_erroritem> GetBatchRenewalErrorItem(long batchId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var _dbContext = new EntityContext())
            {
                var batchRenewalErrorItemList = _dbContext.bx_batchrenewal_erroritem.Where(x => x.BatchId == batchId);
                totalCount = batchRenewalErrorItemList.Count();
                return batchRenewalErrorItemList.OrderByDescending(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public IList<bx_batchrenewal_erroritem> GetBatchRenewalErrorDataByBathId(long batchId)
        {
            var sql = "SELECT * FROM bx_batchrenewal_erroritem WHERE batchid = ?batchid ORDER BY createtime desc";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="?batchid",
                    Value=batchId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return _dbHelper.ExecuteDataTable(CommandType.Text, string.Format(sql), param).ToList<bx_batchrenewal_erroritem>();
        }

        public int GetSettedCount(int agentId)
        {
            using (var _dbContext = new EntityContext())
            {
                var toDayStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                var toDayEndTime = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00");
                //获取未处理数
                int completedCount = 0;
                try
                {
                    var sql1 = string.Format(@"SELECT IFNULL(SUM(IFNULL(UntreatedCount,0)),0) FROM bx_batchrenewal WHERE IsCompleted = 0 AND BatchRenewalType = 0 AND TopAgentId = {0} AND IsDelete = 0 ", agentId);
                    completedCount = _dbContext.Database.SqlQuery<int>(sql1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("获取未处理数sql1异常:" + ex);
                }

                var disposeCount = 0;
                try
                {
                    var sql2 = string.Format(@"SELECT COUNT(1) FROM (
SELECT item1.id FROM bx_batchrenewal_item item1 
WHERE item1.BatchId IN (SELECT id FROM bx_batchrenewal batch1 WHERE batch1.TopAgentId = {0} AND batch1.IsCompleted=1 AND batch1.BatchRenewalType=0)
AND item1.ItemStatus= -1 AND item1.IsDelete = 0
UNION ALL
SELECT item2.id FROM bx_batchrenewal_item item2 
WHERE item2.BatchId IN (SELECT id FROM bx_batchrenewal batch2 WHERE batch2.TopAgentId = {0} AND batch2.IsCompleted=1 AND batch2.BatchRenewalType=0)
AND item2.ItemStatus=3 AND item2.SendTime IS NOT NULL
UNION ALL
SELECT item3.id FROM bx_batchrenewal_item item3
WHERE item3.BatchId IN (SELECT id FROM bx_batchrenewal batch3 WHERE batch3.TopAgentId = {0} AND batch3.IsCompleted=1 AND batch3.BatchRenewalType=0)
AND item3.ItemStatus IN(0,1,2,4,5) AND (item3.CreateTime >= '{1}' AND item3.CreateTime < '{2}') 
AND item3.SendTime IS NOT NULL) AS t ", agentId, toDayStartTime, toDayEndTime);
                    disposeCount = _dbContext.Database.SqlQuery<int>(sql2).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("获取未处理数sql2异常:" + ex);
                }
                //获取条数
                return disposeCount + completedCount;
            }
        }

        /// <summary>
        /// 查询客户类别
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_customercategories> SelectCategories(int agentId)
        {
            //查询条件代理人+启动+未删除
            using (var _dbContext = new EntityContext())
            {
                var customerInfo = _dbContext.bx_customercategories.Where(x => x.AgentId == agentId && !x.Deleted && x.IsStart == 1).ToList();
                return customerInfo;
            }
            //if (dr != null && dr.Table.Rows.Count > 0)
            //    return Convert.ToInt32(dr.Table.Rows[0]["Id"]);
            //else
            //    return 0;
        }

        public IList<BatchRenewalViewModel> GetBatchRenewalList(BatchRenewalListRequest listRequest, List<bx_agent> agentInfos, out int totalCount)
        {
            //异常数据
            string errorSqlCount = "";
            try
            {

                //  List<int> agentIds = GetSonsList(listRequest.agentId);
                List<int> agentIds = agentInfos.Select(x => x.Id).ToList();
                string sql = @"SELECT CreateTime AS CreateTime,Id,TreateFailedCount AS FailedCount,FileName,StartExecuteTime,TreateSuccessedCount AS SuccessfullCount,TotalCount,UntreatedCount,ItemTaskStatus  AS TaskStatus,IsDistributed,ErrorDataCount,IsCompleted,CASE WHEN (ItemTaskStatus=2 AND TreateFailedCount>0 AND  DATEDIFF(CreateTime,NOW())<7 AND IsAnewBatchRenewal=0) THEN 1  ELSE 0  END AS IsAgainRenewal,CityId as CityId,ChannelPattern as ChannelPattern,BatchRenewalType as BatchRenewalType,FilePath   from  bx_batchrenewal {0}";
                string sqlGetCount = @"select count(1) from bx_batchrenewal  {0}";
                StringBuilder sb = new StringBuilder(string.Format(" where agentid in ({0})   AND IsDelete=0 ", string.Join(",", agentIds)));
                List<MySqlParameter> ps = new List<MySqlParameter>();
                if (!string.IsNullOrEmpty(listRequest.FileName))
                {
                    sb.Append(string.Format(" and FileName like concat('%',?FileName,'%') "));
                    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.VarChar, ParameterName = "FileName", Value = ReplaceSQLChar(listRequest.FileName) });
                }
                //如果已输入车牌号
                if (!string.IsNullOrEmpty(listRequest.licenseNo))
                {
                    sb.Append(string.Format(" and Id IN (select Distinct BatchId from bx_batchrenewal_item where   LicenseNo =?LicenseNo AND IsDelete=0 ) "));
                    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.VarChar, ParameterName = "LicenseNo", Value = ReplaceSQLChar(listRequest.licenseNo) });
                }
                if (!string.IsNullOrEmpty(listRequest.uploadStartTime))
                {
                    sb.Append("  and CreateTime>=?StartCreateTime ");
                    DateTime time = Convert.ToDateTime(listRequest.uploadStartTime);
                    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "StartCreateTime", Value = time });
                }
                if (!string.IsNullOrEmpty(listRequest.uploadEndTime))
                {
                    sb.Append(" and CreateTime<?EndCreateTime");
                    DateTime time = Convert.ToDateTime(listRequest.uploadEndTime).AddDays(1);
                    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.DateTime, ParameterName = "EndCreateTime", Value = time });
                }
                if (listRequest.TaskStatus != -1)
                {
                    sb.Append(" and ItemTaskStatus=?ItemTaskStatus ");
                    ps.Add(new MySqlParameter() { MySqlDbType = MySqlDbType.Int32, ParameterName = "ItemTaskStatus", Value = listRequest.TaskStatus });
                }
                //语句
                errorSqlCount = string.Format(sqlGetCount, sb.ToString());
                totalCount = Convert.ToInt32(_dbHelper.ExecuteScalar(CommandType.Text, errorSqlCount, ps.ToArray()));
                sb.Append(string.Format(" order by CreateTime  desc limit {0},{1}", (listRequest.pageIndex - 1) * listRequest.pageSize, listRequest.pageSize));
                return _dbHelper.ExecuteDataTable(CommandType.Text, string.Format(sql, sb.ToString()), ps.ToArray()).ToList<BatchRenewalViewModel>();

            }
            catch (Exception ex)
            {
                LogHelper.Error("批量续保列表异常SQL：" + listRequest.agentId + "---" + errorSqlCount + "--异常消息:" + ex.Message + "--错误对象名称:" + ex.Source + "--堆栈信息:" + ex.StackTrace);
                totalCount = 0;
                return null;
            }
        }


        /// 过滤SQL字符。
        /// </summary>
        /// <param name="str">要过滤SQL字符的字符串。</param>
        /// <returns>已过滤掉SQL字符的字符串。</returns>
        public static string ReplaceSQLChar(string str)
        {
            if (str == String.Empty)
                return String.Empty;
            str = str.Replace("'", "‘");
            str = str.Replace(";", "；");
            str = str.Replace(",", ",");
            str = str.Replace("?", "?");
            str = str.Replace("<", "＜");
            str = str.Replace(">", "＞");
            str = str.Replace("(", "(");
            str = str.Replace(")", ")");
            str = str.Replace("@", "＠");
            str = str.Replace("=", "＝");
            str = str.Replace("+", "＋");
            str = str.Replace("*", "＊");
            str = str.Replace("&", "＆");
            str = str.Replace("#", "＃");
            str = str.Replace("%", "％");
            str = str.Replace("$", "￥");

            return str;
        }


        /// <summary>
        ///修改成历史状态
        /// </summary>
        /// <param name="BatchrenewalId"></param>
        /// <returns></returns>
        public bool UpdateHistoryStatus(int BatchrenewalId, out List<BatchRenewalUserInfoModel> needUpdateStatus)
        {
            using (var _dbContext = new EntityContext())
            {
                bool isSuccess = false;
                string selectHistorySql = string.Format(" SELECT Id FROM  bx_batchrenewal_item WHERE  BUId IN   (SELECT  Buid  FROM bx_batchrenewal_item   WHERE  BatchId={0})  AND ItemStatus=0 AND IsDelete=0 ", BatchrenewalId);
                List<long> updateHistoryIds = _dbContext.Database.SqlQuery<long>(selectHistorySql).ToList<long>();
                //历史
                if (updateHistoryIds.Any())
                {
                    string updateHistoryItemStatus = string.Format(" UPDATE bx_batchrenewal_item SET ItemStatus=HistoryItemStatus  WHERE Id IN  ({0})", string.Join(",", updateHistoryIds));
                    isSuccess = _dbContext.Database.ExecuteSqlCommand(updateHistoryItemStatus) > 0 ? true : false;
                }
                else
                {
                    isSuccess = true;
                }
                //string historyStatus = string.Format(" SELECT Buid AS Id,(CASE WHEN ItemStatus=1 THEN 1 WHEN ItemStatus=4 THEN 1 WHEN ItemStatus=2 THEN 0 ELSE -1 END ) AS RenewalStatus  FROM  bx_batchrenewal_item WHERE  BUId IN   (SELECT  Buid FROM bx_batchrenewal_item   WHERE  BatchId={0})   AND IsNew=1 AND IsDelete=0 ", BatchrenewalId);
                ////修改状态
                //needUpdateStatus = _dbContext.Database.SqlQuery<BatchRenewalUserInfoModel>(historyStatus).ToList<BatchRenewalUserInfoModel>();
                needUpdateStatus = null;
                return isSuccess;
            }
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="BatchrenewalIdList"></param>
        /// <returns></returns>
        public bool SelectBatchrenewal(List<int> BatchrenewalIdList)
        {
            bool isscuess = false;
            using (var _dbContext = new EntityContext())
            {

                if (BatchrenewalIdList.Any())
                {
                    int batchid = BatchrenewalIdList[0];
                    var buids = _dbContext.bx_batchrenewal_item.Where(x => x.BatchId == batchid).Select(x => x.BUId).ToList();
                    if (buids.Any())
                    {
                        string sql_1 = string.Format("    SELECT MAX(A.ID) AS ID FROM (SELECT BUID,ID,UpdateTime,CreateTime FROM  bx_batchrenewal_item  WHERE  BUId IN ({0}) AND IsDelete=0     ORDER BY UpdateTime DESC ) AS A GROUP BY A.BUID ", string.Join(",", buids));
                        List<long> newbuids = _dbContext.Database.SqlQuery<long>(sql_1).ToList<long>();
                        if (newbuids.Any())
                        {
                            string sql_2 = string.Format("  UPDATE  bx_batchrenewal_item  SET IsNew=1  WHERE  ID IN ({0})  ", string.Join(",", newbuids));
                            isscuess = _dbContext.Database.ExecuteSqlCommand(sql_2) > 0;
                        }

                    }
                    isscuess = true;
                }

                return isscuess;

            }
        }


        public bool TaskUpdateCount(List<long> batchRenewalIdList)
        {

            var isUpdateSuccess = false;
            var sql = string.Empty;
            try
            {
                if (!batchRenewalIdList.Any())
                {
                    return false;
                }
                sql = string.Format(@"      SELECT Temp1.Id  AS Id,
  CASE WHEN IFNULL(SUM(temp2.TreateSuccessedCount),0)<=0 AND IFNULL(SUM(temp2.TreateFailedCount),0)<=0 AND IFNULL(SUM(temp2.UntreatedCount),0)<=0 THEN 0 ELSE COUNT(*) END  AS TotalCount,
 CASE WHEN SUM(temp2.ItemTaskStatus)<=0 THEN 2 ELSE 1 END AS ItemTaskStatus,
 IFNULL(SUM(temp2.TreateSuccessedCount),0) AS TreateSuccessedCount,
 IFNULL(SUM(temp2.TreateFailedCount),0) AS TreateFailedCount,
 IFNULL(SUM(temp2.UntreatedCount),0) AS UntreatedCount
 FROM 
 ( SELECT Id FROM bx_batchrenewal  
WHERE  Id IN ({0})  AND BatchRenewalType=0 )  AS Temp1
LEFT   JOIN 
(
 SELECT 
BatchId,
 CASE WHEN ItemStatus=-1 OR ItemStatus=3 OR ItemStatus=0 THEN 1 ELSE 0 END AS ItemTaskStatus,
 CASE WHEN ItemStatus=1 OR ItemStatus=4 THEN 1 ELSE 0 END AS TreateSuccessedCount,
 CASE WHEN ItemStatus=2  THEN 1 ELSE 0 END AS TreateFailedCount,
 CASE WHEN  ItemStatus=-1 OR ItemStatus=3 OR ItemStatus=0 THEN 1 ELSE 0 END AS UntreatedCount 
 FROM bx_batchrenewal_item   
WHERE  IsDelete=0 AND BatchId IN ({0}) )
 AS temp2  ON temp2.BatchId=Temp1.Id GROUP  BY Temp1.Id      ", string.Join(",", batchRenewalIdList));

                var countResult = _dbHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<UpdateBatchRenewalModel>();
                if (countResult != null && countResult.Any())
                {
                    isUpdateSuccess = Convert.ToInt32(BulkUpdateByListToEf(countResult, DbConfig, "bx_batchrenewal", "Id")) == countResult.Count();
                    LogHelper.Info("task批量更新bx_batchrenewal续保Ids:" + string.Join(",", batchRenewalIdList));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("task批量更新bx_batchrenewal续保Ids:" + string.Join(",", batchRenewalIdList) + "; 错误信息:" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.InnerException + "\n" + sql);
            }
            return isUpdateSuccess;

        }

        public List<long> GetBuidByBatchId(long batchId, int isDelete = 0)
        {
            var sql = "SELECT BUId FROM bx_batchrenewal_item WHERE BatchId=?batchId AND IsDelete=?isDelete ";//AND ItemStatus IN (-1,0,1,2,3,4,5)
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="batchId",
                    Value=batchId,
                    MySqlDbType=MySqlDbType.Int64
                },
                new MySqlParameter
                {
                    ParameterName="isDelete",
                    Value=isDelete,
                    MySqlDbType=MySqlDbType.Int32
                }
            };
            return db.Database.SqlQuery<long>(sql, param).ToList();
        }



        /// <summary>
        /// 获取批续主表个数
        /// </summary>
        /// <returns></returns>
        public int GetBatchRenewalCompleteCount()
        {
            var sql = " SELECT COUNT(*) FROM bx_batchrenewal WHERE IsCompleted = 1 AND  IsDelete = 0 ";
            var param = new MySqlParameter[] { };
            return db.Database.SqlQuery<int>(sql, param).FirstOrDefault();
        }

        /// <summary>
        /// 刷新批续主表统计数
        /// </summary>
        /// <returns></returns>
        public bool RefreshBatchRenewalStatistics()
        {
            var result = false;
            try
            {
                var sql = @"SELECT Temp1.Id  AS Id,
                  CASE WHEN IFNULL(SUM(temp2.TreateSuccessedCount), 0) <= 0 AND IFNULL(SUM(temp2.TreateFailedCount),0)<= 0 AND IFNULL(SUM(temp2.UntreatedCount),0)<= 0 THEN 0 ELSE COUNT(*) END AS TotalCount,
                 CASE WHEN SUM(temp2.ItemTaskStatus) <= 0 THEN 2 ELSE 1 END AS ItemTaskStatus,
                 IFNULL(SUM(temp2.TreateSuccessedCount), 0) AS TreateSuccessedCount,
                  IFNULL(SUM(temp2.TreateFailedCount), 0) AS TreateFailedCount,
                   IFNULL(SUM(temp2.UntreatedCount), 0) AS UntreatedCount
                 FROM
                 (SELECT Id FROM bx_batchrenewal
                WHERE BatchRenewalType = 0 AND UntreatedCount > 0 AND IsDelete = 0 )  AS Temp1
                LEFT JOIN
                (
                 SELECT
                BatchId,
                 CASE WHEN ItemStatus= -1 OR ItemStatus = 3 OR ItemStatus = 0 THEN 1 ELSE 0 END AS ItemTaskStatus,
                 CASE WHEN ItemStatus = 1 OR ItemStatus = 4 THEN 1 ELSE 0 END AS TreateSuccessedCount,
                 CASE WHEN ItemStatus = 2  THEN 1 ELSE 0 END AS TreateFailedCount,
                 CASE WHEN  ItemStatus = -1 OR ItemStatus = 3 OR ItemStatus = 0 THEN 1 ELSE 0 END AS UntreatedCount
                       FROM bx_batchrenewal_item
                      WHERE  IsDelete = 0)
                 AS temp2  ON temp2.BatchId = Temp1.Id GROUP BY Temp1.Id  ";
                var countResult = _dbHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<UpdateBatchRenewalModel>();
                if (countResult != null && countResult.Any())
                {
                    result = Convert.ToInt32(BulkUpdateByListToEf(countResult, DbConfig, "bx_batchrenewal", "Id")) == countResult.Count();
                    LogHelper.Info("定时任务批量刷新批续主表统计数量结果：" + result);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Info("定时任务批量刷新批续主表统计数量异常：" + ex);
                return result;
            }   
        }

        /// <summary>
        /// 更新到期时间
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool UpdateBatchRenewalItemByBuid(int buid, long source, string ForceEndTime,string BizEndTime)
        {
            try
            {
                List<MySqlParameter> sqlParams = new List<MySqlParameter>();
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("UPDATE bx_batchrenewal_item SET LastYearSource=" + source);
                if (!string.IsNullOrEmpty(ForceEndTime))
                {
                    sqlBuilder.Append(" ,ForceEndDate='"+ForceEndTime+"'");
                }
                if (!string.IsNullOrEmpty(BizEndTime))
                {
                    sqlBuilder.Append(" ,BizEndDate='"+BizEndTime+"'");
                }
                
                sqlBuilder.Append(" WHERE buid="+buid+" AND IsDelete=0");
                
                return db.Database.ExecuteSqlCommand(sqlBuilder.ToString()) > 0;
            }
            catch (Exception ex)
            {
                logError.Error("BUID=" + buid + ",source=" + source + ";发生异常：" + ex);
            }
            return false;
        }

    }
}