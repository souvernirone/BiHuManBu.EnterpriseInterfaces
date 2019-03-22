
using System.Collections.Generic;
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetConsumerInfoViewModel : BaseViewModel
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

        public List<bx_tagflag> TagFlags { get; set; }
        public int CategoryInfoId { get; set; }
        public string CategoryInfoName { get; set; }
    }
}
