using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class CreateSureOrderRequest : BaseRequest
    {
        /// <summary>
        /// 报价ID
        /// </summary>
        [Range(1, 21000000000)]
        public long Buid { get; set; }

        /// <summary>
        /// 保险公司 新source
        /// </summary>
        [Range(0, 9223372036854775807)]
        public long Source { get; set; }

        /// <summary>
        /// 当前代理人id
        /// </summary>
        [Range(1, 2100000000)]
        public int CurAgent { get; set; }

        /// <summary>
        /// 商业险费率
        /// </summary>
        public decimal? BizRate { get; set; }

        /// <summary>
        /// 报价单传OrderId，判断是0就没有预约单，否则就直接执行更新状态操作
        /// </summary>
        [Range(0, 21000000000)]
        public long OrderId { get; set; }

        /// <summary>
        /// 数据来源 2接口,4crm,6ios,7andriod,8微信（跟bx_userinfo的renewaltype一致）
        /// </summary>
        public int Fountain { get; set; }
    }
}
