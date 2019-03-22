using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class BatchRefreshRenewalModel
    {
        public long buid { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string licenseno { get; set; }
        /// <summary>
        /// 1:刷新成功3:只取到行驶本4:刷新失败5:续保中6:排队中
        /// 状态2暂时废弃
        /// </summary>
        public int refrenewalstatus { get; set; }
    }
}
