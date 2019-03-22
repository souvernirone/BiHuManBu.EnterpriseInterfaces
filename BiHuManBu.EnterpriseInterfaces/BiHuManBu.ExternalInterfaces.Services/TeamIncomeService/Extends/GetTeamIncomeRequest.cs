using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends
{
    /// <summary>
    /// 获取团队收益两个时间区间
    /// </summary>
    public class GetTeamIncomeRequest : BaseRequest2
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 代理人注册时间（yyyy-MM）
        /// </summary>
        public string CreateTime { get; set; }
    }

    /// <summary>
    /// 计算团队收益根据时间计算（备用服务出问题的时候）
    /// </summary>
    public class GetTeamIncomeByDayRequest : BaseRequest2
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string Date { get; set; }
    }
}
