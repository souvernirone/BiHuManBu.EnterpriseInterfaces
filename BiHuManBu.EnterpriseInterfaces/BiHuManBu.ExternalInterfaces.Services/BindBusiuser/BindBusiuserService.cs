using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.BindBusiuser
{
    public class BindBusiuserService : IBindBusiuserService
    {
        private ICheckCanBindBusiuserListService _checkCanBindBusiuserListService;
        private IOrderRepository _orderRepository;

        public BindBusiuserService(ICheckCanBindBusiuserListService checkCanBindBusiuserListService, IOrderRepository orderRepository)
        {
            _checkCanBindBusiuserListService = checkCanBindBusiuserListService;
            _orderRepository = orderRepository;
        }

        public async Task<BaseViewModel> CanBindBusiuserAsync(CanBandBusiuserRequest request)
        {
            var checkResult =await  _checkCanBindBusiuserListService.CheckRequestAsync(request);
            if (checkResult.Item1.BusinessStatus != 1)
                return checkResult.Item1;

            // 根据bx_busiuser.Id获取设备列表，并且除去已经绑定的
            var busiuserList = await _orderRepository.GetBusiuserListAsync(checkResult.Item2, request.Agent);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, busiuserList);
        }
    }
}
