using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GroupAuthenViewModel : BaseViewModel
    {
        public GroupAuthenModel GroupAuthen { get; set; }
    }

    public class GroupAuthenModel
    {
        #region bx_group_authen表
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AuthenState { get; set; }
        public string CardHolder { get; set; }
        public string CardId { get; set; }
        public string BankCardNum { get; set; }
        public string BankName { get; set; }
        public string ChildBankName { get; set; }
        public string BusinessLicenceUrl { get; set; }
        public string CardFaceUrl { get; set; }
        public string CardReverseUrl { get; set; }
        public string FieldUrl { get; set; }
        public string BankcardFaceUrl { get; set; }
        public string BankcardReverseUrl { get; set; }
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
        /// <summary>
        /// 成团状态 0未成团 1已经成团
        /// </summary>
        public int IsCompleteTask { get; set; }
        #endregion

        #region bx_agent表
        /// <summary>
        /// 单位名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 单位地址
        /// </summary>
        public string AgentAddress { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ChargePeson { get; set; }
        #endregion
    }
}
