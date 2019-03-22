namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class IsOrderAgentViewModel : BaseViewModel<IsOrderAgentViewModel>
    {
        /// <summary>
        /// 是否是出单员
        /// 0:不是  1:是
        /// </summary>
        public int IsOrderAgent { get; set; }
    }
}
