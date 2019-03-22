using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// CrmTimeLine 业务的类型
    /// </summary>
    public enum EnumCrmTimeLineType
    {
      
        [Description("回访")]
        HuiFang = 1,
        [Description("短信报价")]
        DuanXingBaoJia = 2,
        [Description("预约单")]
        YuYueDan = 3,
        [Description("保单已打印")]
        BaoDanDaYin = 4,
      
    }
}
