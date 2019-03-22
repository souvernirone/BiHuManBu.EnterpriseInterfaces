namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 短信设置请求数据模型
    /// </summary>
    public class ShortMsgSettingRequest
    {
        /// <summary>
        /// 经济人ID
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 是否微信同号
        /// </summary>
        public int PhoneIsWechat { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 免责提示
        /// </summary>
        public string DisclaimerTips { get; set; }
    }
}
