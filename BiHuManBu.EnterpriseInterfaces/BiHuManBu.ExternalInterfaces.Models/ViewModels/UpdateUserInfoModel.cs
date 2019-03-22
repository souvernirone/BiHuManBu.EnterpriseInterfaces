using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UpdateUserInfoModel
    {
        public long Id { get; set; }
        /// <summary>
        /// 分配的新的代理人
        /// </summary>
        public string Agent { get; set; }
        /// <summary>
        /// 分配前的代理人
        /// </summary>
        public string OldAgent { get; set; }

        public string OpenId { get; set; }
        public int IsDistributed { get; set; }

        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? LastBizEndDate { get; set; }
        public DateTime DistributedTime { get; set; }

        public string LicenseNo { get; set; }
        public int TopAgentId { get; set; }

        #region 分配操作时，获取今日摄像头进店数据使用
        public bool IsCamera { get; set; }
        public DateTime CameraTime { get; set; }

        #endregion
    }   
    /// <summary>
    /// 分配摄像头进店数据，也要讲进店跟进记录插入到跟进记录表
    /// </summary>
    public class CrmStepsUserInfoModel
    {
        public int agent_id { get; set; }
        public long b_uid { get; set; }
        public DateTime camertime { get; set; }
    }
}
