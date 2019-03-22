using BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq;
using System;
using System.Collections.Generic;


namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{


    [RabbitMqQueue(DelayQueueName = "tx_business_delay_queue", DelayRouteKey = "tx_business")]
    public class PushBusinessModel
    {
        public List<int> AgentIds { get; set; }

        public int ClueId { get; set; }

        public bool ArtificialHandleTime { get; set; }
    }



    [RabbitMqQueue(DelayQueueName = "tx_exchange", DelayRouteKey = "bind-tx")]
    public class ClueNotificationDto
    {
        /// <summary>
        /// tx_cluefollowuprecord.id
        /// </summary>
        public int Id { get; set; }

        public string ClueCreateTime { get; set; }


        /// <summary>
        /// 消息类型
        /// </summary>
        public int MessageType { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 线索编号
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 线索状态
        /// </summary>
        public int ClueState { get; set; }
        /// <summary>
        /// 代理人编号（操作 ）
        /// </summary>
        public int OprateAgentId { get; set; }

        public string CarPeopleName { get; set; }

        public string InsureCompanyName { get; set; }

        public string DangerDesc { get; set; }

        /// <summary>
        /// 代理人信息（接受）
        /// </summary>
        public List<AcceptanceAgentInfo> AcceptanceAgentInfoes { get; set; }


        public double MaintainAmount { get; set; }




    }
    public class AcceptanceAgentInfo
    {
        /// <summary>
        /// tx_cluefollowuprecord.id
        /// </summary>
        public int FollowId { get; set; }

        public int TopAgentId { get; set; }
        /// <summary>
        /// 代理人类型
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 代理人编号（接受）
        /// </summary>
        public int AcceptanceAgentId { get; set; }
        /// <summary>
        /// 延迟时间
        /// </summary>

        public int CumulativeTimeout { get; set; }

        public int CumulativeTimeoutCount { get; set; }



    }

    public class TitleType
    {

        public static string OutDangerTitle = "出险通知";
        public static string OutTimeTitle = "超时通知";
        public static string LossTitle = "流失通知";
        public static string ArriveTitle = "到店通知";
        public static string ReviewTitle = "回访通知";
        public static string ReceivingCarTitle = "接车通知";
        public static string ReceptionTitle = "接待通知";
        public static string GrabOrderTitle = "抢单通知";
        public static string GrabOrderSuccessTitle = "抢单成功";
        public static string GrabOrderFailTitle = "抢单失败";
        public static string FixedLossRemindTitle = "定损提醒";
        public static string FixedLossed = "已定损";
        public static string LeaveCarTitle = "已交车";
        public static string LeaveCarRemindTitle = "交车提醒";
        public static string AssignOrderTitle = "指派通知";
        public static string ClueFailureTitle = "线索失效";
    }
    public class ContentType
    {
        private string _licenseno;

        private string _moldName;

        private string _licensenoAndMoldName;
        private int _cumulativeTimeout;

        private string carPeopleName;

        private string insureCompanyName;

        private string dangerDesc;


        private double _maintainAmount;

        public ContentType(string _licenseno, string _moldName, int _cumulativeTimeout = 0, string _carPeopleName = "", string _insureCompanyName = "", string _dangerDesc = "", double maintainAmount = 0.0)
        {
            this._licenseno = _licenseno;
            this._moldName = _moldName;
            this._cumulativeTimeout = _cumulativeTimeout;
            if (!string.IsNullOrEmpty(_moldName))
            {
                _licensenoAndMoldName = string.Format("{0}（{1}）", _licenseno, _moldName);
            }
            else
            {
                _licensenoAndMoldName = string.Format("{0}", _licenseno);
            }

            carPeopleName = _carPeopleName;
            insureCompanyName = _insureCompanyName;
            dangerDesc = _dangerDesc;
            _maintainAmount = maintainAmount;


        }

        public string OutDangerContent { get { return string.Format("{0}该车出险，请尽快处理。", _licensenoAndMoldName); } private set { } }
        public string OutTimeContent { get { return string.Format("{0}已经{1}分钟没有处理，请尽快处理！", _licensenoAndMoldName, _cumulativeTimeout); } private set { } }

        public string LossContent { get { return string.Format("{0}该客户已流失。", _licensenoAndMoldName); } private set { } }
        public string ArriveContent { get { return string.Format("{0}已进店。", _licensenoAndMoldName); } private set { } }
        public string ReviewContent { get { return string.Format("{0}现在需要跟进，请尽快联系客户。", _licensenoAndMoldName); } private set { } }
        public string ReceivingCarContent { get { return string.Format("{0}需要接车，请尽快处理。", _licensenoAndMoldName); } private set { } }

        public string ReceptionNoticeContent { get { return string.Format("{0}正在来店途中，准备接待。", _licensenoAndMoldName); } private set { } }

        public string ClueFailureTitleContent { get { return string.Format("{0}已被他人领取，请悉知", _licenseno); } private set { } }

        public string GrabOrderContent { get { return string.Format("{0},{1},{2},{3}", _licensenoAndMoldName, carPeopleName, insureCompanyName, dangerDesc); } private set { } }

        public string GrabOrderSuccessContent { get { return string.Format("{0},{1},{2},{3}", _licensenoAndMoldName, carPeopleName, insureCompanyName, dangerDesc); } private set { } }

        //京N97896(保时捷911)，张大龙，中国人保， 非多方事故，无法正常行驶





        public string FixedLossRemindContent { get { return string.Format("{0}该车是否定损结束，请及时维护线索。", _licenseno); } private set { } }


        public string FixedLossedContent { get { return string.Format("{0}已定损，定损金额{1}元，点击查看详情。", _licenseno, _maintainAmount); } private set { } }




        public string LeaveCarContent { get { return string.Format("{0}已交车，点击查看详情。", _licenseno); } private set { } }

        public string AssignOrderContent { get { return string.Format("您收到系统指派的线索{0}，请尽快处理", _licenseno); } private set { } }




        public string LeaveCarRemindContent { get { return string.Format("{0}该车是否维修完成，请及时维护线索", _licenseno); } private set { } }

        public string Desensitization(string orig,int count)
        {
            return "*" + orig.Substring(count);
        }


    }
}
