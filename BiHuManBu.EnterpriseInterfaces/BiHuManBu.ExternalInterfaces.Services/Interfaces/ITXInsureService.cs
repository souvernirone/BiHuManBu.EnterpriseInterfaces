using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
  public   interface ITXInsureService
    {
        string GetTXInsure(int clueId);


       bool AddTXInsure(int clueId, string insureInfo);


    }
}
