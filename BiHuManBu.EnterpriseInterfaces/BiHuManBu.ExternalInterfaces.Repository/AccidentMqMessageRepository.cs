using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AccidentMqMessageRepository : IAccidentMqMessageRepository
    {
        private static readonly string DbConfig = ConfigurationManager.ConnectionStrings["zb"].ConnectionString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        EntityContext _dbContext;
       
        public AccidentMqMessageRepository()
        {

            _dbContext = DataContextFactory.GetDataContext();
        }
        public List<int> AddMessages(IEnumerable<tx_noticemessage> tx_Noticemessages)
        {

            var result = _dbContext.tx_noticemessage.AddRange(tx_Noticemessages);
            _dbContext.SaveChanges();
            return result.Select(x => x.id).ToList();
            //StringBuilder sb = new StringBuilder("insert into tx_noticemessage(mesaagetype,title,content,operateagentId,reciveaentId,culeid,culestate,createTime,updatetime)values");

            //foreach (var item in clueNotificationDto.AcceptanceAgentInfoes)
            //{
            //    sb.Append(string.Format(" ({0},{1},{2},{3},{4},{5},{6},now(),now()), ", clueNotificationDto.MessageType, "", "", clueNotificationDto.OprateAgentId, item.AcceptanceAgentId, clueNotificationDto.ClueId, clueNotificationDto.ClueState));
            //}
            //sb.Remove(sb.ToString().LastIndexOf(','), 1);
            //sb.Append(";");
            //sb.Append("select @@IDENTITY");
            //DataContextFactory.GetDataContext().Database.SqlQuery<int>(sb.ToString()).fi;
        }

        public bool UpdateSendState(Dictionary<int, bool> dictionary)
        {
            StringBuilder sb = new StringBuilder("UPDATE tx_noticemessage set issend=case id ");
            foreach (var item in dictionary)
            {
                sb.Append(string.Format(" when {0} then {1} ", item.Key, item.Value?1:0));
            }
            sb.Append(string.Format(" end  where id in ({0})", string.Join(",", dictionary.Select(x => x.Key))));
            return _dbContext.Database.ExecuteSqlCommand(sb.ToString()) > 0;
        }



        public List<tx_noticemessage> GetPollingPeople(string ids, int count) {
            string sql = string.Format("SELECT *FROM(SELECT * FROM tx_noticemessage WHERE tx_noticemessage.reciveaentId IN({0}) AND mesaagetype=0   ORDER BY createTime DESC) t GROUP BY t.reciveaentId  ORDER BY createTime DESC", ids);
            var parameters = new List<MySqlParameter>()
            { };
            return _dbHelper.ExecuteDataSet(sql, parameters.ToArray()).ToList<tx_noticemessage>() as List<tx_noticemessage>;

        }

    }
}
