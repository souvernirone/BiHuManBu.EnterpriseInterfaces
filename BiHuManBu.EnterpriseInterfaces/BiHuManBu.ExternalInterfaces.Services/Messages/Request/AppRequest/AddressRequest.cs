using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AddressRequest
    {
        /// <summary>
        /// 地址id，更新订单用
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 用户id，废弃，无需传
        /// </summary>
        public Nullable<int> userid { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 省份id（暂未启用）
        /// </summary>
        public Nullable<int> provinceId { get; set; }
        /// <summary>
        /// 城市id（暂未启用）
        /// </summary>
        public Nullable<int> cityId { get; set; }
        /// <summary>
        /// 县id（暂未启用）
        /// </summary>
        public Nullable<int> areaId { get; set; }
        /// <summary>
        /// 当前代理人id
        /// </summary>
        public int agentId { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        public string SecCode { get; set; }
        public string BhToken { get; set; }
        /// <summary>
        /// 当前代理人id
        /// </summary>
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理人openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        /// <summary>
        /// 当前代理人openid
        /// </summary>
        public string openId { get; set; }
    }
}
