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
    
    public partial class tx_noticemessage
    {
        public int id { get; set; }
        public int mesaagetype { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int operateagentId { get; set; }
        public int reciveaentId { get; set; }
        public int culeid { get; set; }
        public int culestate { get; set; }
        public int lookoverstate { get; set; }
        public System.DateTime createTime { get; set; }
        public System.DateTime updatetime { get; set; }
        public int deleted { get; set; }
        public int issend { get; set; }
    }
}
