using System;
using System.Text;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class OrderAgentService : IOrderAgentService
    {
        private readonly IAgentDistributedRepository _agentDistributedRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly ICameraDistributeServices _cameraDistributeServices;

        public OrderAgentService(
            IAgentDistributedRepository agentDistributedRepository
            , IAgentRepository agentRepository, ICameraDistributeServices cameraDistributeServices)
        {
            _agentDistributedRepository = agentDistributedRepository;
            _agentRepository = agentRepository;
            _cameraDistributeServices = cameraDistributeServices;
        }

        public GetOrderAgentViewModel GetOrderAgent(GetOrderAgentRequest request)
        {
            var list = _agentDistributedRepository.GetOrderAgent(request.Agent);
            var listAgentSource = GetOrderAgentSources(request);
            foreach (var orderAgentDto in list)
            {
                var model = listAgentSource.FirstOrDefault(x => x.OrderAgentId == orderAgentDto.OrderAgentId);
                orderAgentDto.InsuranceIds = model == null ? "" : model.SourceIds;
            }
            var result = GetOrderAgentViewModel.GetModel(BusinessStatusType.OK);
            result.ListAgent = list;
            return result;
        }

        public GetAgentForOrderViewModel GetAgentForOrder(GetAgentForOrderRequest request)
        {
            var list = _agentDistributedRepository.GetAgent(request.Agent);
            //if(request.ContainTopAgent==0)
            //{
            //    list.RemoveAll(o => o.AgentId == request.Agent);
            //}
            var result = GetAgentForOrderViewModel.GetModel(BusinessStatusType.OK);
            result.ListAgent = list;
            return result;
        }

        public async Task<BaseViewModel> DeleteOrderAgentAsync(DeleteOrderAgentRequest request)
        {
            var orderAgent = await _agentDistributedRepository.FirstOrDefaultAsync(o => o.Id == request.OrderAgentId);
            if (orderAgent == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "出单员不存在");
            }
            if (orderAgent.ParentAgentId != request.Agent)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.NoCondition, "出单员Id不是当前代理人下的");

            _agentDistributedRepository.Delete(orderAgent);

            var result = await _agentDistributedRepository.SaveChangesAsync();

            var delbl = DeleteDistributedSourceAsync(request);

            _cameraDistributeServices.SetOrderAgentRedis(new GetOrderAgentRequest() { Agent = request.Agent });

            if (result > 0)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }


        public async Task<BaseViewModel> MultipleEditOrderAgentAsync(MultipleEditOrderAgentRequest request)
        {
            var count = _agentDistributedRepository.GetAll<bx_agent>().Count(o => o.TopAgentId == request.Agent && request.ListAgentId.Contains(o.Id));
            if (request.ListAgentId.Count != count)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "不能将他人的业务员设置到自己的出单员");
            }

            List<bx_agent_distributed> listOrderAgent = new List<bx_agent_distributed>();
            var date = DateTime.Now;
            foreach (var item in request.ListAgentId)
            {
                listOrderAgent.Add(new bx_agent_distributed
                {
                    AgentId = item,
                    AgentType = 5,
                    CreateTime = date,
                    Deteled = false,
                    ParentAgentId = request.Agent
                });
            }

            try
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    _agentDistributedRepository.DeleteByParentAgentIdAgentType(request.Agent, 5);
                    await _agentDistributedRepository.InsertAsync(listOrderAgent);
                    await _agentDistributedRepository.SaveChangesAsync();
                    trans.Complete();
                }

                _cameraDistributeServices.SetOrderAgentRedis(new GetOrderAgentRequest() {Agent = request.Agent});
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }
            catch
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
            }
        }

        public async Task<IsOrderAgentViewModel> IsOrderAgentAsync(int agentId)
        {
            var isOrderAgent = _agentDistributedRepository.AnyAsync(o => o.AgentId == agentId && !o.Deteled && o.AgentType == 5);
            var result = IsOrderAgentViewModel.GetModel(BusinessStatusType.OK);
            result.IsOrderAgent = (await isOrderAgent) ? 1 : 0;
            return result;
        }



        public bool IsOrderAgent(int agentId)
        {
            return _agentDistributedRepository.Any(o => o.AgentId == agentId && !o.Deteled && o.AgentType == 5);
        }

        public BaseViewModel SingleEditOrderAgentAsync(SingleEditOrderAgentRequest request)
        {
            var orderAgent = _agentDistributedRepository.FirstOrDefaultAsync(o => o.Id == request.OrderAgentId).Result;
            if (orderAgent == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "出单员不存在");
            }
            if (orderAgent.ParentAgentId != request.Agent)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.NoCondition, "出单员Id不是当前代理人下的");

            var sql = new StringBuilder();
            var ids = request.InsuranceIds.Split(',');
            for (int i = 0; i < ids.Length; i++)
            {
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var childSql =
                    string.Format(
                        "INSERT INTO bx_agent_distributed_source (AgentId,ParentAgentId,Source,CreateTime,UpdateTime,AgentType) VALUES ({0},{1},{2},'{3}','{3}',{4});",
                        request.OrderAgentId, request.Agent, SourceGroupAlgorithm.GetOldSource(Convert.ToInt32(ids[i])),
                        time, 5);
                sql.Append(childSql);
            }

            var del = new DeleteOrderAgentRequest()
            {
                OrderAgentId = request.OrderAgentId,
                Agent = request.Agent
            };

            var delbl = DeleteDistributedSourceAsync(del);
            if (ids.Length > 0)
            {
                _agentDistributedRepository.AddDistributedSource(sql.ToString());
            }


            var result = _agentDistributedRepository.SaveChangesAsync().Result;
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        public BaseViewModel DeleteDistributedSourceAsync(DeleteOrderAgentRequest request)
        {
            var orderAgent = _agentDistributedRepository.FirstOrDefaultAsync(o => o.Id == request.OrderAgentId).Result;
            if (orderAgent == null)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "出单员不存在");
            }
            if (orderAgent.ParentAgentId != request.Agent)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.NoCondition, "出单员Id不是当前代理人下的");

            string sql =
                string.Format(
                    "DELETE FROM bx_agent_distributed_source WHERE ID IN (SELECT * FROM (SELECT ID FROM bx_agent_distributed_source WHERE AgentId = {0} AND ParentAgentId = {1} AND AgentType = 5) AS TB);",
                    request.OrderAgentId, request.Agent);
            _agentDistributedRepository.AddDistributedSource(sql);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        public List<OrderAgentSource> GetOrderAgentSources(GetOrderAgentRequest request)
        {
            var listAgentSources = new List<OrderAgentSource>();
            var list = GetOrderAgentSource(request);
            var listOrderAgent = list.Select(c => c.OrderAgentId).Distinct();
            foreach (var item in listOrderAgent)
            {
                var agentSource = new OrderAgentSource();
                var listSource = list.Where(n => n.OrderAgentId == item).ToList();
                foreach (var itemS in listSource)
                {
                    itemS.Source = SourceGroupAlgorithm.GetNewSource(Convert.ToInt32(itemS.Source));
                }
                string ids = string.Join(",", listSource.Select(x => x.Source).ToArray());
                agentSource.OrderAgentId = item;
                agentSource.SourceIds = ids;
                listAgentSources.Add(agentSource);
            }
            return listAgentSources;
        }

        public List<OrderAgentSourceDto> GetOrderAgentSource(GetOrderAgentRequest request)
        {
            return _agentDistributedRepository.GetOrderAgentSource(request);
        }
    }
}
