using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    [Flags]
    public enum EnumQuoteStatus
    {
        TaiPy = 1,
        PinAn = 2,
        RenBao = 4,
        GuoShouCai = 8

    }
}
