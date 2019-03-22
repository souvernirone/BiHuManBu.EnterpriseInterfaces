using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class JobNumberRequest
    {
        private int _pageSize = 10;
        private int _pageIndex = 1;

        /// <summary>
        /// 集团id(集团账号的代理人id)
        /// </summary>
        [Range(1, 100000000)]
        public int GroupId { get; set; }
        /// <summary>
        /// 机构id
        /// </summary>
        public int OrgId { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// Ukey名称（工号名称）
        /// </summary>
        public string UkeyName { get; set; }
        /// <summary>
        /// UkeyID 
        /// </summary>
        public int UkeyId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 城市Id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

    }
}
