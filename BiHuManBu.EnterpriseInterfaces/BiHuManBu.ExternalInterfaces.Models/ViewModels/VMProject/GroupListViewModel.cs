using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.VMProject
{
    public class GroupListViewModel : BaseViewModel
    {
        public IList<GroupModel> GroupList { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }

    }
    public class GroupModel
    {
        /// <summary>
        /// 团长ID
        /// </summary>
        public int HeaderId { get; set; }
        /// <summary>
        /// 团长姓名
        /// </summary>
        public string HeaderName { get; set; }
        /// <summary>
        /// 团长手机号
        /// </summary>
        public string HeaderMobile { get; set; }
        /// <summary>
        /// 二级团员人数
        /// </summary>
        public int SecondCount { get; set; }
        /// <summary>
        /// 累计二级团员保费
        /// </summary>
        public decimal SecondPremium { get; set; }
        /// <summary>
        /// 三级团员人数
        /// </summary>
        public int ThirdCount { get; set; }
        /// <summary>
        /// 累计三级团员保费
        /// </summary>
        public decimal ThirdPremium { get; set; }
    }
}
