using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class WaTaiPySubmitResponse : BaseResponse
    {
        public WaTaiPySubmitResponseInfo TaiPySubmitInfo;
    }
    public class WaTaiPySubmitResponseInfo
    {
        public string Random;
        //public string orderId;
        public string OrderNo;
        public string SessionId;
        public string engineNo { get; set; }
        public string carVin { get; set; }
        public string oriPurchasePrice;
        public string moldName;
        public string currentValue;
        public int IsNewTaiPyUser;
        public string registerDate;
        public double NonClaimDiscountRate;
        public int lastYearAccTimes;
        public double LastYearClaimAmount;
        public int lastYearClaimTimes;
        public string licenseOwner;
        /// <summary>
        /// 身份证号，只有太平洋老用户能取到
        /// </summary>
        public string InsuredIdentifyNo;
    }
    public class TaiPyQueryVehicleByVinEngiInfo
    {
        public string bodyType;//3厢
        public string EmptyWeight;
        public string engineCapacity;
        public string engineDesc;
        public string moldName;//雪佛兰SGM7140MTB
        public double price;
        public string rVehicleFamily;//赛欧
        public string vehicleBrand;//上海通用雪佛兰
    }
}
