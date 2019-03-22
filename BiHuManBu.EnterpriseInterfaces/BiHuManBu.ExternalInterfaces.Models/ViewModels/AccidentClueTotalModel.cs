using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
  public class AccidentClueTotalModel
    {
        /// <summary>
        /// 未处理总数
        /// </summary>
        public int UntreatedTotalCount { get; set; }
        /// <summary>
        /// 跟进中
        /// </summary>
        public int FollowUpTotalCount { get; set; }
        /// <summary>
        /// 车辆到店
        /// </summary>
        public int VehicleToStoreTotalCount { get; set; }
        /// <summary>
        /// 流失
        /// </summary>
        public int LossTotalCount { get; set; }

        public int TotalCount { get; set; }

        /// <summary>
        /// 上门接车
        /// </summary>
        public int PickUpTotalCount { get; set; }

        /// <summary>
        /// 已发短信
        /// </summary>
        public int SMSTotalCount { get; set; }

        /// <summary>
        /// 已打电话
        /// </summary>
        public int CallTotalCount { get; set; }

        /// <summary>
        /// 跟进中总数量(包含状态为1,2,5,6的数据)
        /// </summary>
        public int SumFollowUpTotalCount { get; set; }

        /// <summary>
        /// 店内接待数量
        /// </summary>
        public int StoreReceivedCount { get; set; }

        /// <summary>
        /// 已定损数量
        /// </summary>
        public int MaintainCount { get; set; }

        /// <summary>
        /// 已交车数量
        /// </summary>
        public int HandOverCount { get; set; }
    }
}
