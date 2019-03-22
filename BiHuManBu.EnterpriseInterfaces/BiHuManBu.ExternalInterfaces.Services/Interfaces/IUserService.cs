using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 顶级代理人添加下级代理人
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AddAgentViewModel AddAgent(AddAgentRequest request);
        
        user FindUserByOpenId(string openid);

        user AddUser(string openid, string mobile);
    }
}
