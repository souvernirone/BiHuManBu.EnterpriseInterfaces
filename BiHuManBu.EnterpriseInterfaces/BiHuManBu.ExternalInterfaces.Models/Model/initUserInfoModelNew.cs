using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class InitUserInfoModel
    {
        public long Id { get; set; }
        private int _lastYearSource = -1;
        public int LastYearSource
        {
            get { return _lastYearSource; }
            set { _lastYearSource = value; }
        }

        //private int _quoteStatus = -1;

        //public int QuoteStatus
        //{
        //    get { return _quoteStatus; }
        //    set { _quoteStatus = value; }
        //}
        private int _renewalStatus = -1;

        public int RenewalStatus
        {
            get { return _renewalStatus; }
            set { _renewalStatus = value; }
        }
        private int _needEngineNo = 1;

        public int NeedEngineNo
        {
            get { return _needEngineNo; }
            set
            {
                _needEngineNo = value;
            }
        }
        private int? _categoryInfoId;

        public int? CategoryInfoId
        {
            get { return _categoryInfoId; }
            set { _categoryInfoId = value; }
        }
        private int _renewalType;

        public int RenewalType
        {
            get { return _renewalType; }
            set { _renewalType = value; }
        }
        /// <summary>
        /// 车牌类型
        /// </summary>
        public int RenewalCarType { get; set; }
        public string Agent { get; set; }
        public int agent_id { get; set; }
        public DateTime DistributedTime { get; set; }
        public int IsDistributed { get; set; }
        public string OpenId { get; set; }
        public string RegisterDate { get; set; }
        public string MoldName { get; set; }
        public int IsBatchRenewalData { get; set; }

        public string EngineNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }

    }
    public class initUserInfoModelNew
    {
        public long Id { get; set; }

        private int? _categoryInfoId;

        public int? CategoryInfoId
        {
            get { return _categoryInfoId; }
            set { _categoryInfoId = value; }
        }
        private int _renewalType;

        public int RenewalType
        {
            get { return _renewalType; }
            set { _renewalType = value; }
        }
        /// <summary>
        /// 车牌类型
        /// </summary>
        public int RenewalCarType { get; set; }
        public string Agent { get; set; }
        public int agent_id { get; set; }
        public DateTime DistributedTime { get; set; }
        public int IsDistributed { get; set; }
        public string OpenId { get; set; }
        public int IsBatchRenewalData { get; set; }
        //public string RegisterDate
        //{
        //    get
        //  ;
        //    set;
        //}
        //public string MoldName
        //{
        //    get
        //  ;
        //    set;
        //}
    }
}
