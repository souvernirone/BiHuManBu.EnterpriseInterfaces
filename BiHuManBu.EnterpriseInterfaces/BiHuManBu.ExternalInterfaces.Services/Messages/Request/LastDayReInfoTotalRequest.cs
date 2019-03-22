using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class LastDayReInfoTotalRequest:BaseRequest
    {
        [Required]
        public string StrId { get; set; }

        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        /// <summary>
        /// 6 管理员，4 普通员工
        /// </summary>
        [Range(1, 10)]
        public int LevelType { get; set; }

        public string LicenseNo { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
