using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 客户列表搜索公共参数 
    /// </summary>
    public class BaseCustomerSearchRequest : BaseRequest2
    {
        /// <summary>
        /// 交强险到期 开始 不需要时分秒 
        /// </summary>
        public string ForceEndDateStart { get; set; }

        /// <summary>
        /// 交强险到期 结束 不需要时分秒
        /// </summary>
        public string ForceEndDateEnd { get; set; }

        /// <summary>
        /// 商业险到期 开始 不需要时分秒
        /// </summary>
        public string BusinessEndDateStart { get; set; }

        /// <summary>
        /// 商业险到期 结束 不需要时分秒
        /// </summary>
        public string BusinessEndDateEnd { get; set; }

        /// <summary>
        /// 1.交强到期天数，默认90天   
        /// 2.续保期未回访(续保期天数,登录时会拿到,然后前端传给CRM接口)
        /// </summary>
        public int DaysNum { get; set; }

        /// <summary>
        /// 商业险到期天数
        /// </summary>
        public int BizDaysNum { get; set; }
    
        /// <summary>
        /// 1.今日摄像头进店（续保期内） 使用
        /// 2.续保期未回访(续保期天数,登录时会拿到,然后前端传给CRM接口)
        /// </summary>
        public int Label3DaysNum { get; set; }

        /// <summary>
        /// 模糊查询子集下的代理人
        /// </summary>
        public string ChildAgentName { get; set; }

        /// <summary>
        /// 下级代理人id，在搜索的时候传id过来，以前是传名称的
        /// </summary>
        public int SearchAgentId { get; set; }

        private List<long> _buids = new List<long>();
        /// <summary>
        /// 根据buid批量查询
        /// </summary>
        public List<long> Buids
        {
            get { return _buids; }
            set
            {
                if (value != null)
                    _buids = value;
            }
        }


        private string _renewalType = "-1";

        /// <summary>
        /// 录入方式 3:摄像头 4:PC 5:批量 6:APP（包括6苹果和7安卓） 8:微信 
        /// 格式：1,2,3 （多选用逗号隔开）
        /// </summary>
        public string RenewalType
        {
            get { return _renewalType; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _renewalType = value;
            }
        }

        private int _noQuote = -1;
        /// <summary>
        /// 是否不取报价 1是 0不是
        /// </summary>
        public int NoQuote
        {
            get { return _noQuote; }
            set { _noQuote = value; }
        }

        private string _lastYearSource = "-1";

        /// <summary>
        /// 去年投保公司，新的Id编号1 2 4 8...
        /// 格式：1,2,3 （多选用逗号隔开）
        /// </summary>
        public string LastYearSource
        {
            get { return _lastYearSource; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _lastYearSource = value;
            }
        }

        private string _renewalStatus = "-1";

        /// <summary>
        /// 续保状态 续保成功1 续保失败2 只取到行驶本3 未处理4
        /// 格式：1,2,3 （多选用逗号隔开）
        /// </summary>
        public string RenewalStatus
        {
            get { return _renewalStatus; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _renewalStatus = value;
            }
        }

        private string _customerStatus = "-1";

        /// <summary>
        /// 客户状态
        /// 格式：1,2,3 （多选用逗号隔开）
        /// </summary>
        public string CustomerStatus
        {
            get { return _customerStatus; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _customerStatus = value;
            }
        }

        /// <summary>
        /// 记录创建搜索 开始 需要时分秒
        /// </summary>
        public string UpdateDateStart { get; set; }

        /// <summary>
        /// 记录创建搜索 结束 需要时分秒
        /// </summary>
        public string UpdateDateEnd { get; set; }

        /// <summary>
        /// 车辆注册时间 开始 不需要时分秒
        /// </summary>
        public string RegistDateStart { get; set; }

        /// <summary>
        /// 车辆注册时间 结束 不需要时分秒
        /// </summary>
        public string RegistDateEnd { get; set; }

        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }

        /// <summary>
        ///  车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 录入回访时间的起始时间 需要时分秒
        /// </summary>
        public string InputVisitTimeStart { get; set; }

        /// <summary>
        /// 录入回访时间结束时间 需要时分秒
        /// </summary>
        public string InputVisitTimeEnd { get; set; }

        private List<int> _isDistributed = new List<int>();

        /// <summary>
        /// 是否分配，这里要是添加其他值的话要考虑用到这个值的所有地方
        /// 4已分配 1未分配
        /// </summary>
        public List<int> IsDistributed
        {
            get { return _isDistributed; }
            set
            {
                if (value != null)
                    _isDistributed = value;
            }
        }

        /// <summary>
        /// 批量续保的批次号
        /// </summary>
        public long BatchId { get; set; }

        /// <summary>
        /// 来自哪里的查询 -1:回收站  3:摄像头进店列表
        /// </summary>
        public int FormType { get; set; }

        //private int label = -1;

        ///// <summary>
        ///// 下级代理人标签的类型
        ///// -1||0：全部  1：上级新分配  2：今日计划回访  3：今日摄像头进店（续保期内） 4:预约到店
        ///// </summary>
        //[Range(-1, 10)]
        //public int Label { get { return label; } set { label = value; } }

        /// <summary>
        /// 顶级代理人标签的类型
        /// -1||0：全部    1：已分配未回访    2：今日计划回访    3：今日进店（续保期） 4：预约到店   5：0-90天内到期（未分配）    6：未分配
        /// 8：逾期客户  9：续保期未回访
        /// </summary>
        public int TopLabel { get; set; }

        /// <summary>
        /// 当label=2时需要根据这个参数判断是获取几天后的数据
        /// 0:当天  1:明天  2:后天  3:第3天  4:第4天  5:第5天  6:第6天  7:第7天  -1:预期
        /// </summary>
        public int LabelTimeSpan { get; set; }

        /// <summary>
        /// 分配开始时间
        /// </summary>
        public string DistriStartTime { get; set; }

        /// <summary>
        /// 分配结束时间
        /// </summary>
        public string DistriEndTime { get; set; }

        /// <summary>
        /// 摄像头进店开始时间 不需要时分秒
        /// </summary>
        public string CameraStartTime { get; set; }

        /// <summary>
        /// 摄像头进店结束时间 不需要时分秒
        /// </summary>
        public string CameraEndTime { get; set; }

        private string _clientCategoryID = "-1";

        /// <summary>
        /// 客户类别关联编号
        /// 格式：1,2,3 （多选用逗号隔开）
        /// </summary>
        public string ClientCategoryID
        {
            get { return _clientCategoryID; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _clientCategoryID = value;
            }
        }

        /// <summary>
        /// 客户电话号码
        /// </summary>
        public string ClientMobile { get; set; }

        /// <summary>
        /// 计划回访时间的起始时间 需要时分秒
        /// </summary>
        public string VisitTimeStart { get; set; }

        /// <summary>
        /// 计划回访时间结束时间 需要时分秒
        /// </summary>
        public string VisitTimeEnd { get; set; }


        private List<int> _isQuote = new List<int>();

        /// <summary>
        /// 报价状态
        /// 0:全部 1:报过价  2:没报过价 
        /// </summary>
        public List<int> IsQuote
        {
            get { return _isQuote; }
            set
            {
                if (value != null)
                    _isQuote = value;
            }
        }

        /// <summary>
        /// 角色类型，这个参数目前已经不用了，但是为了防止校验失败，没有去掉
        /// </summary>
        [Obsolete("这个参数目前已经不用了，但是为了防止校验失败，没有去掉")]
        public int AgentRoleType { get; set; }


        private bool _displayHandledData = true;

        /// <summary>
        /// 是否展示已出单的和战败数据
        /// </summary>
        public bool DisplayHandledData { get { return _displayHandledData; } set { _displayHandledData = value; } }

        private string _buidString;

        /// <summary>
        /// 这是在摄像头进店导出和客户列表导出时，配合前端添加了参数
        /// </summary>
        public string BuidString
        {
            set
            {
                _buidString = value;
                if (this.Buids.Count == 0 && !string.IsNullOrWhiteSpace(value))
                {
                    var array = value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in array)
                    {
                        Buids.Add(Convert.ToInt64(item));
                    }
                }
            }
            get { return _buidString; }
        }

        private List<int> _customerIsReview = new List<int>();

        /// <summary>
        /// 当前顶级代理人所有的客户状态
        /// 前端不是每次请求都会传，所以用到该参数了，要是没有值就从库里面取一次
        /// </summary>
        public List<int> CustomerIsReview
        {
            set
            {
                if (value != null)
                {
                    _customerIsReview = value;
                }
            }
            get
            {
                return _customerIsReview;
            }
        }

        private List<int> _isHasClientMobile = new List<int>();

        /// <summary>
        /// 有无客户电话   【2018-04-17】
        /// 1:有  2:无 
        /// </summary>
        public List<int> IsHasClientMobile
        {
            get { return _isHasClientMobile; }
            set
            {
                if (value != null)
                    _isHasClientMobile = value;
            }
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        private int _deleteType = -1;

        /// <summary>
        /// 删除类型：0.普通删除，1.去重删除，2.摄像头过滤
        /// add 2018-06-13
        /// </summary>
        public int DeleteType
        {
            get { return _deleteType; }
            set { _deleteType = value; }
        }
        /// <summary>
        /// 删除时间：开始，对应delete_time
        /// add 2018-06-13
        /// </summary>
        public string StartDeleteTime { get; set; }
        /// <summary>
        /// 删除时间：结束，对应delete_time
        /// add 2018-06-13
        /// </summary>
        public string EndDeleteTime { get; set; }


        public int HasZhuDianYuanRole { get; set; }
        public string CameraId { get; set; }

        /// <summary>
        /// 车主名称
        /// 2018-08-16
        /// </summary>
        public string LicenseOwner { get; set; }

        private List<int> _searchAgentIdList = new List<int>();

        /// <summary>
        /// 下级代理人id集合，在搜索的时候传多个代理人Id过来
        /// 业务员筛选支持多选
        /// 2018-08-16
        /// </summary>
        public List<int> SearchAgentIdList
        {
            get { return _searchAgentIdList; }
            set
            {
                if (_searchAgentIdList == null)
                {
                    _searchAgentIdList = value;
    }
            }
        }

        private int _bizEndMonth = 0;
        /// <summary>
        /// 按月：商业险到期时间
        /// 2018-08-16
        /// 格式：1，12
        /// </summary>
        public int BizEndMonth
        {
            get { return _bizEndMonth; }
            set { _bizEndMonth = value; }
        }

        private int _forceEndMonth = 0;
        /// <summary>
        /// 按月：交强险到期时间
        /// 2018-08-16
        /// </summary>
        public int ForceEndMonth
        {
            get { return _forceEndMonth; }
            set { _forceEndMonth = value; }
        }
    }
}
