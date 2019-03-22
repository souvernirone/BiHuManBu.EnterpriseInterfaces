using System;
using System.Data.Entity.Migrations;
using BiHuManBu.ExternalInterfaces.Models;
using System.Linq;
using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class UserinfoRenewalInfoRepository : EfRepositoryBase<bx_userinfo_renewal_info>, IUserinfoRenewalInfoRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private EntityContext _context;
        public UserinfoRenewalInfoRepository(DbContext context)
            : base(context)
        {
            _context = new EntityContext();
        }

        public int Add(bx_userinfo_renewal_info bxWorkOrder)
        {
            int workOrderId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_userinfo_renewal_info.Add(bxWorkOrder);
                DataContextFactory.GetDataContext().SaveChanges();
                workOrderId = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderId = 0;
            }
            return workOrderId;
        }

        public int Update(bx_userinfo_renewal_info bxWorkOrder)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_userinfo_renewal_info.AddOrUpdate(bxWorkOrder);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public bx_userinfo_renewal_info FindById(int workOrderId)
        {
            var bxWorkOrder = new bx_userinfo_renewal_info();
            try
            {
                bxWorkOrder = DataContextFactory.GetDataContext().bx_userinfo_renewal_info.FirstOrDefault(x => x.id == workOrderId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxWorkOrder;
        }

        public bx_userinfo_renewal_info GetByBuid(int buid)
        {
            return FirstOrDefault(o => o.b_uid == buid);
        }

        public bx_userinfo_renewal_info FindByBuid(long buid)
        {
            var bxWorkOrder = new bx_userinfo_renewal_info();
            //try
            //{
            bxWorkOrder = Table.OrderByDescending(o => o.create_time).FirstOrDefault(x => x.b_uid == buid);
            //}
            //catch (Exception ex)
            //{
            //    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            //}
            return bxWorkOrder;
        }

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<MinUserInfoRenewalInfo> FindBuIdAndClientMobile(List<long> buids)
        {
            var data = from info in Table
                       where info.b_uid.HasValue && buids.Contains(info.b_uid.Value)
                       select new MinUserInfoRenewalInfo
                       {
                           BuId = info.b_uid.Value,
                           ClientMobile = info.client_mobile,
                           ClientMobileOther = info.client_mobile_other,
                           Remark = info.remark,
                           ClientName = info.client_name,
                           IntentionRemark=info.intention_remark,
                           ClientAddress=info.client_address
                       };
            return data.ToList();
        }

        /// <summary>
        /// 根据buid获取BuIdAndClientMobile(sql)
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<MinUserInfoRenewalInfo> FindBuIdAndClientMobileSql(List<long> buids)
        {
            //string sql = "select b_uid,client_mobile,client_mobile_other,remark,client_name from bx_userinfo_renewal_info where b_uid in (" + string.Join(",", buids) + ")";
            //return DataContextFactory.GetDataContext().Database.SqlQuery<MinUserInfoRenewalInfo>(sql).ToList();

            var list = DataContextFactory.GetDataContext().bx_userinfo_renewal_info.Where(t => buids.Contains((long)t.b_uid)).ToList();
            var result = list.Select(t => new MinUserInfoRenewalInfo
            {
                BuId = (long)t.b_uid,
                ClientMobile = t.client_mobile,
                ClientMobileOther = t.client_mobile_other,
                Remark = t.remark,
                ClientName = t.client_name,
            }).ToList();

            return result;
        }

        /// <summary>
        /// 添加车牌和关注微信的人的关系
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="licenseNo"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public int AddWChatLicenseNoOpenIdRelationship(string openId, string licenseNo,
            int topAgentId, int cityCode)
        {

            var wchat = new bx_agent_wchat()
            {
                create_time = DateTime.Now,
                licenseno = licenseNo,
                open_id = openId,
                top_agent_id = topAgentId,
                CityCode = cityCode
            };
            _context.bx_agent_wchat.Add(wchat);
            int result = _context.SaveChanges();
            return result;
        }
        /// <summary>
        /// 通过车牌，顶级代理获取openid
        /// </summary>
        /// <returns></returns>
        public bx_agent_wchat GetOpenIdByLicenseNo(string licenseNo, int topAgentId, string openId, int requestType)
        {
            string strSql = string.Empty;
            switch (requestType)
            {
                case 1:
                    if (!string.IsNullOrEmpty(licenseNo) && topAgentId != 0)
                        strSql = string.Format("select * from bx_agent_wchat where licenseno='{0}' and top_agent_id={1} limit 1",licenseNo,topAgentId);
                    break;
                case 2:
                    if (!string.IsNullOrEmpty(openId) && topAgentId != 0)
                        strSql = string.Format("select * from bx_agent_wchat where open_id='{0}' and top_agent_id={1} limit 1", openId, topAgentId);
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(strSql)) {
                return null;
            }
            bx_agent_wchat model = DataContextFactory.GetDataContext().Database.SqlQuery<bx_agent_wchat>(strSql).FirstOrDefault();
            return model;
        }
    }
}
