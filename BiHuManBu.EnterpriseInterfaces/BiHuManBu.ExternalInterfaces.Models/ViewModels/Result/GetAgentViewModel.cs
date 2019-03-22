using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    public class GetAgentForOrderViewModel : BaseViewModel<GetAgentForOrderViewModel>
    {
        public List<BriefAgentDto> ListAgent { get; set; }
    }
}
