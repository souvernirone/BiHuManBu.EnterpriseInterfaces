using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// add by qdk 2018-12-11
    /// </summary>
    public class QuoteDetailModel
    {
        public int Buid { get; set; }
        
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 城市id
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string NextBusinessStartDate { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string NextForceStartDate { get; set; }
        /// <summary>
        /// 1：有报价信息 0：无报价信息
        /// </summary>
        public int IsPrecisePrice { get; set; }

        private List<PrecisePriceModel> _precisePrice = new List<PrecisePriceModel>();

        public List<PrecisePriceModel> PrecisePrice
        {
            get { return _precisePrice; }
            set { _precisePrice = value; }
        }
        #region add by qdk 2018-12-12 
        /// <summary>
        /// 商业险到期时间  
        /// </summary>
        public string BusinessExpireDate { get; set; }
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public string ForceExpireDate { get; set; }
        /// <summary>
        /// 报价商业险到期时间
        /// </summary>
        public string QuoteBusinessExpireDate { get; set; }
        /// <summary>
        /// 报价交强险到期时间
        /// </summary>
        public string QuoteForceExpireDate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 到期天数
        /// </summary>
        public int ExpireDateNum { get; set; }
        /// <summary>
        /// 车牌号所属业务员
        /// bx_userinfo.Agent
        /// </summary>
        public int ItemChildAgent { get; set; }
        /// <summary>
        /// bx_userinfo.OPENID
        /// </summary>
        public string ItemCustKey { get; set; }
        /// <summary>
        /// 上年投保公司
        /// </summary>
        public int LastYearSource { get; set; }
        /// <summary>
        /// 上年投保公司名称
        /// </summary>
        public string LastYearSourceName { get; set; }
        /// <summary>
        /// TUDO
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 续保状态
        /// </summary>
        public int RenewalStatus { get; set; }
        #endregion
    }
    public class PrecisePriceModel
    {
        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public int QuoteStatus { get; set; }
        public long Source { get; set; }
        public string SubmitResult { get; set; }
        public int SubmitStatus { get; set; }
    }
}
