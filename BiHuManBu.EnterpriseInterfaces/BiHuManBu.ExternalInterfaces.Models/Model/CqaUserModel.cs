using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
   public  class CqaUserModel
    {

        #region Model
        /// <summary>
        /// 主键
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public int creator_id { get; set; }
        /// <summary>
        /// 更新人员名称
        /// </summary>
        public string modifi_name { get; set; }
        /// <summary>
        /// 更新人员id
        /// </summary>
        public int? modifi_id { get; set; }
        /// <summary>
        /// 区域id，枚举类型
        /// </summary>
        public int? sell_area_id { get; set; }

        public string sell_areaName { get; set; }
        /// <summary>
        /// 0销售，1城市经理，2大区总
        /// </summary>
        public int? sell_position { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string sell_positionName { get; set; }
        /// <summary>
        /// 上级销售id
        /// </summary>
        public int? sell_parent_id { get; set; }
        /// <summary>
        /// 角色 1、销售 2、运营
        /// </summary>
        public int role { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }
        #endregion
 

    }
}
