using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class GetBusiuserViewModel : BaseViewModel<GetBusiuserViewModel>
    {
        public List<ListBusiuserDto> ListBusiuser { get; set; }
    }
}
