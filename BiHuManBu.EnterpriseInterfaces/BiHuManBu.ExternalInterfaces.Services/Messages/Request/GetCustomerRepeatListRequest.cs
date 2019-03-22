using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetCustomerRepeatListRequest : BaseRequest2
    {
        private int _pageIndex = 1;
        private int _pageSize = 15;
        /// <summary>
        /// 当前页
        /// </summary>
        [Range(1, 10000000)]
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }
        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize 
        {
            get { return _pageSize; }
            set { _pageSize = value; }
            
        }

        private int _roleType = -1;
        /// <summary>
        /// 4:管理员   
        /// </summary>
        public int RoleType
        {
            get { return _roleType; }
            set { _roleType = value; }
        }
        /// <summary>
        /// 1.车牌号
        /// 2.车架号
        /// 3.客户电话和客户名称
        /// </summary>
        [Range(1, 3)]
        public int GroupByType { get; set; }
    }
}
