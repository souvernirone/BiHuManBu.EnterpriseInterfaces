using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class GroupAuthenService : IGroupAuthenService
    {
        private readonly IGroupAuthenRepository _groupAuthenRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IManagerUserRepository _manageruserRepository;
        private readonly IUpdateCompleteTaskService _updateCompleteTaskService;
        private string zcTopAgentId = ConfigurationManager.AppSettings["autoOpenUsedId"].ToString();

        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("INFO");

        public GroupAuthenService(IGroupAuthenRepository groupAuthenRepository, IAgentRepository agentRepository, IManagerUserRepository manageruserRepository, IUpdateCompleteTaskService updateCompleteTaskService)
        {
            _groupAuthenRepository = groupAuthenRepository;
            _agentRepository = agentRepository;
            _manageruserRepository = manageruserRepository;
            _updateCompleteTaskService = updateCompleteTaskService;
        }

        /// <summary>
        /// 根据id获取实体对象 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupAuthenModel GetModel(int id)
        {
            var authenModel = _groupAuthenRepository.GetModel(id);
            if (authenModel != null)
            {
                var agent = _agentRepository.GetAgent(authenModel.AgentId);
                authenModel.AgentAddress = agent != null ? agent.AgentAddress : "";
                authenModel.AgentName = agent != null ? agent.AgentName : "";
                authenModel.ChargePeson = agent != null ? agent.charge_person : "";
            }
            return authenModel;
        }

        /// <summary>
        /// 2108-09-19 张克亮 小V盟项目时加入 根据经济人查询头像及证件信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetGroupAuthen(int agentId)
        {
            BaseViewModel baseView = new BaseViewModel();
            try
            {
                //查询信息
                var groupAuthen = _groupAuthenRepository.GetByAgentId(agentId);
                if (groupAuthen!=null)
                {
                    //封装赋值结果
                    baseView.BusinessStatus = 1;
                    baseView.StatusMessage = "查询成功";
                    GroupAuthenModel gaModel = new GroupAuthenModel();
                    gaModel.Id = groupAuthen.id;
                    gaModel.AgentId = groupAuthen.agentId;
                    gaModel.AuthenState = groupAuthen.authen_state.Value;
                    gaModel.CardHolder = groupAuthen.cardholder ?? "";
                    gaModel.CardId = groupAuthen.card_id ?? "";
                    gaModel.BankCardNum = groupAuthen.bank_card_number ?? "";
                    gaModel.BankName = groupAuthen.bank_name ?? "";
                    gaModel.ChildBankName = groupAuthen.bank_son_name ?? "";
                    gaModel.BusinessLicenceUrl = groupAuthen.business_licence_url ?? "";
                    gaModel.CardFaceUrl = groupAuthen.card_face_url ?? "";
                    gaModel.CardReverseUrl = groupAuthen.card_reverse_url ?? "";
                    gaModel.FieldUrl = groupAuthen.field_url ?? "";
                    gaModel.BankcardFaceUrl = groupAuthen.bankcard_face_url ?? "";
                    gaModel.BankcardReverseUrl = groupAuthen.bankcard_reverse_url;
                    gaModel.BankId = groupAuthen.bank_id ?? 0;
                    gaModel.HeadPortrait = groupAuthen.head_portrait ?? "";
                    gaModel.Nickname = groupAuthen.nickname ?? "";
                    gaModel.IsCompleteTask = groupAuthen.is_complete_task;
                    baseView.Data = gaModel;
                }
                else
                {
                    baseView.BusinessStatus = 0;
                    baseView.StatusMessage = "没有查询到相关信息";
                }

            }
            catch (Exception ex)
            {
                baseView.BusinessStatus = -999;
                baseView.StatusMessage = "查询出现异常";
                logError.Error("GroupAuthenService-GetGroupAuthen，入参{agentId="+ agentId .ToString()+ "}，异常信息:"+ex.Message);
            }
            return baseView;
        }

        /// <summary>
        /// 更新实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel UpdateModel(UpdateGroupAuthenRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var authen = _groupAuthenRepository.Get(request.Id);
            var agentModel = _agentRepository.GetAgent(request.AgentId);
            if (agentModel == null || authen == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败，认证信息不存在";
                return viewModel;
            }

            authen.id = request.Id;
            authen.authen_state = request.AuthenState;
            authen.card_id = request.CardId;
            authen.bank_card_number = request.BankCardNum;
            authen.card_face_url = request.CardFaceUrl;
            authen.card_reverse_url = request.CardReverseUrl;
            authen.bankcard_face_url = request.BankcardFaceUrl;
            authen.bankcard_reverse_url = request.BankcardReverseUrl;
            authen.bank_id = request.BankId;
            authen.modify_time = DateTime.Now;
            //2018-09-18 张克亮 加入更新头像昵称信息
            authen.head_portrait = request.HeadPortrait;
            authen.nickname = request.Nickname;

            if (zcTopAgentId.Contains(agentModel.TopAgentId.ToString()))
            {
                authen.cardholder = request.CardHolder;
            }
            if (agentModel.zhen_bang_type == 1 || agentModel.zhen_bang_type == 2)//网点和机构认证
            {
                if (agentModel.RegType == 1)
                {
                    authen.field_url = request.FieldUrl;
                    authen.business_licence_url = request.BusinessLicenceUrl;
                    agentModel.AgentAddress = request.AgentAddress;//认证信息单位地址
                }
                _agentRepository.UpdateAgent(agentModel);
            }

            if (_groupAuthenRepository.UpdateModel(authen))
            {
                AuthenPassCanCreateTeam(agentModel, request.AuthenState);
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 添加实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel AddModel(UpdateGroupAuthenRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var authen = _groupAuthenRepository.GetList(t => t.agentId == request.AgentId).FirstOrDefault();
            var agentModel = _agentRepository.GetAgent(request.AgentId);
            if (agentModel == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
                return viewModel;
            }
            if (authen != null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "认证信息已存在";
                return viewModel;
            }

            authen = new bx_group_authen();
            authen.agentId = request.AgentId;
            authen.authen_state = request.AuthenState;
            authen.card_id = request.CardId;
            authen.bank_card_number = request.BankCardNum;
            authen.business_licence_url = request.BusinessLicenceUrl;
            authen.card_face_url = request.CardFaceUrl;
            authen.card_reverse_url = request.CardReverseUrl;
            authen.bankcard_face_url = request.BankcardFaceUrl;
            authen.bankcard_reverse_url = request.BankcardReverseUrl;
            authen.bank_id = request.BankId;
            authen.create_time = DateTime.Now;
            //2018-09-18 张克亮 加入头像昵称信息
            authen.head_portrait = request.HeadPortrait;
            authen.nickname = request.Nickname;

            if (zcTopAgentId.Contains(agentModel.TopAgentId.ToString()))
            {
                authen.cardholder = request.CardHolder;
            }

            if (agentModel.zhen_bang_type == 1 || agentModel.zhen_bang_type == 2)//网点和机构认证
            {
                authen.field_url = request.FieldUrl;
                authen.business_licence_url = request.BusinessLicenceUrl;

                agentModel.AgentAddress = request.AgentAddress;//认证信息单位地址
                _agentRepository.UpdateAgent(agentModel);
            }

            if (_groupAuthenRepository.AddModel(authen))
            {
                AuthenPassCanCreateTeam(agentModel, request.AuthenState);

                viewModel.Data = authen.id;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
            }
            return viewModel;

        }

        public BaseViewModel ZhenBangUpdate(int TopAgentId, int AgentId, int IsUsed, int IsSubmit, int IsShowCalc, int MessagePayType, int RoleId, int ZhenBangType, string UpdateFrom, int ParentShareCode)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var curAgent = _agentRepository.GetAgent(AgentId);
            if (curAgent == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败，用户信息不存在";
                return viewModel;
            }
            if (curAgent.TopAgentId != TopAgentId)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败，当前用户不属于该机构";
                return viewModel;
            }

            curAgent.IsShowCalc = IsShowCalc;
            curAgent.IsUsed = IsUsed;
            curAgent.MessagePayType = MessagePayType;
            curAgent.IsSubmit = IsSubmit;
            if (UpdateFrom == "User")
            {
                var parentAgent = _agentRepository.GetAgent(ParentShareCode - 1000);
                curAgent.ManagerRoleId = RoleId;
                if (parentAgent.zhen_bang_type == 2 && curAgent.zhen_bang_type == 4)
                {
                    //当外部代理变更到网点下面时类型转成内部员工
                    curAgent.zhen_bang_type = 3;
                }
                else
                {
                    curAgent.zhen_bang_type = ZhenBangType;
                }
                
                //外部代理修改上级邀请码
                if (curAgent.ParentShareCode != ParentShareCode.ToString())
                {
                    curAgent.ParentShareCode = ParentShareCode.ToString();
                    curAgent.ParentAgent = parentAgent.Id;
                    curAgent.agent_level = parentAgent.agent_level + 1;
                }
            }
            else if (UpdateFrom == "Site")
            {
                curAgent.ManagerRoleId = RoleId;
            }
            else if (UpdateFrom == "Woker")
            {
                var parentAgent = _agentRepository.GetAgent(ParentShareCode - 1000);
                curAgent.zhen_bang_type = ZhenBangType;

                //内部员工修改上级邀请码
                if (curAgent.ParentShareCode != ParentShareCode.ToString())
                {
                    curAgent.ParentShareCode = ParentShareCode.ToString();
                    curAgent.ParentAgent = parentAgent.Id;
                    curAgent.agent_level = parentAgent.agent_level + 1;
                }
            }

            var result = _agentRepository.UpdateAgent(curAgent);
            if (result)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
                return viewModel;
            }
            viewModel.BusinessStatus = 0;
            viewModel.StatusMessage = "保存失败";
            return viewModel;
        }

        /// <summary>
        /// 查询所有的银行 zky 2018-1-2
        /// </summary>
        /// <returns></returns>
        public IList<BankModel> GetAllBank()
        {
            return _groupAuthenRepository.GetAllBank().Select(t => new BankModel
            {
                BankId = t.id,
                BankName = t.bank_name
            }).ToList();
        }

        /// <summary>
        /// 更新机构账号信息 zky 2018-1-2 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel UpdateOrgInfo(SaveOrgInfoRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            bool result = false;

            var agentItem = _agentRepository.GetAgent(request.OrgId);
            if (agentItem == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "机构信息不存在";
                return viewModel;
            }
            var manageruserItem = _manageruserRepository.Find(t => t.Name == agentItem.AgentAccount);
            if (manageruserItem == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "账号信息不存在";
                return viewModel;
            }

            if (request.Account != manageruserItem.Name)//修改了用户名，判断新的用户名是否存在
            {
                var userInfo = _manageruserRepository.Find(t => t.Name == request.Account);
                if (userInfo != null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "账号已存在";
                    return viewModel;
                }
            }
            if (request.Mobile != agentItem.Mobile)
            {
                var exsitMobile = false;
                if (agentItem.IsDaiLi == 1)
                {
                    exsitMobile = _agentRepository.GetList(t => t.Mobile == request.Mobile).FirstOrDefault() != null;
                }
                else
                {
                    exsitMobile = _agentRepository.GetList(t => t.Mobile == request.Mobile && t.TopAgentId == agentItem.TopAgentId).FirstOrDefault() != null;
                }
                if (exsitMobile)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "手机号已存在";
                    return viewModel;
                }
            }

            using (TransactionScope ts = new TransactionScope())
            {
                agentItem.AgentName = request.OrgName;
                agentItem.charge_person = request.ChargePerson;
                agentItem.Mobile = request.Mobile;
                agentItem.AgentAccount = request.Account;
                if (!string.IsNullOrEmpty(request.PassWord))
                {
                    agentItem.AgentPassWord = CommonHelper.GetMd5(request.PassWord);
                    manageruserItem.PwdMd5 = CommonHelper.GetMd5(request.PassWord);
                }
                _agentRepository.UpdateAgent(agentItem);

                manageruserItem.Name = request.Account;
                manageruserItem.Mobile = request.Mobile;
                _manageruserRepository.Update(manageruserItem);

                ts.Complete();
                result = true;
            }

            if (result)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }

            return viewModel;
        }

        /// <summary>
        /// 工号管理列表 zky 2018-1-2/crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="ukeyName"></param>
        /// <param name="cityName"></param>
        /// <param name="orgName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<JobNumberDto> JobNumberList(int agentId, string ukeyName, string cityName, string orgName, int pageIndex, int pageSize, int cityId, int ukeyId, int groupId, out int total)
        {
            return _groupAuthenRepository.JobNumberList(agentId, ukeyName, cityName, orgName, pageIndex, pageSize, cityId, ukeyId, groupId, out total);
        }

        public JobNumberListViewModel CityInfoAndUkeyInfo(int agentId)
        {
            JobNumberListViewModel viewModel = new JobNumberListViewModel();
            var list = _groupAuthenRepository.CityInfoAndUkeyInfo(agentId);
            viewModel.CityList = viewModel.CityList = list.GroupBy(t => new { t.CityId, t.CityName }).Select(g => new JobNumberDto
            {
                CityId = g.Key.CityId,
                CityName = g.Key.CityName
            }).ToList();
            viewModel.UkeyList = list.Select(t => new JobNumberDto
            {
                UkeyName = t.UkeyName,
                UkeyId = t.UkeyId
            }).ToList();
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel;
        }

        /// <summary>
        /// 认证审核不通过 zky 2018-2-4/crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel AuditFailed(AuditFailedRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var authenModel = _groupAuthenRepository.GetList(t => t.id == request.AuthenId).FirstOrDefault();
            if (authenModel == null || authenModel.agentId != request.AgentId)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "认证信息错误";
                return viewModel;
            }

            authenModel.audit_time = DateTime.Now;
            authenModel.audit_remark = request.AuditRemark;
            authenModel.authen_state = 2;
            authenModel.audit_user = request.AuditUserId.ToString();

            var result = _groupAuthenRepository.UpdateModel(authenModel);
            if (result)
            {
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "保存成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "保存失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 认证通过后检查是否可以建团
        /// </summary>
        /// <param name="agentItem"></param>
        /// <param name="authenState"></param>
        public void AuthenPassCanCreateTeam(bx_agent agentItem, int authenState)
        {
            if (authenState == 1)
            {
                _updateCompleteTaskService.UpdateCompleteTask(agentItem.Id);
            }
        }
    }
}
