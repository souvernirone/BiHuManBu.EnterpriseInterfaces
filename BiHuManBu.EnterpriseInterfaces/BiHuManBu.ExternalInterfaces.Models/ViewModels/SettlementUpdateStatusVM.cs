using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SettlementUpdateStatusVM
        //: IValidatableObject
    {
        /// <summary>
        /// 结算单编号集合
        /// </summary>
        public List<int> Ids { get; set; }
        /// <summary>
        /// 更新状态类型：1->标记为结算、2->标记为已对账、3->标记为已开票、4->标记为已回款
        /// </summary>
        public int UpdateStatusType { get; set; }
        /// <summary>
        ///结算类型：1->代理人结算、2->网点结算、3->机构佣金结算、4->机构毛利结算
        /// </summary>

        public int SettleType { get; set; }
        /// <summary>
        /// 模式类型：1->模式一、2->模式二、3->模式三
        /// </summary>

        public int ModelType { get; set; }
        
        /// <summary>
        /// 结算单查询条件模型
        /// </summary>

        public SettlementListSearchRequestVM SearchWhere { get; set; }
        /// <summary>
        /// 结算信息
        /// </summary>
        public SettlementStatus SettlementStatus { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        public ReceiptStatus ReceiptStatus { get; set; }
        /// <summary>
        /// 回款信息
        /// </summary>
        public BackPriceStatus BackPriceStatus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResult = new List<ValidationResult>();
            if (!new List<int> { 1, 2, 3, 4 }.Contains(UpdateStatusType))
            {
                validationResult.Add(new ValidationResult("UpdateStatusType取值范围为1-4"));
            }
            if (!new List<int> { 1, 2, 3 }.Contains(ModelType))
            {
                validationResult.Add(new ValidationResult("ModelType取值范围为1-3"));
            }
            if (!new List<int> { 1, 2, 3, 4 }.Contains(UpdateStatusType))
            {
                validationResult.Add(new ValidationResult("UpdateStatusType取值范围为1-4"));

            }

            if (UpdateStatusType == 1)
            {

                if (!new List<int> { 1, 2, 3, 4 }.Contains(SettleType))
                {
                    validationResult.Add(new ValidationResult("UpdateStatusType=1时SettleType取值范围为1-4"));
                }
                if (SettlementStatus == null)
                {
                    validationResult.Add(new ValidationResult("UpdateStatusType=1时SettlementStatus不能为空"));
                }
                else
                {
                    validationResult.AddRange(SettlementStatus.Validate(validationContext).ToList());
                }

            }

            if (!Ids.Any())
            {
                if (SearchWhere == null)
                {
                    validationResult.Add(new ValidationResult("Ids为空时，SearchWhere不能为空"));

                }
                else {
                    validationResult.AddRange(SearchWhere.Validate(validationContext));
                }
            }
            if (UpdateStatusType == 3)
            {
                if (ReceiptStatus == null)
                {
                    validationResult.Add(new ValidationResult("UpdateStatusType=1时ReceiptStatus不能为空"));
                }
                else
                {
                    validationResult.AddRange(ReceiptStatus.Validate(validationContext).ToList());
                }
            }

            if (UpdateStatusType == 4)
            {
                if (BackPriceStatus == null)
                {
                    validationResult.Add(new ValidationResult("UpdateStatusType=1时BackPriceStatus不能为空"));
                }
                else
                {
                    validationResult.AddRange(BackPriceStatus.Validate(validationContext).ToList());
                }
            }

            return validationResult;

        }
    }
}
public class SettlementStatus : IValidatableObject
{
    /// <summary>
    /// 结算日期
    /// </summary>
    public DateTime? SettleTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        if (SettleTime == null)
        {
            validationResult.Add(new ValidationResult("SettleTime不能为空"));

        }
        return validationResult;
    }
}

public class ReceiptStatus : IValidatableObject
{
    /// <summary>
    /// 发票抬头
    /// </summary>
    public string ReceiptTitle { get; set; }
    /// <summary>
    /// 发票编号
    /// </summary>
    public string ReceiptNum { get; set; }
    /// <summary>
    /// 发票金额
    /// </summary>
    public double ReceiptPrice { get; set; }
    /// <summary>
    /// 发票日期
    /// </summary>

    public DateTime ReceiptDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(ReceiptTitle))
        {
            validationResult.Add(new ValidationResult("ReceiptTitle不能为空"));

        }

        if (string.IsNullOrWhiteSpace(ReceiptNum))
        {
            validationResult.Add(new ValidationResult("ReceiptNum不能为空"));

        }

        if (ReceiptPrice <= 0)
        {
            validationResult.Add(new ValidationResult("ReceiptPrice为非负数"));

        }
        return validationResult;
    }
}

public class BackPriceStatus : IValidatableObject
{
    /// <summary>
    /// 付款方
    /// </summary>
    public string Payer { get; set; }
    /// <summary>
    /// 收款金额
    /// </summary>
    public double BackPrice { get; set; }
    /// <summary>
    /// 收款日期
    /// </summary>

    public DateTime BackPriceDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(Payer))
        {
            validationResult.Add(new ValidationResult("Payer不能为空"));

        }

        if (BackPrice <= 0)
        {
            validationResult.Add(new ValidationResult("BackPrice为非负数"));

        }
        return validationResult;
    }
}

