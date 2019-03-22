using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends
{
    public class TeamIncomeViewModel : BaseViewModel
    {
        /// <summary>
        /// 是否有团队
        /// </summary>
        public int IsCompleteTask { get; set; }

        /// <summary>
        /// 二级团队收益
        /// </summary>
        public double SonIncome { get; set; }

        /// <summary>
        /// 三级团队收益
        /// </summary>
        public double GrandSonIncome { get; set; }

        /// <summary>
        /// 二级团队人数
        /// </summary>
        public int SonCount { get; set; }

        /// <summary>
        /// 三级级团队人数
        /// </summary>
        public int GrandSonCount { get; set; }

        /// <summary>
        /// 团队当前等级
        /// </summary>
        public int NowGrade { get; set; }

        /// <summary>
        /// 预计收益
        /// </summary>
        public double ExpectIncome { get; set; }

    }
}
