using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UnSettlementRequestVM /*: IValidatableObject*/
    {

        /// <summary>
        /// 刷卡日期
        /// </summary>
        public DateTime SwingCardDate { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 保单号
        /// </summary>
        public string ReconciliationNum { get; set; }
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public int InsuranceType { get; set; }
        /// <summary>
        /// 保司编号
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// 保司名称
        /// </summary>

        public string CompanyName { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 渠道Id
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 保费金额
        /// </summary>

        public double InsurancePrice { get; set; }
        /// <summary>
        /// 车船税
        /// </summary>
        public  double TaxPrice { get; set; }
        /// <summary>
        /// 数据所在代理人Id
        /// </summary>

        public int DataInAgentId { get; set; }
        /// <summary>
        /// 数据所在代理人姓名
        /// </summary>
        public string DataInAgentName { get; set; }
        /// <summary>
        /// 父级代理人Id
        /// </summary>
        public int ParentAgentId { get; set; }
        /// <summary>
        /// 父级代理人名称
        /// </summary>
        public string ParentAgentName { get; set; }
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 顶级代理人名称
        /// </summary>

        public string TopAgentName { get; set; }

        /// <summary>
        /// 振邦账号类型：1->代理人、2->网点、3->机构佣金、4->机构毛利、5->保司
        /// </summary>

        public int AgentType_ZB { get; set; }


        /// <summary>
        /// 抓单时间
        /// </summary>

        public DateTime CatchSingleTime { get; set; }


        public string GuId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>

        public DateTime UpdateTime
        {
            get { return DateTime.Now; }
            set { }
        }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return DateTime.Now; }
            set { }
        }

        public double SettleRate { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    List<ValidationResult> validationResult = new List<ValidationResult>();
        //    if (string.IsNullOrWhiteSpace(Licenseno))
        //    {
        //        validationResult.Add(new ValidationResult("Licenseno为必要参数"));
        //    }
        //    else {
        //        Licenseno = Licenseno.ToUpper();
        //    }
        //    if (string.IsNullOrWhiteSpace(ReconciliationNum))
        //    {
        //        validationResult.Add(new ValidationResult("ReconciliationNum为必要参数"));
        //    }
        //    if (string.IsNullOrWhiteSpace(UserName))
        //    {
        //        validationResult.Add(new ValidationResult("UserName为必要参数"));

        //    }
        //    if (!new List<int> { 1, 2 }.Contains(InsuranceType))
        //    {
        //        validationResult.Add(new ValidationResult("InsuranceType只能为1或者2"));
        //    }

        //    if (string.IsNullOrWhiteSpace(CompanyName))
        //    {
        //        validationResult.Add(new ValidationResult("CompanyName为必要参数"));
        //    }
        //    if (ChannelId <= 0)
        //    {
        //        validationResult.Add(new ValidationResult("ChannelId为必要参数"));

        //    }
        //    if (string.IsNullOrWhiteSpace(ChannelName))
        //    {
        //        validationResult.Add(new ValidationResult("ChannelName为必要参数"));

        //    }
        //    if (Price < 0)
        //    {
        //        validationResult.Add(new ValidationResult("Price必须为非负数"));
        //    }
        //    if (InsurancePrice < 0)
        //    {
        //        validationResult.Add(new ValidationResult("InsurancePrice必须为非负数"));
        //    }
        //    if (!new List<int> { 1, 2, 3, 4, 5 }.Contains(AgentType_ZB))
        //    {
        //        validationResult.Add(new ValidationResult("AgentType_ZB取值范围为1-5"));
        //    }
        //    if (AgentType_ZB != 5)
        //    {
        //        if (DataInAgentId <= 0)
        //        {
        //            validationResult.Add(new ValidationResult("DataInAgentId为必填参数"));
        //        }
        //        if (string.IsNullOrWhiteSpace(DataInAgentName))
        //        {
        //            validationResult.Add(new ValidationResult("DataInAgentName为必填参数"));
        //        }
        //        if (ParentAgentId <= 0)
        //        {
        //            validationResult.Add(new ValidationResult("ParentAgentId为必填参数"));
        //        }
        //        if (string.IsNullOrWhiteSpace(ParentAgentName))
        //        {
        //            validationResult.Add(new ValidationResult("ParentAgentName为必填参数"));

        //        }


        //    }
        //    if (string.IsNullOrWhiteSpace(GuId))
        //    {
        //        validationResult.Add(new ValidationResult("GuId为必填参数"));

        //    }
        //    return validationResult;

        //}


    }
}
