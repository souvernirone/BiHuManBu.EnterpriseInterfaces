using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnumSource = BiHuManBu.ExternalInterfaces.Models.ViewModels.EnumSource;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.MapperService.Implementations
{
    public class CustomerListMapperService : ICustomerListMapperService
    {
        private IConsumerReviewService _consumerReviewService;
        private IAgentService _agentService;
        private IQuoteReqCarInfoService _quoteReqCarInfoService;
        private IEnterpriseAgentService _enterpriseAgentService;
        private IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private ICustomerCategories _customercategoriesService;
        private IAgentRepository _agentRepository;
        private IUserInfoRepository _userInfoRepository;
        private readonly ICustomerStatusService _customerStatusService;



        public CustomerListMapperService(
            IConsumerReviewService consumerReviewService
            , IAgentService agentService
            , IQuoteReqCarInfoService quoteReqCarInfoService
            , IEnterpriseAgentService enterpriseAgentService
            , IUserinfoRenewalInfoService userinfoRenewalInfoService
            , ICustomerCategories customercategoriesService
            , IAgentRepository agentRepository
            , ICustomerStatusService customerStatusService
            , IUserInfoRepository userInfoRepository
            )
        {
            _consumerReviewService = consumerReviewService;
            _agentService = agentService;
            _quoteReqCarInfoService = quoteReqCarInfoService;
            _enterpriseAgentService = enterpriseAgentService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            _customercategoriesService = customercategoriesService;
            _agentRepository = agentRepository;
            _customerStatusService = customerStatusService;
            _userInfoRepository = userInfoRepository;
        }

        public List<CustomerExportViewModel> ConvertToViewModelExport(List<GetCustomerViewModel> list, int topAgent, bool specialDistribute, int currentAgent)
        {

            if (!list.Any())
                return new List<CustomerExportViewModel>();

            //从bx_quotereq_carinfo获取所有的is_newcar
            List<long> listBuid = list.Select(o => o.Id).ToList();
            List<IsNewCarViewModel> listNewCar = _quoteReqCarInfoService.GetIsNewCarList(listBuid);
            //根据buid获取bx_userinfo_renewal_info表的client_mobile
            List<MinUserInfoRenewalInfo> listRenewalInfo = _userinfoRenewalInfoService.FindBuIdAndClientMobile(listBuid);
            //查询所有的bx_customercategories
            List<bx_customercategories> categoryList = _customercategoriesService.GetCategoriesList(topAgent);

            var customerStatus = _customerStatusService.GetCustomerStatus(topAgent, -1, false, true);

            bool isHasRenewalInfo = listRenewalInfo.Count > 0;

            //根据List=>Agent属性,获取到bx_agent中的AgentName;
            var agentIds = new List<int>();
            var enumAgentId = list.Select(o => o.Agent);
            foreach (var item in enumAgentId)
            {
                agentIds.Add(int.Parse(item));
            }
            var agentNames = _agentRepository.GetAgentName(agentIds);

            var modelList = new List<CustomerExportViewModel>();

            //获取计划回访时间
            List<ConsumerReviewModel> nextReviewDateList = _consumerReviewService.GetConsumerReview(listBuid);

            List<GetCustomerViewModel> listFill = _userInfoRepository.FillCustomerInformation(listBuid);
            foreach (var item in list)
            {
                var model = new CustomerExportViewModel();



                model.CarVIN = item.CarVIN;
                model.EngineNo = item.EngineNo;
                model.InsuredIdCard = item.InsuredIdCard;
                model.InsuredName = item.InsuredName;
                //去年所上保险公司 -1获取失败
                model.LastYearSource = item.LastYearSource.HasValue ? item.LastYearSource.Value : -1;
                if (model.LastYearSource > -1)
                {
                    model.LastYearSourceName = ((int)model.LastYearSource).ToEnumDescription(typeof(EnumSource));
                    if (item.LastYearSource == 34359738368)
                    {
                        model.LastYearSourceName = "众诚车险";
                    }
                }
                else
                {
                    model.LastYearSourceName = "";
                }
                model.Organization = string.IsNullOrEmpty(item.Organization) ? "" : item.Organization;
                model.RenewalStatus = item.RenewalStatus.HasValue ? item.RenewalStatus.Value : -1;
                //查询状态 -1查询失败 0未取到险种 1查询成功
                string statusName = string.Empty;
                model.RenewalStatus = GetReInfoState(item.NeedEngineNo.HasValue ? item.NeedEngineNo.Value : 0,
                    model.RenewalStatus, model.LastYearSource, out statusName);
                model.StatusName = statusName;
                model.LicenseNo = item.LicenseNo;
                model.LicenseOwner = item.LicenseOwner;
                model.MoldName = item.MoldName;
                model.RegisterDate = item.RegisterDate;
                //客户状态编码 
                model.IsReView = item.IsReView ?? 14;//  ChangeViewStatus(item.IsReView);
                //客户状态
                model.ResultStatus = customerStatus.Where(o => o.T_Id == model.IsReView).Select(o => o.StatusInfo).FirstOrDefault();
                //model.IsReView.ToEnumDescription(typeof(EnumCustomerStatus));

                var bizForce = SettingBizForceDate(item);
                model.LastBizEndDate = bizForce.Item1;
                model.LastForceEndDate = bizForce.Item2;

                model.UpdateTime = item.UpdateTime.HasValue ? item.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                model.CameraTime = item.CameraTime.ToString("yyyy-MM-dd HH:mm:ss") == "1970-01-01 00:00:00" ? string.Empty : item.CameraTime.ToString("yyyy-MM-dd HH:mm:ss");
                model.CameraName = item.CameraName == null ? "" : item.CameraName.ToString();
                var intAgent = int.Parse(item.Agent);
                var agent = agentNames.FirstOrDefault(o => o.AgentId == intAgent);
                if (item.IsDistributed == 0 || (specialDistribute && intAgent == currentAgent))
                {
                    model.AgentName = "未分配";
                }
                else
                {//已分配
                    model.AgentName = agent != null ? (agent.IsUsed == 2 ? agent.AgentName + "(已禁)" : agent.AgentName) : "";
                }
                if (listNewCar != null)
                {
                    IsNewCarViewModel isNewCar = listNewCar.FirstOrDefault(o => o.Buid == item.Id);
                    if (isNewCar != null)
                    {
                        model.IsNewCar = isNewCar.IsNewCar.Value;
                    }
                }

                //是否含有用户信息
                if (isHasRenewalInfo)
                {
                    var renewalInfo = listRenewalInfo.Where(o => o.BuId == item.Id).FirstOrDefault();
                    if (renewalInfo != null)
                    {
                        model.ClientMobile = renewalInfo.ClientMobile;
                        model.ClientMobileOther = renewalInfo.ClientMobileOther;
                        model.Remark = renewalInfo.Remark;
                        model.ClientName = renewalInfo.ClientName;
                        model.IntentionRemark = renewalInfo.IntentionRemark;
                        model.ClientAddress = renewalInfo.ClientAddress;
                    }
                }

                var exportAdd = listFill.Where(o => o.Id == item.Id).FirstOrDefault();
                if (exportAdd != null)
                {
                    model.LastReviewTime = exportAdd.LastReviewTime == null ? "" : exportAdd.LastReviewTime.ToString();
                    model.LastReviewContent = exportAdd.LastReviewContent == null ? "" : exportAdd.LastReviewContent;
                    model.DefeatReason = exportAdd.DefeatReason == null ? "" : exportAdd.DefeatReason;
                }


                //根据用户的CategoryInfoId 获取用户类别信息
                var category = categoryList.Where(t => t.Id == item.CategoryInfoId).FirstOrDefault();
                model.CategoryInfo = category != null ? category.CategoryInfo : "";
                //计划回访时间
                var nextReviewDateModel = nextReviewDateList.Where(a => a.b_uid == item.Id).FirstOrDefault();
                if (nextReviewDateModel != null)
                {
                    model.NextReviewDate = nextReviewDateModel.next_review_date.HasValue ? (nextReviewDateModel.next_review_date.Value.Date == DateTime.MinValue.Date ? "" : nextReviewDateModel.next_review_date.Value.ToString("yyyy-MM-dd HH:mm")) : "";
                }
                else
                {
                    model.NextReviewDate = "";
                }
                modelList.Add(model);
            }




            return modelList;
        }

        public async Task<List<CustomerViewModel>> ConvertToViewModelTopLevelAsync(List<GetCustomerViewModel> list, bool specialDistribute, int currentAgent, int agentLevel = 0)
        {
            if (!list.Any())
            {
                return new List<CustomerViewModel>();
            }

            List<long> listBuid = list.Select(o => o.Id).ToList();
            // 获取本年回访数量
            var taskReviewCount = _consumerReviewService.GetYearReviewCountAsync(listBuid);

            // 获取agentName
            var listStrAgentId = list.Where(o => o.IsDistributed != 0).Select(o => o.Agent)
                                .Distinct()
                                .ToList();
            var listAgentId = listStrAgentId.ConvertAll(o => Convert.ToInt32(o));
            List<AgentIdAndAgentName> listAgentName = new List<AgentIdAndAgentName>();
            if (listAgentId.Count > 0)
            {
                listAgentName = _agentService.GetAgentNames(listAgentId);
            }

            // 从bx_quotereq_carinfo获取所有的is_newcar
            string strBuid = string.Join(",", listBuid);
            List<IsNewCarViewModel> listNewCar = _quoteReqCarInfoService.GetIsNewCarList(listBuid);
            var quoteStatuslist = _enterpriseAgentService.GetQuoteStatus(strBuid);

            var listReviewCount = await taskReviewCount;

            var modelList = new List<CustomerViewModel>();
            foreach (var item in list)
            {
                var model = new CustomerViewModel();
                model.CameraId = item.CameraId;
                model.CameraName = item.CameraName;

                //2:禁用、1:启用
                var agentModelTemp = listAgentName.Where(o => o.AgentId == int.Parse(item.Agent)).FirstOrDefault();
                if (agentModelTemp != null)
                {
                    model.IsUsed = agentModelTemp.IsUsed;
                }
                model.Agent = item.Agent;
                model.IsDistributed = item.IsDistributed;
                int intAgent = int.Parse(item.Agent);
                if (model.IsDistributed == 0 || (specialDistribute && intAgent == currentAgent))
                {
                    model.IsDistributed = 1;
                    model.AgentName = "未分配";
                }
                else
                {//已分配
                    model.IsDistributed = 2;
                    var agentObj = listAgentName.Where(o => o.AgentId == int.Parse(item.Agent)).FirstOrDefault();

                    model.AgentName = agentObj == null ? "" : agentObj.AgentName;

                }

                //去年所上保险公司 -1获取失败
                model.LastYearSource = item.LastYearSource.HasValue ? item.LastYearSource.Value : -1;
                if (model.LastYearSource > -1)
                {
                    var lastYearSource = (int)model.LastYearSource;
                    model.LastYearSourceName = lastYearSource.ToEnumDescription(typeof(EnumSource));
                    if (model.LastYearSource == 34359738368)
                    {
                        model.LastYearSourceName = "众诚车险";
                    }
                }
                else
                {
                    model.LastYearSourceName = "";
                }

                //上年机构名称
                model.Organization = string.IsNullOrEmpty(item.Organization) ? "" : item.Organization;

                model.QuoteContent = new List<GetQuoteStatusViewModel>();
                if (item.QuoteStatus > -1)
                {
                    model.QuoteContent = quoteStatuslist.Where(x => x.Buid == item.Id).ToList();
                }

                model.CreateTime = item.CreateTime.ToString();
                model.Id = item.Id;
                model.LicenseNo = item.LicenseNo;
                model.ClientName = string.IsNullOrEmpty(item.ClientName) ? "" : item.ClientName;
                model.CarVIN = item.CarVIN;
                model.LicenseOwner = item.LicenseOwner;
                model.InsuredName = item.InsuredName;
                model.MoldName = item.MoldName;
                //model.QuoteStatus = item.QuoteStatus;
                model.RegisterDate = item.RegisterDate;
                //model.RenewalStatus = item.RenewalStatus;
                model.RenewalType = item.RenewalType;
                model.UpdateTime = item.UpdateTime.ToString();
                model.IsReView = item.IsReView ?? 14;// ChangeViewStatus(item.IsReView);
                //model.ResultStatus = model.IsReView.ToEnumDescription(typeof(EnumCustomerStatus));
                if (item.NextReviewDate.HasValue)
                {
                    model.NextReviewDate =
                        item.NextReviewDate.Value.Date == DateTime.MinValue.Date
                        ? "" : item.NextReviewDate.Value.ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    model.NextReviewDate = "";
                }

                model.RenewalStatus = item.RenewalStatus ?? 0;

                var bizForce = SettingBizForceDate(item);
                model.LastBizEndDate = bizForce.Item1;
                model.LastForceEndDate = bizForce.Item2;

                if (listNewCar != null)
                {
                    IsNewCarViewModel isNewCar = listNewCar.FirstOrDefault(o => o.Buid == item.Id);
                    if (isNewCar != null)
                    {
                        model.IsNewCar = isNewCar.IsNewCar.Value;
                    }
                }

                //摄像头进店时间
                model.CameraTime = (item.CameraTime.Year == 1970 || item.CameraTime.Year == DateTime.MinValue.Year) ? "" : item.CameraTime.ToString("yyyy-MM-dd HH:mm:ss");
                model.DistributedTime = (item.DistributedTime.Year == 1970 || item.DistributedTime.Year == DateTime.MinValue.Year) ? "" : item.DistributedTime.ToString("yyyy-MM-dd HH:mm:ss");

                //客户类别
                model.ClientCategoryID = item.CategoryInfoId;
                // 计划回访时间
                model.NextReviewDate = item.NextReviewDate.HasValue ? item.NextReviewDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

                model.LastReviewContent = item.LastReviewContent;
                model.LastReviewTime = item.LastReviewTime.HasValue ? item.LastReviewTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                if (listReviewCount != null && listReviewCount.Count > 0)
                {
                    var reviewCount = listReviewCount.Where(o => o.BuId == item.Id).FirstOrDefault();
                    model.ReviewCountThisYear = reviewCount == null ? 0 : reviewCount.ReviewCount;
                }
                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// 新旧状态转换
        /// </summary>
        /// <param name="oldStatus"></param>
        /// <returns></returns>
        private int ChangeViewStatus(int? oldStatus)
        {
            int status = 0;
            switch (oldStatus)
            {
                //未回访
                case 0:
                case 18:
                    status = 18; break;
                ////流失
                //case 4:
                //    status = 4; break;
                //忙碌中待联系
                case 5:
                case 7:
                case 8:
                case 15:
                    status = 5;
                    break;
                ////已预约出单
                //case 6: status = 6; break;
                ////已出单
                //case 9: status = 9; break;
                ////已报价考虑中（重点）
                //case 13: status = 13; break;
                //其他
                case 3:
                case 10:
                case 11:
                case 12:
                case 14:
                    status = 14; break;
                ////无效数据（停机、空号）
                //case 16:
                //    status = 16;
                //    break;
                //已报价考虑中（普通）
                case 1:
                case 2:
                case 17:
                    status = 17; break;
                //case 19:
                //    //成功出单
                //    status = 19;
                //    break;
                //其他 无需合并的只需返回当前传入的值即可，这样每增加一个枚举，无需修改此处
                default:
                    status = oldStatus.HasValue ? oldStatus.Value : 14;
                    break;
            }
            return status;
        }

        /// <summary>
        /// 设置交强和商业险到期时间
        /// </summary>
        /// <param name="item"></param>
        /// <returns>item1:lastBizEndDate,item2:lastForceEndDate</returns>
        public Tuple<string, string, string, string> SettingBizForceDate(GetCustomerViewModel item)
        {
            var lastBizEndDate = string.Empty;
            var lastForceEndDate = string.Empty;

            var lastBizEndDate1 = string.Empty;
            var lastForceEndDate1 = string.Empty;

            //var lastYearSource = item.LastYearSource ?? -1;
            //if (item.RenewalStatus.HasValue)
            //{
            //    if (item.RenewalStatus.Value == 1 || lastYearSource > -1)
            //    {
            // 商业险到期时间
            if (item.LastBizEndDate.HasValue)
            {
                lastBizEndDate =
                    (item.LastBizEndDate.Value.Year <= 1970)
                    ? "" : item.LastBizEndDate.Value.ToString("yyyy-MM-dd");
                lastBizEndDate1 =
                    (item.LastBizEndDate.Value.Year <= 1970)
                    ? "" : item.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            // 交强险到期时间
            if (item.LastForceEndDate.HasValue)
            {
                lastForceEndDate =
                    (item.LastForceEndDate.Value.Year <= 1970)
                    ? "" : item.LastForceEndDate.Value.ToString("yyyy-MM-dd");
                lastForceEndDate1 =
                    (item.LastForceEndDate.Value.Year <= 1970)
                    ? "" : item.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //    }
            //}
            return new Tuple<string, string, string, string>(lastBizEndDate, lastForceEndDate, lastBizEndDate1, lastForceEndDate1);
        }

        /// <summary>
        /// 获取续保状态
        /// </summary>
        /// <param name="needEngineNo"></param>
        /// <param name="renewalStatus"></param>
        /// <param name="lastYearSource"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        private int GetReInfoState(int needEngineNo, int renewalStatus, long lastYearSource, out string stateName)
        {
            int state = 2;
            stateName = "续保失败";
            if (needEngineNo == 0 && renewalStatus == 0)
            {//只获取到车辆信息，未取到险种
                state = 3;
                stateName = "只取到行驶本";
            }
            else if (renewalStatus == 1)
            {//续保成功
                state = 1;
                stateName = "续保成功";
            }
            else if (needEngineNo == 1 && renewalStatus == 0)
            {//续保失败
                state = 2;
                stateName = "续保失败";
            }
            else if (renewalStatus == -1)
            {//未处理
                state = 4;
                stateName = "未处理";
            }
            return state;
        }

        public List<MyAppInfo> ConvertToViewModelApp(List<GetCustomerViewModel> list)
        {
            if (!list.Any())
                return new List<MyAppInfo>();

            List<long> listBuid = list.Select(o => o.Id).ToList();
            string strBuid = string.Join(",", listBuid);
            //根据ID获取所有车辆报价核保信息
            var quoteStatuslist = _enterpriseAgentService.GetQuoteStatusForApp(strBuid);

            var modelList = new List<MyAppInfo>();
            foreach (var item in list)
            {
                var model = new MyAppInfo();
                //列表没用车架号和发动机号，sql也没查
                model.CarVIN = "";
                model.EngineNo = "";
                model.ItemChildAgent = item.Agent;
                model.ItemCustKey = item.OpenId;
                model.CityCode = item.CityCode;
                //model.IsDistributed = item.IsDistributed;
                //if (model.IsDistributed == 0)
                //{
                //    model.IsDistributed = 1;
                //    model.AgentName = "未分配";
                //}
                //else
                //{//已分配
                //    model.IsDistributed = 2;
                //    model.AgentName = agentRepository.GetAgentName(int.Parse(item.Agent));
                //}

                //去年所上保险公司 - 1获取失败
                model.LastYearSource = (int)(item.LastYearSource ?? -1);
                if (model.LastYearSource > -1)
                {
                    var LastYearSource = (int)model.LastYearSource;
                    model.LastYearSourceName = LastYearSource.ToEnumDescription(typeof(EnumSource));
                    if (item.LastYearSource == 34359738368)
                    {
                        model.LastYearSourceName = "众诚车险";
                    }
                }

                model.PrecisePrice = new List<GetQuoteStatusForAppViewModel>();
                if (item.QuoteStatus > -1)
                {
                    model.IsPrecisePrice = 1;
                    model.PrecisePrice = quoteStatuslist.Where(x => x.Buid == item.Id).ToList();
                }

                model.CreateTime = item.CreateTime.ToString();
                model.Buid = item.Id;

                var bizForce = SettingBizForceDate(item);
                model.BusinessExpireDate = bizForce.Item1;
                model.ForceExpireDate = bizForce.Item2;

                //赋值到期时间
                model.ExpireDateNum = 0;
                if (!string.IsNullOrEmpty(bizForce.Item4))
                {
                    model.ExpireDateNum = Infrastructure.Helpers.AppHelpers.TimeHelper.GetDayMinus(DateTime.Parse(bizForce.Item4), DateTime.Now);
                }
                else if (!string.IsNullOrEmpty(bizForce.Item3))
                {
                    model.ExpireDateNum = Infrastructure.Helpers.AppHelpers.TimeHelper.GetDayMinus(DateTime.Parse(bizForce.Item3), DateTime.Now);
                }

                model.QuoteBusinessExpireDate = model.BusinessExpireDate;


                model.QuoteForceExpireDate = model.ForceExpireDate;
                model.LicenseNo = item.LicenseNo;
                model.LicenseOwner = item.LicenseOwner ?? "";
                //model.InsuredName = item.InsuredName;
                model.MoldName = item.MoldName ?? "";
                //model.QuoteStatus = item.QuoteStatus;
                model.RegisterDate = item.RegisterDate ?? "";
                //model.RenewalStatus = item.RenewalStatus;
                //model.RenewalType = item.RenewalType;
                model.CreateTime = item.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                //model.IsReView = ChangeViewStatus(item.IsReView);
                //model.ResultStatus = model.IsReView.ToEnumDescription(typeof(EnumCustomerStatus));

                if (item.RenewalStatus != null)
                    model.RenewalStatus = Convert.ToInt32(item.RenewalStatus);

                //摄像头进店时间
                //model.CameraTime = (item.CameraTime.Year == 1970 || item.CameraTime.Year == DateTime.MinValue.Year) ? "" : item.CameraTime.ToString("yyyy-MM-dd HH:mm:ss");
                //model.DistributedTime = (item.DistributedTime.Year == 1970 || item.DistributedTime.Year == DateTime.MinValue.Year) ? "" : item.DistributedTime.ToString("yyyy-MM-dd HH:mm:ss");

                //客户类别
                //model.ClientCategoryID = item.CategoryInfoId;
                model.NextBusinessStartDate = item.BizStartDate ?? "";
                model.NextForceStartDate = item.ForceStartDate ?? "";
                modelList.Add(model);
            }
            return modelList;
        }
    }
}
