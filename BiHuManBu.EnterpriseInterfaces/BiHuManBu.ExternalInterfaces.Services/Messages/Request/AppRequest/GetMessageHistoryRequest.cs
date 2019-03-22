using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetMessageHistoryRequest : BaseRequest
    {
        private int _readStatus = -1;
        /// <summary>
        /// 读取状态:-1:全部信息，1:已读，2:未读
        /// </summary>
        public int ReadStatus { get { return _readStatus; } set { _readStatus = value; } }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }
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

        /// <summary>
        /// 角色类型 3、4管理员
        /// </summary>
        public int RoleType { get; set; }
    }
}
