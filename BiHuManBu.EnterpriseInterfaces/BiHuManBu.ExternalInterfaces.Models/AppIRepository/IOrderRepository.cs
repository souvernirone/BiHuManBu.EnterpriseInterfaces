using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using AppViewModels=BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IOrderRepository
    {
        int Update(bx_car_order order);
        bx_car_order FindBy(long orderId);
        bx_car_order FindBy(string orderNo);
        bx_car_order FindBy(string licenseNo, string openId, int topAgent);
        List<AppViewModels.CarOrderModel> FindListBy(int status,bool isAgent, List<bx_agent> sonself, string openid, int agentId, string search, int pageIndex, int pageSize, out int totalCount);
        List<AppViewModels.CarOrderModel> FindListForApp(int status, List<string> sonself, string openId, int agentId, string search, int pageIndex, int pageSize, out int totalCount);
        //List<bx_car_order> FindListBy(long user_id, int top_agent);
        //List<bx_car_order> FindByUserId(long user_id, int top_agent);
        AppViewModels.CarOrderModel FindCarOrderBy(long orderId);//, string openId

        long CreateOrder(bx_car_order order, user user, bx_address address, bx_lastinfo lastinfo, bx_userinfo userinfo, bx_savequote savequote, bx_submit_info submitInfo, bx_quoteresult quoteresult, bx_quoteresult_carinfo carInfo, List<bx_claim_detail> claimDetails);
        
        /// <summary>
        /// 获取报价单，预约单，保单的数量
        /// </summary>
        /// <param name="agentStr"></param>
        /// <param name="custKey"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        AppViewModels.GetCountsViewModel GetCounts (string agentStr, string custKey, int agent);
        bx_car_order FindOrderByOrderId(long orderId,string orderNum);
    }
}
