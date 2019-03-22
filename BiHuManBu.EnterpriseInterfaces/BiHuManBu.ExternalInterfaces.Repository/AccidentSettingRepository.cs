using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AccidentSettingRepository : IAccidentSettingRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private EntityContext _context = DataContextFactory.GetDataContext();
        public tx_overnoticesetting GetOverNoticeSetting(int agentId, int roleType)
        {
            return _context.tx_overnoticesetting.Where(x => x.agentid == agentId && x.roletype == roleType).FirstOrDefault();
        }


        public List<tx_overnoticesetting> GetOverNoticeSetting(int agentId)
        {
            return _context.tx_overnoticesetting.Where(x => x.agentid == agentId).ToList();
        }



        public int AddOrUpdateOverNoticeSetting(OverNoticeSettingRequest overNoticeSettingRequest)
        {
            var overNoticeSetting = GetOverNoticeSetting(overNoticeSettingRequest.TopAgentId, overNoticeSettingRequest.RoleType);
            if (overNoticeSetting == null) //新增
            {
                var setting = new tx_overnoticesetting();
                setting.agentid = overNoticeSettingRequest.TopAgentId;
                setting.roletype = overNoticeSettingRequest.RoleType;
                setting.overtime = overNoticeSettingRequest.OverTime;
                setting.pollingcycle = overNoticeSettingRequest.PollingCycle;
                setting.createtime = DateTime.Now;
                setting.updatetime = DateTime.Now;
                setting.deleted = 0;
                var s = _context.tx_overnoticesetting.Add(setting);
                _context.SaveChanges();
                if (overNoticeSettingRequest.RoleType == 7)
                {
                    var topSetting = setting;
                    topSetting.roletype = 3;
                    _context.tx_overnoticesetting.Add(topSetting);
                    _context.SaveChanges();
                }
                return s.id;
            }
            else //修改
            {
                overNoticeSetting.overtime = overNoticeSettingRequest.OverTime;
                overNoticeSetting.pollingcycle = overNoticeSettingRequest.PollingCycle;
                overNoticeSetting.updatetime = DateTime.Now;
                _context.SaveChanges();
                if (overNoticeSettingRequest.RoleType == 7)
                {
                    var topSetting = GetOverNoticeSetting(overNoticeSettingRequest.TopAgentId, 3);
                    topSetting.overtime = overNoticeSettingRequest.OverTime;
                    topSetting.pollingcycle = overNoticeSettingRequest.PollingCycle;
                    topSetting.updatetime = DateTime.Now;
                    _context.SaveChanges();
                }
            }
            return overNoticeSetting.id;
        }

        public tx_cluedistributerulesetting GetClueDistributeRuleSetting(int agentId)
        {
            return _context.tx_cluedistributerulesetting.Where(x => x.agentid == agentId).FirstOrDefault();
        }

        public int AddClueDistributeRuleSetting(tx_cluedistributerulesetting setting)
        {
            var s = _context.tx_cluedistributerulesetting.Add(setting);
            _context.SaveChanges();
            return s.id;
        }

        public int UpdateClueDistributeRuleSetting(int agentId, int distributeType)
        {
            var setting = GetClueDistributeRuleSetting(agentId);
            setting.DistributeType = distributeType;
            setting.updatetime = DateTime.Now;
            _context.SaveChanges();
            return setting.id;
        }

        public tx_receivesdistributerulesetting GetReceivesDistributeRuleSetting(int agentId)
        {
            return _context.tx_receivesdistributerulesetting.Where(x => x.agentid == agentId).FirstOrDefault();
        }

        public int AddOrUpdateReceivesDistributeRuleSetting(int agentId, int distributeType)
        {
            var receivesDistributeRuleSetting = GetReceivesDistributeRuleSetting(agentId);
            if (receivesDistributeRuleSetting == null) //新增
            {
                var setting = new tx_receivesdistributerulesetting();
                setting.agentid = agentId;
                setting.DistributeType = distributeType;
                setting.createtime = DateTime.Now;
                setting.updatetime = DateTime.Now;
                setting.deleted = 0;
                var s = _context.tx_receivesdistributerulesetting.Add(setting);
                _context.SaveChanges();
                return s.id;
            }
            else //修改
            {
                receivesDistributeRuleSetting.DistributeType = distributeType;
                receivesDistributeRuleSetting.updatetime = DateTime.Now;
                _context.SaveChanges();
            }
            return receivesDistributeRuleSetting.id;
        }

        public List<tx_smstemplatesetting> GetSmsTemplate(int agentId, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = _context.tx_smstemplatesetting.Count(x => x.agentid == agentId && x.deleted == 0);
            return _context.tx_smstemplatesetting.Where(x => x.agentid == agentId && x.deleted == 0).OrderBy(x => x.id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public tx_smstemplatesetting GetSingleSmsTemplate(int agentId, int templateId)
        {
            return _context.tx_smstemplatesetting.Where(x => x.agentid == agentId && x.id == templateId && x.deleted == 0).FirstOrDefault();
        }

        public int AddSmsTemplate(tx_smstemplatesetting template)
        {
            var s = _context.tx_smstemplatesetting.Add(template);
            _context.SaveChanges();
            return s.id;
        }

        public int UpdateSmsTemplate(SmsTemplateRequest request)
        {
            var template = _context.tx_smstemplatesetting.Where(x => x.id == request.Id && x.deleted == 0).FirstOrDefault();
            if (template != null)
            {
                template.SmsTemplateName = request.SmsTemplateName;
                template.SmsTemplateContent = request.SmsTemplateContent;
                template.updatetime = DateTime.Now;
                _context.SaveChanges();
            }
            return 1;
        }

        public int DeleteSmsTemplate(int templateId)
        {
            var template = _context.tx_smstemplatesetting.Where(x => x.id == templateId).FirstOrDefault();
            if (template != null)
            {
                template.deleted = 1;
                template.updatetime = DateTime.Now;
                return _context.SaveChanges();
            }
            return -1;
        }


        public tx_mobile_agent_relationship GetPhoneSetting(string mobile)
        {
            return _context.tx_mobile_agent_relationship.Where(x => x.mobile == mobile && x.deleted == 0).FirstOrDefault();
        }

        public tx_mobile_agent_relationship GetPhoneSetting(string mobile,string mobileCode)
        {
            return _context.tx_mobile_agent_relationship.Where(x => x.mobile == mobile && x.mobile_imei== mobileCode && x.deleted == 0).FirstOrDefault();
        }



        public List<tx_mobile_agent_relationship> GetPhoneSetting(int agentId)
        {
            return _context.tx_mobile_agent_relationship.Where(x => x.agentid == agentId && x.deleted == 0).OrderByDescending(x => x.id).ToList();
        }

        public List<tx_mobile_agent_relationship> GetPhoneSettingByIMEI(string IMEI)
        {
            return _context.tx_mobile_agent_relationship.Where(x => x.mobile_imei == IMEI && x.deleted == 0).OrderByDescending(x => x.id).ToList();
        }

        public int BindPhoneWithIMEI(string IMEI, string phoneNumber)
        {
            var existsPhone = _context.tx_mobile_agent_relationship.Where(x => x.mobile == phoneNumber && x.mobile_imei != null && x.mobile_imei.Trim() != string.Empty && x.deleted == 0).FirstOrDefault();
            if (existsPhone != null)
            {
                return -1;
            }
            var phone = _context.tx_mobile_agent_relationship.Where(x => x.mobile == phoneNumber && x.deleted == 0).FirstOrDefault();
            if (phone != null)
            {
                phone.mobile_imei = IMEI;
                phone.updatetime = DateTime.Now;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }
        public int UnbindPhone(int phoneId, bool delete)
        {
            var phone = _context.tx_mobile_agent_relationship.Where(x => x.id == phoneId).FirstOrDefault();
            if (phone != null)
            {
                if (delete)
                {
                    phone.deleted = 1;
                }
                else
                {
                    phone.mobile_imei = "";
                }
                phone.updatetime = DateTime.Now;
                return _context.SaveChanges();
            }
            return -1;
        }

        public int AddPhone(tx_mobile_agent_relationship phone)
        {
            var s = _context.tx_mobile_agent_relationship.Add(phone);
            _context.SaveChanges();
            return s.id;
        }

        public bool IsExistsPhone(string phoneNumber)
        {
            var phone = _context.tx_mobile_agent_relationship.Where(x => x.mobile == phoneNumber && x.deleted == 0).FirstOrDefault();
            if (phone != null)
            {
                return true;
            }
            return false;
        }

        public bool IsExistsSmsTemplateName(Expression<Func<tx_smstemplatesetting, bool>> predicate)
        {
            var template = _context.tx_smstemplatesetting.Where(predicate).FirstOrDefault();
            if (template != null)
            {
                return true;
            }
            return false;
        }

        public int AddOperationLog(tx_operation_log log)
        {
            var s = _context.tx_operation_log.Add(log);
            _context.SaveChanges();
            return s.id;
        }

        public bool IsBindMaxCount(int topAgentId)
        {
            var sql = string.Format(@"SELECT
	                                    a.sgc_phone_count > IFNULL(b.cnt, 0) IsCanBind
                                    FROM
	                                    (
		                                    SELECT
			                                    agent_id AgentId,
			                                    sgc_phone_count
		                                    FROM
			                                    bx_agent_setting
		                                    WHERE
			                                    agent_id = {0}
	                                    ) a
                                    LEFT JOIN (
	                                    SELECT
		                                    agentid AgentId,
		                                    count(1) cnt
	                                    FROM
		                                    tx_mobile_agent_relationship t
	                                    WHERE
		                                    t.agentid = {0}
	                                    AND deleted = 0
                                    ) b ON a.AgentId = b.AgentId", topAgentId);
            return _context.Database.SqlQuery<int>(sql).FirstOrDefault() == 1;
        }
    }
}
