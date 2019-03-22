using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations
{
    public class IsHaveLicensenoService : IIsHaveLicensenoService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ITwoLevelHaveLicensenoService _twoLevelHaveLicensenoService;
        private readonly string _needLevel2LicensenoAgent = ConfigurationManager.AppSettings["HaveLicensenoTwoLevelAgent"];
        private readonly IAgentService _agentService;
        public IsHaveLicensenoService(IUserInfoRepository userInfoRepository, ITwoLevelHaveLicensenoService twoLevelHaveLicensenoService, IAgentService agentService)
        {
            _userInfoRepository = userInfoRepository;
            _twoLevelHaveLicensenoService = twoLevelHaveLicensenoService;
            _agentService = agentService;
        }

        /// <summary>
        /// 判断是否代报价
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="type"></param>
        /// <param name="repeatQuote"></param>
        /// <param name="isBehalfQuote"></param>
        /// <returns></returns>
        public AgentNameViewModel GetRepeatQuoteInfo(int topAgentId, int agentId, string licenseno, string vinNo, int type, int? repeatQuote, int isBehalfQuote)
        {
            LogHelper.Info("topAgentId=" + topAgentId + "agentId=" + agentId);
            //取库里的重复续保报价的记录
            List<AgentNameViewModel> agentList = null;
            List<string> listAgent = null;
            AgentNameViewModel agentModel = null;

            if (!repeatQuote.HasValue)
            {
                repeatQuote = 0;
            }

            #region (1)允许重复报价（repeatQuote=1）,直接进行报价
            if (repeatQuote == 1)
            {
                //当允许重复报价时候，直接返回null，去进行报价
                return null;
            }
            #endregion

            #region （2）当车牌号、车架号属于自己的时候，直接去报价
            agentList = _userInfoRepository.GetJuniorRepeat(topAgentId, agentId, licenseno, vinNo, type);
            //LogHelper.Info("当车牌号、车架号属于自己的时候，直接去报价:" + string.Join(",", agentList));
            if (agentList != null && agentList.Count > 0)
            {
                return null;//证明属于自己的。
            }
            #endregion

            #region （3）不允许重复报价（repeatQuote=0） || 允许二级之间重复（repeatQuote=2）
            #region 1.0顶级代理人直接去报价
            if (topAgentId == agentId)
            {
                
                //顶级代理人下边对licenseno报价过得数据，是否有重复
                agentList = _userInfoRepository.GetJuniorRepeat(topAgentId, 0, licenseno, vinNo, type);
                if (agentList == null || agentList.Count <= 0)
                {
                    return null;
                }
                agentModel = agentList.FirstOrDefault();
                if (agentModel == null)
                {
                    return null;
                }
                if (agentModel.AgentId == topAgentId)
                {
                    return null;
                }
                agentModel.Type = 1;
                return agentModel;
            }

            #endregion

            //判断当前代理人是几级代理人（排除顶级代理人）
            int parentId = _agentService.VerificationThirdAccount(agentId);
            #region 2.0二级代理人
            if (parentId == 0)
            {
                //2.1二级报顶级、三级（子级）
                listAgent = _agentService.GetOtherAgentIdList(agentId, 1);
                //LogHelper.Info("二级报顶级、三级（子级）：" + string.Join(",", listAgent));
                if (listAgent != null && listAgent.Count > 0)
                {
                    #region 2.1先判断当前二级代理人报三级（同父级）、顶级
                    agentList = _userInfoRepository.SecondRepeat(listAgent, licenseno, vinNo, type);
                    //LogHelper.Info(" 2.1先判断当前二级代理人报三级（同父级）、顶级：" + string.Join(",", agentList));
                    if (agentList != null && agentList.Count > 0)
                    {
                        agentModel = agentList.FirstOrDefault();
                        //只在全局不允许重复报价
                        if (repeatQuote != 2)
                        {
                            //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
                            if (topAgentId != agentId && agentModel != null && agentModel.AgentId == topAgentId)
                            {
                                string agentMd5 = agentId.ToString().GetMd5();
                                _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agentModel.Buid, agentId);
                                return null;
                            }
                        }
                        if (agentModel.AgentId == agentId)
                        {
                            return null;
                        }
                        else
                        {
                            //全局不允许重复报价      允许代报价
                            if (repeatQuote == 0 && isBehalfQuote == 1)
                            {
                                agentModel.Type = 1;
                                return agentModel;
                            }
                            //全局不允许重复报价      不允许代报价
                            if (repeatQuote == 0 && isBehalfQuote == 2)
                            {
                                if (agentModel.AgentId == topAgentId)
                                {
                                    agentModel.Type = 0;
                                    return agentModel;
                                }
                                agentModel.Type = 1;
                                return agentModel;
                            }
                            //允许二级之间重复        允许代报价
                            if (repeatQuote == 2 && isBehalfQuote == 1)
                            {
                                agentModel.Type = 1;
                                return agentModel;
                            }
                            //允许二级之间重复        不允许代报价
                            if (repeatQuote == 2 && isBehalfQuote == 2)
                            {
                                if (agentModel.AgentId == topAgentId)
                                {
                                    //agentModel.Type = 0;
                                    //return agentModel;
                                    return null;
                                }
                                agentModel.Type = 1;
                                return agentModel;
                            }
                        }
                    }
                }
                    #endregion
                    #region 2.2二级报二级（同级）、三级（非子集）
                #region 二级报其他二级和三级
                listAgent = _agentService.GetOtherAgentList(agentId, topAgentId);
                //LogHelper.Info("二级报其他二级和三级：" + string.Join(",", listAgent));
                if (listAgent == null || !listAgent.Any())
                {
                    return null;
                }
                agentList = _userInfoRepository.SecondRepeat(listAgent, licenseno, vinNo, type);
                if (agentList == null || !agentList.Any())
                {
                    return null;
                }
                agentModel = agentList.FirstOrDefault();
                if (repeatQuote != 2)
                {
                    //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
                    if (topAgentId != agentId && agentModel != null && agentModel.AgentId == topAgentId)
                    {
                        string agentMd5 = agentId.ToString().GetMd5();
                        _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agentModel.Buid, agentId);
                        return null;
                    }
                }
                if (agentModel == null)
                {
                    return null;
                }
                if (agentId == agentModel.AgentId)
                {
                    return null;
                }
                //全局不允许重复报价      允许代报价
                if (repeatQuote == 0 && isBehalfQuote == 1)
                {
                    agentModel.Type = 1;
                    return agentModel;
                }
                //全局不允许重复报价      不允许代报价
                if (repeatQuote == 0 && isBehalfQuote == 2)
                {
                    agentModel.Type = 0;
                    return agentModel;
                }
                //允许二级之间重复        允许代报价
                if (repeatQuote == 2 && isBehalfQuote == 1)
                {
                    return null;
                }
                //允许二级之间重复        不允许代报价
                if (repeatQuote == 2 && isBehalfQuote == 2)
                {
                    return null;//允许重复报价
                }
                #endregion
                #endregion

            }
            #endregion

            #region 3.0三级代理人
            //①A1（三级）→A2（三级）， ②A1（三级） → A（二级），A1（三级） →  T（顶级）
            if (parentId != 0)
            {
                #region 3.1先判断当前三级代理人报三级（同父级）、二级（父级）、顶级
                listAgent = _agentService.GetOtherAgentIdList(parentId, 1);//拿到体系内二级和三级、顶级代理人
                //LogHelper.Info("先判断当前三级代理人报三级（同父级）、二级（父级）、顶级：" + string.Join(",", listAgent));
                if (listAgent != null && listAgent.Count > 0)
                {
                    agentList = _userInfoRepository.SecondRepeat(listAgent, licenseno, vinNo, type);
                    if (agentList != null && agentList.Count > 0)
                    {
                        agentModel = agentList.FirstOrDefault();
                        if (repeatQuote != 2)
                        {
                            //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
                            if (topAgentId != agentId && agentModel != null && agentModel.AgentId == topAgentId)
                            {
                                string agentMd5 = agentId.ToString().GetMd5();
                                _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agentModel.Buid, agentId);
                                return null;
                            }
                        }
                        if (agentModel.AgentId == agentId)
                        {
                            return null;
                        }
                        else
                        {
                            //全局不允许重复报价      允许代报价
                            if (repeatQuote == 0 && isBehalfQuote == 1)
                            {
                                agentModel.Type = 1;
                                return agentModel;
                            }
                            //全局不允许重复报价      不允许代报价
                            if (repeatQuote == 0 && isBehalfQuote == 2)
                            {
                                agentModel.Type = 0;
                                return agentModel;
                            }
                            //允许二级之间重复        允许代报价
                            if (repeatQuote == 2 && isBehalfQuote == 1)
                            {
                                agentModel.Type = 1;
                                return agentModel;
                            }
                            //允许二级之间重复        不允许代报价
                            if (repeatQuote == 2 && isBehalfQuote == 2)
                            {
                                //特殊配置的代理人：商社  商社三级领取二级数据
                                string[] specialAgentList = _needLevel2LicensenoAgent.Split(',');
                                bool result = _agentService.VerifySecond(agentModel.AgentId, 2);//判断车牌号所属业务员是否是二级代理人
                                if (result && specialAgentList.Contains(topAgentId.ToString()))
                                {
                                    string agentMd5 = agentId.ToString().GetMd5();
                                    _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agentModel.Buid, agentId);
                                    return null;                                  
                                }
                                //三级报顶级会重复新增数据
                                if (agentModel.AgentId==topAgentId)
                                {
                                    return null;
                                }
                                agentModel.Type = 0;
                                return agentModel;
                                
                            }
                        }
                    }
                }
                #endregion                

                #region 3.2三级报二级(非父级)、三级（非父级）的车牌： A1（三级）→B（二级）、A1（三级）→B1（三级）
                listAgent = _agentService.GetOtherAgentList(parentId, topAgentId);
                //LogHelper.Info("3.2三级报二级(非父级)、三级（非父级）的车牌：：" + string.Join(",", listAgent));
                if (listAgent == null || !listAgent.Any())
                {
                    return null;
                }
                agentList = _userInfoRepository.SecondRepeat(listAgent, licenseno, vinNo, type);
                if (agentList == null || !agentList.Any())
                {
                    return null;
                }
                agentModel = agentList.FirstOrDefault();
                if (agentModel == null)
                {
                    return null;
                }
                if (repeatQuote != 2)
                {
                    //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
                    if (topAgentId != agentId && agentModel != null && agentModel.AgentId == topAgentId)
                    {
                        string agentMd5 = agentId.ToString().GetMd5();
                        _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agentModel.Buid, agentId);
                        return null;
                    }
                }
                if (agentModel.AgentId == agentId)
                {
                    return null;
                }
                //全局不允许重复报价      允许代报价
                if (repeatQuote == 0 && isBehalfQuote == 1)
                {
                    agentModel.Type = 1;//2018-07-19
                    return agentModel;
                }
                //全局不允许重复报价      不允许代报价
                if (repeatQuote == 0 && isBehalfQuote == 2)
                {
                    agentModel.Type = 0;
                    return agentModel;
                }
                //允许二级之间重复        允许代报价
                if (repeatQuote == 2 && isBehalfQuote == 1)
                {
                    return null;
                }
                //允许二级之间重复        不允许代报价
                if (repeatQuote == 2 && isBehalfQuote == 2)
                {
                    return null;//允许重复报价
                }

                #endregion
            }
            #endregion
            #endregion
            return null;

        }




        /// <summary>
        /// 判断一个顶级下面有其他人算过请求的车牌 /pc、微信、app
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="licenseno"></param>
        /// <param name="vinNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public AgentNameViewModel IsHaveLicenseno(int topAgentId, int agentId, string licenseno, string vinNo, int type, int? repeatQuote)
        {
            //特殊配置的代理人
            string[] specialAgentList = _needLevel2LicensenoAgent.Split(',');
            //取库里的重复续保报价的记录
            List<AgentNameViewModel> agentList;

            AgentNameViewModel topModel;
            List<string> listAgent;
            if (repeatQuote == 2)
            {

                int parentId = _agentService.VerificationThirdAccount(agentId);
                if (parentId != 0)//三级账号
                {
                    listAgent = _agentService.GetSonsListFromRedisToString(parentId);//拿到体系内二级和三级账号
                    listAgent.Remove(agentId.ToString());
                }
                else
                {//二级账号
                    listAgent = _agentService.GetSonsListFromRedisToString(agentId, false);//拿到体系三级账号
                }
                if (listAgent.Count > 0)//拿到体系内账号
                {
                    agentList = _userInfoRepository.SecondRepeat(listAgent, licenseno, vinNo, type);
                    if (agentList.Count == 0)//体系没数据，取顶级
                    {
                        List<string> listTopAgent = new List<string>();
                        listTopAgent.Add(topAgentId.ToString());
                        agentList = _userInfoRepository.SecondRepeat(listTopAgent, licenseno, vinNo, type);
                        if (agentList.Count == 0)
                        {
                            return null;
                        }
                        else if (agentList[0].AgentId == topAgentId)
                        {//如果试算顶级自己的车，直接返回空
                            return null;
                        }

                        // else 顶级有数据
                    }
                    //else 体系内有数据（三级账号会包括2级和同类三级，二级账号不会包括自己）
                }
                else
                {
                    return null;
                }
            }
            else
            {
                agentList = _userInfoRepository.IsHaveLicenseno(topAgentId, agentId, licenseno, vinNo, type);
            }

            if (agentList == null || agentList.Count == 0)
            {
                return null;
            }
            var agent = agentList.FirstOrDefault();
            //如果顶级以前有试算的数据则把顶级的数据分配给 试算的下级代理人
            if (topAgentId != agentId && agent != null && agent.AgentId == topAgentId)
            {
                string agentMd5 = agentId.ToString().GetMd5();
                _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, agent.Buid, agentId);
                agent = null;
            }
            //20180129逻辑修改
            //下级能拿到上级的未分配的数据
            else if (specialAgentList.Contains(topAgentId.ToString()))
            {
                agent = _twoLevelHaveLicensenoService.GetLevel2LicenseNo(topAgentId, agentId, agentList);
                return agent;
            }
            //顶级试算下级已有车牌，不拦截跳转到下级对应车牌的详情页。
            if (topAgentId == agentId && agent != null)
            {
                if (agent.AgentId == topAgentId)
                {//如果试算顶级自己的车，直接返回空
                    return null;
                }//否则，就是顶级算下级的车，返回查看详情
                agent.Type = 1;
            }
            return agent;
        }



    }
}
