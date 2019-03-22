using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuoteResultCarinfoRepository : IQuoteResultCarinfoRepository
    {
        public bx_quoteresult_carinfo Find(long buid, int source)
        {
            return DataContextFactory.GetDataContext().bx_quoteresult_carinfo.FirstOrDefault(x => x.b_uid == buid && x.source == source);
        }

        public bx_quoteresult_carinfo Find(long buid)
        {
            return DataContextFactory.GetDataContext().bx_quoteresult_carinfo.FirstOrDefault(x => x.b_uid == buid);
        }

        public int? GetOwnerIdType(long buid)
        {
            bx_quoteresult_carinfo quoteresultCarinfo =
                DataContextFactory.GetDataContext().bx_quoteresult_carinfo.FirstOrDefault(x => x.b_uid == buid && (x.owner_idno_type != null));
            return quoteresultCarinfo != null ? quoteresultCarinfo.owner_idno_type : null;
        }

        public List<bx_quoteresult_carinfo> FindList(long buid)
        {
            return DataContextFactory.GetDataContext().bx_quoteresult_carinfo.Where(x => x.b_uid == buid).ToList();
        }
    }
}
