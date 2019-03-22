using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    public enum OptionType
    {
        [Description("删除")]
        Delete=0,
        [Description("恢复")]
        Revert=1
    }
}
