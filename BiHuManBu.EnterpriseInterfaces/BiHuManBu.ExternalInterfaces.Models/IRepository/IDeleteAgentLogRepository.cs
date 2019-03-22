using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IDeleteAgentLogRepository
    {
        int InsertDeleteAgentLog(IList<bx_agent> delList);
    }
}
