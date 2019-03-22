using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class AXEntity
    {

    }

    /// <summary>
    /// 安心支付请求数据
    /// </summary>
    public class AXPayRequest : BaseRequest2
    {
        /// <summary>
        /// dd_order.id
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 商户订单号（唯一）
        ///保持在系统内唯一 编码规则：渠道编码+yyyyMMrrHHmmss+随机数（最多32个字符，可包含字母、数字
        /// </summary>
        public string transNo { get; set; }
        /// <summary>
        /// 请求方代码
        /// 安心提供
        /// </summary>
        public string requestCode { get; set; }
        /// <summary>
        /// int 交易金额    Y 以分为单位Int类型
        /// </summary>       
        public int transAmt { get; set; }
        /// <summary>
        /// 支付名称 Y   支付商品的描述，例如：保险产品名称“机动车保险”
        /// </summary>
        public string payName { get; set; }
        /// <summary>
        ///  后台回调地址 Y   回调参数详见2.3 商户通知
        /// </summary>
        public string bgRetUrl { get; set; }
        /// <summary>
        /// 描述  N
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 加密串 Y 通过md5对(合作验证码 + 原始数据) 进行签名, 测试环境：合作验证码：xxx，加密方式
        /// </summary>
        public string checkValue { get; set; }
        /// <summary>
        /// 支付取消页面地址    Y 第三方页面地址
        /// </summary>
        public string payCancelURL { get; set; }
        /// <summary>
        /// 支付完成页面地址 Y   第三方页面地址，支付操作完成，并不意味着支付成功
        /// </summary>
        public string payFinishURL { get; set; }
        /// <summary>
        ///  支付方式    Y 支付方式 payType支付方式 枚举值
        /// </summary>
        public string payType { get; set; }
        /// <summary>
        ///  附加数据 N   通知原样返回，携带自定义参数
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 错误返回页面  N 此页面为支付出现错误时，跳转页面，如不传则跳转平台默认错误页面。
        /// </summary>
        public string payErrorURL { get; set; }
        /// <summary>
        /// 支付截止日期 N   支付失效的截止日期,格式（yyyyMMrrHHmmss）
        /// </summary>
        public string limitTime { get; set; }
        /// <summary>
        /// 投保单号 N   投保单号，多个用逗号相隔。例如111111,22222（展示作用）
        /// </summary>
        public string appNoList { get; set; }
        public string Biztno { get; set; }
        public string Forcetno { get; set; }
        /// <summary>
        /// 是否实名认证 Y	0 否， 1 是 （实名认证仅限微信相关支付类型）
        /// </summary>
        public string isTrueName { get; set; }
        /// <summary>
        /// 投保人名称 N   当实名认证为1时，字段必传，名称需urlencode编码,编码格式utf-8，例如：%E6%9D%8E%E5%9B%9B
        /// </summary>
        public string appNameList { get; set; }
        /// <summary>
        /// 投保人证件类型 N 当实名认证为1时，字段必传
        /// </summary>
        public string cardTypeList { get; set; }
        /// <summary>
        /// 投保人证件号码 N 当实名认证为1时，字段必传
        /// </summary>
        public string cardNoList { get; set; }
        public int BuId { get; set; }
        public string LicenseNo { get; set; }
        /// <summary>
        /// 预下单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 交易金额(单位:分)
        /// </summary>
        public int PayAmt { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }

        public string CarVin { get; set; }

        public int PayMent { get; set; }
    }

    /// <summary>
    /// 安心支付接收数据
    /// </summary>
    public class AXPayResponse
    {
        /// <summary>
        /// 预下单号
        /// </summary>
        public string orderNo { get; set; }

        public string OrderNum { get; set; }

        /// <summary>
        /// 商户订单号（唯一）
        /// </summary>
        public string transNo { get; set; }
        /// <summary>
        /// 交易时间 yyyyMMddHHmmss
        /// </summary>
        public string transDate { get; set; }
        /// <summary>
        /// 请求方代码
        /// </summary>
        public string requestCode { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string payNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string payType { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>

        public string payAmt { get; set; }
        /// <summary>
        /// 支付日期
        /// </summary>
        public string payDate { get; set; }
        /// <summary>
        /// 支付结果
        /// </summary>

        public string payResult { get; set; }
        /// <summary>
        /// 支付结果描述
        /// </summary>
        public string payRemark { get; set; }
        /// <summary>
        /// 加密串
        /// </summary>
        public string checkValue { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 银联POS交易参考号
        /// </summary>
        public string referNo { get; set; }
        public long BuId { get; set; }

        public int ChannelId { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string CAppValidateNo { get; set; }
        public string Biztno { get; set; }
        public string ForcetNo { get; set; }
    }

    /// <summary>
    /// 安心支付请求返回数据
    /// </summary>
    public class AXPayEntity
    {
        /// <summary>
        /// 校验码
        /// </summary>
        public string checkValue { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizpNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcepNo { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public string PaymentDateStr { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string TransNo { get; set; }
        /// <summary>
        /// 请求方代码
        /// </summary>
        public string RequestCode { get; set; }

        public string ErrorMsg { get; set; }
        public int? Code { get; set; }
        public double? Money { get; set; }
        public int? FindPayResult { get; set; }
    }

    /// <summary>
    /// 安心承保请求主体
    /// </summary>
    public class PolicyGenerateRequestMain
    {
        /// <summary>
        /// 支付申请号 Y 支付生成唯一编号(2018年5月9日21:06:02  修改为orderNo)
        /// </summary>
        public string cPaySequence { get; set; }
        /// <summary>
        /// 银行交易流水 Y  支付成功后返回的
        /// </summary>
        public string cBizConsultNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string cTerNo { get; set; }

        /// <summary>
        /// 支付时间  Y 如20171023113704  
        /// </summary>
        public string tChargeTm { get; set; }

        /// <summary>
        /// 收费方式 Y  微信支付，
        /// </summary>
        public string cPayTyp { get; set; }

        /// <summary>
        /// 支付账户（卡号）
        /// </summary>
        public string cCardNo { get; set; }

        /// <summary>
        /// 缴费确认信息 Y 区分交强和商业
        /// </summary>
        public List<PayConfirmInfoVO> PayConfirmInfoList { get; set; }

        public int? ChannelId { get; set; }
    }

    public class PayConfirmInfoVO
    {
        /// <summary>
        /// 申请单号 Y   投保单号
        /// </summary>
        public string cAppNo { get; set; }

        /// <summary>
        /// 保单号 Y    保单号
        /// </summary>
        public string cPlyNo { get; set; }

        /// <summary>
        /// 状态  Y  1:生成保单成功；0：生成保单失败
        /// </summary>
        public string flag { get; set; }

        /// <summary>
        /// 结果备注  ,失败时传送异常信息 
        /// </summary>
        public string cRstMsg { get; set; }
        /// <summary>
        /// 承保验证码
        /// </summary>
        public string cAppValidateNo { get; set; }
    }

    /// <summary>
    /// 安心承保接口返回实体
    /// </summary>
    public class PolicyGenerateResponse : WaBaseResponse
    {
        /// <summary>
        /// 缴费确认信息列表
        /// </summary>
        public List<PayConfirmInfoVO> PolicyGenerateResponseMain { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WaBaseResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrMsg { get; set; }
        /// <summary>
        /// 机器人接口版本号 
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 版本类型
        /// </summary>
        public string VersionType { get; set; }
    }




}
