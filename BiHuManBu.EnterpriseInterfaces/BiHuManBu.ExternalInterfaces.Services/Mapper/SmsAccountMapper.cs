using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper
{
    public static class SmsAccountMapper
    {
        public static SmsAccount ConverToViewModel(this bx_sms_account smsAccount)
        {
            SmsAccount vm = new SmsAccount();
            if (smsAccount != null)
                vm = new SmsAccount
                {
                    id = smsAccount.id,
                    agent_id = smsAccount.agent_id,
                    sms_account = smsAccount.sms_account,
                    sms_password = smsAccount.sms_password,
                    total_times = smsAccount.total_times,
                    avail_times = smsAccount.avail_times,
                    status = smsAccount.status,
                    create_time = smsAccount.create_time
                };
            return vm;
        }
    }
}
