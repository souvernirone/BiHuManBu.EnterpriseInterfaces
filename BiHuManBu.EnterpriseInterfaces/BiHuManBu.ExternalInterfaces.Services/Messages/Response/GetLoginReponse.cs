using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class GetLoginReponse
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<bx_tagsetting> tagsetting { get; set; }
        public List<manager_module_db> module { get; set; }
    }
}
