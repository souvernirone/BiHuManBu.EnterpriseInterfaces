using BiHuManBu.ExternalInterfaces.Models.Specification;

namespace BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent.Specification
{
    public class AgentHasQuoteGrantSpecification : CompositeSpecification<Models.bx_agent>
    {
        public override bool IsSatisfiedBy(Models.bx_agent candidate)
        {
            return candidate.IsQuote == 1;
        }
    }
}