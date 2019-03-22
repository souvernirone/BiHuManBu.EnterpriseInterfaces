using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class AddMessageRequest:BaseRequest
    {//[Required]
        //public string Title { get; set; }

        public string Body { get; set; }

        /// <summary>
        /// 消息类型,0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知
        /// </summary>
        [Range(0, 10)]
        public int MsgType { get; set; }

        [Range(1, 2100000000)]
        public int ToAgentId { get; set; }

        /// <summary>
        /// 消息重要级别。0不紧急1一般2紧急
        /// </summary>
        [Range(0, 10)]
        public int MsgLevel { get; set; }

        public string SendTime { get; set; }

        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        public string Url { get; set; }
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public int EndDaysNum { get; set; }
        public long Buid { get; set; }

        public int? Source { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string BusinessMsg { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
