namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    /// <summary>
    /// 
    /// </summary>
    public class GetZuoXiViewModel : BaseViewModel<GetZuoXiViewModel>
    {
        private string addNumber = "";

        /// <summary>
        /// 加拨号码
        /// </summary>
        public string AddNumber { get {
                return addNumber ?? "";
                } set { addNumber = value; } }

        /// <summary>
        /// 能否打电话
        /// </summary>
        public bool CanCall { get; set; }
    }
}
