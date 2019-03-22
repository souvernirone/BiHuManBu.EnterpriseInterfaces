using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 分配用到的模型
    /// </summary>
    public class DistributeDto
    {
        /// <summary>
        /// 要分配的代理人集合
        /// </summary>
        public List<int> DistributeAgentIds { get; set; }
        /// <summary>
        /// 要分配数据的buid
        /// </summary>
        public List<long> DistributeBuids { get; set; }
        /// <summary>
        /// 没人分配多少条数据
        /// </summary>
        public int AverageCount { get; set; }
        /// <summary>
        /// 执行分配操作的顶级代理人id
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 执行分配操作的代理人id
        /// </summary>
        public int OperageAgentId { get; set; }
    }
}
