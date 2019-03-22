using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetTempRelationRequest : BaseRequest
    {
      

        private long buId = -1;
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId
        {
            get
            {
                return buId;
            }

            set
            {
                buId = value;
            }
        }
        private bool? _tempUserType = null;
        /// <summary>
        /// 临时关系人类型查询方式-默认null:查询全部,false:查询个人,true：查询公户
        /// </summary>
        public bool? TempUserType
        {
            get
            {
                return _tempUserType;
            }

            set
            {
                _tempUserType = value;
            }
        }
        private int _tempType = 0;
        /// <summary>
        /// 1：临时车主 2：临时保险人
        /// </summary>

        public int TempType
        {
            get
            {
                return _tempType;
            }

            set
            {
                _tempType = value;
            }
        }

 
        
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        [Required]
        public string OpenId { get; set; }
        public string BhToken { get; set; }
        /// <summary>
        /// 当前代理Id
        /// </summary>
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }


    }
}
