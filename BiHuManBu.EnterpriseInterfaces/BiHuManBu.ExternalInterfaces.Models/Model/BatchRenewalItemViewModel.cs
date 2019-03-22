using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class BatchRenewalItemViewModel
    {

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        private string _mobile;
        /// <summary>
        /// 客户电话1
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }
        private string _licenseNo;
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo
        {
            get { return _licenseNo; }
            set { _licenseNo = value; }
        }

        private string _engineNo;
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo
        {
            get { return _engineNo; }
            set { _engineNo = value; }
        }
        private string _moldName;
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName
        {
            get { return _moldName; }
            set { _moldName = value; }
        }
        //private string _registerDate;
        /// <summary>
        /// 注册时间
        /// </summary>
        //public string RegisterDate
        //{
        //    get { return _registerDate; }
        //    set { _registerDate = value; }
        //}

        private string _vinNo;
        /// <summary>
        /// 车架号
        /// </summary>
        public string VinNo
        {
            get { return _vinNo; }
            set { _vinNo = value; }
        }
        private int _itemStatus;
        /// <summary>
        /// 状态 0续保失败，需要重新续保 -1需要续保，待中心处理  非0不需要续保
        /// </summary>
        public int ItemStatus
        {
            get { return _itemStatus; }
            set { _itemStatus = value; }
        }
        private long _buId;

        public long BuId
        {
            get { return _buId; }
            set { _buId = value; }
        }
        private int _isLastYearNewCar;
        /// <summary>
        /// 是否为新车
        /// </summary>
        public int IsLastYearNewCar
        {
            get { return _isLastYearNewCar; }
            set { _isLastYearNewCar = value; }
        }
        private int _renewalStatus;
        public int RenewalStatus
        {
            get { return _renewalStatus; }
            set { _renewalStatus = value; }
        }
        private int _needEngineNo;

        public int NeedEngineNo
        {
            get { return _needEngineNo; }
            set
            {
                _needEngineNo = value;

            }
        }
        private string _lastYearSource = "-1";
        public string LastYearSource
        {
            get { return _lastYearSource; }
            set { _lastYearSource = value; }
        }
        //  public string LastYearSource { get; set; }
        public string SalesManName { get; set; }
        public string SalesManAccount { get; set; }

        public string Agent { get; set; }
        /// <summary>
        /// 光鹏杰使用
        /// </summary>
        public int agent_id { get; set; }
        /// <summary>
        /// 光鹏杰使用
        /// </summary>
        public int top_agent_id { get; set; }
        public DateTime DistributedTime { get; set; }
        public int IsDistributed { get; set; }
        public string OpenId { get; set; }

        /// <summary>
        /// 客户类别主键编号
        /// </summary>
        private int? _categoryinfoId;

        public int? CategoryinfoId
        {
            get { return _categoryinfoId; }
            set { _categoryinfoId = value; }
        }
        /// <summary>
        /// 客户类别信息编号
        /// </summary>
        private string _categoryInfo;

        public string CategoryInfo
        {
            get { return _categoryInfo; }
            set { _categoryInfo = value; }
        }

        /// <summary> 
        /// 客户电话2
        /// </summary>
        private string _client_mobile_other;

        public string Client_mobile_other
        {
            get { return _client_mobile_other; }
            set { _client_mobile_other = value; }
        }

        public DateTime? ForceEndDate { get; set; }

        public DateTime? BizEndDate { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        /// <summary>
        /// 备注2
        /// </summary>
        public string Intention_Remark { get; set; }

        public string RegisterDate { get; set; }

        /// <summary>
        /// 身份证后六位
        /// </summary>
        public string SixDigitsAfterIdCard { get; set; }
    }

    public class ExcelErrorData //: IValidatableObject
    {
        public string LicenseNo { get; set; }
        public string VinNo { get; set; }
        public string EngineNo { get; set; }
        public string MoldName { get; set; }
        public string LastYearSource { get; set; }
        public string ForceEndDate { get; set; }
        public string BizEndDate { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public string MobileOther { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 备注2
        /// </summary>
        public string Intention_Remark { get; set; }

        ////客户类型
        public string CategoryInfo { get; set; }
        public string SalesManName { get; set; }
        ////客户类型
        public string SalesManAccount { get; set; }
        public string ErrorMsg { get; set; }

        private DateTime _createTime = DateTime.Now;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        public int RowIndex { get; set; }
        public string RegisterDate { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    List<ValidationResult> validationResult = new List<ValidationResult>();

        //    if (!string.IsNullOrWhiteSpace(LicenseNo) && LicenseNo.Length > 100)
        //    {
        //        sb.Append("车牌号最大长度为100；");
        //        LicenseNo = LicenseNo.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(VinNo) && VinNo.Length > 100)
        //    {
        //        sb.Append("车架号最大长度为100；");
        //        VinNo = VinNo.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(EngineNo) && EngineNo.Length > 100)
        //    {
        //        sb.Append("发动机号最大长度为100；");
        //        EngineNo = EngineNo.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(MoldName) && MoldName.Length > 100)
        //    {
        //        sb.Append("品牌型号最大长度为100；");
        //        MoldName = MoldName.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(LastYearSource) && LastYearSource.Length > 200)
        //    {
        //        sb.Append("去年投保公司最大长度为200；");
        //        LastYearSource = LastYearSource.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(ForceEndDate) && ForceEndDate.Length > 50)
        //    {
        //        sb.Append("交强险到期时间最大长度为50；");
        //        ForceEndDate = ForceEndDate.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(BizEndDate) && BizEndDate.Length > 50)
        //    {
        //        sb.Append("商业险到期时间最大长度为50；");
        //        BizEndDate = BizEndDate.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(CustomerName) && CustomerName.Length > 100)
        //    {
        //        sb.Append("客户姓名最大长度为100；");
        //        CustomerName = CustomerName.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(Mobile) && Mobile.Length > 20)
        //    {
        //        sb.Append("客户电话1最大长度为20；");
        //        Mobile = Mobile.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(MobileOther) && MobileOther.Length > 20)
        //    {
        //        sb.Append("客户电话2最大长度为20；");
        //        MobileOther = MobileOther.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(Remark) && Remark.Length > 200)
        //    {
        //        sb.Append("客户备注最大长度为200；");
        //        Remark = Remark.Substring(0, 100).ToString();
        //    }
        //    if (!string.IsNullOrWhiteSpace(ErrorMsg) && ErrorMsg.Length > 200)
        //    {
        //        ErrorMsg = sb.ToString();
        //    }




        //    return validationResult;

        //}
    }

    public class CheckBackModel
    {
        private DateTime? lastForceEndDate;
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public DateTime? LastForceEndDate
        {
            get { return lastForceEndDate; }
            set { lastForceEndDate = value; }
        }
        private DateTime? lastBizEndDate;
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? LastBizEndDate
        {
            get { return lastBizEndDate; }
            set { lastBizEndDate = value; }
        }

        /// <summary>
        /// bx_userInfo.id
        /// </summary>
        public long BuId { get; set; }

        public string Agent { get; set; }

        public string AgentName { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string RenewalType { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 备注2
        /// </summary>
        public string Intention_Remark { get; set; }

        /// <summary>
        /// 客户电话1
        /// </summary>
        public string client_mobile { get; set; }
        /// <summary>
        /// 客户电话2
        /// </summary>
        public string client_mobile_other { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string client_name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        private string _openId;

        public string OpenId
        {
            get { return _openId; }
            set { _openId = value; }
        }
        private DateTime _distributedTime;

        public DateTime DistributedTime
        {
            get { return _distributedTime; }
            set { _distributedTime = value; }
        }
        //是否分配

        public int IsDistributed { get; set; }

        private int? _categoryInfoId;

        public int? CategoryInfoId
        {
            get { return _categoryInfoId; }
            set { _categoryInfoId = value; }
        }

        public int RenewalStatus { get; set; }

        public int NeedEngineNo { get; set; }

        public string LastYearSource { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        public string MoldName { get; set; }
        public string EngineNo { get; set; }
    }

    public class BatchRenewalViewModel
    {
        private long _id;
        /// <summary>
        /// BatchRenewal 主键
        /// </summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _fileName;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        public long ErrorDataCount { get; set; }
        public int IsCompleted { get; set; }
        public int IsAgainRenewal { get; set; }
        private DateTime _createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        /// <summary>
        /// 总条数
        /// </summary>
        public long TotalCount { get; set; }
        private long _successfullCount;
        /// <summary>
        /// 处理成功条数
        /// </summary>
        public long SuccessfullCount
        {
            get { return _successfullCount; }
            set { _successfullCount = value; }
        }

        private long _failedCount;
        /// <summary>
        /// 处理失败条数
        /// </summary>
        public long FailedCount
        {
            get { return _failedCount; }
            set { _failedCount = value; }
        }

        private long _untreatedCount;
        /// <summary>
        /// 未处理条数
        /// </summary>
        public long UntreatedCount
        {
            get { return _untreatedCount; }
            set { _untreatedCount = value; }
        }

        private DateTime _startExecuteTime;
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime StartExecuteTime
        {
            get { return _startExecuteTime; }
            set { _startExecuteTime = value; }
        }

        /// <summary>
        /// 是否分发
        /// </summary>
        private bool _isDistributed;

        public bool IsDistributed
        {
            get { return _isDistributed; }
            set { _isDistributed = value; }
        }

        private int _taskStatus;
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStatus
        {
            get { return _taskStatus; }
            set { _taskStatus = value; }
        }
        public int CityId { get; set; }
        public string ChannelPattern { get; set; }
        public int BatchRenewalType { get; set; }
        public string FilePath { get; set; }


    }




    /// <summary>
    /// 批量修改
    /// </summary>
    public class BatchRenewalUserInfoModel
    {
        public long Id { get; set; }
        public int RenewalStatus { get; set; }
    }
    /// <summary>
    /// 批量修改
    /// </summary>
    public class ChannelPatternModel
    {
        public int ChannelType;
        public List<int> SelectedSources;
        /// <summary>
        /// 是否历史承保
        /// </summary>
        public int IsHistoryRenewal;
    }
    public class UpdateBatchRenewalModel
    {
        public long Id { get; set; }
        public int TotalCount { get; set; }
        public int TreateSuccessedCount { get; set; }
        public int TreateFailedCount { get; set; }
        public int UntreatedCount { get; set; }
        public int ItemTaskStatus { get; set; }
    }

}
