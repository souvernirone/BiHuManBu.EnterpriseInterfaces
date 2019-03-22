using System.Collections.Generic;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 客户列表搜索
    /// </summary>
    public class SearchCustomerListDto
    {
        private StringBuilder _sqlBuilder = new StringBuilder();
        /// <summary>
        /// 存放拼接好的sql
        /// </summary>
        public StringBuilder SqlBuilder
        {
            get { return _sqlBuilder; }
            set { _sqlBuilder = value; }
        }

        /// <summary>
        /// 用来存放IsReviewList的HashSet
        /// </summary>
        private HashSet<int> _isReviewHashSet = new HashSet<int>();
        /// <summary>
        /// 客户状态的集合
        /// </summary>
        public HashSet<int> IsReviewHashSet
        {
            get { return _isReviewHashSet; }
            set { _isReviewHashSet = value; }
        }

        /// <summary>
        /// 用来存放IsReviewList的HashSet
        /// 最后生成的是not in的SQL
        /// </summary>
        private HashSet<int> _isReviewHashSetNotIn = new HashSet<int>();
        /// <summary>
        /// 客户状态的集合
        /// </summary>
        public HashSet<int> IsReviewHashSetNotIn
        {
            get { return _isReviewHashSetNotIn; }
            set { _isReviewHashSetNotIn = value; }
        }

        /// <summary>
        /// 需要连接的sql
        /// </summary>
        public string JoinWhere { get; set; }

        /// <summary>
        /// UserInfoRepository.GetJoinSql的JoinType
        /// </summary>
        public int JoinType { get; set; }

        public int PageSize { get; set; }

        public int CurPage { get; set; }
        /// <summary>
        /// GetCustomerListRequest.OrderBy
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShowPageNum { get; set; }

        public int CurrentAgent { get; set; }

        /// <summary>
        /// 是否有条件
        /// </summary>
        public bool NeedForceIndex { get; set; }

        private List<int> _listAgent = new List<int>();

        /// <summary>
        /// 搜索的代理人id
        /// </summary>
        public List<int> ListAgent
        {
            get { return _listAgent; }
            set
            {
                if (value != null)
                    _listAgent = value;
            }
        }

        /// <summary>
        /// 不是系统管理员和管理员，是否有分配权限
        /// </summary>
        public bool HasDistribute { get; set; }

        /// <summary>
        /// 是否是下级代理人数量超过2000的顶级代理人
        /// </summary>
        public bool HasMoreThan2000ChildAgent { get; set; }

        /// <summary>
        /// 是否关联扩展表:0不关联，1关联
        /// </summary>
        private int _isJoinExpand = 0;
        public int IsJoinExpand
        {
            get { return _isJoinExpand; }
            set { _isJoinExpand = value; }
        }

        private int _isOwnerInquiry = 0;

        /// <summary>
        /// 是否是车主报价: 1是，0否
        /// </summary>
        public int IsOwnerInquiry
        {
            get { return _isOwnerInquiry; }
            set { _isOwnerInquiry = value; }
        }
        /// <summary>
        /// 是否关联摄像头配置表
        /// </summary>
        private int _isJoinCameraConfig = 0;
        public int IsJoinExpandCameraConfig
        {
            get { return _isJoinCameraConfig; }
            set { _isJoinCameraConfig = value; }
        }
    }
}
