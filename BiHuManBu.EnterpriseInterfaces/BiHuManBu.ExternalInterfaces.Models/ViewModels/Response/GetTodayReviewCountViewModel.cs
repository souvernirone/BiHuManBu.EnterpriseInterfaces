namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTodayReviewCountViewModel : BaseViewModel<GetTodayReviewCountViewModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Today { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Tomorrow { get; set; }
        /// <summary>
        /// 后天
        /// </summary>
        public int AfterTomorrow { get; set; }
        /// <summary>
        /// 第三天
        /// </summary>
        public int Third { get; set; }
        /// <summary>
        /// 第四天
        /// </summary>
        public int Fourth { get; set; }
        /// <summary>
        /// 第五天
        /// </summary>
        public int Fifth { get; set; }
        /// <summary>
        /// 第六天
        /// </summary>
        public int Sixth { get; set; }
        /// <summary>
        /// 第七天
        /// </summary>
        public int Seventh { get; set; }

        /// <summary>
        /// 过期数据
        /// </summary>
        public int Expire { get; set; }
    }
}
