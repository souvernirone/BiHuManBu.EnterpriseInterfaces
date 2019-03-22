namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateAgentEffectiveCallDurationRequest
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 有效时常
        /// </summary>
        public int EffectiveCallDuration { get; set; }
    }
}
