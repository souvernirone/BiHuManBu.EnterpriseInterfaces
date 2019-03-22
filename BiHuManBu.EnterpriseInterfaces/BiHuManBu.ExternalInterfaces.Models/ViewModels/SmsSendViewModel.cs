using BiHuManBu.ExternalInterfaces.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SmsSendViewModel : BaseViewModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string ShareCode { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 验证码有效时间（分钟）
        /// </summary>
        public int SaveMinutes { get; set; }
        /// <summary>
        /// 有效期内发送的验证码是否需要重新生成(0不需要 1需要)
        /// </summary>
        public bool SendNew { get; set; }
        /// <summary>
        /// 是否需要验证码(0不需要 1需要)
        /// </summary>
        public bool NeedCode { get; set; }
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        public int TopAgent { get; set; }
        /// <summary>
        /// 验证码类型(用途)0 注册、1 导出
        /// </summary>
        public int CodeType { get; set; }
    }
}
