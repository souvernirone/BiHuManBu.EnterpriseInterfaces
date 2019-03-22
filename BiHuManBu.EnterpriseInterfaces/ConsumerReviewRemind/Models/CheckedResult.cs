using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerReviewRemind.Models
{
   public class CheckedResult
    {
       /// <summary>
       /// bx_userinfo.Id
       /// </summary>
        public long BuId { get; set; }
       /// <summary>
       /// bx_agent.id
       /// </summary>
        public int AgentId { get; set; }
       /// <summary>
       /// 车牌号
       /// </summary>
        public string Licenseno { get; set; }
       /// <summary>
       /// 提醒分钟数
       /// </summary>
        public int RemindMinute { get;set;}
    }
}
