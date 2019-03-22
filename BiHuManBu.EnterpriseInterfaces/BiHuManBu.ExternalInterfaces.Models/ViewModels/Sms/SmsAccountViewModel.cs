using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms
{
    public class SmsAccountViewModel : BaseViewModel
    {
        public SmsAccount SmsAccount { get; set; }
    }

    public class SmsAccount
    {
        public int id { get; set; }
        public int? agent_id { get; set; }
        public string sms_account { get; set; }
        public string sms_password { get; set; }
        public long? total_times { get; set; }
        public long? avail_times { get; set; }
        public int? status { get; set; }
        public DateTime? create_time { get; set; }
    }
}
