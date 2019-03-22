using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetCreaditInfoViewModel:BaseViewModel
    {
        public List<UserClaimViewModel> List { get; set; } 
    }
    public class GetCreaditDetailInfoViewModel : BaseViewModel
    {
        public List<UserClaimDetailViewModel> List { get; set; }
    }
}