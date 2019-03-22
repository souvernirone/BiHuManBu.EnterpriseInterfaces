using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class OrgListRequest : BaseRequest
    {
        private bool _needPage = true;

        /// <summary>
        /// 集团Id(集团账号的代理人Id)
        /// </summary>
        [Range(1, 100000000)]
        public int GroupId { get; set; }
        /// <summary>
        /// 认证状态 0未认证、1已认证
        /// </summary>
        public int AuthenState { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 是否需要分页 默认true
        /// </summary>
        public bool NeedPage { get { return _needPage; } set { _needPage = value; } }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int OrgId { get; set; }
    }
}
