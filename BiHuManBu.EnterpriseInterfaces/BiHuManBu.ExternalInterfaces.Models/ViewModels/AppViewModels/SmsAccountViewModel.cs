using System;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class SmsAccountViewModel:BaseViewModel
    {
        public SmsAccount SmsAccount { get; set; }
    }

    public class SmsAccount
    {
        public int id { get; set; }
        public Nullable<int> agent_id { get; set; }
        public string sms_account { get; set; }
        public string sms_password { get; set; }
        public Nullable<long> total_times { get; set; }
        public Nullable<long> avail_times { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<DateTime> create_time { get; set; }
    }
}
