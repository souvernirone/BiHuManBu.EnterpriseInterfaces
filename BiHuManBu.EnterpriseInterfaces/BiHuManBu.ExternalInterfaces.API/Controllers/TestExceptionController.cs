using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 测试异常
    /// </summary>
    public class TestExceptionController : ApiController
    {
        public TestExceptionController()
        {
            var i = Convert.ToDateTime("asdfsadf");
        }

        [HttpGet]
        public string Get([FromUri] TestParam request)
        {
            Convert.ToDateTime("dafds");
            throw new Exception("get");
        }

        [HttpPost]
        public string Post([FromBody]TestParam request)
        {
            //var dd= Request.Content.ReadAsStringAsync().Result;
            Convert.ToDateTime("4234234234");
            throw new Exception("Post");
        }
    }

    public class TestParam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> ListBuid { get; set; }
    }
}
