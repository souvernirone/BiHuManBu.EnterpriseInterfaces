using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZuoXiRepository : IRepositoryBase<bx_zuoxi>
    {
        /// <summary>
        /// 判断自己或者下级是否有坐席权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="agentLevel"></param>
        /// <returns></returns>
        bool CheckSelfOrChildAgentHasZuoXi(int agentId, int agentLevel);

        bx_zuoxi GetZXByAgentId(int agentId);

    }
}
