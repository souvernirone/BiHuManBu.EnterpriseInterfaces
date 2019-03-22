using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class UpdateOrderStatusRequest : BaseRequest2
    {
        public long OrderId { get; set; }
        public string OrderNum { get; set; }
        public int OrderType { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string Remark { get; set; }

    }

    public class UpdateOrderDetile
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string Remark { get; set; }
    }
}
