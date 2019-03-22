using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class AccidentOrderController : ApiController
    {
        private readonly IAccidentOrderService _accidentOrderService;
        public AccidentOrderController(IAccidentOrderService accidentOrderService)
        {
            _accidentOrderService = accidentOrderService;
        }

        /// <summary>
        /// pc获取保险公司
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public List<AccidentSource> GetPcSource(string sourceName = "")
        {
           return _accidentOrderService.GetSource(sourceName);
        }

        /// <summary>
        ///  pc 获取车辆品牌
        /// </summary>
        /// <param name="brandName"></param>
        /// <returns></returns>
        public List<AccidentCarBrand> GetPcCarBrands(string brandName = "")
        {
           return _accidentOrderService.GetCarBrands(brandName);
        }

        [HttpGet]
        public HttpResponseMessage GetSettlementList([FromUri]AccidentSettlementRequest request)
        {
            var viewModel = new BaseViewModel();
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                viewModel.BusinessStatus = 10000;
                viewModel.StatusMessage = statusMessage;
                return viewModel.ResponseToJson();
            }
            viewModel.Data = _accidentOrderService.GetSettlementList(request);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }
    }
}
