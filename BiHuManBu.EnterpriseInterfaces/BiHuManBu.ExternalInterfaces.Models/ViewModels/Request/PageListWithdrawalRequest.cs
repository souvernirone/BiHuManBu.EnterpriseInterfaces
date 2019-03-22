using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class PageListWithdrawalRequest : BaseRequest2 
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
        /// 体现人名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 体现人手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 审核状态：0为支付，1已支付
        /// </summary>
        public int AuditStatus { get; set; }
    }
}
