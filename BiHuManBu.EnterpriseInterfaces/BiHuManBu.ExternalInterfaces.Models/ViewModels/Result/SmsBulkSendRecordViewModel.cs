using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    public class SmsBulkSendRecordViewModel
    {
        /// <summary>
        /// bx_sms_batch_history.Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 客户总量
        /// </summary>
        public int CustomerTotalCount { get; set; }
        /// <summary>
        /// 发送状态，1->已发送，0->待发送，2->撤销发送，3->发送失败
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 已发送数量
        /// </summary>
        public int SendedCount { get; set; }
        /// <summary>
        /// 等待发送数量
        /// </summary>
        public int WaitToSendCount { get; set; }
        /// <summary>
        /// 发送失败数量
        /// </summary>
        public int FailedCount { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public string SendTime { get; set; }




    }
}
