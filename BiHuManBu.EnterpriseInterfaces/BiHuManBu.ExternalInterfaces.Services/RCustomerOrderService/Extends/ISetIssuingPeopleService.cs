using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public interface ISetIssuingPeopleService
    {
        /// <summary>
        /// 获取随机获取的业务员信息
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bx_agent SetIssuingPeople(long agent);
    }
}
