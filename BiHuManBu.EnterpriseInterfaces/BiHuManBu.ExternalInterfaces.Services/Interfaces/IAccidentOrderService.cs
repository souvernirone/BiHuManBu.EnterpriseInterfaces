using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAccidentOrderService
    {
        List<AccidentSource> GetSource(string sourceName);

        List<AccidentCarBrand> GetCarBrands(string brandName);

        MobileStatisticsBaseVM<SettlementViewModel> GetSettlementList(AccidentSettlementRequest request);

        List<RecivesCarPeople> GetGrabOrderPeoples(int topAgentId);
    }
}
