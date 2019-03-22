using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class RecycleHistoryService : IRecycleHistoryService
    {
        private readonly IRecycleHistoryRepository _recycleHistoryRepository;

        public RecycleHistoryService(IRecycleHistoryRepository recycleHistoryRepository)
        {
            _recycleHistoryRepository = recycleHistoryRepository;
        }

        public async Task<bool> InsertRecycleHistoryAsync(List<DistributedRecycleDto> listBuid, int topAgent, int operateAgent,int recycleType)
        {
            List<bx_recycle_history> list = new List<bx_recycle_history>();
            var now = DateTime.Now;
            listBuid.ForEach(o =>
            {
                list.Add(new bx_recycle_history
                {
                    b_uid = o.Buid,
                    create_time = now,
                    now_agent_id = topAgent,
                    operate_agent_id = operateAgent,
                    operate_top_agent_id = topAgent,
                    past_agent_id = int.Parse(o.AgentId),
                    recycle_type = recycleType
                });
            });
            return await  _recycleHistoryRepository.InsertRecycleHistoryAsync(list);
        }

        public bool InsertTransferRecycleHistory(List<TransferRecycleDto> list, int topAgent, int operateAgent, int recycleType)
        {
            List<bx_recycle_history> listData = new List<bx_recycle_history>();
            var now = DateTime.Now;
            list.ForEach(o =>
            {
                listData.Add(new bx_recycle_history
                {
                    b_uid = o.Buid,
                    create_time = now,
                    now_agent_id = int.Parse(o.NewAgentId),
                    operate_agent_id = operateAgent,
                    operate_top_agent_id = topAgent,
                    past_agent_id = int.Parse(o.OldAgentId),
                    recycle_type = recycleType
                });
            });
            return _recycleHistoryRepository.InsertRecycleHistoryAsync(listData).Result;
        }
    }
}
