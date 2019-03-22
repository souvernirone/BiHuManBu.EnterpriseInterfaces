namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class BaseViewModel
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }
    }
}