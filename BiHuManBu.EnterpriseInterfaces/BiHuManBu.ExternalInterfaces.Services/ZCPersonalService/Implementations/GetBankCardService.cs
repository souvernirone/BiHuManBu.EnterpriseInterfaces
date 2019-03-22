using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class GetBankCardService : IGetBankCardService
    {
        private readonly IZCPersonalRepository _zcPersonalRepository;

        public GetBankCardService(IZCPersonalRepository zcPersonalRepository)
        {
            _zcPersonalRepository = zcPersonalRepository;
        }

        /// <summary>
        /// 根据用户获取银行卡信息 sjy 2018-2-3
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetBankCardMessage(int agentId)
        {
            var bankCardMessageList = new List<BankCardMessageResponse>();
            var bankCard = _zcPersonalRepository.GetBankCardMessage(agentId);
            if (bankCard.Count != 0)
            {
                bankCard.ForEach(x =>
                {
                    bankCardMessageList.Add(new BankCardMessageResponse
                    {
                        Id = x.id,
                        BankId = x.bank_id,
                        BankCardNo = x.bank_card_number
                    });
                });
                return BaseViewModel.GetBaseViewModel(1, "OK", bankCardMessageList);
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(0, "未绑定银行卡");
            }

        }
    }
}
