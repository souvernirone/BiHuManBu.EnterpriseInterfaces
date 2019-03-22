using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 电子保单下载
    /// </summary>
    public class EPolicycs
    {

        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }
        /// <summary>
        /// @BuId
        /// </summary>
        public int BuId { get; set; }
        //交强投保单号
        public string ForcetNo { get; set; }
        //商业投保单号
        public string BiztNo { get; set; }
        //ChannelId
        public int ChannelId { get; set; }
        // @Source老的source值
        public int? Source { get; set; }
        public string OrderNum { get; set; }
        public string NotifyCacheKey { get; set; }
        public string ForcepNo { get; set; }
        public string BizpNo { get; set; }
        public string CarVin { get; set; }
        public string LicenseNo { get; set; }
        public string SecCode { get; set; }
        /// <summary>
        /// 被保人身份证号
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 投保人身份证号
        /// </summary>
        public string PolicyHoderCard { get; set; }
        public int IsOut { get; set; }
    }


}
