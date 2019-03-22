using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models
{
   public  interface IClueManagerRepository
    {
        int Add(tx_clues model);
        bool CluesIsExist(string mobile);
        List<TXPushPeople> GetPushPeople(int topAgentId, int type);


        List<TXPushPeople> GetPushLeader(int topAgentId);

    }
}
