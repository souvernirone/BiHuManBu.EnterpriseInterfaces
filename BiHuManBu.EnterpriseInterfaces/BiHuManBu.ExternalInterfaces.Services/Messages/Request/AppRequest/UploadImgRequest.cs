
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class UploadImgRequest
    {
        [Required]
        public string baseContent { get; set; }
    }
}
