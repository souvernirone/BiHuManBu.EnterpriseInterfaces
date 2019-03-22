using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;


namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
  public  class CreateJdOrderRequest:BaseRequest,IValidatableObject
    {
     
        /// <summary>
        /// 流水号
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string TradeNum { get; set; }
        /// <summary>
        /// 金额（APP获取订单支付状态时不传）
        /// </summary>
       
        public decimal Amount { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [Range(1, 2100000000)]
        public long OrderId { get; set; }
        public string BhToken { get; set; }
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
       
             if(Amount * 100<1.00m) {
                validationResult.Add(new ValidationResult("金额最少为1分"));
            }
    
            return validationResult;
        }
    }
}
