using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Configuration
{
    /// <summary>
    /// 配置文件设置
    /// </summary>
    public interface IApplicationSettings
    {
        string CreateOrderPath { get; }
        string LogToDbString { get; }
        string LogToTextString { get; }

        string LogToDbForBusiness { get; }

        string LogConfigFile { get; }

        string DbConfigString { get; }
        /// <summary>
        /// 报价历史库
        /// </summary>
        //string QuoteHistoryConfigString { get; }

        string ImageSavePath { get; }
        string HtmlSavePath { get; }
        string ImgServer { get; }
        string ImageUpload { get; }
        string ExcelSavePath { get; }
        int PageSize { get; }
        int BatchRenewalPageSize { get; }
        string WeiXinDbConfigString { get; }
        string SmsAccount { get; }
        string SmsPassword { get; }
        string SmsCenter { get; }

        string SmsCenterSendSmsMethod { get; }
        /// <summary>
        /// 生码接口Url
        /// </summary>
        string ValidCodeConfigString { get; }

        /// <summary>
        /// 订单保持时间
        /// </summary>
        string OrderKeepTime { get; }

        /// <summary>
        /// 验证短信模板内容
        /// </summary>
        string MsgOrderCode { get; }

        /// <summary>
        /// 微博登录key
        /// </summary>
        string WeiboAppKey { get; }
        string WeiboAppSec { get; }
        string WeiboCallBack { get; }
        string MessageCenter { get; }

        string AuthUrl { get; }
        string BaoJiaJieKou { get; }
        string AuthSaveKey { get; }
        string AuthSaveType { get; }

        string SecurityAppKey { get; }
        string SecurityAppCode { get; }

        string Memcached { get; }
        string AddComment { get; }
        string SettlementMq { get; }
        string BaoxianCenter { get; }
        string RenbaoCenter { get; }

        string HostUrl { get; }
        int SendDataSize { get; }
        string SuccessFullSheetName { get; }
        string FailEdSheetName { get; }
        int ExcelStartRowIndex { get; }
        int CanUploadCount { get; }
        int IsPartsOpen { get; }
        bool IsSupportTags { get; }
        string JDPay_PriKey { get; }
        string JDPay_DesKey { get; }
        string JDPay_PubKey { get; }
        string JDPay_Merchant { get; }
        string JDPay_Version { get; }
        string JDPay_NotifyUrl { get; }
        string JDPay_CreateOrderUrl { get; }
        string JDPay_Md5Solt { get; }
        string AX_PayPath { get; }
        /// <summary>
        /// 批量短信签名
        /// </summary>
        string BatchSmsSign { get; }
        /// <summary>
        /// 批量短信结尾
        /// </summary>
        string BatchSmsTail { get; }
        //批量短信单个字数最大值
        int BatchSmsCount { get; }
        /// <summary>
        /// 批量短信发送方法名
        /// </summary>
        string SmsCenterBulkSendMethod { get; }
        /// <summary>
        /// 单个短信签名
        /// </summary>
        string SmsSign { get; }
        string RequestEngino { get; }
        string UploadFail { get; }
        string AlreadyDel { get; }
        string AgentIds { get; }
        int DataGrowNum { get; }
    }
}
