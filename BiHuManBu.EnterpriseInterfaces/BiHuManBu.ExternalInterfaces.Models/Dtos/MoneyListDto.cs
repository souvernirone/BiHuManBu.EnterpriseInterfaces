using System;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 获取顶级代理人的累计佣金和积分总计返回模型
    /// </summary>
    public class TopAgentStatistics
    {
        private double? totalMoney;
        /// <summary>
        /// 累计佣金合计
        /// </summary>
        public double? TotalMoney
        {
            get { return totalMoney ?? 0; }
            set { totalMoney = value; }
        }
        private double? totalOneselfCredit;
        /// <summary>
        /// 累计个人积分合计
        /// </summary>
        public double? TotalOneselfCredit { get { return totalOneselfCredit ?? 0; } set { totalOneselfCredit = value; } }
        private double? totalTeamCredit;
        /// <summary>
        /// 累计团队积分合计
        /// </summary>
        public double? TotalTeamCredit
        {
            get
            {
                return totalTeamCredit ?? 0;
            }
            set
            {
                totalTeamCredit = value;
            }
        }
    }

    /// <summary>
    /// 当前代理人统计模型
    /// </summary>
    public class CurAgentStatisticsVM
    {
        /// <summary>
        /// 累计收益
        /// </summary>
        public double TotalMoney { get; set; }
        /// <summary>
        /// 当月收益
        /// </summary>
        public double MonthTotalMoney { get; set; }
        /// <summary>
        /// 账户余额
        /// </summary>
        public double AccountBalance { get; set; }
    }

    public class CurAgentStatistics
    {
        private double? totalMoney;
        /// <summary>
        /// 累计佣金收益
        /// </summary>
        public double? TotalMoney { get { return totalMoney; } set { totalMoney = value ?? 0; } }

        private double? monthTotalMoney;
        /// <summary>
        /// 当月收益（佣金+积分）
        /// </summary>
        public double? MonthTotalMoney { get { return monthTotalMoney; } set { monthTotalMoney = value ?? 0; } }
        private double? withdraw;
        /// <summary>
        /// 累计团队收益
        /// </summary>
        public double? Withdraw { get { return withdraw; } set { withdraw = value ?? 0; } }
    }

    public class OrderCommissionBillInfo
    {
        /// <summary>
        /// 佣金类型，1:个人收益 2:资金提现 3:团队收益
        /// </summary>
        private static string[] arrBillType = { "", "个人收益", "资金提现", "团队收益" };

        /// <summary>
        /// 保单类型，1:交强 2:商业
        /// </summary>
        private static string[] arrPolicyType = { "", "交强险", "商业险" };

        public int id { get; set; }
        public double money { get; set; }
        public double credit { get; set; }
        public string license_no { get; set; }
        public string policy_no { get; set; }
        /// <summary>
        /// 保单类型-库里面存的值
        /// </summary>
        public int policy_type { get; set; }

        /// <summary>
        /// 保单类型-对应的文字
        /// </summary>
        public string PolicyType
        {
            get
            {
                if (policy_type >= 0 && policy_type <= arrPolicyType.Length)
                {
                    return arrPolicyType[policy_type];
                }
                else
                {
                    return "";
                }
            }
        }

        public sbyte commission_type { get; set; }
        public System.DateTime create_time { get; set; }

        public string BillType
        {
            get
            {
                if (commission_type > arrBillType.Length)
                    return "";
                else
                {
                    return arrBillType[commission_type];
                }
            }
        }
    }

    public class OrderCommissionSearchParam
    {
        public int AgentId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        private sbyte status = 1;

        public sbyte Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
