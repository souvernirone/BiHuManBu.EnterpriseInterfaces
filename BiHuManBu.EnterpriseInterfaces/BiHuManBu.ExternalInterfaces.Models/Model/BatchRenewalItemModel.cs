using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class BatchRenewalItemModel
    {
        public long BatchId { get; set; }
        public long BUId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string EngineNo { get; set; }
        public int ItemStatus { get; set; }
        public string LicenseNo { get; set; }
        public string Mobile { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string VinNo { get; set; }
        public string CustomerName { get; set; }
        public long InitBuId { get; set; }
        public int LastYearSource { get; set; }
        public DateTime? ForceEndDate { get; set; }
        public int HistoryItemStatus { get; set; }
        public DateTime? BizEndDate { get; set; }
        public string SalesManName { get; set; }
        public string SalesManAccount { get; set; }
        //是否最新 0:不是最新,1:最新
        public int IsNew { get; set; }
        /// <summary>
        /// 身份证后六位
        /// </summary>
        public string SixDigitsAfterIdCard { get; set; }

    }
    public class BatchQuotereqCarinfoModel
    {
        public string licenseNo { get; set; }
        public long b_uid { get; set; }
        public int is_lastnewcar { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }
    public class NewBatchQuotereqCarinfoModel
    {
        public long b_uid { get; set; }
        public int is_lastnewcar { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }
    public class BatchRenewalItemModelNew
    {
        public long BUId { get; set; }
        public string Mobile { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string CustomerName { get; set; }
        public string remark { get; set; }

        /// <summary>
        /// 备注2
        /// </summary>
        public string Intention_Remark { get; set; }
        public string client_mobile_other { get; set; }
    }

    public class UserRenewalInfoModel
    {
        public long b_uid { get; set; }
        public DateTime create_time { get; set; }
        public string client_mobile { get; set; }
        public string client_mobile_other { get; set; }
        public string remark { get; set; }
        public string Intention_Remark { get; set; }
        public string client_name { get; set; }
    }

    public class BatchUpdateQuotereqCarinfoModel
    {
        public long b_uid { get; set; }
        public int is_lastnewcar { get; set; }
        public DateTime update_time { get; set; }
    }

    public class BatchRenewalSearchWhere
    {
        private string _UploadStartTime_Str;
        /// <summary>
        /// 上传开始时间
        /// </summary>
        public string UploadStartTime_Str
        {
            get { return _UploadStartTime_Str; }
            set { _UploadStartTime_Str = value; }
        }

        private string _UploadEndTime_Str;
        /// <summary>
        /// 上传结束时间
        /// </summary>
        public string UploadEndTime_Str
        {
            get { return _UploadEndTime_Str; }
            set { _UploadEndTime_Str = value; }
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
        private int _taskStatus;
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStatus
        {
            get { return _taskStatus; }
            set { _taskStatus = value; }
        }

        /// <summary>
        /// 车牌号
        /// </summary>
        private string _licenseNo;
        public string LicenseNo
        {
            get { return _licenseNo; }
            set { _licenseNo = value; }
        }

    }


    public class DownLoadExcel
    {

        private string _licenseNo;
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo
        {
            get { return _licenseNo; }
            set { _licenseNo = value; }
        }
        private string _name;
        /// <summary>
        /// 车主
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
        private string _engineNo;
        /// <summary>
        /// 发动机型号
        /// </summary>
        public string EngineNo
        {
            get { return _engineNo; }
            set { _engineNo = value; }
        }
        private string _vinNo;
        /// <summary>
        /// 车辆识别代码
        /// </summary>
        public string VinNo
        {
            get { return _vinNo; }
            set { _vinNo = value; }
        }
        private string _registerDate;
        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegisterDate
        {
            get { return _registerDate; }
            set { _registerDate = value; }
        }



        private string _preSource;
        /// <summary>
        /// 上年承保公司
        /// </summary>
        public string PreSource
        {
            get { return _preSource; }
            set { _preSource = value; }
        }
        private string _businessRisksEndTime;
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string BusinessRisksEndTime
        {
            get { return _businessRisksEndTime; }
            set { _businessRisksEndTime = value; }
        }
        private string _forceRisksEndTime;
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public string ForceRisksEndTime
        {
            get { return _forceRisksEndTime; }
            set { _forceRisksEndTime = value; }
        }

        private string _insuredPeopleName;
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredPeopleName
        {
            get { return _insuredPeopleName; }
            set { _insuredPeopleName = value; }
        }


        private string _idCard;
        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard
        {
            get { return _idCard; }
            set { _idCard = value; }
        }

        private string _categoryInfo;

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }
        //private string _mobile;
        ///// <summary>
        ///// 手机号
        ///// </summary>
        //public string Mobile
        //{
        //    get { return _mobile; }
        //    set { _mobile = value; }
        //}
        private string _mobile1;

        /// <summary>
        /// 客户电话1
        /// </summary>
        public string Mobile1
        {
            get { return _mobile1; }
            set { _mobile1 = value; }
        }
        /// <summary>
        /// 客户电话2
        /// </summary>
        private string _mobile2;

        public string Mobile2
        {
            get { return _mobile2; }
            set { _mobile2 = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string _remark;
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryInfo
        {
            get { return _categoryInfo; }
            set { _categoryInfo = value; }
        }

        private string _customerName;
        private string _salesManName;

        public string SalesManName
        {
            get { return _salesManName; }
            set { _salesManName = value; }
        }
        private string _salesManAccount;

        public string SalesManAccount
        {
            get { return _salesManAccount; }
            set { _salesManAccount = value; }
        }


        private int _itemStatus;

        /// <summary>
        /// 状态
        /// </summary>
        public int ItemStatus
        {
            get { return _itemStatus; }
            set { _itemStatus = value; }
        }

        private int _istest;

        public int Istest
        {
            get { return _istest; }
            set { _istest = value; }
        }
    }

    public class BatchRenewalSource
    {
        public int Id { get; set; }
        public string SourceName { get; set; }
        public int isuse{ get; set; }
    }

    /// <summary>
    /// 续保列表请求类
    /// </summary>
    public class BatchRenewalListRequest : BaseRequest2
    {

        //上传开始时间
        public string uploadStartTime
        {
            set;
            get;
        }
        //上传结束时间
        public string uploadEndTime
        {
            set;
            get;
        }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string licenseNo
        {
            set;
            get;
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            set;
            get;
        }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStatus
        {
            set;
            get;
        }
        /// <summary>
        ///条数
        /// </summary>
        public int pageSize
        {
            set;
            get;
        }
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex
        {

            set;
            get;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int orderBy
        {
            set;
            get;
        }
        /// <summary>
        /// 代理人
        /// </summary>
        public int agentId
        {
            set;
            get;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int type
        {

            set;
            get;
        }
        /// <summary>
        /// 批量续保当前页面批次编号集合
        /// </summary>
        public List<long> presentBatchIds { get; set; }
      
    }
   
}
