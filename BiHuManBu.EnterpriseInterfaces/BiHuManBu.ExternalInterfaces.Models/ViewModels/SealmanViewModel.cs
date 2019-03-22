using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SealmanViewModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string mobile { get; set; }
        public string leaveTime { get; set; }

        public string workStatus { get; set; }
        /// <summary>
        /// 0、删除 1、添加 2、默认已添加
        /// </summary>
        public int isAdd { get; set; }

        public string roleName { get; set; }

    }
}
