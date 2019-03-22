namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Message
{
    public class MsgInfoViewModel:BaseViewModel
    {
        /// <summary>
        /// 消息详情
        /// </summary>
        public MsgInfo Info { get; set; }
    }

    public class MsgInfo : Msg
    {
        /// <summary>
        /// html内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 创建代理人姓名
        /// </summary>
        public string CreateAgentName { get; set; }
        /// <summary>
        /// 消息Id
        /// </summary>
        public long MsgInfoId { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息类型 -1未取到
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 消息简介
        /// </summary>
        public string MsgIntro { get; set; }
    }
}
