using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Collections.Generic;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AccidentOrderService: IAccidentOrderService
    {
        private readonly IAccidentOrderRepository _accidentOrderRepository;
        private ILog logError = LogManager.GetLogger("ERROR");

        public AccidentOrderService(IAccidentOrderRepository accidentOrderRepository)
        {
            _accidentOrderRepository = accidentOrderRepository;
        }

        public List<AccidentSource> GetSource(string sourceName)
        {
            return _accidentOrderRepository.GetSource(sourceName);
        }

        public List<AccidentCarBrand> GetCarBrands(string brandName)
        {
            return _accidentOrderRepository.GetCarBrands(brandName);
        }

        public MobileStatisticsBaseVM<SettlementViewModel> GetSettlementList(AccidentSettlementRequest request)
        {
            var result = new MobileStatisticsBaseVM<SettlementViewModel>();
            result.PageIndex = request.PageIndex;
            result.PageSize = request.PageSize;
            int totalCount = 0;
            result.DataList = _accidentOrderRepository.GetSettlementList(request,out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        public List<RecivesCarPeople> GetGrabOrderPeoples(int topAgentId)
        {
            return _accidentOrderRepository.GetGrabOrderPeoples(topAgentId);
        }
    }
}
