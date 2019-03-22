using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 事故线索 线索管理实体
    /// </summary>
    public class AccidentClueModel
    {
        /// <summary>
        /// 线索id
        /// </summary>
        public int ClueId { get; set; }
        private string _licenseNo;
        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get { return _licenseNo != null ? _licenseNo : ""; } set { _licenseNo = value; } }
        private string _moldName;
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get { return _moldName != null ? _moldName : ""; } set { _moldName = value; } }

        public int? Source { get; set; }

        private string _sourceName;
        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string SourceName
        {
            get
            {
                var sName = "";
                switch (_sourceName)
                {
                    case "0":
                        sName = "平安";
                        break;
                    case "1":
                        sName = "太平洋";
                        break;
                    case "2":
                        sName = "人保";
                        break;
                    case "3":
                        sName = "人寿";
                        break;
                    default:
                        sName = _sourceName ?? "";
                        break;
                }
                return sName;
            }
            set { _sourceName = value; }
        }

        /// <summary>
        /// 案件类型:1:送修、2:返修
        /// </summary>
        public int CaseType { get; set; }

        /// <summary>
        /// 跟进状态-1：未跟进 1:跟进中、2:上门接车、3:车辆到店、4:流失、5:发短信（跟进中）、6:打电话（跟进中）
        /// </summary>
        public int FollowupState { get; set; }
        private string _carVIN;
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get { return _carVIN != null ? _carVIN : ""; } set { _carVIN = value; } }
        /// <summary>
        /// 报案时间
        /// </summary>
        private string _smsRecivedTime;
        public string SmsRecivedTime { get { return _smsRecivedTime != null ? _smsRecivedTime : ""; } set { _smsRecivedTime = value; } }
        /// <summary>
        /// 报案人
        /// </summary>
        private string _reportCasePeople;
        public string ReportCasePeople { get { return _reportCasePeople != null ? _reportCasePeople : ""; } set { _reportCasePeople = value; } }

        private string _reportCaseNum;
        public string ReportCaseNum { get { return _reportCaseNum != null ? _reportCaseNum : ""; } set { _reportCaseNum = value; } }
        /// <summary>
        /// 出险地点
        /// </summary>
        private string _dangerArea;
        public string DangerArea { get { return _dangerArea != null ? _dangerArea : ""; } set { _dangerArea = value; } }
        /// <summary>
        /// 短信接收时间
        /// </summary>
        private string _UpdateTime;
        public string UpdateTime { get { return _UpdateTime != null ? _UpdateTime : ""; } set { _UpdateTime = value; } }
        /// <summary>
        /// 接车地点
        /// </summary>
        public string ReceiveCarAddress { get; set; }

        /// <summary>
        /// 时间戳 （更新时间距离当前时间差，以秒为单位）
        /// </summary>
        public double TimeDifference { get; set; }



        public double AcceptTimeDifference { get; set; }

        private string _mobile;
        /// <summary>
        /// 报案人手机号
        /// </summary>
        public string Mobile { get { return _mobile != null ? _mobile : ""; } set { _mobile = value; } }

        private string _lastFollowAgent;
        public string LastFollowAgent { get { return _lastFollowAgent != null ? _lastFollowAgent : ""; } set { _lastFollowAgent = value; } }

        private string _lastFollowTime;
        public string LastFollowTime { get { return _lastFollowTime != null ? _lastFollowTime : ""; } set { _lastFollowTime = value; } }


        private string _smsContent;
        public string SmsContent { get { return _smsContent != null ? _smsContent : ""; } set { _smsContent = value; } }

        public DateTime ClueTime { get; set; }




        public DateTime AcceptedTime { get; set; }

        /// <summary>
        /// 接车人id
        /// </summary>
        public int ReciveCaragentid { get; set; }

        private string _remark;
        public string Remark { get { return _remark != null ? _remark : ""; } set { _remark = value; } }

        /// <summary>
        /// 超时提醒次数
        /// </summary>
        public int TimeoutNoticeCount { get; set; }


        private string _insureInfo;
        /// <summary>
        /// 承保险种
        /// </summary>
        public string InsureInfo { get { return _insureInfo != null ? _insureInfo : ""; } set { _insureInfo = value; } }


        /// <summary>
        /// 线索来源 1:短信推送 2：留修app人工编辑推送 3.wap推送 4.pc端推送的消息
        /// </summary>
        public int ClueFromType { get; set; }

        private string _chosedModelName;
        /// <summary>
        /// 爱推修 品牌型号
        /// </summary>
        public string ChosedModelName { get { return _chosedModelName != null ? _chosedModelName : ""; } set { _chosedModelName = value; } }

        public int IsMany { get; set; }

        public int IsDrivering { get; set; }

        public string DangerDec { get; set; }

        /// <summary>
        /// 定损图片
        /// </summary>
        public List<string> CluesImage { get; set; }
        /// <summary>
        /// 定损金额
        /// </summary>
        public decimal MaintainAmount { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }

        public string LastFollowContent { get; set; }

        public int AuditedState { get; set; }

        public string AccidentRemark { get; set; }

    }

    public class CustomClueDataModel : tx_clues
    {
        public int ReciveCaragentid { get; set; }

        public string CompanyName { get; set; }

        public int TimeoutNoticeCount { get; set; }

        public string SmsContent { get; set; }

        public string InsureInfo { get; set; }
    }
}
