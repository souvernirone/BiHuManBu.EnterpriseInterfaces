using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IQuoteRecordRepository
    {
        Task<int> Add(bx_quote_record entity);
        List<bx_quote_record> Get(int agentId);
    }
}
