using BiHuManBu.ExternalInterfaces.Models.Specification;

namespace BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent.Specification
{
    public class AgentHasSubmitGrantSpecification : CompositeSpecification<Models.bx_agent>
    {
        public override bool IsSatisfiedBy(Models.bx_agent candidate)
        {
            return candidate.IsSubmit == 1;
        }
    }
}