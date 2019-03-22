using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class ThirdOrderCarInfo
    {
        /// <summary>
        /// 精友编码
        /// </summary>
        public string AutoMoldCode { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string ForceStartDate { get; set; }
        /// <summary>
        /// 车辆使用性质：1：家庭自用车（默认），2：党政机关、事业团体，3：非营业企业客车，6：营业货车，7：非营业货车以下几个不支持4：不区分营业非营业，5：出租租赁
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车辆识别代码
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 新车购置价
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 是否新车
        /// </summary>
        public int IsNewCar { get; set; }
        /// <summary>
        /// 车辆类型：1：客车（默认），2：货车（仅人保），3：半挂牵引车（仅人保），4：货车挂车（仅人保），5：油罐车（仅人保），6：气罐车（仅人保），7：液罐车（仅人保），8：冷藏车（仅人保），9：罐车挂车（仅人保），10：混凝土搅拌车（仅人保），11：特种车二挂车（仅人保），12：特种车二类其他（仅人保），13：监测车（仅人保），14：警用特种车（仅人保），15：混凝土泵车（仅人保），16：特种车三类挂车（仅人保），17：特种车三类其他（仅人保）
        /// </summary>
        public int CarType { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
        /// <summary>
        /// 是否是公车：0:默认(按照续保出来的结果处理，如果续保失败，默认按照非公车处理),1:是公车,2:非公车
        /// </summary>
        public int IsPublic { get; set; }
        /// <summary>
        /// 是否贷款车  1贷款  0 不贷款  -1不知道是否贷款
        /// </summary>
        public int? IsLoans { get; set; }

        /// <summary>
        /// 过户时间  具体时间: 过户   "": 未过户    "-1": 不知道是否过户
        /// </summary>
        public string TransferDate { get; set; }
    }
}
