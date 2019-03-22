using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public int Add(bx_address bxAddress)
        {
            int addressId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_address.Add(bxAddress);
                DataContextFactory.GetDataContext().SaveChanges();
                addressId = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                addressId = 0;
            }
            return addressId;
        }

        public bx_address Find(int addressId, int userid=0)
        {
            bx_address bxAddress=new bx_address();
            try
            {
                if (userid == 0)
                {
                    bxAddress = DataContextFactory.GetDataContext().bx_address.FirstOrDefault(x => x.id == addressId );
                }
                else {
                    bxAddress = DataContextFactory.GetDataContext().bx_address.FirstOrDefault(x => x.id == addressId && x.userid == userid);
                }
              
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
            return bxAddress;
        }

        public int Delete(int addressId, int userid)
        {
            int count = 0;
            try
            {
                var address =
                    DataContextFactory.GetDataContext().bx_address.FirstOrDefault(x => x.id == addressId && x.userid == userid);
                if (address != null)
                {
                    address.Status = 0;

                    DataContextFactory.GetDataContext().bx_address.AddOrUpdate(address);
                    count = DataContextFactory.GetDataContext().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public int Update(bx_address bxAddress)
        {
            int count = 0;
            try
            {
                var address =
                    DataContextFactory.GetDataContext().bx_address.FirstOrDefault(x => x.id == bxAddress.id && x.userid == bxAddress.userid);
                if (address != null)
                {
                    address.Name = bxAddress.Name;
                    address.address = bxAddress.address;
                    address.agentId = bxAddress.agentId;
                    address.areaId = bxAddress.areaId;
                    address.cityId = bxAddress.cityId;
                    address.phone = bxAddress.phone;
                    address.provinceId = bxAddress.provinceId;
                    address.updatetime = bxAddress.updatetime;
                    //2018-09-15 张克亮 省、市、区 名称
                    address.province_name = bxAddress.province_name;
                    address.city_name = bxAddress.city_name;
                    address.area_name = bxAddress.area_name;

                    DataContextFactory.GetDataContext().bx_address.AddOrUpdate(address);
                    count = DataContextFactory.GetDataContext().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public List<bx_address> FindByBuidAndAgentId(int userId,bool  isGetDefaultAddress=false)//, int agentId
        {
            List<bx_address> bxAddresses=new List<bx_address>();
            try
            {
                bxAddresses =
                   isGetDefaultAddress? DataContextFactory.GetDataContext().bx_address.Where(x => x.userid == userId && x.Status == 1&&x.isdefault).ToList(): DataContextFactory.GetDataContext().bx_address.Where(x => x.userid == userId && x.Status == 1).ToList();//&& x.agentId == agentId
            }
            catch (Exception ex )
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                
            }
            return bxAddresses;
        }
    }
}
