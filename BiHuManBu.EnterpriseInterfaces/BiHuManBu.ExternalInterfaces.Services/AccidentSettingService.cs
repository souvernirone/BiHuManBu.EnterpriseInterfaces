using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AccidentSettingService : IAccidentSettingService
    {
        private readonly IAccidentSettingRepository _accidentSettingRepository;
        public AccidentSettingService(IAccidentSettingRepository accidentSettingRepository)
        {
            this._accidentSettingRepository = accidentSettingRepository;
        }
        public int AddOrUpdateClueDistributeRuleSetting(int agentId, int distributeType)
        {
            var clueDistributeRuleSetting = _accidentSettingRepository.GetClueDistributeRuleSetting(agentId);
            if (clueDistributeRuleSetting == null) //新增
            {
                var setting = new tx_cluedistributerulesetting();
                setting.agentid = agentId;
                setting.DistributeType = distributeType;
                setting.createtime = DateTime.Now;
                setting.updatetime = DateTime.Now;
                setting.deleted = 0;
                var result = _accidentSettingRepository.AddClueDistributeRuleSetting(setting);
                return result > 0 ? 1 : 0;
            }
            else //修改
            {
                var result = _accidentSettingRepository.UpdateClueDistributeRuleSetting(agentId, distributeType);
                return result > 0 ? 1 : 0;
            }
        }

        public int AddOrUpdateOverNoticeSetting(OverNoticeSettingRequest overNoticeSettingRequest)
        {
            return _accidentSettingRepository.AddOrUpdateOverNoticeSetting(overNoticeSettingRequest);
        }

        public int AddOrUpdateReceivesDistributeRuleSetting(int agentId, int distributeType)
        {
            return _accidentSettingRepository.AddOrUpdateReceivesDistributeRuleSetting(agentId, distributeType);
        }

        public int AddOrUpdateSmsTemplate(SmsTemplateRequest request)
        {
            if (request.Id == 0) //新增
            {
                var isExitsSmsTemplateName = _accidentSettingRepository.IsExistsSmsTemplateName(x => x.agentid == request.TopAgentId && x.SmsTemplateName == request.SmsTemplateName && x.deleted == 0);
                if (isExitsSmsTemplateName)
                {
                    return -1;
                }
                var template = new tx_smstemplatesetting();
                template.agentid = request.TopAgentId;
                template.SmsTemplateName = request.SmsTemplateName;
                template.SmsTemplateContent = request.SmsTemplateContent;
                template.createtime = DateTime.Now;
                template.updatetime = DateTime.Now;
                template.deleted = 0;
                var r = _accidentSettingRepository.AddSmsTemplate(template);
                return r > 0 ? 1 : -1;
            }
            else //修改
            {
                var isExitsSmsTemplateName = _accidentSettingRepository.IsExistsSmsTemplateName(x => x.agentid == request.TopAgentId && x.SmsTemplateName == request.SmsTemplateName && x.deleted == 0 && x.id != request.Id);
                if (isExitsSmsTemplateName)
                {
                    return -1;
                }
                return _accidentSettingRepository.UpdateSmsTemplate(request);
            }
        }

        public BaseViewModel AddPhone(int agentId, string phoneNumber)
        {
            BaseViewModel viewModel = new BaseViewModel();
            int isCanBind = IsCanBind(agentId, phoneNumber);
            if (isCanBind == -1)
            {
                viewModel.Data = -1;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "绑定失败,数量已达上限";
                return viewModel;
            }
            if (isCanBind == 0)
            {
                viewModel.Data = -1;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "绑定失败,该手机号已存在";
                return viewModel;
            }
            var phone = new tx_mobile_agent_relationship();
            phone.agentid = agentId;
            phone.mobile = phoneNumber;
            phone.createTime = DateTime.Now;
            phone.updatetime = DateTime.Now;
            phone.deleted = 0;
            viewModel.BusinessStatus = 200;
            viewModel.Data = _accidentSettingRepository.AddPhone(phone); ;
            viewModel.StatusMessage = "绑定成功";
            return viewModel;
        }

        public int IsCanBind(int agentId, string phoneNumber)
        {
            bool isBindMaxCount = _accidentSettingRepository.IsBindMaxCount(agentId);
            if (!isBindMaxCount)
            {
                return -1;
            }
            bool isExistsPhone = _accidentSettingRepository.IsExistsPhone(phoneNumber);
            if (isExistsPhone)
            {
                return 0;
            }
            return 1;
        }

        public int UnbindPhone(int phoneId, bool delete)
        {
            return _accidentSettingRepository.UnbindPhone(phoneId, delete);
        }

        public int DeleteSmsTemplate(int templateId)
        {
            return _accidentSettingRepository.DeleteSmsTemplate(templateId);
        }

        public tx_cluedistributerulesetting GetClueDistributeRuleSetting(int agentId)
        {
            return _accidentSettingRepository.GetClueDistributeRuleSetting(agentId);
        }

        public tx_overnoticesetting GetOverNoticeSetting(int agentId, int roleType)
        {
            return _accidentSettingRepository.GetOverNoticeSetting(agentId, roleType);
        }

        public List<tx_mobile_agent_relationship> GetPhoneSetting(int agentId)
        {
            return _accidentSettingRepository.GetPhoneSetting(agentId);
        }

        public List<tx_mobile_agent_relationship> GetPhoneSettingByIMEI(string IMEI)
        {
            return _accidentSettingRepository.GetPhoneSettingByIMEI(IMEI);
        }

        public int BindPhoneWithIMEI(string IMEI, string phoneNumber)
        {
            return _accidentSettingRepository.BindPhoneWithIMEI(IMEI, phoneNumber);
        }

        public tx_receivesdistributerulesetting GetReceivesDistributeRuleSetting(int agentId)
        {
            return _accidentSettingRepository.GetReceivesDistributeRuleSetting(agentId);
        }

        public MobileStatisticsBaseVM<tx_smstemplatesetting> GetSmsTemplate(int agentId, int pageIndex, int pageSize)
        {
            var totalCount = 0;
            var datalist = _accidentSettingRepository.GetSmsTemplate(agentId, pageIndex, pageSize, out totalCount);
            return new MobileStatisticsBaseVM<tx_smstemplatesetting>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                DataList = datalist
            };
        }

        public tx_smstemplatesetting GetSingleSmsTemplate(int agentId, int templateId)
        {
            return _accidentSettingRepository.GetSingleSmsTemplate(agentId, templateId);
        }





    }
}
