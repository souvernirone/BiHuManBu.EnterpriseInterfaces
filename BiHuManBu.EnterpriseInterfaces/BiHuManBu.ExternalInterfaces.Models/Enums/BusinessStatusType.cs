using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models
{
    /// <summary>
    /// 返回值状态枚举
    /// </summary>
    public enum BusinessStatusType
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        OK = 1,

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        ParamError = -10000,
        /// <summary>
        /// 参数校验错误，请检查您的校验码，（seccode）
        /// </summary>
        [Description("参数校验错误，请检查您的校验码（seccode）")]
        ParamVerifyError = -10001,

        /// <summary>
        /// 获取信息失败
        /// </summary>
        [Description("获取信息失败")]
        GetFailed = -10002,

        /// <summary>
        /// 系统出现错误
        /// </summary>
        [Description("系统出现错误")]
        SystemError = -10003,

        /// <summary>
        /// 条件不符合
        /// </summary>
        [Description("条件不符合")]
        NoCondition = -10016,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        OperateError = -10017,

        /// <summary>
        /// 数据错误
        /// </summary>
        [Description("数据错误")]
        DataError= -10018,

        /// <summary>
        /// 没有数据
        /// </summary>
        [Description("没有数据")]
        NoData= -10019,

        /// <summary>
        /// 该数据所属的代理人下已经有相同车牌号的数据
        /// </summary>
        [Description("客户列表已存在相同数据，不可撤销本条数据")]
        RevokeHasData= -10021,

        /// <summary>
        /// 可以让前端展示给客户的提示，这里的错误信息需要自己写
        /// </summary>
        ErrorCanShowCustomer =-10009,
    }
}
