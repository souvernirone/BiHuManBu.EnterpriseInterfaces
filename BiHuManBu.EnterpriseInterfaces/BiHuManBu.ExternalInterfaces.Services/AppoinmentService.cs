using System.Net.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 预约单服务
    /// </summary>
    public class AppoinmentService : IAppoinmentService
    {
        private IAppoinmentRepository _appoinmentRepository;
        private IUserInfoRepository _userInfoRepository;
        public AppoinmentService(IAppoinmentRepository appoinmentRepository, IUserInfoRepository userInfoRepository)
        {
            _appoinmentRepository = appoinmentRepository;
            _userInfoRepository = userInfoRepository;
        }

        public bool UpdateAppoinmentInfo(AppoinmentInfoRequest request)
        {
            return _appoinmentRepository.UpdateAppoinmentInfo(request);
        }

        public IList<AppointmentOrderRequest> GetOrderList(AppointmentOrderRequest appointmentWhere, out int totalCount)
        {

            return _appoinmentRepository.GetOrderList(appointmentWhere, out totalCount);
        }

        public bx_car_order GetCarOrderByOrderId(long orderId)
        {
            return _appoinmentRepository.GetCarOrderByOrderId(orderId);
        }

        public IList<QuotationReceiptViewModel> GetQuotationReceiptList(QuotationReceiptRequest searchWhere, out int totalCount)
        {
            if (searchWhere.Source != -1)
            {
                searchWhere.Source = searchWhere.Source == 2 ? 0 : searchWhere.Source > 1 && searchWhere.Source != 9999 ? Convert.ToInt32(Math.Log(searchWhere.Source, 2)) : searchWhere.Source;
            }

            var quotationReceiptList = _appoinmentRepository.GetQuotationReceiptList(searchWhere, out totalCount).ToList();
            foreach (var item in quotationReceiptList)
            {
                if (item.Source == 0 || !item.Source.HasValue)
                {
                    item.Source = 2;
                }
                else if (item.Source > 1 && item.Source != 9999)
                {
                    item.Source = Convert.ToInt64(Math.Pow(2, item.Source.Value));
                }
            }

            return quotationReceiptList;

        }


        public HttpResponseMessage GetQuotationReceiptListData(QuotationReceiptRequest request)
        {
            int totalCount;
            var AppoinmentInfoListData = GetQuotationReceiptList(request, out totalCount).Select(x => new
            {
                x.CustomerCategory,
                CreateTime = x.CreateTime.Value.ToString("yyyy-MM-dd"),
                OrderTime = x.SingleTime.HasValue ?
                        x.SingleTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.SingleTime.Value.ToString("yyyy-MM-dd") : "--",
                PolicyId = x.PolicyId,
                BuId = x.BuId,
                Id = x.OrderId,
                HoderName = x.InsuredName,
                ForceStartDate = x.ForceStartDate == null ? "" : x.ForceStartDate == DateTime.MinValue ? "" : x.ForceStartDate.ToString("yyyy-MM-dd"),
                BizStartDate = x.BizStartDate == null ? "" : x.BizStartDate == DateTime.MinValue ? "" : x.BizStartDate.ToString("yyyy-MM-dd"),
                ForceEndDate = x.ForceStartDate == null ? "" : x.ForceStartDate == DateTime.MinValue ? "" : x.ForceStartDate.AddYears(1).ToString("yyyy-MM-dd"),
                BizEndDate = x.BizStartDate == null ? "" : x.BizStartDate == DateTime.MinValue ? "" : x.BizStartDate.AddYears(1).ToString("yyyy-MM-dd"),
                LicenseNo = x.LicenseNo,
                MoldName = x.MoldName,
                Source = x.Source,
                SourceName = x.Source.HasValue ? x.Source == 9999 ? "其他" : (x.Source.Value == 0 ? "平安" : x.Source.Value == 1 ? "太平洋" : "人保") : "平安",
                BusinessRisksDuration = (x.BusinessRisksStartTime.HasValue ? x.BusinessRisksStartTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.BusinessRisksStartTime.Value.ToString("yyyy-MM-dd") : "") + "至" + (x.BusinessRisksEndTime.HasValue ? x.BusinessRisksEndTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.BusinessRisksEndTime.Value.ToString("yyyy-MM-dd") : ""),
                ForceRisksDuration = (x.ForceRisksStartTime.HasValue ? x.ForceRisksStartTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.ForceRisksStartTime.Value.ToString("yyyy-MM-dd") : "") + "至" + (x.ForceRisksEndTime.HasValue ? x.ForceRisksEndTime.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "--" : x.ForceRisksEndTime.Value.ToString("yyyy-MM-dd") : ""),
                AgentName = x.AgentName,
                DistributionType = x.DistributionType.HasValue ? (x.DistributionType.Value == 0 ? "快递保单" : x.DistributionType.Value == 1 ? "网点自取" : "网点配送") : ""
            });

            var obj = new
            {
                AppoinmentInfoListData = AppoinmentInfoListData,
                BusinessStatus = 1,
                StatusMessage = "查询成功",
                TotalCount = totalCount
            };
            return obj.ResponseToJson();
        }
        /// <summary>
        /// 出单成功数据续保，只使用一次
        /// </summary>
        public BaseViewModel UpdateQuotationReceiptOldList()
        {
            BaseViewModel viewModel=new BaseViewModel();
            
		 //获取最大buid
            long maxBuid=_userInfoRepository.GetMaxBuid();
            long startBuid = 1;
            long interval = 10000;
            long endBuid = 10000;
            while (startBuid > maxBuid && endBuid > maxBuid)
            {
                try
                {
                    List<UserInfoModel2> data = _userInfoRepository.GetUserListByQuotationReceipt(startBuid, endBuid);
                    GetReInfo(data);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("开始BUID：(" + startBuid + "),结束BUId：(" + endBuid + ")；发生异常：" + ex);
                    try
                    {
                        List<UserInfoModel2> data = _userInfoRepository.GetUserListByQuotationReceipt(startBuid, endBuid);
                        GetReInfo(data);
                    }
                    catch (Exception ex2)
                    {
                        LogHelper.Error("开始BUID：(" + startBuid + "),结束BUId：(" + endBuid + ")；发生异常：" + ex2);
                    }
                }
                finally
                {
                    startBuid = startBuid + 1;
                    endBuid = startBuid + interval;
                }
            }
            viewModel.BusinessStatus=1;
            viewModel.StatusMessage="出单成功数据刷新续保成功";
            return viewModel;
	
        }
        private void GetReInfo(List<UserInfoModel2> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }
            foreach (UserInfoModel2 userModel in data)
            {
                string url = CreateReInfoUrl(userModel);
                LogHelper.Info("成功出单续保URL：" + url);
                string result = HttpHelper.GetHttpClientAsync(url);

            }
        }
        private string CreateReInfoUrl(UserInfoModel2 model)
        {
            Dictionary<string, string> queryparm = new Dictionary<string, string>();
            queryparm.Add("ChildAgent", model.Agent.ToString());
            queryparm.Add("CustKey", model.OpenId.ToString());
            if (!string.IsNullOrWhiteSpace(model.LicenseNo.ToString()))
            {
                queryparm.Add("LicenseNo", model.LicenseNo.ToUpper());
                if (!string.IsNullOrWhiteSpace(model.SixDigitsAfterIdCard))
                {
                    queryparm.Add("SixDigitsAfterIdCard", model.SixDigitsAfterIdCard);
                }
                //号牌种类
                queryparm.Add("RenewalCarType", model.RenewalCarType.Value.ToString());
            }
            //获取号牌种类默认值为1（获取）  
            queryparm.Add("ShowRenewalCarType", "1");
            queryparm.Add("CityCode", model.CityCode);
            queryparm.Add("Group", "1");//续保新接口
            if (!string.IsNullOrEmpty(model.CarVIN))
            {
                queryparm.Add("CarVin", model.CarVIN.ToUpper());
            }
            if (!string.IsNullOrEmpty(model.EngineNo))
            {
                queryparm.Add("EngineNo", model.EngineNo.ToUpper());
            }
            queryparm.Add("CanShowNo", "1");
            queryparm.Add("Agent", model.TopAgentId.ToString());
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
            url = url + "api/CarInsurance/getreinfo" + queryparm.ToQueryString();
            return url;

        }
    }
}