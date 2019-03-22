using System.Web.Mvc;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class HomeController : Controller
    {
        //private IUserService _userService;

        //public HomeController(IUserService userService)
        //{
        //    _userService = userService;
        //}



        public ActionResult Index()
        {
            //using (HttpClient client = new HttpClient())
            //{
            // client.BaseAddress = new Uri("http://i.91bihu.com/");

            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //DateTime dtNow = DateTime.Parse(DateTime.Now.ToString());
            //TimeSpan toNow = dtNow.Subtract(dtStart);
            //string timeStamp = toNow.Ticks.ToString();
            //timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            // var request = new
            //{
            //    _from = "bihu",
            //    _nonce = "abcdefgh",
            //    _timestamp = timeStamp,
            //    _sign = ""
            //};
            //var queryString = "bjData=sfdfdfdfdsfdfdfsf";
            //var sign = (queryString.GetMd5() + request._nonce + request._timestamp + request._from +
            //            "NmQ0YzhmNzhlZmM1OWNk").GetMd5().ToUpper();

            //HttpContent content2 = new StringContent(JsonConvert.SerializeObject(request));
            //content2.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //HttpResponseMessage response2 = client.PostAsync("http://210.13.242.24:7001/api/insurance/updatePrice", content2).Result;
            //var rr2 = response2.Content.ReadAsStringAsync().Result;
            return View();
            //}
        }
    }
}
