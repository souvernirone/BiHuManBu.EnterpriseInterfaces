
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface INoticexbService
    {
        long AddNoticexb(int cityCode,string licenseNo, string businessExpireDate, string forceExpireDate, string nextBusinessStartDate, string nextForceExpireDate, int source, int childAgent,int agentId, long buid, int isRead);
        bx_notice_xb Find(long buid);
    }
}
