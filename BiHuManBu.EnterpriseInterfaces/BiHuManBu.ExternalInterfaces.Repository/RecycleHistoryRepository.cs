using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class RecycleHistoryRepository : EfRepositoryBase<bx_recycle_history>, IRecycleHistoryRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        public RecycleHistoryRepository(DbContext context) : base(context)
        {
        }

        public async Task< bool> InsertRecycleHistoryAsync(List<bx_recycle_history> list)
        {
            if (!list.Any())
                return true;
            // 将list中的数据2w一组的生成sql，将生成的sql放入listSql中
            List<string> listSql = new List<string>();
            var scope = 2 * 10000;
            var listCount = list.Count;
            var loopCount = Math.Ceiling(listCount * 1.0 / scope);
            var now = DateTime.Now.ToString();
            for (int i = 0; i < loopCount; i++)
            {
                StringBuilder builder = new StringBuilder("insert into bx_recycle_history (b_uid,past_agent_id,now_agent_id,operate_agent_id,operate_top_agent_id,create_time,recycle_type)  values ");
                for (int j = i * scope; j < ((i + 1) * scope) && j < listCount; j++)
                {
                    builder.Append(string.Format("({0},{1},{2},{3},{4},'{5}',{6}),", list[j].b_uid.ToString(), list[j].past_agent_id.ToString(), list[j].now_agent_id.ToString(), list[j].operate_agent_id.ToString(), list[j].operate_top_agent_id.ToString(), now, list[j].recycle_type));
                }
                builder.Remove(builder.Length - 1, 1);

                listSql.Add(builder.ToString());
            }
            // 执行sql
            foreach (var item in listSql)
            {
                await DataContextFactory.GetDataContext().Database.ExecuteSqlCommandAsync(item);
            }
            return true;
        }
    }
}
