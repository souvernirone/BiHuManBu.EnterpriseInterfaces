using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuotehistoryRelatedRepository : EfRepositoryBase<bx_quotehistory_related>, IQuotehistoryRelatedRepository
    {
        private EntityContext _db = DataContextFactory.GetDataContext();
        public QuotehistoryRelatedRepository(DbContext context) : base(context)
        {
        }
    }
}
