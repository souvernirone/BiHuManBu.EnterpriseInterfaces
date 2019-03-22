using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public interface IOrderRepository
    {
        bx_quotereq_carinfo FindQuotereqCarinfo(long buid);
        dd_order FindOrderNum(long buId);
        dd_order FindOrderNum(List<long> buIds);
        dd_order FindOrder(long orderId);
        dd_order FindOrder(string orderNum);
        List<dd_order> FindOrderListByBuid(long buId);
        dd_order_quoteresult FindDdOrderQuoteresult(long orderId);

        Task<dd_order_quoteresult> FindDdOrderQuoteresultAsync(long orderId);

        dd_pay_way FindBankWayId(int cityId);

        List<dd_order_savequote> FinDdOrderSavequotes(long[] ids);
        dd_order_savequote FindDdOrderSavequote(long orderId);

        Task<dd_order_savequote> FindDdOrderSavequoteAsync(long orderId);
        dd_order_related_info FindDdOrderRelatedinfo(long orderId);

        Task<dd_order_related_info> FindDdOrderRelatedinfoAsync(long orderId);
        List<dd_order_paymentresult> GetOrderPayResult(string orderNum);

        Task<List<dd_order_paymentresult>> GetOrderPayResultAsync(string orderNum);

        /// <summary>
        /// 查询订单
        /// 陈亮  2017-08-16  
        /// </summary>
        /// <param name="search"></param>
        /// <param name="listAgentId">业务员集合</param>
        /// <param name="listIssuingPerpleId">出单员集合</param>
        /// <param name="isAndOr">业务员和出单员是and还是or的关系，1：and  2：or</param>
        /// <returns></returns>
        SearchOrderDto SearchOrder(SearchOrderRequest search, List<int> listAgentId, List<int> listIssuingPerpleId, int isAndOr, int? CarOwnerId = null);


        #region 身份证采集&支付
        /// <summary>
        /// 获取机采集的设备码
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="configId">渠道</param>
        /// <param name="source">保险公司</param>
        /// <returns></returns>
        List<Machine> GetMachines(int agentId, int configId, int source);

        List<Machine> GetMachines(int agentId, int source);
        /// <summary>
        /// 更新验证码的有效时间
        /// </summary>
        /// <param name="orderNum">订单Id(dd_order.orderNum)</param>
        /// <returns></returns>
        bool UpdateOrderVerificationDate(string orderNum);
        /// <summary>
        /// 身份证验码状态 0未采集 1已验证 2已失效
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="status">状态 0未采集 1已验证 2已失效</param>
        /// <returns></returns>
        bool UpdateVerificationCodeStatus(string orderNum, int status);
        /// <summary>
        /// 查看验证码的时间是否有效
        /// </summary>
        /// <param name="orderNum">订单号(dd_order.orderNum)</param>
        /// <returns></returns>
        bool CheckOrderVerificationDate(string orderNum);
        /// <summary>
        /// 获取支付状态
        /// </summary>
        /// <param name="orderNum">订单Id(dd_order.orderNum)</param>
        /// <param name="type">支付类型 1=全款支付 、2=净费支付</param>
        /// <returns></returns>
        dd_order_paymentresult GetOrderPayResult(string orderNum, int type);
        /// <summary>
        /// 添加支付结果
        /// </summary>
        /// <param name="model">支付结构实体</param>
        /// <returns></returns>
        bool InsertOrderPayResult(dd_order_paymentresult model);

        /// <summary>
        /// 通过代理人获取绑定的采集系统账号
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<bx_agent_busiuser> GetBusiuserIdsByAgentId(int agentId);

        /// <summary>
        /// 修改订单支付维码创建时间及地址
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="qrUrl">二维码地址</param>
        /// <returns></returns>
        bool ModifyOrderQR(string orderNum, string qrUrl);
        /// <summary>
        /// 添加orderSerial
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool InsertOrderSerial(dd_order_serial model);
        /// <summary>
        /// 根据订单获取支付的二维码
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        string GetQROrderQR(string orderNum);

        /// <summary>
        /// 根据订单修改支付状态
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="payType">支付类型 1=净费支付 2=全款支付</param>
        ///  <param name="payStatus">支付状态 0=未支付  1=已支付</param>
        /// <returns></returns>
        bool ModifyOrderPayStatus(string orderNum, int payType, int payStatus);
        bool ModifyOrder(dd_order model);
        /// <summary>
        /// 获取该订单的最近一条支付流水
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        dd_order_serial FindOrderSerial(string orderNum);

        /// <summary>
        /// 修改支付流水结果
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="payResult">状态 0=初始、1=成功</param>
        /// <returns></returns>
        bool ModifyOrderSerialPayResult(string orderNum, int payResult);
        /// <summary>
        /// 修改支付流水结果
        /// </summary>
        /// <param name="model">流水表实体</param>
        /// <returns></returns>
        bool ModifyOrderSerialPayResult(dd_order_serial model);
        /// <summary>
        /// 获取支付方式合作银行
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<PayWayBanksModel> GetPayWayBanks(PayWayBanksModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">dd_pay_way.id</param>
        /// <returns></returns>
        PayWayBanksModel GetPayWayBank(int id);
        /// <summary>
        /// 获取最近的一次采集信息记录
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        dd_order_collection GetOrdderCollection(string orderNum, int? status);
        /// <summary>
        /// 添加次采集信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        int InsertOrdderCollection(dd_order_collection model);
        /// <summary>
        /// 修改次采集信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        int ModifyOrdderCollection(dd_order_collection model);

        /// <summary>
        /// 获取最新的一条记录
        /// </summary>
        /// <param name="transNo">商户订单号</param>
        /// <returns></returns>
        dd_pay_ax GetPayAx(string transNo);
        dd_pay_ax GetPayAx(long BuId, string orderNum);
        dd_pay_ax GetPayAxByOrderNo(string orderNo, long buId = 0);
        /// <summary>
        /// 修改安心支付信息信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        int ModifyPayAx(dd_pay_ax model);
        /// <summary>
        /// 添加安心支付信息记录
        /// </summary>
        /// <param name="model">修改实体</param>
        /// <returns>影响行数</returns>
        dd_pay_ax InsertPayAx(dd_pay_ax model);

        /// <summary>
        /// bx_anxin_delivery
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bx_anxin_delivery GetAnxinDelivery(long buId);
        #endregion

        /// <summary>
        /// 根据账号密码获取bx_busiuser
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        Task<bx_busiuser> GetBusiuserByUserNamePwdAsync(string userName, string pwd);

        /// <summary>
        /// 绑定采集码
        /// </summary>
        /// <param name="listAgentBusiuser"></param>
        /// <returns></returns>
        Task<bool> BandAgentBusiuserAsync(List<bx_agent_busiuser> listAgentBusiuser);

        /// <summary>
        /// 更新bx_agent_busiuser
        /// </summary>
        /// <param name="agentBusiuser"></param>
        /// <returns></returns>
        bool UpdateAgentBusiuser(bx_agent_busiuser agentBusiuser);

        /// <summary>
        /// 获取bx_agent_busiuser
        /// </summary>
        /// <param name="agentBusiuserId"></param>
        /// <returns></returns>
        Task<bx_agent_busiuser> FindAgentBusiuserAsync(int agentBusiuserId);

        /// <summary>
        /// 根据agent获取Busiuser的部分信息
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        List<ListBusiuserDto> GetBusiuserByAgent(int agent);


        long CreateOrderDetail(dd_order order, dd_order_related_info orderRelatedInfo, bx_userinfo userinfo, bx_savequote savequote, bx_submit_info submitInfo, bx_quoteresult quoteresult, bx_quoteresult_carinfo carInfo, out CreateOrderReturnModel model);

        int UpdateOrder(dd_order order);

        int Update(dd_order_quoteresult quoteresult);

        int UpdateSubmitinfo(bx_submit_info submitInfo);

        int AddOrderSteps(dd_order_steps orderSteps);

        /// <summary>
        /// 判断是否录入支付结果
        /// 陈亮   2017-9-1
        /// </summary>
        /// <param name="orderId">订单id(dd_order.id )</param>
        /// <param name="type">支付类型 1=全款支付 、2=净费支付</param>
        /// <returns></returns>
        Task<bool> IsExistPaymentResultAsync(long orderId, int type);

        List<dd_order_steps> GetOrderSteps(long[] orderIds);
        List<dd_order_steps> GetOrderSteps(long orderId);

        Task<List<dd_order_steps>> GetOrderStepsAsync(long orderId);

        /// <summary>
        /// 根据bx_agent_busiuser.Id和agentid获取还没有绑定的设备列表
        /// </summary>
        /// <param name="busiuserId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        Task<List<BusiuserSettingDto>> GetBusiuserListAsync(int busiuserId, int agentId);

        /// <summary>
        /// 绑定采集器时获取bx_busiusersetting的部分信息
        /// </summary>
        /// <param name="listId">bx_busiusersetting.id</param>
        /// <returns></returns>
        Task<List<BandBusiuserSettingDto>> GetBusiusersettingPartialAsync(List<int> listId);

        /// <summary>
        /// 获取bx_busiusersetting
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bx_busiusersetting> GetBusiusersettingAsync(int id);

        OrderInformationViewModel GetOrderInformation(string sql);

        /// <summary>
        /// 获取可以使用的渠道
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="source"></param>
        /// <param name="agentBusiuserId"></param>
        /// <returns></returns>
        List<BusiuserAgentConfigDto> GetCanUseAgentConfig(int agent, int source, int agentBusiuserId);

        /// <summary>
        /// 获取团队下代理的订单
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<OrderAgentAmountViewModel> GetTeamOrder(string sql);

        /// <summary>
        /// 修改订单配送信息 2018-09-14 张克亮
        /// </summary>
        /// <param name="request">修改订单请求模型</param>
        /// <returns></returns>
        int UpdateOrderDeliveryInfo(OrderDeliveryInfoRequest request);

        /// <summary>
        /// 获取订单配送信息 2018-09-17 张克亮
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <returns></returns>
        OrderDeliveryInfoResponse GetOrderDeliveryInfo(string orderNum);

        List<dd_order_quoteresult> GetOrderQuoteResultListByOrderId(List<long> orderIds);
    }
}
