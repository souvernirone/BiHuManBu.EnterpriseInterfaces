﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
   public class AccidentResponse
    {
       public AccidentLoginModel Data { get; set; }

       public int Code { get; set; }

       public string Message { get; set; }
    }
}