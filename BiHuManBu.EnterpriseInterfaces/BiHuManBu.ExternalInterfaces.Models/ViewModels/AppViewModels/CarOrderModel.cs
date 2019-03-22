using System;
using System.Net;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CarOrderModel
    {
        //bx_car_order数据实体
        public long id { get; set; }
        public string order_num { get; set; }
        public Nullable<long> buid { get; set; }
        public long? source { get; set; }
        public string insured_name { get; set; }
        public string contacts_name { get; set; }
        public string mobile { get; set; }
        public Nullable<int> receipt_head { get; set; }

        public string receipt { get; set; }
        public string receipt_title { get; set; }
        public Nullable<int> pay_type { get; set; }
        public Nullable<int> distribution_type { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<decimal> total_price { get; set; }
        public Nullable<decimal> carriage_price { get; set; }
        public Nullable<decimal> insurance_price { get; set; }
        public Nullable<int> id_type { get; set; }
        public string id_num { get; set; }
        public Nullable<int> order_status { get; set; }
        public Nullable<int> pay_status { get; set; }
        public int addressid { get; set; }
        public string distribution_address { get; set; }
        public string distribution_phone { get; set; }
        public string distribution_name { get; set; }
        public Nullable<DateTime> distribution_time { get; set; }
        public string id_img_firs { get; set; }
        public string id_img_secd { get; set; }
        public Nullable<int> top_agent { get; set; }
        public string openid { get; set; }
        public Nullable<decimal> bizRate { get; set; }
        /// <summary>
        /// 交强险费率
        /// </summary>
        public Nullable<decimal> forcerate { get; set; }
        public string GetOrderTime { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        public int? InvoiceType { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public string imageUrls { get; set; }

        /// <summary>
        /// 商业险注册时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强险注册时间
        /// </summary>
        public string ForceStartDate { get; set; }


        //bx_userinfo数据实体
        public string UserName { get; set; }
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }

        public string CityCode { get; set; }

        /// <summary>
        /// 预约单的联系人邮箱 新增
        /// </summary>
        public string OrderEmail { get; set; }
        /// <summary>
        /// 预约单的联系人电话 新增
        /// </summary>
        public string OrderPhone { get; set; }

        //public string InsuredMobile { get; set; }
        //public string Email { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 车辆使用类型1：营运，0：非运营
        /// </summary>
        public int CarUsedType { get; set; }

        //保额
        //bx_savequote数据实体
        public Nullable<double> CheSun { get; set; }
        public Nullable<double> SanZhe { get; set; }
        public Nullable<double> DaoQiang { get; set; }
        public Nullable<double> SiJi { get; set; }
        public Nullable<double> ChengKe { get; set; }
        public Nullable<double> BoLi { get; set; }
        public Nullable<double> HuaHen { get; set; }
        public Nullable<double> BuJiMianCheSun { get; set; }
        public Nullable<double> BuJiMianSanZhe { get; set; }
        public Nullable<double> BuJiMianDaoQiang { get; set; }
        public Nullable<double> BuJiMianRenYuan { get; set; }
        public Nullable<double> BuJiMianFuJian { get; set; }

        //2.1.5版本修改 新增6个字段
        public Nullable<double> BuJiMianChengKe { get; set; }
        public Nullable<double> BuJiMianSiJi { get; set; }
        public Nullable<double> BuJiMianHuaHen { get; set; }
        public Nullable<double> BuJiMianSheShui { get; set; }
        public Nullable<double> BuJiMianZiRan { get; set; }
        public Nullable<double> BuJiMianJingShenSunShi { get; set; }
        //2.1.5新增补充
        public Nullable<double> HcSheBeiSunshi { get; set; }
        public Nullable<double> HcHuoWuZeRen { get; set; }
        public Nullable<double> HcFeiYongBuChang { get; set; }
        public Nullable<double> HcJingShenSunShi { get; set; }
        public Nullable<double> HcSanFangTeYue { get; set; }
        public Nullable<double> HcXiuLiChang { get; set; }

        public Nullable<double> TeYue { get; set; }
        public Nullable<double> SheShui { get; set; }
        public Nullable<double> CheDeng { get; set; }
        public Nullable<double> ZiRan { get; set; }
        public Nullable<int> JiaoQiang { get; set; }



        //保费
        //bx_quoteresult数据实体
        public Nullable<double> CheSunBaoFei { get; set; }
        public Nullable<double> SanZheBaoFei { get; set; }
        public Nullable<double> DaoQiangBaoFei { get; set; }
        public Nullable<double> SiJiBaoFei { get; set; }
        public Nullable<double> ChengKeBaoFei { get; set; }
        public Nullable<double> BoLiBaoFei { get; set; }
        public Nullable<double> HuaHenBaoFei { get; set; }
        public Nullable<double> BuJiMianCheSunBaoFei { get; set; }
        public Nullable<double> BuJiMianSanZheBaoFei { get; set; }
        public Nullable<double> BuJiMianDaoQiangBaoFei { get; set; }
        public Nullable<double> BuJiMianRenYuanBaoFei { get; set; }
        public Nullable<double> BuJiMianFuJianBaoFei { get; set; }

        //2.1.5版本修改 新增6个字段
        public Nullable<double> BuJiMianChengKeBaoFei { get; set; }
        public Nullable<double> BuJiMianSiJiBaoFei { get; set; }
        public Nullable<double> BuJiMianHuaHenBaoFei { get; set; }
        public Nullable<double> BuJiMianSheShuiBaoFei { get; set; }
        public Nullable<double> BuJiMianZiRanBaoFei { get; set; }
        public Nullable<double> BuJiMianJingShenSunShiBaoFei { get; set; }
        //2.1.5新增补充
        public Nullable<double> HcSheBeiSunshiBaoFei { get; set; }
        public Nullable<double> HcHuoWuZeRenBaoFei { get; set; }
        public Nullable<double> HcFeiYongBuChangBaoFei { get; set; }
        public Nullable<double> HcJingShenSunShiBaoFei { get; set; }
        public Nullable<double> HcSanFangTeYueBaoFei { get; set; }
        public Nullable<double> HcXiuLiChangBaoFei { get; set; }

        public Nullable<double> TeYueBaoFei { get; set; }
        public Nullable<double> SheShuiBaoFei { get; set; }
        public Nullable<double> CheDengBaoFei { get; set; }
        public Nullable<double> ZiRanBaoFei { get; set; }
        public Nullable<int> JiaoQiangBaoFei { get; set; }
        public Nullable<double> BizTotal { get; set; }
        public Nullable<double> ForceTotal { get; set; }
        public Nullable<double> TaxTotal { get; set; }

        public long OrderId { get; set; }
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public string CreateTime_Str { get; set; }
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }
    }
}
