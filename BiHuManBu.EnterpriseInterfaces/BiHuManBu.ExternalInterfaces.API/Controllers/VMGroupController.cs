using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.VMProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.VMProject;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class VMGroupController : ApiController
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public VMGroupController()
        {

        }
        [HttpGet, ModelVerify]
        //[CustomizedRequestAuthorize]
        public HttpResponseMessage GetGroupList([FromUri]GetGroupListRequest request)
        {
            return null;
        }
    }
}
