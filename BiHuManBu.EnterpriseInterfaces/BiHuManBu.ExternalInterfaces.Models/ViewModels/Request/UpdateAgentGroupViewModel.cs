using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class UpdateAgentGroupViewModel
    {
        /// <summary>
        /// 当前代理人编号
        /// </summary>
        public int CurrentAgentId { get; set; }
        /// <summary>
        ///属于代理人编号集合
        /// </summary>
        public List<int> FromParentAgentIdKeys { get; set; }
        /// <summary>
        /// 去向代理人编号
        /// </summary>
        public int ToAgentIdKey { get; set; }
        private int _operationType = -1;
        /// <summary>
        /// 操作类型：1->升级，0->降级
        /// </summary>
        public int OperationType { get { return _operationType; } set{ _operationType = value; } }
    }
}
