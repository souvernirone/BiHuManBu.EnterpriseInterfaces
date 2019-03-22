using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.BindBusiuser
{
    public class CheckCanBandBusiuserListService : ICheckCanBindBusiuserListService
    {
        private IAgentRepository _agentRepository;
        private IOrderRepository _orderRepository;
        /// <summary>
        /// 采集系统的账号密码salt
        /// </summary>
        private const string UQ = "(*HY53U&3#+-34MF&`\t\"&gt;";

        public CheckCanBandBusiuserListService(IAgentRepository agentRepository, IOrderRepository orderRepository)
        {
            _agentRepository = agentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Tuple<BaseViewModel, int>> CheckRequestAsync(CanBandBusiuserRequest request)
        {
            // 判断是否是顶级代理人
            var agent = _agentRepository.GetAgent(request.Agent);
            if (agent == null || agent.IsDaiLi != 1)
            {
                return Tuple.Create(BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "不是顶级代理人"), -1);
            }
            var md5Pwd = CommonHelper.GetMd5(UQ + request.Password);
            var busiuser = await _orderRepository.GetBusiuserByUserNamePwdAsync(request.UserName, md5Pwd);
            if (busiuser == null)
            {
                return Tuple.Create(BaseViewModel.GetBaseViewModel(BusinessStatusType.GetFailed, "账号或密码错误！"), -1);
            }
            return Tuple.Create(BaseViewModel.GetBaseViewModel(BusinessStatusType.OK), busiuser.Id);
        }
    }
}
