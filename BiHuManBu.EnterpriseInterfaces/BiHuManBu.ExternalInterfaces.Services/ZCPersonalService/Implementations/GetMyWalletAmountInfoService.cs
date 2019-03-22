using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class GetMyWalletAmountInfoService : IGetMyWalletAmountInfoService
    {
        private readonly IOrderCommissionRepository _orderCommissionRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        public GetMyWalletAmountInfoService(IOrderCommissionRepository orderCommissionRepository)
        {
            _orderCommissionRepository = orderCommissionRepository;
        }

        public GetMyWalletAmountInfoViewModel GetMyWalletAmountInfo(int childAgent)
        {
            GetMyWalletAmountInfoViewModel viewModel = new GetMyWalletAmountInfoViewModel();
            try
            {
                List<dd_order_commission> items = new List<dd_order_commission>();
                items = _orderCommissionRepository.GetMyAmountList(childAgent);
                if (items.Any())
                {
                    viewModel.MyCommision = items.Where(l => l.commission_type == 1).Sum(s => s.money);
                    viewModel.TeamInComing = items.Where(l => l.commission_type == 3).Sum(s => s.money);
                    viewModel.MyBalance = viewModel.MyCommision + viewModel.TeamInComing;//等提现上了，此处需要减去提现
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "获取成功";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "获取失败";
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -100003;
                viewModel.StatusMessage = "获取模型异常，请重试";
            }
            return viewModel;
        }
    }
}
