

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AddReVisitedRequest:AppBaseRequest
    {
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
        /// @
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 回访时间
        /// </summary>
        public string NextReviewDate { get; set; }

    }
}
