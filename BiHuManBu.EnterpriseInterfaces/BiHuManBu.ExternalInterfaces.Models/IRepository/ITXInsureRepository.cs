using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
   public  interface ITXInsureRepository
    {
        tx_clueinsureinfo GetTXInsure(int clueId);



        bool AddTXInsure(tx_clueinsureinfo model);
        

    }
}
