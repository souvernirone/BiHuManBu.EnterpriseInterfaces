using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAccidentSettingService
    {
        tx_overnoticesetting GetOverNoticeSetting(int agentId, int roleType);

        tx_cluedistributerulesetting GetClueDistributeRuleSetting(int agentId);

        tx_receivesdistributerulesetting GetReceivesDistributeRuleSetting(int agentId);

        List<tx_mobile_agent_relationship> GetPhoneSetting(int agentId);

        List<tx_mobile_agent_relationship> GetPhoneSettingByIMEI(string IMEI);

        int BindPhoneWithIMEI(string IMEI, string phoneNumber);

        MobileStatisticsBaseVM<tx_smstemplatesetting> GetSmsTemplate(int agentId,int pageIndex,int pageSize);

        tx_smstemplatesetting GetSingleSmsTemplate(int agentId, int templateId);

        int UnbindPhone(int phoneId, bool delete);

        int DeleteSmsTemplate(int templateId);

        BaseViewModel AddPhone(int agentId, string phoneNumber);
        int IsCanBind(int agentId, string phoneNumber);
        int AddOrUpdateSmsTemplate(SmsTemplateRequest smsTemplateRequest);

        int AddOrUpdateOverNoticeSetting(OverNoticeSettingRequest overNoticeSettingRequest);

        int AddOrUpdateClueDistributeRuleSetting(int agentId, int distributeType);

        int AddOrUpdateReceivesDistributeRuleSetting(int agentId, int distributeType);
    }
}
