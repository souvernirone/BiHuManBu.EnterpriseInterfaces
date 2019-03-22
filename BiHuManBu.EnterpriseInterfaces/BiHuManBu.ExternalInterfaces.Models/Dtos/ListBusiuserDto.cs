using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class ListBusiuserDto
    {
        /// <summary>
        /// bx_agent_busiuser.id
        /// </summary>
        public int AgentBusiuserId { get; set; }
        /// <summary>
        /// bx_agent_busiuser.source
        /// </summary>
        public int Source { get; set; }

        public string SourceName
        {
            get { return ConvertHelper.ConvertSource(Source); }
        }

        /// <summary>
        /// bx_busiusersetting.machineCode
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// bx_agent_config.config_name
        /// </summary>
        public string ConfigName { get; set; }
    }
}
