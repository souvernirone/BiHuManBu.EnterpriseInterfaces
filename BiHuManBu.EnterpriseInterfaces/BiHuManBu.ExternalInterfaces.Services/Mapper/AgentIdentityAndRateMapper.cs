using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;


namespace BiHuManBu.ExternalInterfaces.Services.Mapper
{
    public static class AgentIdentityAndRateMapper
    {
        public static AgentIdentityAndRateViewModel ConvertToViewModel(this GetAgentIdentityAndRateResponse response)
        {
            var model = new AgentIdentityAndRateViewModel();
            model.Item = new AgentIdentityAndRateViewModel.AgentIdentityAndRate
            {
                BizRate = response.BizRate,
                ForceRate = response.ForceRate,
                IsAgent = response.IsAgent,
                TaxRate = response.TaxRate
            };

            return model;
        }
    }
}
