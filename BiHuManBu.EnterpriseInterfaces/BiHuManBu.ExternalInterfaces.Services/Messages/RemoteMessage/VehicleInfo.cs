using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.RemoteMessage
{
    public class VehicleInfo
    {
        /// <summary>
        /// 排气量
        /// </summary>
        public string ExhaustScale { get; set; }
        /// <summary>
        /// 购置价格
        /// </summary>
        public string PurchasePrice { get; set; }
        /// <summary>
        /// 动力类型
        /// </summary>
        public string VehicleAlias { get; set; }
        /// <summary>
        /// 年款
        /// </summary>
        public string VehicleYear { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public string SeatCount { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string VehicieName { get; set; }
    }
}
