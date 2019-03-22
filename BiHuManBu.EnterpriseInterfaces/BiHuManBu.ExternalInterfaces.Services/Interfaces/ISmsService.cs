using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ISmsService
    {
        SmsResultViewModel_sms SendSms(SmsRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        GetSmsAccountResponse GetSmsAccount(SmsAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        GetSmsOrderStatusResponse SmsOrderStatus(SmsOrderStatusRequest request, IEnumerable<KeyValuePair<string, string>> enumerable);

        GetSmsOrderResponse SmsCreateOrder(SmsCreateOrderRequest request, IEnumerable<KeyValuePair<string, string>> enumerable);

        GetSmsAccountResponse CreateAccount(CreateAccountRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);
        GetSmsOrderDetailResponse GetSmsOrderDetail(GetSmsOrderDetailRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);
    }
}
