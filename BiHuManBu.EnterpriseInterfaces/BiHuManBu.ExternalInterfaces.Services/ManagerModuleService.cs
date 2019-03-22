using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using ServiceStack.Logging;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class ManagerModuleService : IManagerModuleService
    {
        private IManagerModuleRepository _moduleRepository;
        private IManagerRoleModuleRelationRepository _moduleRelationRepository;
        private IManagerRoleRepository _managerRoleRepository;
        private IManagerModuleButtonRepository _managerModuleButtonRepository;
        private IManagerRoleButtonRelationRepository _managerRoleButtonRelationRepostory;
        private IManagerFunctionRepository _managerFunctionRepository;
        private IManagerFunctionRelationRepository _managerFunctionRelationRepository;
        
        private ILog logError = LogManager.GetLogger("ERROR");

        public ManagerModuleService(IManagerModuleRepository moduleRepository, IManagerRoleModuleRelationRepository moduleRelationRepository, IManagerRoleRepository managerRoleRepository, IManagerModuleButtonRepository managerModuleButtonRepository, IManagerRoleButtonRelationRepository managerRoleButtonRelationRepostory, IManagerFunctionRepository managerFunctionRepository
            , IManagerFunctionRelationRepository managerFunctionRelationRepository)
        {
            _moduleRepository = moduleRepository;
            _moduleRelationRepository = moduleRelationRepository;
            _managerRoleRepository = managerRoleRepository;
            _managerModuleButtonRepository = managerModuleButtonRepository;
            _managerRoleButtonRelationRepostory = managerRoleButtonRelationRepostory;
            _managerFunctionRelationRepository = managerFunctionRelationRepository;
            _managerFunctionRepository = managerFunctionRepository;
        }

     


        /// <summary>
        /// 展示菜单(添加、修改) zky 2017-08-03 /crm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public List<ManagerModuleViewModel> LoadManagerModule(int roleId = -1, int roleType = -1)
        {
            List<ManagerModuleViewModel> moduleTree = new List<ManagerModuleViewModel>();
            try
            {
                //所有角色都不需要展示的菜单
                var noDisplayList = new List<string>() { "insure_module", "statistical_report", "callstatistics_business", "account_statistic", "org_list", "RenewalSetting", "setting_jobNumber", "finance_management" };

                //助理roleType=5显示的菜单
                List<string> helperModule = new List<string> { "customer_module", "addQuotedPrice", "appoinment_list", "QuotationReceipt_List", "defeatReasonHistory", "customer_list" };

                //深圳人保agentId=102247、ManagerRoleId=10684 单独显示的菜单
                List<string> shenZhenRenBao = new List<string> { "statistics_analysisEntry", "statistics_analysisQuote", "statistics_analysisInsure", "customer_statistics" };
                List<int?> shenZhenAgentId = Array.ConvertAll<string, int?>(ConfigurationManager.AppSettings["sfGroupId"].Split(','), s => int.Parse(s)).ToList();

                //获取角色菜单
                var roleModule = roleId > 0 ? _moduleRelationRepository.GetList(t => t.role_id == roleId).ToList() : null;

                List<manager_module_db> allModule;
                if (roleType == 9)
                {
                    allModule = _moduleRepository.GetAllModule().Where(t =>( t.pater_code == "reconciliation_module" || t.pater_code == "financemanagement" || t.module_code == "financemanagement" || t.module_code == "reconciliation_module")&& t.module_name!="个人资金列表").OrderBy(t => t.order_by).ToList();
                }
                else
                {
                    //获取所有需要展示的菜单
                    allModule = _moduleRepository.GetAllModule().Where(t => t.module_type == 1 && t.module_code!= "financemanagement" && !noDisplayList.Contains(t.module_code)).OrderBy(t => t.order_by).ToList();
                }
                var shenZhenRenBaoRole = _managerRoleRepository.GetList(t => shenZhenAgentId.Contains(t.top_agent_id)).ToList<manager_role_db>();
                if (shenZhenRenBaoRole == null || !shenZhenRenBaoRole.Select(x => x.id).Contains(roleId))
                {
                    allModule = allModule.Where(t => !shenZhenRenBao.Contains(t.module_code)).ToList();
                }

                //获取所有父级菜单
                var parentModule = allModule.Where(t => t.pater_code == "system_all").ToList();

                //管理员和系统管理员类型
                List<int> typeList = new List<int> { 3, 4 };

                //系统管理员、管理员展示的菜单
                List<string> adminModule = new List<string>();
                if (roleId > 0)
                {
                    adminModule = new List<string>() { "authManager_module", "order_agent_list", "lattice_point" };
                }
                else
                {
                    //新建角色时放开部分菜单
                    adminModule = new List<string>() { "authManager_module", "lattice_point" };
                }

                //获取所有的子菜单
                var childrenModule = allModule.Where(t => t.pater_code != "system_all" && (typeList.Contains(roleType) || !adminModule.Contains(t.module_code))).ToList();

                //获取二级菜单的按钮列表
                var buttonList = _managerModuleButtonRepository.LoadAll().ToList();
                var btnRelation = _managerRoleButtonRelationRepostory.LoadAll().Where(t => t.role_id == roleId).Select(t => t.button_id).ToList();

                if (roleType == 5)
                {
                    parentModule = allModule.Where(t => t.module_code == "customer_module").ToList();
                    childrenModule = allModule.Where(t => helperModule.Contains(t.module_code)).ToList();
                }
                #region 特殊账号处理（不显示营销管理菜单）
                var sepcialAgentId = ConfigurationManager.AppSettings["SpecialAgentId"].Split(',').Select(t => Convert.ToInt32(t)).ToList();
                var roleList = _managerRoleRepository.GetList(r => sepcialAgentId.Contains(r.top_agent_id.Value)).Select(t => t.id).ToList();
                if (roleList.Contains(roleId))
                {
                    //去掉大菜单
                    parentModule = parentModule.Where(t => t.module_code != "operate_module").ToList();
                    //去掉营销管理下的子菜单
                    childrenModule = childrenModule.Where(t => t.pater_code != "operate_module").ToList();
                }
                #endregion


                //构建菜单树
                moduleTree = parentModule.Select(p => new ManagerModuleViewModel()
                {
                    roleId = roleId,
                    module_code = p.module_code,
                    text = p.module_name,
                    pater_code = p.pater_code,
                    buttonId = 0,
                    crm_module_type=p.crm_module_type,
                     orderby=p.order_by,
                    status = roleId > 0 ? GetRoleModuleStatus(roleModule, p.module_code) : "",
                    attrs = parentModule.Where(a => a.module_code == p.module_code).Select(a => new ManagerModuleViewModel()
                    {
                        module_code = a.module_code,
                        pater_code = a.pater_code,
                        status = roleId > 0 ? GetRoleModuleStatus(roleModule, p.module_code) : "",
                        roleId = roleId,
                        buttonId = 0,
                        orderby=a.order_by,
                        crm_module_type = a.crm_module_type
                    }).ToList(),
                    nodes = childrenModule.Where(c => c.pater_code == p.module_code).Select(c => new ManagerModuleViewModel()
                    {
                        roleId = roleId,
                        module_code = c.module_code,
                        text = c.module_name,
                        pater_code = c.pater_code,
                        buttonId = 0,
                        status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.module_code) : "",
                        crm_module_type = c.crm_module_type,
                         orderby=c.order_by,
                        attrs = childrenModule.Where(a => a.module_code == c.module_code).Select(a => new ManagerModuleViewModel()
                        {
                            module_code = c.module_code,
                            pater_code = c.pater_code,
                            status = roleId > 0 ? GetRoleModuleStatus(roleModule, c.module_code) : "",
                            roleId = roleId,
                            buttonId = 0,
                            crm_module_type = c.crm_module_type
                        }).ToList(),
                        state = "",
                        nodes = buttonList.Where(t => t.pater_module == c.module_code).Select(b => new ManagerModuleViewModel()
                        {
                            roleId = roleId,
                            module_code = b.action_url,
                            text = b.button_name,
                            pater_code = b.pater_module,
                            buttonId = b.id,
                            buttonCode = b.button_code,
                           
                            status = btnRelation.Contains(b.id) ? "modify" : "",
                       
                            attrs = new List<ManagerModuleViewModel>()
                            {
                                new ManagerModuleViewModel()
                                {
                                    module_code= b.action_url,
                                    text=b.button_name,
                                    pater_code=b.pater_module,
                                    status=btnRelation.Contains(b.id)?"modify":"",
                                    buttonId=b.id,
                                    buttonCode=b.button_code
                                 
                                }
                            },
                        }).ToList(),
                    }).ToList()
                }).ToList();


               
                foreach (var item in moduleTree)
                {
                    if (item.crm_module_type==2) {
                        item.listFunction = _managerFunctionRepository.GetByParentCode(item.module_code);
                        List<manager_role_function_relation> lrs = new List<manager_role_function_relation>();
                        lrs = _managerFunctionRelationRepository.Get(roleId);
                        item.SelectFuncCode = new List<string>();
                        foreach (var lr in lrs)
                        {
                            item.SelectFuncCode.Add(lr.function_code);
                        }
                    }
                    
                }

                











                if (roleType == 8)
                {
                    moduleTree.RemoveRange(1, 5);
                    moduleTree[0].nodes[1].nodes.RemoveAt(4);
                    moduleTree[0].nodes[1].nodes.RemoveRange(0, 3);
                    moduleTree[0].nodes.RemoveAt(6);
                    moduleTree[0].nodes.RemoveAt(3);

                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return moduleTree;
        }



      




        /// <summary>
        /// 获取菜单状态 zky 2017-08-10
        /// </summary>
        /// <param name="list"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetRoleModuleStatus(IList<manager_role_module_relation> list, string code)
        {
            return list.Where(t => t.module_code == code).FirstOrDefault() != null ? "modify" : "";
        }

        /// <summary>
        /// 刷库的方法（给系统管理员、管理员添加 三级菜单按钮的权限）zky 2017-12-21 /crm
        /// </summary>
        /// <returns></returns>
        public int ManagerAddButton()
        {
            return _managerRoleButtonRelationRepostory.ManagerAddButton();
        }

        public BaseViewModel AddEditModule(AddEditModuleRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            var result = false;

            var parentModule = _moduleRepository.GetList(t => t.module_code == request.paterCode).FirstOrDefault();

            manager_module_db module = request.saveType == "add" ? new manager_module_db() : _moduleRepository.GetList(t => t.module_code == request.moduleCode).FirstOrDefault();

            module.module_code = request.moduleCode;
            module.module_name = request.moduleName;
            module.pater_code = request.paterCode;
            module.action_url = request.actionUrl;
            module.order_by = request.orderby;
            module.module_level = 2;
            module.is_menu = 1;
            module.is_action = 1;
            module.module_status = request.status;
            module.module_type = parentModule.module_type;

            if (request.saveType == "add")
            {
                var allModuleList = _moduleRepository.GetAllModule();
                var existModuleCode = allModuleList.Where(t => t.module_code == request.moduleCode).FirstOrDefault();
                var existModuleName = allModuleList.Where(t => t.module_name == request.moduleName).FirstOrDefault();
                if (existModuleCode != null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "保存失败,module_code已存在";
                    return viewModel;
                }
                if (existModuleName != null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "保存失败,module_name已存在";
                    return viewModel;
                }

                module.creator_name = request.operateName;
                module.creator_full_name = request.operateName;
                module.creator_time = System.DateTime.Now;
                result = _moduleRepository.Add(module);

            }

            if (request.saveType == "edit")
            {
                module.modifi_name = request.operateName;
                module.modifi_full_name = request.operateName;
                module.modifi_time = System.DateTime.Now;
                result = _moduleRepository.Update(module);
            }

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

        public bool DeleteModule(string moduleCode)
        {
            return _moduleRepository.Delete(moduleCode);
        }
    }
}
