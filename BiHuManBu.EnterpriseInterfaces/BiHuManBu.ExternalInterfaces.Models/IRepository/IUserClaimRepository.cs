using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IUserClaimRepository
    {
        List<bx_claim_detail> FindList(long buid);
    }
}
