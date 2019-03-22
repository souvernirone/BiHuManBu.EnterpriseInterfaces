using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class RatePolicySettingService : IRatePolicySettingService
    {

        private readonly IRatePolicySettingRepository _ratePolicySettingRepository;
        private readonly IRatepolicyItemRepository _ratepolicyItemRepository;
        private EntityContext context;

        public RatePolicySettingService(IRatePolicySettingRepository ratePolicySettingRepository
            , IRatepolicyItemRepository ratepolicyItemRepository)
        {
            _ratePolicySettingRepository = ratePolicySettingRepository;
            _ratepolicyItemRepository = ratepolicyItemRepository;
            context = DataContextFactory.GetDataContext();
        }

        public async Task<BaseViewModel> AddRateAsync(AddRateRequest request)
        {
            // 校验重复
            if (!await CheckRepetition(request.Agent, request.Id, request.CarUsedType, request.ActuarialCalibre))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ErrorCanShowCustomer, "费率重复");
            }

            if (request.Id > 0)
            {
                // 编辑，将setting和item都设置为已删除，然后重新添加
                if (!await DeleteRatePolicyAsync(request.Id))
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
            }

            var data = new bx_ratepolicy_setting
            {
                actuarial_calibre = string.IsNullOrEmpty(request.ActuarialCalibre)?"":request.ActuarialCalibre.ToUpper(),
                biz_rate = request.BizRate,
                force_rate = request.ForceRate,
                car_used_type = request.CarUsedType,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                is_delete = 0,
                top_agent_id = request.Agent,
                //2018-10-10 张克亮 加入商业险或交强险费率超过此值的部分，自动转化为积分
                over_transfer_credits = request.OverTransferCredits
            };
            _ratePolicySettingRepository.Insert(data);
            await _ratePolicySettingRepository.SaveChangesAsync();
            var rateItem = GenerateRateItem(data);
            _ratepolicyItemRepository.Insert(rateItem);
            await _ratepolicyItemRepository.SaveChangesAsync();

            // 没有报异常就返回成功
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        /// <summary>
        /// 判断是否重复
        /// 重复：false  不重复：true
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="rateSettingId"></param>
        /// <param name="carUsedType"></param>
        /// <param name="actuarialCalibre"></param>
        /// <returns></returns>
        private async Task<bool> CheckRepetition(int topAgentId, int rateSettingId, string carUsedType, string actuarialCalibre)
        {
            // 根据TopAgentId获取所有的费率设置
            var dbRate = await _ratepolicyItemRepository.GetAll().Where(o => o.top_agent_id == topAgentId && o.is_delete == 0 && o.ratepolicy_setting_id != rateSettingId).ToListAsync();
            if (!dbRate.Any())
                return true;
            var dbList = dbRate.Select(o => o.car_used_type + o.actuarial_calibre).ToList();

            // 获取要添加的list
            var checkList = new List<string>();

            //2018-09-28 张克亮 精算品径为空时不检查
            if (!string.IsNullOrEmpty(carUsedType) && !string.IsNullOrEmpty(actuarialCalibre))
            {
                var arrUseType = carUsedType.Split(',').Select(o => Convert.ToSByte(o));
                var arrCalibre = actuarialCalibre.ToUpper().Split(',');

                foreach (var item in arrUseType)
                {
                    foreach (var calibre in arrCalibre)
                    {
                        checkList.Add(item + calibre);
                    }
                }
            }

            if (dbList.Intersect(checkList).Any())
                return false;
            return true;
        }

        /// <summary>
        /// 删除setting和item
        /// </summary>
        /// <param name="rateSettingId"></param>
        /// <returns></returns>
        private async Task<bool> DeleteRatePolicyAsync(int rateSettingId)
        {
            return await _ratePolicySettingRepository.DeleteRatePolicyAsync(rateSettingId);
        }

        /// <summary>
        /// 根据bx_ratepolicy_setting生成bx_ratepolicy_item
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<bx_ratepolicy_item> GenerateRateItem(bx_ratepolicy_setting data)
        {
            List<bx_ratepolicy_item> result = new List<bx_ratepolicy_item>();
            var arrUseType = data.car_used_type.Split(',').Select(o => Convert.ToSByte(o));
            var arrCalibre = data.actuarial_calibre.ToUpper().Split(',');
            var now = DateTime.Now;
            foreach (var item in arrUseType)
            {
                foreach (var calibre in arrCalibre)
                {
                    bx_ratepolicy_item rateItem = new bx_ratepolicy_item
                    {
                        actuarial_calibre = calibre,
                        car_used_type = item,
                        create_time = now,
                        is_delete = 0,
                        ratepolicy_setting_id = data.id,
                        top_agent_id = data.top_agent_id
                    };
                    result.Add(rateItem);
                }
            }
            return result;
        }

        public async Task<BaseViewModel> DeleteRateAsync(DeleteRateRequest request)
        {
            var data = await _ratePolicySettingRepository.FirstOrDefaultAsync(o => o.id == request.Id);
            if (data == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);

            if (data.top_agent_id != request.Agent)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "操作不合法，删除的数据不属于当前顶级代理人");
            }

            await DeleteRatePolicyAsync(data.id);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        public async Task<BaseViewModel> GetOverTransferCreditsAsync(BaseRequest2 request)
        {
            // 先查找，没有的话初始化一个默认值：27
            var number = 0.0;
            var info = await GetOverTransferCreditsAsync(request.Agent);
            if (info != null)
                number = info.over_transfer_credits;
            else
            {
                // 初始化一个，并返回
                var ratePolicy = GenerateRatePolicySetting(request.Agent, 27);
                _ratePolicySettingRepository.Insert(ratePolicy);
                await _ratePolicySettingRepository.SaveChangesAsync();
                number = 27.0;
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, new { Number = number });
        }

        public async Task<BaseViewModel> GetRateListAsync(BaseRequest2 request)
        {
            var list = await _ratePolicySettingRepository.GetAll().Where(o => o.top_agent_id == request.Agent && o.is_delete == 0).OrderByDescending(o => o.id).ToListAsync();
            List<RatePolicySettingDto> result = new List<RatePolicySettingDto>();
            foreach (var item in list)
            {
                var model = new RatePolicySettingDto
                {
                    Id = item.id,
                    ActuarialCalibre = item.actuarial_calibre,
                    BizRate = item.biz_rate,
                    CarUsedType = item.car_used_type,
                    ForceRate = item.force_rate
                };
                result.Add(model);
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, result);
        }

        public async Task<BaseViewModel> SetOverTransferCreditsAsync(SetOverTransferCreditsRequest request)
        {
            var rate = GetListOverTransferCredits(request.Agent, request.Number);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
        }

        /// <summary>
        /// 初始化一个设置制动转换积分的阈值
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="overTransferCredits"></param>
        /// <returns></returns>
        private bx_ratepolicy_setting GenerateRatePolicySetting(int topAgentId, double overTransferCredits)
        {
            return new bx_ratepolicy_setting
            {
                actuarial_calibre = "-1",
                biz_rate = 0,
                car_used_type = "-1",
                create_time = DateTime.Now,
                force_rate = 0,
                is_delete = 1,
                over_transfer_credits = overTransferCredits,
                top_agent_id = topAgentId,
                update_time = DateTime.Now
            };

        }

        /// <summary>
        /// 获取自动转换积分的阈值
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        private async Task<bx_ratepolicy_setting> GetOverTransferCreditsAsync(int topAgentId)
        {
            return await _ratePolicySettingRepository.FirstOrDefaultAsync(o => o.top_agent_id == topAgentId && o.car_used_type == "-1" && o.actuarial_calibre == "-1" && o.biz_rate == 0 && o.force_rate == 0 && o.is_delete == 1);
        }

        public int GetListOverTransferCredits(int topAgentId, double num)
        {
            return context.Database.SqlQuery<int>("select 1;Update bx_ratepolicy_setting set over_transfer_credits = " + num + " where top_agent_id = " + topAgentId + ";").FirstOrDefault(); ;
        }
    }
}
