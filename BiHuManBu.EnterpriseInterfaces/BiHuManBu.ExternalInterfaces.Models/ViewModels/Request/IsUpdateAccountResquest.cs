﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class IsUpdateAccountResquest
   {
       public string OpenId { get; set; }
       public int TopAgentId { get; set; }
       public string SecCode { get; set; }
       public string CustKey { get; set; }
    }
}