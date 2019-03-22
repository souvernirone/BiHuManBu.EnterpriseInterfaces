 namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 订单配送信息请求模型 2018-09-14 张克亮
    /// </summary>
    public class OrderDeliveryInfoRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 经济人ID
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// 配送联系人
        /// </summary>
        public string DeliveryContacts { get; set; }
        /// <summary>
        /// 配送联系人电话
        /// </summary>
        public string DeliveryContactsMobile { get; set; }
        /// <summary>
        /// 配送方法
        /// </summary>
        public int DeliveryMethod { get; set; }
        /// <summary>
        /// 配送地址表Id
        /// </summary>
        public int DeliveryAddressId { get; set; }
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 市ID
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 地区ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 省名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区名称
        /// </summary>
        public string AreaName { get; set; }
    }
}
