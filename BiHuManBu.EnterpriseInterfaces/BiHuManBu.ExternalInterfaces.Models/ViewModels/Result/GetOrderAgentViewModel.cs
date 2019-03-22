using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    /// <summary>
    /// 获取出单员返回值
    /// </summary>
    public class GetOrderAgentViewModel: BaseViewModel<GetOrderAgentViewModel>
    {
        public List<OrderAgentDto> ListAgent { get; set; }
    }
}
