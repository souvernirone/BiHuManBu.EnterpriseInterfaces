using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CreateSettlementVM /*: IValidatableObject*/
    {
        /// <summary>
        /// 待结算单编号集合
        /// </summary>
        public List<int> Ids { get; set; }
        /// <summary>
        /// 结算类型：1->代理人佣金结算、2->网点佣金结算、3->机构佣金结算、4->机构毛利结算、5->保司手续费结算
        /// </summary>

        public int SettleType { get; set; }
        /// <summary>
        /// 待结算单搜索条件模型
        /// </summary>
        public UnSettlementListSearchRequestVM SearchWhere { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var validationResult = new List<ValidationResult>();
        //    if (!new List<int> { 1, 2, 3, 4, 5 }.Contains(SettleType))
        //    {
        //        validationResult.Add(new ValidationResult("SettleType取值范围为1-5"));
        //    }
        //    if (!Ids.Any())
        //    {
        //        if (SearchWhere == null)
        //        {
        //            validationResult.Add(new ValidationResult("Ids不填时，SearchWhere不能为null"));
        //        }
        //        else
        //        {
        //             validationResult .AddRange(SearchWhere.Validate(validationContext).ToList());
        //        }
        //    }
        //    return validationResult;

        //}
    }
}
