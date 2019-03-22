using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    /// <summary>
    /// 
    /// </summary>
    public class AppIsHaveLicensenoRequest:AppBaseRequest
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string VinNo { get; set; }
        /// <summary>
        /// 请求类型，1车牌号，2车架号
        /// </summary>
        [Range(1, 2, ErrorMessage = "请求类型不能是1,2的其他数字")]
        public int TypeId { get; set; }

        private int _roleType = -1;
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleType
        {
            get { return _roleType; }
            set { _roleType = value; }
        }
        /// <summary>
        /// 0不允许重复报价、1允许重复报价、2允许二级之间重复
        /// </summary>
        public int? RepeatQuote { get; set; }

        private int _isBehalfQuote = -1;//默认-1,则调用老版本
        /// <summary>
        /// 是否代报价：2不允许代报价，1允许代报价
        /// </summary>
        public int IsBehalfQuote
        {
            get { return _isBehalfQuote; }
            set { _isBehalfQuote = value; }
        }
    }
}
