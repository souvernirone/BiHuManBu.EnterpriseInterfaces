using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Transactions;
using System.Text;
using Newtonsoft.Json;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerRoleModuleRelationRepository : IManagerRoleModuleRelationRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");

        private EntityContext db = DataContextFactory.GetDataContext();

       private bool AddLiPeiModuleRelation(int roleId,string operatorName)
        {
            //9,19,21,23,25
            //17,9,11,13,15

            string sql1 = string.Format("INSERT INTO  manager_role_module_relation (role_id,module_code,creator_name,creator_full_name,creator_time)  values ({0},'TuiXiu_Manager','{1}','{1}',NOW()),({0},'Accident_Car_Clue','{1}','{1}',NOW()),({0},'Clue_Statistics','{1}','{1}',NOW()),({0},'SMS_Template_Set','{1}','{1}',NOW());", roleId, operatorName);
            string sql2 = string.Empty;
            if (ConfigurationManager.AppSettings["TXRoleButton"] == "online")
            {
                 sql2 = string.Format("INSERT INTO  manager_role_button_relation (role_id,button_id,module_code,creator_name,creator_time) values ({0},17,'OutTimeSet','{1}',NOW()),({0},9,'RecivedClueDistributeSet','{1}',NOW()),({0},11,'RecivedPeopleDistributeSet','{1}',NOW()),({0},13,'ExportExcelFunction','{1}',NOW()),({0},15,'AccidentCarMobileSet','{1}',NOW());", roleId, operatorName);
            }
            else {
                 sql2 = string.Format("INSERT INTO  manager_role_button_relation (role_id,button_id,module_code,creator_name,creator_time) values ({0},9,'OutTimeSet','{1}',NOW()),({0},19,'RecivedClueDistributeSet','{1}',NOW()),({0},21,'RecivedPeopleDistributeSet','{1}',NOW()),({0},23,'ExportExcelFunction','{1}',NOW()),({0},25,'AccidentCarMobileSet','{1}',NOW());", roleId, operatorName);
            }

            string sql3 = string.Format("INSERT INTO   manager_role_function_relation (role_id,function_code,operator_name,operator_time) values ({0},'reciveclue','{1}',NOW()),({0},'dtd_connection','{1}',NOW()),({0},'reception_notice','{1}',NOW())", roleId, operatorName);
            logInfo.Info(sql3);
            string sql = sql1 + sql2 + sql3;
            return db.Database.ExecuteSqlCommand(sql) > 0;
        }




        /// <summary>
        /// 角色跟模块关系
        /// </summary>
        /// <param name="role"></param>
        /// <param name="name"></param>
        public void AddRoleModuleRelation(List<manager_role_db> role, string name, int regType, int zhenBangType)
        {
            try
            {

                string[] moduleAll = ConfigurationManager.AppSettings["moduleAll"].Split(',');

                //续保
                //string[] xubao_module = ConfigurationManager.AppSettings["xubao_module"].Split(',');
                ////报价
                //string[] insure_module = ConfigurationManager.AppSettings["insure_module"].Split(',');
                //对账
                //string[] duizhang_module = ConfigurationManager.AppSettings["duizhang_module"].Split(',');
                //台帐
                string[] reconciliation_module = ConfigurationManager.AppSettings["reconciliation_module"].Split(',');
                //组织结构
                string[] feilv_module = ConfigurationManager.AppSettings["feilv_module"].Split(',');
                //运营
                string[] operate_module = ConfigurationManager.AppSettings["operate_module"].Split(',');
                //报表
                string[] statistical_report = ConfigurationManager.AppSettings["statistical_report"].Split(',');
                //系统设置
                string[] systemSetting_module = ConfigurationManager.AppSettings["systemSetting_module"].Split(',');
                //客户列表新放开
                string[] customerModule = ConfigurationManager.AppSettings["customer_module"].Split(',');
                //业务统计
                string[] statistical_module = ConfigurationManager.AppSettings["statistical_module"].Split(',');

                //获取所有的三级菜单按钮
                var allBtn = db.manager_module_button.Where(t => t.delete_state == 0).ToList();

                foreach (var item in role)
                {
                    // 0:普通员工 
                    if (item.role_type == 0)
                    {
                        //台帐
                        //DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        //{
                        //    role_id = item.id,
                        //    module_code = moduleAll[2].Trim(),
                        //    creator_full_name = "lzl",
                        //    creator_name = "lzl",
                        //    creator_time = DateTime.Now
                        //});
                        //foreach (var str in reconciliation_module)
                        //{
                        //    DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        //    {
                        //        role_id = item.id,
                        //        module_code = str.Trim(),
                        //        creator_full_name = name,
                        //        creator_name = name,
                        //        creator_time = DateTime.Now
                        //    });
                        //}

                        //客户管理
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[6].Trim(),
                            creator_full_name = "lzl",
                            creator_name = "lzl",
                            creator_time = DateTime.Now
                        });
                        foreach (var str in customerModule)
                        {
                            if (str != "batchRenewal_list" && str != "RenewalSetting")
                            {
                                // 普通员工没有摄像头进店权限
                                if (str == "camera_list" && item.role_type == 0)
                                    continue;

                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        //费率
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[3].Trim(),
                            creator_full_name = name,
                            creator_name = name,
                            creator_time = DateTime.Now
                        });
                        foreach (var str in feilv_module)
                        {
                            if (str == "agent_list" || str == "Solicitor_registration")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }
                        }

                        //运营
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[4].Trim(),
                            creator_full_name = name,
                            creator_name = name,
                            creator_time = DateTime.Now
                        });
                        foreach (var str in operate_module)
                        {
                            if (str == "smsSend_list")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        //业务统计
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[8].Trim(),
                            creator_full_name = name,
                            creator_name = name,
                            creator_time = DateTime.Now
                        });
                        foreach (var str in statistical_module)
                        {
                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = name,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }

                        #region 三级菜单 录入跟进回访(btn_review)
                        var btnReview = allBtn.Where(t => t.button_code == "btn_review").FirstOrDefault();
                        DataContextFactory.GetDataContext().manager_role_button_relation.Add(new manager_role_button_relation()
                        {
                            role_id = item.id,
                            button_id = btnReview.id,
                            module_code = btnReview.button_code,
                            creator_name = name,
                            creator_time = DateTime.Now
                        });
                        #endregion 

                    }
                    else if (item.role_type == 4)
                    {
                        //管理员

                        //客户列表
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[6].Trim(),
                            creator_full_name = "lzl",
                            creator_name = "lzl",
                            creator_time = DateTime.Now
                        });


                        foreach (var str in customerModule)
                        {
                            if (str != "QuotationReceipt_List" && str != "appoinment_list")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        #region 三级菜单
                        var customerButtonList = allBtn.Where(t => t.pater_module == "customer_list").ToList();
                        foreach (var btn in customerButtonList)
                        {
                            DataContextFactory.GetDataContext().manager_role_button_relation.Add(new manager_role_button_relation()
                            {
                                role_id = item.id,
                                button_id = btn.id,
                                module_code = btn.button_code,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }
                        #endregion
                    }
                    else if (item.role_type == 5)
                    {
                        //助理

                        //客户列表
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = moduleAll[6].Trim(),
                            creator_full_name = "lzl",
                            creator_name = "lzl",
                            creator_time = DateTime.Now
                        });
                        foreach (var str in customerModule)
                        {
                            if (str != "camera_list" && str != "batchRenewal_list" && str != "customer_recyclelist")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        #region 三级菜单 录入跟进回访(btn_review)
                        var btnReview = allBtn.Where(t => t.button_code == "btn_review").FirstOrDefault();
                        DataContextFactory.GetDataContext().manager_role_button_relation.Add(new manager_role_button_relation()
                        {
                            role_id = item.id,
                            button_id = btnReview.id,
                            module_code = btnReview.button_code,
                            creator_name = name,
                            creator_time = DateTime.Now
                        });
                        #endregion 
                    }
                    else if (item.role_type == 3)
                    {
                        //系统管理员

                        #region 一级菜单
                        foreach (var itemModule in moduleAll)
                        {
                            var removeModule = itemModule != "insure_module" && itemModule != "xubao_module";
                            if (zhenBangType == 1)//集团下的机构账号不给财务管理(finance_management)
                            {
                                removeModule = itemModule != "insure_module" && itemModule != "xubao_module" && itemModule != "finance_management";
                            }
                            if (removeModule)
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = itemModule.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }

                        }
                        #endregion

                        #region 二级菜单

                        foreach (var str in customerModule)
                        {

                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = "lzl",
                                creator_name = "lzl",
                                creator_time = DateTime.Now
                            });


                        }

                        foreach (var str in reconciliation_module)
                        {
                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = name,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }
                        foreach (var str in feilv_module)
                        {
                            if (role.Where(t => t.role_type == 6).Count() > 0)
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });

                            }
                            else
                            {
                                if (str != "lattice_point")
                                {
                                    DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                    {
                                        role_id = item.id,
                                        module_code = str.Trim(),
                                        creator_full_name = name,
                                        creator_name = name,
                                        creator_time = DateTime.Now
                                    });
                                }
                            }
                        }

                        foreach (var str in operate_module)
                        {
                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = name,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }
                        foreach (var str in systemSetting_module)
                        {
                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = name,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }

                        foreach (var str in statistical_module)
                        {

                            DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                            {
                                role_id = item.id,
                                module_code = str.Trim(),
                                creator_full_name = name,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }

                        #endregion

                        #region 三级菜单
                        var customerButtonList = allBtn.Where(t => t.pater_module == "customer_list").ToList();

                        foreach (var btn in customerButtonList)
                        {
                            DataContextFactory.GetDataContext().manager_role_button_relation.Add(new manager_role_button_relation()
                            {
                                role_id = item.id,
                                button_id = btn.id,
                                module_code = btn.button_code,
                                creator_name = name,
                                creator_time = DateTime.Now
                            });
                        }
                        #endregion

                        #region 集团账号 菜单
                        if (regType == 1)
                        {
                            foreach (var str in statistical_report)
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }
                        }
                        #endregion

                        AddLiPeiModuleRelation(item.id, item.creator_name);
                    }
                    else if (item.role_type == 6)//振邦机构的网点角色
                    {
                        #region 一级菜单
                        foreach (var itemModule in moduleAll)
                        {
                            if (itemModule != "systemSetting_module" && itemModule != "operate_module" && itemModule != "insure_module")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = itemModule.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }

                        }
                        #endregion

                        #region 二级菜单

                        //客户管理
                        foreach (var str in customerModule)
                        {
                            if (str != "batchRenewal_list" && str != "camera_list" && str != "appoinment_list")
                            {
                                // 普通员工没有摄像头进店权限
                                if (str == "camera_list" && item.role_type == 0)
                                    continue;

                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        //组织结构
                        foreach (var str in feilv_module)
                        {
                            if (str == "agent_list")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = "lzl",
                                    creator_name = "lzl",
                                    creator_time = DateTime.Now
                                });
                            }

                        }

                        //财务权限
                        foreach (var str in reconciliation_module)
                        {
                            if (str == "Ledger_Entry" || str == "Ledger_List")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }
                        }

                        //统计
                        foreach (var str in statistical_module)
                        {
                            if (str == "statistics_business" || str == "statistics_defeatAnalyze")
                            {
                                DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                                {
                                    role_id = item.id,
                                    module_code = str.Trim(),
                                    creator_full_name = name,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }
                        }
                        #endregion

                        #region 三级菜单
                        var customerButtonList = allBtn.Where(t => t.pater_module == "customer_list").ToList();
                        foreach (var btn in customerButtonList)
                        {
                            if (btn.button_code != "btn_recycle")
                            {
                                DataContextFactory.GetDataContext().manager_role_button_relation.Add(new manager_role_button_relation()
                                {
                                    role_id = item.id,
                                    button_id = btn.id,
                                    module_code = btn.button_code,
                                    creator_name = name,
                                    creator_time = DateTime.Now
                                });
                            }
                        }
                        #endregion
                    }
                    else if (item.role_type == 7)//理赔主管 
                    {

                        AddLiPeiModuleRelation(item.id, item.creator_name);
                    }

            }
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        /// <summary>
        /// 删除角色所有的功能 zky 2017-08-03
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public bool DeleteRoleRelation(int roleId,int crmModuleType=1)
        {
            var sql = string.Format(@"DELETE FROM manager_role_module_relation WHERE role_id={0};", roleId);
            //var sql = string.Format(@"DELETE   mrmr FROM manager_role_module_relation  mrmr  INNER JOIN manager_module_db mmd  ON mrmr.module_code = mmd.module_code WHERE mrmr.role_id ={0}  AND  mmd.crm_module_type ={1}", roleId, crmModuleType);
            return db.Database.ExecuteSqlCommand(sql) > 0;
        }

        /// <summary>
        /// 删除角色所有的权限按钮 zky 2017-11-29
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool DeleteRoleButton(int roleId, int crmModuleType = 1)
        {
            var sql = string.Format(@"DELETE FROM manager_role_button_relation WHERE role_id={0};", roleId);
            //var sql = string.Format(@"DELETE mrbr FROM manager_role_button_relation mrbr  INNER JOIN manager_module_db mmd  ON mrbr.module_code = mmd.module_code WHERE mrbr.role_id ={0}  AND  mmd.crm_module_type ={1}", roleId, crmModuleType);
            return db.Database.ExecuteSqlCommand(sql) > 0;
        }



      

      



        /// <summary>
        /// 给角色添加菜单 zky 2017-08-03
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="agentName">代理人姓名</param>
        /// <param name="list">菜单列表</param1
        /// <returns></returns>
        public bool AddRoleRelationList(int roleId, string agentName, List<ManagerModuleViewModel> list)
        {
            list.Where(p => p.status == "add" || p.status == "modify").ToList().ForEach(p =>
            {
                manager_role_module_relation pItem = new manager_role_module_relation();
                pItem.role_id = roleId;
                pItem.module_code = p.module_code;
                pItem.creator_name = agentName;
                pItem.creator_full_name = agentName;
                pItem.creator_time = System.DateTime.Now;
                this.db.manager_role_module_relation.Add(pItem);//添加一级菜单
                p.nodes.Where(c => c.status == "add" || c.status == "modify").ToList().ForEach(c =>
                {
                    manager_role_module_relation cItem = new manager_role_module_relation();
                    cItem.role_id = roleId;
                    cItem.module_code = c.module_code;
                    cItem.creator_name = agentName;
                    cItem.creator_full_name = agentName;
                    cItem.creator_time = System.DateTime.Now;
                    this.db.manager_role_module_relation.Add(cItem);//添加二级菜单
                    c.nodes.Where(t => t.status == "add" || t.status == "modify").ToList().ForEach(b =>
                    {
                        manager_role_button_relation btnItem = new manager_role_button_relation();
                        btnItem.role_id = roleId;
                        btnItem.button_id = b.buttonId;
                        btnItem.module_code = b.buttonCode;
                        btnItem.creator_name = agentName;
                        btnItem.creator_time = System.DateTime.Now;
                        this.db.manager_role_button_relation.Add(btnItem);//添加三级按钮
                    });
                });
            });
            return this.db.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据条件查询List zky 2017-08-03
        /// </summary>
        /// <param name="lamda"></param>
        /// <returns></returns>
        public List<manager_role_module_relation> GetList(Expression<Func<manager_role_module_relation, bool>> lamda)
        {
            return this.db.manager_role_module_relation.Where(lamda).ToList();
        }

        /// <summary>
        /// 给助理添加菜单  zky 2017-09-01 /crm
        /// </summary>
        /// <param name="roleList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool HelperAddModule(IList<manager_role_db> roleList, out int count)
        {
            //所有的父级菜单
            string[] moduleAll = ConfigurationManager.AppSettings["moduleAll"].Split(',');
            //客户管理的子菜单
            string[] customerModule = ConfigurationManager.AppSettings["customer_module"].Split(',');
            int addRows = 0;
            foreach (var item in roleList)
            {
                //先添加父级菜单（客户管理）
                db.manager_role_module_relation.Add(new manager_role_module_relation()
                {
                    role_id = item.id,
                    module_code = moduleAll[6].Trim(),
                    creator_full_name = "lzllzl",
                    creator_name = "lzllzl",
                    creator_time = DateTime.Now
                });

                //添加子菜单
                foreach (var str in customerModule)
                {
                    if (str != "camera_list" && str != "batchRenewal_list" && str != "customer_recyclelist" && str != "customer_checklist")
                    {
                        DataContextFactory.GetDataContext().manager_role_module_relation.Add(new manager_role_module_relation()
                        {
                            role_id = item.id,
                            module_code = str.Trim(),
                            creator_full_name = "lzllzl",
                            creator_name = "lzllzl",
                            creator_time = DateTime.Now
                        });
                        addRows++;
                    }
                }
                addRows++;
            }
            count = db.SaveChanges();
            return count == addRows;
        }

        /// <summary>
        /// 添加 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Add(manager_role_module_relation entity)
        {
            db.manager_role_module_relation.Add(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            var entity = db.manager_role_module_relation.Find(id);
            db.manager_role_module_relation.Attach(entity);
            db.Entry(entity).State = EntityState.Deleted;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 给多个角色添加同一个菜单 zky 2017-09-21 /crm
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="moduleCode"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public bool AddList(List<int> roleIds, string moduleCode, string creator)
        {
            foreach (var item in roleIds)
            {
                manager_role_module_relation entity = new manager_role_module_relation();
                entity.role_id = item;
                entity.module_code = moduleCode;
                entity.creator_full_name = creator;
                entity.creator_name = creator;
                entity.creator_time = DateTime.Now;

                db.manager_role_module_relation.Add(entity);
            }
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据id批量删除  zky 2017-09-21 /crm
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public bool DeleteList(List<int> idList)
        {
            foreach (var item in idList)
            {
                var entity = db.manager_role_module_relation.Where(t => t.id == item);
                db.Entry(entity).State = EntityState.Deleted;
            }
            return db.SaveChanges() > 0;
        }
    }
}
