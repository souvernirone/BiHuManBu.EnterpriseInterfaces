using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class ThirdRelatedInfo
    {
        /// <summary>
        /// 车主
        /// </summary>
        public RelatedPerson OwerPerson { get; set; }
        /// <summary>
        /// 投保人
        /// </summary>
        public RelatedPerson HolderPerson { get; set; }
        /// <summary>
        /// 被保险人
        /// </summary>
        public RelatedPerson InsuredPerson { get; set; }
    }
}
