using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetPreferentialActivityListRequest : BaseVerifyRequest
    {
        /// <summary>
        /// 查询用的ID集合  格式为：xx,xx,xx / xx
        /// </summary>
        public string Ids { get; set; }
    }

    public class GetActivityPageListRequest : BaseVerifyRequest
    {
        #region 分页参数
        private int _orderBy = 1;
        private int _roleType = -1;
        private int _curPage = 1;
        private int _showPageNum = 1;


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
        public int CurPage
        {
            get { return _curPage; }
            set { _curPage = value; }
        }

        #endregion

        /// <summary>
        /// 活动类别
        /// </summary>
        public int activityType { get; set; }
    }

    public class DelPreferentialActivityListRequest : BaseVerifyRequest 
    {
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyName { get; set; }

        /// <summary>
        /// 查询用的ID集合  格式为：xx,xx,xx / xx
        /// </summary>
        public string Ids { get; set; }
    }
}
