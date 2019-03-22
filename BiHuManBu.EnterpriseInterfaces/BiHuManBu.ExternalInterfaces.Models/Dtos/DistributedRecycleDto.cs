namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class DistributedRecycleDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Buid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AgentId { get; set; }
    }


    /// <summary>
    /// 客户转移，插入记录表模型
    /// </summary>
    public class TransferRecycleDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Buid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OldAgentId { get; set; }

        public string NewAgentId { get; set; }
    }
}
