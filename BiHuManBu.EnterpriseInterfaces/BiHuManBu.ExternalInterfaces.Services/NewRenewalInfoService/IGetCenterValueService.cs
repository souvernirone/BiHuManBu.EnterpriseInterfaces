using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public interface IGetCenterValueService
    {
        string GetValue(string type, string identifie, string key);
    }
}
