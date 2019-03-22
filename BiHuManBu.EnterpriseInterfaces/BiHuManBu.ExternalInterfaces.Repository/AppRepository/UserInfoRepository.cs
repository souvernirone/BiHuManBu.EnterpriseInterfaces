using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_userinfo;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using log4net;
using MySql.Data.MySqlClient;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class UserInfoRepository : AppIRepository.IUserInfoRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        
        public bx_userinfo FindByBuid(long buid)
        {
            bx_userinfo userinfo = new bx_userinfo();
            try
            {
                userinfo = DataContextFactory.GetDataContext().bx_userinfo.AsNoTracking().FirstOrDefault(x => x.Id == buid);
            }
            catch (Exception ex)
            {

                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return userinfo;
        }

        /// <summary>
        /// 查询唯一bx_userinfo
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="licenseno"></param>
        /// <param name="agent"></param>
        /// <param name="cartype">大小车标识 0小车</param>
        /// <param name="isTest">0未删除数据 1已删除</param>
        /// <returns></returns>
        public bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno, string agent, int cartype=0, int isTest = 0)
        {
            bx_userinfo tt = new bx_userinfo();
            try
            {
                tt = DataContextFactory.GetDataContext().bx_userinfo.FirstOrDefault(x => x.Agent == agent && x.LicenseNo == licenseno && x.OpenId == openid && x.RenewalCarType == cartype && x.IsTest == isTest);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return tt;
        }

        public long Add(bx_userinfo userinfo)
        {
            bx_userinfo item = new bx_userinfo();
            try
            {
                item = DataContextFactory.GetDataContext().bx_userinfo.Add(userinfo);
                var returnResult = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return item.Id;
        }

        public int Update(bx_userinfo userinfo)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_userinfo.AddOrUpdate(userinfo);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {

                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
    }
}
