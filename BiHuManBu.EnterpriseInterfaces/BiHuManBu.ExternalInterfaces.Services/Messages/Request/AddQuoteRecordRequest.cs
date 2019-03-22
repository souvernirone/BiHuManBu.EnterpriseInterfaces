using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class AddQuoteRecordRequest
    {
        [Range(1, 200000000)]
        public long Buid { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public long LotNO { get; set; }
        /// <summary>
        /// 代理人Id（车主）
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "AgentId参数错误")]
        public int ReQuoteAgent { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        public string SecCode { get; set; }
        public int Agent { get; set; }
        public int ChildAgent { get; set; }
    }
}
