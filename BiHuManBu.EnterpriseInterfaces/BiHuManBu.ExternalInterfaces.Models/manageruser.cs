//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BiHuManBu.ExternalInterfaces.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class manageruser
    {
        public int ManagerUserId { get; set; }
        public string Name { get; set; }
        public string PwdMd5 { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> ManagerState { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> OperatorId { get; set; }
        public string OperatorName { get; set; }
        public Nullable<System.DateTime> OperatorTime { get; set; }
        public int ManagerRoleId { get; set; }
        public string CreateTime { get; set; }
        public Nullable<int> AccountType { get; set; }
        public Nullable<int> department_id { get; set; }
    }
}