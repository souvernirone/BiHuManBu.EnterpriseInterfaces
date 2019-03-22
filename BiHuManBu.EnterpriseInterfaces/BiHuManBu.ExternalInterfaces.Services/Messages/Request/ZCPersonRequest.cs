using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class ZCPersonRequest
    {
        /// <summary>
        /// bx_group_authen的id:修改时需要传值
        /// </summary>
        public int Id { get; set; }
        public int AgentId { get; set; }
        /// <summary>
        /// CardHolder:真实姓名
        /// </summary>
        public string CardHolder { get; set; }
        /// <summary>
        /// CardId:身份证号
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// CardFace:身份证正面照片地址
        /// </summary>
        public string CardFace { get; set; }
        /// <summary>
        /// CardReverse:身份证反面照片地址
        /// </summary>
        public string CardReverse { get; set; }
        /// <summary>
        /// BankId:银行id
        /// </summary>
        public string BankId { get; set; }
        /// <summary>
        /// BankCardNo:银行卡号
        /// </summary>
        public string BankCardNo { get; set; }
        /// <summary>
        /// BankCardFront:银行卡正面照片地址
        /// </summary>
        public string BankCardFront { get; set; }
        /// <summary>
        ///  BankCardBack:银行卡反面照片地址
        /// </summary>
        public string BankCardBack { get; set; }
        /// <summary>
        /// 2018-09-20 张克亮 小V盟项目加入
        /// 认证状态 0未认证、1认证不通过、2认证通过
        /// </summary>
        public int AuthenState { get; set; }
    }
}
