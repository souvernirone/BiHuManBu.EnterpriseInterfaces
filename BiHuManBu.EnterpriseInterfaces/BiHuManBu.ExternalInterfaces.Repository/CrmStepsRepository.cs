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
    public class CrmStepsRepository : EfRepositoryBase<bx_crm_steps>, ICrmStepsRepository
    {
        public CrmStepsRepository(DbContext context) : base(context)
        {
        }


    }
}
