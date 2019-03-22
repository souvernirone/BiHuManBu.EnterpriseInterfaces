using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SendMessageViewModel
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
        /// 发送内容 （占位符: {{Code}}）
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 验证码有效时间（分钟）
        /// </summary>
        public int SaveMinutes { get; set; }
        /// <summary>
        /// 有效期内发送的验证码是否需要重新生成
        /// </summary>
        public bool SendNew { get; set; }
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        public bool NeedCode { get; set; }
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        public int TopAgent { get; set; }
        /// <summary>
        /// 验证码类型(用途) 0注册;1导出;3支付;4修改客户状态和类别(数据超过500条),5批量删除
        /// </summary>
        public int CodeType { get; set; }
    }
}
