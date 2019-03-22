using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TableHeaderRepository : EfRepositoryBase<bx_table_header>, ITableHeaderRepository
    {
        public TableHeaderRepository(DbContext context) : base(context)
        {
        }
    }
}
