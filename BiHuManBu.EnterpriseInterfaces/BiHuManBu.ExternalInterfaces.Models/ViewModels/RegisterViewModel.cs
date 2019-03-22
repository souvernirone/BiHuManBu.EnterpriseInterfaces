using BiHuManBu.ExternalInterfaces.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>

        public string AgentName { get; set; }
        /// <summary>
        /// 代理人类型
        /// </summary>
        public int AgentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IsDaiLi { get; set; }
        /// <summary>
        /// 父级代理人Id
        /// </summary>
        public int ParentAgent { get; set; }

        public string SecCode { get; set; }
        public string CustKey { get; set; }

        private int _RegType = 0;

        public int RegType { get { return _RegType; } set { _RegType = value; } }
        /// <summary>
        /// 代理人详细地址
        /// </summary>
        public string AgentAddress { get; set; }
        /// <summary>
        /// 是否需要短信验证码，IsCheckCode=true 不需要，false 需要
        /// </summary>
        private bool IsCheckCode = false;
        public bool _IsCheckCode
        {
            get { return IsCheckCode; }
            set { IsCheckCode = value; }
        }
        /// <summary>
        /// 代理人状态，0待审核，1可用，2禁用，3删除
        /// </summary>
        private bool IsUsed = false;
        public bool _IsUsed
        {
            get { return IsUsed; }
            set { IsUsed = value; }
        }

        /// <summary>
        /// 已购商品 1:接口 2:机器人 4:摄像头 8:呼叫，这里存放最后相加的结果，比如：同时选择了机器人（2）和摄像头（4），这里就存6
        /// </summary>
        public int Commodity { get; set; }

        /// <summary>
        /// 可以使用的平台 1:PC 2:微信 4:APP，这里存放最后相加的结果，比如：同时选择了微信（2）和APP（4），这里就存6
        /// </summary>
        public int Platfrom { get; set; }

        /// <summary>
        /// 是否允许业务员之间重复报价 0不允许 1允许
        /// </summary>
        public int repeatQuote { get; set; }
        /// <summary>
        /// 账号类型 0试用 1付费
        /// </summary>
        public int accountType { get; set; }
        /// <summary>
        /// 试用账号到期时间
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 是否开通新车报价 0不开通 1开通
        /// </summary>
        public int openQuote { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        public int  CodeType { get; set; }
        /// <summary>
        /// 安装机器人数量
        /// </summary>
        public int robotCount { get; set; }
        /// <summary>
        /// 4S店品牌
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 收费账号合同截止日期
        /// </summary>
        public string contractEnd { get; set; }
        /// <summary>
        /// 可报价保险公司
        /// </summary>
        public int quoteCompany { get; set; }
        public int addRenBao { get; set; }
        /// <summary>
        /// 手机号加星0不加星、1加星
        /// </summary>
        public int hidePhone { get; set; }
        public int zhenBangType { get; set; }
        public int openMultiple { get; set; }
        public int configCityId { get; set; }
        public int rbCount { get; set; }
        public int tpyCount { get; set; }
        public int paCount { get; set; }
        public int gscCount { get; set; }
        public int settlement { get; set; }
        public int structType { get; set; }

        public int desensitization { get; set; }

        public int ceditOpenTuiXiu { get; set; }
        

    }
}
