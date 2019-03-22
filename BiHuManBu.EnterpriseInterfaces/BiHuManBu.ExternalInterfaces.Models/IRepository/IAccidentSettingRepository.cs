using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAccidentSettingRepository
    {
        tx_overnoticesetting GetOverNoticeSetting(int agentId, int roleType);

        List<tx_overnoticesetting> GetOverNoticeSetting(int agentId);

        int AddOrUpdateOverNoticeSetting(OverNoticeSettingRequest overNoticeSettingRequest);

        tx_cluedistributerulesetting GetClueDistributeRuleSetting(int agentId);

        int AddClueDistributeRuleSetting(tx_cluedistributerulesetting setting);

        int UpdateClueDistributeRuleSetting(int agentId, int distributeType);

        tx_receivesdistributerulesetting GetReceivesDistributeRuleSetting(int agentId);

        int AddOrUpdateReceivesDistributeRuleSetting(int agentId, int distributeType);

        List<tx_smstemplatesetting> GetSmsTemplate(int agentId, int pageIndex, int pageSize, out int totalCount);

        tx_smstemplatesetting GetSingleSmsTemplate(int agentId, int templateId);

        int AddSmsTemplate(tx_smstemplatesetting template);
        int UpdateSmsTemplate(SmsTemplateRequest request);
        int DeleteSmsTemplate(int templateId);

        List<tx_mobile_agent_relationship> GetPhoneSetting(int agentId);
        tx_mobile_agent_relationship GetPhoneSetting(string mobile, string mobileCode);
        List<tx_mobile_agent_relationship> GetPhoneSettingByIMEI(string IMEI);
        int BindPhoneWithIMEI(string IMEI, string phoneNumber);
        tx_mobile_agent_relationship GetPhoneSetting(string mobile);

        int UnbindPhone(int phoneId, bool delete);

        int AddPhone(tx_mobile_agent_relationship phone);
        bool IsExistsPhone(string phoneNumber);
        bool IsExistsSmsTemplateName(Expression<Func<tx_smstemplatesetting, bool>> predicate);

        bool IsBindMaxCount(int topAgentId);
    }
}
