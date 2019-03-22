using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net.Util;
using System.Threading.Tasks;
using System.Text;
using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class ConsumerDetailService : CommonBehaviorService, IConsumerDetailService
    {
        private readonly IConsumerDetailRepository _consumerDetailRepository;
        private IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;
        private readonly IAgentService _agentService;

        public ConsumerDetailService(IConsumerDetailRepository consumerDetailRepository,
            ICacheHelper cacheHelper,
            IAgentRepository agentRepository,
            IAgentService agentService)
            : base(agentRepository, cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _agentRepository = agentRepository;
            _consumerDetailRepository = consumerDetailRepository;
            _agentService = agentService;

        }
        public int AddCrmSteps(bx_crm_steps bxCrmSteps)
        {
            return _consumerDetailRepository.AddCrmSteps(bxCrmSteps);
        }

        public int UpdateCrmSteps(bx_crm_steps bxCrmSteps)
        {
            return _consumerDetailRepository.UpdateCrmSteps(bxCrmSteps);
        }

        public async Task<int> AddCrmStepsAsync(bx_crm_steps bxCrmSteps)
        {
            Task<int> num = _consumerDetailRepository.AddCrmStepsAsync(bxCrmSteps);
            return num.Result;
        }

        public async Task<bool> InsertBySqlAsync(List<bx_crm_steps> list) 
        {
            return await _consumerDetailRepository.InsertBySqlAsync(list);
        }

        public List<bx_crm_steps> GetCrmStepsList(long buid)
        {
            return _consumerDetailRepository.GetCrmStepsList(buid);
        }

        public string GetTopAgent(int agentId)
        {
            return _consumerDetailRepository.GetTopAgent(agentId);
        }

        public bx_sms_account GetBxSmsAccount(int agentid)
        {
            return _consumerDetailRepository.GetBxSmsAccount(agentid);
        }

        public void InsetBxSmsAccount(bx_sms_account bxSmsAccount)
        {
            _consumerDetailRepository.InsetBxSmsAccount(bxSmsAccount);
        }

        public SmsResultModel SendSmsForBaoJia(string mobile, string smsContent, EnumSmsBusinessType businessType,
            string smsAccount, string smsPassword,int topAgentId, string smsSign, int batchId=-1, int isBatch = 0)
        {
            //var commonBehaviorService = new CommonBehaviorService(_agentRepository, _cacheHelper);
            
           return SendSms(mobile, smsContent, businessType,smsAccount, smsPassword, topAgentId, smsSign, batchId, isBatch);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>

        public NewQuoteInfoViewModel GetNewQuoteInfo(long buid, int agentId)
        {

            bx_lastinfo bxLastinfo = _consumerDetailRepository.GetLastinfo(buid);
            bx_userinfo bxUserinfo = _consumerDetailRepository.GetBxUserinfo(buid);
            //bx_quotereq_carinfo bxQuotereqCarinfo = _consumerDetailRepository.GetBxQuotereqCarinfo(buid);
            bx_quoteresult bxQuoteresult = _consumerDetailRepository.GetQuoteresult(buid);
            return new NewQuoteInfoViewModel()
            {
                LastYearAcctimes = bxLastinfo.last_year_acctimes ?? 0,
                LastYearClaimamount = bxLastinfo.last_year_claimamount ?? 0,
                BizStartDate = bxQuoteresult.BizStartDate,
                ForceStartDate = bxQuoteresult.ForceStartDate,
                InsuredName = bxUserinfo.InsuredName,
                InsuredAddress = bxUserinfo.InsuredAddress,
                InsuredIdCard = bxUserinfo.InsuredIdCard,
                InsuredIdType = bxUserinfo.InsuredIdType ?? 1,
                InsuredMobile = bxUserinfo.Mobile
            };

        }

        public void SaveNewQuoteInfo(RequestNewQuoteInfoViewModel requestNew)
        {
            _consumerDetailRepository.SaveNewQuoteInfo(requestNew);
        }

        public BaseViewModel DoSubmit(long buid, int source)
        {
            bx_userinfo bxUserinfo = _consumerDetailRepository.GetBxUserinfo(buid);
            int submitGroup = int.Parse(bxUserinfo.Source.ToString());
            if ((bxUserinfo.Source & source) <= 0)
            {
                submitGroup += source;
            }
            int quoteGroup = int.Parse(bxUserinfo.IsSingleSubmit.ToString());
            string url = string.Format("{0}/api/CarInsurance/PostPrecisePriceAgain?Buid={1}&QuoteGroup={2}&SubmitGroup={3}", ApplicationSettingsFactory.GetApplicationSettings().BaoJiaJieKou, buid, quoteGroup, submitGroup);
            var ret = HttpWebAsk.HttpGet(url);

            return JsonHelper.DeSerialize<BaseViewModel>(ret);
        }

        public string UpdateRole()
        {

            //更新管理员问题数据




            //List<bx_agent> bxAgents = _consumerDetailRepository.OldNoManagerRoleBxAgents().ToList();
            //#region 顶级代理人添加新的
            ////客户管理
            //string[] customerModule = ConfigurationManager.AppSettings["customer_module"].Split(',');
            //string[] moduleAll = ConfigurationManager.AppSettings["moduleAll"].Split(',');
            // int num = 0;



            List<manager_role_db> managerRoleDbs = _consumerDetailRepository.ManagerRoleDbs().ToList();
            foreach (var item in managerRoleDbs)
            {
                _consumerDetailRepository.InsertManagerRoleModuleRelation(item);
            }
            // #region 顶级代理人删除角色和模型关系表中的续保和报价的模块,给已有角色添加新的客户模块
            // foreach (var role in managerRoleDbs)
            // {

            //     var newManagerRoleModuleRelations = new List<manager_role_module_relation>();

            //     if (role.role_type != 3 && role.role_type != 4)
            //     {
            //         #region 普通 报价员 出单员 客户模块
            //         //
            //         newManagerRoleModuleRelations.Add(new manager_role_module_relation()
            //         {
            //             role_id = role.id,
            //             module_code = moduleAll[6].Trim(),
            //             creator_full_name = "lzl",
            //             creator_name = "lzl",
            //             creator_time = DateTime.Now
            //         });
            //         newManagerRoleModuleRelations.AddRange(from str in customerModule
            //                                                where str != "customer_list" && str != "batchRenewal_list"&&str!="RenewalSetting"
            //                                                select new manager_role_module_relation()
            //                                                {
            //                                                    role_id = role.id,
            //                                                    module_code = str.Trim(),
            //                                                    creator_full_name = "lzl",
            //                                                    creator_name = "lzl",
            //                                                    creator_time = DateTime.Now
            //                                                });

            //         #endregion
            //     }
            //     if (role.role_type == 4)
            //     {
            //         #region 管理员 客户模块
            //         //
            //         newManagerRoleModuleRelations.Add(new manager_role_module_relation()
            //         {
            //             role_id = role.id,
            //             module_code = moduleAll[6].Trim(),
            //             creator_full_name = "lzl",
            //             creator_name = "lzl",
            //             creator_time = DateTime.Now
            //         });
            //         newManagerRoleModuleRelations.AddRange(from str in customerModule
            //                                                where str != "customer_checklist" && str != "QuotationReceipt_List" && str != "appoinment_list"
            //                                                select new manager_role_module_relation()
            //                                                {
            //                                                    role_id = role.id,
            //                                                    module_code = str.Trim(),
            //                                                    creator_full_name = "lzl",
            //                                                    creator_name = "lzl",
            //                                                    creator_time = DateTime.Now
            //                                                });

            //         #endregion
            //     }
            //     if (role.role_type == 3)
            //     {
            //         #region 超级管理员  客户模块
            //         //
            //         newManagerRoleModuleRelations.Add(new manager_role_module_relation()
            //         {
            //             role_id = role.id,
            //             module_code = moduleAll[6].Trim(),
            //             creator_full_name = "lzl",
            //             creator_name = "lzl",
            //             creator_time = DateTime.Now
            //         });
            //         newManagerRoleModuleRelations.AddRange(from str in customerModule
            //                                                where str != "customer_checklist"
            //                                                select new manager_role_module_relation()
            //                                                {
            //                                                    role_id = role.id,
            //                                                    module_code = str.Trim(),
            //                                                    creator_full_name = "lzl",
            //                                                    creator_name = "lzl",
            //                                                    creator_time = DateTime.Now
            //                                                });

            //         #endregion
            //     }


            //     _consumerDetailRepository.AddConsumerRole(newManagerRoleModuleRelations, role.id);
            // }
            // foreach (var agent in bxAgents)
            // {
            //     num++;
            //     LogLog.Debug("总数："+bxAgents.Count+"执行数:"+num+"代理人:"+agent.AgentName);


            //    var managerRole = new manager_role_db()
            //    {
            //        role_name = "管理员",
            //        role_type = 4,
            //        role_status = 1,
            //        creator_name = "lzl",
            //        creator_full_name = "lzl",
            //        creator_time = DateTime.Now,
            //        modifi_name = "lzl",
            //        modifi_full_name = "lzl",
            //        modifi_time = DateTime.Now,
            //        top_agent_id = agent.Id
            //    };

            //    var managerRoleModuleRelations = new List<manager_role_module_relation>();
            //    if (_consumerDetailRepository.GetManagerRoleDb(agent.Id) == null)
            //    {
            //        var managerRoleModuleRelation = new manager_role_module_relation();
            //        managerRoleModuleRelations.Add(managerRoleModuleRelation);
            //        #region 添加客户管理模块

            //        managerRoleModuleRelations.Add(new manager_role_module_relation()
            //        {
            //            //role_id = item.id,
            //            module_code = moduleAll[6].Trim(),
            //            creator_full_name = "lzl",
            //            creator_name = "lzl",
            //            creator_time = DateTime.Now
            //        });

            //        managerRoleModuleRelations.AddRange(from str in customerModule
            //                                            where str != "customer_checklist" && str != "QuotationReceipt_List" && str != "appoinment_list"
            //                                            select new manager_role_module_relation()
            //                                            {
            //                                                module_code = str.Trim(),
            //                                                creator_full_name = "lzl",
            //                                                creator_name = "lzl",
            //                                                creator_time = DateTime.Now
            //                                            });
            //        #endregion

            //        _consumerDetailRepository.AddRole(managerRole, managerRoleModuleRelations);
            //    }

            //    #endregion
            // }
            //#endregion



            return "执行完成";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public int GetAppoinmentInfoNum(int agentid)
        {
            List<string> agentids = _agentService.GetSonsListFromRedisToString(agentid);
            //_agentRepository.GetSonsList(agentid);
            string ids = string.Join(",", agentids);
            if (!agentids.Any())
            {
                return 0;
            }
            return _consumerDetailRepository.GetAppoinmentInfoNum(ids);
        }
        // <summary>
        /// 获取代报价内容，用于跟进记录使用
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="behalfAgent">代报价人</param>
        /// <param name="behalfAgentName">代报价人名称</param>
        public async Task<string> GetBehelfQuoteAsync(long buid, int behalfAgent, string behalfAgentName, int sumValue)
        {
            BehalfQuoteModel behalfQuoteModel = new BehalfQuoteModel();
            behalfQuoteModel.ReQuoteName = behalfAgentName;
            behalfQuoteModel.InsuredCompany = "";
            behalfQuoteModel.BehalfContent = "";
            var sourceList = SourceGroupAlgorithm.ParseSource(sumValue);

            //保额
            bx_savequote sQuoteModel = _consumerDetailRepository.GetSaveQuoteByBuid(buid);
            if (sQuoteModel == null)
            {
                return JsonHelper.Serialize(behalfQuoteModel);
            }
            //保费
            List<bx_quoteresult> quoteResList = await _consumerDetailRepository.GetQuoteResultListByBuid(buid);
            StringBuilder insuredCpyBuilder = new StringBuilder();

            #region 投保公司
            if (quoteResList == null && !quoteResList.Any())
            {
                foreach (long source in sourceList)
                {
                    string sourceNameTemp = RenewalInfoService.ToEnumDescription(source, typeof(EnumSourceNew));
                    insuredCpyBuilder.Append(sourceNameTemp).Append("（").Append("0").Append("元").Append("）").Append("/");
                }
            }
            //投保公司
            else
            {
                var sourceTempList = sourceList;//存储保险公司编号，最后剩下的也会进行拼接

                foreach (bx_quoteresult quoteResModel in quoteResList)
                {
                    int source = GetNewSource(quoteResModel.Source.Value);
                    //int source = quoteResModel.Source.Value;
                    if (sourceList.Contains(source))
                    {
                        sourceTempList.Remove(source);
                        //                             交强险总额                       车船税                         商业险总额
                        var tempTotal = quoteResModel.ForceTotal.Value + quoteResModel.TaxTotal.Value + quoteResModel.BizTotal.Value;
                        tempTotal = Math.Round(tempTotal, 2);
                        string sourceName = RenewalInfoService.ToEnumDescription(source, typeof(EnumSourceNew));
                        insuredCpyBuilder.Append(sourceName).Append("（").Append(tempTotal).Append("元").Append("）").Append("/");
                    }

                }
                foreach (long item in sourceTempList)
                {
                    string sourceNameTemp = RenewalInfoService.ToEnumDescription(item, typeof(EnumSourceNew));
                    insuredCpyBuilder.Append(sourceNameTemp).Append("（").Append("0").Append("元").Append("）").Append("/");
                }
            }
            string insuredCpyStr = insuredCpyBuilder.ToString().TrimEnd('/');
            #endregion

            //报价内容
            StringBuilder strBuilder = new StringBuilder();
            //单交强
            if (sQuoteModel.JiaoQiang == 2)
            {
                strBuilder.Append("交强险；");
                if (quoteResList[0].TaxTotal > 0)
                {
                    strBuilder.Append("车船税；");
                }
            }
            else
            {
                strBuilder.Append("商业险(");
                #region 保额
                if (sQuoteModel.CheSun != null)
                {
                    //0：不投保，1：投保， >1：保额
                    var cheSunBaoE = sQuoteModel.CheSun.Value;
                    if (cheSunBaoE == 1)
                    {
                        strBuilder.Append("机动车损失保险；");
                    }
                    else if (cheSunBaoE > 1)
                    {
                        strBuilder.Append("机动车损失保险" + GetBaoE(cheSunBaoE) + "；");
                    }
                }

                if (sQuoteModel.SanZhe != null)
                {
                    //0：不投保，>0：保额
                    var sanZheBaoE = sQuoteModel.SanZhe.Value;
                    if (sanZheBaoE > 0)
                    {
                        strBuilder.Append("第三者责任保险" + GetBaoE(sanZheBaoE) + "；");
                    }
                }
                if (sQuoteModel.DaoQiang.HasValue)
                {
                    //0：不投保，1：投保， >1：保额
                    var daoQiangBaoE = sQuoteModel.DaoQiang.Value;
                    if (daoQiangBaoE == 1)
                    {
                        strBuilder.Append("全车盗抢保险；");
                    }
                    else if (daoQiangBaoE > 1)
                    {
                        strBuilder.Append("全车盗抢保险" + GetBaoE(daoQiangBaoE) + "；");
                    }
                }
                if (sQuoteModel.SiJi.HasValue)
                {
                    //0：不投保，>0：保额
                    var siJiBaoE = sQuoteModel.SiJi.Value;
                    if (siJiBaoE > 0)
                    {
                        strBuilder.Append("车上人员责任险(司机)" + GetBaoE(siJiBaoE) + "；");
                    }
                }
                if (sQuoteModel.ChengKe.HasValue)
                {
                    //0：不投保， >0：保额
                    var chengKeBaoE = sQuoteModel.ChengKe.Value;
                    if (chengKeBaoE > 0)
                    {
                        strBuilder.Append("车上人员责任险(乘客)" + GetBaoE(chengKeBaoE) + "；");
                    }
                }
                //玻璃单独破碎险
                if (sQuoteModel.BoLi.HasValue)
                {
                    //0:不投保，1:国产，2:进口
                    var boLi = sQuoteModel.BoLi.Value;
                    if (boLi == 1)
                    {
                        strBuilder.Append("玻璃单独破碎险（国产）；");
                    }
                    else if (boLi == 2)
                    {
                        strBuilder.Append("玻璃单独破碎险（进口）；");
                    }
                }
                //车身划痕损失险
                if (sQuoteModel.HuaHen.HasValue)
                {
                    var huaHenBaoE = sQuoteModel.HuaHen.Value;
                    if (huaHenBaoE > 0)
                    {
                        strBuilder.Append("车身划痕损失险" + GetBaoE(huaHenBaoE) + "；");
                    }
                }
                //指定修理厂险
                //if (sQuoteModel.HcXiuLiChang.HasValue)
                //{
                //    //0：不投保，>0:指定修理厂险
                //    if (sQuoteModel.HcXiuLiChang.Value > 0)
                //    {
                //        strBuilder.Append("指定修理厂险；");
                //    }
                //}
                //涉水行驶损失险
                if (sQuoteModel.SheShui.HasValue)
                {
                    //0:不投保，1：投保
                    var sheShui = sQuoteModel.SheShui.Value;
                    if (sheShui > 0)
                    {
                        strBuilder.Append("涉水行驶损失险；");
                    }
                }
                //自燃损失险
                if (sQuoteModel.ZiRan.HasValue)
                {
                    var ziRan = sQuoteModel.ZiRan.Value;
                    if (ziRan > 0)
                    {
                        strBuilder.Append("自燃损失险；");
                    }
                }
                //新增设备损失险
                if (sQuoteModel.HcSheBeiSunshi.HasValue)
                {
                    //0:不投保，>0：投保
                    var hcSheBeiSunShi = sQuoteModel.HcSheBeiSunshi.Value;
                    if (hcSheBeiSunShi > 0)
                    {
                        strBuilder.Append("新增设备损失险；");
                    }
                }
                //修理期间费用补偿险
                if (sQuoteModel.HcFeiYongBuChang.HasValue)
                {
                    //0:不投保，>0：投保
                    if (sQuoteModel.HcFeiYongBuChang.Value > 0)
                    {
                        strBuilder.Append("修理期间费用补偿险；");
                    }
                }
                //车损无法找到第三方险
                if (sQuoteModel.HcSanFangTeYue != null)
                {
                    //0:不投保，1：投保
                    var hcSanFangTeYue = sQuoteModel.HcSanFangTeYue.Value;
                    if (hcSanFangTeYue == 1)
                    {
                        strBuilder.Append("车损无法找到第三方险；");
                    }
                }
                //指定修理厂险
                if (sQuoteModel.HcXiuLiChangType.HasValue)
                {
                    var hcXiuLiChangType = sQuoteModel.HcXiuLiChangType.Value;
                    if (hcXiuLiChangType == 0)
                    {
                        strBuilder.Append("指定修理厂险（国产）；");
                    }
                    else if (hcXiuLiChangType == 1)
                    {
                        strBuilder.Append("指定修理厂险（进口）；");
                    }
                }
                #region 不计免总额
                if (sQuoteModel.BuJiMianCheSun.Value == 1 || sQuoteModel.BuJiMianSanZhe.Value == 1
                    || sQuoteModel.BuJiMianSiJi.Value == 1 || sQuoteModel.BuJiMianChengKe.Value == 1
                    || sQuoteModel.BuJiMianDaoQiang.Value == 1 || sQuoteModel.BuJiMianHuaHen.Value == 1
                    || sQuoteModel.BuJiMianJingShenSunShi.Value == 1 || sQuoteModel.BuJiMianSheShui.Value == 1
                    || sQuoteModel.BuJiMianZiRan.Value == 1 || sQuoteModel.BuJiMianSheBeiSunshi.Value == 1
                    || sQuoteModel.BuJiMianRenYuan.Value == 1)
                {
                    strBuilder.Append("不计免赔险");
                }
                #endregion
                strBuilder.Append(")");
                //双险：交强+商业
                if (sQuoteModel.JiaoQiang == 1)
                {
                    strBuilder.Append("交强险；");
                    if (quoteResList[0].TaxTotal > 0)
                    {
                        strBuilder.Append("车船税；");
                    }
                }
                #endregion
            }



            behalfQuoteModel.ReQuoteName = behalfAgentName;
            behalfQuoteModel.InsuredCompany = insuredCpyStr;
            behalfQuoteModel.BehalfContent = strBuilder.ToString();

            string result = JsonHelper.Serialize(behalfQuoteModel);
            return result;

        }
        /// <summary>
        /// 保司老对应新
        /// 1-1
        /// 0-2
        /// 2-4
        /// 3-8
        /// </summary>
        /// <param name="tempSource"></param>
        /// <returns></returns>
        private int GetNewSource(int tempSource)
        {
            int source = -1;
            switch (tempSource)
            {
                case 1:
                    source = 1;
                    break;
                case 0:
                    source = 2;
                    break;
                case 2:
                    source = 4;
                    break;
                case 3:
                    source = 8;
                    break;
                default:
                    source = -1;
                    break;
            }
            return source;
        }
        /// <summary>
        /// 获得保额
        /// </summary>
        /// <param name="baoE"></param>
        /// <returns></returns>
        private string GetBaoE(double baoE)
        {
            if (baoE >= 10000)
            {
                return (baoE / 10000) + "万";
            }
            if (baoE >= 1000)
            {
                return (baoE / 1000) + "千";
            }
            return baoE + "元";
        }
    }
}
