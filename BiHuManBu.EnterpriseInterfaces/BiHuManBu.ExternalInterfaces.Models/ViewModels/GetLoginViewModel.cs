using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetLoginViewModel : BaseViewModel
    {
        /// <summary>
        /// 手机是否与微信同号
        /// </summary>
        public int PhoneIsWechat { get; set; }
        public int IsHasChildAgents { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int agentId { get; set; }
        public string agentName { get; set; }
        public string agentMobile { get; set; }
        public string mobile { get; set; }
        public int? agentType { get; set; }

        public int? isShow { get; set; }

        public int? IsShowCalc { get; set; }
        public int managerRoleId { get; set; }

        public string secretKey { get; set; }

        public int? isDaiLi { get; set; }
        public int? roleType { get; set; }

        /// <summary>
        /// 是否完成团队任务1是0否
        /// </summary>
        public int? IsCompleteTask { get; set; }
        /// <summary>
        /// 认证状态 0未认证、1认证不通过、2认证通过,
        /// </summary>
        public int? AuthenState { get; set; }
        /// <summary>
        /// 考试状态：默认-1，传1和0分别表示通过和不通过
        /// </summary>
        public int? TestState { get; set; }

        public int? topAgentId { get; set; }
        public string topAgentName { get; set; }

        public string parentAgentName { get; set; }
        public string parentAgentMobile { get; set; }
        public int parentAgentId { get; set; }

        public int? messagePayType { get; set; }
        public int? isUsed { get; set; }
        public long? avail_times { get; set; }
        public int lastForceDays { get; set; }

        /// <summary>
        /// 商业险到期天数
        /// </summary>
        public int BizDaysNum { get; set; }

        public ManagerRole roleInfo { get; set; }
        public List<ManagerModule> module { get; set; }

        //public List<manager_module_db> BaseModules { get; set; }
        public List<ParentAgent> parentAgent { get; set; }

        private int _isDistribute = 1;
        public Nullable<int> desensitization { get; set; }

        public int isDistribute
        {
            get { return _isDistribute; }
            set { _isDistribute = value; }
        }

        public int? regType { get; set; }
        public string parentSecretKey { get; set; }

        public string token { get; set; }
        public string OpenId { get; set; }

        public List<int?> CityList { get; set; }

        /// <summary>
        /// 是否有摄像头1有0没有
        /// </summary>
        public int IsDisplayCamera { get; set; }

        /// <summary>
        /// 顶级代理人类型：0:车商 1:修理厂 2:专业代理 3:互联网公司 4:其他
        /// </summary>
        public int TopAgentType { get; set; }
        /// <summary>
        /// 是否允许业务员之间重复报价 0不允许 1允许
        /// </summary>
        public int RepeatQuote { get; set; }
        /// <summary>
        /// 是否开通新车报价 0未开通 1开通
        /// </summary>
        public int OpenQuote { get; set; }
        /// <summary>
        /// 顶级代理人手机号
        /// </summary>
        public string TopAgentMobile { get; set; }
        /// <summary>
        /// Ukey的Mac地址
        /// </summary>
        public string MacUrl { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        public ButtonModel Buttons { get; set; }

        public Dictionary<string, bool> ButtonState { get; set; }
        /// <summary>
        /// 是否显示未分配标签
        /// </summary>
        public bool ShowNoDistributedLabel { get; set; }

        public int HidePhone { get; set; }
        public int IsSubmit { get; set; }
        public int HasCamera { get; set; }
        public int ZhenBangType { get; set; }
        /// <summary>
        /// 结算方式 （0：单店结算 1:只给机构结算 2:机构、代理人、网点结算)
        /// </summary>
        public int SettlementType { get; set; }
        /// <summary>
        /// 模式类型（结算）
        /// </summary>
        public int ModelType { get; set; }


        public Nullable<int> ServiceType { get; set; }

        public bool HasZhuDianYuanRole { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }

        public int OpenTuiXiu { get; set; }

        /// <summary>
        /// 头像 2018-09-19 张克亮
        /// </summary>
        public string HeadPortrait { get; set; }


        /// <summary>
        ///  平台推送 签约状态  0：未签约   1：已签约
        /// </summary>
        public int SignedState { get; set; }

        public int GroupId { get; set; }
    }

    public class ManagerRole
    {
        public int id { get; set; }
        public string role_name { get; set; }
        public Nullable<double> role_status { get; set; }
        public Nullable<int> top_agent_id { get; set; }
        public Nullable<int> role_type { get; set; }
        public string creator_name { get; set; }
        public DateTime? createor_time { get; set; }
        public Nullable<int> isRequote { get; set; }
    }

    public class ManagerModule
    {
        public string module_code { get; set; }
        public string module_name { get; set; }
        public string pater_code { get; set; }
        public Nullable<double> module_level { get; set; }
        public Nullable<double> is_menu { get; set; }
        public Nullable<double> is_action { get; set; }
        public string action_url { get; set; }
        public Nullable<double> module_status { get; set; }
        public List<ManagerModule> child { get; set; }
        public decimal? orderBy { get; set; }

        public int? crm_module_type { get; set; }

    }
    public class ParentAgent
    {
        public int id { get; set; }
        public string agentName { get; set; }
        public string shareCode { get; set; }
        public string secretKey { get; set; }
        public int parentAgent { get; set; }
        public int? isUsed { get; set; }
        public int? isDaiLi { get; set; }
        public int ManagerRoleId { get; set; }
    }

    public class ButtonModel
    {
        public bool btn_recycle { get; set; }
        public bool btn_delete { get; set; }
        public bool btn_export { get; set; }
        public bool btn_review { get; set; }
    }
}
