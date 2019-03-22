using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AddPreferentialActivityRequest : BaseVerifyRequest
    {
        /// <summary>
        /// 活动类别
        /// </summary>
        public int activityType { get; set; }

        /// <summary>
        /// 活动名称，可空（ISNULL? activityType 1 = [返现]，activityType 2 = [返卷]，activityType 3 = [优惠活动]）
        /// </summary>
        public string activityName { get; set; }

        /// <summary>
        /// 活动内容
        /// </summary>
        public string activityContent { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createName { get; set; }
    }
}
