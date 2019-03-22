using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BihuBusinessSystem;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.BihuBusinessSystem;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 运营后台接口
    /// </summary>
    public class BihuBusinessSystemController : Controller
    {
        private IClass1Service _class1Service;
        //
        // GET: /BihuBusinessSystem/

        public BihuBusinessSystemController(IClass1Service class1Service)
        {
            _class1Service = class1Service;
        }

        /// <summary>
        /// 测试方法
        /// 光鹏洁 2017-10-11 /PC运营后台
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage GetAgentIdentity([FromUri]Class1Request request)
        {
            return new HttpResponseMessage();
        }
        //public ActionResult Index()
        //{
        //    return View();
        //}

    }
}
