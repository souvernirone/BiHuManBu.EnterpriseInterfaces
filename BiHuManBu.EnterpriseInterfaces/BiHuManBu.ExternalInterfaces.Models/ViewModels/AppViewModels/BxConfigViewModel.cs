using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class BxConfigViewModel : BaseViewModel
    {
        /// <summary>
        /// 版本比较结果 0相同  1不同
        /// </summary>
        public int CompareResult { get; set; }

        /// <summary>
        /// 版本内容
        /// </summary>
        public string UpContent { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }

    public class EditBxConfigViewModel : BaseViewModel
    {
        /// <summary>
        /// 修改版本号结果，1：成功，0：失败
        /// </summary>
        public int EditResult { get; set; }
    }

    public class ConfigViewModel 
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
    }

    /// <summary>
    /// 比较请求
    /// </summary>
    public class RequestCompareConfig 
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 比较来源类别（6【苹果】、7【安卓】）
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 比较修改版本验证开关
    /// </summary>
    public class RequestKeyConfig
    {
        /// <summary>
        /// 开关ConfigKey
        /// </summary>
        [Required(ErrorMessage = "ConfigKey不能为空")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 修改的值0不验证，1验证
        /// </summary>
        [Required(ErrorMessage = "KeyValue不能为空")]
        public string KeyValue { get; set; }
    }

    /// <summary>
    /// 修改请求
    /// </summary>
    public class RequestEditConfig
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
        /// 升级的内容
        /// </summary>
        public string UpContent { get; set; }
    }
}

