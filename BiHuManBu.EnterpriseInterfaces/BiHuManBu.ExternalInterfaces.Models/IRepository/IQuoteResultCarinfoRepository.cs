
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IQuoteResultCarinfoRepository
    {
        bx_quoteresult_carinfo Find(long buid,int source);
        bx_quoteresult_carinfo Find(long buid);
        int? GetOwnerIdType(long buid);

        List<bx_quoteresult_carinfo> FindList(long buid);
    }
}
