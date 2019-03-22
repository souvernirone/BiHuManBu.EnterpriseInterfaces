using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class UpdateGroupAuthenRequest 
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 代理人Id
        /// </summary>
        [Range(1, 100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 认证状态 0未认证、1已认证
        /// </summary>
        [Range(0, 1)]
        public int AuthenState { get; set; }
        /// <summary>
        /// 持卡人
        /// </summary>
        [StringLength(10, ErrorMessage = "持卡人限制输入10位字符")]
        public string CardHolder { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(18, ErrorMessage = "身份证号限制输入18位字符")]
        public string CardId { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        [StringLength(25, ErrorMessage = "银行卡号限制输入25位字符")]
        public string BankCardNum { get; set; }
        /// <summary>
        /// 营业执照路径
        /// </summary>
        public string BusinessLicenceUrl { get; set; }
        /// <summary>
        /// 身份证照正面路径
        /// </summary>
        public string CardFaceUrl { get; set; }
        /// <summary>
        /// 身份证照反面路径
        /// </summary>
        public string CardReverseUrl { get; set; }
        /// <summary>
        /// 银行卡正面照
        /// </summary>
        public string BankcardFaceUrl { get; set; }
        /// <summary>
        /// 银行卡反面照
        /// </summary>
        public string BankcardReverseUrl { get; set; }
        /// <summary>
        /// 场地照
        /// </summary>
        public string FieldUrl { get; set; }
        /// <summary>
        /// 单位地址
        /// </summary>
        [StringLength(30, ErrorMessage = "机构地址长度限制输入30位字符")]
        public string AgentAddress { get; set; }
        /// <summary>
        /// 银行id
        /// </summary>
        public int BankId { get; set; }

        //2018-09-18 张克亮
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadPortrait { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
    }

    public class AuditFailedRequest
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 认证信息id
        /// </summary>
        public int AuthenId { get; set; }
        /// <summary>
        /// 认证不通过备注
        /// </summary>
        //[StringLength(50,ErrorMessage ="认证备注不能超过50个字符")]
        public string AuditRemark { get; set; }
        /// <summary>
        /// 审核人id
        /// </summary>
        public int AuditUserId { get; set; }
        /// <summary>
        /// 持卡人姓名（真实姓名）
        /// </summary>
        public string CardHolder { get; set; }
    }
}
