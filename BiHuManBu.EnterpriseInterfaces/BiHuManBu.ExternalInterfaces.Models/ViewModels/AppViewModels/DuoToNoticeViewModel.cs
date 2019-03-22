using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    /// <summary>
    /// 发送续保通知的模型
    /// </summary>
    public class DuoToNoticeViewModel
    {
        public int AgentId { get; set; }
        public List<CompositeBuIdLicenseNo> Data { get; set; }
        public string BuidsString { get; set; }
    }

    public class CompositeBuIdLicenseNo
    {
        public long BuId { get; set; }
        public string LicenseNo { get; set; }
        public int Days { get; set; }

    }


}
