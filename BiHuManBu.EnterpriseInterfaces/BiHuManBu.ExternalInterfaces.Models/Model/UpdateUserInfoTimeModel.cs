using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class UpdateUserInfoTimeModel
    {
        public long Id { get; set; }
     

        private int? _categoryInfoId;

        public int? CategoryInfoId
        {
            get { return _categoryInfoId; }
            set { _categoryInfoId = value; }
        }
        /// <summary>
        /// 续保类型
        /// </summary>
        public int RenewalType { get; set; }
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
        //    ;
        //    set;
        //}
        //public string MoldName
        //{
        //    get
        //   ;
        //    set;
        //}
    }
}
