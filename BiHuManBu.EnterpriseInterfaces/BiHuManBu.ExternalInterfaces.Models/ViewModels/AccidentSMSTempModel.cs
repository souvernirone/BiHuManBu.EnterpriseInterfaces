using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AccidentSMSTempModel
    {
        /// <summary>
        /// 短信模板id
        /// </summary>
        public int TemplateId { get; set; }
        /// <summary>
        /// 顶级代理人id
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        private string _smsTemplateName;
        public string SmsTemplateName { get { return _smsTemplateName != null ? _smsTemplateName : ""; } set { _smsTemplateName = value; } }
        /// <summary>
        /// 短信内容
        /// </summary>
        private string _smsTemplateContent;
        public string SmsTemplateContent { get {return _smsTemplateContent != null ? _smsTemplateContent : ""; } set { _smsTemplateContent = value; } }

    }
}
