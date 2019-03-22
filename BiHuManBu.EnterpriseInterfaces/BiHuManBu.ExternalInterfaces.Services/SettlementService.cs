
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class SettlementService : ISettlementService
    {
        readonly ISettlementRepository _settlementRepository;
        readonly IAgentRepository _agentRepository;

        public SettlementService(ISettlementRepository _settlementRepository, IAgentRepository _agentRepository)
        {
            this._settlementRepository = _settlementRepository;
            this._agentRepository = _agentRepository;
        }

        public bool AddUnSettlementRange(List<UnSettlementRequestVM> unSettlementListVM)
        {
            return _settlementRepository.AddUnSettlementRange(unSettlementListVM);

        }

        public Tuple<bool, string> CreateSettlement(List<int> ids, int settleType)
        {
            return _settlementRepository.CreateSettlement(ids, settleType);
        }

        public List<SettlementResponseVM> GetSettlementList(SettlementListSearchRequestVM settlementListRequestVM, out int totalCount, out double totalPrice,out int settleCount)
        {
            totalCount = 0;
            totalPrice = 0.00;
            settleCount = 0;
            List<SettlementResponseVM> settlementResponseVMList = new List<SettlementResponseVM>();
  
            if (settlementListRequestVM.SettleType != 5)
            {
                var agentIds = settlementListRequestVM.SearchAgentId == -1 ? _agentRepository.GetAgentsByAgentIdAndModelTypeAndSearchType(settlementListRequestVM.CurrentAgentId, settlementListRequestVM.ModelType, settlementListRequestVM.SettleType).Select(x => x.Id).ToList() : new List<int> { settlementListRequestVM.SearchAgentId };
                if (!agentIds.Any())
                {
                    return settlementResponseVMList;

                }
                settlementResponseVMList = _settlementRepository.GetSettlementListNotAboutCompany(agentIds, settlementListRequestVM, out totalCount, out totalPrice,out settleCount);
            }
            else
            {
                var agentIds = _agentRepository.GetAgentsByAgentIdAndModelTypeAndSearchType(settlementListRequestVM.CurrentAgentId, settlementListRequestVM.ModelType, settlementListRequestVM.ModelType == 1 || settlementListRequestVM.ModelType == 2 ? 3 : 4).Select(x => x.Id).ToList();
                var channelIds = settlementListRequestVM.ChannelId == -1 ? _agentRepository.GetAgentConfigs(settlementListRequestVM.CurrentAgentId, settlementListRequestVM.ModelType).Select(x => x.ukey_id.Value).ToList() : new List<int> { settlementListRequestVM.ChannelId };
                if (!channelIds.Any())
                {
                    return settlementResponseVMList;
                }
                settlementResponseVMList = _settlementRepository.GetSettlementListAboutCompany(channelIds,agentIds, settlementListRequestVM, out totalCount, out totalPrice,out settleCount);
            }
    
            return settlementResponseVMList;

        }

        public List<UnSettlementResponseVM> GetUnSettlementList(UnSettlementListSearchRequestVM unSettlementListSearchVM, out int totalCount, out double totalPrice, out int reconciliationCount)
        {
            List<UnSettlementResponseVM> unSettlementResponseListVM = new List<UnSettlementResponseVM>();
            totalCount = 0;
            totalPrice = 0.00;
            reconciliationCount = 0;
        
            if (unSettlementListSearchVM.UnSettleType != 5)
            {
                var agentIds = unSettlementListSearchVM.SearchAgentId == -1 ? _agentRepository.GetAgentsByAgentIdAndModelTypeAndSearchType(unSettlementListSearchVM.CurrentAgentId, unSettlementListSearchVM.ModelType, unSettlementListSearchVM.UnSettleType).Select(x => x.Id).ToList() : new List<int> { unSettlementListSearchVM.SearchAgentId };
                if (!agentIds.Any())
                {
                    return unSettlementResponseListVM;


                }
                unSettlementResponseListVM = _settlementRepository.GetUnSettlementListNotAboutCompany(unSettlementListSearchVM.UnSettleType, agentIds, unSettlementListSearchVM.PageIndex, unSettlementListSearchVM.PageSize, out totalCount, out totalPrice, out reconciliationCount, unSettlementListSearchVM.SwingCardStartDate, unSettlementListSearchVM.SwingCardEndDate);
            }
            else
            {
                var agentIds = _agentRepository.GetAgentsByAgentIdAndModelTypeAndSearchType(unSettlementListSearchVM.CurrentAgentId, unSettlementListSearchVM.ModelType,  unSettlementListSearchVM.ModelType==1|| unSettlementListSearchVM.ModelType==2?3:4).Select(x => x.Id).ToList();
                var channelIds = unSettlementListSearchVM.ChannelId == -1 ? _agentRepository.GetAgentConfigs(unSettlementListSearchVM.CurrentAgentId, unSettlementListSearchVM.ModelType).Select(x => x.ukey_id.Value).ToList() : new List<int> { unSettlementListSearchVM.ChannelId };
                if (!channelIds.Any())
                {
                    return unSettlementResponseListVM;
                }
                unSettlementResponseListVM = _settlementRepository.GetUnSettlementListAboutCompany(channelIds, unSettlementListSearchVM.PageIndex, unSettlementListSearchVM.PageSize, agentIds,out totalCount, out totalPrice, out reconciliationCount, unSettlementListSearchVM.SwingCardStartDate, unSettlementListSearchVM.SwingCardEndDate);
            }
            return unSettlementResponseListVM;
        }

        public bool UpdateStaus(SettlementUpdateStatusVM settlementUpdateStatusVM, out List<string> guids)
        {

            int totalCount = 0;
            double totalPrice = 0.00;
            int settleCount = 0;
            if (settlementUpdateStatusVM.Ids == null || !settlementUpdateStatusVM.Ids.Any())
            {
                settlementUpdateStatusVM.SearchWhere.PageSize = 100000;
                settlementUpdateStatusVM.Ids = GetSettlementList(settlementUpdateStatusVM.SearchWhere, out totalCount, out totalPrice, out settleCount).Select(x => x.Id).ToList();
            }

            return _settlementRepository.UpdateStaus(settlementUpdateStatusVM,out guids);
        }

        public bool RollBackSettlementList(int batchId, List<int> settleIds,out int rollbackCount)
        {
            return _settlementRepository.RollBackSettlementList(batchId, settleIds,out rollbackCount);
        }
        public Tuple<bool, string> AddToSettlementList(int batchId, List<int> unSettleIds)
        {
            return _settlementRepository.AddToSettlementList(batchId, unSettleIds);
        }
        public bool CheckSameData(List<int> ids, int settleType, int batchId)
        {

            return _settlementRepository.CheckSameData(ids, settleType, batchId);
        }
        public List<UnSettlementResponseVM> GetSettleListDetail(int batchId, string licenseNo, string reconciliationNum, out int totalCount, out int reconciliationCount, int pageIndex = 1, int pageSize = 10)
        {
            return _settlementRepository.GetSettleListDetail(batchId, licenseNo, reconciliationNum, out totalCount, out reconciliationCount, pageIndex, pageSize);
        }

    }
}
