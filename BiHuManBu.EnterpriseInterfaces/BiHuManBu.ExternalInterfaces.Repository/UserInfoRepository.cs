using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using System.Configuration;
using System.Transactions;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private ILog logError;
        private EntityContext _context;

        /// <summary>
        /// app端用查询userinfo，由于无openid限制，故3个条件不用openid查
        /// 顶级代理下的车辆有可能重复，以最新的一条记录来处理，即使查看老数据，也展示最新的一条数据
        /// </summary>
        /// <param name="licenseno"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public bx_userinfo FindByAgentLicense(string licenseno, string agent)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {
                tt = DataContextFactory.GetDataContext().bx_userinfo.OrderByDescending(o => o.UpdateTime.HasValue ? o.UpdateTime : o.CreateTime).ThenByDescending(o => o.CreateTime).FirstOrDefault(x => x.LicenseNo == licenseno && x.Agent == agent && x.OpenId.Length > 9);// && (!x.IsSingleSubmit.HasValue)
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tt;
        }

        public UserInfoRepository()
        {
            logError = LogManager.GetLogger("ERROR");
            _context = DataContextFactory.GetDataContext();
        }

        /// <summary>
        /// 下级代理人数量超过两千的顶级代理人Id
        /// </summary>
        private static int[] HasMoreThan2000ChildAgentTopAgentId = new[] { 8031, 2668, 4245, 12457 };

        /// <summary>
        /// 强制索引并且转换代理人的的TopAgentId
        /// HasMoreThan2000ChildAgentTopAgentId中剩余的是：不用强制索引，不转换代理人的
        /// </summary>
        private static int[] ForceAndConvertTopAgentId = new[] { 8031, 2668 };

        private const string NO_CONVERT_AGNET = " bx_agent.Id ";

        private const string CONVERT_AGNET = " CONVERT( bx_agent.Id , CHAR) ";

        private readonly string SqlHasPlaceholderNoIndexHasJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui ON ui.Id = uri.b_uid
                    {1}  
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                    INNER JOIN
                bx_agent ON ui.Agent = {0}
                {2}
            WHERE
                1 = 1
            ";

        //private readonly string SqlHasPlaceholderNoIndexNoJoinAgent = @"
        //    FROM
        //        bx_car_renewal cr
        //            RIGHT JOIN
        //        bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
        //            RIGHT JOIN
        //        bx_userinfo ui ON ui.Id = uri.b_uid
        //            {0}  
        //            LEFT JOIN
        //        bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
        //            AND bx_batchrenewal_item.IsNew = 1
        //            AND bx_batchrenewal_item.IsDelete = 0
        //    WHERE
        //        1 = 1
        //    ";

        private readonly string SqlNoPlaceholderNoIndexHasJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui ON ui.Id = uri.b_uid
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                    INNER JOIN
                bx_agent ON ui.Agent = {0}
                {1}
            WHERE
                1 = 1
            ";

        //private readonly string SqlNoPlaceholderNoIndexNoJoinAgent = @"
        //    FROM
        //        bx_car_renewal cr
        //            RIGHT JOIN
        //        bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
        //            RIGHT JOIN
        //        bx_userinfo ui ON ui.Id = uri.b_uid
        //            LEFT JOIN
        //        bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
        //            AND bx_batchrenewal_item.IsNew = 1
        //            AND bx_batchrenewal_item.IsDelete = 0
        //    WHERE
        //        1 = 1
        //    ";


        /// <summary>
        /// 之所以此处不加ui.IsTest=0 是因为之前拼接where语句时有一处区分了istest
        /// 顶级获取列表sql
        /// </summary>
        public readonly string StrSqlNoJoin = @" FROM bx_userinfo ui  WHERE 1=1  ";

        public readonly string SqlNoPlaceholderHasIndexHasJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                    INNER JOIN
                bx_agent ON ui.Agent = {0}
                {1}
            WHERE
                1 = 1
            ";

        public readonly string SqlNoPlaceholderHasIndexNoJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                {0}
            WHERE
                1 = 1
            ";

        public readonly string SqlNoPlaceholderHasIndexNoJoinAgentFillInformation = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0

                LEFT JOIN bx_consumer_review ON ui.Id=bx_consumer_review.b_uid 
                LEFT JOIN bx_defeatreasonsetting ON bx_consumer_review.DefeatReasonId=bx_defeatreasonsetting.Id
            WHERE
                1 = 1
            ";



        public readonly string SqlHasPlaceholderHasIndexHasJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                    {1}        
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                    INNER JOIN
                bx_agent ON ui.Agent = {0}
                {2}
            WHERE
                1 = 1
            ";

        public readonly string SqlHasPlaceholderHasIndexNoJoinAgent = @"
            FROM
                bx_car_renewal cr
                    RIGHT JOIN
                bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id
                    RIGHT JOIN
                bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid
                    {0}        
                    LEFT JOIN
                bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId
                    AND bx_batchrenewal_item.IsNew = 1
                    AND bx_batchrenewal_item.IsDelete = 0
                   {1}
            WHERE
                1 = 1
            ";

        public readonly string StrBusiSqlNoJoin = @" FROM bx_userinfo ui {0} WHERE 1=1  ";

        /// <summary>
        /// 业务员条件存在下次回访时间
        /// </summary>
        public readonly string strVisitTimeSql =
            " LEFT JOIN bx_consumer_review  as creview on ui.Id =creview.b_uid and creview.id=(select max(id) from bx_consumer_review where b_uid=ui.id) ";

        /// <summary>
        /// 关联bx_userinfo_renewal_info获取客户名称
        /// 齐大康 2018-04-17
        /// </summary>
        public readonly string strUserRenewalInfo = " LEFT JOIN bx_userinfo_renewal_info ON ui.id=bx_userinfo_renewal_info.b_uid ";

        /// <summary>
        /// bx_userinfo_expand扩展表
        /// 2018-06-13
        /// </summary>
        private readonly string UserInfoExpandSql = " LEFT JOIN bx_userinfo_expand ue ON ui.Id=ue.b_uid AND ue.delete_type>-1 ";

        /// <summary>
        /// bx_userinfo_expand扩展表 刘松年
        /// 2018-08-20
        /// </summary>
        private readonly string UserInfoExpandSqlNotDelete = " LEFT JOIN bx_userinfo_expand ue ON ui.Id=ue.b_uid AND ue.delete_type=-1 ";
        /// <summary>
        /// 车主询价
        /// 2018-07-18
        /// </summary>
        private readonly string ExpandOwnerInquirySql = " INNER JOIN bx_userinfo_expand bue ON bue.b_uid =ui.Id AND bue.CarOwnerStatus=1 ";
        /// <summary>
        /// 摄像头配置
        /// 2018-07-18
        /// </summary>
        private readonly string CameraConfigSql = " LEFT JOIN bx_camera_config cc ON cc.camera_id=ue.CameraId ";


        #region 一些通用的方法
        /// <summary>
        /// 生成页码参数
        /// </summary>
        /// <param name="curPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private MySqlParameter[] GeneratePageParameter(int curPage, int pageSize)
        {
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@pagebegin", MySqlDbType.Int32),
                    new MySqlParameter("@pageend", MySqlDbType.Int32)
                };
            parameters[0].Value = (curPage - 1) * pageSize;
            parameters[1].Value = pageSize;
            return parameters;
        }

        /// <summary>
        /// 要返回模型的字段
        /// sql语句最终要输入的字段
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetOutsideField()
        {
            var builder = new StringBuilder();
            builder.Append("SELECT ui.Id,ui.LicenseNo,ui.OpenId,ui.CityCode,s.MoldName AS MoldName,ui.LicenseOwner,ui.InsuredName,ui.RegisterDate,")
                .Append("ui.CreateTime,ui.UpdateTime,ui.RenewalType,ui.IsCamera,")
                .Append("IF(s.IsBatch=1,s.LastYearSource,ui.LastYearSource) AS LastYearSource,ui.NeedEngineNo,ui.RenewalStatus,ui.QuoteStatus,")
                .Append(" s.ValueLastForceEndDate,")
                .Append(" s.ValueLastBizEndDate,")
                .Append("ui.agent,ui.IsSingleSubmit,CASE ui.IsTest WHEN 1 THEN 3 ELSE ui.IsDistributed END AS IsDistributed,ui.IsReView,")
                .Append(" s.LastForceEndDate,s.LastBizEndDate  ")
                .Append(" ,ui.CameraTime,ui.DistributedTime  ")
                .Append(" ,ui.CategoryInfoId ");
            return builder;
        }

        /// <summary>
        /// inner join中的字段
        /// 子查询中的字段
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetInnerJoinField()
        {
            var builder = new StringBuilder();

            builder.Append(string.Format("SELECT ui.Id,{0} as LastForceEndDate ,{1} as LastBizEndDate ",
                CompareBatchAndRenewalDateHelpler.GetLastForceEndDate(),
                CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                .Append(string.Format(",{0} AS IsBatch ", CompareBatchAndRenewalDateHelpler.GetIsBatch()))
                .Append(",bx_batchrenewal_item.LastYearSource ")
                .Append(string.Format(",{0} AS ValueLastForceEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)))
                .Append(string.Format(",{0} AS ValueLastBizEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)))
                .Append(string.Format(", {0} AS MoldName ", CompareBatchAndRenewalDateHelpler.GetMoldName()));
            return builder;
        }

        /// <summary>
        /// 根据条件装配成最终的sql
        /// </summary>
        /// <param name="outSideSql">外面的sql</param>
        /// <param name="innerJoinSql">子查询里面的sql</param>
        /// <param name="orderBy"></param>
        private void GenerateSql(ref StringBuilder outSideSql, StringBuilder innerJoinSql, int orderBy)
        {
            // 获取排序信息
            GetSqlOrderBy(ref innerJoinSql, orderBy);
            outSideSql.Append(" FROM  bx_userinfo  AS ui ");
            outSideSql.Append(@" INNER JOIN  (  ");
            outSideSql.Append(innerJoinSql)
                .Append(" ) AS s ON ui.id=s.id");
            // 最后排序
            GetSqlOrderByTwo(ref outSideSql, orderBy);
        }
        /// <summary>
        /// 根据SearchCustomerListDto
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private StringBuilder GenerateSearchSql(SearchCustomerListDto search)
        {
            var builder = new StringBuilder();
            if (search == null)
                return builder;
            #region bx_userinfo.IsReview
            if (search.IsReviewHashSet.Any())
            {
                var isReview = string.Join(",", search.IsReviewHashSet);
                builder.Append(string.Format(" AND ui.IsReview in ({0}) ", isReview));
            }
            #endregion

            #region agent
            // 只有代理人数量小于2000时才用in，否则用关联bx_agent
            if (!search.HasMoreThan2000ChildAgent)
            {
                if (search.ListAgent.Any())
                {
                    var agent = string.Join("','", search.ListAgent);
                    builder.Append(string.Format(" AND ui.agent in ('{0}') ", agent));
                }
                else
                {
                    // 某些搜索添加回导致agen为空，这时bx_userinfo将不会走agent的索引，导致全表扫描，所以设置agent='-1'
                    builder.Append(" AND ui.agent ='-1' ");
                }
            }
            #endregion

            return builder;
        }

        #endregion 一些通用的方法

        #region new 列表与总条数分离

        public List<GetCustomerViewModel> FindCustomerListJoinConsumerReview(SearchCustomerListDto search)
        {
            // 页码参数
            var parameters = GeneratePageParameter(search.CurPage, search.PageSize);

            var selectSqlPart = GetWhereSql(search);

            // 最外层的sql
            var sqlSelect = GetOutsideField();

            // inner join 里面的查询 
            var sqlList = GetInnerJoinField();

            // 需要从关联的表中查出那些字段
            sqlSelect.Append(" ,s.NextReviewDate,s.LastReviewTime,s.LastReviewContent ");
            sqlSelect.Append(" ,s.ClientName,ui.CarVIN,s.Organization ");//客户名称: 2018-04-17
            sqlList.Append(" ,bx_userinfo_renewal_info.client_name AS ClientName "); //本行代码: 2018-04-17
            sqlList.Append(" ,creview.next_review_date as NextReviewDate,creview.create_time as LastReviewTime, creview.content as LastReviewContent,cr.Organization ");

            sqlList.Append(selectSqlPart);

            GenerateSql(ref sqlSelect, sqlList, search.OrderBy);
            //LogHelper.Info("客户列表SQL脚本：" + sqlSelect.ToString());
            // 查询列表
            var listuserinfo = _context.Database.SqlQuery<GetCustomerViewModel>(sqlSelect.ToString(), parameters).ToList();
            return listuserinfo;
        }

        public List<GetCustomerViewModel> FindCustomerList(SearchCustomerListDto search)
        {
            // 页码参数
            var parameters = GeneratePageParameter(search.CurPage, search.PageSize);

            #region SQL语句
            // 获取from后面的sql
            var selectSqlPart = GetWhereSql(search);

            // 列表查询字段
            var sqlSelect = GetOutsideField();

            // inner join 里面的查询 
            var sqlList = GetInnerJoinField();

            // 需要从关联的表中查出那些字段
            if (search.JoinType == 1)
            {
                sqlSelect.Append(" ,NextReviewDate");
                sqlList.Append(",creview.next_review_date as NextReviewDate ");
            }
            if (search.IsJoinExpandCameraConfig == 1)
            {
                sqlSelect.Append(" ,s.CameraId").Append(" , s.CameraName");
                sqlList.Append(string.Format(", {0} AS CameraId ", "cc.camera_id")).Append(string.Format(", {0} AS CameraName ", "cc.CameraName"));
            }
            sqlList.Append(selectSqlPart);

            GenerateSql(ref sqlSelect, sqlList, search.OrderBy);
            #endregion

            //查询列表
            var listuserinfo = _context.Database.SqlQuery<GetCustomerViewModel>(sqlSelect.ToString(), parameters).ToList();
            return listuserinfo;
        }

        public GetCustomerViewModel FindCustomerModel(int buid)
        {
            GetCustomerViewModel model = new GetCustomerViewModel();
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append(@" SELECT ui.Id,ui.LicenseNo,ui.OpenId,ui.CityCode,ui.LicenseOwner,ui.InsuredName,ui.RegisterDate
                                     ,ui.CreateTime,ui.UpdateTime,ui.RenewalType
                                     ,ui.NeedEngineNo,ui.RenewalStatus,ui.QuoteStatus,ui.agent");
                sqlBuilder.Append(string.Format(",{0} as LastForceEndDate ,{1} as LastBizEndDate ",
               CompareBatchAndRenewalDateHelpler.GetLastForceEndDate(),
               CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
               .Append(string.Format(",{0} AS LastYearSource ", CompareBatchAndRenewalDateHelpler.GetLastYearSource()))               
               .Append(string.Format(",{0} AS ValueLastForceEndDate ",
                   CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)))
               .Append(string.Format(",{0} AS ValueLastBizEndDate ",
                   CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)))
               .Append(string.Format(", {0} AS MoldName ", CompareBatchAndRenewalDateHelpler.GetMoldName()));
                sqlBuilder.Append(string.Format(SqlNoPlaceholderHasIndexNoJoinAgent, ""));
                sqlBuilder.Append(string.Format(" and ui.id={0} AND ui.IsTest=0;",buid));
                model = _context.Database.SqlQuery<GetCustomerViewModel>(sqlBuilder.ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return model;
        }

        /// <summary>
        /// 摄像头进店列表导出
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="joinWhere"></param>
        /// <param name="orderBy"></param>
        /// <param name="isRec"></param>
        /// <returns></returns>
        public List<GetCustomerViewModel> FindCustomerListForCameraExport(SearchCustomerListDto search)
        {
            try
            {
                #region SQL语句

                var selectSqlPart = GetWhereSql(search);
                //GetJoinSql(joinType, sqlWhere, joinWhere);

                //声明sql查询列表sql串
                var sqlList = new StringBuilder();
                //列表查询字段
                var sqlSelect = new StringBuilder();
                sqlSelect.Append("SELECT ui.Id,ui.LicenseNo,ui.OpenId,ui.CityCode,IF(s.IsBatch=1,s.MoldName,ui.MoldName) AS MoldName,ui.LicenseOwner,ui.InsuredName,ui.RegisterDate,")
                    .Append("ui.RenewalStatus,ui.EngineNo,ui.CarVIN,ui.InsuredIdCard,")
                    .Append("ui.CreateTime,ui.UpdateTime,ui.RenewalType,ui.IsCamera,")
                    .Append("IF(s.IsBatch=1,s.LastYearSource,ui.LastYearSource) AS LastYearSource,ui.NeedEngineNo,ui.RenewalStatus,ui.QuoteStatus,")
                    .Append(" s.ValueLastForceEndDate,")
                    .Append(" s.ValueLastBizEndDate,")
                    .Append("ui.agent,ui.IsSingleSubmit,CASE ui.IsTest WHEN 1 THEN 3 ELSE ui.IsDistributed END AS IsDistributed,ui.IsReView,")
                    .Append(" s.LastForceEndDate,s.LastBizEndDate  ")
                    .Append(" ,ui.CameraTime,ui.DistributedTime  ")
                    .Append(",ui.CategoryInfoId");
                if (search.IsJoinExpandCameraConfig == 1)
                {
                    sqlSelect.Append(" ,s.CameraId , s.CameraName");
                }
                //inner join 里面的查询
                sqlList.Append(" SELECT ui.Id,bx_batchrenewal_item.MoldName,bx_batchrenewal_item.LastYearSource");
                if (search.IsJoinExpandCameraConfig == 1)
                {
                    sqlList.Append("  ,cc.camera_id AS CameraId , cc.CameraName AS CameraName ");
                }

                sqlList.Append(string.Format(",{0} as LastForceEndDate", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()))
                      .Append(string.Format(",{0} as LastBizEndDate", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                      .Append(string.Format(",{0} AS IsBatch", CompareBatchAndRenewalDateHelpler.GetIsBatch()))
                      .Append(string.Format(",{0} AS ValueLastForceEndDate", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)))
                      .Append(string.Format(",{0} AS ValueLastBizEndDate ", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)));
                //业务员
                if (search.JoinType == 1)
                {
                    sqlSelect.Append(" ,NextReviewDate");
                    sqlList.Append(",creview.next_review_date as NextReviewDate ");
                }
                sqlList.Append(selectSqlPart);

                switch (search.OrderBy)
                {
                    case 2: //交强降
                        sqlList.Append(string.Format(
                            " ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,{0} DESC,Id DESC ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                        break;
                    case 21: //交强升
                        sqlList.Append(string.Format(
                            " ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,{0} ASC,Id DESC ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                        break;
                    case 3: //商业降
                        sqlList.Append(string.Format(
                            " ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,{0} DESC,Id DESC", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                        break;
                    case 31: //商业升
                        sqlList.Append(string.Format(
                            " ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,{0} ASC,Id DESC ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                        break;
                    case 4: //回访时间降
                        //业务员，取回访时间
                        sqlList.Append(" ORDER BY NextReviewDate DESC,ui.UpdateTime DESC,Id DESC ");
                        break;
                    case 41: //回访时间升
                        //业务员，取回访时间
                        sqlList.Append(" ORDER BY NextReviewDate ASC,ui.UpdateTime DESC,Id DESC ");
                        break;
                    case 5://摄像头进店时间 降序
                        sqlList.Append(" order by ui.CameraTime desc,ui.UpdateTime DESC,ui.Id DESC ");
                        break;
                    case 51://摄像头进店时间 升序
                        sqlList.Append(" order by ui.CameraTime ASC,ui.UpdateTime DESC,ui.Id DESC limit ");
                        break;
                    case 6://分配时间 降序
                        sqlList.Append(" order by ui.DistributedTime desc,ui.UpdateTime DESC,ui.Id DESC limit ");
                        break;
                    case 61://分配时间 升序
                        sqlList.Append(" order by ui.DistributedTime ASC,ui.UpdateTime DESC,ui.Id DESC limit ");
                        break;
                    default: //更新时间降
                        sqlList.Append(" ORDER BY ui.UpdateTime DESC,Id DESC ");
                        break;
                }
                sqlSelect.Append(" FROM  bx_userinfo  AS ui ");
                sqlSelect.Append(@" INNER JOIN  (  ");
                sqlSelect.Append(sqlList)
                    .Append(" ) AS s ON ui.id=s.id");
                //最后排序
                GetSqlOrderByTwo(ref sqlSelect, search.OrderBy);
                #endregion
                //查询列表
                var listuserinfo =
                    _context.Database.SqlQuery<GetCustomerViewModel>(sqlSelect.ToString()).ToList();
                return listuserinfo;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<GetCustomerViewModel>();
        }
        /// <summary>
        /// 获取摄像头进店导出列表总数
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="joinWhere"></param>
        /// <param name="orderBy"></param>
        /// <param name="isRec"></param>
        /// <returns></returns>
        public DistributedCountViewModel GetExportCount(SearchCustomerListDto search)
        {
            try
            {
                #region SQL语句
                //sql语句拼接判断
                var selectSqlPart = GetWhereSql(search);
                //GetJoinSql(joinType, sqlWhere, joinWhere);
                //声明sql查询列表sql串
                var sqlList = new StringBuilder();
                //列表查询字段
                var sqlSelect = new StringBuilder();
                sqlSelect.Append(" SELECT COUNT(1) as TotalCount,COUNT(IF(IsDistributed>0,TRUE,NULL)) as DistributedCount ");
                //inner join 里面的查询
                sqlList.Append(" SELECT ui.Id,IF(ISNULL(cr.LastBizEndDate)=1,bx_batchrenewal_item.ForceEndDate,IF(bx_batchrenewal_item.BizEndDate>(IF(ISNULL(cr.LastBizEndDate),'',cr.LastBizEndDate)),bx_batchrenewal_item.ForceEndDate,cr.LastForceEndDate)) as LastForceEndDate ,IF(ISNULL(cr.LastBizEndDate)=1,bx_batchrenewal_item.BizEndDate,IF(bx_batchrenewal_item.BizEndDate>(IF(ISNULL(cr.LastBizEndDate),'',cr.LastBizEndDate)),bx_batchrenewal_item.BizEndDate,cr.LastBizEndDate)) as LastBizEndDate, ")
                    .Append("IF(ISNULL(cr.LastBizEndDate)=1,1,IF(bx_batchrenewal_item.BizEndDate>cr.LastBizEndDate,1,0)) AS IsBatch,bx_batchrenewal_item.MoldName,bx_batchrenewal_item.LastYearSource,")
                    .Append("IF(YEAR(IF(ISNULL(cr.LastBizEndDate)=1,bx_batchrenewal_item.BizEndDate,IF(bx_batchrenewal_item.BizEndDate>(IF(ISNULL(cr.LastBizEndDate),'',cr.LastBizEndDate)),bx_batchrenewal_item.ForceEndDate,cr.LastForceEndDate)))!='0001',1,0) AS ValueLastForceEndDate, ")
                    .Append("IF(YEAR(IF(ISNULL(cr.LastBizEndDate)=1,bx_batchrenewal_item.BizEndDate,IF(bx_batchrenewal_item.BizEndDate>(IF(ISNULL(cr.LastBizEndDate),'',cr.LastBizEndDate)),bx_batchrenewal_item.BizEndDate,cr.LastBizEndDate)))!='0001',1,0) AS ValueLastBizEndDate ");
                //业务员
                if (search.JoinType == 1)
                {
                    sqlSelect.Append(" ,NextReviewDate");
                    sqlList.Append(",creview.next_review_date as NextReviewDate ");

                }
                sqlList.Append(selectSqlPart);

                sqlSelect.Append(" FROM  bx_userinfo  AS ui ");
                sqlSelect.Append(@" INNER JOIN  (  ");
                sqlSelect.Append(sqlList)
                    .Append(" ) AS s ON ui.id=s.id");
                #endregion
                return _context.Database.SqlQuery<DistributedCountViewModel>(sqlSelect.ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new DistributedCountViewModel();
        }

        public DistributedCountViewModel FindCustomerCountContainDistributedCount(SearchCustomerListDto search)
        {
            //sql语句拼接判断
            var countSqlPart = GetWhereSql(search);
            //声明查询数量的sql
            var sqlCount = new StringBuilder();
            sqlCount.Append(" SELECT COUNT(1) as TotalCount,COUNT(IF(IsDistributed>0,TRUE,NULL)) as DistributedCount ")
                .Append(countSqlPart);

            return _context.Database.SqlQuery<DistributedCountViewModel>(sqlCount.ToString()).FirstOrDefault();
        }

        public int FindCustomerCountNew(SearchCustomerListDto search)
        {
            if (search.PageSize < 1 || search.CurPage < 1 || search.ShowPageNum < 1)
            {
                return 0;
            }

            #region SQL语句
            var countSqlPart = GetWhereSql(search);
            //声明查询数量的sql
            var sqlCount = new StringBuilder();
            sqlCount.Append(" SELECT COUNT(1) FROM ( ");
            sqlCount.Append(" SELECT 1 ");

            sqlCount.Append(countSqlPart);

            int elsepage = 0;
            if (search.CurPage > search.ShowPageNum / 2 + 1)
            {
                elsepage = search.ShowPageNum / 2 - 1;
            }
            else
            {
                elsepage = search.ShowPageNum - search.CurPage;
            }
            sqlCount.Append(string.Format(" limit {0},{1}) AS LastCount ", search.CurPage * search.PageSize, elsepage * search.PageSize));
            #endregion
            return _context.Database.SqlQuery<int>(sqlCount.ToString()).FirstOrDefault();

        }

        public void GetSqlOrderBy(ref StringBuilder sqlList, int orderBy)
        {
            //排序
            switch (orderBy)
            {
                case 2: //交强降
                    //sqlList.Append(string.Format(" ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,{0} DESC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    sqlList.Append(string.Format(" ORDER BY {0} DESC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    break;
                case 21: //交强升
                    //sqlList.Append(string.Format(" ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,{0} ASC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    sqlList.Append(string.Format(" ORDER BY {0} ASC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()));
                    break;
                case 3: //商业降
                    //sqlList.Append(string.Format(" ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,{0} DESC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                    sqlList.Append(string.Format(" ORDER BY {0} DESC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                    break;
                case 31: //商业升
                    //sqlList.Append(string.Format(" ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC, {0} ASC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                    sqlList.Append(string.Format(" ORDER BY {0} ASC limit @pagebegin,@pageend ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                    break;
                case 4: //回访时间降
                    //业务员，取回访时间
                    sqlList.Append(" ORDER BY NextReviewDate DESC,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                case 41: //回访时间升
                    //业务员，取回访时间
                    //if (agentType == 1)
                    sqlList.Append(" ORDER BY NextReviewDate ASC,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                case 5://摄像头进店时间 降序
                    sqlList.Append(" order by ui.CameraTime desc,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                case 51://摄像头进店时间 升序
                    sqlList.Append(" order by ui.CameraTime ASC,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                case 6://分配时间 降序
                    sqlList.Append(" order by ui.DistributedTime desc,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                case 61://分配时间 升序
                    sqlList.Append(" order by ui.DistributedTime ASC,ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
                default: //更新时间降
                    sqlList.Append(" ORDER BY ui.UpdateTime DESC limit @pagebegin,@pageend ");
                    break;
            }
        }
        public void GetSqlOrderByTwo(ref StringBuilder sqlList, int orderBy)
        {
            //排序
            switch (orderBy)
            {
                case 2: //交强降
                    //sqlList.Append(
                    //" ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,LastForceEndDate DESC ");
                    sqlList.Append(" ORDER BY LastForceEndDate DESC ");
                    break;
                case 21: //交强升
                    //sqlList.Append(
                    // " ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,LastForceEndDate ASC ");
                    sqlList.Append(" ORDER BY LastForceEndDate ASC ");
                    break;
                case 3: //商业降
                    //sqlList.Append(" ORDER BY RenewalStatus DESC, ValueLastBizEndDate DESC,LastBizEndDate DESC ");
                    sqlList.Append(" ORDER BY LastBizEndDate DESC ");
                    break;
                case 31: //商业升
                    //sqlList.Append(" ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,LastBizEndDate ASC ");
                    sqlList.Append(" ORDER BY LastBizEndDate ASC ");
                    break;
                case 4: //回访时间降
                    //业务员，取回访时间
                    sqlList.Append(" ORDER BY NextReviewDate DESC,ui.UpdateTime DESC ");
                    break;
                case 41: //回访时间升
                    //业务员，取回访时间
                    sqlList.Append(" ORDER BY NextReviewDate ASC,ui.UpdateTime DESC ");
                    break;
                case 5://摄像头进店时间 降序
                    sqlList.Append(" order by ui.CameraTime desc,ui.UpdateTime DESC ");
                    break;
                case 51://摄像头进店时间 升序
                    sqlList.Append(" order by ui.CameraTime ASC,ui.UpdateTime DESC ");
                    break;
                case 6://分配时间 降序
                    sqlList.Append(" order by ui.DistributedTime desc,ui.UpdateTime DESC ");
                    break;
                case 61://分配时间 升序
                    sqlList.Append(" order by ui.DistributedTime ASC,ui.UpdateTime DESC ");
                    break;
                default: //更新时间降
                    sqlList.Append(" ORDER BY ui.UpdateTime DESC ");
                    break;
            }
        }
        #endregion

        public List<bx_userinfo> FindList(string agent)
        {
            return _context.bx_userinfo.Where(x => x.Agent == agent).ToList();
        }

        public List<GetCustomerViewModel> FindCustomerListForExport(SearchCustomerListDto search)
        {
            #region SQL语句
            //声明sql查询列表sql串
            var sqlList = new StringBuilder();
            //列表查询字段
            var sqlSelect = new StringBuilder();

            var selectSqlPart = GetWhereSql(search);

            sqlSelect.Append("SELECT ui.Id,ui.LicenseNo,ui.OpenId,ui.CityCode,ui.CategoryInfoId")
                .Append(string.Format(",{0} as MoldName", CompareBatchAndRenewalDateHelpler.GetMoldName()))
                .Append(",ui.LicenseOwner,ui.CreateTime,ui.UpdateTime,ui.RenewalType,ui.IsCamera")
                .Append(",ui.EngineNo,ui.CarVIN")
                .Append(string.Format(",{0} as LastYearSource", CompareBatchAndRenewalDateHelpler.GetLastYearSource()))
                .Append(",ui.RegisterDate,ui.NeedEngineNo,ui.QuoteStatus,ui.RenewalStatus")
                .Append(",ui.LicenseOwner,ui.InsuredIdCard,ui.InsuredName")
                .Append(string.Format(",{0} AS ValueLastForceEndDate", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)))
                .Append(string.Format(",{0} AS ValueLastBizEndDate", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)))
                .Append(",ui.agent,ui.IsSingleSubmit,CASE ui.IsTest WHEN 1 THEN 3 ELSE ui.IsDistributed END AS IsDistributed,ui.IsReView")
                .Append(string.Format(",{0} AS LastForceEndDate ", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()))
                .Append(string.Format(",{0} AS LastBizEndDate", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                .Append(" ,ui.CameraTime,ui.DistributedTime,cr.Organization ")
                ;
            //拼接查询列表sql串 //select where union all select where2                 
            sqlList.Append(sqlSelect).Append(string.Format(selectSqlPart.ToString(), ""));

            //获取排序信息              
            switch (search.OrderBy)
            {
                case 2: //交强降
                    sqlList.Append(
                        " ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,LastForceEndDate DESC,Id DESC  ");
                    break;
                case 21: //交强升
                    sqlList.Append(
                        " ORDER BY RenewalStatus DESC,ValueLastForceEndDate DESC,LastForceEndDate ASC,Id DESC   ");
                    break;
                case 3: //商业降
                    sqlList.Append(
                        " ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,LastBizEndDate DESC,Id DESC   ");
                    break;
                case 31: //商业升
                    sqlList.Append(
                        " ORDER BY RenewalStatus DESC,ValueLastBizEndDate DESC,LastBizEndDate ASC,Id DESC ");
                    break;
                //case 4://回访时间降
                //    //业务员，取回访时间
                //    if (agentType == 1)
                //        sqlList.Append(" ORDER BY NextReviewDate DESC,UpdateTime DESC,Id DESC ");
                //    break;
                //case 41://回访时间升
                //    //业务员，取回访时间
                //    if (agentType == 1)
                //        sqlList.Append(" ORDER BY NextReviewDate ASC,UpdateTime DESC,Id DESC limit @pagebegin,@pageend ");
                //    break;
                default: //更新时间降
                    sqlList.Append(" ORDER BY UpdateTime DESC,Id DESC  ");
                    break;
            }
            #endregion

            //查询列表
            List<GetCustomerViewModel> listuserinfo =
                _context.Database.SqlQuery<GetCustomerViewModel>(sqlList.ToString()).ToList();

            return listuserinfo;
        }

        public async Task<int> FindCustomerCountAsync(SearchCustomerListDto search)
        {
            var countSqlPart = GetWhereSql(search);

            // 声明查询数量的sql
            var sqlCount = new StringBuilder();
            // 拼接查询数量的sql
            sqlCount.Append(" SELECT COUNT(1) ").Append(countSqlPart);

            return await Task.FromResult(_context.Database.SqlQuery<int>(sqlCount.ToString()).FirstOrDefault());
        }

        public int FindCustomerCount(SearchCustomerListDto search)
        {
            var countSqlPart = GetWhereSql(search);

            // 声明查询数量的sql
            var sqlCount = new StringBuilder();
            // 拼接查询数量的sql
            sqlCount.Append(" SELECT COUNT(1) ").Append(countSqlPart);

            return _context.Database.SqlQuery<int>(sqlCount.ToString()).FirstOrDefault();
        }

        public List<long> FindCustomerBuid(SearchCustomerListDto search)
        {
            var countSqlPart = GetWhereSql(search);
            var sqlList = new StringBuilder("SELECT ui.Id ")
                .Append(countSqlPart);
            //查询列表
            List<long> listuserinfo = _context.Database.SqlQuery<long>(sqlList.ToString()).ToList();
            return listuserinfo;
        }

        public List<long> FindCustomerBuid2(SearchCustomerListDto search, int DistributeAgentIds, int AverageCount, int OrderBy)
        {
            StringBuilder tempBuilder = new StringBuilder();
            tempBuilder.Append(",ui.RenewalStatus");
            tempBuilder.Append(string.Format(",{0} AS ValueLastBizEndDate ", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)));
            tempBuilder.Append(string.Format(",{0} as LastBizEndDate ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));

            var countSqlPart = GetWhereSql(search);
            var sqlList = new StringBuilder("SELECT DISTINCT ui.Id ")
                .Append(tempBuilder)//方便查询数据
                .Append(countSqlPart);

            GetSqlOrderByTwo(ref sqlList, OrderBy);
            var takeCount = DistributeAgentIds * AverageCount;
            sqlList.Append(string.Format(" limit {0}", takeCount));
            //LogHelper.Info("FindCustomerBuid2=" + sqlList.ToString());
            //查询列表
            List<long> listuserinfo = _context.Database.SqlQuery<DistributeUserinfoDto>(sqlList.ToString()).Select(a => a.Id).ToList();
            return listuserinfo;
        }
        /// <summary>
        /// 齐大康 2018-04-24
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<UserInfoIdAgentModel> FindCustomerBuidAndAgent(SearchCustomerListDto search)
        {
            var countSqlPart = GetWhereSql(search);
            var sqlList = new StringBuilder("SELECT ui.Id,Agent ")
                .Append(countSqlPart);
            //查询列表
            List<UserInfoIdAgentModel> listuserinfo = _context.Database.SqlQuery<UserInfoIdAgentModel>(sqlList.ToString()).ToList();
            return listuserinfo;
        }

        public List<DistributeUserinfoDto> FindCustomerBuidOrderBy(List<long> listBuid, int orderBy, int takeCount)
        {
            StringBuilder sqlSb = new StringBuilder(" SELECT ui.Id ")
                 .Append(",ui.IsDistributed,ui.RenewalType,ui.LicenseNo,ui.UpdateTime,ui.RenewalStatus,ui.IsCamera,ui.CameraTime ")
                 .Append(",ui.Agent,ui.IsInputBxData")
                 .Append(string.Format(",{0} AS ValueLastBizEndDate ", CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)))
                 .Append(string.Format(",{0} as LastBizEndDate ", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                 .Append(" FROM bx_car_renewal cr RIGHT JOIN bx_userinfo_renewal_index uri ON cr.Id=uri.car_renewal_id RIGHT JOIN bx_userinfo ui ON ui.Id =uri.b_uid  LEFT JOIN bx_batchrenewal_item ON ui.Id=bx_batchrenewal_item.BUId AND bx_batchrenewal_item.IsNew=1 AND bx_batchrenewal_item.IsDelete=0 WHERE 1=1 ")
                 .Append(" and ui.Id in (" + string.Join(",", listBuid) + ") ");

            GetSqlOrderByTwo(ref sqlSb, orderBy);
            sqlSb.Append(string.Format(" limit {0}", takeCount));

            //LogHelper.Info("分配SQL脚本：" + sqlSb.ToString());
            //LogHelper.Info("分配BUID：" + string.Join(",", listBuid));
            //LogHelper.Info("分配前" + takeCount + "条");
            var list = _context.Database.SqlQuery<DistributeUserinfoDto>(sqlSb.ToString()).ToList();//626
            return list;
        }

        /// <summary>
        /// 获取新进店车辆数量
        /// 20170218修改，增加buid返回，并查续保成功的记录
        /// </summary>
        /// <param name="updateDateEnd"></param>
        /// <param name="agentId"></param>
        /// <param name="renewalType"></param>
        /// <param name="updateDateStart"></param>
        /// <returns></returns>
        public List<long> FindBuidLoop(string updateDateStart, string updateDateEnd, int agentId, int renewalType, List<string> listSonAgent)
        {
            var buids = new List<long>();
            DateTime dateTimeStart = (!string.IsNullOrWhiteSpace(updateDateStart) &&
                                      !updateDateStart.ToUpper().Equals("NULL"))
                ? DateTime.Parse(updateDateStart)
                : DateTime.Now.AddMinutes(-5);
            DateTime dateTimeEnd = (!string.IsNullOrWhiteSpace(updateDateEnd) &&
                                    !updateDateEnd.ToUpper().Equals("NULL"))
                ? DateTime.Parse(updateDateEnd)
                : DateTime.Now;

            var sonSb = new StringBuilder("'");
            sonSb.Append(string.Join("','", listSonAgent))
                .Append("'");

            var sons = sonSb.ToString();

            if (string.IsNullOrEmpty(sons))
            {
                return buids;
            }
            // 去掉了QuoteStatus=-1的条件限制  陈亮  2017-9-4
            var querySql = string.Format(@"SELECT ui.Id FROM `bx_userinfo` as ui
                                                              WHERE  `RenewalStatus`=1  AND IsCamera=1 AND CameraTime between '{0}' and '{1}' AND `Agent` IN ({2}) AND IsTest=0 ", dateTimeStart, dateTimeEnd, sons);
            return _context.Database.SqlQuery<long>(querySql.ToString()).ToList();
        }
        /// <summary>
        /// 查询userinfo的agent
        /// </summary>
        /// <param name="lecenseNo"></param>
        /// <returns></returns>
        public List<bx_userinfo> FindAgentListByLicenseNo(string lecenseNo)
        {
            var list = _context.bx_userinfo.Where(x => x.LicenseNo == lecenseNo && x.IsTest == 0).ToList();
            return list;
        }

        /// <summary>
        /// 生成关联的sql，查询的时候根据条件来判断是否需要关联表
        /// </summary>
        /// <param name="joinType">需要关联那个表：0->不关联  1->关联bx_consumer_review </param>
        /// <param name="sqlWhere">这里是不需要关联表的where部分</param>
        /// <param name="joinWhere">这里是需要关联表的where的部分</param>
        /// <param name="isJoinExpand"></param>
        /// <param name="currentAgentId"></param>
        /// <param name="needConvert">是否需要转换代理人</param>
        /// <returns></returns>
        private StringBuilder GetJoinSql(SearchCustomerListDto search, int joinType, string sqlWhere, string joinWhere, int isJoinExpand, int isJoinExpandCameraConfig, int currentAgentId = -1, bool needConvert = false)
        {
            // sql语句的from和where部分
            var sqlFromAndWhere = new StringBuilder();
            string selectSqlPart = string.Empty;
            string strUserRenewalInfoTemp = strUserRenewalInfo;
            //判断是否关联扩展表
            if (isJoinExpand == 1)
            {
                strUserRenewalInfoTemp = strUserRenewalInfoTemp + UserInfoExpandSql;
            }
            if (isJoinExpandCameraConfig == 1)
            {
                strUserRenewalInfoTemp = strUserRenewalInfoTemp + UserInfoExpandSqlNotDelete + CameraConfigSql;
            }
            //是否是车主报价
            if (search.IsOwnerInquiry == 1)
            {
                strUserRenewalInfoTemp = strUserRenewalInfoTemp + ExpandOwnerInquirySql;
            }

            if (joinType == 0)
            {
                if (HasMoreThan2000ChildAgentTopAgentId.Contains(currentAgentId))
                {
                    // 代理人多
                    if (ForceAndConvertTopAgentId.Contains(currentAgentId))
                    {
                        sqlFromAndWhere.Append(string.Format(SqlNoPlaceholderHasIndexHasJoinAgent, CONVERT_AGNET, strUserRenewalInfoTemp));
                    }
                    else
                    {
                        if (currentAgentId == 4245)
                        {
                            if (needConvert)
                            {
                                sqlFromAndWhere.Append(string.Format(SqlNoPlaceholderNoIndexHasJoinAgent, CONVERT_AGNET, strUserRenewalInfoTemp));
                            }
                            else
                            {
                                sqlFromAndWhere.Append(string.Format(SqlNoPlaceholderNoIndexHasJoinAgent, NO_CONVERT_AGNET, strUserRenewalInfoTemp));
                            }
                        }
                        else
                        {
                            sqlFromAndWhere.Append(string.Format(SqlNoPlaceholderNoIndexHasJoinAgent, NO_CONVERT_AGNET, strUserRenewalInfoTemp));
                        }
                    }
                }
                else
                {


                    // 代理人数量不多，走in
                    //sqlFromAndWhere.Append(SqlNoPlaceholderHasIndexNoJoinAgent);
                    sqlFromAndWhere.Append(string.Format(SqlNoPlaceholderHasIndexNoJoinAgent, strUserRenewalInfoTemp));
                }
            }
            else if (joinType == 1)
            {
                if (HasMoreThan2000ChildAgentTopAgentId.Contains(currentAgentId))
                {
                    if (ForceAndConvertTopAgentId.Contains(currentAgentId))
                    {
                        sqlFromAndWhere.Append(string.Format(SqlHasPlaceholderHasIndexHasJoinAgent, CONVERT_AGNET, strVisitTimeSql, strUserRenewalInfoTemp));
                    }
                    else
                    {
                        if (currentAgentId == 4245)
                        {
                            if (needConvert)
                            {
                                sqlFromAndWhere.Append(string.Format(SqlHasPlaceholderNoIndexHasJoinAgent, CONVERT_AGNET, strVisitTimeSql, strUserRenewalInfoTemp));
                            }
                            else
                            {
                                sqlFromAndWhere.Append(string.Format(SqlHasPlaceholderNoIndexHasJoinAgent, NO_CONVERT_AGNET, strVisitTimeSql, strUserRenewalInfoTemp));
                            }
                        }
                        else
                        {
                            sqlFromAndWhere.Append(string.Format(SqlHasPlaceholderNoIndexHasJoinAgent, NO_CONVERT_AGNET, strVisitTimeSql, strUserRenewalInfoTemp));
                        }

                    }
                }
                else
                {
                    sqlFromAndWhere.Append(string.Format(SqlHasPlaceholderHasIndexNoJoinAgent, strVisitTimeSql, strUserRenewalInfoTemp));
                }
            }
            else
            {
                var errMsg = "userinforepository->GetJoinSql方法的joinType参数错误：joinType=" + joinType.ToString();
                LogHelper.Error(errMsg);
                throw new Exception(errMsg);
            }
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sqlFromAndWhere.Append(sqlWhere);
            }
            if (joinType != 0 && !string.IsNullOrEmpty(joinWhere))
            {
                sqlFromAndWhere.Append(joinWhere);
            }

            return sqlFromAndWhere;
        }

        /// <summary>
        /// 生成from和where后面的sql
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private StringBuilder GetWhereSql(SearchCustomerListDto search)
        {
            var selectSqlPart = GetJoinSql(search, search.JoinType, search.SqlBuilder.ToString(), search.JoinWhere, search.IsJoinExpand, search.IsJoinExpandCameraConfig, search.CurrentAgent, search.NeedForceIndex);
            // 根据search中的条件生成SQL
            selectSqlPart.Append(GenerateSearchSql(search));

            return selectSqlPart;
        }




        public bx_userinfo GetUserInfo(long id)
        {
            return _context.bx_userinfo.FirstOrDefault(x => x.Id == id);
        }
        /// <summary>
        /// 更新续保类型
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="renewalType"></param>
        /// <returns></returns>
        public int UpdateUserRenewalType(int buId, int renewalType)
        {
            var bxUser = _context.bx_userinfo.FirstOrDefault(c => c.Id == buId);
            bxUser.RenewalType = renewalType;
            return _context.SaveChanges();
        }
        /// <summary>
        /// 更新分配状态
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="distributed"></param>
        /// <returns></returns>
        public int UpdateUserDistributed(int buId, int distributed)
        {
            var bxUser = _context.bx_userinfo.FirstOrDefault(c => c.Id == buId);
            bxUser.IsDistributed = distributed;
            return _context.SaveChanges();
        }
        /// <summary>
        /// 更新续保类型&分配状态
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="renewalType"></param>
        /// <param name="distributed"></param>
        /// <returns></returns>
        public int UpdateUserRenewalTypeAndDistributed(int buId, int renewalType, int distributed,
            bool exitUserinfo = false, bool isDistributedUserInfo = false)
        {
            if (exitUserinfo || isDistributedUserInfo)
            {
                return 1;//数据存在 或执行自动分配的数据无需更新
            }
            var bxUser = _context.bx_userinfo.FirstOrDefault(c => c.Id == buId);
            if (bxUser.IsDistributed != 3 || distributed == 0)
            {
                //已分配的数据 如果原状态和要改的状态不一致，则状态不变
                bxUser.IsDistributed = distributed;
            }
            //摄像头进店，标记为摄像头进店，并且更新进店时间
            if (renewalType == 3)
            {
                bxUser.IsCamera = true;
                bxUser.CameraTime = DateTime.Now;
            }
            return _context.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="topAgentId"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public bool UpdateIsTest(string buids, int topAgentId, int isTest, string strAgents)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("UPDATE bx_userinfo SET istest =?isTest ,UpdateTime=now() WHERE ");
            if (!string.IsNullOrEmpty(strAgents))//齐大康 2018-04-23
            {
                sqlBuilder.Append(" agent in (" + strAgents + ") and ");
            }
            sqlBuilder.Append(" id IN(" + buids + ") ");
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isTest",
                    Value = isTest
                }
            };
            var result = _context.Database.ExecuteSqlCommand(sqlBuilder.ToString(), param);
            if (result > 0)
            {//更新bx_batchrenewal_item
                new BatchRenewalRepository().SoftDelete(buids);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除之后更新统计表
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public bool UpdateReviewdetailRecord(string buids)
        {
            try
            {
                string sql = "update bihu_analytics.tj_reviewdetail_record t set t.Deleted=1, t.UpdateTime=NOW() where BuId in(" + buids + ")";
                var result = _context.Database.ExecuteSqlCommand(sql);
                return result > 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateReviewdetailRecord=" + buids + ";发生异常：" + ex);
            }
            return false;
        }

        public bool UpdateAgent(string buids, int newAgentId)
        {
         
            //string sql = "UPDATE bx_userinfo SET IsTest=3, agent =?newAgentId, agent_id =?newAgentId,top_agent_id=?newAgentId WHERE id IN(" + buids + ") ";
            string sql = "UPDATE bx_userinfo SET IsTest=3, agent =?newAgentId, agent_id =?newAgentId, top_agent_id =?newAgentId,UpdateTime=now() WHERE id IN(" + buids + ") ";
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "newAgentId",
                    Value = newAgentId
                }
            };
            return _context.Database.ExecuteSqlCommand(sql, param) > 0;
        }

        /// <summary>
        /// 根据buid删除数据
        /// </summary>
        /// <param name="newAgentId"></param>
        /// <param name="strBuids"></param>
        /// <returns></returns>
        public bool UpdateAgentByBuid(int newAgentId, string strBuids, int IsTest)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" UPDATE bx_userinfo SET agent =?newAgentId, agent_id =?newAgentId,UpdateTime=now() WHERE ");
            sqlBuilder.Append(" id in (" + strBuids + ") ");
            sqlBuilder.Append(" and istest=@IsTest");
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "newAgentId",
                    Value = newAgentId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "IsTest",
                    Value = IsTest
                }
            };
            return _context.Database.ExecuteSqlCommand(sqlBuilder.ToString(), param) > 0;
        }

        public bool UpdateAgent(int newAgentId, string strAgents)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" UPDATE bx_userinfo SET agent =?newAgentId, agent_id =?newAgentId,UpdateTime=now()  WHERE ");
            if (!string.IsNullOrEmpty(strAgents))//齐大康 2018-04-23
            {
                sqlBuilder.Append(" agent in (" + strAgents + ") and ");
            }
            sqlBuilder.Append("  istest=3 ");
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "newAgentId",
                    Value = newAgentId
                }
            };
            return _context.Database.ExecuteSqlCommand(sqlBuilder.ToString(), param) > 0;
        }
        public List<bx_userinfo> UdapteDateupdate()
        {
            var querySql = string.Format(@"select bx_userinfo.* from bx_userinfo
inner join bx_userinfo_renewal_index on bx_userinfo.id=bx_userinfo_renewal_index.b_uid
left join bx_car_renewal  on bx_car_renewal.id= bx_userinfo_renewal_index.car_renewal_id   
WHERE bx_userinfo.updatetime>'2017-03-29' AND bx_userinfo.updatetime <'2017-03-29 20:21:00' and  bx_car_renewal.id is null  ");
            return _context.Database.SqlQuery<bx_userinfo>(querySql.ToString()).ToList();
        }
        public bx_userinfo_renewal_index GetBxUserinfoRenewalIndex(long buid)
        {
            return _context.bx_userinfo_renewal_index.FirstOrDefault(c => c.b_uid == buid);
        }
        public List<bx_car_renewal> GetBxCarRenewals(string licenseno)
        {
            var querySql =
                string.Format(
                    @"SELECT * FROM bx_car_renewal WHERE licenseno='{0}'  ORDER BY IFNULL(LastBizEndDate,LastBizEndDate) DESC limit 1 ",
                    licenseno);
            return _context.Database.SqlQuery<bx_car_renewal>(querySql.ToString()).ToList();
        }
        public void UpdatebxUserinfoRenewalIndex(bx_userinfo_renewal_index bxUserinfoRenewalIndex, long buid)
        {
            var buri = _context.bx_userinfo_renewal_index.FirstOrDefault(c => c.b_uid == buid);
            buri.car_renewal_id = bxUserinfoRenewalIndex.car_renewal_id;
            _context.SaveChanges();
        }
        /// <summary>
        /// 查询是否是新车
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public int Selquotereqcarinfo(long buId)
        {
            return Convert.ToInt32(_context.bx_quotereq_carinfo.Where(x => x.b_uid == buId).Select(x => x.is_newcar).FirstOrDefault());
        }
        public void UpdateCarRenewalIndex()
        {
            string selectSql = @"SELECT  u.Id AS BuId , cr.id AS CarRenewalId FROM bx_userinfo AS u
LEFT JOIN bx_userinfo_renewal_index AS uri
ON u.Id =uri.b_uid
  LEFT JOIN bx_car_renewal AS cr ON u.LicenseNo=cr.LicenseNo   AND cr.id=
(SELECT id FROM bx_car_renewal AS u1 WHERE u1.LicenseNo=u.LicenseNo  ORDER BY  IFNULL(u1.LastForceEndDate,u1.LastBizEndDate) DESC LIMIT 0,1)
WHERE u.Agent='70014' AND u.CreateTime  >'2017-03-20'AND u.createtime <'2017-03-30 ' AND  uri.id IS NULL  ";
            var carRenewalIndexList = _context.Database.SqlQuery<UpdateCarRenewalIndexModel>(selectSql).ToList();
            List<bx_userinfo_renewal_index> userinfo_renewal_index_list = new List<bx_userinfo_renewal_index>();
            foreach (var item in carRenewalIndexList)
            {
                bx_userinfo_renewal_index userinfo_renewal_index = new bx_userinfo_renewal_index();
                userinfo_renewal_index.b_uid = item.BuId;
                userinfo_renewal_index.car_renewal_id = item.CarRenewalId;
                userinfo_renewal_index.create_time = item.CreateTime;
                userinfo_renewal_index.update_time = DateTime.Now;
                _context.bx_userinfo_renewal_index.Add(userinfo_renewal_index);
            }
            _context.SaveChanges();
        }
        public List<long> GetListBuId(string agent)
        {
            return _context.bx_userinfo.Where(o => o.Agent == agent).Select(o => o.Id).ToList();
        }
        /// <summary>
        /// 根据车牌号和代理人信息查询是否有子集数据 目前仅适用于APP，因为新加了renewalcartype判断
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        public bx_userinfo FindAgentListByLicenseNo(long buid, string licenseNo, List<string> agentIds)
        {
            try
            {
                var model = _context.bx_userinfo
                    .Where(x => //x.IsDistributed != 0 && 
                        x.Id != buid
                        && x.IsTest == 0
                        && x.RenewalCarType == 0
                        && x.LicenseNo == licenseNo
                        && agentIds.Contains(x.Agent))
                    .OrderByDescending(o => o.UpdateTime).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new bx_userinfo();
        }
        /// <summary>
        /// 删除某个buid
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool DeleteUserinfo(long buid, string deleteAgentId)
        {
            try
            {
                bx_userinfo bxUser = _context.bx_userinfo.FirstOrDefault(c => c.Id == buid);
                bxUser.Agent = deleteAgentId;
                bxUser.OpenId = deleteAgentId.GetMd5();
                bxUser.IsTest = 3;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
        /// <summary>
        /// 更新模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(bx_userinfo model)
        {
            int count = 0;
            try
            {
                _context.bx_userinfo.AddOrUpdate(model);
                count = _context.SaveChanges();
                return count;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        #region 从师傅项目搬过来的,目前搁置
        /// <summary>
        /// app端统计用，续保报表
        /// </summary>
        /// <param name="sonself"></param>
        /// <param name="strDate"></param>
        /// <param name="licenseNo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<bx_userinfo> ReportForReInfoList(List<string> sonself, string strDate, string licenseNo,
            out int totalCount)
        {
            totalCount = 0;
            string strBuid = string.Join(",", sonself);
            var listuserinfo = new List<bx_userinfo>();
            try
            {
                string startdt = strDate + " 00:00:00";
                string enddt = strDate + " 23:59:59";
                var sql = new StringBuilder(
                    @"SELECT * FROM bx_userinfo WHERE LENGTH(OpenId)>9 AND id IN(
                                                        SELECT DISTINCT buid FROM bx_transferrecord WHERE ( FromAgentId IN (" +
                    strBuid + @") OR ToAgentId IN (" + strBuid + @")))
                                                         AND id IN ( SELECT DISTINCT b_uid FROM bx_consumer_review WHERE result_status=1 AND create_time >= '" +
                    startdt + @"' AND create_time <= '" + enddt + @"')");
                listuserinfo = _context.Database.SqlQuery<bx_userinfo>(sql.ToString()).ToList();
                totalCount = listuserinfo.Count;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return
                listuserinfo.OrderByDescending(i => (i.UpdateTime.HasValue ? i.UpdateTime : i.CreateTime))
                    .ThenByDescending(i => i.CreateTime)
                    .ToList();
        }
        #endregion
        ///// <summary>
        ///// 人保禁呼
        ///// </summary>
        ///// <param name="licenseno">车牌</param>
        ///// <param name="cityid">城市id</param>
        ///// <param name="source">禁呼保险公司</param>
        ///// <returns></returns>
        //public bool IsForbidMobile(string licenseno, int cityid, int source)
        //{
        //    return _context.bx_forbid_mobile.Any(c => c.license_no == licenseno && c.city_id == cityid && c.source == source);
        //}
        public bx_forbid_mobile GetForbidMobile(string licenseno, int cityid, int source)
        {
            return _context.bx_forbid_mobile.FirstOrDefault(c => c.license_no == licenseno && c.city_id == cityid && c.source == source);
        }
        public bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno, string agent, string citycode, int renewalCarType)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {
                if (!string.IsNullOrEmpty(citycode))
                {
                    if (renewalCarType != -1)
                    {
                        tt =
                       _context
                           .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                           .FirstOrDefault(
                               x =>
                                   x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                                   x.QuoteStatus > 0 &&
                                   x.Agent == agent && x.CityCode == citycode && x.RenewalCarType == renewalCarType);
                    }
                    else
                    {
                        tt =
                       _context
                           .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                           .FirstOrDefault(
                               x =>
                                   x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                                   x.QuoteStatus > 0 &&
                                   x.Agent == agent && x.CityCode == citycode);
                    }
                }
                else
                {
                    //modify 齐大康  由原来renewalCarType！=1没有进行条件筛选改为renewalCarType！=1进行条件筛选
                    //注释掉的为老代码
                    if (renewalCarType != -1)
                    {
                        // tt =
                        //_context
                        //    .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                        //    .FirstOrDefault(
                        //        x =>
                        //            x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                        //            x.QuoteStatus > 0 &&
                        //            x.Agent == agent);
                        tt =
                      _context
                          .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                          .FirstOrDefault(
                              x =>
                                  x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                                  x.QuoteStatus > 0 &&
                                  x.Agent == agent && x.RenewalCarType == renewalCarType);
                    }
                    else
                    {
                        // tt =
                        //_context
                        //    .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                        //    .FirstOrDefault(
                        //        x =>
                        //            x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                        //            x.QuoteStatus > 0 &&
                        //            x.Agent == agent && x.RenewalCarType == renewalCarType);
                        tt =
                       _context
                           .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                           .FirstOrDefault(
                               x =>
                                   x.IsTest == 0 && x.OpenId == openid && x.LicenseNo == licenseno &&
                                   x.QuoteStatus > 0 &&
                                   x.Agent == agent);
                    }
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tt;
        }
        /// <summary>
        /// 根据车架号发动机号获取数据
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="carVin"></param>
        /// <param name="engineNo"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public bx_userinfo FindByCarVin(string openid, string carVin, string engineNo, string agent, string citycode, int renewalCarType)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {

                if (!string.IsNullOrEmpty(citycode))
                {
                    if (renewalCarType != -1)
                    {
                        tt =
                       _context
                           .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                           .FirstOrDefault(
                               x =>
                                   x.IsTest == 0 && x.OpenId == openid && x.CarVIN == carVin &&
                                   x.EngineNo == engineNo &&
                                   x.QuoteStatus > 0 && x.Agent == agent && x.CityCode == citycode && x.RenewalCarType == renewalCarType);
                    }
                    else
                    {
                        tt =
                        _context
                            .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                            .FirstOrDefault(
                                x =>
                                    x.IsTest == 0 && x.OpenId == openid && x.CarVIN == carVin &&
                                    x.EngineNo == engineNo &&
                                    x.QuoteStatus > 0 && x.Agent == agent && x.CityCode == citycode);
                    }
                }
                else
                {
                    if (renewalCarType != -1)
                    {
                        tt =
                       _context
                           .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                           .FirstOrDefault(
                               x =>
                                   x.IsTest == 0 && x.OpenId == openid && x.CarVIN == carVin &&
                                   x.EngineNo == engineNo &&
                                   x.QuoteStatus > 0 && x.Agent == agent && x.RenewalCarType == renewalCarType);
                    }
                    else
                    {
                        tt =
                           _context
                               .bx_userinfo.OrderByDescending(o => o.UpdateTime)
                               .FirstOrDefault(
                                   x =>
                                       x.IsTest == 0 && x.OpenId == openid && x.CarVIN == carVin &&
                                       x.EngineNo == engineNo &&
                                       x.QuoteStatus > 0 && x.Agent == agent);
                    }
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tt;
        }
        public List<AgentNameViewModel> IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type)
        {
            if (type == 1 || type == 2)
            {
                return IsHaveLicensenoList(topAgentId, agentId, licenseno, vinNo, type, true);
            }
            else
            {//当type=3的时候，先执行车牌，如果没有重复数据，再执行车架号
                List<AgentNameViewModel> list = IsHaveLicensenoList(topAgentId, agentId, licenseno, vinNo, 2, true);
                if (!list.Any())
                {
                    list = IsHaveLicensenoList(topAgentId, agentId, licenseno, vinNo, 1, true);
                }
                return list;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="type">1车牌2车架号</param>
        /// <param name="ifElseMe">是否只包含自己</param>
        /// <returns></returns>
        private List<AgentNameViewModel> IsHaveLicensenoList(int topAgentId, int agentId, string licenseno, string vinNo, int type, bool ifElseMe)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" SELECT bx_agent.AgentName,bx_agent.Mobile,bx_agent.Id AS AgentId ,bx_agent.TopAgentId,bx_userinfo.Id  AS Buid  FROM bx_agent INNER JOIN bx_userinfo ON CAST(bx_agent.id AS CHAR)=bx_userinfo.agent ");
            sql.Append(" where bx_agent.TopAgentId=?topAgentId and istest=0");
            if (type == 1)
            {
                sql.Append(" and bx_userinfo.LicenseNo=?licenseno");
            }
            else
            {
                sql.Append(" and bx_userinfo.CarVIN=?vinNo");
            }
            if (ifElseMe)
            {
                sql.Append(" and bx_userinfo.agent !=?agentId");
            }
            sql.Append(" ORDER BY bx_userinfo.updatetime asc ");
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "topAgentId",
                    Value = topAgentId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "licenseno",
                    Value = licenseno
                },
                  new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "vinNo",
                    Value = vinNo
                },
                  new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                }
            };
            var result = DataContextFactory.GetDataContext().Database.SqlQuery<AgentNameViewModel>(sql.ToString(), param).ToList();
            return result;
        }
        public void UpdateBxUserinfoAgent(string agentMd5, long buid, int agentid)
        {
            string sql = @"UPDATE bx_userinfo SET Agent=?agentid,OpenId=?agentMd5, IsDistributed=3,DistributedTime=NOW(),UpdateTime=NOW() WHERE id=?buid";
            var param = new[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentid",
                    Value = agentid
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "agentMd5",
                    Value = agentMd5
                },
                  new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int64,
                    ParameterName = "buid",
                    Value = buid
                }
            };
            _context.Database.ExecuteSqlCommand(sql, param);
        }
        /// <summary>
        /// 根据车牌号和openid获取userinfo
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="licenseno"></param>
        /// <returns></returns>
        public bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {
                string strSql = string.Format("select * from bx_userinfo where IsTest=1 and LicenseNo='{0}' and OpenId='{1}' limit 1", licenseno, openid);
                tt = DataContextFactory.GetDataContext().Database.SqlQuery<bx_userinfo>(strSql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tt;
        }
        /// <summary>
        /// 根据agentIds和isTest获取该顶级下的所有符合isTest的数据
        /// modify 齐大康 2018-04-24 获取Agent
        /// </summary>
        /// <param name="agentIds">格式：'102','103','168'</param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public List<long> FindByAgentsAndIsTest2(string agentIds, int isTest)
        {
            var sql = string.Format("SELECT distinct bx_userinfo.Agent FROM bx_userinfo  WHERE agent IN ({0}) AND IsTest={1}", agentIds, isTest);

            try
            {
                return _context.Database.SqlQuery<long>(sql).ToList();
            }
            catch
            {
                return new List<long>();
            }
        }
        /// <summary>
        /// 根据agentIds和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public List<UserInfoIdAgentModel> FindByAgentsAndIsTest(string agentIds, int isTest)
        {
            var sql = string.Format("SELECT bx_userinfo.Id,bx_userinfo.Agent FROM bx_userinfo  WHERE agent IN ({0}) AND IsTest={1}", agentIds, isTest);

            try
            {
                return _context.Database.SqlQuery<UserInfoIdAgentModel>(sql).ToList();
            }
            catch
            {
                return new List<UserInfoIdAgentModel>();
            }
        }
        public List<RevokeUserInfoDto> FindByAgentsAndLicenseNo(string agentIds, string licenseNo)
        {
            var sql = "SELECT agent FROM bx_userinfo WHERE licenseno =?licenseNo AND agent IN (" + agentIds + ") AND IsTest=0 ";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="licenseNo",
                    Value=licenseNo,
                    MySqlDbType=MySqlDbType.VarChar
                }
            };
            return _context.Database.SqlQuery<RevokeUserInfoDto>(sql, param).ToList();
        }

        public async Task<List<DistributedRecycleDto>> GetDistributedRecycleAsync(List<long> listBuid)
        {
            var sql = string.Format("SELECT id as Buid,agent as AgentId  FROM bx_userinfo WHERE id IN ({0})", string.Join(",", listBuid));

            return await _context.Database.SqlQuery<DistributedRecycleDto>(sql).ToListAsync();

        }

        public bx_userinfo FindByBuid(long buid)
        {
            bx_userinfo userinfo = new bx_userinfo();
            try
            {
                userinfo = DataContextFactory.GetDataContext().bx_userinfo.AsNoTracking().FirstOrDefault(x => x.Id == buid);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return userinfo;
        }

        public IList<bx_userinfo> GetList(Expression<Func<bx_userinfo, bool>> where)
        {
            return DataContextFactory.GetDataContext().bx_userinfo.Where(where).ToList();
        }

        public DataSet GetMyListDateInformation(string sql)
        {
            string connctionStr = DataContextFactory.GetDataContext().Database.Connection.ConnectionString;
            using (var connection = new MySqlConnection(connctionStr))
            {
                var ds = new DataSet();
                try
                {
                    connection.Open();
                    var command = new MySqlDataAdapter(sql, connection);
                    command.Fill(ds);
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                return ds;
            }
        }

        public List<UserInfoIdDistribute> GetIsDistributed(SearchCustomerListDto search)
        {
            //sql语句拼接判断
            var countSqlPart = GetWhereSql(search);
            //声明查询数量的sql
            var sqlCount = new StringBuilder();
            sqlCount.Append(" SELECT ui.id as Buid,ui.IsDistributed,ui.Agent ")
                .Append(countSqlPart);

            return _context.Database.SqlQuery<UserInfoIdDistribute>(sqlCount.ToString()).ToList();
        }

        public List<long> GetListUseridByAgentIds(string ids, string licenseno, string vinNo, int type)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return new List<long>();
            }

            string lic = type == 1 && !string.IsNullOrWhiteSpace(licenseno) ? " and LicenseNo = '" + licenseno + "'" : "";
            string vin = type != 1 && !string.IsNullOrWhiteSpace(vinNo) ? " and CarVIN = '" + vinNo + "'" : "";
            var sql = string.Format("SELECT id FROM bx_userinfo WHERE Agent IN ({0}) {1} {2}", ids, lic, vin);
            return _context.Database.SqlQuery<long>(sql).ToList();
        }

        public List<GetCustomerViewModel> FillCustomerInformation(List<long> list)//改成参数化查询
        {
            StringBuilder sbId = new StringBuilder();
            list.ForEach(l => sbId.Append(l + ","));
            string strSql = @"SELECT bx_consumer_review.create_time as LastReviewTime,bx_consumer_review.content as LastReviewContent,bx_defeatreasonsetting.DefeatReason,bx_userinfo.Id FROM bx_userinfo  
                                  LEFT JOIN bx_consumer_review ON bx_userinfo.Id=bx_consumer_review.b_uid 
                                  LEFT JOIN bx_defeatreasonsetting ON bx_consumer_review.DefeatReasonId=bx_defeatreasonsetting.Id
                                   WHERE bx_userinfo.Id  IN (" + sbId.Remove(sbId.Length - 1, 1) + ")  ORDER BY bx_consumer_review.create_time desc";
            return _context.Database.SqlQuery<GetCustomerViewModel>(strSql).ToList();
        }

        public List<AgentNameViewModel> SecondRepeat(List<string> agentIds, string licenseno, string vinNo, int typeId)//改成参数化查询
        {
            //List<AgentNameViewModel>
            StringBuilder sbId = new StringBuilder();
            agentIds.ForEach(l => sbId.Append(l + ","));
            string sqlWhere = " where bx_agent.Id in ({0}) and  istest=0";
            if (typeId == 1)
            {
                sqlWhere += " and bx_userinfo.LicenseNo='{1}'";
            }
            else
            {
                sqlWhere += " and bx_userinfo.CarVIN='{2}'";
            }
            string sql = @"SELECT bx_agent.AgentName,bx_agent.Mobile,bx_agent.Id AS AgentId ,bx_agent.TopAgentId,bx_userinfo.Id  AS Buid  FROM bx_agent INNER JOIN bx_userinfo ON CAST(bx_agent.id AS CHAR)=bx_userinfo.agent";
            sql += sqlWhere + " ORDER BY bx_userinfo.updatetime asc";
            sql = string.Format(sql, sbId.Remove(sbId.Length - 1, 1), licenseno, vinNo);
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentNameViewModel>(sql).ToList();
        }

        public List<UpdateCustomerStatusAndCategoriesJson> GetCustomerStatusAndCategories(List<long> userIds)//改成参数化查询
        {
            string buidStr = string.Join(",", userIds);
            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format(@"   
             select bx_customercategories.CategoryInfo as OldCategoryInfo,bx_userinfo.IsReView as OldStatus,bx_userinfo.Id as UserId from  bx_userinfo LEFT JOIN bx_customercategories ON bx_userinfo.CategoryInfoId=bx_customercategories.Id   WHERE bx_userinfo.Id IN({0})
             ", buidStr);
                return _dbContext.Database.SqlQuery<UpdateCustomerStatusAndCategoriesJson>(sql).ToList();
            }
        }

        public AgentNameViewModel GetTopData(int topagentId)//改成参数化查询
        {
            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format(@"   
             select bx_customercategories.CategoryInfo as OldCategoryInfo,bx_userinfo.IsReView as OldStatus,bx_userinfo.Id as UserId from  bx_userinfo LEFT JOIN bx_customercategories ON bx_userinfo.CategoryInfoId=bx_customercategories.Id   WHERE bx_userinfo.Id ={0}
             ", topagentId);
                return _dbContext.Database.SqlQuery<AgentNameViewModel>(sql).ToList()[0];
            }


        }

        public bool DistributionData(int topAgentId, int agentId)
        {

            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format(@"   
             UPDATE bx_userinfo SET bx_userinfo.agent={0},bx_userinfo.OpenId={1},bx_userinfo.UpdateTime={2} WHERE bx_userinfo.agent={3}
             ", agentId, agentId.ToString().GetMd5(), DateTime.Now, topAgentId);
                return _dbContext.Database.ExecuteSqlCommand(sql) > 0;
            }

        }

        /// <summary>
        /// 获得普通用户报价次数
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="agent"></param>
        /// <param name="QuoteLimit"></param>
        /// <returns></returns>
        public int GetQuoteLimitCount(string openid, string agent, int QuoteLimit)
        {

            string sql = " SELECT 1 AS TempCount FROM bx_userInfo WHERE  OpenId = @OpenId AND Agent = @Agent LIMIT @QuoteLimit";

            var sqlParams = new List<MySqlParameter>()
            {
               new MySqlParameter
                             {
                               MySqlDbType = MySqlDbType.VarChar,
                               ParameterName = "OpenId",
                               Value = openid
                             },
                              new MySqlParameter{
                               MySqlDbType = MySqlDbType.VarChar,
                               ParameterName = "Agent",
                               Value = agent
                             },
                              new MySqlParameter{
                               MySqlDbType = MySqlDbType.Int32,
                               ParameterName = "QuoteLimit",
                               Value = QuoteLimit
                             }
            };
            int num = 0;
            var list = DataContextFactory.GetDataContext().Database.SqlQuery<UserInfoQuote>(sql, sqlParams.ToArray()).ToList();
            if (list == null)
            {
                return num;
            }
            return num = list.Count;
        }

        public List<bx_userinfo> GetUserInfoByLicenseNo(string LicenseNo, int AgentId)
        {
            if (AgentId > 0)
            {
                return _context.bx_userinfo.Where(x => x.LicenseNo == LicenseNo && x.IsTest == 0 && x.agent_id == AgentId).ToList();
            }
            return _context.bx_userinfo.Where(x => x.LicenseNo == LicenseNo && x.IsTest == 0).ToList();
        }

        public List<bx_userinfo> GetUserinfoByLicenseAndAgent(long buid, string licenseNo, List<string> agentIds)
        {
            try
            {
                var model = _context.bx_userinfo
                    .Where(x => x.IsTest == 0
                        && x.RenewalCarType == 0
                        && x.LicenseNo == licenseNo
                        && agentIds.Contains(x.Agent))
                    .OrderByDescending(o => o.UpdateTime).ToList();
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<bx_userinfo>();
        }

        /// <summary>
        /// 根据车架号代理人获取重复记录
        /// </summary>
        /// <param name="carvin"></param>
        /// <param name="engino"></param>
        /// <param name="agentIds"></param>
        /// <returns></returns>
        public List<bx_userinfo> GetUserinfoByCarVinAndAgent(string carvin, string engino, List<string> agentIds)
        {
            try
            {
                var model = _context.bx_userinfo
                    .Where(x => x.IsTest == 0
                        && x.RenewalCarType == 0
                        && x.CarVIN == carvin
                        && x.EngineNo == engino
                        && agentIds.Contains(x.Agent))
                    .OrderByDescending(o => o.UpdateTime).ToList();
                return model;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<bx_userinfo>();
        }

        /// <summary>
        /// 要删除记录的agendid集合 
        /// 齐大康 2018-04-23
        /// </summary>
        /// <param name="Buids"></param>
        /// <returns></returns>
        public string GetDelAgentIdsByBuids(string Buids)
        {
            string delAgentIds = string.Empty;
            string sql = " SELECT DISTINCT agent FROM bx_userinfo  WHERE id IN (" + Buids + ")";
            var list = DataContextFactory.GetDataContext().Database.SqlQuery<string>(sql).ToList();
            if (list == null || list.Count == 0)
            {
                return delAgentIds;
            }
            delAgentIds = string.Join(",", list);
            return delAgentIds;
        }

        /// <summary>
        /// 根据条件查询客户列表重复数据
        /// </summary>
        /// <param name="childAgent"></param>
        /// <param name="topAgentId"></param>
        /// <param name="groupByType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserInfoRepeatModel> GetCustomerRepeatList(int childAgent, int topAgentId, int groupByType, int pageIndex, int pageSize, string agentContidion, string joinCondition, string agentWhere)
        {
            try
            {
                string joinSql = " INNER JOIN bx_agent ON bx_agent.Id = ui.Agent ";
                joinSql = joinSql + joinCondition;
                string sql = string.Empty;
                StringBuilder sqlBuilder = new StringBuilder();
                #region SQL主体
                sqlBuilder.Append(" SELECT u.Id AS Buid, u.LicenseNo,u.LicenseOwner,u.CarVIN,u.MoldName ");
                sqlBuilder.Append(" ,DATE_FORMAT(u.RegisterDate, '%Y-%m-%d %H:%i:%s') AS RegisterDate,DATE_FORMAT(cr.next_review_date, '%Y-%m-%d %H:%i:%s') AS NextReviewDate ");
                sqlBuilder.Append(" ,bx_agent.AgentName ");
                if (groupByType == 3)
                {
                    sqlBuilder.Append(" ,buri.client_mobile AS ClientMobile,buri.client_name AS ClientName ");
                }
                sqlBuilder.Append(" FROM bx_userinfo u ");
                sqlBuilder.Append(" LEFT JOIN bx_agent ON u.Agent=bx_agent.Id ");
                sqlBuilder.Append(" LEFT JOIN bx_consumer_review cr ON u.Id=cr.b_uid AND cr.IsDeleted=0 ");
                #endregion

                //客户电话和客户名称
                if (groupByType == 3)
                {
                    #region 客户电话和客户名称
                    sqlBuilder.Append(" INNER JOIN bx_userinfo_renewal_info buri ON u.Id=buri.b_uid ");
                    sqlBuilder.Append("{0}");
                    sqlBuilder.Append(" WHERE 1=1 AND u.IsTest=0 and u.Id>0 ");
                    sqlBuilder.Append(" AND (IFNULL(buri.client_name,'')<>'' OR IFNULL(buri.client_mobile,'')<>'') ");
                    sqlBuilder.Append(" and u.Id IN (SELECT b_uid FROM bx_userinfo_renewal_info WHERE CONCAT(IFNULL(client_name,''),IFNULL(client_mobile,'')) IN ");
                    sqlBuilder.Append(" (SELECT t.ClientNameMobile FROM (");
                    sqlBuilder.Append(" SELECT CONCAT(IFNULL(uri.client_name,''),IFNULL(uri.client_mobile,'')) AS ClientNameMobile FROM bx_userinfo_renewal_info uri ");
                    sqlBuilder.Append(" INNER JOIN bx_userinfo ui ON ui.Id=uri.b_uid ");
                    sqlBuilder.Append("{1}");
                    sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest = 0 AND ui.Id>0 and uri.Id>0 ");
                    sqlBuilder.Append(" AND (IFNULL(uri.client_name,'')<>'' OR IFNULL(uri.client_mobile,'')<>'') ");
                    sqlBuilder.Append("{2}");
                    sqlBuilder.Append(" GROUP BY CONCAT(IFNULL(uri.client_name,''),IFNULL(uri.client_mobile,'')) HAVING COUNT(CONCAT(IFNULL(uri.client_name,''),IFNULL(uri.client_mobile,'')))>1 LIMIT @RowIndex,@PageSize) AS t )) ");
                    #endregion
                }
                else if (groupByType == 2)//车架号
                {
                    #region 按车架号查重
                    sqlBuilder.Append(" WHERE 1=1 AND u.IsTest=0 and u.Id>0 ");
                    sqlBuilder.Append(" {0} ");
                    sqlBuilder.Append(" and u.Id IN (SELECT Id FROM bx_userinfo WHERE CarVIN IN (SELECT t.CarVIN FROM ( ");
                    sqlBuilder.Append(" SELECT ui.CarVIN FROM bx_userinfo ui ");
                    sqlBuilder.Append(" {1} ");
                    sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest = 0 AND ui.Id>0 ");
                    sqlBuilder.Append(" AND IFNULL(ui.CarVIN,'')<>'' ");
                    sqlBuilder.Append(" {2} ");
                    sqlBuilder.Append(" GROUP BY ui.CarVIN HAVING COUNT(ui.CarVIN)>1 LIMIT @RowIndex,@PageSize ) AS t ))");
                    #endregion
                }
                else//车牌号
                {
                    #region 按车牌号查重
                    sqlBuilder.Append(" WHERE 1=1 AND u.IsTest=0 and u.Id>0 ");
                    sqlBuilder.Append("{0}");
                    sqlBuilder.Append(" and u.Id IN (SELECT Id FROM bx_userinfo WHERE LicenseNo IN (SELECT t.LicenseNo FROM ( ");
                    sqlBuilder.Append(" SELECT ui.LicenseNo FROM bx_userinfo ui ");
                    sqlBuilder.Append("{1}");
                    sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest=0 AND ui.Id>0 ");
                    sqlBuilder.Append(" AND IFNULL(ui.LicenseNo,'')<>'' ");
                    sqlBuilder.Append("{2}");
                    sqlBuilder.Append(" GROUP BY ui.LicenseNo HAVING COUNT(ui.LicenseNo)>1 LIMIT @RowIndex,@PageSize) AS t )) ");
                    #endregion
                }
                //总sql语句
                sql = string.IsNullOrEmpty(agentContidion) ? string.Format(sqlBuilder.ToString(), agentWhere, joinSql, "") : string.Format(sqlBuilder.ToString(), agentWhere, "", agentContidion);
                #region 参数
                MySqlParameter[] paramsSql ={
                                              new MySqlParameter(){
                                                  ParameterName="@RowIndex",
                                                  MySqlDbType = MySqlDbType.Int32,
                                                  Value=(pageIndex-1)*pageSize
                                              },
                                              new MySqlParameter(){
                                                  ParameterName="@PageSize",
                                                  MySqlDbType = MySqlDbType.Int32,
                                                  Value=pageSize
                                              }                                            
                                          };
                #endregion

                return _context.Database.SqlQuery<UserInfoRepeatModel>(sql, paramsSql).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("GetCustomerRepeatList发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<UserInfoRepeatModel>();
        }

        /// <summary>
        ///根据查询条件获得重复条件总数量
        /// </summary>
        /// <param name="groupByType">分组类型：1.车牌号，2.车架号，3.客户电话和客户名称</param>
        /// <param name="search">查询条件对象</param>
        /// <param name="searchSqlBuilder">查询sql</param>
        /// <returns></returns>
        public int GetCustomerRepeatListCount(int pageIndex, int pageSize, int groupByType, string agentContidion, string joinCondition)
        {
            try
            {
                int showPageNum = 10;
                int elsepage = 0;
                if (pageIndex > showPageNum / 2 + 1)
                {
                    elsepage = showPageNum / 2 - 1;
                }
                else
                {
                    elsepage = showPageNum - pageIndex;
                }

                string joinSql = " INNER JOIN bx_agent ON bx_agent.Id = ui.Agent ";
                joinSql = joinSql + joinCondition;
                StringBuilder sqlBuilder = new StringBuilder();
                string sql = string.Empty;
                //根据客户电话和客户名称进行查重
                if (groupByType == 3)
                {
                    #region 根据客户名称和客户电话查询
                    sqlBuilder.Append(" SELECT '1' AS num  FROM bx_userinfo_renewal_info uri ");
                    sqlBuilder.Append(" INNER JOIN bx_userinfo ui ON ui.Id=uri.b_uid ");
                    sqlBuilder.Append("{0}");
                    sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest = 0 AND ui.Id>0 AND uri.id>0 ");
                    sqlBuilder.Append(" AND (IFNULL(uri.client_name,'')<>'' OR IFNULL(uri.client_mobile,'')<>'') ");
                    sqlBuilder.Append("{1}");
                    sqlBuilder.Append(" GROUP BY CONCAT(IFNULL(uri.client_name,''),IFNULL(uri.client_mobile,'')) HAVING COUNT(CONCAT(IFNULL(uri.client_name,''),IFNULL(uri.client_mobile,'')))>1 ");
                    #endregion
                }
                else if (groupByType == 2)//根据车架号查重
                {
                    #region 根据车架号查询
                    sqlBuilder.Append("SELECT ui.CarVIN FROM bx_userinfo ui ");
                    sqlBuilder.Append("{0}");
                    sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest = 0 AND ui.Id>0 ");
                    sqlBuilder.Append(" AND IFNULL(ui.CarVIN,'')<>'' ");
                    sqlBuilder.Append("{1}");
                    sqlBuilder.Append(" GROUP BY ui.CarVIN HAVING COUNT(ui.CarVIN)>1 ");
                    #endregion
                }
                else//根据车牌号查重
                {
                    #region 根据车牌号查询
                    sqlBuilder.Append("SELECT ui.LicenseNo FROM bx_userinfo ui ");
                    sqlBuilder.Append("{0}");
                    sqlBuilder.Append(" WHERE 1 = 1 AND ui.IsTest = 0  AND ui.Id > 0 ");
                    sqlBuilder.Append(" AND IFNULL(ui.LicenseNo,'')<>'' ");
                    sqlBuilder.Append("{1}");
                    sqlBuilder.Append(" GROUP BY ui.LicenseNo  HAVING COUNT(ui.LicenseNo) > 1 ");
                    #endregion
                }
                sqlBuilder.Append(" limit {2},{3} ");
                sql = string.IsNullOrEmpty(agentContidion) ? string.Format(sqlBuilder.ToString(), joinSql, "", pageIndex * pageSize, pageSize * elsepage) : string.Format(sqlBuilder.ToString(), "", agentContidion, pageIndex * pageSize, pageSize * elsepage);
                return _context.Database.SqlQuery<string>(sql).ToList().Count();
            }
            catch (Exception ex)
            {
                logError.Error("GetCustomerRepeatListCount发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return 0;
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddRangeUserInfoExpand(List<bx_userinfo_expand> list)
        {
            bool result = false;
            try
            {
                _context.bx_userinfo_expand.AddRange(list);
                result = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                logError.Error("AddRangeUserInfoExpand发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
        /// <summary>
        /// 根据buids获得客户列表集合
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<UserInfoPartModel> GetUserInfoList(List<long> buids)
        {
            try
            {
                return _context.bx_userinfo.Where(a => buids.Contains(a.Id) && (new[] { 0, 3 }.Contains(a.IsTest.Value)))
                    .Select(b => new UserInfoPartModel()
                    {
                        Buid = b.Id,
                        LicenseNo = b.LicenseNo,
                        AgentId = b.Agent,
                        IsTest = b.IsTest.Value
                    }).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("GetUserInfoList发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }
        /// <summary>
        /// 获得代理人和车牌号
        /// </summary>
        /// <param name="agentIds"></param>
        /// <param name="licenseNos"></param>
        /// <returns></returns>
        public List<UserInfoPartModel> FindByAgentsAndLicenseNos(string agentIds, string licenseNos)
        {
            try
            {
                string sql = "SELECT Agent as AgentId,LicenseNo FROM bx_userinfo WHERE IsTest=0 AND Agent IN (" + agentIds + ") AND LicenseNo IN(" + licenseNos + ") ";
                return _context.Database.SqlQuery<UserInfoPartModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("FindByAgentsAndLicenseNos发生异常: " + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// 从回收站批量撤销
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool BatchRevokeFiles(string buids)
        {
            var result = false;
            try
            {
                var modifySql = string.Format("update bx_userinfo set isTest=0 ,UpdateTime=now() where Id IN (" + buids + ")");
                var effectRow = _context.Database.ExecuteSqlCommand(modifySql);
                result = effectRow > 0;
            }
            catch (Exception ex)
            {
                logError.Error("BatchRevokeFiles发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 获得下级存在车牌号的数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        public List<AgentNameViewModel> GetJuniorRepeat(int topAgentId, int agentId, string licenseno, string vinNo, int TypeId)
        {
            StringBuilder sbId = new StringBuilder();
            sbId.Append("SELECT bx_agent.AgentName,bx_agent.Mobile,bx_agent.Id AS AgentId ,bx_agent.TopAgentId,bx_userinfo.Id  AS Buid ");
            sbId.Append(" FROM bx_agent INNER JOIN bx_userinfo ON CAST(bx_agent.id AS CHAR)=bx_userinfo.agent ");
            sbId.Append(" WHERE bx_agent.TopAgentId=@TopAgentId AND bx_userinfo.IsTest=0 AND bx_agent.IsUsed=1 ");
            if (agentId > 0)
            {
                sbId.Append(" AND bx_agent.Id=@AgentId ");
            }
            if (!string.IsNullOrEmpty(licenseno) && TypeId == 1)
            {
                sbId.Append(" AND bx_userinfo.LicenseNo=@LicenseNo ");
            }
            if (!string.IsNullOrEmpty(vinNo) && TypeId != 1)
            {
                sbId.Append(" AND bx_userinfo.CarVIN=@CarVIN ");
            }
            sbId.Append(" ORDER BY bx_userinfo.updatetime asc limit 1");
            #region 参数
            var sqlParams = new List<MySqlParameter>()
            {
                new MySqlParameter()
                {
                     ParameterName="TopAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.Int32
                 },
                new MySqlParameter()
                {
                     ParameterName="AgentId",
                     Value=agentId,
                     MySqlDbType=MySqlDbType.Int32
                 },
                new MySqlParameter()
                {
                     ParameterName="LicenseNo",
                     Value=licenseno,
                     MySqlDbType=MySqlDbType.VarChar
                 },
                new MySqlParameter()
                {
                     ParameterName="CarVIN",
                     Value=vinNo,
                     MySqlDbType=MySqlDbType.VarChar
                 }
            };
            #endregion
            return DataContextFactory.GetDataContext().Database.SqlQuery<AgentNameViewModel>(sbId.ToString(), sqlParams.ToArray()).ToList();
        }

        public List<bx_userinfo> GetUserListByBuid(List<long> buids)
        {
            try
            {
                List<bx_userinfo> list = _context.bx_userinfo.Where(a => a.IsTest == 0 && buids.Contains(a.Id)).ToList();
                return list;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<bx_userinfo>();
        }
        /// <summary>
        /// 获取bx_userinfo 表最大Id
        /// </summary>
        /// <returns></returns>
        public long GetMaxBuid()
        {
            try
            {
                string sql = "SELECT MAX(Id) FROM bx_userinfo ";
                long maxId = _context.Database.SqlQuery<long>(sql).FirstOrDefault<long>();
                return maxId;
            }
            catch (Exception ex)
            {
                logError.Error("回去最大buid发生异常：" + ex);
            }
            return int.MaxValue;
        }
        /// <summary>
        /// 获取bx_userinfo数据
        /// </summary>
        /// <param name="startBuid"></param>
        /// <param name="endBuid"></param>
        /// <returns></returns>
        public List<UserInfoModel2> GetDefeatHistoryUserList(long startBuid, long endBuid)
        {
            try
            {
                string sql = "SELECT ui.BuId, ui.Agent,ui.OpenId,ui.LicenseNo,ui.SixDigitsAfterIdCard,ui.RenewalCarType,ui.CityCode,ui.CarVIN,ui.EngineNo,a.TopAgentId FROM bx_userinfo ui INNER JOIN bx_agent a ON ui.Agent=a.Id WHERE a.IsUsed=1 AND ui.IsTest=0 and ui.Id BETWEEN " + startBuid + " AND " + endBuid;
                List<UserInfoModel2> list = _context.Database.SqlQuery<UserInfoModel2>(sql).ToList();
                return list;
            }
            catch (Exception ex)
            {
                logError.Error("startBuid=" + startBuid + ", endBuid=" + endBuid + ";发生异常：" + ex);
            }
            return null;
        }
        public List<UserInfoModel2> GetUserListByQuotationReceipt(long startBuid, long endBuid)
        {
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append(" SELECT ui.id as BuId,ui.Agent,ui.OpenId,ui.LicenseNo,ui.SixDigitsAfterIdCard,ui.RenewalCarType,ui.CityCode,ui.CarVIN,ui.EngineNo,ba.TopAgentId ");
                sqlBuilder.Append(" FROM bx_car_order AS bc ");
                sqlBuilder.Append(" LEFT JOIN bx_userinfo AS ui ");
                sqlBuilder.Append(" ON bc.buid=ui.id ");
                sqlBuilder.Append(" LEFT JOIN bx_customercategories AS cc ");
                sqlBuilder.Append(" ON ui.CategoryInfoId=cc.id ");
                sqlBuilder.Append(" LEFT JOIN bx_order_userinfo AS bcu ");
                sqlBuilder.Append(" ON bc.id=bcu.OrderId ");
                sqlBuilder.Append(" LEFT JOIN bd_baodaninfo_carorder_mapping AS bdm ");
                sqlBuilder.Append(" ON bc.id=bdm.CarOrderId ");
                sqlBuilder.Append(" LEFT JOIN bd_baodaninfo AS bdx ");
                sqlBuilder.Append(" ON bdm.BaoDanInfoId=bdx.Id ");
                sqlBuilder.Append(" LEFT JOIN bx_agent AS ba ");
                sqlBuilder.Append(" ON bcu.agent=ba.Id ");
                sqlBuilder.Append(" WHERE ui.Id BETWEEN " + startBuid + " AND " + endBuid);
                sqlBuilder.Append(" ORDER BY bc.create_time DESC ");

                return _context.Database.SqlQuery<UserInfoModel2>(sqlBuilder.ToString()).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("startBuid=" + startBuid + ", endBuid=" + endBuid + ";发生异常：" + ex);
            }
            return null;
        }
        public List<int> GetUserByAgentId(List<int> agentIds)
        {
            try
            {
                string sql = "SELECT DISTINCT Agent FROM bx_userinfo WHERE istest=0 AND agent IN ('" + string.Join("','", agentIds) + "');";
                List<int> list = _context.Database.SqlQuery<int>(sql).ToList<int>();
                return list;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<int>();
        }
        /// <summary>
        /// 根据主键获取baodanxinxi数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public dz_baodanxinxi BaoDanXinXiModel(int id)
        {
            try
            {
                var model = _context.dz_baodanxinxi.Where(a => a.Id == id).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                logError.Error("BaoDanXinXiModel发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        public dz_baodanxinxi BaoDanXinXiModelByRecDuid(string recGuid)
        {
            try
            {
                #region 字段
                string sql = @"select t1.`Id`, 
	                t1.`CarOwner`, 
	                t1.`BizStartDate`, 
	                t1.`BizEndDate`, 
	                t1.`BizNum`, 
	                t1.`CompanyId`, 
	                t1.`ForceStartDate`, 
	                t1.`ForceEndDate`, 
	                t1.`ForceNum`, 
	                t1.`ChargesDate`, 
	                t1.`PrintDate`, 
	                t1.`InsuredName`, 
	                t1.`InsureSex`, 
	                t1.`InsureBirth`, 
	                t1.`InsureIdType`, 
	                t1.`InsureIdNum`, 
	                t1.`CarLicense`, 
	                t1.`CarBrandModel`, 
	                t1.`CarEngineNo`, 
	                t1.`CarVIN`, 
	                t1.`CarDisplacement`, 
	                t1.`CarUsedType`, 
	                t1.`CarOwnerType`, 
	                t1.`CarRegisterDate`, 
	                t1.`CarType`, 
	                t1.`CarTravelArea`, 
	                t1.`CarSeated`, 
	                t1.`CarPrice`, 
	                t1.`CarAvgMileage`, 
	                t1.`AgentId`, 
	                t1.`AgentName`, 
	                t1.`PrinterName`, 
	                t1.`PrinterId`, 
	                t1.`ChannelId`, 
	                t1.`ChannelName`, 
	                t1.`CreateTime`, 
	                t1.`OperatingID`, 
	                t1.`OperatingName`, 
	                t1.`BizRate`, 
	                t1.`ForceRate`, 
	                t1.`BizPrice`, 
	                t1.`ForcePrice`, 
	                t1.`TaxPrice`, 
	                t1.`IsSuccess`, 
	                t1.`QuDaoMingCheng`, 
	                t1.`SaleId`, 
	                t1.`SaleName`, 
	                t1.`OwnerId`, 
	                t1.`OwnerName`, 
	                t1.`InputDate`, 
	                t1.`ArrivalDate`, 
	                t1.`IsNewCar`, 
	                t1.`IsGroup`, 
	                t1.`ForcPrintDate`, 
	                t1.`UKeyId`, 
	                t1.`is_duizhang`, 
	                t1.`ErrorMessgae`, 
	                t1.`BaoDanType`, 
	                t1.`UpdateTime`, 
	                t1.`ForceInputDate`, 
	                t1.`ForceArrivalDate`, 
	                t1.`PolicyHoderName`, 
	                t1.`PolicyHoderIdType`, 
	                t1.`PolicyHoderIdNum`, 
	                t1.`PolicyHoderMoblie`, 
	                t1.`InsureMoblie`, 
	                t1.`NonClaimRate`, 
	                t1.`MultiDiscountRate`, 
	                t1.`AvgMileRate`, 
	                t1.`RiskRate`, 
	                t1.`CarEquQuality`, 
	                t1.`CarTonCount`, 
	                t1.`CarLicenseType`, 
	                t1.`CarOwnerIdNo`, 
	                t1.`CarOwnerIdNoType`, 
	                t1.`CarLicenseColor`, 
	                t1.`CarClauseType`, 
	                t1.`CarFuelType`, 
	                t1.`CarProofType`, 
	                t1.`InsureIdTypeValue`, 
	                t1.`PolicyHoderIdTypeValue`, 
	                t1.`CarOwnerIdNoTypeValue`, 
	                t1.`CarUsedTypeValue`, 
	                t1.`CarTypeValue`, 
	                t1.`CarTravelAreaValue`, 
	                t1.`CarLicenseTypeValue`, 
	                t1.`CarLicenseColorValue`, 
	                t1.`ClauseTypeValue`, 
	                t1.`CarFuelTypeValue`, 
	                t1.`CarProofTypeValue`, 
	                t1.`SubCarType`, 
	                t1.`SubCarTypeValue`, 
	                t1.`BiztNo`, 
	                t1.`ForcetNo`, 
	                t1.`ReturnInsured`, 
	                t1.`LastPolicyNo`, 
	                t1.`PolicyPrintNo`, 
	                t1.`SigningDate`, 
	                t1.`SubmitDate`, 
	                t1.`CardDate`, 
	                t1.`OwnerPeople`, 
	                t1.`VehiclePeople`, 
	                t1.`BatchNo`, 
	                t1.`QuDaoCode`, 
	                t1.`TransferDate`, 
	                t1.`ServiceLife`, 
	                t1.`BaoDanNature`, 
	                t1.`LastYearAccTimes`, 
	                t1.`LastYearClaimAmount`, 
	                t1.`LastYearClaimTimes`, 
	                t1.`NoDefendedYear`, 
	                t1.`CurrentYearClaimTimes`, 
	                t1.`OwnerEmail`, 
	                t1.`InsuredEmail`, 
	                t1.`PolicyHoderEmail`, 
	                t1.`ForceChargesDate`, 
	                t1.`Guid`, 
	                t1.`OwnerMobile`, 
	                t1.`LastBizpNo`, 
	                t1.`LastForcepNo`, 
	                t1.`IncludeTaxTotal`, 
	                t1.`CleanTotal`, 
	                t1.`ShuiJinTotal`, 
	                t1.`DzSaleName`, 
	                t1.`DzPracticeNo`, 
	                t1.`DataSource`, 
	                t1.`IsHaveLicenseNo`, 
	                t1.`co_real_value`, 
	                t1.`Organization` "; 
                #endregion
                sql += " from dz_baodanxinxi as t1 INNER JOIN dz_reconciliation as t2 on t1.id=t2.baodanxinxi_id where t2.guid='" + recGuid + "';";
                var model = _context.Database.SqlQuery<dz_baodanxinxi>(sql).ToList().FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                logError.Error("BaoDanXinXiModelByRecDuid发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }

        public dz_reconciliation ReconciliationModel(string recGuid)
        {
            try
            {
                var model = _context.dz_reconciliation.Where(a => a.guid == recGuid).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                logError.Error("ReconciliationModel发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }
        private bool UpdateCorrectLicensePlate(long buid, int clpId)
        {
            bool result = false;
            try
            {
                string sql = " UPDATE dz_correct_license_plate SET bu_id="+buid+" where id=" + clpId;
                result= _context.Database.ExecuteSqlCommand(sql)>0;
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常：" + ex);
            }
            return result;
        }
        public bool AddUserInfo(bx_userinfo model, int clpId, dz_baodanxinxi baodanxinxiModel)
        {
            bool result = false;
            try
            {
                _context.bx_userinfo.Add(model);
                result =_context.SaveChanges() > 0;
                if (result&&clpId>0)
                {
                    UpdateCorrectLicensePlate(model.Id, clpId);
                    bx_quotereq_carinfo temp = AddQuotereqCarinfo(model, baodanxinxiModel);
                    if (temp!=null&&temp.id<=0&&temp.b_uid>0)
                    {
                        _context.bx_quotereq_carinfo.Add(temp);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logError.Error("bx_userinfo="+JsonHelper.Serialize(model)+ ";clpId="+ clpId + ";baodanxinxiModel="+JsonHelper.Serialize(baodanxinxiModel) +";AddUserInfo发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
        private bx_quotereq_carinfo AddQuotereqCarinfo(bx_userinfo userInfoModel,dz_baodanxinxi baodanxinxiModel)
        {
            bx_quotereq_carinfo model = new bx_quotereq_carinfo();
            try
            {
                bx_quotereq_carinfo temp = _context.bx_quotereq_carinfo.Where(a => a.b_uid == userInfoModel.Id).FirstOrDefault();
                if (temp != null && temp.id > 0)
                {
                    return temp;
                }
                model = new bx_quotereq_carinfo()
                {
                    b_uid = userInfoModel.Id,                    
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    transfer_date = baodanxinxiModel.TransferDate,
                    biz_end_date = baodanxinxiModel.BizEndDate,
                    force_end_date = baodanxinxiModel.ForceEndDate,
                    PriceT = baodanxinxiModel.CarPrice,
                    ownerOrg = baodanxinxiModel.Organization,
                    auto_model_code_source=-1,
                    is_newcar=2,//批改车流转到客户列表后是旧车
                    is_lastnewcar =1,
                    car_used_type=1,
                    //car_type=1
                };
                if (!string.IsNullOrEmpty(baodanxinxiModel.CarSeated))
                {
                    model.seat_count = int.Parse(baodanxinxiModel.CarSeated);
                }
                if (!string.IsNullOrEmpty(baodanxinxiModel.CarDisplacement))
                {
                    model.exhaust_scale = Math.Round(Convert.ToDecimal(baodanxinxiModel.CarDisplacement), 5, MidpointRounding.AwayFromZero);
                    //model.exhaust_scale = Convert.ToDecimal(baodanxinxiModel.CarDisplacement);
                }
                if (!string.IsNullOrEmpty(baodanxinxiModel.CarEquQuality))
                {
                    model.car_equ_quality = Math.Round(Convert.ToDecimal(baodanxinxiModel.CarEquQuality), 2, MidpointRounding.AwayFromZero);
                }
                if (!string.IsNullOrEmpty(baodanxinxiModel.CarTonCount))
                {
                    model.car_ton_count = Math.Round(Convert.ToDecimal(baodanxinxiModel.CarTonCount), 2, MidpointRounding.AwayFromZero);
                    //model.car_ton_count = Convert.ToDecimal(baodanxinxiModel.CarTonCount);
                }
                if (!string.IsNullOrEmpty(baodanxinxiModel.CarProofTypeValue))
                {
                    model.proof_type =int.Parse(baodanxinxiModel.CarProofTypeValue);
                }
                //if (!string.IsNullOrEmpty(baodanxinxiModel.CarUsedTypeValue))
                //{
                //    model.car_used_type =int.Parse(baodanxinxiModel.CarUsedTypeValue);
                //}
                if (baodanxinxiModel.BizRate.HasValue)
                {                    
                    model.ActualDtaryBizRatio = Math.Round(Convert.ToDecimal(baodanxinxiModel.BizRate.Value), 4, MidpointRounding.AwayFromZero);
                }
                if (baodanxinxiModel.ForceRate.HasValue)
                {                    
                    model.ActualDtaryForceRatio = Math.Round(Convert.ToDecimal(baodanxinxiModel.ForceRate.Value), 4, MidpointRounding.AwayFromZero);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常:"+ex);
            }
            return model;
        }
        /// <summary>
        /// 将已批改车牌添加到bx_userinfo
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool AddUserInfo(bx_userinfo model)
        {

            try
            {
                _context.bx_userinfo.Add(model);
                return _context.SaveChanges() > 0;
                
            }
            catch (Exception ex)
            {
                logError.Error("AddUserInfo发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
        /// <summary>
        /// 是否存在相同数据
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <param name="openId"></param>
        /// <param name="agentId"></param>
        /// <param name="RenewalCarType"></param>
        /// <returns></returns>
        public bx_userinfo GetUserInfoByCondition(string licenseNo, string openId, string agentId, int renewalCarType)
        {
            try
            {
                return _context.bx_userinfo.Where(a => a.LicenseNo == licenseNo && a.OpenId == openId && a.Agent == agentId && a.RenewalCarType == renewalCarType && a.IsTest == 0).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Error("IsExistsUserInfo发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }
        /// <summary>
        /// 更新表中存在的数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserInfo(bx_userinfo model)
        {
            try
            {
                _context.Entry<bx_userinfo>(model).State = EntityState.Modified;
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                logError.Error("UpdateUserInfo发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
        public dz_correct_license_plate GetCorrectLicensePlate(int id)
        {
            dz_correct_license_plate model = new dz_correct_license_plate();
            try
            {
                //string sql = "SELECT * FROM dz_correct_license_plate WHERE car_vin=" + engineNo + " AND car_engin_no=" + carVIN;
                model = _context.dz_correct_license_plate.Where(a => a.id==id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
            }
            return model;
        }
        public dz_correct_license_plate GetCorrectLicensePlate(string engineNo, string carVIN)
        {
            dz_correct_license_plate model = new dz_correct_license_plate();
            try
            {
                //string sql = "SELECT * FROM dz_correct_license_plate WHERE car_vin=" + engineNo + " AND car_engin_no=" + carVIN;

                model= _context.dz_correct_license_plate.Where(a=>a.car_vin==carVIN&&a.car_engine_no==engineNo).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
            }
            return model;
        }

        public List<GetCustomerViewModel> GetUserByVehLicense(string agentWhere, string carVin, string engineNo)
        {
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append(string.Format("SELECT ui.*,{0} as LastForceEndDate ,{1} as LastBizEndDate ",
                CompareBatchAndRenewalDateHelpler.GetLastForceEndDate(),
                CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                sqlBuilder.Append(string.Format(",{0} AS ValueLastForceEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)));
                sqlBuilder.Append(string.Format(",{0} AS ValueLastBizEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)));
                sqlBuilder.Append(" FROM ");
                sqlBuilder.Append(" bx_car_renewal cr");
                sqlBuilder.Append(" RIGHT JOIN");
                sqlBuilder.Append(" bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id");
                sqlBuilder.Append(" RIGHT JOIN");
                sqlBuilder.Append(" bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid");
                sqlBuilder.Append(" LEFT JOIN");
                sqlBuilder.Append(" bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId");
                sqlBuilder.Append(" AND bx_batchrenewal_item.IsNew = 1");
                sqlBuilder.Append(" AND bx_batchrenewal_item.IsDelete = 0 ");
                sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest=0 ");
                sqlBuilder.Append(agentWhere);
                //modify 2018-11-09
                //if (!string.IsNullOrEmpty(licenseNo))
                //{
                //    sqlBuilder.Append("AND ui.LicenseNo=@LicenseNo ");
                //}
                if (!string.IsNullOrEmpty(carVin))
                {
                    sqlBuilder.Append("AND ui.CarVIN=@CarVIN ");
                }
                if (!string.IsNullOrEmpty(engineNo))
                {
                    sqlBuilder.Append("AND ui.EngineNo=@EngineNo");
                }
                //测试
                //sqlBuilder.Append(" limit 15");
                #region 参数
                List<MySqlParameter> sqlParams = new List<MySqlParameter>()
                {
                   //new MySqlParameter() { ParameterName="@LicenseNo", MySqlDbType = MySqlDbType.VarChar, Value=licenseNo },
                   new MySqlParameter() { ParameterName="@CarVIN", MySqlDbType = MySqlDbType.VarChar, Value=carVin },
                   new MySqlParameter() { ParameterName="@EngineNo",MySqlDbType = MySqlDbType.VarChar, Value=engineNo}
                };
                #endregion

                return _context.Database.SqlQuery<GetCustomerViewModel>(sqlBuilder.ToString(), sqlParams.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：CarVin=" + carVin + ";EngineNo=" + engineNo + Environment.NewLine + ex);
            }
            return new List<GetCustomerViewModel>();
        }

        public List<GetCustomerViewModel> GetUserByVehLicense(int buid)
        {
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append(string.Format("SELECT ui.*,{0} as LastForceEndDate ,{1} as LastBizEndDate ",
                CompareBatchAndRenewalDateHelpler.GetLastForceEndDate(),
                CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()));
                sqlBuilder.Append(string.Format(",{0} AS ValueLastForceEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(0)));
                sqlBuilder.Append(string.Format(",{0} AS ValueLastBizEndDate ",
                    CompareBatchAndRenewalDateHelpler.GetValueLastEndDate(1)));
                sqlBuilder.Append(" FROM ");
                sqlBuilder.Append(" bx_car_renewal cr");
                sqlBuilder.Append(" RIGHT JOIN");
                sqlBuilder.Append(" bx_userinfo_renewal_index uri ON cr.Id = uri.car_renewal_id");
                sqlBuilder.Append(" RIGHT JOIN");
                sqlBuilder.Append(" bx_userinfo ui FORCE INDEX (IDX_AGENT_ISTEST_UPTIME_RTYPE_ISDISTRIBUTED_RSTATUS) ON ui.Id = uri.b_uid");
                sqlBuilder.Append(" LEFT JOIN");
                sqlBuilder.Append(" bx_batchrenewal_item ON ui.Id = bx_batchrenewal_item.BUId");
                sqlBuilder.Append(" AND bx_batchrenewal_item.IsNew = 1");
                sqlBuilder.Append(" AND bx_batchrenewal_item.IsDelete = 0 ");
                sqlBuilder.Append(" WHERE 1=1 AND ui.IsTest=0 ");
                sqlBuilder.Append(" and ui.id=" + buid);
                
                return _context.Database.SqlQuery<GetCustomerViewModel>(sqlBuilder.ToString()).ToList();
            }
            catch (Exception ex)
            {
                logError.Error("发生异常："+ ex);
            }
            return new List<GetCustomerViewModel>();
        }
        public bx_userinfo GetCustomerById(long buid)
        {
            try
            {
                return _context.bx_userinfo.Where<bx_userinfo>(a=>a.Id==buid).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Error("发送异常：" + ex);
            }
            return new bx_userinfo();
        }

        /// <summary>
        /// 插入bx_userinfo
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public long Add(bx_userinfo userinfo)
        {
            bx_userinfo item = new bx_userinfo();
            try
            {
                item = DataContextFactory.GetDataContext().bx_userinfo.Add(userinfo);
                var returnResult = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return item.Id;
        }
        /// <summary>
        /// 根据车牌、custkey、代理人获取一条记录
        /// </summary>
        /// <param name="licenseno"></param>
        /// <param name="custkey"></param>
        /// <param name="agent"></param>
        /// <param name="renewaltype"></param>
        /// <param name="renewalcartype"></param>
        /// <returns></returns>
        public bx_userinfo Find(string licenseno, string custkey, string agent, int renewalcartype)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {
                var result = DataContextFactory.GetDataContext().bx_userinfo.Where(x => x.Agent == agent && x.IsTest == 0 && x.RenewalCarType == renewalcartype && x.LicenseNo == licenseno && x.OpenId == custkey);
                tt = result.OrderByDescending(l => l.UpdateTime).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tt;
        }
    }
}