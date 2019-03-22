using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAccidentOrderRepository
    {
        List<AccidentSource> GetSource(string sourceName);

        List<AccidentCarBrand> GetCarBrands(string brandName);

        List<SettlementViewModel> GetSettlementList(AccidentSettlementRequest request, out int totalCount);

        List<RecivesCarPeople> GetGrabOrderPeoples(int topAgentId);
    }
}
