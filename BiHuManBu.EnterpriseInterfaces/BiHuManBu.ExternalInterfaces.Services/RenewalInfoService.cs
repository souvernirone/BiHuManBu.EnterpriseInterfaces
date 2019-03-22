using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Configuration;
using log4net;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using MetricsLibrary;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class RenewalInfoService : IRenewalInfoService
    {
        private IRenewalInfoRepository _renewalInfoRepository;
        private IAgentRepository _agentRepository;
        readonly ICustomerCategories _customerCategories;
        readonly IDefeatReasonHistoryService _defeatReasonHistoryService;
        private readonly IUserinfoExpandRepository _userinfoExpandRepository;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ICacheHelper _cacheHelper;
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IBatchRenewalRepository _batchRenewalRepository;
        private readonly IAgentUkeyRepository _agentUkeyRepository;
        public RenewalInfoService(IRenewalInfoRepository renewalInfoRepository, IAgentRepository agentRepository, ICustomerCategories _customerCategories, IDefeatReasonHistoryService _defeatReasonHistoryService, ICacheHelper _cacheHelper, IConsumerDetailService _consumerDetailService, IUserinfoRenewalInfoService userinfoRenewalInfoService, IUserinfoExpandRepository userinfoExpandRepository, IUserInfoRepository userInfoRepository, IBatchRenewalRepository batchRenewalRepository, IAgentUkeyRepository agentUkeyRepository)
        {
            _userinfoExpandRepository = userinfoExpandRepository;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            _renewalInfoRepository = renewalInfoRepository;
            _agentRepository = agentRepository;
            this._customerCategories = _customerCategories;
            this._defeatReasonHistoryService = _defeatReasonHistoryService;
            this._consumerDetailService = _consumerDetailService;
            _userInfoRepository = userInfoRepository;
            _batchRenewalRepository = batchRenewalRepository;
            _agentUkeyRepository = agentUkeyRepository;
        }
        public async Task<bx_userinfo> GetUserInfoAsync(long buId, List<string> agentIds)
        {
            return await _renewalInfoRepository.GetUserInfoAsync(buId, agentIds);
        }

        public async Task<CustomerInfo> GetCustomerInfoAsync(long buId)
        {

            return await _renewalInfoRepository.GetCustomerInfoAsync(buId);
        }

        public async Task<bx_quotereq_carinfo> GetQuotereqCarInfoAsync(long buId)
        {
            return await _renewalInfoRepository.GetQuotereqCarInfoAsync(buId);
        }

        public async Task<PreRenewalInfo> GetCarRenwalInfoAsync(long buId)
        {

            var result = await _renewalInfoRepository.GetCarRenwalInfoAsync(buId);
            if (!string.IsNullOrWhiteSpace(result.SheBei))
            {
                var sheBeiTempList = Infrastructure.Helper.JsonHelper.DeSerialize<List<SheBeiTemp>>(result.SheBei);
                List<SheBei> sheBeis = new List<SheBei>();
                foreach (var item in sheBeiTempList)
                {
                    SheBei sheBei = new SheBei();
                    sheBei.DA = item.device_amount;
                    sheBei.DD = item.device_depreciationamount;
                    sheBei.DN = item.device_name;
                    sheBei.DQ = item.device_quantity;
                    sheBei.DT = item.device_type;
                    sheBei.PD = item.purchase_date.HasValue ? item.purchase_date.Value.ToString("yyyy-MM-dd") : "";
                    sheBeis.Add(sheBei);
                }
                result.SheBeis = sheBeis;
            }
            return result;
        }


        public async Task<bool> SaveCustomerInfoAsync(CustomerInfo customerInfo)
        {
            if (!string.IsNullOrEmpty(customerInfo.TagId))
            {
                var str = customerInfo.TagId;
                var info = _userinfoRenewalInfoService.GetByBuid((int)customerInfo.BuId);
                if (info != null)
                {
                    info.TagId = str;
                }
                if (_userinfoRenewalInfoService.Update(info))
                {
                    return await _renewalInfoRepository.SaveCustomerInfoAsync(customerInfo);
                }
                else
                {
                    return false;
                }
            }
            return await _renewalInfoRepository.SaveCustomerInfoAsync(customerInfo);

        }

        public async Task<bool> SaveCarInfoAsync(CarInfo carInfo)
        {

            bx_userinfo userInfo = new bx_userinfo()
            {
                Id = carInfo.BuId,
                CarVIN = string.IsNullOrWhiteSpace(carInfo.CarVin) ? "" : carInfo.CarVin.Trim().ToUpper(),
                EngineNo = string.IsNullOrWhiteSpace(carInfo.EngineNo) ? "" : carInfo.EngineNo.Trim().ToUpper(),
                LicenseNo = string.IsNullOrWhiteSpace(carInfo.LicenseNo) ? "" : carInfo.LicenseNo.Trim().ToUpper(),


                MoldName = carInfo.MoldName,
                RegisterDate = carInfo.RegisterDate,

                RenewalCarType = carInfo.RenewalCarType,
                UpdateTime = DateTime.Now

            };
            bx_quotereq_carinfo quotereqCarInfo = new bx_quotereq_carinfo()
            {
                is_loans = carInfo.IsLoans,
                id = carInfo.QuotereqCarInfoId,
                car_used_type = carInfo.CarUsedType,
                is_public = userInfo.OwnerIdCardType == 1 ? 2 : 1,
                seat_count = carInfo.SeatCount,
                exhaust_scale = carInfo.ExhaustScale,
                transfer_date = carInfo.IsTransferCar ? Convert.ToDateTime(carInfo.TransferDate) as DateTime? : null,
                pa_remark = carInfo.Pa_Remark,
                auto_model_code = carInfo.VehicleNo,
                PriceT = carInfo.PurchasePrice,
                drivlicense_cartype_value = carInfo.DrivlicenseCartypeValue,
                car_ton_count = carInfo.CarTonCount,
                is_truck = carInfo.IsTruck,
                moldcode_company = carInfo.MoldcodeCompany,
                VehicleAlias = carInfo.VehicleAlias
            };

            return await _renewalInfoRepository.SaveCarInfoAsync(userInfo, quotereqCarInfo, carInfo.IsChangeForAuotModelCode, carInfo.BatchItemId);
        }

        /// <summary>
        /// 保存UserInfo主表信息
        /// </summary>
        /// <param name="userInfo">userInfo</param>
        /// <returns>bool</returns>
        public bool SaveUserInfo(UserInfo userInfo)
        {
            bx_userinfo bx_userInfo = new bx_userinfo()
            {
                Id = userInfo.Id,
                LicenseOwner = userInfo.LicenseOwner,
                OwnerIdCardType = userInfo.OwnerIdCardType,
                IdCard = userInfo.OwnerIdCard,
                InsuredName = userInfo.InsuredName,
                InsuredIdType = userInfo.InsuredIdType,
                InsuredIdCard = userInfo.InsuredIdCard,
                InsuredMobile = userInfo.InsuredMobile,
                InsuredAddress = userInfo.InsuredAddress,
                InsuredEmail = userInfo.InsuredEmail,
                HolderName = userInfo.HolderName,
                HolderIdType = userInfo.HolderIdType,
                HolderIdCard = userInfo.HolderIdCard,
                HolderMobile = userInfo.HolderMobile,
                HolderEmail = userInfo.HolderEmail,
                OwnerSex = userInfo.OwnerSex,
                OwnerBirthday = userInfo.OwnerBirthday,
                InsuredSex = userInfo.InsuredSex,
                InsuredBirthday = userInfo.InsuredBirthday,
                HolderSex = userInfo.HolderSex,
                HolderBirthday = userInfo.HolderBirthday,
                IsChangeRelation = userInfo.IsChangeRelation,
                HolderNation = userInfo.HolderNation,
                HolderIssuer = userInfo.HolderIssuer,
                HolderCertiStartdate = userInfo.HolderCertiStartdate,
                HolderCertiEnddate = userInfo.HolderCertiEnddate,
                InsuredNation = userInfo.InsuredNation,
                InsuredIssuer = userInfo.InsuredIssuer,
                InsuredCertiStartdate = userInfo.InsuredCertiStartdate,
                InsuredCertiEnddate = userInfo.InsuredCertiEnddate,
                HolderAddress = userInfo.HolderAddress,
                UpdateTime = DateTime.Now
            };
            return _renewalInfoRepository.SaveUserInfo(bx_userInfo);
        }

        public BaseViewModel SavePreRenewalInfoAsync(bx_car_renewal carRenewal)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 当成功出单的时候进行抓单处理
        /// </summary>
        /// <param name="consumerReview"></param>
        private void GetPolicyOrRenewal(ConsumerReview consumerReview)
        {
            try
            {
                //没有渠道直接返回
                if (consumerReview.ChannelId == -1)
                {
                    return;
                }
                //保单号和车牌号都没有，直接返回
                if (string.IsNullOrEmpty(consumerReview.LicenseNo) && string.IsNullOrEmpty(consumerReview.PolicyNo))
                {
                    return;
                }
                //根据报价渠道获取工号(抓单渠道）
                bx_agent_ukey ukeyModel = _agentUkeyRepository.GetUKeyByConfigId(consumerReview.ChannelId);
                if (ukeyModel == null || ukeyModel.id <= 0)
                {
                    return;
                }
                //出单成功
                string urlPolicy = CreatePolicyUrl(consumerReview, ukeyModel);//获取保单
                var policyResult = HttpHelper.GetHttpClientAsync(urlPolicy);
                ForeignViewModel foreinModel = null;
                if (!string.IsNullOrEmpty(policyResult))
                {
                    foreinModel = JsonHelper.DeSerialize<ForeignViewModel>(policyResult);
                }
                if (foreinModel != null && foreinModel.BusinessStatus != 1)
                {
                    LogHelper.Info("录入跟进-成功出单-抓单操作：URL:" + urlPolicy + Environment.NewLine + "返回信息：" + policyResult);
                }

            }
            catch (Exception ex)
            {
                MetricUtil.UnitReports("GetPolicyOrRenewal_service");
                LogHelper.Error("发生异常：" + ex + Environment.NewLine + JsonHelper.Serialize(consumerReview));
            }

        }
        private GetReInfoResponse2 GetReInfo(ConsumerReview consumerReview)
        {
            GetReInfoResponse2 reInfoModel = null;
            string reInfoUrl = CreateReInfoUrl(consumerReview);
            if (!string.IsNullOrEmpty(reInfoUrl))
            {
                var reInfoResult = HttpHelper.GetHttpClientAsync(reInfoUrl);
                reInfoModel = JsonHelper.DeSerialize<GetReInfoResponse2>(reInfoResult);
                UpdateBatchRenewalTime(consumerReview, reInfoModel);
            }
            return reInfoModel;
        }
        private void UpdateBatchRenewalTime2(ConsumerReview consumerReview, ForeignViewModel foreinModel)
        {
            Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
            string BizEndTime = "";
            if (consumerReview.BizEndTime.Value > foreinModel.Data.BizEndDate.Value)
            {
                BizEndTime = consumerReview.BizEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            string ForceEndTime = "";
            if (consumerReview.ForceEndTime.Value > foreinModel.Data.ForceEndDate.Value)
            {
                ForceEndTime = consumerReview.ForceEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            _batchRenewalRepository.UpdateBatchRenewalItemByBuid(consumerReview.BuId, consumerReview.Source, ForceEndTime, BizEndTime);
        }
        private void UpdateBatchRenewalTime(ConsumerReview consumerReview, GetReInfoResponse2 reInfo)
        {
            Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
            string BizEndTime = "";
            if (consumerReview.BizEndTime.Value > DateTime.Parse(reInfo.UserInfo.BusinessExpireDate))
            {
                BizEndTime = consumerReview.BizEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            string ForceEndTime = "";
            if (consumerReview.ForceEndTime.Value > DateTime.Parse(reInfo.UserInfo.ForceExpireDate))
            {
                ForceEndTime = consumerReview.ForceEndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            _batchRenewalRepository.UpdateBatchRenewalItemByBuid(consumerReview.BuId, consumerReview.Source, ForceEndTime, BizEndTime);
        }
        /// <summary>
        /// 组装续保URL
        /// </summary>
        /// <param name="consumerReview"></param>
        /// <returns></returns>
        private string CreateReInfoUrl(ConsumerReview consumerReview)
        {
            bx_userinfo userModel = _userInfoRepository.GetUserInfo(consumerReview.BuId);
            Dictionary<string, string> queryparm = new Dictionary<string, string>();
            queryparm.Add("ChildAgent", userModel.Agent);
            queryparm.Add("CustKey", userModel.OpenId);
            if (!string.IsNullOrWhiteSpace(userModel.LicenseNo))
            {
                queryparm.Add("LicenseNo", userModel.LicenseNo.ToUpper());
                if (!string.IsNullOrWhiteSpace(userModel.SixDigitsAfterIdCard))
                {
                    queryparm.Add("SixDigitsAfterIdCard", userModel.SixDigitsAfterIdCard);
                }
                //号牌种类
                queryparm.Add("RenewalCarType", userModel.RenewalCarType.ToString());
            }
            //获取号牌种类默认值为1（获取）  
            queryparm.Add("ShowRenewalCarType", "1");
            queryparm.Add("CityCode", userModel.CityCode.ToString());
            queryparm.Add("Group", "1");//续保新接口
            if (!string.IsNullOrEmpty(userModel.CarVIN))
            {
                queryparm.Add("CarVin", userModel.CarVIN.ToUpper());
            }
            if (!string.IsNullOrEmpty(userModel.EngineNo))
            {
                queryparm.Add("EngineNo", userModel.EngineNo.ToUpper());
            }
            queryparm.Add("CanShowNo", "1");
            queryparm.Add("Agent", consumerReview.TopAgentId.ToString());
            queryparm.Add("RenewalType", "4");
            queryparm.Add("ShowXiuLiChangType", "1");
            queryparm.Add("ShowInnerInfo", "1");
            queryparm.Add("ShowAutoMoldCode", "1");
            //llj edit 2017.05.24 
            queryparm.Add("ShowFybc", "1");//修理期间费用补偿险：0:（默认）:否  1：是
            queryparm.Add("ShowSheBei", "1");//新增设备险种：0:（默认）:否  1：是
            //llj add 2017.09.15 关系人 性别和出生日期
            queryparm.Add("ShowRelation", "1");
            //添加是否展示 三责险附加法定节假日限额翻倍险 
            queryparm.Add("ShowSanZheJieJiaRi", "1");
            //按照实时起保返回到期日期
            queryparm.Add("TimeFormat", "1");
            string url = ConfigurationManager.AppSettings["BaoJiaJieKou"];
            url = url + "api/CarInsurance/getreinfo" + ToQueryString(queryparm);
            return url;
        }
        /// <summary>
        /// 组装抓取保单URL
        /// </summary>
        /// <param name="consumerReview"></param>
        /// <returns></returns>
        private string CreatePolicyUrl(ConsumerReview consumerReview, bx_agent_ukey ukeyModel)
        {
            string urlPolicy = ConfigurationManager.AppSettings["SettleModifyStateUrl"] + "/api/baodan/GetPolicy";
            Dictionary<string, string> queryparm = new Dictionary<string, string>();
            queryparm.Add("Agent", consumerReview.AgentId.ToString());
            if (!string.IsNullOrEmpty(consumerReview.LicenseNo))
            {
                queryparm.Add("LicenseNo", consumerReview.LicenseNo);
            }
            if (!string.IsNullOrEmpty(consumerReview.PolicyNo))
            {
                queryparm.Add("PolicyNo", consumerReview.PolicyNo);
            }
            //bx_agent_config ac=_agentConfigRepository.GetAgentConfigById(consumerReview.ChannelId)
            queryparm.Add("ChannelId", ukeyModel.id.ToString());
            queryparm.Add("ChannelName", ukeyModel.name);
            queryparm.Add("AgentName", consumerReview.AgentName);
            queryparm.Add("BaoDanType", "0");
            queryparm.Add("BuId", consumerReview.BuId.ToString());
            return urlPolicy = urlPolicy + ToQueryString(queryparm);
        }
        /// <summary>
        /// 将Dictionary对象转换为queryString, 并计算SecCode值。
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>返回http请求querystring字符串</returns>
        private string ToQueryString(Dictionary<string, string> dict)
        {
            var tmp = dict.Where(x => !string.IsNullOrWhiteSpace(x.Value)).OrderBy(y => y.Key);
            StringBuilder data = new StringBuilder();
            string result = string.Join("&", tmp.Select(p => p.Key + '=' + p.Value.Trim()).ToArray());
            return '?' + result + "&SecCode=" + result.GetMd5().ToLower();
        }
        public Tuple<int, bool> SaveConsumerReviewAsync(ConsumerReview consumerReview)
        {
            //针对成功出单进行抓单处理
            //if (consumerReview.ReviewStatus == 9)
            //{
            //    GetPolicyOrRenewal(consumerReview);
            //}
            bool isDelete = false;
            if (consumerReview.BizEndTime.HasValue)
            {
                if (consumerReview.BizEndTime.Value.Month == 2 && consumerReview.BizEndTime.Value.Day == 29)
                {
                    consumerReview.BizEndTime = new DateTime(consumerReview.BizEndTime.Value.Year, 3, 1);
                }
                var newBizEndTime = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-" + consumerReview.BizEndTime.Value.Month + "-" + consumerReview.BizEndTime.Value.Day.ToString() + " " + consumerReview.BizEndTime.Value.Hour + ":" + consumerReview.BizEndTime.Value.Minute + ":" + consumerReview.BizEndTime.Value.Second);
                var nextBizEndTime = newBizEndTime.AddYears(1);
                if (consumerReview.Source != -1)
                {
                    consumerReview.BizEndTime = GetBizEndTime(newBizEndTime, nextBizEndTime, consumerReview.SingleTime.Value);
                }
                else if (consumerReview.DefeatReasonId != -1)
                {
                    consumerReview.BizEndTime = GetBizEndTime(newBizEndTime, nextBizEndTime, consumerReview.CreateTime);
                }
                else
                {
                    consumerReview.BizEndTime = GetBizEndTime(newBizEndTime, nextBizEndTime, consumerReview.ReviewTime.Value);
                }

            }
            bx_consumer_review bxConsumerReview = new bx_consumer_review()
            {
                b_uid = consumerReview.BuId,
                create_time = DateTime.Now,
                content = consumerReview.ReviewContent,
                status = consumerReview.ReviewStatus,
                next_review_date = consumerReview.ReviewTime,
                operatorId = consumerReview.AgentId,
                DefeatReasonId = consumerReview.DefeatReasonId,
                DefeatReasonContent = consumerReview.DefeatReasonContent,
                BizEndTime = consumerReview.BizEndTime,
                singletime = consumerReview.SingleTime,
                ParentStatus = consumerReview.ParentStatus,
                xgAccount = consumerReview.XgAccount

            };
            consumerReview.PreReviewStatus = GetConsumerReviewStatus(consumerReview.BuId);
            var result = _renewalInfoRepository.SaveConsumerReviewAsync(bxConsumerReview);

            isDelete = result.Item2;
            if (result.Item1)
            {
                if (isDelete)
                {
                    if (consumerReview.PreReviewStatus == 9)
                    {
                        DeleteCarOrder(consumerReview.BuId);
                    }
                    else if (consumerReview.PreReviewStatus == 4 || consumerReview.PreReviewStatus == 16)
                    {
                        _defeatReasonHistoryService.DeleteDefeatReasonHistory(consumerReview.BuId);
                    }
                }
                if (consumerReview.Source != -1)
                {
                    //var SearchBuidUnique = _renewalInfoService.SearchBuidUnique(consumerReview.BuId);
                    //if (SearchBuidUnique == 0)
                    //{
                    var keyValuePairs = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Buid",consumerReview.BuId.ToString()),
                            new KeyValuePair<string, string>("Buid",consumerReview.BuId.ToString()),new KeyValuePair<string, string>("Source",consumerReview.Source.ToString()),new KeyValuePair<string, string>("CurAgent",consumerReview.AgentId.ToString()),new KeyValuePair<string, string>("Fountain","4"),new KeyValuePair<string, string>("Agent",consumerReview.TopAgentId.ToString()),new KeyValuePair<string, string>("ChildAgent",consumerReview.AgentId.ToString()),new KeyValuePair<string, string>("OrderStatus","-2"),new KeyValuePair<string, string>("SingleTime",consumerReview.SingleTime.Value.ToString("yyyy-MM-dd"))};

                    var resultMessage = HttpWebAsk.HttpClientPostAsync(
                        JsonHelper.Serialize(new
                        {
                            Buid = consumerReview.BuId,
                            Source = consumerReview.Source,
                            CurAgent = consumerReview.AgentId,
                            Fountain = 4,
                            SingleTime = consumerReview.SingleTime,
                            Agent = consumerReview.TopAgentId,
                            ChildAgent = consumerReview.AgentId,
                            OrderStatus = -2,
                            SecCode = new CommonBehaviorService(_agentRepository, _cacheHelper).GetSecCode(keyValuePairs, null)
                        }), ConfigurationManager.AppSettings["BaoJiaJieKou"] + "api/order/CreateSureOrder");

                    _logInfo.Info(string.Format("请求url为：{0}，请求数据为：{1}，创建保单结果：{2}", ConfigurationManager.AppSettings["BaoJiaJieKou"] + "api/order/CreateSureOrder", JsonHelper.Serialize(new { Buid = consumerReview.BuId, Source = consumerReview.Source, SingleTime = consumerReview.SingleTime, CurAgent = consumerReview.AgentId, Fountain = 4, Agent = consumerReview.TopAgentId, ChildAgent = consumerReview.AgentId, OrderStatus = -2, SecCode = new CommonBehaviorService(_agentRepository, _cacheHelper).GetSecCode(keyValuePairs, null) }), resultMessage));
                    //}
                }


                var updateResultAsync = UpdateUserInfoAsync(consumerReview.BuId, consumerReview.ReviewStatus, consumerReview.CategoryId);

                var consumerReviewInCrm = new
                {
                    AgentId = consumerReview.AgentId,
                    BuId = consumerReview.BuId,
                    ReviewStatus = consumerReview.ReviewStatus,
                    ReviewTime = consumerReview.ReviewTime.HasValue ? consumerReview.ReviewTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                    ReviewContent = consumerReview.ReviewContent,
                    consumerReview.DefeatReasonContent,
                    consumerReview.Source,
                    sourceName = GetSourceName(consumerReview.Source),
                    ConsumerReviewName = consumerReview.ConsumerReviewName,
                    SingleTime = consumerReview.SingleTime.HasValue ? consumerReview.SingleTime.Value.ToString("yyyy-MM-dd") : string.Empty
                };
                _consumerDetailService.AddCrmSteps(new bx_crm_steps() { agent_id = consumerReview.AgentId, b_uid = consumerReview.BuId, create_time = DateTime.Now, type = 1, json_content = JsonHelper.Serialize(consumerReviewInCrm) });

                consumerReview.CategoryId = updateResultAsync.CategoryId;


            }
            return new Tuple<int, bool>(consumerReview.CategoryId, isDelete);

        }
        private DateTime GetBizEndTime(DateTime newBizEndTime, DateTime nextBizEndTime, DateTime sourceTime)
        {
            DateTime useBizEndTime = DateTime.Now;
            if (sourceTime >= nextBizEndTime.AddDays(-180))
            {
                useBizEndTime = nextBizEndTime;
            }
            else if (sourceTime >= newBizEndTime.AddDays(-180))
            {

                useBizEndTime = newBizEndTime;
            }
            else
            {
                useBizEndTime = newBizEndTime.AddYears(-1);
            }
            return useBizEndTime;
        }
        public string GetSourceName(long newSource)
        {
            if (newSource == 2147483648)
            {
                return "恒邦车险";
            }
            if (newSource == 4294967296)
            {
                return "中铁车险";
            }
            if (newSource == 8589934592)
            {
                return "美亚车险";
            }
            if (newSource == 17179869184)
            {
                return "富邦车险";
            }
            if (newSource == 34359738368)
            {
                return "众诚车险";
            }
            return ToEnumDescription(newSource, typeof(EnumSourceNew));
        }
        public static String ToEnumDescription(long value, Type enumType)
        {
            NameValueCollection nvc = GetNvcFromEnumValue(enumType);
            return nvc[value.ToString()];
        }
        public static NameValueCollection GetNvcFromEnumValue(Type enumType)
        {
            var nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }
        public dynamic UpdateUserInfoAsync(long buid, int reviewStatus, int categoryId = 0)
        {
            if (categoryId != 0)
            {
                int type = -1;
                type = reviewStatus == 9 ? 0 : reviewStatus == 4 || reviewStatus == 16 ? 1 : -1;
                if (type != -1)
                {
                    categoryId = _customerCategories.GetCirculationCategoryId(categoryId, type);
                }
            }
            bx_userinfo userInfo = new bx_userinfo()
            {
                Id = buid,
                IsReView = reviewStatus,
                CategoryInfoId = categoryId,
                UpdateTime = DateTime.Now
            };
            if (reviewStatus != 0)
            {
                var userInfoExtend = _userinfoExpandRepository.Get(buid);
                if (userInfoExtend != null)
                {
                    if (userInfoExtend.CarOwnerStatus == 1)
                    {
                        userInfoExtend.CarOwnerStatus = 2;
                        _userinfoExpandRepository.Update(userInfoExtend);
                    };
                }
            }
            //dynamic result = new { IsSuccess = await _renewalInfoRepository.UpdateUserInfoAsync(userInfo), CategoryId = categoryId };
            dynamic result = new System.Dynamic.ExpandoObject();
            result.IsSuccess = _renewalInfoRepository.UpdateUserInfoAsync(userInfo);
            result.CategoryId = categoryId;

            return result;
        }




        public async Task<bx_carinfo> GetPurchasePriceAsync(string licenseNo)
        {
            var carInfo = await _renewalInfoRepository.GetCarInfoAsync(licenseNo);
            return carInfo;
        }


        public async Task<string> GetVehicleName(string vehicleNo)
        {
            return await _renewalInfoRepository.GetVehicleName(vehicleNo);
        }

        public string GetVehicleNameNew(string vehicleNo)
        {
            return _renewalInfoRepository.GetVehicleNameNew(vehicleNo);
        }

        public bool UpdateReqCarInfo(long buId)
        {
            return _renewalInfoRepository.UpdateReqCarInfo(buId);
        }
        public int SearchBuidUnique(long buid)
        {
            return _renewalInfoRepository.SearchBuidUnique(buid);
        }

        public int GetConsumerReviewStatus(long buId)
        {

            return _renewalInfoRepository.GetConsumerReviewStatus(buId);
        }


        public bool DeleteCarOrder(long buId)
        {
            return _renewalInfoRepository.DeleteCarOrder(buId);
        }



        public void UpdateConsumerReviewBizEndDate()
        {
            _renewalInfoRepository.UpdateConsumerReviewBizEndDate();

        }

        /// <summary>
        /// 获取业务需要信息集合（1、用户信息2、关联保险信息3、回访信息4、车辆保险信息5、车辆信息6、代理人信息7、批续信息,8险种保费）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public RenewalInformationViewModel GetRenewalInformation(GetRenewalRequest request, bx_userinfo userinfo)
        {
            string sql = string.Format(@"SELECT * FROM bx_userinfo_renewal_info WHERE b_uid = {0} LIMIT 0,1;
                                        SELECT * FROM bx_consumer_review WHERE b_uid = {0} AND IsDeleted = 0 ORDER BY id DESC LIMIT 0,1;
                                        SELECT * FROM bx_quotereq_carinfo WHERE b_uid = {0} LIMIT 0,1;
                                        SELECT * FROM bx_car_renewal WHERE Id IN (SELECT car_renewal_id FROM bx_userinfo_renewal_index WHERE b_uid = {0} ) LIMIT 0,1;
                                        SELECT * FROM bx_carinfo WHERE license_no = '{1}' ORDER BY create_time,update_time LIMIT 0,1;
                                        SELECT * FROM bx_agent WHERE Id = {2} LIMIT 0,1;
                                        SELECT * FROM bx_batchrenewal_item WHERE BUId = {0} AND IsDelete = 0 AND IsNew = 1 LIMIT 0,1;
                                        SELECT * FROM bx_car_renewal_premium WHERE car_renewal_id=
                                        (SELECT car_renewal_id FROM bx_userinfo_renewal_index WHERE b_uid={0} ORDER BY update_time DESC LIMIT 1) limit 0,1; ", request.Buid, userinfo.LicenseNo, userinfo.Agent);
            //RenewalInformationViewModel model = _renewalInfoRepository.GetRenewalInformation(sql);
            return _renewalInfoRepository.GetRenewalInformation(sql);

        }

        public bx_userinfo GetUserInfo(bx_userinfo info, List<string> agentIds)
        {
            return _renewalInfoRepository.GetUserInfo(info, agentIds);
        }

    }
}
