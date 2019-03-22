using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AddOrUpdateWorkOrderDetailRequest:BaseRequest
    {
        /// <summary>
        /// 工单主表Id
        /// </summary>
        public int WorkOrderId { get; set; }

        /// <summary>
        /// 续保顾问会用到
        /// 受理结果。1已在本店投保2改日回访3暂不投保4其他
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// @当前代理人Id，兼sa和续保顾问
        /// </summary>
        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        public string ChildName { get; set; }

        /// <summary>
        /// @（意向和受理记录都需要）
        /// </summary>
        public int OwnerAgent { get; set; }
        
        /// <summary>
        /// @车牌号（意向和受理记录都需要）
        /// </summary>
        public string LicenseNo { get; set; }

        public int SaAgent { get; set; }
        public string SaAgentName { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public long? Buid { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string CustKey { get; set; }

        /// <summary>
        /// 续保顾问会用到
        /// 意向投保公司。0人保1太平洋2平安3人寿
        /// </summary>
        public int IntentionCompany { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 回访时间
        /// </summary>
        public string NextReviewDate { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
