using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent.Specification;
using BiHuManBu.ExternalInterfaces.Models.Specification;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public  partial  class bx_agent:IBxAgent
    {
        private readonly AgentIsActiveSpecification _isActiveSpecification;
        private readonly AgentHasQuoteGrantSpecification _hasQuoteGrantSpecification;
        private readonly AgentHasSubmitGrantSpecification _hasSubmitGrantSpecification;

        public bx_agent()
        {
           
            _hasQuoteGrantSpecification = new AgentHasQuoteGrantSpecification();
            _hasSubmitGrantSpecification = new AgentHasSubmitGrantSpecification();
            _isActiveSpecification = new AgentIsActiveSpecification();
        }

        /// <summary>
        /// 经纪人是否是可用状态
        /// </summary>
        /// <returns></returns>
        public bool AgentCanUse()
        {
            ISpecification<Models.bx_agent> candidate = _isActiveSpecification;
            return candidate.IsSatisfiedBy(this);
        }
        /// <summary>
        /// 经纪人是否拥有报价权限
        /// </summary>
        /// <returns></returns>
        public bool AgentCanQuote()
        {
            ISpecification<Models.bx_agent> candicate = _isActiveSpecification.And(_hasQuoteGrantSpecification);

            return candicate.IsSatisfiedBy(this);
        }

        /// <summary>
        /// 经纪人是否拥有核保权限
        /// </summary>
        /// <returns></returns>
        public bool AgentCanSubmit()
        {
            ISpecification<bx_agent> candidate =
                _isActiveSpecification.And(_hasQuoteGrantSpecification).And(_hasSubmitGrantSpecification);

            return candidate.IsSatisfiedBy(this);
        }
        
    }
}
