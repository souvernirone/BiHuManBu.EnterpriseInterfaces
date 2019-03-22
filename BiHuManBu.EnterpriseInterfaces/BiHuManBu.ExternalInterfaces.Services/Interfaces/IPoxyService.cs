using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    [ServiceContract]
    public interface IPoxyService
    {
        [OperationContract]
        Dictionary<DateTime, Dictionary<long, long>> GetRenewalCount(DateTime startDate, DateTime endDate, List<int> topAgent);
    }
}
