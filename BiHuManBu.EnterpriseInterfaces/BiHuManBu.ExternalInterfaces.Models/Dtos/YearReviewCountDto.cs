namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 本年回访次数
    /// </summary>
    public class YearReviewCountDto
    {
        public long BuId { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ConsumerReviewModel
    {
        public int? b_uid { get; set; }
        public System.DateTime? next_review_date { get; set; }
    }
}
