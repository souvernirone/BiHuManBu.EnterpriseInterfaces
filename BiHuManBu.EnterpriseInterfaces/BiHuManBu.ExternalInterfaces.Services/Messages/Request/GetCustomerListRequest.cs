using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class GetCustomerListRequest : BaseCustomerSearchRequest
    {
        private int _orderBy = 1;
        private int _roleType = -1;
        private int _curPage = 1;
        private int _showPageNum = 10;


        /// <summary>
        /// 4:管理员   
        /// </summary>
        public int RoleType
        {
            get { return _roleType; }
            set { _roleType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 展示第几页
        /// </summary>
        [Range(1, 10000000)]
        public int CurPage
        {
            get { return _curPage; }
            set { _curPage = value; }
        }

        /// <summary>
        /// 一屏显示多少页
        /// </summary>
        public int ShowPageNum
        {
            get { return _showPageNum; }
            set { _showPageNum = value; }
        }

        /// <summary>
        /// 排序 1录入时间降 2交强险到期日期降 21升 3商业险到期日期降 31升 4回访时间降  41回访时间升 5摄像头进店时间 降序排列 51摄像头进店时间 升序
        /// </summary>
        public int OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        /// <summary>
        /// 是否只要统计数据 1是 0不是
        /// </summary>
        public int OnlyCount { get; set; }

        /// <summary>
        /// 是否查全部标签的数量
        /// </summary>
        public bool FindAllCount { get; set; }
    }

    public class GetCustomerCountRequest: BaseCustomerSearchRequest
    {
        ///// <summary>
        ///// 是否是主管：不是系统角色，但是有分配权限
        ///// </summary>
        //public bool IsManager { get; set; }
    }
}
