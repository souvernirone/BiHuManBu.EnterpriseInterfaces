
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class CreateOrderDetailViewModel:BaseViewModel
    {
        public long OrderId { get; set; }
        public string OrderNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChangeStr ChangeStr { get; set; }
        /// <summary>
        /// 核保状态
        /// 0=核保失败，1=核保成功,2=未核保,3=核保中,4=非意向公司未核保,5=报价失败未核保,6=核保关闭未核保
        /// </summary>
        public int SubmitStatus { get; set; }
        /// <summary>
        /// 核保状态描述
        /// </summary>
        public string SubmitResult { get; set; }
        /// <summary>
        /// 报价状态
        /// 0=报价失败，1=报价成功
        /// </summary>
        public int QuoteStatus { get; set; }
        /// <summary>
        /// 报价状态描述
        /// </summary>
        public string QuoteResult { get; set; }
        /// <summary>
        /// 身份证采集状态
        /// </summary>
        public int VerificationCodeStatus { get; set; }
    }

    public class ChangeStr
    {
        /// <summary>
        ///  折扣系数变化值
        /// </summary>
        public string TotalRateChangeStr { get; set; }
        /// <summary>
        /// 使用性质 1：家庭自用车（默认），2：党政机关、事业团体，3：非营业企业客车，4：不区分营业非营业（仅人保），5：出租租赁（仅人保），6：营业货车（仅人保），7：非营业货车（仅人保）
        /// </summary>
        public string CarUsedTypeChangeStr { get; set; }
        /// <summary>
        /// 上次保费
        /// </summary>
        public double OldPurchaseAmount { get; set; }
        /// <summary>
        /// 当前保费
        /// </summary>
        public double PurchaseAmount { get; set; }
    }
}
