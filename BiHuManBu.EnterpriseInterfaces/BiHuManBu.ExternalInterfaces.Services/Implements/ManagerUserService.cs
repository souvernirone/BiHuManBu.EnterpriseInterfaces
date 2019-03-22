using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using log4net;
using ServiceStack.Text;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// manageruser逻辑
    /// </summary>
    public class ManagerUserService : IManagerUserService
    {
        private readonly IManagerRoleRepository _managerRoleRepository;
        private readonly IManagerRoleModuleRelationRepository _managerRoleModuleRelationRepository;
        private readonly IManagerUserRepository _managerUserRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IManagerModuleRepository _moduleRepository;

        private readonly IManagerUserRoleRelationService _managerUserRoleRelationService;
        private ILog logError = LogManager.GetLogger("ERROR");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="managerRoleRepository"></param>
        /// <param name="managerRoleModuleRelationRepository"></param>
        /// <param name="managerUserRepository"></param>
        /// <param name="agentRepository"></param>
        /// <param name="managerUserRoleRelationService"></param>
        public ManagerUserService(IManagerRoleRepository managerRoleRepository, IManagerRoleModuleRelationRepository managerRoleModuleRelationRepository, IManagerUserRepository managerUserRepository, IAgentRepository agentRepository, IManagerUserRoleRelationService managerUserRoleRelationService, IManagerModuleRepository moduleRepository)
        {
            _managerRoleRepository = managerRoleRepository;
            _managerRoleModuleRelationRepository = managerRoleModuleRelationRepository;
            _managerUserRepository = managerUserRepository;
            _agentRepository = agentRepository;
            _managerUserRoleRelationService = managerUserRoleRelationService;
            _moduleRepository = moduleRepository;
        }

        /// <summary>
        /// 更新经纪人平台账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel UpdateManagerUserAccount(UpdateManagerUserAccountRequest request)
        {
            bx_agent agent = _agentRepository.GetAgent(request.AgentId);
            if (agent == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "代理人账号不正确");

            if (request.Source == "CRM")
            {
                //验证手机号
                if (!FormatHelper.IsMobile(request.Mobile))
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "手机号格式不正确");
                }

                bool exsitMobile = false;
                if (agent.IsDaiLi == 1)//顶级全局验证
                {
                    exsitMobile = _agentRepository.GetList(t => t.Mobile == request.Mobile && t.Id != request.AgentId).FirstOrDefault() != null;
                }
                else//下级验证当前代理人下面是否存在
                {
                    exsitMobile = _agentRepository.GetList(t => t.Mobile == request.Mobile && t.Id != request.AgentId&&t.TopAgentId==agent.TopAgentId).FirstOrDefault() != null;
                }

                if (exsitMobile)
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "手机号已存在");
                }
            }

            var roleId = 0;
            //为了避免用户刚添加了机器人账号，又修改机器人账号的问题
            if (request.EditType == "Add" && !string.IsNullOrEmpty(agent.AgentAccount))
            {
                request.EditType = "Update";
            }

            //判断账号是否存在
            bool isCheckExist = true;
            if (request.EditType == "Update")
            {
                if (string.IsNullOrEmpty(agent.AgentAccount))
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "参数错误");

                if (agent.AgentAccount.ToLower() == request.AccountName.ToLower())
                    isCheckExist = false;
            }
            if (isCheckExist)
            {
                if (_managerUserRepository.IsExist(o => o.Name == request.AccountName))
                {
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "用户名已存在");
                }
            }

            roleId = agent.ManagerRoleId;
            if (roleId == 0)
            {
                //从manager_role_db中找见当前代理人的顶级代理人的role_type=0的id
                manager_role_db role = _managerRoleRepository.Find(o => o.top_agent_id == request.Agent && o.role_type == 0);
                if (role == null)
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "顶级代理人的角色不存在");
                roleId = role.id;
            }

            //判断是修改还是添加
            switch (request.EditType)
            {
                case "Add":
                    return AddManageUser(request, agent, roleId);
                case "Update":
                    return UpdateManagerUser(request, agent, roleId);
                default:
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "EditType不正确");
            }
        }

        /// <summary>
        /// 代理人存在，机器人账号不存在时添加机器人账号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="agent"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private BaseViewModel AddManageUser(UpdateManagerUserAccountRequest request, bx_agent agent, int roleId)
        {
            if (string.IsNullOrWhiteSpace(request.AccountPassword))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "在添加机器人账号时AccountPassword不能为空");
            }
            //判断当前代理人是否已经有机器人账号
            if (!string.IsNullOrEmpty(agent.AgentAccount))
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "代理人的机器用户名已存在");

            var now = DateTime.Now;

            //添加manageruser
            manageruser user = new manageruser
            {
                Name = request.AccountName,
                PwdMd5 = request.AccountPassword,
                Mobile = request.Mobile,
                AccountType = 1,
                CreateTime = now.ToString(),
                ManagerRoleId = roleId,
                OperatorId = request.OperatorId,
                OperatorName = request.OperatorName,
                OperatorTime = now
            };

            //修改bx_agent
            SetAgent(agent, request);

            using (TransactionScope tran = new TransactionScope())
            {
                _managerUserRepository.AddManagerUser(user);

                //修改managerUserRoleRelation
                _managerUserRoleRelationService.UpdateManagerUserToUpdateUserRoleRelation(request.AccountName, request.OperatorName, roleId);

                //修改agent
                _agentRepository.UpdateAgent(agent);
                try
                {
                    tran.Complete();
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
                }
                catch (Exception ex)
                {
                    logError.Error("ManagerUserService->AddManageUser方法执行失败，参数：" + request.ToJson() + "，错误信息:" + ex.Message + "/n" + ex.Source + "/n" + ex.StackTrace + "/n" + ex.InnerException);
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
                }
            }
        }

        /// <summary>
        /// 机器人账号存在时更新机器人账号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="agent"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private BaseViewModel UpdateManagerUser(UpdateManagerUserAccountRequest request, bx_agent agent, int roleId)
        {
            var now = DateTime.Now;
            manageruser user = _managerUserRepository.Find(o => o.Name == agent.AgentAccount);
            if (user == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "机器人账号不存在");
            user.Name = request.AccountName;
            if (!string.IsNullOrWhiteSpace(request.AccountPassword))
                user.PwdMd5 = request.AccountPassword;
            user.Mobile = request.Mobile;
            user.OperatorId = request.OperatorId;
            user.OperatorName = request.OperatorName;
            user.OperatorTime = now;
            if (!string.IsNullOrEmpty(request.ChargePerson))
            {
                agent.charge_person = request.ChargePerson;
            }

            SetAgent(agent, request);

            using (TransactionScope tran = new TransactionScope())
            {
                _managerUserRepository.Update(user);
                //修改managerUserRoleRelation
                _managerUserRoleRelationService.UpdateManagerUserToUpdateUserRoleRelation(request.AccountName, request.OperatorName, roleId);

                //修改agent
                _agentRepository.UpdateAgent(agent);

                //禁用业务员，推送消息到前端
                if (agent.IsUsed==2)
                {
                    //禁用代理人是推送消息
                    _agentRepository.PushSignal(request.AgentId, 2);
                }

                try
                {
                    tran.Complete();
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
                }
                catch (Exception ex)
                {
                    logError.Error("ManagerUserService->AddManageUser方法执行失败，参数：" + request.ToJson() + "，错误信息:" + ex.Message + "/n" + ex.Source + "/n" + ex.StackTrace + "/n" + ex.InnerException);
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
                }
            }
        }

        /// <summary>
        /// 修改agent
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="request"></param>
        private void SetAgent(bx_agent agent, UpdateManagerUserAccountRequest request)
        {
            agent.AgentAccount = request.AccountName;
            agent.AgentName = request.AgentName;
            agent.Mobile = request.Mobile;
            if (request.Source == "Manager")
            {
                agent.IsUsed = request.IsUsed;
            }
        }

        /// <summary>
        /// 用户列表查询 zky 2017-08-31 /运营后台
        /// </summary>
        /// <param name="accountType">0运营后台用户，1CRM用户</param>
        /// <param name="mobile">手机号</param>
        /// <param name="account">用户名</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<ManageruserViewModel> GetManageruserList(int accountType, string mobile, string account, int pageSize, int pageIndex, out int total)
        {
            return _managerUserRepository.GetManageruserList(accountType, mobile, account, pageSize, pageIndex, out total);
        }

        /// <summary>
        /// 查询菜单列表
        /// </summary>
        /// <param name="platform">0运营后台菜单 1CRM菜单</param>
        /// <returns></returns>
        public List<ManagerModuleViewModel> GetModuleList(int platform)
        {
            //platform 区分CRM菜单 运营后台菜单
            //var allModule = _moduleRepository.GetAllModule().Where(t => (platform == 0 || t.module_type == 1) && (platform == 1 || t.module_type == null));
            var allModule = _moduleRepository.GetAllModule();

            var parentModule = allModule.Where(t => t.pater_code == "system_all");
            var childrenModule = allModule.Where(t => t.pater_code != "system_all" && t.module_code != "system_all");
            List<ManagerModuleViewModel> moduleTree = new List<ManagerModuleViewModel>();

            moduleTree = parentModule.Select(p => new ManagerModuleViewModel()
            {
                module_code = p.module_code,
                text = p.module_name,
                pater_code = p.pater_code,
                action_url = p.action_url,
                orderby = p.order_by.Value,
                moduleType = p.module_type,
                status = p.module_status.ToString(),
                nodes = childrenModule.Where(t => t.pater_code == p.module_code).Select(c => new ManagerModuleViewModel()
                {
                    module_code = c.module_code,
                    text = c.module_name,
                    pater_code = c.pater_code,
                    action_url = c.action_url,
                    orderby = c.order_by.Value,
                    moduleType = p.module_type,
                    status = c.module_status.ToString(),
                }).ToList(),
            }).ToList();

            return moduleTree;
        }
    }
}
