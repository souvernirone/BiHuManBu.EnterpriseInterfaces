using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 客户状体添加模型
    /// </summary>
    public class CustomerStatusModel:BaseRequest2
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string StatusInfo { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool Deleted { get; set; }
        public Nullable<int> IsSystemData { get; set; }
        public int T_Id { get; set; }
    }
    /// <summary>
    /// 修改模型
    /// </summary>
    public class UpdateCustomerStatusModel
    {
        public int Id { get; set; }
        public string StatusInfo { get; set; }

    }

    /// <summary>
    /// 删除模型
    /// </summary>
    public class RemoveCustomerStatusModel:BaseRequest2
    {
        public int Id { get; set; }
        public int T_Id { get; set; }
        //1 直接删除， 2 转移数据
        public int removeWay { get; set; }
        //移动到的类别ID
        public int moveTo { get; set; }
        //代理人ID
        public int agentId { get; set; }
    }
}
