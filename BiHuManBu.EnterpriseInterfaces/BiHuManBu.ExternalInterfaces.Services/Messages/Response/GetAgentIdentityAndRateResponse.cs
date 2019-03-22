using System.Net;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class GetAgentIdentityAndRateResponse
    {
        /// <summary>
        /// 是否是经纪人 1:经纪人 0：直客
        /// </summary>
        public int IsAgent { get; set; }

        public double BizRate { get; set; }
        public double ForceRate { get; set; }
        public double TaxRate { get; set; }

        public HttpStatusCode Status { get; set; }
      //  public List<int> CompList { get; set; } 

    }
}
