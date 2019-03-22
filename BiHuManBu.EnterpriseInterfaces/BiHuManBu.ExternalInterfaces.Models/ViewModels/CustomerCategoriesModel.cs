using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 添加模型
    /// </summary>
    public class CustomerCategoriesModel
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        /// <summary>
        /// 顶级代理人
        /// </summary>
        public int Agent { get; set; }
        public string CategoryInfo { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool Deleted { get; set; }
        //出单后变为
        public int IssuingTrans { get; set; }
        //战败后变为
        public int DefeatTrans { get; set; }
        //是否启动
        private int _isStart = 1;
        public int IsStart
        {
            get { return _isStart; }
            set { _isStart = value; }
        }

    }
    /// <summary>
    /// 修改模型
    /// </summary>
    public class UpdateCustomerCategoriesModel
    {
        public int Id { get; set; }
        public string CategoryInfo { get; set; }
        //出单后变为
        public int IssuingTrans { get; set; }
        //战败后变为
        public int DefeatTrans { get; set; }
        //是否启动
        public int IsStart { get; set; }
    }

    /// <summary>
    /// 删除模型
    /// </summary>
    public class RemoveModel
    {
        public int Id { get; set; }
        //1 直接删除， 2 转移数据
        public int removeWay { get; set; }
        //移动到的类别ID
        public int moveTo { get; set; }
        //代理人ID
        public int agentId { get; set; }
    }
}
