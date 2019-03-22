using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MySql.Data.MySqlClient;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class RatePolicySettingRepository : EfRepositoryBase<bx_ratepolicy_setting>, IRatePolicySettingRepository
    {
        public RatePolicySettingRepository(DbContext context) : base(context)
        {
        }

        public async Task<bool> DeleteRatePolicyAsync(int ratePolicySettingId)
        {
            var sql = @"
UPDATE bx_ratepolicy_setting
SET is_delete = 1,update_time=now()
WHERE id = ?id;
UPDATE bx_ratepolicy_item
SET is_delete = 1
WHERE ratepolicy_setting_id = ?id;
";
            var param = new MySqlParameter
            {
                Value = ratePolicySettingId,
                MySqlDbType = MySqlDbType.Int32,
                ParameterName = "id"
            };

            int result = await Context.Database.ExecuteSqlCommandAsync(sql, param);

            return result > 0;
        }

        //        public async Task<bool> SetOverTransferCreditsAsync(int topAgentId, double percent)
        //        {
        //            var sql = @"
        //UPDATE bx_ratepolicy_setting
        //SET over_transfer_credits = ?over_transfer_credits
        //WHERE top_agent_id = ?top_agent_id ;
        //";
        //            var param = new MySqlParameter[]
        //            {
        //                new MySqlParameter
        //                {
        //                    ParameterName="over_transfer_credits",
        //                    Value=percent,
        //                    MySqlDbType=MySqlDbType.Double
        //                },
        //                new MySqlParameter
        //                {
        //                    ParameterName="top_agent_id",
        //                    Value=topAgentId,
        //                    MySqlDbType=MySqlDbType.Int32
        //                }
        //            };
        //            await Context.Database.ExecuteSqlCommandAsync(sql, param);
        //            return true;
        //        }
    }
}
