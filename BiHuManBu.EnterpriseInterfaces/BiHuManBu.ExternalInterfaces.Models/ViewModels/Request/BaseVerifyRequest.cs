namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseVerifyRequest : BaseRequest
    {
        /// <summary>
        /// 子级代理人id
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 是否包含禁用业务员
        /// </summary>
        public int? IsHas { get; set; }
    }

    public class BatchVerifyRequest 
    {
        /// <summary>
        /// 子级代理人id
        /// </summary>
        public long batchRenewalId { get; set; }
    }
}
