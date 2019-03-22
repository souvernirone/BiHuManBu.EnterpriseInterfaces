using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class UntreatedModel
    {
        /// <summary>
        /// dz_correct_license_plate.id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 当前代理人
        /// </summary>
        public int Agent { get; set; }
        /// <summary>
        /// 顶级代理人
        /// </summary>
        public int TopAgent { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 直接从前段拿到的数据
        /// </summary>
        public int IsCorrectSource { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string secCode { get; set; }
    }
}
