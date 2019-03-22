using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetMyBjdDetailRequest : BaseRequest
    {
        private int _source = -1;
        [Range(1, 2100000000)]
        public long Buid { get; set; }

        public string Activity { get; set; }

        #region forApp新增，微信端未用到
        /// <summary>
        /// 请求哪家保险公司，默认为全部，Source值为新值
        /// </summary>
        public int Source { get { return _source; } set { _source = value; } }

        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }

        public string BhToken { get; set; }
        #endregion

        /// <summary>
        /// 子经纪人Id，App验证+PC端crm调用
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 是否用新的险种模型 1是0否
        /// </summary>
        public int IsNewClaim { get; set; }
        /// <summary>
        /// 是否显示第三者节假日险  0 不显示 1显示
        /// </summary>
        public int ShowSanZheJieJiaRi { get; set; }
    }
}
