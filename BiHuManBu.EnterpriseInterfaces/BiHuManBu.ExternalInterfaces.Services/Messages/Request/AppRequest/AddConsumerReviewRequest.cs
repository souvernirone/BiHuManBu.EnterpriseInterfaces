using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
   public class AddConsumerReviewRequest : AppBaseRequest, IValidatableObject
    {
        /// <summary>
        /// 信鸽账号
        /// </summary>
        public string  XgAccout { get; set; }
        /// <summary>
        /// 回访内容
        /// </summary>
        public string ConsumerReviewName { get; set; }
        /// <summary>
        /// 父级状态
        /// </summary>
        public int ParentStatus { get; set; }

        private int _preReviewStatus = -1;

        private DateTime _createTime = DateTime.Now;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        ///// <summary>
        ///// bx_userinfo.Id
        ///// </summary>
        //public int BuId { get; set; }
        private int _reviewStatus = -2;
        /// <summary>
        /// 回访状态
        /// </summary>
        public int ReviewStatus
        {
            get { return _reviewStatus; }
            set { _reviewStatus = value; }
        }
        /// <summary>
        /// 回访时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }
        /// <summary>
        /// 回访内容
        /// </summary>
        public string ReviewContent { get; set; }
        private int _defeatReasonId = -1;
        /// <summary>
        /// 战败标签编号(以此字段判断当前是否选择是战败状态)
        /// </summary>
        public int DefeatReasonId
        {
            get
            {
                return _defeatReasonId;
            }

            set
            {
                _defeatReasonId = value;
            }
        }
        /// <summary>
        /// 战败标签内容
        /// </summary>
        public string DefeatReasonContent { get; set; }
        /// <summary>
        /// 保险公司（以此字段判断当前状态是否选择是已出单）
        /// </summary>
        public long Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
            }
        }
        private long _source = -1;
        /// <summary>
        /// 出单时间
        /// </summary>
        public DateTime? SingleTime { get; set; }

        /// <summary>
        /// 类别编号
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? BizEndTime { get; set; }
        private int _bizEndTimeIsBySeft = 0;
        /// <summary>
        /// 是否为自己写入（0：否，1：是）
        /// </summary>
        public int BizEndTimeIsBySeft { get { return _bizEndTimeIsBySeft; } set { _bizEndTimeIsBySeft = value; } }
        /// <summary>
        /// 上次回访状态
        /// </summary>
        public int PreReviewStatus
        {
            get
            {
                return _preReviewStatus;
            }

            set
            {
                _preReviewStatus = value;
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (ReviewStatus <= -2)
            {
                validationResult.Add(new ValidationResult("ReviewStatus为必要参数"));
            }
            if (DefeatReasonId > -1)
            {
                if (string.IsNullOrWhiteSpace(DefeatReasonContent))
                {
                    validationResult.Add(new ValidationResult("战败标签必须选择"));
                }
            }
            if (!Buid.HasValue || Buid == 0)
            {
                validationResult.Add(new ValidationResult("Buid为必要参数"));
            }
            return validationResult;
        }
    }
}
