using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class QuoteRecordRepository : IQuoteRecordRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public async Task<int> Add(bx_quote_record entity)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    _dbContext.bx_quote_record.Add(entity);
                    return _dbContext.SaveChanges();
                });
            }
        }
        public List<bx_quote_record> Get(int agentId)
        {
            List<bx_quote_record> records = DataContextFactory.GetDataContext().bx_quote_record.Where(record => record.AgentId == agentId && record.ValidFlag == 1).ToList();
            return records;
        }
    }

}
