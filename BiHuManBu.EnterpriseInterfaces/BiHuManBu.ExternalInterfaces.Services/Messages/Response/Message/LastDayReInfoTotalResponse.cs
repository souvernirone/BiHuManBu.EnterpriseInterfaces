using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class LastDayReInfoTotalResponse : BaseResponse
    {
        public int InStoreNum { get; set; }
        public int ExpireNum { get; set; }
        public int IntentionNum { get; set; }
        public int OrderNum { get; set; }
        public List<bx_userinfo> ReInfo { get; set; }
    }
}
