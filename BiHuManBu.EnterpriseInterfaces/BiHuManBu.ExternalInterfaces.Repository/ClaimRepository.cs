using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ClaimRepository: IClaimRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private ILog logInfo = LogManager.GetLogger("INFO");

        /// <summary>
        /// 获取订单跟进记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<FollowDetail> GetOrderFollow(int orderId)
        {
            var sql = string.Format(@"SELECT * FROM (
SELECT * FROM (
SELECT c.AgentName,b.CurrentAgentId AS 'AgentId',d.BusinessName,a.State,DATE_FORMAT(a.CreateTime,'%Y-%m-%d %H:%i:%s') CreateTime
FROM tx_cluefollowuprecord a 
INNER JOIN tx_clues b ON a.clueid=b.id
LEFT JOIN bx_agent c ON c.Id = b.CurrentAgentId
LEFT JOIN tx_agent d ON d.AgentId = b.agentid
WHERE a.clueid={0} AND a.Deleted=0 AND a.state IN (3,8,9,10)
) AS t
UNION ALL
SELECT * FROM (
SELECT bg.AgentName,cu.CurrentAgentId AS 'AgentId',tg.BusinessName,tc.State,DATE_FORMAT(tc.CreateTime,'%Y-%m-%d %H:%i:%s') CreateTime 
FROM tx_cluefollowuprecord tc
INNER JOIN  tx_clues cu ON tc.clueid=cu.id 
LEFT JOIN bx_agent bg ON bg.Id = cu.CurrentAgentId
LEFT JOIN tx_agent tg ON tg.AgentId =cu.agentid
WHERE tc.clueid={0} AND tc.Deleted=0 AND tc.state=-1 ORDER BY tc.CreateTime DESC LIMIT 1) AS t1
) AS tp
ORDER BY tp.CreateTime DESC ", orderId);
            try
            {
                return db.Database.SqlQuery<FollowDetail>(sql).ToList();
            }
            catch
            {
                return new List<FollowDetail>();
            }

        }

        /// <summary>
        /// pc 获取订单列表
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="totalCount">总数</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="licenseNo">车牌</param>
        /// <param name="mobile">车主手机号</param>
        /// <param name="salesman">业务员名称或id</param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="times">开始提交时间</param>
        /// <param name="endCreateTime">结束提交时间</param>
        /// <param name="salesmanMoblie">业务员手机号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<PcClaimOrderModel> GetPcClaimOrderPage(int agentId, out int totalCount, int orderState, string licenseNo = "", string mobile = "", string salesman = "", string orderNo = "", string startCreateTime = "", string endCreateTime = "", string salesmanMoblie = "", int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var ids = GetChildAgentId(agentId);
                var sql = @"SELECT cu.id AS 'OrderId',cu.OrderNum AS 'OrderNo',cu.licenseno AS 'LicenseNo',cb.Name AS 'BrandName',cu.CarOwner,cu.mobile AS 'Mobile',cu.IsMany,cu.IsDrivering,cu.Only4s,cu.FromAgentName
         AS 'BusinessName',cu.casetype AS 'CaseType',cu.followupstate AS 'OrderState',cu.MaintainAmount,cu.CreateTime,cu.CurrentAgentId as 'AgentId',bg.AgentName,bg.Mobile as 'SalesmanMoblie' FROM tx_clues cu
        LEFT JOIN tx_carbrands cb ON cu.ChosedModelId = cb.BrandId
        LEFT JOIN bx_agent bg ON bg.Id = cu.CurrentAgentId
        WHERE cu.ClueFromType <> 1 AND cu.CurrentAgentId IN (" + ids + ")  ";

                if (orderState != 0)
                {
                    if (orderState == 26)//待接单包括：待接单、匹配失败、接单超时、已提交 
                    {
                        sql = sql + " AND cu.followupstate in(21,22,23,26) ";
                    }
                    else if (orderState == 24)//无人接单包括：指派失败、流失、无人接单
                    {
                        sql = sql + " AND cu.followupstate in(4,24,25) ";
                    }
                    else if (orderState == -1)//已接单包括：已接单、跟进中、上门接车、发短信、打电话、店内接待
                    {
                        sql = sql + " AND cu.followupstate in(-1,1,2,5,6,7) ";
                    }
                    else
                    {
                        sql = sql + " AND cu.followupstate = ?orderState ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(licenseNo))
                {
                    licenseNo = licenseNo.Trim();
                    sql = sql + " AND cu.licenseno like ?licenseNo  ";
                }
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    mobile = mobile.Trim();
                    sql = sql + " AND cu.mobile LIKE ?mobile  ";
                }
                if (!string.IsNullOrWhiteSpace(salesman))
                {
                    salesman = salesman.Trim();
                    Regex reg = new Regex("^[0-9]+$");
                    Match ma = reg.Match(salesman);
                    if (ma.Success)
                    {
                        sql = sql + " AND cu.CurrentAgentId = ?salesman1 ";
                    }
                    else
                    {
                        sql = sql + " AND  bg.AgentName LIKE ?salesman2 ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    orderNo = orderNo.Trim();
                    sql = sql + " AND cu.OrderNum LIKE ?orderNo ";
                }
                if (!string.IsNullOrWhiteSpace(startCreateTime) && !string.IsNullOrWhiteSpace(endCreateTime))
                {
                    sql = sql + " AND DATE_FORMAT(cu.CreateTime,'%Y-%m-%d') between DATE_FORMAT(?startCreateTime,'%Y-%m-%d') and DATE_FORMAT(?endCreateTime,'%Y-%m-%d') ";
                }
                if (!string.IsNullOrWhiteSpace(salesmanMoblie))
                {
                    salesmanMoblie = salesmanMoblie.Trim();
                    sql = sql + " AND bg.Mobile LIKE ?salesmanMoblie ";
                }

                sql = sql + " ORDER BY cu.CreateTime DESC ";
                List<MySqlParameter> paramList2 = new List<MySqlParameter>();
                //paramList2.Add(new MySqlParameter("?ids", MySqlDbType.VarChar) { Value = ids });
                paramList2.Add(new MySqlParameter("?orderState", MySqlDbType.Int32) { Value = orderState });
                paramList2.Add(new MySqlParameter("?licenseNo", MySqlDbType.VarChar) { Value = '%' + licenseNo + '%' });
                paramList2.Add(new MySqlParameter("?mobile", MySqlDbType.VarChar) { Value = '%' + mobile + '%' });
                paramList2.Add(new MySqlParameter("?salesman1", MySqlDbType.VarChar) { Value = salesman });
                paramList2.Add(new MySqlParameter("?salesman2", MySqlDbType.VarChar) { Value = '%' + salesman + '%' });
                paramList2.Add(new MySqlParameter("?orderNo", MySqlDbType.VarChar) { Value = '%' + orderNo + '%' });
                paramList2.Add(new MySqlParameter("?startCreateTime", MySqlDbType.DateTime) { Value = startCreateTime });
                paramList2.Add(new MySqlParameter("?endCreateTime", MySqlDbType.DateTime) { Value = endCreateTime });
                paramList2.Add(new MySqlParameter("?salesmanMoblie", MySqlDbType.VarChar) { Value = '%' + salesmanMoblie + '%' });
                totalCount = db.Database.SqlQuery<int>("select count(1) from (" + sql + ") as t ", paramList2.ToArray()).FirstOrDefault();
                sql = sql + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize + " ";
                var data = db.Database.SqlQuery<PcClaimOrderModel>(sql, paramList2.ToArray()).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        try
                        {
                            if (item.CreateTime != null && item.CreateTime > DateTime.MinValue)
                            {
                                item.CreateTimeString = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                totalCount = 0;
                LogHelper.Error("理赔pc端订单列表异常：" + ex);
                return new List<PcClaimOrderModel>();
            }
        }

        /// <summary>
        /// 获取pc端订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public PcOrderDetail GetPcOrderDetail(int orderId)
        {
            var result = new PcOrderDetail();
            if (orderId < 1)
            {
                return result;
            }
            try
            {
                var sql = @"SELECT cu.id AS 'OrderId',cu.OrderNum AS 'OrderNo',cu.licenseno AS 'LicenseNo',cb.Name AS 'BrandName',cu.CarOwner,cu.mobile AS 'Mobile',
	cu.IsMany,cu.IsDrivering,cu.Only4s,cu.FromAgentName
         AS 'BusinessName',cu.agentid AS 'BusinessId',cu.casetype AS 'CaseType',cu.followupstate AS 'OrderState',
         cu.MaintainAmount, IFNULL(cu.CreateTime,'0001-01-01 00:00:00') AS 'CreateTime',cu.CurrentAgentId AS 'AgentId',bg.AgentName,cu.sourcename AS 'SourceName',
         cu.ReceiveCarAddress,cu.ExpectedAddress,
        IFNULL((SELECT CreateTime AS 'CreateTime' FROM tx_cluefollowuprecord WHERE clueid=cu.id AND state =-1 LIMIT 1),'0001-01-01 00:00:00') AS 'ReceiveOrderTime',
          tx.Address AS 'AgentAddress'
          FROM tx_clues cu 
        LEFT JOIN tx_carbrands cb ON cu.ChosedModelId = cb.BrandId
        LEFT JOIN bx_agent bg ON bg.Id = cu.CurrentAgentId
        LEFT JOIN tx_agent tx ON tx.AgentId = cu.agentid
        WHERE cu.id=?id ";
                List<MySqlParameter> paramList = new List<MySqlParameter>();
                paramList.Add(new MySqlParameter("?id", MySqlDbType.Int32) { Value = orderId });
                result.DetailModel = db.Database.SqlQuery<PcClaimOrderDetailModel>(sql, paramList.ToArray()).FirstOrDefault();
                var orderFollow = GetOrderFollow(orderId);
                if (result.DetailModel != null && result.DetailModel.OrderId > 0)
                {
                    var model = new FollowDetail { AgentName = result.DetailModel.AgentName + "(" + result.DetailModel.AgentId + ")", CreateTime = result.DetailModel.CreateTime, State = -100 };
                    orderFollow.Add(model);
                    if (result.DetailModel.CreateTime != null && result.DetailModel.CreateTime != DateTime.MinValue)
                    result.DetailModel.CreateTimeString = result.DetailModel.CreateTime.ToString("yyyy-MM-dd HH:mm");
                    if (result.DetailModel.ReceiveOrderTime != null && result.DetailModel.ReceiveOrderTime != DateTime.MinValue)
                        result.DetailModel.ReceiveOrderTimeString = result.DetailModel.ReceiveOrderTime.ToString("yyyy-MM-dd HH:mm");
                }
                if (orderFollow != null)
                {
                    result.FollowDetail = orderFollow.OrderByDescending(o => o.CreateTime).ToList();
                    foreach (var item in result.FollowDetail)
                    {
                        item.CreateTimeString = item.CreateTime == null ? "" : item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                    }
                }
                result.MaintainPic = db.tx_clue_image.Where(o => o.ClueId == orderId && o.ImageType == 1 && o.Deleted == 1).Select(o => o.Url).ToList();

                if(result.DetailModel != null)
                    result.DetailModel.Score = (double)GetAgent(result.DetailModel.BusinessId).Score;
            }
            catch(Exception ex) 
            {
                LogHelper.Error("获取pc端订单详情:" + ex);
            }
            return result;
        }

        public tx_agent GetAgent(int topAgentId)
        {
            var agent = db.tx_agent.Where(x => x.AgentId == topAgentId).FirstOrDefault();
            return agent;
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<PcClaimOrderModel> ExportOrder(int agentId)
        {
            try
            {
                var ids = GetChildAgentId(agentId);
                var sql = @"SELECT cu.id AS 'OrderId',cu.OrderNum AS 'OrderNo',cu.licenseno AS 'LicenseNo',cb.Name AS 'BrandName',cu.CarOwner,cu.mobile AS 'Mobile',cu.IsMany,cu.IsDrivering,cu.Only4s,cu.FromAgentName
             AS 'BusinessName',cu.casetype AS 'CaseType',cu.followupstate AS 'OrderState',cu.MaintainAmount,cu.CreateTime,cu.CurrentAgentId,bg.AgentName FROM tx_clues cu
            LEFT JOIN tx_carbrands cb ON cu.ChosedModelId = cb.BrandId
            LEFT JOIN bx_agent bg ON bg.Id = cu.CurrentAgentId
            WHERE cu.ClueFromType <> 1 AND cu.CurrentAgentId IN (?ids)  ";
                sql = sql + " ORDER BY cu.CreateTime DESC ";
                List<MySqlParameter> paramList2 = new List<MySqlParameter>();
                paramList2.Add(new MySqlParameter("?ids", MySqlDbType.VarChar) { Value = ids });
                var data = db.Database.SqlQuery<PcClaimOrderModel>(sql, paramList2.ToArray()).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        try
                        {
                            if (item.CreateTime != null && item.CreateTime > DateTime.MinValue)
                            {
                                item.CreateTimeString = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                return data;
            }
            catch 
            {
                return new List<PcClaimOrderModel>();
            }
          
        }

        /// <summary>
        /// 获取业务员
        /// </summary>
        /// <param name="agentId">当前代理人id</param>
        /// <param name="salesmanName">业务员姓名</param>
        /// <returns></returns>
        public List<Salesman> GetSalesman(int agentId,string salesmanName)
        {
            var agentIdsql = @"SELECT SQL_CACHE  ?curAgent UNION ALL SELECT id FROM  bx_agent WHERE parentagent=?curAgent UNION ALL 
 SELECT id FROM bx_agent WHERE parentagent IN(SELECT id FROM bx_agent WHERE parentagent = ?curAgent)
 UNION ALL SELECT id FROM bx_agent WHERE parentagent IN(
                            SELECT id FROM bx_agent WHERE parentagent IN(
                            SELECT id FROM  bx_agent WHERE parentagent = ?curAgent))";
            List<MySqlParameter> paramList1 = new List<MySqlParameter>();
            paramList1.Add(new MySqlParameter("?curAgent", MySqlDbType.Int32) { Value = agentId });
            var agentIds = db.Database.SqlQuery<int>(agentIdsql, paramList1.ToArray()).ToList();
            var result = db.bx_agent.Where(o => o.AgentName.Contains(salesmanName) && agentIds.Contains(o.Id)).ToList();
            if (result != null)
            {
              return result.Select(o => new Salesman { SalesmanId = o.Id, SalesmanName = o.AgentName }).ToList();
            }
            return new List<Salesman>();
        }

        /// <summary>
        /// 获取代理商台账列表
        /// </summary>
        /// <param name="agentId">代理人id</param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="salesman">业务员名称或id</param>
        /// <param name="topSalesman">上级业务员名称或id</param>
        /// <param name="settledState">结算状态</param>
        /// <param name="startSettledTime">结算开始时间</param>
        /// <param name="endSettledTime">结算结束时间</param>
        /// <param name="moblie">业务员手机号</param>
        /// <param name="startCreateTime">台账开始生成时间</param>
        /// <param name="endCreateTime">台账结束生成时间</param>
        /// <param name="toSettlementId">代理商结算单id</param>
        /// <returns></returns>
        public List<SetTlement> GetSetTlementPage(int agentId,out int totalCount, int pageIndex = 1, int pageSize = 10, string orderNo = "", string  licenseNo = "",string salesman = "",string topSalesman = "",int settledState = 0,string startSettledTime = "",string endSettledTime = "",string moblie = "",string startCreateTime = "",string endCreateTime="",int toSettlementId = 0)
        {
            try
            {
                var ids = "";
                var sql = @"SELECT IFNULL(tsd.CreateTime,'0001-01-01 00:00:00') AS 'CreateTime', tsd.OrderNum,tsd.LicenseNo,tsd.ModleName,tsd.FromAgentName as 'BusinessName',tsd.MaintainAmount,
            tsd.ToRate,tsd.ToPayType as 'ToSettledState',IFNULL(tsd.ToEarnedTime,'0001-01-01 00:00:00') AS 'ToSettledTime',tc.CurrentAgentId,bg.AgentName as 'CurrentAgentName',bg.Mobile,bg.TopAgentId,
            bg2.AgentName as 'TopAgentName',tsd.ClueType as 'CaseType'
                FROM tx_settlement_detail  tsd
            inner join tx_clues tc on tc.id = tsd.OrderId
            LEFT JOIN bx_agent bg on bg.id = tc.CurrentAgentId
            LEFT JOIN bx_agent bg2 on bg2.id = bg.TopAgentId
            LEFT JOIN tx_settlement tse on tse.Id = tsd.ToSettlementId ";
                if (toSettlementId > 0)
                {
                    sql = sql + " WHERE tsd.ToSettlementId = ?toSettlementId ";
                }
                else
                {
                    ids = GetChildAgentId(agentId);
                    sql = sql + " where tsd.AuditedState = 2 and  tc.CurrentAgentId in(" + ids + ") ";

                }
                List<MySqlParameter> paramList2 = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    orderNo = orderNo.Trim();
                    sql = sql + " and tsd.OrderNum = ?orderNo ";
                }
                if (!string.IsNullOrWhiteSpace(licenseNo))
                {
                    licenseNo = licenseNo.Trim();
                    sql = sql + " and tsd.LicenseNo like ?licenseNo ";
                }
                if (!string.IsNullOrWhiteSpace(salesman))
                {
                    salesman = salesman.Trim();
                    Regex reg = new Regex("^[0-9]+$");
                    Match ma = reg.Match(salesman);
                    if (ma.Success)
                    {
                        sql = sql + " and  bg.id = ?salesman ";
                        paramList2.Add(new MySqlParameter("?salesman", MySqlDbType.Int32) { Value = salesman });
                    }
                    else
                    {
                        sql = sql + " and bg.AgentName like ?salesman ";
                        paramList2.Add(new MySqlParameter("?salesman", MySqlDbType.VarChar) { Value = '%' + salesman + '%' });
                    }
                }
                if (!string.IsNullOrWhiteSpace(topSalesman))
                {
                    topSalesman = topSalesman.Trim();
                    Regex reg = new Regex("^[0-9]+$");
                    Match ma = reg.Match(topSalesman);
                    if (ma.Success)
                    {
                        sql = sql + " and   bg.TopAgentId = ?topSalesman ";
                        paramList2.Add(new MySqlParameter("?topSalesman", MySqlDbType.Int32) { Value = topSalesman });
                    }
                    else
                    {
                        sql = sql + " and bg2.AgentName like ?topSalesman  ";
                        paramList2.Add(new MySqlParameter("?topSalesman", MySqlDbType.VarChar) { Value = '%' + topSalesman + '%' });
                    }
                }
                if (settledState > 0)
                {
                    sql = sql + " and tsd.ToPayType  = ?settledState ";
                }
                if (!string.IsNullOrWhiteSpace(startSettledTime) && !string.IsNullOrWhiteSpace(endSettledTime))
                {
                    sql = sql + " AND DATE_FORMAT(tsd.ToEarnedTime,'%Y-%m-%d') between DATE_FORMAT(?startSettledTime,'%Y-%m-%d') and DATE_FORMAT(?endSettledTime,'%Y-%m-%d') ";
                }
                if (!string.IsNullOrWhiteSpace(moblie))
                {
                    moblie = moblie.Trim();
                    sql = sql + " and bg.Mobile  = ?moblie ";
                }
                if (!string.IsNullOrWhiteSpace(startCreateTime) && !string.IsNullOrWhiteSpace(endCreateTime))
                {
                    sql = sql + " AND DATE_FORMAT(tse.CreateTime,'%Y-%m-%d') between DATE_FORMAT(?startCreateTime,'%Y-%m-%d') and DATE_FORMAT(?endCreateTime,'%Y-%m-%d') ";
                }
                //paramList2.Add(new MySqlParameter("?ids", MySqlDbType.VarChar) { Value = ids });
                paramList2.Add(new MySqlParameter("?orderNo", MySqlDbType.VarChar) { Value = orderNo });
                paramList2.Add(new MySqlParameter("?licenseNo", MySqlDbType.VarChar) { Value = licenseNo + '%' });
                paramList2.Add(new MySqlParameter("?settledState", MySqlDbType.Int32) { Value = settledState });
                paramList2.Add(new MySqlParameter("?startSettledTime", MySqlDbType.DateTime) { Value = startSettledTime });
                paramList2.Add(new MySqlParameter("?endSettledTime", MySqlDbType.DateTime) { Value = endSettledTime });
                paramList2.Add(new MySqlParameter("?moblie", MySqlDbType.VarChar) { Value = moblie });
                paramList2.Add(new MySqlParameter("?startCreateTime", MySqlDbType.DateTime) { Value = startCreateTime });
                paramList2.Add(new MySqlParameter("?endCreateTime", MySqlDbType.DateTime) { Value = endCreateTime });
                paramList2.Add(new MySqlParameter("?toSettlementId", MySqlDbType.Int32) { Value = toSettlementId });
                var totalSql = "select count(1) from (" + sql + ") as t ";
                totalCount = db.Database.SqlQuery<int>(totalSql, paramList2.ToArray()).FirstOrDefault();
                sql = sql + " ORDER BY tsd.CreateTime DESC  ";
                if (toSettlementId > 0)
                {
                    pageSize = totalCount;
                }
                sql = sql + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize + " ";
                var data = db.Database.SqlQuery<SetTlement>(sql, paramList2.ToArray()).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.ToAmount = item.ToRate / 100 * item.MaintainAmount;
                        if (item.CreateTime != null && item.CreateTime > DateTime.MinValue)
                        {
                            item.CreateTimeString = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                        }
                        if (item.ToSettledTime != null && item.ToSettledTime > DateTime.MinValue)
                        {
                            item.ToSettledTimeString = item.ToSettledTime.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                totalCount = 0;
                LogHelper.Error("获取pc代理商台账列表异常：" + ex);
                return new List<SetTlement>();
            }
        }

        /// <summary>
        /// 获取台账导出数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<SetTlement> GetExportSetTlementList(int agentId)
        {
            var ids = GetChildAgentId(agentId);
            var sql = @"SELECT tsd.CreateTime, tsd.OrderNum,tsd.LicenseNo,tsd.ModleName,tsd.FromAgentName as 'BusinessName',tsd.MaintainAmount,
                    tsd.ToRate,tc.ToSettledState,tc.ToSettledTime,tc.CurrentAgentId,bg.AgentName as 'CurrentAgentName',bg.Mobile,bg.TopAgentId,
                    bg2.AgentName as 'TopAgentName'
                     FROM tx_settlement_detail  tsd
                    inner join tx_clues tc on tc.id = tsd.OrderId
                    LEFT JOIN bx_agent bg on bg.id = tc.CurrentAgentId
                    LEFT JOIN bx_agent bg2 on bg2.id = bg.TopAgentId
                    where tsd.AuditedState = 2 and  tc.CurrentAgentId in(?ids) 
                     ORDER BY tsd.CreateTime DESC ";
            List<MySqlParameter> paramList2 = new List<MySqlParameter>();
            paramList2.Add(new MySqlParameter("?ids", MySqlDbType.VarChar) { Value = ids });
            var data = db.Database.SqlQuery<SetTlement>(sql, paramList2.ToArray()).ToList();
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.ToAmount = item.ToRate / 100 * item.MaintainAmount;
                }
            }
            return data;
        }
         
        public string  GetChildAgentId(int agentId)
        {
            try
            {
                var agentIdsql = @"SELECT SQL_CACHE  ?curAgent UNION ALL SELECT id FROM  bx_agent WHERE parentagent=?curAgent UNION ALL 
 SELECT id FROM bx_agent WHERE parentagent IN(SELECT id FROM bx_agent WHERE parentagent = ?curAgent)
 UNION ALL SELECT id FROM bx_agent WHERE parentagent IN(
                            SELECT id FROM bx_agent WHERE parentagent IN(
                            SELECT id FROM  bx_agent WHERE parentagent = ?curAgent))";
                List<MySqlParameter> paramList1 = new List<MySqlParameter>();
                paramList1.Add(new MySqlParameter("?curAgent", MySqlDbType.Int32) { Value = agentId });
                var agentIds = db.Database.SqlQuery<int>(agentIdsql, paramList1.ToArray()).ToList();
                var ids = (agentIds != null && agentIds.Count > 0) ? string.Join(",", agentIds) : agentId.ToString();
                return ids;
            }
            catch
            {
                return agentId.ToString();
            }         
        }

        /// <summary>
        /// 车商台账列表
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="state">结算状态</param>
        /// <param name="startTime">开始结算时间</param>
        /// <param name="endTime">结束结算时间</param>
        /// <param name="startCreateTime">开始结算单生成时间</param>
        /// <param name="endCreateTime">结束结算单生成时间</param>
        /// <param name="fromSettlementId">车商结算单id</param>
        /// <returns></returns>
        public List<CarDealerSetTlement> GetCarDealerSetTlementPage(int topAgentId, out int totalCount, int pageIndex = 1,int pageSize = 10,string orderNo = "",string licenseNo = "",int state = 0,string startTime = "",string endTime = "",string startCreateTime = "",string endCreateTime = "",int fromSettlementId = 0)
        {
            try
            {
                var sql = @"SELECT tsd.Id as 'SetTlementId',IFNULL(tc.CreateTime,'0001-01-01 00:00:00') AS 'OrderTime',tc.OrderNum AS 'OrderNum',tc.licenseno AS 'LicenseNo', 
                tsd.ModleName,tsd.MaintainAmount,tsd.FromRate,tsd.AuditedState,tsd.FromPayType AS 'FromSettledState',
                IFNULL(tsd.FromExpendedTime,'0001-01-01 00:00:00') AS 'FromSettledTime',tsd.ClueType as 'CaseType'
                FROM tx_settlement_detail tsd 
                INNER JOIN tx_clues tc ON tc.id = tsd.OrderId
                LEFT JOIN tx_settlement tse ON tse.Id = tsd.FromSettlementId ";
                if (fromSettlementId > 0)
                {
                    sql = sql + " WHERE tsd.FromSettlementId = ?fromSettlementId ";
                }
                else
                {
                    sql = sql + " WHERE tc.agentid = ?topAgentId  ";
                }
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    orderNo = orderNo.Trim();
                    sql = sql + " and  tc.OrderNum = ?orderNo ";
                }
                if (!string.IsNullOrWhiteSpace(licenseNo))
                {
                    licenseNo = licenseNo.Trim();
                    sql = sql + " and tc.LicenseNo like ?licenseNo ";
                }
                if (state > 0)
                {
                    sql = sql + " and tsd.FromPayType = ?state ";
                }
                if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
                {
                    sql = sql + " AND DATE_FORMAT(tsd.FromExpendedTime,'%Y-%m-%d') between DATE_FORMAT(?startTime,'%Y-%m-%d') and DATE_FORMAT(?endTime,'%Y-%m-%d') ";
                }
                if (!string.IsNullOrWhiteSpace(startCreateTime) && !string.IsNullOrWhiteSpace(endCreateTime))
                {
                    sql = sql + " AND DATE_FORMAT(tse.CreateTime,'%Y-%m-%d') between DATE_FORMAT(?startCreateTime,'%Y-%m-%d') and DATE_FORMAT(?endCreateTime,'%Y-%m-%d') ";
                }
                List<MySqlParameter> paramList = new List<MySqlParameter>();
                paramList.Add(new MySqlParameter("?topAgentId", MySqlDbType.Int32) { Value = topAgentId });
                paramList.Add(new MySqlParameter("?orderNo", MySqlDbType.VarChar) { Value = orderNo });
                paramList.Add(new MySqlParameter("?licenseNo", MySqlDbType.VarChar) { Value = licenseNo + '%' });
                paramList.Add(new MySqlParameter("?startTime", MySqlDbType.DateTime) { Value = startTime });
                paramList.Add(new MySqlParameter("?endTime", MySqlDbType.DateTime) { Value = endTime });
                paramList.Add(new MySqlParameter("?state", MySqlDbType.Int32) { Value = state });
                paramList.Add(new MySqlParameter("?startCreateTime", MySqlDbType.DateTime) { Value = startCreateTime });
                paramList.Add(new MySqlParameter("?endCreateTime", MySqlDbType.DateTime) { Value = endCreateTime });
                paramList.Add(new MySqlParameter("?fromSettlementId", MySqlDbType.Int32) { Value = fromSettlementId });
                var totalSql = "select count(1) from (" + sql + ") as t ";
                totalCount = db.Database.SqlQuery<int>(totalSql, paramList.ToArray()).FirstOrDefault();
                sql = sql + " ORDER BY tsd.CreateTime DESC  ";
                if (fromSettlementId > 0)
                {
                    pageSize = totalCount;
                }
                sql = sql + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize + " ";
                var data = db.Database.SqlQuery<CarDealerSetTlement>(sql, paramList.ToArray()).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.FromAmount = item.FromRate / 100 * item.MaintainAmount;
                        item.OrderTimeString = string.Empty;
                        item.FromSettledTimeString = string.Empty;
                        if (item.OrderTime != null && item.OrderTime > DateTime.MinValue)
                        {
                            item.OrderTimeString = item.OrderTime.ToString("yyyy-MM-dd HH:mm");
                        }
                        if (item.FromSettledTime != null && item.FromSettledTime > DateTime.MinValue)
                        {
                            item.FromSettledTimeString = item.FromSettledTime.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取pc端车商台账列表异常:" + ex);
                totalCount = 0;
                return new List<CarDealerSetTlement>();
            }
        }

        /// <summary>
        /// 结算单列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state"></param>
        /// <param name="settledTime"></param>
        /// <returns></returns>
        public List<SetTlementPage> GetSettlementsPage(int agentId,out int totalCount, int pageIndex = 1,int pageSize = 10,int state = 0,string settledTime = "")
        {
            var ids = GetChildAgentId(agentId);
            var sql = @"SELECT ts.Id,ts.BatchNum,ts.SettledStart, ts.SettledEnd,
                    ts.InvoiceType,ts.OrderCount,ts.Cost,ts.WithholdAmount,ts.ExptectAmount,ts.SettledState,ts.SettledTime
                    FROM tx_settlement ts
                    WHERE ts.AgentId in (?ids) ";
            List<MySqlParameter> paramList = new List<MySqlParameter>();
            if (state > 0)
            {
                sql = sql + " and ts.SettledState=?state ";
            }
            if (!string.IsNullOrWhiteSpace(settledTime))
            {
                sql = sql + " and DATE_FORMAT(ts.SettledTime,'%Y-%m-%d') = DATE_FORMAT(?settledTime,'%Y-%m-%d')";
            }
            paramList.Add(new MySqlParameter("?ids", MySqlDbType.VarChar) { Value = ids });
            paramList.Add(new MySqlParameter("?state", MySqlDbType.Int32) { Value = state });
            paramList.Add(new MySqlParameter("?settledTime", MySqlDbType.DateTime) { Value = settledTime });
            var totalSql = "select count(1) from (" + sql + ") as t ";
            totalCount = db.Database.SqlQuery<int>(totalSql, paramList.ToArray()).FirstOrDefault();
            sql = sql + " ORDER BY ts.SettledTime DESC  ";
            sql = sql + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize + " ";
            return db.Database.SqlQuery<SetTlementPage>(sql, paramList.ToArray()).ToList();
        }


    }
}
