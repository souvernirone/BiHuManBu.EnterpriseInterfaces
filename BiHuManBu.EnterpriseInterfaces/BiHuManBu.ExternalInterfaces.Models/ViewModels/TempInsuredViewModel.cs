using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class TempInsuredViewModel : IValidatableObject
    {
        /// <summary>
        /// bx_TempInsuredInfo.Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// bx_agent.Id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// 删除标记
        /// </summary>
        private bool _deleted = false;

        public bool Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }
        public long BuId { get; set; }
        /// <summary>
        /// 标签类型：0：去年投保，1：临时被保险人，2：正常值
        /// </summary>
        public int TagType { get; set; }
        /// <summary>
        /// 被保险人详细信息
        /// </summary>
        public TempInsuredDetailInfo DetailInfo { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(LicenseNo))
            {
                validationResult.Add(new ValidationResult("LicenseNo不能为空"));
            }
            else if (LicenseNo.Length > 20)
            {
                validationResult.Add(new ValidationResult("LicenseNo最大长度为20"));
            }

            if (string.IsNullOrWhiteSpace(DetailInfo.InsuredName))
            {
                validationResult.Add(new ValidationResult("InsuredName不能为空"));
            }
            else if (DetailInfo.InsuredName.Length > 50)
            {
                validationResult.Add(new ValidationResult("InsuredName最大长度为50"));
            }


            if (string.IsNullOrWhiteSpace(DetailInfo.InsuredMobile))
            {
                validationResult.Add(new ValidationResult("InsuredMobile不能为空"));
            }
            else if (DetailInfo.InsuredMobile.Length > 20)
            {
                validationResult.Add(new ValidationResult("InsuredMobile最大长度为20"));
            }
            if (string.IsNullOrWhiteSpace(DetailInfo.InsuredIdCard))
            {
                validationResult.Add(new ValidationResult("InsuredIdCard不能为空"));
            }
            else if (DetailInfo.InsuredIdCard.Length > 100)
            {
                validationResult.Add(new ValidationResult("InsuredIdCard最大长度为100"));
            }
            if (BuId <= 0) {
                validationResult.Add(new ValidationResult("BuId异常"));
            }

            if (string.IsNullOrWhiteSpace(DetailInfo.Email))
            {
                validationResult.Add(new ValidationResult("Email不能为空"));
            }
            else if (DetailInfo.Email.Length > 50)
            {
                validationResult.Add(new ValidationResult("Email最大长度为50"));
            }
            else if (!new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").IsMatch(DetailInfo.Email))
            {
                validationResult.Add(new ValidationResult("请输入正确的邮箱"));
            }
            return validationResult;
        }
    }
    public class TempInsuredDetailInfo
    {
        /// <summary>
        /// 临时被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 临时被保险人证件类型
        /// </summary>
        public int InsuredType { get; set; }
        /// <summary>
        /// 被保险人证件号
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人电话
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人邮箱
        /// </summary>
        public string Email { get; set; }

    }
}
