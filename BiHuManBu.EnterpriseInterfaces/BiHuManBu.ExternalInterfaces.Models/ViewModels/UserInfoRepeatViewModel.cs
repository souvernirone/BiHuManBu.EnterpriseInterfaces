using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UserInfoRepeatViewModel:BaseViewModel
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int GroupByType { get; set; }
        public List<UserRepModel> UserInfoRepeatList { get; set; }
        //public IEnumerable<IGrouping<string, UserInfoRepeatModel>> UserInfoRepeatList { get; set; }

    }
    public class UserRepModel
    {
        //public string LicenseNo { get; set; }
        //public string CarVIN { get; set; }
        //public string ClientMobile { get; set; }
        //public string ClientName { get; set; }
        public List<UserInfoRepeatModel> UserList { get; set; }
    }
}
