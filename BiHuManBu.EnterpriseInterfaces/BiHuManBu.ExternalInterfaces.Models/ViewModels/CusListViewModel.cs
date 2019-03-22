using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CusListViewModel:BaseViewModel
    {
        /// <summary>
        /// 客户列表
        /// </summary>
        public IList<CustomerModel> CustomerList { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 待审核账号的数量
        /// </summary>
        public int NoAuditCount { get; set; }
    }

    public class CustomerModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string AgentName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string OpenId
        {
            get;
            set;
        }
        /// <summary>
        /// 分享码
        /// </summary>
        public string ShareCode
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 是否月光宝盒经纪人,0否，1是
        /// </summary>
        public int IsBigAgent
        {
            get;
            set;
        }
        /// <summary>
        /// 1-内部，0-外部  
        /// </summary>
        public int FlagId
        {
            get;
            set;
        }
        /// <summary>
        /// 父级经纪人
        /// </summary>
        public int ParentAgent
        {
            get;
            set;
        }
        /// <summary>
        /// 父级经纪人可以获取的回扣点数
        /// </summary>
        public double ParentRate
        {
            get;
            set;
        }
        /// <summary>
        /// 经纪人可以获取的回扣点数
        /// </summary>
        public double AgentRate
        {
            get;
            set;
        }
        /// <summary>
        /// insideSales获取的回扣点数
        /// </summary>
        public double ReviewRate
        {
            get;
            set;
        }
        /// <summary>
        /// 0按百分比返佣1按金额返佣
        /// </summary>
        public int PayType
        {
            get;
            set;
        }
        /// <summary>
        /// 经纪人获得金额
        /// </summary>
        public decimal AgentGetPay
        {
            get;
            set;
        }
        /// <summary>
        /// 返佣类型0百分比返佣,1金额返佣
        /// </summary>
        public int CommissionType
        {
            get;
            set;
        }
        /// <summary>
        /// 父级经纪人分享码
        /// </summary>
        public string ParentShareCode { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public int IsUsed { get; set; }
        /// <summary>
        /// 上级经纪人名称
        /// </summary>
        public string ParentAgentName { get; set; }
        /// <summary>
        /// 快保账号
        /// </summary>
        public string AgentAccount { get; set; }
        /// <summary>
        /// 是否可编辑 0 可编辑 1 不可编辑
        /// </summary>
        public int IsEdit { get; set; }
        /// <summary>
        /// 是否可以在微信端展示,0是,1否
        /// </summary>
        public int IsShow { get; set; }
        /// <summary>
        /// 是否展示微信端计算器
        /// </summary>
        public int IsShowCalc { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }
        //经纪人微信昵称
        public string NickName { get; set; }
        /// <summary>
        /// 经纪人类型0普通经纪人、1服务顾问、2续保顾问
        /// </summary>
        public int AgentType { get; set; }
        /// <summary>
        /// 0从顶级业务员扣费(短信)，1业务员本身扣费（短信）
        /// </summary>
        public int MessagePayType { get; set; }

        public int ManagerRoleId { get; set; }

        public int IsDaiLi { get; set; }
        public int IsSubmit { get; set; }

        /// <summary>
        /// 是否有密码
        /// </summary>
        public int IsHasPwd { get; set; }

        /// <summary>
        /// 顶级代理人id
        /// </summary>
        public int TopAgentId { get; set; }

        /// <summary>
        /// 代理人级别 1：顶级  2：二级  3：三级
        /// </summary>
        public int Agent_level { get; set; }

        /// <summary>
        /// 可用平台
        /// 可以使用的平台 1:PC 2:微信 4:APP，这里存放最后相加的结果，比如：同时选择了微信（2）和APP（4），这里就存6
        /// </summary>
        public int Platform { get; set; }
        public string RegTime { get; set; }
        /// <summary>
        /// 振邦账号类型 1机构、2网点、3内部员工、4外部代理
        /// </summary>
        public int ZhenBangType { get; set; }
        /// <summary>
        /// 认证状态 0未认证 1已认证
        /// </summary>
        public int AuthenState { get; set; }
        /// <summary>
        /// 认证id
        /// </summary>
        public int AuthenId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ChargePerson { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        public int TestState { get; set; }


        public int department_id { get; set; }

        public string department_name { get; set; }


        public int IsGrabOrder { get; set; }

    }
}
