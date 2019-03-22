
using System.ComponentModel.DataAnnotations;
namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetOrderByBuidRequest
    {
        [Range(1, 200000000)]
        public int Buid { get; set; }

        [Range(1, 200000000)]
        public int TopAgentId { get; set; }
    }
}
