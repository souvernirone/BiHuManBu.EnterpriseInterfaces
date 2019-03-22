using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    //[Serializable]
    public class CameraRequest
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        public int agentId { get; set; }
        public int userId { get; set; }
        public int buId { get; set; }
        public string userName { get; set; }
        public string mobile { get; set; }
        public string condition { get; set; }
        public int type { get; set; }
        public List<LeaveDate> Leave { get; set; }

        public List<carMold> carModes { get; set; }

        public List<SealmanViewModel> sealmans { get; set; }
        public string carModelKey { get; set; }
    }

    public class carMold
    {
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// del  add  md
        /// </summary>
        public string status { get; set; }
    }

    public class LeaveDate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime leave { get; set; }
        /// <summary>
        /// del  add  md
        /// </summary>
        public string status { get; set; }
    }

    public class CameraDistributeModel
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        [Range(1, 1000000)]
        public int Agent { get; set; }
        //public int agentId { get; set; }
        public int buId { get; set; }
        public int source { get; set; }
        public int isRead { get; set; }

        public int cityCode { get; set; }
        public string licenseNo { get; set; }
        public string businessExpireDate { get; set; }
        public string forceExpireDate { get; set; }
        public int childAgent { get; set; }
        public string carModelKey { get; set; }

        /// <summary>
        /// 是否是老数据
        /// </summary>
        public bool existUserinfo { get; set; }
        /// <summary>
        /// 是否是顶级
        /// </summary>
        public bool isTopAgent { get; set; }
        /// <summary>
        /// 续保状态
        /// </summary>
        public int renewalStatus { get; set; }

        /// <summary>
        /// 原来的录入方式
        /// </summary>
        public int uiRenewalType { get; set; }
        /// <summary>
        /// 新的录入方式
        /// </summary>
        public int reqRenewalType { get; set; }

        /// <summary>
        /// 原来的openid//目前仅方法内部调用
        /// </summary>
        public string uiCustKey { get; set; }

        /// <summary>
        /// 摄像头绑定的代理人20180131启用
        /// </summary>
        public int CameraAgent { get; set; }

    }

    public class SetCameraConfigRequest : BaseRequest2
    {
        /// <summary>
        /// 距离到期时间设置
        /// </summary>
        [Range(0, 10000)]
        public int Days { get; set; }
        /// <summary>
        /// 设置是否出单、战败不提醒
        /// </summary>
        [Range(0, 1)]
        public int IsRemind { get; set; }
    }
}
