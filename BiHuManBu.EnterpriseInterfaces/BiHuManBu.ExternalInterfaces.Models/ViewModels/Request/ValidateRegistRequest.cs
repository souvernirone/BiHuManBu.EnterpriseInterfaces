
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ValidateRegistRequest
    {
        #region 手机验证码校验
        /// <summary>
        /// 验证码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "Mobile不能为空")]
        public string Mobile { get; set; }
        #endregion

        #region 邀请码校验
        /// <summary>
        /// 父级代理人Id
        /// </summary>
        public int ShareCode { get; set; }
        #endregion

        #region 手机号存在校验
        //手机号 Mobile，在上面
        #endregion

        /// <summary>
        /// 
        /// </summary>
        //public string KeyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustKey { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        [Required(ErrorMessage = "SecCode不能为空")]
        public string SecCode { get; set; }
    }
}
