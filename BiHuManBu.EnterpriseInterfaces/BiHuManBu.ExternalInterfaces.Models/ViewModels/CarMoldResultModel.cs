﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CarMoldResultModel
    {

        public int Id { get; set; }
        public int AgentId { get; set; }

        public string CarType { get; set; }

        public int IsDel { get; set; }

    }
}