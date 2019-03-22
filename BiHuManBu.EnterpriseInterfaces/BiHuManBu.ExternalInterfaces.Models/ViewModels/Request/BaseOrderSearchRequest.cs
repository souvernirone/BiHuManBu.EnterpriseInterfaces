namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 订单搜索的基类
    /// 这里没有包含分页和排序的参数
    /// </summary>
    public class BaseOrderSearchRequest: BaseRequest2
    {
        /// <summary>
        /// 投保人
        /// </summary>
        public string HolderName { get; set; }

        public string LicenseNo { get; set; }

        private int source = -1;
        /// <summary>
        /// 意向投保公司
        /// -1全部 0平安 1太平洋 2人保 3国寿财
        /// </summary>
        public int Source { get { return source; } set { source = value; } }

        /// <summary>
        /// 提交订单开始时间
        /// </summary>
        public string CreateBegainTime { get; set; }

        /// <summary>
        /// 提交订单结束时间
        /// </summary>
        public string CreateEndTime { get; set; }

        /// <summary>
        /// 录入时间/签单日期开始时间
        /// </summary>
        public string IssueBegainTime { get; set; }

        /// <summary>
        /// 录入时间/签单日期结束时间
        /// </summary>
        public string IssueEndTime { get; set; }

        private int _agentId = -1;
        /// <summary>
        /// 业务员id
        /// </summary>
        public int AgentId
        {
            get { return _agentId; }
            set { _agentId = value; }
        }

        private int orderStatus = -1;
        /// <summary>
        /// 订单状态 
        ///    -1全部  0暂存、1已过期、2取消、3被踢回、41待支付（待支付待承保&待支付已承保）、42待承保（已支付待承保）、5已承保（已支付已承保）
        /// </summary>
        public int OrderStatus { get { return orderStatus; } set { orderStatus = value; } }

        private int consumerPayStatus = -1;
        /// <summary>
        /// 客户付款状态
        /// -1全部、0待支付、1已支付
        /// </summary>
        public int ConsumerPayStatus { get { return consumerPayStatus; } set { consumerPayStatus = value; } }

        private int iDCardVerifyStatus = -1;
        /// <summary>
        /// 身份证采集状态
        /// -1全部0未采集1已验证2已失效
        /// </summary>
        public int IDCardVerifyStatus { get { return iDCardVerifyStatus; } set { iDCardVerifyStatus = value; } }

        private int insuranceCompanyPayStatus = -1;
        /// <summary>
        /// 保险公司投保状态
        /// -1全部0未支付1已支付
        /// </summary>
        public int  InsuranceCompanyPayStatus { get { return insuranceCompanyPayStatus; } set { insuranceCompanyPayStatus = value; } }

        private int _issuingPeopleId = -1;

        /// <summary>
        /// 出单员Id，0：保险公司出单员
        /// </summary>
        public int IssuingPeopleId
        {
            get { return _issuingPeopleId; }
            set { _issuingPeopleId = value; }
        }

    }
}
