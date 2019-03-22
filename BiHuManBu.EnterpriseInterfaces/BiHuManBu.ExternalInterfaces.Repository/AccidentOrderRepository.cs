using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AccidentOrderRepository : IAccidentOrderRepository
    {
        private EntityContext _entityContext = DataContextFactory.GetDataContext();

        public List<AccidentSource> GetSource(string sourceName)
        {
            var sql = string.Format(@"SELECT source SourceId,name SourceName FROM bx_companyrelation  {0} ORDER  BY SourceId ASC ", string.IsNullOrWhiteSpace(sourceName) ? "" : "WHERE name LIKE '%" + sourceName + "%'");
            return _entityContext.Database.SqlQuery<AccidentSource>(sql).ToList();
        }

        public List<AccidentCarBrand> GetCarBrands(string brandName)
        {
            var sql = string.Format(@"select * from (
SELECT BrandId,Name BrandName,Initial,ParentId,HasChild FROM tx_carbrands WHERE IsUsed=1  
UNION  
SELECT BrandId,Name BrandName,Initial,ParentId,HasChild FROM tx_carbrands WHERE ParentId in (
SELECT BrandId FROM tx_carbrands WHERE IsUsed=1
) 
) as t order by t.Initial ", string.IsNullOrWhiteSpace(brandName) ? "" : "AND t.BrandName LIKE '%" + brandName + "%'");
            return _entityContext.Database.SqlQuery<AccidentCarBrand>(sql).ToList();
        }

        public List<SettlementViewModel> GetSettlementList(AccidentSettlementRequest request, out int totalCount)
        {
            var strWhere = new StringBuilder();
            if (request.SettledState > 0)
            {
                strWhere.Append(" and SettledState=" + request.SettledState);
            }
            if (request.SettledStartTime > Convert.ToDateTime("0001/1/1 0:00:00") && request.SettledEndTime != Convert.ToDateTime("0001/1/1 0:00:00"))
            {
                strWhere.Append(string.Format(" and SettledTime>='{0}' and SettledTime<'{1}'", request.SettledStartTime, request.SettledEndTime.AddDays(1)));
            }
            var sql = string.Format(@"SELECT Id,BatchNum,DATE_FORMAT(SettledStart,'%Y-%m-%d %H:%i:%s') SettledStart,DATE_FORMAT(SettledEnd,'%Y-%m-%d %H:%i:%s') SettledEnd,
                                    OrderCount,ExptectAmount,SettledState,DATE_FORMAT(SettledTime,'%Y-%m-%d %H:%i:%s') SettledTime,
                                    InvoiceType,Cost,ActualAmount
                                    FROM tx_settlement t WHERE t.AgentId={0} {3} order by Id desc Limit {1},{2}",
                                    request.AgentId, (request.PageIndex - 1) * request.PageSize, request.PageSize, strWhere);
            var countSql = string.Format(@"SELECT count(1) FROM tx_settlement t WHERE t.AgentId={0} {3} Limit {1},{2}",
                                    request.AgentId, (request.PageIndex - 1) * request.PageSize, request.PageSize, strWhere);
            totalCount = _entityContext.Database.SqlQuery<int>(countSql).FirstOrDefault();
            return _entityContext.Database.SqlQuery<SettlementViewModel>(sql).ToList();
        }

        public List<RecivesCarPeople> GetGrabOrderPeoples(int topAgentId)
        {
            var sql = string.Format(@"SELECT a.Id AgentId,a.AgentName FROM bx_agent a INNER JOIN manager_role_function_relation b
                                    ON a.ManagerRoleId=b.role_id AND b.function_code='graborder'
                                    WHERE a.TopAgentId={0}", topAgentId);
            return _entityContext.Database.SqlQuery<RecivesCarPeople>(sql).ToList();
        }
    }
}
