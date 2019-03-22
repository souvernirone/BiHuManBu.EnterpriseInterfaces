using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms
{
    public class SmsOrderViewModel : BaseViewModel
    {
        public SmsOrderModel SmsOrder { get; set; }
    }

    public class SmsOrderModel
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string OrderNum { get; set; }
        public decimal RechargeAmount { get; set; }
        public int PayType { get; set; }
        public int OrderStatus { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreateTime { get; set; }
        public int SmsQuantity { get; set; }
        public DateTime? PayDateTime { get; set; }
        public string ThirdPartOrderNum { get; set; }
    }
}
