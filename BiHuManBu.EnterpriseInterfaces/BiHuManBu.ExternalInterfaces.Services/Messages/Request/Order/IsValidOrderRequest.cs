using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    public class IsValidOrderRequest : BaseRequest2
    {
        [Range(1, 9999999999)]
        public long Buid { get; set; }
    }
}
