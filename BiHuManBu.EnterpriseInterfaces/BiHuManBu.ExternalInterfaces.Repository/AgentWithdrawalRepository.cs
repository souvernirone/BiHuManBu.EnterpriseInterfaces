using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentWithdrawalRepository:IAgentWithdrawalRepository
    {
        private readonly EntityContext _db = DataContextFactory.GetDataContext();

        public int AddWithdrawal(bx_agent_withdrawal withdrawal)
        {
            _db.bx_agent_withdrawal.Add(withdrawal);
            return _db.SaveChanges();
        }

        public int UpdateListCommissionIdStatus(int withdrawalId,string ids)
        {
            string sql =
                string.Format(
                    "UPDATE dd_order_commission SET withdrawal_id = {0},withdrawal_status = 0 WHERE id IN ({1})",
                    withdrawalId, ids);

            return _db.Database.SqlQuery<int>(sql).FirstOrDefault();
        }

        public List<WithdrawalViewModel> GetPageListWithdrawal(PageListWithdrawalRequest request, List<string> listAgentIds)
        {
            string sqlwhere = "";
            if (!string.IsNullOrWhiteSpace(request.Mobile))
            {
                sqlwhere = sqlwhere + " AND B.Mobile = '" + request.Mobile + "'";
            }
            if (!string.IsNullOrWhiteSpace(request.CreateTime))
            {
                sqlwhere = sqlwhere + " AND A.create_time = '" + request.CreateTime + "'";
            }
            if (request.AuditStatus == 1)
            {
                sqlwhere = sqlwhere + " AND A.audit_status IN (2)";
            }
            else
            {
                sqlwhere = sqlwhere + " AND A.audit_status IN (-1,1)";
            }

            string sql =
                string.Format(
                    "SELECT B.AgentName,B.Mobile,D.bank_name AS BankName,C.bank_card_no AS BankCard,A.withdrawal_type AS WithdrawalType,A.money AS Money, "
                    + "A.Credit AS Credit,A.create_time AS CreateTime,A.audit_status AS AuditStatus"
                    + "FROM bx_agent_withdrawal AS A LEFT JOIN bx_agent AS B ON A.agent = B.id "
                    + "LEFT JOIN bx_agent_bankcard AS C ON A.bank_id = C.id LEFT JOIN bx_bank AS D ON C.bank_id = D.id"
                    + "WHERE A.agent IN '{0}' {1} "
                    + "ORDER BY ID DESC LIMIT {2},{3}", string.Join(",", listAgentIds), sqlwhere, (request.CurPage - 1) * request.PageSize, request.PageSize);
            
            return _db.Database.SqlQuery<WithdrawalViewModel>(sql).ToList();
        }


        public List<dd_order_commission> GetListCommissions(BaseRequest2 request)
        {
            return _db.dd_order_commission.Where(x => x.cur_agent == request.ChildAgent && x.status == 1).ToList();
        }

        public int UpdateWithdrawalAuditStatus(int id, int status)
        {
            var withdrawal = _db.bx_agent_withdrawal.FirstOrDefault(x => x.id == id);
            if (withdrawal != null)
            {
                withdrawal.audit_status = status;
                _db.bx_agent_withdrawal.AddOrUpdate(withdrawal);
            }
            return _db.SaveChanges();
        }

        public List<MoneyWithdrawalDetailViewModel> GetMoneyWithdrawalDetial(int id)
        {
             string sql =
                string.Format(
                    "SELECT A.create_time AS CreateTime,B.license_no AS LicenseNo,IFNULL(C.biz_pno,C.force_pno) AS Pno,B.money AS Money,B.credit AS Credit FROM bx_agent_withdrawal AS A"
                    + "LEFT JOIN dd_order_commission AS B ON A.id = B.withdrawal_id "
                    + "LEFT JOIN dd_order_quoteresult AS C ON A.id = C.dd_order_id "
                    + "WHERE withdrawal_id = {0}", id);

             return _db.Database.SqlQuery<MoneyWithdrawalDetailViewModel>(sql).ToList();
        }

        public List<TeamWithdrawalDetailViewModel> GetTeamMoneyWithdrawalDetial(int id)
        {
            string sql =
               string.Format(
                   "SELECT A.create_time AS CreateTime,B.license_no AS LicenseNo,IFNULL(C.biz_pno,C.force_pno) AS Pno,B.money AS Money,(C.ForceTotal + C.BizTotal) AS PnoAmount, "
                    +"D.AgentName AS SonAgentName,B.child_agent_grade AS AgentGrade,B.team_reward_proportion AS RewardProportion"
                    + "FROM bx_agent_withdrawal AS A "
                    + "LEFT JOIN dd_order_commission AS B ON A.id = B.withdrawal_id "
                    + "LEFT JOIN dd_order_quoteresult AS C ON A.id = C.dd_order_id "
                    + "LEFT JOIN bx_agent AS D ON B.child_agent = D.id "
                    + "WHERE withdrawal_id = {0}", id);

            return _db.Database.SqlQuery<TeamWithdrawalDetailViewModel>(sql).ToList();
        }
    }
}
