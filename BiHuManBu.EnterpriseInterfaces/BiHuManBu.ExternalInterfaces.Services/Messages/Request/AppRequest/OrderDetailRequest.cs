using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class OrderDetailRequest
    {
        [Range(1, 1000000000)]
        //当前Agent
        public int Agent { get; set; }
        //校验码
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        //顶级Agent
        [Range(1, 1000000000)]
        public int Topagent { get; set; }
        //通知商户此处由stirng改为long
        [Range(1, 90000000000000000)]
        public long OrderId { get; set; }
    }
}
