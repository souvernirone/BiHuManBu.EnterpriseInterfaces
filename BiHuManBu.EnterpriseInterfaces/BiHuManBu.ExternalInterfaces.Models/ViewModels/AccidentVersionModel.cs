using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AccidentVersionModel
    {
        private string _version;
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get { return _version != null ? _version : ""; } set { _version = value; } }
        private string _upContent;
        /// <summary>
        /// 版本内容
        /// </summary>
        public string UpContent { get { return _upContent != null ? _upContent : ""; } set { _upContent = value; } }
        /// <summary>
        /// 是否强制更新  0否 1是
        /// </summary>
        public int CompulsoryRenewal { get; set; }
    }

    public class AccidentVersionResponse
    {
        public AccidentVersionModel Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class AccidentConfigModel
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver { get; set; }

        /// <summary>
        ///  6：苹果，7：安卓
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 版本内容ID 关联messageId
        /// </summary>
        public int MsgId { get; set; }
        /// <summary>
        /// 是否强制更新 0否 1是
        /// </summary>
        public int CompulsoryRenewal { get; set; }
    }

    public class AccidentConfigRequest
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// android ios 
        /// </summary>
        [Required]
        public string LoginType { get; set; }
    }

    public class AccidentEditVersionConfig
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 比较来源类别（6【苹果】、7【安卓】）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 升级的内容 版本更新内容多条时，用“&&”符号分割
        /// </summary>
        public string UpContent { get; set; }

        /// <summary>
        /// 是否强制更新 0否 1是
        /// </summary>
        public int CompulsoryRenewal { get; set; }
    }
}
