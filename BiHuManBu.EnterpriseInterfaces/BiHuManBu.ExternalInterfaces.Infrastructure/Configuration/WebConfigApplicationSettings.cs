using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Configuration
{
    public class WebConfigApplicationSettings : IApplicationSettings
    {
        public string LogToDbString
        {
            get { return ConfigurationManager.AppSettings["LoggerDbName"]; }
        }

        public string LogToTextString { get { return ConfigurationManager.AppSettings["LoggerTxtName"]; } }
        public string LogToDbForBusiness { get { return ConfigurationManager.AppSettings["LogDbForBusiness"]; } }
        /// <summary>
        /// log4net配置文件
        /// </summary>
        public string LogConfigFile { get { return ConfigurationManager.AppSettings["LogConfigFile"]; } }

        public string DbConfigString { get { return ConfigurationManager.ConnectionStrings["zb"].ConnectionString; } }
        /// <summary>
        /// 报价历史库
        /// </summary>
        //public string QuoteHistoryConfigString { get { return ConfigurationManager.ConnectionStrings["dbhistory"].ConnectionString; } }
        public string ImageSavePath { get { return ConfigurationManager.AppSettings["ImageSavePath"]; } }
        public string ImgServer { get { return ConfigurationManager.AppSettings["ImageServer"]; } }
        public string ImageUpload { get { return ConfigurationManager.AppSettings["ImageUpload"]; } }
        public string HtmlSavePath { get { return ConfigurationManager.AppSettings["HtmlSavePath"]; } }
        public int PageSize { get { return Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]); } }
        public int BatchRenewalPageSize { get { return Convert.ToInt32(ConfigurationManager.AppSettings["BatchRenewalPageSize"]); } }
        public string CreateOrderPath { get { return ConfigurationManager.AppSettings["CreateOrderPath"]; } }
        public string WeiXinDbConfigString { get { return ConfigurationManager.ConnectionStrings["wx"].ConnectionString; } }
        public string ExcelSavePath { get { return ConfigurationManager.ConnectionStrings["ExcelSavePath"].ConnectionString; } }
        public string SmsAccount { get { return ConfigurationManager.AppSettings["SmsAccount"]; } }
        public string SmsPassword { get { return ConfigurationManager.AppSettings["SmsPassword"]; } }
        public string SmsCenter { get { return ConfigurationManager.AppSettings["SmsCenter"]; } }
        public string SmsCenterSendSmsMethod { get { return ConfigurationManager.AppSettings["SmsCenterSendSmsMethod"]; } }

        /// <summary>
        /// 生码接口
        /// </summary>
        public string ValidCodeConfigString { get { return ConfigurationManager.AppSettings["GenerateCodeUrl"]; } }

        /// <summary>
        /// 订单保存时间
        /// </summary>
        public string OrderKeepTime { get { return ConfigurationManager.AppSettings["OrderKeepTime"]; } }
        /// <summary>
        /// 
        /// </summary>
        public string MsgOrderCode { get { return ConfigurationManager.AppSettings["MsgOrderCode"]; } }

        public string WeiboAppKey { get { return ConfigurationManager.AppSettings["WeiboAppKey"]; } }
        public string WeiboAppSec { get { return ConfigurationManager.AppSettings["WeiboAppSec"]; } }

        public string WeiboCallBack
        {
            get { return ConfigurationManager.AppSettings["WeiboAppCallBack"]; }
        }

        public string MessageCenter { get { return ConfigurationManager.AppSettings["MessageCenter"]; } }


        public string BaoJiaJieKou { get { return ConfigurationManager.AppSettings["BaoJiaJieKou"]; } }
        public string AuthUrl { get { return ConfigurationManager.AppSettings["AuthUrl"]; } }
        public string AuthSaveKey { get { return ConfigurationManager.AppSettings["AuthSaveKey"]; } }
        public string AuthSaveType { get { return ConfigurationManager.AppSettings["AuthSaveType"]; } }

        public string SecurityAppKey { get { return ConfigurationManager.AppSettings["SecurityAppKey"]; } }
        public string SecurityAppCode { get { return ConfigurationManager.AppSettings["SecurityAppCode"]; } }

        public string Memcached { get { return Convert.ToString(ConfigurationManager.AppSettings["Memcached"]); } }
        public string AddComment { get { return Convert.ToString(ConfigurationManager.AppSettings["AddComment"]); } }
        public string SettlementMq { get { return Convert.ToString(ConfigurationManager.AppSettings["SettlementMq"]); } }
        public string BaoxianCenter { get { return Convert.ToString(ConfigurationManager.AppSettings["BaoxianCenter"]); } }

        public string BaoJiaCenter { get { return Convert.ToString(ConfigurationManager.AppSettings["BaoJiaCenter"]); } }
        public string RenbaoCenter
        {
            get { return Convert.ToString(ConfigurationManager.AppSettings["RenbaoCenter"]); }
        }

        public string HostUrl { get { return Convert.ToString(ConfigurationManager.AppSettings["HostUrl"]); } }
        public int SendDataSize { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SendDataSize"]); } }

        public string SuccessFullSheetName { get { return ConfigurationManager.AppSettings["SuccessFullSheetName"]; } }
        public string FailEdSheetName { get { return ConfigurationManager.AppSettings["FailEdSheetName"]; } }
        public int ExcelStartRowIndex { get { return Convert.ToInt32(ConfigurationManager.AppSettings["ExcelStartRowIndex"]); } }
        public int CanUploadCount { get { return Convert.ToInt32(ConfigurationManager.AppSettings["CanUploadCount"]); } }
        public int IsPartsOpen { get { return Convert.ToInt32(ConfigurationManager.AppSettings["IsPartsOpen"]); } }
        public bool IsSupportTags { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsSupportTags"]); } }

        public string JDPay_PriKey
        {
            get
            {
                return ConfigurationManager.AppSettings["wepay.merchant.rsaPrivateKey"];
            }
        }

        public string JDPay_DesKey
        {
            get
            {
                return ConfigurationManager.AppSettings["wepay.merchant.desKey"];
            }
        }

        public string JDPay_PubKey
        {
            get
            {
                return ConfigurationManager.AppSettings["wepay.jd.rsaPublicKey"];
            }
        }

        public string JDPay_Merchant
        {
            get
            {
                return ConfigurationManager.AppSettings["JDPay_Merchant"];
            }
        }

        public string JDPay_Version
        {
            get
            {
                return ConfigurationManager.AppSettings["JDPay_Version"];
            }
        }

        public string JDPay_NotifyUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["JDPay_NotifyUrl"];
            }
        }

        public string JDPay_CreateOrderUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["wepay.server.uniorder.url"];
            }
        }

        public string JDPay_Md5Solt
        {
            get
            {
                return ConfigurationManager.AppSettings["JDPay_Md5Solt"];
            }
        }

        public string AX_PayPath { get { return ConfigurationManager.AppSettings["AX_PayPath"]; } }

        /// <summary>
        /// 不展示的菜单
        /// </summary>
        public string NoDisplayModule { get { return ConfigurationManager.AppSettings["NoDisplayModule"]; } }
   
        public string BatchSmsSign { get { return ConfigurationManager.AppSettings["BatchSmsSign"]; } }
        public string BatchSmsTail { get { return ConfigurationManager.AppSettings["BatchSmsTail"]; } }
        public int  BatchSmsCount { get { return Convert.ToInt32(ConfigurationManager.AppSettings["smsWordCount"]); } }
        public string SmsCenterBulkSendMethod { get { return ConfigurationManager.AppSettings["SmsCenterBulkSendMethod"]; } }
        public string SmsSign { get { return ConfigurationManager.AppSettings["SmsSign"]; } }

        public string RequestEngino { get { return ConfigurationManager.AppSettings["RequestEngino"]; } }
        public string UploadFail { get { return ConfigurationManager.AppSettings["UploadFail"]; } }
        public string AlreadyDel { get { return ConfigurationManager.AppSettings["AlreadyDel"]; } }
        public string AgentIds { get { return ConfigurationManager.AppSettings["AgentIds"]; } }
        public int DataGrowNum { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DataGrowNum"]); } }
    }
}
