using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
   public class ConsumerReviewToJson
    {
      
            /// <summary>
            /// bx_agent.id
            /// </summary>
            public int AgentId { get; set; }
            /// <summary>
            /// bx_userinfo.Id
            /// </summary>
            public int BuId { get; set; }
            /// <summary>
            /// 回访状态
            /// </summary>
            public int ReviewStatus { get; set; }
            /// <summary>
            /// 回访时间
            /// </summary>
            public DateTime? ReviewTime { get; set; }
            /// <summary>
            /// 回访内容
            /// </summary>
            public string ReviewContent { get; set; }

         
    }
}
