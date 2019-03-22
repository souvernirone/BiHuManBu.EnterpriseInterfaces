using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{

    public class UserInfoModel
    {
        private int? _isSingleSubmit = 0;

        public int? IsSingleSubmit
        {
            get { return _isSingleSubmit; }
            set { _isSingleSubmit = value; }
        }
        public string LicenseNo { get; set; }
        public string Mobile { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Agent { get; set; }
        public int RenewalType { get; set; }
        public int LastYearSource { get; set; }
        public string OpenId { get; set; }
        public string CityCode { get; set; }
        public int QuoteStatus { get; set; }
        public int Source { get; set; }
        public int NeedEngineNo { get; set; }
        public int CategoryInfoId { get; set; }
        public int IsReView { get; set; }
        public int RenewalCarType { get; set; }
        public DateTime DistributedTime { get; set; }
        public int IsDistributed { get; set; }
        public int IsBatchRenewalData { get; set; }
        /// <summary>
        /// 身份证后六位
        /// </summary>
        public string SixDigitsAfterIdCard { get; set; }

        /// <summary>
        /// 光鹏杰使用
        /// </summary>
        public int top_agent_id { get; set; }
        /// <summary>
        /// 光鹏杰使用
        /// </summary>
        public int agent_id { get; set; }
    }
    public class UserInfoModel2
    {
        public long BuId { get; set; }
        public string Agent { get; set; }
        public string OpenId { get; set; }
        public string LicenseNo { get; set; }
        public string SixDigitsAfterIdCard { get; set; }
        public Nullable<int> RenewalCarType { get; set; }
        public string CityCode { get; set; }
        public string CarVIN { get; set; }
        public string EngineNo { get; set; }
        public int TopAgentId { get; set; }
    }
}
