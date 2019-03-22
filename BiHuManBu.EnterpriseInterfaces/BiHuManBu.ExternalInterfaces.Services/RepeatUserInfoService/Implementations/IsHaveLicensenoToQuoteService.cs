using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations
{
    public class IsHaveLicensenoToQuoteService : IIsHaveLicensenoToQuoteService
    {
        private readonly IIsHaveLicensenoNoBusiService _isHaveLicensenoNoBusiService;
        public IsHaveLicensenoToQuoteService(IIsHaveLicensenoNoBusiService isHaveLicensenoNoBusiService)
        {
            _isHaveLicensenoNoBusiService = isHaveLicensenoNoBusiService;
        }
        public IsHaveLicensenoToQuoteViewModel HaveLicensenoToQuote(IsHaveLicensenoToQuoteRequest request)
        {
            IsHaveLicensenoToQuoteViewModel viewModel = new IsHaveLicensenoToQuoteViewModel();
            //执行查询方法
            bx_userinfo userinfo = _isHaveLicensenoNoBusiService.IsHaveLicenseno(request.Agent, request.ChildAgent, request.LicenseNo, request.VinNo, request.EngineNo, request.TypeId);
            if (userinfo == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "无数据";
                viewModel.Buid = 0;
                viewModel.CurAgent = 0;
                viewModel.CurOpenId = "";
                return viewModel;
            }
            viewModel.Buid = userinfo.Id;
            viewModel.CurAgent = int.Parse(userinfo.Agent);
            viewModel.CurOpenId = userinfo.OpenId;
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            return viewModel;
        }
    }
}
