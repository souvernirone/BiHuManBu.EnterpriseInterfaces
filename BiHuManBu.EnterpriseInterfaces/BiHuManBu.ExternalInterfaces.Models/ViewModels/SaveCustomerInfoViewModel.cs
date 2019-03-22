using BiHuManBu.ExternalInterfaces.Models.ViewModels;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SaveCustomerInfoViewModel
    {
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>

        public string CustomerName { get; set; }
        /// <summary>
        /// 客户电话
        /// </summary>

        public string CustomerMobile { get; set; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public int CustomerType { get; set; }
        /// <summary>
        /// 投保地区
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 客户电话2
        /// </summary>
        public string ClientMobileOther { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>
        public string ClientAddress { get; set; }
        /// <summary>
        /// 客户备注2
        /// </summary>
        public string IntentionRemark { get; set; }

        public string TagId { get; set; }
    }
}
