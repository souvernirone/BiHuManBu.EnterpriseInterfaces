using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class GetCommissionIntegralService : IGetCommissionIntegralService
    {
        private readonly IRatePolicySettingRepository _ratePolicySettingRepository;
        private readonly IRatepolicyItemRepository _ratepolicyItemRepository;

        public GetCommissionIntegralService(IRatePolicySettingRepository ratePolicySettingRepository, IRatepolicyItemRepository ratepolicyItemRepository)
        {
            _ratePolicySettingRepository = ratePolicySettingRepository;
            _ratepolicyItemRepository = ratepolicyItemRepository;
        }

        public CommissionIntegralViewModel GetCommissionIntegral(CommissionIntegralRequest request)
        {
            try
            {
                var ll = Convert.ToSByte(0);
                //查询关系费率  佣金关系是否存在
                bx_ratepolicy_item ratepolicyItem = new bx_ratepolicy_item();
                if (!string.IsNullOrWhiteSpace(request.RbJSKJ))
                {
                    ratepolicyItem = _ratepolicyItemRepository.FirstOrDefault(
                            x =>
                                x.actuarial_calibre == request.RbJSKJ && x.car_used_type == request.CarUsedType &&
                                x.top_agent_id == request.Agent && x.is_delete == ll);
                }
                else
                {
                    ratepolicyItem =
                        _ratepolicyItemRepository.FirstOrDefault(
                            x => x.car_used_type == request.CarUsedType &&
                                x.top_agent_id == request.Agent && x.is_delete == ll);
                }
                if (ratepolicyItem != null)
                {
                    //确认费率
                    var ratePolicySetting =
                    _ratePolicySettingRepository.FirstOrDefault(x => x.id == ratepolicyItem.ratepolicy_setting_id);

                    if (ratePolicySetting != null)
                    {
                        var blBiz = ratePolicySetting.over_transfer_credits > ratePolicySetting.biz_rate;
                        var blForce = ratePolicySetting.over_transfer_credits > ratePolicySetting.force_rate;

                        var bizCommission = blBiz
                            ? request.BizTotal * (ratePolicySetting.biz_rate / 100) /1.06
                            : request.BizTotal * (ratePolicySetting.over_transfer_credits / 100) / 1.06;
                        var forceCommission = blForce
                            ? request.ForceTotal * (ratePolicySetting.force_rate / 100) / 1.06
                            : request.ForceTotal * (ratePolicySetting.over_transfer_credits / 100) / 1.06;

                        var bizIntegral = blBiz ? 0 : request.BizTotal * (ratePolicySetting.biz_rate - ratePolicySetting.over_transfer_credits) / 100 / 1.06;
                        var forceIntegral = blForce ? 0 : request.ForceTotal * (ratePolicySetting.force_rate - ratePolicySetting.over_transfer_credits) / 100 / 1.06;

                        //var commission = bizCommission + forceCommission;
                        //var integral = bizIntegral + forceIntegral;

                        return new CommissionIntegralViewModel()
                        {
                            BusinessStatus = 1,
                            StatusMessage = "获取成功",
                            BizCommission = Convert.ToDouble(String.Format("{0:F}", bizCommission)),
                            BizIntegral = Convert.ToInt32(bizIntegral),
                            ForceCommission = Convert.ToDouble(String.Format("{0:F}", forceCommission)),
                            ForceIntegral = Convert.ToInt32(forceIntegral),
                            Commission = Convert.ToDouble(String.Format("{0:F}", bizCommission)) + Convert.ToDouble(String.Format("{0:F}", forceCommission)),
                            Integral = Convert.ToInt32(bizIntegral) + Convert.ToInt32(forceIntegral)
                        };
                    }
                }

                return new CommissionIntegralViewModel()
                {
                    BusinessStatus = 0,
                    StatusMessage = "获取成功"
                };
            }
            catch (Exception ex)
            {
                return new CommissionIntegralViewModel()
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器发生异常：" + ex.Message
                };
            }

        }
    }
}
