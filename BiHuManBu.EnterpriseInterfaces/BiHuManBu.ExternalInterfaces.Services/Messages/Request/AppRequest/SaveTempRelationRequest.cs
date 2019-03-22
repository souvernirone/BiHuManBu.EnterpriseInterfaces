using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class SaveTempRelationRequest : BaseRequest
    {
        public List<TempUser> TempUsers { get; set; }
        public List<RelationDetailInfo> RelationDetailInfoes { get; set; }
        private int _step = 1;
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
        /// 1：录入临时关系人 2：录入中间表
        /// </summary>
        public int Step
        {
            get
            {
                return _step;
            }

            set
            {
                _step = value;
            }
        }
    }


}
