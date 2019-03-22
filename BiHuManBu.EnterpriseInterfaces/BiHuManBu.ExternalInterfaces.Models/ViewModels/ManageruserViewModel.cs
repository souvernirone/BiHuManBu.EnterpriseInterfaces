using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ManageruserViewModel
    {
        public int ManagerUserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public virtual string PwdMd5 { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string Mobile { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public virtual ManagerRole ManagerRole { get; set; }

        /// <summary>
        /// 创建时间
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public virtual int ManagerRoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public virtual string ManagerRoleName { get; set; }
    }
}
