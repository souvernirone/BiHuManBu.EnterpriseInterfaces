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
    public class RatepolicyItemRepository : EfRepositoryBase<bx_ratepolicy_item>, IRatepolicyItemRepository
    {
        public RatepolicyItemRepository(DbContext context) : base(context)
        {
        }
    }
}
