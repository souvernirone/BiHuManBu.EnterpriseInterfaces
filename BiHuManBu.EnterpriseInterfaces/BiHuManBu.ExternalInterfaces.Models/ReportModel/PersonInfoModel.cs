using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class PersonInfoModel
    {
        /// <summary>
        /// bx_group_authen的id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// bx_agent_bankcard的id
        /// </summary>
        public int BankCardId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// agent名字
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 真实姓名(持卡人)
        /// </summary>
        public string Cardholder { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCardNo { get; set; }
        /// <summary>
        /// 银行id
        /// </summary>
        public int BankId { get; set; }
        /// <summary>
        /// 认证状态 -1:未认证 0:审核中 1:已认证 2:认证不通过
        /// </summary>
        public int AuthenState { get; set; }
        /// <summary>
        /// 驳回原因
        /// </summary>
        public string Dismissal { get; set; }
        /// <summary>
        /// CardFace:身份证正面照片地址
        /// </summary>
        public string CardFace { get; set; }
        /// <summary>
        /// CardReverse:身份证反面照片地址
        /// </summary>
        public string CardReverse { get; set; }
        /// <summary>
        /// BankCardFront:银行卡正面照片地址
        /// </summary>
        public string BankCardFront { get; set; }
        /// <summary>
        ///  BankCardBack:银行卡反面照片地址
        /// </summary>
        public string BankCardBack { get; set; }
    }
}
