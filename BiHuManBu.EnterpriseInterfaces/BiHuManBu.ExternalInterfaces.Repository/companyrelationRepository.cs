using BiHuManBu.ExternalInterfaces.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CompanyrelationRepository: ICompanyrelationRepository
    {
        private readonly EntityContext db = DataContextFactory.GetDataContext();
        public List<bx_companyrelation> GetCompany(List<int> sources)
        {
            try
            {
              return  db.bx_companyrelation.Where(o => sources.Contains(o.source)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
