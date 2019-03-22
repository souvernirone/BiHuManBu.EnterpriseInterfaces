using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class MessageListViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<BxMessage> MsgList { get; set; }
    }
    public class BxMessage
    {
        //==========
        //bx_message
        //==========
        public string StrId { get; set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int MsgType { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public int MsgStatus { get; set; }
        public int MsgLevel { get; set; }
        public string SendTime { get; set; }
        public int CreateAgentId { get; set; }
        public string CreateAgentName { get; set; }
        public string Url { get; set; }

        //============
        //bx_notice_xb
        //============
        public string LicenseNo { get; set; }
        //public int Stauts { get; set; }
        public string LastForceEndDate { get; set; }
        public string LastBizEndDate { get; set; }
        public string NextForceStartDate { get; set; }
        public string NextBizStartDate { get; set; }
        public int Source { get; set; }
        //public int AgentId { get; set; }
        //保险到期天数类型  3：30天，2：60天，1：90天
        public int Days { get; set; }
        public long Buid { get; set; }

        //通过buid查询当前的userinfo的agent值
        public int OwnerAgent { get; set; }
    }

    /// <summary>
    /// 新消息列表
    /// </summary>
    public class MsgListViewModel : BaseViewModel
    {
        /// <summary>
        /// 消息总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<Msg> MsgList { get; set; }
    }
    public class Msg
    {
        /// <summary>
        /// 
        /// </summary>
        public long IndexId { get; set; }
        /// <summary>
        /// 消息Id
        /// </summary>
        public long MsgInfoId { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 读取消息状态 1已读0未读
        /// </summary>
        public int ReadStatus { get; set; }
        /// <summary>
        /// 消息类型 -1未取到
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 消息时间
        /// </summary>
        public string MsgTime { get; set; }
        /// <summary>
        /// 是否包含图片或超过500字   1:右侧弹窗展示 2:详情页展示
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 消息简介
        /// </summary>
        public string MsgIntro { get; set; }
    }
}
