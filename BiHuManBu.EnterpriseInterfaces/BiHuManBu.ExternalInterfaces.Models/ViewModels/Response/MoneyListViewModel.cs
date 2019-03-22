using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class MoneyListViewModel
    {
        public int Count { get; set; }

        public List<MoneyListInfo> list = new List<MoneyListInfo>();
        /// <summary>
        /// 列表
        /// </summary>
        public List<MoneyListInfo> List
        {
            get { return list; }
            set
            {
                if (value != null)
                    list = value;
            }
        }
    }

    public class MoneyListInfo
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }

        private double? totalMoney;
        /// <summary>
        /// 累计佣金
        /// </summary>
        public double? TotalMoney
        {
            get { return totalMoney; }
            set
            {
                totalMoney = value ?? 0;
            }
        }

        private double? totalCredit;
        /// <summary>
        /// 累计积分
        /// </summary>
        public double? TotalCredit
        {
            get
            {
                return totalCredit;
            }
            set
            {
                totalCredit = value ?? 0;
            }
        }

        private double? totalTeamMoney;
        /// <summary>
        /// 累计团队收益
        /// </summary>
        public double? TotalTeamMoney
        {
            get { return totalTeamMoney; }
            set
            {
                totalTeamMoney = value ?? 0;
            }
        }

        /// <summary>
        /// 现金余额
        /// </summary>
        public double? CashBalance { get { return TotalMoney + Withdraw; } }

        /// <summary>
        /// 积分余额
        /// </summary>
        public double? CreditBalance { get { return TotalCredit; } }

        private double? withdraw;
        /// <summary>
        /// 提现
        /// </summary>
        public double? Withdraw
        {
            get { return withdraw; }

            set
            {
                withdraw = value ?? 0;
            }
        }
    }

    public class BillInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 明细类型
        /// </summary>
        public string BillType { get; set; }

        public string LicenseNo { get; set; }

        /// <summary>
        /// 保单号
        /// </summary>
        public string PolicyNo { get; set; }

        /// <summary>
        /// 保单类型
        /// </summary>
        public string PolicyType { get; set; }

        public double Money { get; set; }

        public double Credit { get; set; }

        public string CreateTime { get; set; }

        public string DisplayDay { get; set; }
    }

    public class MonthAndTotalMoney
    {
        /// <summary>
        /// 年月
        /// </summary>
        public int YearMonth { get; set; }
        /// <summary>
        /// 每月总收益
        /// </summary>
        public double TotalMoney { get; set; }
    }

    public class SonAndGrandSonIncome
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 代理人数量
        /// </summary>
        public int AgentCount { get; set; }
        /// <summary>
        /// 预计收益
        /// </summary>
        public double TotalMoney { get; set; }
        /// <summary>
        /// 总净保费
        /// </summary>
        public double TotalPreminu { get; set; }
    }

    public class TeamIncomeModel
    {
        /// <summary>
        /// 团队级别
        /// </summary>
        public int TeamLevel { get; set; }
        /// <summary>
        /// 总净保费
        /// </summary>
        public double TotalPremium { get; set; }
        /// <summary>
        /// 预计收益
        /// </summary>
        public double TotalMoney { get; set; }
    }
}
