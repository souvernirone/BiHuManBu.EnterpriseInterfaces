using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetByOpenIdRequest
    {
        [Required]
        [StringLength(32, MinimumLength = 16)]
        public string OpenId { get; set; }
        [Range(0, 21000000000)]
        public int TopParentAgent { get; set; }
    }
}
