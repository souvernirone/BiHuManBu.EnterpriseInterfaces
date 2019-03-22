using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SetOverTransferCreditsRequest : BaseRequest2
    {
        [Range(0.01, 100)]
        public double Number { get; set; }
    }

    public class AddRateRequest : BaseRequest2
    {
        /// <summary>
        /// 车辆使用性质，使用逗号分隔
        /// </summary>
        [Required]
        public string CarUsedType { get; set; }
        /// <summary>
        /// 精算口径，使用逗号分隔
        /// </summary>
        [Required]
        public string ActuarialCalibre { get; set; }
        /// <summary>
        /// 交强险费率
        /// </summary>
        [Range(0.01, 100)]
        public double ForceRate { get; set; }
        /// <summary>
        /// 商业险费率
        /// </summary>
        [Range(0.01, 100)]
        public double BizRate { get; set; }

        /// <summary>
        /// 在新增时不用传id，在编辑时传id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商业险或交强险费率超过此值的部分，自动转化为积分
        /// 2018-10-10 张克亮
        /// </summary>
        public double OverTransferCredits {get;set;}
    }

    //public class GetRateListRequest : BaseRequest2
    //{

    //}

    public class DeleteRateRequest : BaseRequest2
    {
        /// <summary>
        /// bx_ratepolicy_setting.id
        /// </summary>
        public int Id { get; set; }
    }
}
