using System.Collections.Generic;
using AppViewModels=BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class OrderDetailResponse:BaseResponse
    {
        public AppViewModels.CarOrder CarOrder { get; set; }
        public AppViewModels.UserInfo UserInfo { get; set; }
        public List<AppViewModels.ClaimDetail> ClaimDetail { get; set; }
        public AppViewModels.PrecisePrice PrecisePrice { get; set; }
    }

}
