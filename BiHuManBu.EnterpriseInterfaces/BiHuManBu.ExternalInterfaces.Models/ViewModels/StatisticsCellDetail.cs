using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class StatisticsCellDetail
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string BizEndDate { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string  CustomerName { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public string CustomerStatusName{ get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
