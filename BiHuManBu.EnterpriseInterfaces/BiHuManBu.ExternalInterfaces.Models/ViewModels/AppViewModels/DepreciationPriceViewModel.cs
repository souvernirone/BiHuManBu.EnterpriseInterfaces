
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class DepreciationPriceViewModel:BaseViewModel
    {
        public DepreciationPriceItem Item { get; set; }
    }

    public class DepreciationPriceItem
    {
        public decimal DepreciationPrice { get; set; }
        public decimal UpPrice { get; set; }
        public decimal DownPrice { get; set; }
    }
}
