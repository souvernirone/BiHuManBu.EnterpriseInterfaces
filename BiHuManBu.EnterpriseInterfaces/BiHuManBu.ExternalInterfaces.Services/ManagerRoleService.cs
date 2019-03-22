using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Repository;
using System.Data.Entity;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class ManagerRoleService : IManagerRoleService
    {
        private ICacheHelper _cacheHelper;
        private readonly IManagerRoleRepository _managerRoleRepository;
        private readonly IManagerRoleModuleRelationRepository _roleModuleRelation;
        private readonly IAgentRepository _agentRepository;
        private readonly IManagerUserRoleRelationRepository _managerUserRoleRelationRepository;
        private readonly IManagerUserRepository _managerUserRepository;
        private readonly IManagerModuleRepository _managerModuleRepository;
        private readonly IManagerRoleModuleRelationRepository _managerModuleRelationRepository;
        private readonly IManagerFunctionRelationRepository _managerFunctionRelationRepository;

        private ILog logError = LogManager.GetLogger("ERROR");

        public ManagerRoleService(IManagerRoleRepository managerrole, ICacheHelper cacheHelper, IManagerUserRoleRelationRepository managerUserRoleRelationRepository, IAgentRepository agentRepository, IManagerUserRepository managerUserRepository
            , IManagerRoleModuleRelationRepository roleModuleRelation, IManagerModuleRepository managerModuleRepository, IManagerRoleModuleRelationRepository managerModuleRelationRepository, IManagerFunctionRelationRepository managerFunctionRelationRepository)
        {
            _managerRoleRepository = managerrole;
            _cacheHelper = cacheHelper;
            _roleModuleRelation = roleModuleRelation;
            _managerUserRoleRelationRepository = managerUserRoleRelationRepository;
            _agentRepository = agentRepository;
            _managerUserRepository = managerUserRepository;
            _managerModuleRepository = managerModuleRepository;
            _managerModuleRelationRepository = managerModuleRelationRepository;
            _managerFunctionRelationRepository = managerFunctionRelationRepository;
        }
        public ManagerRoleListViewModel GetManagerRoleInfo(int topAgentId)
        {
            ManagerRoleListViewModel viewModel = new ManagerRoleListViewModel();
            try
            {
                var list = _managerRoleRepository.GetManagerRoleInfo(topAgentId).Where(x => x.role_type != 3).Select(x => new ManagerRoleViewModel()
                {
                    Id = x.id,
                    RoleName = x.role_name
                }).ToList();
                viewModel.roleInfo = list;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }


        public string RoleExistByAgentId(int agentId)
        {
            return _managerRoleRepository.RoleExistByAgentId(agentId);



        }

        /// <summary>
        /// 根据顶级代理人Id获取角色列表 zky 2017-08-03 /crm
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public ManagerRoleListViewModel GetRoleList(int topAgentId)
        {
            ManagerRoleListViewModel viewModel = new ManagerRoleListViewModel();
            try
            {
                var list = _managerRoleRepository.GetManagerRoleInfo(topAgentId).Select(t => new ManagerRoleViewModel()
                {
                    Id = t.id,
                    RoleName = t.role_name,
                    CreatorName = t.creator_name,
                    CreateorTime = t.creator_time.HasValue ? Convert.ToDateTime(t.creator_time).ToString("yyyy-MM-dd HH:mm:ss") : "",
                    RoleType = t.role_type,
                    customOrder = 0,
                    isRequote = t.isRequote == null ? 2 : t.isRequote//1是,2否
                }).ToList();
                //按照 系统管理员、管理员、续保顾问、销售顾问、普通员工、助理  排序
                //var orderList = new List<int> { 3, 4, 2, 1, 0, 5, 1000 };
                foreach (var item in list)
                {
                    switch (item.RoleType)
                    {
                        case 3:
                            item.customOrder = 0;
                            break;
                        case 4:
                            item.customOrder = 1;
                            break;
                        case 2:
                            item.customOrder = 2;
                            break;
                        case 1:
                            item.customOrder = 3;
                            break;
                        case 0:
                            item.customOrder = 4;
                            break;
                        case 5:
                            item.customOrder = 5;
                            break;
                        case 1000:
                            item.customOrder = 6;
                            break;
                        default:
                            break;
                    }
                }

                viewModel.roleInfo = list.OrderBy(t => t.customOrder).ToList();
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务发生异常:" + ex.Message;
            }
            return viewModel;
        }

        /// <summary>
        /// 添加或修改角色 zky  2017-08-03 /crm
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        /// <param name="topAgentId"></param>
        /// <param name="agentName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public BaseViewModel AddOrUpdateRole(int roleId, string roleName, int topAgentId, string agentName, int? isRequote, List<ManagerModuleViewModel> list)
        {


            BaseViewModel viewModel = new BaseViewModel();
            bool result = false;
            var roleType = new List<int> { 0, 1, 2, 3, 4, 5 };
            var role = _managerRoleRepository.GetRoleInfo(roleId);

            #region 不能保存分配、批量续保的处理
            //获取list中添加的分配权限
            var buttonRecycle = list.Where(t => t.module_code == "customer_module").FirstOrDefault();
            if (buttonRecycle != null)
            {
                buttonRecycle=buttonRecycle.nodes.Where(c => c.module_code == "customer_list").FirstOrDefault().nodes.Where(t => t.buttonCode == "btn_recycle" && (t.status == "add" || t.status == "modify")).FirstOrDefault();

            }
            
            
            //查询当前角色下三级代理人的数量
            int agentCount = roleId > 0 ? _agentRepository.GetList(t => t.ManagerRoleId == roleId && t.agent_level == 3).Count : 0;
            if (agentCount > 0 && buttonRecycle != null)//如果角色有三级代理人，不能拥有分配权限
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "当前角色包含3级账号，不可添加分配或批量续保权限";
                return viewModel;
            }
            //获取list中添加的批量续保权限(排除删除)

            var batchRenewal = list.Where(t => t.module_code == "customer_module").FirstOrDefault();
            if (batchRenewal!=null) {
                batchRenewal= batchRenewal.nodes.Where(c => c.module_code == "batchRenewal_list" && (c.status == "add" || c.status == "modify")).FirstOrDefault();
            }
            if (agentCount > 0 && batchRenewal != null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "当前角色包含3级账号，不可添加分配或批量续保权限";
                return viewModel;
            }


            #endregion


            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    if (role != null)
                    {
                        if (!roleType.Contains(role.role_type.Value))//非系统默认角色才能修改角色名称
                        {
                            role.role_name = roleName;
                        }
                        role.modifi_name = agentName;
                        role.modifi_full_name = agentName;
                        role.modifi_time = System.DateTime.Now;
                        role.isRequote = isRequote;
                        //更新新角色信息
                        _managerRoleRepository.UpdateRole(role);
                    }
                    else
                    {
                        role = new manager_role_db();
                        role.role_name = roleName;
                        role.role_status = 0;
                        role.top_agent_id = topAgentId;
                        if (roleName == "驻店员")
                        {
                            role.role_type = 8;
                        }
                        else
                        {
                            role.role_type = 1000;
                        }

                        role.creator_name = agentName;
                        role.creator_full_name = agentName;
                        role.modifi_name = agentName;
                        role.modifi_full_name = agentName;
                        role.creator_time = System.DateTime.Now;
                        role.modifi_time = System.DateTime.Now;
                        role.isRequote = isRequote;
                        //添加角色
                        if (_managerRoleRepository.AddRole(role))
                        {
                            roleId = role.id;
                        }
                    }

                    if (list != null && list.Count > 0 && roleId != -1)
                    {//删除当前角色所有的菜单
                     //if (list[0].crm_module_type == 1)
                     //{
                     //    _roleModuleRelation.DeleteRoleRelation(roleId);
                     //}
                     //else {
                     //    _roleModuleRelation.DeleteRoleRelation(roleId,2);
                     //}

                        //if (list[0].crm_module_type == 1)
                        //{

                        //    //删除角色所有的权限按钮 2017-11-29新需求
                        //    _roleModuleRelation.DeleteRoleButton(roleId);
                        //}
                        //else {
                        //    _roleModuleRelation.DeleteRoleButton(roleId,2);
                        //}
                        _roleModuleRelation.DeleteRoleRelation(roleId);
                        _roleModuleRelation.DeleteRoleButton(roleId);
                        //给当前角色添加菜单列表
                        _roleModuleRelation.AddRoleRelationList(roleId, agentName, list);
                    }




                    if (list != null)
                    {
                        _managerFunctionRelationRepository.Delete(role.id);
                        foreach (var item in list)
                        {
                            if (item.crm_module_type == 2)
                            {
                                if (item.listFunction!=null) {
                                    foreach (var fucn in item.listFunction)
                                    {
                                        manager_role_function_relation model = new manager_role_function_relation();
                                        model.role_id = role.id; model.operator_name = agentName; model.operator_time = DateTime.Now;
                                        model.function_code = fucn.function_code;
                                        _managerFunctionRelationRepository.Add(model);
                                    }
                                }
                                
                            }


                        }
                    }






                    ts.Complete();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                Transaction.Current.Rollback();
            }
            if (result)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "操作成功"; ;
            }
            else
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "操作失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel DeleteRole(DeleteRoleRequest request)
        {
            var role = _managerRoleRepository.Find(o => o.id == request.RoleId && o.top_agent_id == request.Agent);
            if (role == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "角色不存在");

            var forbidDelete = new int[] { 0, 1, 2, 3, 4, 5 };
            if (role.role_type.HasValue && forbidDelete.Contains(role.role_type.Value))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "默认角色不能删除");
            }

            var isUse = _agentRepository.IsExistManagerRoleId(request.RoleId);
            var newRoleId = 0;
            if (isUse)
            {
                var ordinaryRole = _managerRoleRepository.Find(o => o.top_agent_id == request.Agent && o.role_type == 0);
                if (ordinaryRole == null)
                {
                    logError.Error("TopAgentId=" + request.Agent + "的顶级代理人不存在普通员工角色");
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.DataError, "当前顶级代理人下不存在普通员工角色");
                }
                newRoleId = ordinaryRole.id;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //删角色
                    _managerRoleRepository.DeleteRole(role);
                    //删角色的菜单
                    _roleModuleRelation.DeleteRoleRelation(role.id);
                    if (isUse)
                    {
                        _agentRepository.UpdateRoleId(request.RoleId, newRoleId);
                        _managerUserRepository.UpdateRoleId(request.RoleId, newRoleId);
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.SystemError);
            }

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        /// <summary>
        /// 给顶级代理人添加助理角色（新版上线后注册顶级代理时会添加，老的数据没有助理角色） 
        /// zky 2017-09-01 /crm
        /// </summary>
        /// <param name="roleSuccess">成功添加助理角色的数量</param>
        /// <param name="moduleSuccess">成功添加菜单的数量</param>
        /// <param name="testCount"></param>
        /// <returns></returns>
        public void OldAgentAddHepler(int testCount, out int roleSuccess, out int moduleSuccess)
        {
            roleSuccess = 0;
            moduleSuccess = 0;

            IList<bx_agent> oldAgent;

            testCount = testCount > 10 ? 10 : testCount;
            //获取没有助理角色的顶级代理
            //oldAgent = _agentRepository.GetNoHelperTopAgent().OrderBy(t => t.Id).Take(testCount).ToList();
            oldAgent = _agentRepository.GetNoHelperTopAgent();
            if (oldAgent.Count > 0)
            {
                try
                {

                    //所有的父级菜单
                    string[] moduleAll = ConfigurationManager.AppSettings["moduleAll"].Split(',');
                    //客户管理的子菜单
                    string[] customerModule = ConfigurationManager.AppSettings["customer_module"].Split(',');

                    foreach (var agent in oldAgent)
                    {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            manager_role_db role = new manager_role_db();
                            role.role_name = "助理";
                            role.role_status = 0;
                            role.creator_name = agent.AgentName;
                            role.creator_time = DateTime.Now;
                            role.creator_full_name = agent.AgentName;
                            role.top_agent_id = agent.Id;
                            role.modifi_full_name = agent.AgentName;
                            role.modifi_name = agent.AgentName;
                            role.modifi_time = DateTime.Now;
                            role.role_type = 5;
                            DataContextFactory.GetDataContext().manager_role_db.Add(role);
                            int addRole = DataContextFactory.GetDataContext().SaveChanges();

                            if (addRole > 0)
                            {
                                roleSuccess++;

                                //给当前角色添加菜单
                                //先添加父级菜单（客户管理）
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = role.id,
                                    module_code = moduleAll[6].Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                                moduleSuccess++;
                                //添加子菜单
                                foreach (var str in customerModule)
                                {
                                    if (str != "camera_list" && str != "batchRenewal_list" && str != "customer_recyclelist" && str != "customer_checklist")
                                    {
                                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                        {
                                            role_id = role.id,
                                            module_code = str.Trim(),
                                            creator_full_name = "lzllzl",
                                            creator_name = "lzl",
                                            creator_time = DateTime.Now
                                        });
                                        moduleSuccess++;
                                    }
                                }
                                int addModule = DataContextFactory.GetDataContext().SaveChanges();
                            }

                            ts.Complete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    Transaction.Current.Rollback();
                }
            }
        }

        /// <summary>
        /// 批量更新manageruser的managerRoleId  zky 2017-08-31 /crm
        /// </summary>
        /// <param name="names">名称</param>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool UpdateRoleIdByName(List<string> names, int roleId)
        {
            return _managerUserRepository.UpdateRoleIdByName(names, roleId);
        }

        /// <summary>
        /// 获取顶级代理人下面拥有某个三级按钮权限的角色列表
        /// </summary>
        /// <param name="btnCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public IList<string> GetRoleListByBtnCode(string btnCode, int topAgentId)
        {
            return _managerRoleRepository.GetRoleListByBtnCode(btnCode, topAgentId);
        }

        /// <summary>
        /// 获取顶级代理人下面拥有某个二级菜单权限的角色列表
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public IList<string> GetRoleListByModule(string moduleCode, int topAgentId)
        {
            return _managerRoleRepository.GetRoleListByModule(moduleCode, topAgentId);
        }

        /// <summary>
        /// 保存代报价         
        /// </summary>
        /// <param name="managerRoleId"></param>
        /// <param name="isRequote"></param>
        /// <returns></returns>
        public BaseViewModel UpdateRequoteById(UpdateRoleRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            try
            {
                bool result = _managerRoleRepository.UpdateRequoteById(request.RoleId, request.IsRequote);
                viewModel.StatusMessage = result ? "保存成功" : "保存失败";
                viewModel.BusinessStatus = result ? 1 : 0;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常：" + ex);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器异常";
            }
            return viewModel;
        }





    }
}
