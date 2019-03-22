using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg
{
    [Serializable]
    public class FileUploadModel
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件业务类型
        /// </summary>
        public UploadBusinessType BusinessType { get; set; }
        /// <summary>
        /// 图片处理参数
        /// </summary>
        public Dictionary<string, string> VersionKey { get; set; }

        public string ImgFlag { get; set; }

    }
    /// <summary>
    /// 文件上传业务类型
    /// </summary>
    public enum UploadBusinessType
    {
        [Description("store")]
        Store = 0,//门店
        [Description("contract")]
        Contract = 1,//合同
        [Description("merchant")]
        Merchant = 2,//商户
        [Description("coupon")]
        Coupon = 3,//团单        
        [Description("Advertising")]
        Advertising = 4,//广告  
        [Description("user")]
        User = 5,//用户
        [Description("baoxian")]
        BaoXian = 6,//保险pdf
        [Description("IDCard")]
        IdCard = 7,//身份证
        [Description("YanChe")]
        YanChe = 8,//验车图片
        [Description("JinJingZheng")]
        JinJingZheng = 9,//进京证
        [Description("dingsun")]
        DingSun = 10,//定损图片
        [Description("chejiahao")]
        CheJiaHao = 11,//定损图片
    }
}
