using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
   public class ConsumerDetailCrmListResponseStep
    {
        public long id { get; set; }
        public object json_content { get; set; }
        public int agent_id { get; set; }
        public string create_time { get; set; }

        
        /// <summary>
        /// 1回访，2短信报价，3预约单，4保单，5 bx_userinfo批量删除，6摄像头进店，7分配，9保存短信信息，10转移，11分享报价单，12创建订单,13批量修改客户状态和类别
        /// </summary>
        public int type { get; set; }
        public long b_uid { get; set; }
        public string stime { get; set; }
    }
}
