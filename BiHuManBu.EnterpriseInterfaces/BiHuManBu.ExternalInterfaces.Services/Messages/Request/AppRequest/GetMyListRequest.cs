using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetMyListRequest : AppBaseRequest
    {
        private int _endNum = 1;
        /// <summary>
        /// 到期天数；当前在我的客户中使用，1：代表不参与筛选
        /// </summary>
        public int EndNum
        {
            get
            {
                return _endNum;
            }

            set
            {
                _endNum = value;
            }
        }

        private int bizEndNum = 1;

        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public int BizEndNum { get { return bizEndNum; }set { bizEndNum = value; } }

        private int _dataType = 1;
        /// <summary>
        /// 1:续保报价，2:续保(已和app确认目前没有单独调用2的)，3:报价
        /// </summary>
        public int DataType
        {
            get
            {
                return _dataType;
            }

            set
            {
                _dataType = value;
            }
        }
        private int _isOnlyMine = 1;
        private int _orderCreateTime = 1;
        public string LicenseNo { get; set; }

        [Range(1, 10000)]
        public int PageSize { get; set; }

        [Range(1, 10000)]
        public int CurPage { get; set; }

        /// <summary>
        /// 是否只查属于自己代理的bx_userinfo。1，是；0，否
        /// 如果是1，则不查下级代理
        /// </summary>
        public int IsOnlyMine
        {
            get { return _isOnlyMine; }
            set { _isOnlyMine = value; }
        }

        /// <summary>
        /// 根据什么排序。1，录入时间；2，到期时间
        /// 列表排序默认按照录入时间排
        /// </summary>
        public int OrderBy
        {
            get { return _orderCreateTime; }
            set { _orderCreateTime = value; }
        }

        /// <summary>
        /// 分配Id
        /// </summary>
        public int DistributedId { get; set; }
    }
}
