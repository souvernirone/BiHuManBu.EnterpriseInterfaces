namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetPrecisePriceViewModel : BaseViewModel
    {
        public GetPrecisePriceOfUserInfoViewModel UserInfo { get; set; }
        public PrecisePriceItemViewModel Item { get; set; }
        public QuoteResultCarInfoViewModel CarInfo { get; set; }
        public string CustKey { get; set; }
        public string CheckCode { get; set; }
    }
    public class GetPrecisePriceNewViewModel : AppBaseViewModel
    {
        public GetPrecisePriceOfUserInfoViewModel UserInfo { get; set; }
        public PrecisePriceItemViewModelWithBuid Item { get; set; }
        public QuoteResultCarInfoViewModel CarInfo { get; set; }
        public string CheckCode { get; set; }
    }

    public class GetPrecisePriceViewModelWithBuid : BaseViewModel
    {
        public GetPrecisePriceOfUserInfoViewModel UserInfo { get; set; }
        public PrecisePriceItemViewModelWithBuid Item { get; set; }
        public QuoteResultCarInfoViewModel CarInfo { get; set; }
        public QuoteReqCarInfoViewModel ReqInfo { get; set; }

        public string CustKey { get; set; }
        public string CheckCode { get; set; }
    }

    public class ChannelInfo
    {
        public long ChannelId { get; set; }
        public string ChannelName { get; set; }
    }

    /// <summary>
    /// 安心是否需要验车(0不强制要求验车 1系统要求验车 2核保要求验车)
    /// </summary>
    public class ValidateCar
    {
        public string ForceValidateCar { get; set; }
        public string BizValidateCar { get; set; }
        public string IsValidateCar { get; set; }
    }
}