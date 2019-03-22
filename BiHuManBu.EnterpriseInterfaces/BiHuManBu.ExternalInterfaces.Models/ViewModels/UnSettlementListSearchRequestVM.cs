using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UnSettlementListSearchRequestVM : IValidatableObject
    {


        /// <summary>
        /// 当前代理人编号
        /// </summary>
        public int CurrentAgentId { get; set; }

        /// <summary>
        /// 刷卡开始时间
        /// </summary>
        public DateTime? SwingCardStartDate { get; set; }

        /// <summary>
        /// 刷卡结束时间
        /// </summary>
        public DateTime? SwingCardEndDate { get; set; }

        private int _searchAgentId = -1;
        /// <summary>
        /// 查询代理人编号
        /// </summary>
        public int SearchAgentId
        {
            get { return _searchAgentId; }
            set { _searchAgentId = value; }
        }

        private int _channelId = -1;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public int ChannelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }
        /// <summary>
        /// 模式类型：1->模式一、2->模式二、3->模式三
        /// </summary>
        public int ModelType { get; set; }
        /// <summary>
        /// 1->代理人、2->网点、3->机构佣金、4->机构毛利、5->保司
        /// </summary>
        public int UnSettleType { get; set; }
        private int _pageIndex = 1;
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }

        private int _pageSize = 10;

        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (CurrentAgentId <= 0) {
                validationResult.Add(new ValidationResult("CurrentAgentId为必要参数"));

            }
            if (!new List<int> { 1, 2, 3 }.Contains(ModelType)) {
                validationResult.Add(new ValidationResult("ModelType取值范围为1-3"));
            }
            if (!new List<int> { 1, 2, 3, 4, 5 }.Contains(UnSettleType)) {
                validationResult.Add(new ValidationResult("ModelType取值范围为1-5"));

            }
            if (ModelType == 2) {
                if (!new List<int> { 3, 4, 5 }.Contains(UnSettleType)) {
                    validationResult.Add(new ValidationResult("ModelType=2时，UnSettleType取值范围为3-5"));
                }
            }
            if (ModelType == 3) {
                if (!new List<int> {1, 5 }.Contains(UnSettleType))
                {
                    validationResult.Add(new ValidationResult("ModelType=3时，UnSettleType取值范围为1或者5"));
                }
            }
            return validationResult;
        }
    }

}
