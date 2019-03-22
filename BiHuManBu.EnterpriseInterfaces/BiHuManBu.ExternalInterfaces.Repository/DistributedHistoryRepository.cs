using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class DistributedHistoryRepository : EfRepositoryBase<bx_distributed_history>, IDistributedHistoryRepository
    {
        private readonly ILog _logError = LogManager.GetLogger("ERROR");


        public DistributedHistoryRepository(DbContext context) : base(context)
        {
        }

        public async Task<int> AddDistributedHistoryAsync(bx_distributed_history model)
        {
            try
            {
                DataContextFactory.GetDataContext().bx_distributed_history.Add(model);
                return await DataContextFactory.GetDataContext().SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return 0;
            }
        }

        public async Task<bool> InsertBySqlAsync(List<bx_distributed_history> list)
        {
            var sqlBuilder = GenerateInsertSql(list);
            if (sqlBuilder.Length == 0)
                return true;
            var sql = sqlBuilder.ToString();

            return (await Context.Database.ExecuteSqlCommandAsync(sql))>0;
        }

        private StringBuilder GenerateInsertSql(List<bx_distributed_history> list)
        {
            StringBuilder builder = new StringBuilder();
            if (list.Count == 0)
                return builder;
            builder.Append("INSERT INTO bx_distributed_history  (B_Uid,top_agent_id,now_agent_id,batch_id,type_id,operate_agent_id,create_time) VALUES ");

            foreach (var item in list)
            {
                builder.Append(string.Format("({0},{1},{2},{3},{4},{5},'{6}'),", item.b_uid.ToString(), item.top_agent_id.ToString(), item.now_agent_id.ToString(), item.batch_id.ToString(), item.type_id.ToString(), item.operate_agent_id.ToString(), item.create_time.ToString()));
            }
            return builder.Remove(builder.Length - 1, 1);
        }
    }
}
