
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface INoticexbRepository
    {
        long Add(bx_notice_xb bxNoticeXb);
        bx_notice_xb Find(int id);
        bx_notice_xb FindByBuid(long buid);
        int Update(bx_notice_xb bxNoticeXb);
        List<bx_notice_xb> FindByAgentId(int agentId);
        List<bx_notice_xb> FindNoReadList(int agentId, out int total);
    }
}
