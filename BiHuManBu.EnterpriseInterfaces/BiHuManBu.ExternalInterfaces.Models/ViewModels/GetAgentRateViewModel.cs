using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetAgentRateViewModel : BaseViewModel
    {
        public IList<BxAgentSpecialRate> RateList { get; set; }
        public BxAgentRate AgentRate { get; set; }
    }

    public class BxAgentSpecialRate
    {
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int id
        {
            get;
            set;
        }
        /// <summary>
        /// 系统返回费率
        /// </summary>
        public Double system_rate
        {
            get;
            set;
        }
        /// <summary>
        /// 补点
        /// </summary>
        public Double budian_rate
        {
            get;
            set;
        }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? create_people_id
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string create_people_name
        {
            get;
            set;
        }
        /// <summary>
        /// 保险公司
        /// </summary>
        public int? company_id
        {
            get;
            set;
        }

        public int? agent_id { get; set; }

        public double zhike_rate { get; set; }

        public double agent_default_kd_rate { get; set; }

        #endregion Model
    }

    public class BxAgentRate
    {
        #region Model

        public int id { get; set; }

        /// <summary>
        /// 经纪人Id
        /// </summary>
        public int agnet_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int rate_one { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double rate_two { get; set; }

        /// <summary>
        ///  交强险;
        /// </summary>
        public int rate_three { get; set; }

        /// <summary>
        /// 车船税
        /// </summary>
        public int rate_four { get; set; }

        /// <summary>
        /// 父级经纪人ID
        /// </summary>
        public int? agent_parent_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int? create_people_id { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string create_people_name { get; set; }


        /// <summary>
        /// 公司类型
        /// </summary>

        public int company_id { get; set; }

        /// <summary>
        /// 固定补点
        /// </summary>
        public double fixed_rate { get; set; }

        /// <summary>
        /// 固定返佣
        /// </summary>
        public double fixed_discount { get; set; }

        /// <summary>
        /// 发票系统税点
        /// </summary>
        public double fapiao_system_rate { get; set; }

        /// <summary>
        /// 发票补点税点
        /// </summary>
        public double fapiao_rate { get; set; }

        /// <summary>
        ///  0固定补点，1固定返佣   
        /// </summary>
        public int rate_type_gd { get; set; }

        /// <summary>
        /// 给经纪人的返点
        /// </summary>
        public double agent_rate { get; set; }

        public double zhike_koudian_rate { get; set; }
        public double zhike_budian_rate { get; set; }

        public int qudao_id { get; set; }
        /// <summary>
        /// 业务员默认扣点（固定补点）
        /// </summary>
        public double agent_default_bd_rate { get; set; }
        /// <summary>
        /// 业务员默认扣点（固定返佣）
        /// </summary>
        public double agent_default_kd_rate { get; set; }

        #endregion Model
    }
}
