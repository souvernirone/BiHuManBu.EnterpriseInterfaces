using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class RenewalInfoRepository : IRenewalInfoRepository
    {
        private EntityContext _context;
        private ILog logError;
        private readonly IUserInfoRepository _userInfoRepository;


        public RenewalInfoRepository(IUserInfoRepository userInfoRepository)
        {
            logError = LogManager.GetLogger("ERROR");
            _context = DataContextFactory.GetDataContext();
            _userInfoRepository = userInfoRepository;

        }

        public async Task<bx_userinfo> GetUserInfoAsync(long buId, List<string> agentIds)
        {

            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                 {
                     var userInfoes = _dbContext.bx_userinfo.Where(x => x.Id == buId).ToList();
                     var userInfo = userInfoes.Any(x => agentIds.Contains(x.Agent)) ? userInfoes.SingleOrDefault() : null;
                     if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.CityCode))
                     {
                         int cityId = 0;
                         if (!int.TryParse(userInfo.CityCode, out cityId))
                         {
                             cityId = 0;
                         }
                         if (userInfo.OwnerIdCardType == -1)
                         {
                             var carInfo = _dbContext.bx_carinfo.FirstOrDefault(x => x.license_no == userInfo.LicenseNo);
                             userInfo.OwnerIdCardType = carInfo != null ? (carInfo.owner_idno_type.HasValue ? carInfo.owner_idno_type.Value : -1) : userInfo.OwnerIdCardType;

                         }
                         var city = _dbContext.bx_city.SingleOrDefault(x => x.id == cityId);
                         userInfo.CityName = cityId == 0 ? "" : city == null ? "" : city.city_name;

                     }
                     return userInfo;
                 });
            }
        }
        public bx_userinfo GetUserInfo(bx_userinfo userInfo, List<string> agentIds)
        {
            if (!agentIds.Contains(userInfo.Agent))
            {
                userInfo = null;
            }
            using (var _dbContext = new EntityContext())
            {
                if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.CityCode))
                {
                    int cityId = 0;
                    if (!int.TryParse(userInfo.CityCode, out cityId))
                    {
                        cityId = 0;
                    }
                    if (userInfo.OwnerIdCardType == -1)
                    {
                        var carInfo = _dbContext.bx_carinfo.FirstOrDefault(x => x.license_no == userInfo.LicenseNo);
                        userInfo.OwnerIdCardType = carInfo != null ? (carInfo.owner_idno_type.HasValue ? carInfo.owner_idno_type.Value : -1) : userInfo.OwnerIdCardType;

                    }
                    var city = _dbContext.bx_city.SingleOrDefault(x => x.id == cityId);
                    userInfo.CityName = cityId == 0 ? "" : city == null ? "" : city.city_name;

                }
                return userInfo;
            }
        }
        public async Task<CustomerInfo> GetCustomerInfoAsync(long buId)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    bx_customercategories category = null;
                    var userInfo = _dbContext.bx_userinfo.Where(x => x.Id == buId).FirstOrDefault();
                    if (userInfo != null && userInfo.CategoryInfoId != 0)
                    {
                        category = _dbContext.bx_customercategories.Where(x => x.Id == userInfo.CategoryInfoId).FirstOrDefault();
                    }
                    var categoryInfoName = category == null ? "" : category.CategoryInfo;
                    return _dbContext.bx_userinfo_renewal_info.Where(x => x.b_uid == buId).Select(x => new CustomerInfo
                    {
                        BuId = buId,
                        CustomerMobile = x.client_mobile ?? x.client_mobile_other,
                        CustomerName = x.client_name,
                        CustomerType = x.CustomerType,
                        Remark = x.remark,
                        ClientMobileOther = x.client_mobile_other,
                        TagId = x.TagId,
                        CategoryInfoName = categoryInfoName
                    }).FirstOrDefault();

                });

            }
        }
        public bx_carinfo GetCarInfoAsyncElse(long buId)
        {
            using (var _dbContext = new EntityContext())
            {
                var aa = (from a in _dbContext.bx_carinfo
                          join x in _dbContext.bx_userinfo
                          on a.license_no equals x.LicenseNo
                          where x.Id == buId
                          select a).FirstOrDefault();
                //string sql = string.Format("SELECT license_owner FROM bx_carinfo WHERE license_no=(SELECT LicenseNo FROM bx_userinfo WHERE Id={0})", buId);
                return aa;
            }
        }





        public async Task<bx_quotereq_carinfo> GetQuotereqCarInfoAsync(long buId)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    return _dbContext.bx_quotereq_carinfo.FirstOrDefault(x => x.b_uid == buId);
                });
            }
        }

        /// <summary>
        /// 根据buids查续保信息
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="daysNum">交强险到期时间</param>
        /// <param name="bizDaysNum">商业险到期时间</param>
        /// <returns></returns>
        public List<LoopRenewalViewModel> GetCarRenewals(List<long> buids, int daysNum, int bizDaysNum)
        {
            var renewalInfos = new List<LoopRenewalViewModel>();
            DateTime forceDate = DateTime.Now.AddDays(daysNum - 1);
            DateTime bizDate = DateTime.Now.AddDays(bizDaysNum - 1);
            var sbBuid = new StringBuilder();
            if (buids.Any())
            {
                for (int i = 0; i < buids.Count; i++)
                {
                    sbBuid.Append(buids[i]).Append(",");
                }
            }
            if (sbBuid.Length > 1)
            {
                sbBuid.Remove(sbBuid.Length - 1, 1);
            }
            var strsql = new StringBuilder();
            if (bizDaysNum == 0)
            {
                strsql.Append("SELECT r.b_uid AS Buid,c.LicenseNo,TIMESTAMPDIFF(DAY,NOW(),c.LastForceEndDate) AS DaysNum FROM bx_userinfo_renewal_index r ")
                .Append("LEFT JOIN bx_car_renewal c ON r.car_renewal_id=c.id ")
                .Append(string.Format("WHERE r.b_uid IN ({0}) AND c.LastForceEndDate between '{1} 00:00:00' AND '{2} 23:59:59' ", sbBuid, DateTime.Now.ToString("yyyy-MM-dd"), forceDate.ToString("yyyy-MM-dd")));
            }
            else
            {
                strsql.Append("SELECT r.b_uid AS Buid,cr.LicenseNo ")
                .Append(string.Format(",{0} as LastForceEndDate", CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()))
                .Append(string.Format(",{0} as LastBizEndDate", CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                .Append(" FROM bx_userinfo_renewal_index r")
                .Append(" RIGHT JOIN bx_userinfo ui  ON ui.Id =r.b_uid ")
                .Append(" LEFT JOIN bx_car_renewal cr ON r.car_renewal_id=cr.id ")
                .Append(" left join bx_batchrenewal_item on r.b_uid=bx_batchrenewal_item.BUId")
                .Append(string.Format(" WHERE r.b_uid IN ({0}) ", sbBuid))
                .Append(" AND  ( ")
                .Append(string.Format(" {2} between '{0} 00:00:00' AND '{1} 23:59:59' ", DateTime.Now.ToString("yyyy-MM-dd"), forceDate.ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastForceEndDate()))
                .Append(string.Format(" or {2} between '{0} 00:00:00' AND '{1} 23:59:59' ", DateTime.Now.ToString("yyyy-MM-dd"), bizDate.ToString("yyyy-MM-dd"), CompareBatchAndRenewalDateHelpler.GetLastBizEndDate()))
                .Append(" )");
            }

            //查询列表
            renewalInfos = _context.Database.SqlQuery<LoopRenewalViewModel>(strsql.ToString()).ToList();
            return renewalInfos;
        }

        public async Task<PreRenewalInfo> GetCarRenwalInfoAsync(long buId)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    var carRenewal = (from a in _dbContext.bx_userinfo_renewal_index
                                      join x in _dbContext.bx_car_renewal
                                      on a.car_renewal_id equals x.Id
                                      where a.b_uid == buId
                                      select x).ToList().Select(x => new PreRenewalInfo
                                      {
                                          BizNO = x.BizNO,
                                          BoLi = x.BoLi,
                                          BuJiMianChengKe = x.BuJiMianChengKe,
                                          BuJiMianCheSun = x.BuJiMianCheSun,
                                          BuJiMianDaoQiang = x.BuJiMianDaoQiang,
                                          BuJiMianHuaHen = x.BuJiMianHuaHen,
                                          BuJiMianJingShenSunShi = x.BuJiMianJingShenSunShi,
                                          BuJiMianSanZhe = x.BuJiMianSanZhe,
                                          BuJiMianSheShui = x.BuJiMianSheShui,
                                          BuJiMianSiJi = x.BuJiMianSiJi,
                                          BuJiMianZiRan = x.BuJiMianZiRan,
                                          CheDeng = x.CheDeng,
                                          ChengKe = x.ChengKe,
                                          CheSun = x.CheSun,
                                          DaoQiang = x.DaoQiang,
                                          ForceNO = x.ForceNO,
                                          HuaHen = x.HuaHen,
                                          InsuredIdCard = x.InsuredIdCard,
                                          InsuredIdType = x.InsuredIdType,
                                          InsuredName = x.InsuredName,
                                          JingShenSunShi = x.JingShenSunShi,
                                          LastBizEndDate = x.LastBizEndDate.HasValue ? x.LastBizEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : x.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                                          LastForceEndDate = x.LastForceEndDate.HasValue ? x.LastForceEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : x.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                                          LastBizEndDateTime = x.LastBizEndDate,
                                          LastForceEndDateTime = x.LastForceEndDate,
                                          LastYearSource = x.LastYearSource,
                                          SanFangTeYue = x.SanFangTeYue,
                                          SanZhe = x.SanZhe,
                                          SheShui = x.SheShui,
                                          SiJi = x.SiJi,
                                          TeYue = x.TeYue,
                                          ZiRan = x.ZiRan,
                                          IsJiaoQiang = x.RenewalType.HasValue ? x.RenewalType.Value : default(int),
                                          HcXiuLiChang = x.XiuLiChang.HasValue ? x.XiuLiChang.Value : 0,
                                          HcXiuLiChangType = x.XiuLiChangType.HasValue ? x.XiuLiChangType.Value : -1,//未取到值默认为-1
                                          BjmSheBeiSunshi = x.BuJiMianSheBeiSunshi,
                                          Fybc = x.FeiYongBuChang,
                                          FybcDays = x.FeiYongBuChangDays,
                                          SheBeiSunShi = x.SheBeiSunShi,
                                          SheBei = x.SheBeiSunShiConfig,
                                          Organization = x.Organization,
                                          SanZheJieJiaRi = x.SanZheJieJiaRi.HasValue ? x.SanZheJieJiaRi.Value.ToString() : "0"
                                      }).FirstOrDefault();
                    if (carRenewal == null) return new PreRenewalInfo();
                    if (carRenewal.LastYearSource.HasValue)
                    {
                        if (carRenewal.LastYearSource.Value == 0)
                        {
                            carRenewal.LastYearSource = 2;
                        }
                        else if (carRenewal.LastYearSource.Value > 1)
                        {
                            carRenewal.LastYearSource = Convert.ToInt64(Math.Pow(2, carRenewal.LastYearSource.Value));
                        }


                    }
                    return carRenewal;
                });
            }
        }


        public PreRenewalInfo GetCarRenwalInfo(long buId)
        {
            using (var _dbContext = new EntityContext())
            {

                var carRenewal = (from a in _dbContext.bx_userinfo_renewal_index
                                  join x in _dbContext.bx_car_renewal
                                  on a.car_renewal_id equals x.Id
                                  where a.b_uid == buId
                                  select x).ToList().Select(x => new PreRenewalInfo
                                  {
                                      BizNO = x.BizNO,
                                      BoLi = x.BoLi,
                                      BuJiMianChengKe = x.BuJiMianChengKe,
                                      BuJiMianCheSun = x.BuJiMianCheSun,
                                      BuJiMianDaoQiang = x.BuJiMianDaoQiang,
                                      BuJiMianHuaHen = x.BuJiMianHuaHen,
                                      BuJiMianJingShenSunShi = x.BuJiMianJingShenSunShi,
                                      BuJiMianSanZhe = x.BuJiMianSanZhe,
                                      BuJiMianSheShui = x.BuJiMianSheShui,
                                      BuJiMianSiJi = x.BuJiMianSiJi,
                                      BuJiMianZiRan = x.BuJiMianZiRan,
                                      CheDeng = x.CheDeng,
                                      ChengKe = x.ChengKe,
                                      CheSun = x.CheSun,
                                      DaoQiang = x.DaoQiang,
                                      ForceNO = x.ForceNO,
                                      HuaHen = x.HuaHen,
                                      InsuredIdCard = x.InsuredIdCard,
                                      InsuredIdType = x.InsuredIdType,
                                      InsuredName = x.InsuredName,
                                      JingShenSunShi = x.JingShenSunShi,
                                      LastBizEndDate = x.LastBizEndDate.HasValue ? x.LastBizEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : x.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                                      LastForceEndDate = x.LastForceEndDate.HasValue ? x.LastForceEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : x.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                                      LastBizEndDateTime = x.LastBizEndDate,
                                      LastForceEndDateTime = x.LastForceEndDate,
                                      LastYearSource = x.LastYearSource,
                                      SanFangTeYue = x.SanFangTeYue,
                                      SanZhe = x.SanZhe,
                                      SheShui = x.SheShui,
                                      SiJi = x.SiJi,
                                      TeYue = x.TeYue,
                                      ZiRan = x.ZiRan,
                                      IsJiaoQiang = x.RenewalType.HasValue ? x.RenewalType.Value : default(int),
                                      HcXiuLiChang = x.XiuLiChang.HasValue ? x.XiuLiChang.Value : 0,
                                      HcXiuLiChangType = x.XiuLiChangType.HasValue ? x.XiuLiChangType.Value : -1,
                                      BjmSheBeiSunshi = x.BuJiMianSheBeiSunshi,
                                      Fybc = x.FeiYongBuChang,
                                      FybcDays = x.FeiYongBuChangDays,
                                      SheBeiSunShi = x.SheBeiSunShi,
                                      SheBei = x.SheBeiSunShiConfig,
                                      Organization = x.Organization,
                                      SanZheJieJiaRi = x.SanZheJieJiaRi.HasValue ? x.SanZheJieJiaRi.Value.ToString() : "0"
                                  }).FirstOrDefault();
                if (carRenewal == null) return new PreRenewalInfo()
                {
                    HcXiuLiChangType = -1,
                };
                if (carRenewal.LastYearSource.HasValue)
                {
                    if (carRenewal.LastYearSource.Value == 0)
                    {
                        carRenewal.LastYearSource = 2;
                    }
                    else if (carRenewal.LastYearSource.Value > 1)
                    {
                        carRenewal.LastYearSource = Convert.ToInt64(Math.Pow(2, carRenewal.LastYearSource.Value));
                    }


                }
                return carRenewal;
            }
        }
        /// <summary>
        /// 根据新车获取上年投保信息包括保费
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        public PreRenewalInfo GetNewCarRenwalInfo(long buId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" SELECT a.BizNO,a.BoLi,a.BuJiMianChengKe,a.BuJiMianCheSun,a.BuJiMianDaoQiang,a.BuJiMianHuaHen,a.BuJiMianJingShenSunShi,a.BuJiMianSanZhe,a.BuJiMianSheShui,a.BuJiMianSiJi,a.BuJiMianZiRan,a.CheDeng,a.ChengKe,a.CheSun,a.DaoQiang,a.ForceNO,a.HuaHen,a.InsuredIdCard,a.InsuredIdType,a.InsuredName,a.JingShenSunShi,IF(DATE_FORMAT(a.LastBizEndDate,'%Y-%m-%d')='0001-01-01','',DATE_FORMAT(a.LastBizEndDate,'%Y-%m-%d %h:%i:%s')) as LastBizEndDate,IF(DATE_FORMAT(a.LastForceEndDate,'%Y-%m-%d')='0001-01-01','',DATE_FORMAT(a.LastForceEndDate,'%Y-%m-%d %h:%i:%s')) as LastForceEndDate,a.LastBizEndDate as LastBizEndDateTime,a.LastForceEndDate as LastForceEndDateTime,a.LastYearSource,a.SanFangTeYue,a.SanZhe,a.SheShui,a.SiJi,a.TeYue,a.ZiRan,a.RenewalType as IsJiaoQiang,a.XiuLiChang as HcXiuLiChang,a.XiuLiChangType as HcXiuLiChangType,a.BuJiMianSheBeiSunshi as BjmSheBeiSunshi,a.FeiYongBuChang as Fybc,a.FeiYongBuChangDays as FybcDays,a.SheBeiSunShi ,a.SheBeiSunShiConfig as SheBei,a.Organization ");
            sqlBuilder.Append(",c.`CheSun` AS CheSunBaoFei,c.`SanZhe` AS SanZheBaoFei, c.`DaoQiang` AS DaoQiangBaoFei, c.`SiJi` AS SiJiBaoFei, c.`ChengKe` AS ChengKeBaoFei, c.`BoLi` AS BoLiBaoFei, c.`HuaHen` AS HuaHenBaoFei, c.`BuJiMianCheSun` AS BuJiMianCheSunBaoFei, c.`BuJiMianSanZhe` AS BuJiMianSanZheBaoFei, c.`BuJiMianDaoQiang` AS BuJiMianDaoQiangBaoFei, c.`BuJiMianRenYuan` AS BuJiMianRenYuanBaoFei, c.`BuJiMianSiJi` AS BuJiMianSiJiBaoFei, c.`BuJiMianChengKe` AS BuJiMianChengKeBaoFei, c.`BuJiMianFuJia` AS BuJiMianFuJiaBaoFei, c.`BuJiMianZiRan` AS BuJiMianZiRanBaoFei, c.`BuJiMianSheShui` AS BuJiMianSheShuiBaoFei, c.`BuJiMianHuaHen` AS BuJiMianHuaHenBaoFei, c.`BuJiMianSheBeiSunshi` AS BuJiMianSheBeiSunshiBaoFei,c.`TeYue` AS TeYueBaoFei, c.`SheShui` AS SheShuiBaoFei, c.`CheDeng` AS CheDengBaoFei,c.`ZiRan` AS ZiRanBaoFei, c.`FeiYongBuChang` AS FeiYongBuChangBaoFei, c.`XiuLiChang` AS XiuLiChangBaoFei, c.`SheBeiSunShi` AS SheBeiSunShiBaoFei, c.`HuoWuZeRen` AS HuoWuZeRenBaoFei,c.`SanFangTeYue` AS SanFangTeYueBaoFei, c.`JingShenSunShi` AS JingShenSunShiBaoFei, c.`BuJiMianJingShenSunShi` AS BuJiMianJingShenSunShiBaoFei, c.`SanZheJieJiaRi` AS SanZheJieJiaRiBaoFei, c.`BizPriceTotal`, c.`ForcePriceTotal`, c.`TaxPriceTotal` ");
            sqlBuilder.Append("FROM bx_userinfo_renewal_index b ");
            sqlBuilder.Append("INNER JOIN bx_car_renewal a ON b.car_renewal_id=a.Id ");
            sqlBuilder.Append("LEFT JOIN bx_car_renewal_premium c ON c.car_renewal_id=a.Id ");
            sqlBuilder.Append("WHERE b.b_uid=" + buId);

            PreRenewalInfo model = _context.Database.SqlQuery<PreRenewalInfo>(sqlBuilder.ToString()).FirstOrDefault();
            if (model == null) { return model = new PreRenewalInfo(); }
            model.HcXiuLiChang = model.HcXiuLiChang.HasValue ? model.HcXiuLiChang : 0;
            model.HcXiuLiChangType = model.HcXiuLiChangType.HasValue ? model.HcXiuLiChangType : 0;
            if (model.LastYearSource.HasValue)
            {
                if (model.LastYearSource.Value == 0)
                {
                    model.LastYearSource = 2;
                }
                else if (model.LastYearSource.Value > 1)
                {
                    model.LastYearSource = Convert.ToInt64(Math.Pow(2, model.LastYearSource.Value));
                }
            }

            return model;
        }
        public PreRenewalInfo GetNewCarRenwalInfo(List<PartRenewalInfo> tempList, long buId)
        {

            int bao_dan_type = tempList[0].bao_dan_type;
            List<long> BaoDanXinXiIdList = new List<long>();
            BaoDanXinXiIdList = tempList.Select(a => a.BaoDanXinXiId).ToList();
            List<dz_baodanxianzhong> xianzhongList = _context.dz_baodanxianzhong.Where(a => BaoDanXinXiIdList.Contains(a.BaoDanXinXiId.Value)).ToList();
            xianzhongList = xianzhongList == null ? new List<dz_baodanxianzhong>() : xianzhongList;

            //商业险
            var bizPartRenewalInfo = tempList.Where(a => a.insurance_type == 2).FirstOrDefault();
            bizPartRenewalInfo = bizPartRenewalInfo == null ? new PartRenewalInfo() : bizPartRenewalInfo;

            var bizxianzhongModel = xianzhongList.Where(a => a.BaoDanXinXiId == bizPartRenewalInfo.BaoDanXinXiId).FirstOrDefault();
            bizxianzhongModel = bizxianzhongModel == null ? new dz_baodanxianzhong() : bizxianzhongModel;

            dz_baodanxianzhong model = bizxianzhongModel;
            PartRenewalInfo temp = bizPartRenewalInfo;

            //交强险
            var forcePartRenewalInfo = tempList.Where(a => a.insurance_type == 1).FirstOrDefault();
            forcePartRenewalInfo = forcePartRenewalInfo == null ? new PartRenewalInfo() : forcePartRenewalInfo;

            var forcezhongModel = xianzhongList.Where(a => a.BaoDanXinXiId == forcePartRenewalInfo.BaoDanXinXiId).FirstOrDefault();
            forcezhongModel = forcezhongModel == null ? new dz_baodanxianzhong() : forcezhongModel;

            //dz_baodanxianzhong model = _context.dz_baodanxianzhong.Where(a => a.BaoDanXinXiId == temp.BaoDanXinXiId).FirstOrDefault();
            PreRenewalInfo carRenewal = new PreRenewalInfo()
            {
                BizNO = string.IsNullOrEmpty(temp.BizNO) ? temp.BizNum : temp.BizNO,
                BoLi = model.BoLiBaoE,
                BizPriceTotal = model.BizTotal,
                ForcePriceTotal = forcezhongModel.ForceTotal,
                TaxPriceTotal = forcezhongModel.TaxTotal,
                TotalBaoFei = (model.BizTotal.HasValue ? model.BizTotal.Value : 0) + (forcezhongModel.ForceTotal.HasValue ? forcezhongModel.ForceTotal.Value : 0) + (forcezhongModel.TaxTotal.HasValue ? forcezhongModel.TaxTotal.Value : 0),
                BoLiBaoFei = model.BoLiBaoFei,
                BuJiMianChengKe = model.BuJiMianChengKe,
                BuJiMianChengKeBaoFei = model.BuJiMianChengKe,
                BuJiMianCheSun = model.BuJiMianCheSun,
                BuJiMianCheSunBaoFei = model.BuJiMianCheSun,
                BuJiMianDaoQiang = model.BuJiMianDaoQiang,
                BuJiMianDaoQiangBaoFei = model.BuJiMianDaoQiang,
                BuJiMianHuaHen = model.BuJiMianHuaHen,
                BuJiMianHuaHenBaoFei = model.BuJiMianHuaHen,
                BuJiMianJingShenSunShi = model.BuJiMianJingShenSunShi,
                BuJiMianJingShenSunShiBaoFei = model.BuJiMianJingShenSunShi,
                BuJiMianSanZhe = model.BuJiMianSanZhe,
                BuJiMianSanZheBaoFei = model.BuJiMianSanZhe,
                BuJiMianSheShui = model.BuJiMianSheShui,
                BuJiMianSheShuiBaoFei = model.BuJiMianSheShui,
                BuJiMianSiJi = model.BuJiMianSiJi,
                BuJiMianSiJiBaoFei = model.BuJiMianSiJi,
                BuJiMianZiRan = model.BuJiMianZiRan,
                BuJiMianZiRanBaoFei = model.BuJiMianZiRan,
                CheDeng = model.CheDengBaoE,
                CheDengBaoFei = model.CheDengBaoFei,
                ChengKe = model.ChengKeBaoE,
                ChengKeBaoFei = model.ChengKeBaoFei,
                CheSun = model.CheSunBaoE,
                CheSunBaoFei = model.CheSunBaoFei,
                DaoQiang = model.DaoQiangBaoE,
                DaoQiangBaoFei = model.DaoQiangBaoFei,
                ForceNO = string.IsNullOrEmpty(temp.ForceNO) ? temp.ForceNum : temp.ForceNO,
                HuaHen = model.HuaHenBaoE,
                HuaHenBaoFei = model.HuaHenBaoFei,
                InsuredIdCard = temp.InsuredIdCard,
                //InsuredIdType = temp.InsuredIdType,
                InsuredName = temp.InsuredName,
                JingShenSunShi = model.JingShenSunShiBaoE,
                JingShenSunShiBaoFei = model.JingShenSunShiBaoFei,
                LastBizEndDate = temp.LastBizEndDate.HasValue ? temp.LastBizEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : temp.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                LastForceEndDate = temp.LastForceEndDate.HasValue ? temp.LastForceEndDate.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? "" : temp.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm") : "",
                LastBizEndDateTime = temp.LastBizEndDate,
                LastForceEndDateTime = temp.LastForceEndDate,
                LastYearSource = temp.LastYearSource,
                SanFangTeYue = model.SanFangTeYueBaoE.HasValue && model.SanFangTeYueBaoE.Value > 0 ? 1 : 0,
                SanFangTeYueBaoFei = model.SanFangTeYueBaoFei,
                SanZhe = model.SanZheBaoE,
                SanZheBaoFei = model.SanZheBaoFei,
                SheShui = model.SheShuiBaoE,
                SheShuiBaoFei = model.SheShuiBaoFei,
                SiJi = model.SiJiBaoE,
                SiJiBaoFei = model.SiJiBaoFei,
                TeYue = model.TeYueBaoE,
                TeYueBaoFei = model.TeYueBaoFei,
                ZiRan = model.ZiRanBaoE,
                ZiRanBaoFei = model.ZiRanBaoFei,
                IsJiaoQiang = temp.bao_dan_type,// string.IsNullOrEmpty(temp.ForceNO) ? 2 : (string.IsNullOrEmpty(temp.BizNO) ? 1 : 0),
                HcXiuLiChang = model.XiuLiChangBaoE.HasValue ? model.XiuLiChangBaoE.Value : 0,
                HcXiuLiChangBaoFei = model.XiuLiChangBaoFei.HasValue ? model.XiuLiChangBaoFei.Value : 0,
                HcXiuLiChangType = model.XiuLiChangType.HasValue ? model.XiuLiChangType.Value : -1,
                BjmSheBeiSunshi = model.BuJiMianSheBei,
                Fybc = model.FeiYongBuChangBaoE,
                FeiYongBuChangBaoFei = model.FeiYongBuChangBaoFei,
                //FybcDays
                SheBeiSunShi = !model.SheBeiSunShiBaoE.HasValue ? (model.SheBeiSunShiBaoFei) : model.SheBeiSunShiBaoE,
                SheBeiSunShiBaoFei = model.SheBeiSunShiBaoFei,
                SheBei = model.SheBeiSunShiConfig,
                Organization = temp.Organization,
                SanZheJieJiaRi = model.SanZheJieJiaRiBaoE.ToString(),
                SanZheJieJiaRiBaoFei = model.SanZheJieJiaRiBaoFei

            };

            if (carRenewal == null) return new PreRenewalInfo()
            {
                HcXiuLiChangType = -1,
            };
            if (temp.InsuredIdType == "身份证" || temp.InsuredIdType == "居民身份证")
            {
                carRenewal.InsuredIdType = 1;
            }
            else
            {
                carRenewal.InsuredIdType = GetType(temp.InsuredIdType);
            }

            if (carRenewal.LastYearSource.HasValue)
            {
                if (carRenewal.LastYearSource.Value == 0)
                {
                    carRenewal.LastYearSource = 2;
                }
                else if (carRenewal.LastYearSource.Value > 1)
                {
                    carRenewal.LastYearSource = Convert.ToInt64(Math.Pow(2, carRenewal.LastYearSource.Value));
                }

            }
            return carRenewal;
        }
        /// <summary>
        /// 获得证件类型
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        private int GetType(string strType)
        {
            int result = 0;
            if (string.IsNullOrWhiteSpace(strType))
            {
                return result;
            }
            switch (strType)
            {
                case "身份证":
                    result = 1;
                    break;
                case "居民身份证":
                    result = 1;
                    break;
                case "组织机构代码证":
                    result = 2;
                    break;
                case "护照":
                    result = 3;
                    break;
                case "军官证":
                    result = 4;
                    break;
                case "港澳居民来往内地通行证":
                    result = 5;
                    break;
                case "其他":
                    result = 6;
                    break;
                case "港澳通行证":
                    result = 7;
                    break;
                case "出生证":
                    result = 8;
                    break;
                case "营业执照":
                    result = 9;
                    break;
                case "税务登记证":
                    result = 10;
                    break;
                case "港澳身份证":
                    result = 14;
                    break;
                default:
                    result = 6;
                    break;
            }
            return result;
        }
        /// <summary>
        /// 判断是否是从已批改车进来的数据
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bool IsNewCar(long buid)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT t3.IsNewCar FROM dz_correct_license_plate AS t1 ");
            sqlBuilder.Append("INNER JOIN dz_reconciliation AS t2 ON t1.guid=t2.guid ");
            sqlBuilder.Append("LEFT JOIN dz_baodanxinxi AS t3 ON t3.id=t2.baodanxinxi_id WHERE t3.IsNewCar=1 and t2.insurance_type=2 ");
            sqlBuilder.Append(" AND t1.bu_id=" + buid + ";");
            int result = _context.Database.SqlQuery<int>(sqlBuilder.ToString()).FirstOrDefault();
            return result == 1 ? true : false;
        }
        /// <summary>
        /// 批改车获取部分投保信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public List<PartRenewalInfo> GetPartRenewalInfo(long buid)
        {
            List<PartRenewalInfo> list = new List<PartRenewalInfo>();
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT t2.insurance_type, t3.BizNum,t3.ForceNum, t1.guid,t2.bao_dan_type,t3.IsNewCar,t3.Id as BaoDanXinXiId,t1.biz_pno as BizNO ,t1.force_pno as ForceNO, t3.InsureIdType as InsuredIdType,t3.InsureIdNum as InsuredIdCard,t3.InsuredName as InsuredName");
                sqlBuilder.Append(" ,t1.biz_end_date as LastBizEndDate,t1.force_end_date as LastForceEndDate,t1.source as LastYearSource,t3.Organization");
                sqlBuilder.Append(" FROM dz_correct_license_plate AS t1 ");
                sqlBuilder.Append("INNER JOIN dz_reconciliation AS t2 ON t1.guid=t2.guid ");
                sqlBuilder.Append("LEFT JOIN dz_baodanxinxi AS t3 ON t3.id=t2.baodanxinxi_id WHERE 1=1 ");
                sqlBuilder.Append(" AND t1.bu_id=" + buid + ";");
                list = _context.Database.SqlQuery<PartRenewalInfo>(sqlBuilder.ToString()).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常：" + ex);
                throw;
            }
            return list;
        }

        public bx_carinfo GetCarInfoAsync(long buId)
        {
            using (var _dbContext = new EntityContext())
            {
                var aa = (from a in _dbContext.bx_carinfo
                          join x in _dbContext.bx_userinfo
                          on a.license_no equals x.LicenseNo
                          where x.Id == buId
                          select a).FirstOrDefault();
                //string sql = string.Format("SELECT license_owner FROM bx_carinfo WHERE license_no=(SELECT LicenseNo FROM bx_userinfo WHERE Id={0})", buId);
                return aa;
            }
        }

        public async Task<bool> SaveCustomerInfoAsync(CustomerInfo customerInfo)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    _dbContext.bx_userinfo.FirstOrDefault(x => x.Id == customerInfo.BuId).CategoryInfoId = customerInfo.CustomerType;
                    _dbContext.bx_userinfo.FirstOrDefault(x => x.Id == customerInfo.BuId).UpdateTime = DateTime.Now;
                    var userInfoRenewalInfo = _dbContext.bx_userinfo_renewal_info.FirstOrDefault(x => x.b_uid == customerInfo.BuId);
                    if (userInfoRenewalInfo == null)
                    {
                        _dbContext.bx_userinfo_renewal_info.Add(new bx_userinfo_renewal_info()
                        {
                            b_uid = customerInfo.BuId,
                            client_mobile = customerInfo.CustomerMobile,
                            client_name = customerInfo.CustomerName,
                            client_mobile_other = customerInfo.ClientMobileOther,
                            remark = customerInfo.Remark,
                            create_time = DateTime.Now,
                            intention_remark = customerInfo.IntentionRemark,
                            client_address = customerInfo.ClientAddress,
                            TagId = customerInfo.TagId,
                            CustomerType = customerInfo.CustomerType
                        });
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(userInfoRenewalInfo.client_mobile) && string.IsNullOrWhiteSpace(userInfoRenewalInfo.client_mobile_other))
                        {
                            userInfoRenewalInfo.client_mobile = customerInfo.CustomerMobile;
                        }
                        else if (!string.IsNullOrWhiteSpace(userInfoRenewalInfo.client_mobile))
                        {
                            if (userInfoRenewalInfo.client_mobile != customerInfo.CustomerMobile)
                            {
                                userInfoRenewalInfo.client_mobile_other = customerInfo.CustomerMobile;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(customerInfo.CustomerMobile) && !string.IsNullOrWhiteSpace(customerInfo.ClientMobileOther))
                        {
                            userInfoRenewalInfo.client_mobile = customerInfo.ClientMobileOther;
                            userInfoRenewalInfo.client_mobile_other = customerInfo.CustomerMobile;
                        }
                        if (string.IsNullOrWhiteSpace(customerInfo.CustomerMobile) && string.IsNullOrWhiteSpace(customerInfo.ClientMobileOther))
                        {
                            userInfoRenewalInfo.client_mobile = customerInfo.CustomerMobile;
                            userInfoRenewalInfo.client_mobile_other = customerInfo.ClientMobileOther;
                        }
                        if (!string.IsNullOrWhiteSpace(customerInfo.CustomerMobile) && !string.IsNullOrWhiteSpace(customerInfo.ClientMobileOther))
                        {
                            userInfoRenewalInfo.client_mobile = customerInfo.CustomerMobile;
                            userInfoRenewalInfo.client_mobile_other = customerInfo.ClientMobileOther;
                        }
                        userInfoRenewalInfo.remark = customerInfo.Remark;
                        userInfoRenewalInfo.client_name = customerInfo.CustomerName;
                        userInfoRenewalInfo.intention_remark = customerInfo.IntentionRemark;
                        userInfoRenewalInfo.client_address = customerInfo.ClientAddress;
                        userInfoRenewalInfo.TagId = customerInfo.TagId;
                        userInfoRenewalInfo.CustomerType = customerInfo.CustomerType;
                    }
                    return _dbContext.SaveChanges() >= 0;
                });

            }
        }

        public async Task<bool> SaveCarInfoAsync(bx_userinfo userInfo, bx_quotereq_carinfo quotereqCarInfo, bool isChangeAutoModelCode, long batchItemId)
        {


            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    //bool boolresult = false;
                    //try
                    //{
                        #region 指定更新bx_userinfo
                        _dbContext.Set<bx_userinfo>().Attach(userInfo);
                        _dbContext.Entry<bx_userinfo>(userInfo).Property("CarVIN").IsModified = true;
                        _dbContext.Entry<bx_userinfo>(userInfo).Property("EngineNo").IsModified = true;
                        _dbContext.Entry<bx_userinfo>(userInfo).Property("LicenseNo").IsModified = true;


                        _dbContext.Entry<bx_userinfo>(userInfo).Property("MoldName").IsModified = true;
                        _dbContext.Entry<bx_userinfo>(userInfo).Property("RegisterDate").IsModified = true;

                        _dbContext.Entry<bx_userinfo>(userInfo).Property("RenewalCarType").IsModified = true;

                        _dbContext.Entry<bx_userinfo>(userInfo).Property("UpdateTime").IsModified = true;
                        #endregion
                        #region 指定更新bx_quotereq_carinfo
                        if (quotereqCarInfo.id != -1)
                        {

                            _dbContext.Set<bx_quotereq_carinfo>().Attach(quotereqCarInfo);
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("is_loans").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("car_used_type").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("is_public").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("seat_count").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("exhaust_scale").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("transfer_date").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("pa_remark").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("auto_model_code").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("PriceT").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("drivlicense_cartype_value").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("car_ton_count").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("is_truck").IsModified = true;
                            _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("moldcode_company").IsModified = true;
                            if (isChangeAutoModelCode)
                            {
                                //modify by gpj 去掉auto_model_code置为空的情况。同时，续保详情如果精友码为空，则返回空。
                                //quotereqCarInfo.auto_model_code = null;
                                quotereqCarInfo.auto_model_code_source = 1;
                                //_dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("auto_model_code").IsModified = true;
                                _dbContext.Entry<bx_quotereq_carinfo>(quotereqCarInfo).Property("auto_model_code_source").IsModified = true;
                            }
                        }
                        else
                        {
                            quotereqCarInfo.b_uid = userInfo.Id;
                            quotereqCarInfo.create_time = DateTime.Now;
                            quotereqCarInfo.update_time = DateTime.Now;
                            _dbContext.bx_quotereq_carinfo.Add(quotereqCarInfo);
                        }
                        #endregion

                        if (batchItemId >= 1)
                        {
                            var batchrenewal_item = _dbContext.bx_batchrenewal_item.SingleOrDefault(x => x.Id == batchItemId);
                            batchrenewal_item.MoldName = userInfo.MoldName;
                            batchrenewal_item.RegisterDate = userInfo.RegisterDate;
                            //bx_batchrenewal_item batchrenewal_item = new bx_batchrenewal_item { Id = batchItemId, MoldName = userInfo.MoldName };
                            //_dbContext.Entry<bx_batchrenewal_item>(batchrenewal_item).Property("MoldName").IsModified = true;
                        }
                        return _dbContext.SaveChanges() >= 0;
                    //}
                    //catch (DbEntityValidationException dbEx)
                    //{
                    //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //    {
                    //        foreach (var validationError in validationErrors.ValidationErrors)
                    //        {
                    //            logError.Info(string.Format("Property: {0} Error: {1}",
                    //                validationError.PropertyName,
                    //                validationError.ErrorMessage));
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    logError.Info("发生异常:SaveCarInfoAsync模型失败\n" + userInfo.Id + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" +
                    //                  ex.InnerException);
                    //}
                    //return boolresult;
                });

            }

        }


        /// <summary>
        /// 更新UserInfo主表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool SaveUserInfo(bx_userinfo userInfo)
        {

            using (var _dbContext = new EntityContext())
            {

                #region 指定更新bx_userinfo
                _dbContext.Set<bx_userinfo>().Attach(userInfo);
                _dbContext.Entry<bx_userinfo>(userInfo).Property("LicenseOwner").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("OwnerIdCardType").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("IdCard").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredName").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredIdType").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredIdCard").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredMobile").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredAddress").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredEmail").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderName").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderIdType").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderIdCard").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderMobile").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderEmail").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("OwnerSex").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("OwnerBirthday").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredSex").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredBirthday").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderSex").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderBirthday").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("IsChangeRelation").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderNation").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderIssuer").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderCertiStartdate").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderCertiEnddate").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredNation").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredIssuer").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredCertiStartdate").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("InsuredCertiEnddate").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("HolderAddress").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("UpdateTime").IsModified = true;
                #endregion
                return _dbContext.SaveChanges() >= 0;
            }

        }


        public BaseViewModel SavePreRenewalInfoAsync(bx_car_renewal carRenewal)
        {
            BaseViewModel baseViewModel = new BaseViewModel();
            using (var _dbContext = new EntityContext())
            {
                _dbContext.Set<bx_car_renewal>().Attach(carRenewal);
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("LastYearSource").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("LastBizEndDate").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("LastForceEndDate").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("LastBizEndDate").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("InsuredName").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("InsuredIdType").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("InsuredIdCard").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("SingleChannel").IsModified = true;
                _dbContext.Entry<bx_car_renewal>(carRenewal).Property("SingleTime").IsModified = true;
                if (_dbContext.SaveChanges() >= 0)
                {
                    baseViewModel.BusinessStatus = 1;
                    baseViewModel.StatusMessage = "保存成功";
                }
                else
                {
                    baseViewModel.BusinessStatus = 0;
                    baseViewModel.StatusMessage = "保存失败";
                }
                return baseViewModel;
            }
        }


        public Tuple<bool, bool> SaveConsumerReviewAsync(bx_consumer_review consumerReview)
        {
            using (var _dbContext = new EntityContext())
            {
                bool isDelete = false;
                var consumerReviewModel = _dbContext.bx_consumer_review.Where(x => x.b_uid == consumerReview.b_uid && !x.IsDeleted).OrderByDescending(x => x.id).FirstOrDefault();

                if (consumerReviewModel != null)
                {
                    if (consumerReviewModel.status != 16 && consumerReviewModel.status != 4 && consumerReviewModel.status != 9)
                    {
                        consumerReviewModel.IsDeleted = true;

                    }
                    else
                    {
                        DateTime? preReviewTime = consumerReviewModel.status == 9 ? consumerReviewModel.singletime : consumerReviewModel.create_time;
                        DateTime? nextReviewTime = consumerReview.status == 9 ? consumerReview.singletime : consumerReview.create_time;
                        if (preReviewTime.HasValue)
                        {
                            if (nextReviewTime.HasValue)
                            {
                                if (preReviewTime.Value.Year == nextReviewTime.Value.Year)
                                {
                                    consumerReviewModel.IsDeleted = true;
                                }
                            }
                            else
                            {
                                consumerReviewModel.IsDeleted = true;
                            }
                        }
                        else
                        {
                            consumerReviewModel.IsDeleted = true;
                        }
                        isDelete = consumerReviewModel.IsDeleted;
                    }
                }

                _dbContext.bx_consumer_review.Add(consumerReview);

                var isSuccess = _dbContext.SaveChanges() > 0;
                return new Tuple<bool, bool>(isSuccess, isDelete);
            }
        }


        public List<string> GetAgentIds(int currentAgentId)
        {
            using (var _dbContext = new EntityContext())
            {
                string sql = string.Format(@"select `queryChildrenAgentID`({0})", currentAgentId);
                var idStr = _dbContext.Database.SqlQuery<string>(sql).First();
                return idStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

        }


        public bool UpdateUserInfoAsync(bx_userinfo userInfo)
        {
            using (var _dbContext = new EntityContext())
            {

                _dbContext.Set<bx_userinfo>().Attach(userInfo);
                _dbContext.Entry<bx_userinfo>(userInfo).Property("IsReView").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("CategoryInfoId").IsModified = true;
                _dbContext.Entry<bx_userinfo>(userInfo).Property("UpdateTime").IsModified = true;
                return _dbContext.SaveChanges() >= 0;

            }
        }


        public async Task<bx_carinfo> GetCarInfoAsync(string licenseNo)
        {
            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                    {

                        return _dbContext.bx_carinfo.OrderByDescending(x => x.create_time ?? x.update_time).FirstOrDefault(x => x.license_no == licenseNo);
                    });
            }
        }
        public async Task<string> GetVehicleName(string vehicleNo)
        {
            using (var _dbContext = new EntityContext())
            {
                var vehicleName = string.Empty;
                return await Task.Run(() =>
                {
                    var carModel = _dbContext.bx_carmodel.FirstOrDefault(x => x.VehicleNo == vehicleNo);
                    if (carModel != null)
                    {
                        if (!string.IsNullOrWhiteSpace(carModel.VehicleName))
                        {
                            vehicleName += carModel.VehicleName + "/";
                        }
                        if (!string.IsNullOrWhiteSpace(carModel.VehicleAlias))
                        {
                            vehicleName += carModel.VehicleAlias + "/";
                        }
                        if (carModel.VehicleExhaust.HasValue)
                        {
                            vehicleName += carModel.VehicleExhaust + "/";
                        }
                        if (carModel.VehicleSeat.HasValue)
                        {
                            vehicleName += carModel.VehicleSeat + "/";
                        }
                        if (carModel.PriceT.HasValue)
                        {
                            vehicleName += carModel.PriceT + "/";
                        }
                        if (!string.IsNullOrWhiteSpace(carModel.VehicleYear))
                        {
                            vehicleName += carModel.VehicleYear.Substring(0, 4);
                        }

                    }
                    return vehicleName;
                });
            }
        }

        public string GetVehicleNameNew(string vehicleNo)
        {
            using (var _dbContext = new EntityContext())
            {
                var vehicleName = string.Empty;

                var carModel = _dbContext.bx_carmodel.FirstOrDefault(x => x.VehicleNo == vehicleNo);
                if (carModel != null)
                {
                    if (!string.IsNullOrWhiteSpace(carModel.VehicleName))
                    {
                        vehicleName += carModel.VehicleName + "/";
                    }
                    if (!string.IsNullOrWhiteSpace(carModel.VehicleAlias))
                    {
                        vehicleName += carModel.VehicleAlias + "/";
                    }
                    if (carModel.VehicleExhaust.HasValue)
                    {
                        vehicleName += carModel.VehicleExhaust + "/";
                    }
                    if (carModel.VehicleSeat.HasValue)
                    {
                        vehicleName += carModel.VehicleSeat + "/";
                    }
                    if (carModel.PriceT.HasValue)
                    {
                        vehicleName += carModel.PriceT + "/";
                    }
                    if (!string.IsNullOrWhiteSpace(carModel.VehicleYear))
                    {
                        vehicleName += carModel.VehicleYear.Substring(0, 4);
                    }

                }
                return vehicleName;
            }
        }

        public bool UpdateReqCarInfo(long buId)
        {
            var isSuccess = false;
            using (var _dbContext = new EntityContext())
            {
                var reqCarinfoModel = _dbContext.bx_quotereq_carinfo.SingleOrDefault(x => x.b_uid == buId);
                if (reqCarinfoModel != null)
                {

                    reqCarinfoModel.auto_model_code = null;
                    reqCarinfoModel.auto_model_code_source = -1;
                    isSuccess = _dbContext.SaveChanges() >= 0;
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime1">大日期</param>
        /// <param name="dateTime2">小日期</param>
        /// <returns></returns>
        public int DateDiff(DateTime? dateTime1, DateTime dateTime2)
        {
            if (!dateTime1.HasValue) return 0;
            TimeSpan ts = dateTime1.Value - dateTime2;
            return ts.Days;
        }
        ///曹晨旭
        /// <summary>
        /// 录入回访信息>查找是否存在OrderStatus=-2的buid
        /// </summary>
        /// <param name="buid">buid</param>
        /// <returns></returns>
        public int SearchBuidUnique(long buid)
        {

            var SearchBuidUnique = _context.bx_car_order.FirstOrDefault(x => x.buid == buid && -2 == x.order_status);
            if (SearchBuidUnique == null)
            {
                return 0;
            }
            return 1;
        }

        public int GetConsumerReviewStatus(long buId)
        {
            var consumerReviewModel = _context.bx_consumer_review.Where(x => x.b_uid == buId && !x.IsDeleted).OrderByDescending(x => x.id).FirstOrDefault();
            if (consumerReviewModel == null)
            {
                return -1;
            }
            else
            {
                return consumerReviewModel.status.HasValue ? consumerReviewModel.status.Value : -1;
            }
        }
        public bool DeleteCarOrder(long buId)
        {
            var carOrderModel = _context.bx_car_order.Where(x => x.buid == buId).OrderByDescending(x => x.id).FirstOrDefault();
            if (carOrderModel != null)
            {
                carOrderModel.order_status = -1;
            }
            return _context.SaveChanges() >= 0;
        }
        public void UpdateConsumerReviewBizEndDate()
        {
            var dateTime = new DateTime(2017, 7, 1);

            var consumer_reviewModel = _context.bx_consumer_review.Where(x => x.create_time >= dateTime).ToList();
            var idList = consumer_reviewModel.GroupBy(x => x.b_uid).Select(x => x.Max(a => a.id)).ToList();
            var useconsumer_reviewModel = consumer_reviewModel.Where(x => idList.Contains(x.id)).Select(x => new { BuId = Convert.ToInt64(x.b_uid), UseTime = x.status != 9 && x.singletime != null ? x.singletime : (x.status == 4 || x.status == 16) && x.next_review_date != null ? x.next_review_date : x.create_time }).ToList();
            var buIdList = useconsumer_reviewModel.Select(x => x.BuId).ToList();
            var rBuids = _context.Database.SqlQuery<long>(string.Format("select id from bx_userinfo where id in({0})", string.Join(",", buIdList))).ToList();
            var successUserModel = _context.Database.SqlQuery<bx_userinfo>(string.Format("SELECT * FROM bx_userinfo WHERE  id IN({0})   AND LastYearSource>-1", string.Join(",", rBuids))).ToList();

            var successUserModelHasBatchrenewal = successUserModel.Where(x => x.RenewalType == 5).ToList();


            var successUserModelHasBatchrenewalBuIds = successUserModelHasBatchrenewal.Select(x => x.Id).ToList();

            var aa = _context.Database.SqlQuery<CC>(string.Format("select BuId,BizEndDate from bx_batchrenewal_item where BUId in({0}) and IsNew=1", string.Join(",", successUserModelHasBatchrenewalBuIds))).ToList();

            var bb = _context.Database.SqlQuery<CC>(string.Format("select a.b_uid as BuId, b.LastBizEndDate as BizEndDate from bx_userinfo_renewal_index as a join bx_car_renewal as b on a.car_renewal_id=b.Id where a.b_uid in({0})", string.Join(",", successUserModelHasBatchrenewalBuIds))).ToList();

            var aBuid = aa.Select(x => x.BuId).ToList();
            var bBuid = bb.Select(x => x.BuId).ToList();
            var intersectIds = bBuid.Intersect(aBuid).ToList();
            var exceptIds = aBuid.Except(bBuid).Union(bBuid.Except(aBuid)).ToList();
            List<BB> cc = new List<BB>();
            if (intersectIds.Any())
            {
                cc = (from b in bb.Where(x => intersectIds.Contains(x.BuId))
                      join a in aa.Where(x => intersectIds.Contains(x.BuId))
                      on b.BuId equals a.BuId
                      select new BB { BuId = b.BuId, ABizEndDate = a.BizEndDate, BBizEndDate = b.BizEndDate }).ToList();
            }
            foreach (var item in aa.Where(x => exceptIds.Contains(x.BuId)))
            {
                cc.Add(new BB { BuId = item.BuId, ABizEndDate = item.BizEndDate, BBizEndDate = null });
            }
            foreach (var item in bb.Where(x => exceptIds.Contains(x.BuId)))
            {
                cc.Add(new BB { BuId = item.BuId, ABizEndDate = null, BBizEndDate = item.BizEndDate });
            }

            var dd = cc.Where(x => x.BBizEndDate != null || (x.ABizEndDate != null && x.ABizEndDate.Value.ToString("yyyy") != "1900")).ToList();
            var r1 = dd.Where(x => x.ABizEndDate == null || x.ABizEndDate.Value.ToString("yyyy") == "1900").Select(x => new CC { BuId = x.BuId, BizEndDate = x.BBizEndDate }).ToList();
            var r2 = dd.Where(x => x.BBizEndDate == null).Select(x => new CC { BuId = x.BuId, BizEndDate = x.ABizEndDate }).ToList();
            var r3 = dd.Where(x => x.ABizEndDate != null && x.BBizEndDate != null && x.ABizEndDate.Value.ToString("yyyy") != "1900" && x.ABizEndDate.Value.Year >= x.BBizEndDate.Value.Year).Select(x => new CC { BuId = x.BuId, BizEndDate = x.ABizEndDate }).ToList();
            var r4 = dd.Where(x => x.ABizEndDate != null && x.BBizEndDate != null && x.ABizEndDate.Value.ToString("yyyy") != "1900" && x.ABizEndDate.Value.Year < x.BBizEndDate.Value.Year).Select(x => new CC { BuId = x.BuId, BizEndDate = x.BBizEndDate }).ToList();
            var rt1 = r1.Union(r2).Union(r3).Union(r4).ToList();
            var successUserModelNotHasBatchrenewal = successUserModel.Where(x => x.RenewalType != 5).ToList();
            var successUserModelNotHasBatchrenewalIds = successUserModelNotHasBatchrenewal.Select(x => x.Id).ToList();
            var r5 = _context.Database.SqlQuery<CC>(string.Format(@"select  a.b_uid as BuId,b.LastBizEndDate as  BizEndDate from bx_userinfo_renewal_index as a join  bx_car_renewal as b
                on a.car_renewal_id= b.Id where a.b_uid in({0}) and b.LastBizEndDate IS NOT NULL ", string.Join(",", successUserModelNotHasBatchrenewalIds))).ToList();

            var rt2 = rt1.ToList().Union(r5).ToList();

            var faileUserModel = _context.Database.SqlQuery<bx_userinfo>(string.Format("SELECT * FROM bx_userinfo WHERE  id IN({0})   AND (LastYearSource IS  NULL OR LastYearSource<=-1)", string.Join(",", rBuids))).ToList();
            var xx = rBuids.Except(successUserModel.Select(x => x.Id).Union(faileUserModel.Select(x => x.Id))).ToList();
            var faileUserModelHasBatchrenewal = faileUserModel.Where(x => x.RenewalType == 5).ToList();
            var faileUserModelHasBatchrenewalIds = faileUserModelHasBatchrenewal.Select(x => x.Id).ToList();
            var ee = _context.Database.SqlQuery<bx_batchrenewal_item>(string.Format("SELECT * FROM bx_batchrenewal_item WHERE  buid IN({0}) AND isnew=1", string.Join(",", faileUserModelHasBatchrenewalIds))).ToList();

            var r6 = ee.Where(x => x.BizEndDate != null && x.BizEndDate.Value.ToString("yyyy") != "1900").Select(x => new CC { BuId = x.BUId, BizEndDate = x.BizEndDate }).ToList();
            var rt3 = rt2.Union(r6).ToList();
            var rt = (from a in useconsumer_reviewModel
                      join b in rt3
                      on a.BuId equals b.BuId
                      select new { BuId = a.BuId, BizEndDate = b.BizEndDate, CreateTime = a.UseTime }).ToList();
            List<AA> aaList = new List<AA>();
            foreach (var item in rt)
            {
                aaList.Add(new AA { b_uid = item.BuId, BizEndTime = GetDateTime(item.BizEndDate.Value, item.CreateTime.Value) });

            }
            if (aaList.Count > 10000)
            {
                for (int i = 0; i <= aaList.Count / 10000; i++)
                {
                    MySqlHelper.BulkUpdateByList<AA>(aaList.Skip(i * 10000).Take(10000).ToList(), System.Configuration.ConfigurationManager.ConnectionStrings["zb"].ConnectionString, "bx_consumer_review", "b_uid");
                }
            }
            else
            {
                MySqlHelper.BulkUpdateByList<AA>(aaList, System.Configuration.ConfigurationManager.ConnectionStrings["zb"].ConnectionString, "bx_consumer_review", "b_uid");
            }



        }
        private DateTime GetDateTime(DateTime bizEndTime, DateTime useTime)
        {
            var newBizEndTime = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-" + bizEndTime.Month + "-" + bizEndTime.Day.ToString() + " " + bizEndTime.Hour + ":" + bizEndTime.Minute + ":" + bizEndTime.Second);
            var nextBizEndTime = newBizEndTime.AddYears(1);
            DateTime useBizEndTime = DateTime.Now;
            if (useTime >= nextBizEndTime.AddDays(-180))
            {
                useBizEndTime = nextBizEndTime;
            }
            else if (useTime >= newBizEndTime.AddDays(-180))
            {

                useBizEndTime = newBizEndTime;
            }
            else
            {
                useBizEndTime = newBizEndTime.AddYears(-1);
            }
            return useBizEndTime;
        }

        public RenewalInformationViewModel GetRenewalInformation(string sql)
        {
            var renewalInformation = new RenewalInformationViewModel();
            var sqlhelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zb"].ConnectionString);
            var dataSet = sqlhelper.ExecuteDataSet(sql);

            string userinfoRenewalInfoStr = CommonHelper.TToJson(dataSet.Tables[0]);
            string consumerReviewStr = CommonHelper.TToJson(dataSet.Tables[1]);
            string quotereqCarinfoStr = CommonHelper.TToJson(dataSet.Tables[2]);
            string carRenewalStr = CommonHelper.TToJson(dataSet.Tables[3]);
            string carinfoStr = CommonHelper.TToJson(dataSet.Tables[4]);
            string agentStr = CommonHelper.TToJson(dataSet.Tables[5]);
            string batchrenewalItemStr = CommonHelper.TToJson(dataSet.Tables[6]);
            string carRenewalPremiumStr = CommonHelper.TToJson(dataSet.Tables[7]);

            renewalInformation.UserinfoRenewalInfo =
                userinfoRenewalInfoStr.ToListT<bx_userinfo_renewal_info>().FirstOrDefault();
            renewalInformation.ConsumerReview =
                consumerReviewStr.ToListT<bx_consumer_review>().FirstOrDefault();
            renewalInformation.QuotereqCarinfo =
                quotereqCarinfoStr.ToListT<bx_quotereq_carinfo>().FirstOrDefault();
            renewalInformation.CarRenewal =
                carRenewalStr.ToListT<bx_car_renewal>().FirstOrDefault();
            renewalInformation.Carinfo =
                carinfoStr.ToListT<bx_carinfo>().FirstOrDefault();
            renewalInformation.Agent =
                agentStr.ToListT<bx_agent>().FirstOrDefault();
            renewalInformation.BatchrenewalItem =
                batchrenewalItemStr.ToListT<bx_batchrenewal_item>().FirstOrDefault();
            renewalInformation.CarRenewalPremium =
                carRenewalPremiumStr.ToListT<bx_car_renewal_premium>().FirstOrDefault();

            return renewalInformation;
        }

    }
    class AA
    {
        public long b_uid { get; set; }
        public DateTime BizEndTime { get; set; }
    }
    class CC
    {
        public long BuId { get; set; }
        public DateTime? BizEndDate { get; set; }
    }
    class BB
    {
        public long BuId { get; set; }
        public DateTime? ABizEndDate { get; set; }
        public DateTime? BBizEndDate { get; set; }
    }
}
