 namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    /// <summary>
    /// 短信设置数据返回模型
    /// </summary>
    public class ShortMsgSettingResponse
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
