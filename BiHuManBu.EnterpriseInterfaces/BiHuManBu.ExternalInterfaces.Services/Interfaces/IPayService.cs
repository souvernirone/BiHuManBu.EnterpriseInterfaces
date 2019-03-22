using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IPayService
    {
        #region wyy 支付
        IList<Machine> Machines(CollectIdCardRequest request);
        BaseViewModel CollectIdCard(CollectIdCardRequest request);
        BaseViewModel CollectStatus(CollectIdCardRequest request);
        OrderPayment PayQR(OrderPayRequest requestt, dd_order order = null);
        BaseViewModel SendSms(OrderPayRequest request);

        Task<OrderPayment> Pay(OrderPayRequest reques);
        /// <summary>
        /// 作废原支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        OrderPayment VoidPayQR(OrderPayRequest request);
        /// <summary>
        /// 获取支付的合作银行
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IList<PayWayBanksModel> GetPayWayBanks(PayWayBanksModel model);

        /// <summary>
        /// 申请电子保单下载
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>                            
        BaseViewModel ApplyElecPolicy(EPolicycs request);
        BaseViewModel GetElecPolicyApply(EPolicycs request);
        /// <summary>
        /// 电子保单下载
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel DownloadElecPolicy(EPolicycs request);

        AXPayEntity GetAXPaySecrek(AXPayRequest model, bool isOut = false);

        string GetAxPayPath(AXPayRequest model, out string message);
        bool GetAXPayBack(AXPayResponse model);
        Task<AXPayEntity> GetAXPayInfo(AXPayResponse model, bool isOut = false);
        /// <summary>
        /// 补发电子保单
        /// </summary>
        /// <returns></returns>
        BaseViewModel ReissueElectronicPolicy(OrderPayRequest request);
        #endregion

        
    }
}
