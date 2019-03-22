using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 判断顶级是否包含请求车牌，请求模型
    /// </summary>
    public class IsHaveLicensenoRequest : BaseRequestViewModel
    {
        /// <summary>
        /// 顶级代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "顶级代理人Id不能小于1")]
        public int TopAgentId { get; set; }
        /// <summary>
        /// 试算代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "代理人Id不能小于1")]
        public int AgentId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string VinNo { get; set; }
        /// <summary>
        /// 请求类型，1车牌号，2车架号，3车牌号车架号都有
        /// </summary>
        [Range(1, 3, ErrorMessage = "请求类型不能是1,2的其他数字")]
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

        private bool _ShowNoDistributedLabel = false;
        /// <summary>
        /// 当前账号是否有分配和批续权限
        /// </summary>
        public bool ShowNoDistributedLabel
        {
            get { return _ShowNoDistributedLabel; }
            set { _ShowNoDistributedLabel = value; }
        }

        /// <summary>
        /// 0不允许重复报价、1允许重复报价、2允许二级之间重复
        /// </summary>
        public int? RepeatQuote { get; set; }

        /// <summary>
        /// 是否代报价：2不允许代报价，1允许代报价
        /// </summary>
        [Range(1, 2)]
        public int IsBehalfQuote { get; set; }
    }
}
