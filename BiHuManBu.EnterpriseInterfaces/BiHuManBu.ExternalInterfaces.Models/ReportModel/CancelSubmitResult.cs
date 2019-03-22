
namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class CancelSubmitResult
    {
        /// <summary>
        /// 商业险撤销核保状态
        /// </summary>
        public int BizStatus { get; set; }
        /// <summary>
        /// 商业险撤销结果描述
        /// </summary>
        public string BizMessage { get; set; }
        /// <summary>
        /// 交强险撤销核保状态
        /// </summary>
        public int ForceStatus { get; set; }
        /// <summary>
        /// 交强险撤销结果描述
        /// </summary>
        public string ForceMessage { get; set; }

    }
}
