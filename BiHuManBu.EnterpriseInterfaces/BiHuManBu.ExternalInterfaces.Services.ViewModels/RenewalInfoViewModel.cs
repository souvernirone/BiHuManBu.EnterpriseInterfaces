using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
   public class RenewalInfoViewModel:BaseViewModel
    {
       /// <summary>
       /// 客户信息
       /// </summary>
        public CustomerInfo CusomerInfoModel { get; set; }
       /// <summary>
       /// 车辆信息
       /// </summary>
        public CarInfo CarInfoModel { get; set; }

       
    }
   public class CustomerInfo
   {
       /// <summary>
       /// 客户姓名
       /// </summary>
       public string CustomerName { get; set; }
       /// <summary>
       /// 客户电话
       /// </summary>
       public string CustomerMobile { get; set; }
   }

   public class CarInfo
   {
       /// <summary>
       /// 车牌号
       /// </summary>
       public string  Licenseno { get; set; }
       /// <summary>
       /// 车架号
       /// </summary>
       public string CarVin { get; set; }
       /// <summary>
       /// 发动机号
       /// </summary>
       public string  EngineNo { get; set; }
       /// <summary>
       /// 车辆使用性质
       /// </summary>
       public int CarUsedType { get; set; }
       /// <summary>
       /// 品牌型号
       /// </summary>
       public string MoldName { get; set; }
       /// <summary>
       /// 车辆注册日期
       /// </summary>
       public string  RegisterDate { get; set; }
       /// <summary>
       /// 车主姓名
       /// </summary>
       public string  LicenseOwner { get; set; }
       /// <summary>
       /// 证件类型
       /// </summary>
       public int OwnerIdCardType { get; set; }
       /// <summary>
       /// 证件号码
       /// </summary>
       public string OwnerIdCard { get; set; }
   }

}
