using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class JobNumberListViewModel : BaseViewModel
    {
        /// <summary>
        /// 工号管理列表
        /// </summary>
        public IList<JobNumberDto> JobNumberList { get; set; }
        /// <summary>
        /// 城市列表
        /// </summary>
        public IList<JobNumberDto> CityList { get; set; }
        /// <summary>
        /// 工号列表
        /// </summary>
        public IList<JobNumberDto> UkeyList { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }
    }

    public class JobNumberDto
    {
        /// <summary>
        /// 机构id
        /// </summary>
        public int OrgId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// Ukey名称(工号名称)
        /// </summary>
        public string UkeyName { get; set; }
        /// <summary>
        /// UkeyId
        /// </summary>
        public int UkeyId { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}
