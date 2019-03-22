using System;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 分配用到的bx_userinfo的模型
    /// </summary>
    public class DistributeUserinfoDto
    {
        public long Id { get; set; }
        public int IsDistributed { get; set; }
        public int? RenewalType { get; set; }
        public string LicenseNo { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Agent { get; set; }
        /// <summary>
        /// 还有多少天到期
        /// </summary>
        public int? IsInputBxData { get; set; }
        public bool IsCamera { get; set; }
        public DateTime CameraTime { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? LastBizEndDate { get; set; }
    }

    /// <summary>
    /// 通知的模型
    /// </summary>
    public class DistributeExpireDto
    {
        public long Buid { get; set; }
        public string LicenseNo { get; set; }
        /// <summary>
        /// 还有多少天到期
        /// </summary>
        public int? IsInputBxData { get; set; }

        public int Agent { get; set; }
    }
}
