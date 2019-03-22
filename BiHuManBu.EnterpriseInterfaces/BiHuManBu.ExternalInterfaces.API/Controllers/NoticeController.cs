using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class NoticeController : ApiController
    {
        private ILog _logInfo;
        private INoticeService _noticeService;
        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
            _logInfo = LogManager.GetLogger("INFO");
        }
        [HttpPost]
        public HttpResponseMessage ShareBatchRenewal([FromBody]BuisRequest buisRequest)
        {
            BaseViewModel bv = new BaseViewModel();
            int result = _noticeService.ShareBatchRenewal(buisRequest.buids);
            bv.BusinessStatus = result;
            return bv.ResponseToJson();
        }
        
    }

    public class BuisRequest
    {
        public string buids { get; set; }
    }
}