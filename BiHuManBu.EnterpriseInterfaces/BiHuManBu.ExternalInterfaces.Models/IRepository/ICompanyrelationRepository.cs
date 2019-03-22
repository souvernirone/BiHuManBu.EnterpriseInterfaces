using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public interface ICompanyrelationRepository
    {
        List<bx_companyrelation> GetCompany(List<int> sources);
    }
}
