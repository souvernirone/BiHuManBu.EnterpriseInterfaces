using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    /// <summary>
    /// 添加代理人接口返回模型
    /// </summary>
    public class AddAgentViewModel : BaseViewModel<AddAgentViewModel>
    {
        public int AgentId { get; set; }

        public string AgentName { get; set; }
    }
}
