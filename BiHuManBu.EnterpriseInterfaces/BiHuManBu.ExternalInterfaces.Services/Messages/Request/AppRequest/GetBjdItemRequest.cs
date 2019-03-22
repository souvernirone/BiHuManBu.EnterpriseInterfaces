
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetBjdItemRequest//:BaseRequest
    {
        public int Source { get; set; }
        public long Bxid { get; set; }

        #region forApp新增，微信端未用到
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }

        public int ChildAgent { get; set; }

        public string BhToken { get; set; }
        public int Agent { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        #endregion
    }
}
