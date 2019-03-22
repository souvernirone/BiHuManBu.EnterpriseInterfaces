using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Enums;
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AccidentNoticeMessageModel
    {
        /// <summary>
        ///消息id
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// 消息类型 :0:出险通知、1：超时通知、2：回访通知、3：接车通知、4：流失通知、5：到店通知 6：店内接待 7待抢单 8抢单成功 9 抢单失败
        /// </summary>
        public int MesaageType { get; set; }
        private string _title;
        /// <summary>
        ///  消息标题
        /// </summary>
        public string Title { get { return _title != null ? _title : ""; } set { _title = value; } }
        private string _content;
        /// <summary>
        ///  消息标题
        /// </summary>
        public string Content { get { return _content != null ? _content : ""; } set { _content = value; } }
        /// <summary>
        /// 操作人
        /// </summary>
        public int OperateAgentId { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public int ReciveAgentId { get; set; }
        /// <summary>
        /// 线索id
        /// </summary>
        public int CuleId { get; set; }
        /// <summary>
        ///  线索状态   -1：未跟进 1:跟进中、2:上门接车、3:车辆到店、4:流失、5:发短信（跟进中）、6:打电话（跟进中）
        /// </summary>
        public int CuleState { get; set; }
        private string _createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get { return _createTime != null ? _createTime : ""; } set { _createTime = value; } }
        /// <summary>
        /// 时间戳 （创建时间距离当前时间差，以秒为单位）
        /// </summary>
        public double TimeDifference { get; set; }
        /// <summary>
        /// 1已处理  0未处理
        /// </summary>
        public int IsHandle { get; set; }

        /// <summary>
        /// 超时提醒条数
        /// </summary>
        public int TimeoutNoticeCount { get; set; }
    }

    public class RobbingModel
    {
        public int CuleId { get; set; }

        private string _sourceName;
        public String SourceName { get { return _sourceName != null ? _sourceName : ""; } set { _sourceName = value; } }

        public int IsMany { get; set; }

        public int IsDrivering { get; set; }

        public string Tip { get; set; }

        private string _brandName;
        public String BrandName { get { return _brandName != null ? _brandName : ""; } set { _brandName = value; } }

        private string _licenseNo;
        public String LicenseNo { get { return _licenseNo != null ? _licenseNo : ""; } set { _licenseNo = value; } }
        private string _carOwner;
        public String CarOwner { get { return _carOwner != null ? _carOwner : ""; } set { _carOwner = value; } }
        private string _orderNum;
        public String OrderNum { get { return _orderNum != null ? _orderNum : ""; } set { _orderNum = value; } }

        public int SourceId { get; set; }

    }
}
