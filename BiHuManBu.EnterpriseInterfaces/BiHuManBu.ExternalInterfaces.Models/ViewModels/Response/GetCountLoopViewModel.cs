namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    /// <summary>
    /// 获取摄像头进店提醒
    /// </summary>
    public class GetCountLoopViewModel : BaseViewModel<GetCountLoopViewModel>
    {
        /// <summary>
        /// 进店数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 进店的buid，多个用逗号分隔
        /// </summary>
        public string Buids { get; set; }

        /// <summary>
        /// 只要一个摄像头进店时，这里存放车辆的基本信息
        /// </summary>
        public LoopRenewalViewModel RenewalInfo { get; set; }
    }
}
