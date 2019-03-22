using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
   /// <summary>
   /// 城市商业交强有效报价时间
   /// </summary>
   public class AgentDredgeCity
    {
        /// <summary>
        /// 城市id
        /// </summary>
        public int cityid { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string cityname { get; set; }
        /// <summary>
        /// 交强险有效报价时间
        /// </summary>
        public int quotedays { get; set; }
        /// <summary>
        /// 商业险有效报价时间
        /// </summary>
        public int bizquotedays { get; set; }
        /// <summary>
        /// 省份简写
        /// </summary>
        public string provice_short_name { get; set; }
    }
}
