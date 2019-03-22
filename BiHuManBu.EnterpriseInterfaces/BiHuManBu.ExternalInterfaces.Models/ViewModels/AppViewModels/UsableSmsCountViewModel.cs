﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class UsableSmsCountViewModel : BaseViewModel
    {
       /// <summary>
       /// 可用条数
       /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 短信账户
        /// </summary>
        public string  AgentAccount { get; set; }
    }
}