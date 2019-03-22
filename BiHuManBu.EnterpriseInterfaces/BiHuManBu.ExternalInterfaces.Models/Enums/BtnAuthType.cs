namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    /// <summary>
    /// 这里的枚举值和manager_module_button.button_code是一一对应的，增加的时候千万要注意
    /// </summary>
    public enum BtnAuthType
    {
        /// <summary>
        /// 分配/回收数据
        /// </summary>
        btn_recycle,
        /// <summary>
        /// 批量删除
        /// </summary>
        btn_delete,
        /// <summary>
        /// 导出Excel
        /// </summary>
        btn_export,
        /// <summary>
        /// 录入回访
        /// </summary>
        btn_review,
        /// <summary>
        /// 批量修改数据
        /// </summary>
        btn_mass_edit,
    }
}
