using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 删除角色
    /// </summary>
    public class DeleteRoleRequest : BaseRequest
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "角色Id不正确")]
        public int RoleId { get; set; }

    }

    public class UpdateRoleRequest : BaseRequest2
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 是否代报价
        /// </summary>
        public int IsRequote { get; set; }
    }
}
