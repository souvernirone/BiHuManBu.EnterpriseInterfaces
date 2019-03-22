using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySql.Data.MySqlClient;
using System.Data.Entity.Migrations;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ZCPersonalRepository : IZCPersonalRepository
    {
        private readonly EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        /// <summary>
        /// 根据用户获取银行卡信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_group_authen> GetBankCardMessage(int agentId)
        {
            return db.bx_group_authen.Where(x => x.agentId == agentId).ToList();
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public PersonInfoModel GetPerson(int agentId)
        {
            var param = new MySqlParameter
            {
                ParameterName = "agentId",
                Value = agentId,
                MySqlDbType = MySqlDbType.Int32
            };
            var count = db.bx_group_authen.Count(x => x.agentId == agentId);
            var sql = "";
            var request = new PersonInfoModel();

            if (count == 0)
            {
                sql = @"SELECT AgentName,Mobile FROM bx_agent WHERE Id=?agentId";
                request = db.Database.SqlQuery<PersonInfoModel>(sql, param).FirstOrDefault();
                if (request != null)
                {
                    request.AuthenState = -1;
                }
            }
            else
            {
                sql = @"SELECT
                        bx_group_authen.id AS Id,
                        bx_agent.AgentName,
                        bx_agent.Mobile,
                        bx_group_authen.cardholder AS Cardholder,
                        bx_group_authen.card_id AS CardId,
                        bx_group_authen.bank_card_number AS BankCardNo,
                        bx_group_authen.bank_id AS BankId,
                        bx_group_authen.authen_state AS AuthenState,
                        bx_group_authen.audit_remark AS Dismissal,
                        bx_group_authen.bankcard_face_url AS BankCardFront,
                        bx_group_authen.bankcard_reverse_url AS BankCardBack,
                        bx_group_authen.card_face_url AS CardFace,
                        bx_group_authen.card_reverse_url AS CardReverse 
                        FROM 
                        bx_agent 
                        LEFT JOIN bx_group_authen ON bx_group_authen.agentId=bx_agent.Id 
                        WHERE 
                        bx_agent.Id=?agentId";
                //var query = db.Database.SqlQuery<PersonInfoModel>(sql, param);
                //request = query.FirstOrDefault();
                request = _dbHelper.ExecuteDataTable(sql, param).ToList<PersonInfoModel>().FirstOrDefault();
            }


            return request;
        }
        /// <summary>
        /// 判断能否报价 sjy 2018/2/7
        /// </summary>
        /// <returns></returns>
        public int AgentCanQuote(int childAgentId)
        {
            return db.bx_group_authen.Where(x => x.agentId == childAgentId && x.authen_state == 1).Count();
        }
    }
}