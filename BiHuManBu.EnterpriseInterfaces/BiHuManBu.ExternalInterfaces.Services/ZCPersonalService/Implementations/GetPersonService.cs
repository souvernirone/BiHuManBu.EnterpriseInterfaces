using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class GetPersonService : IGetPersonService
    {
        private readonly IZCPersonalRepository _zcPersonalRepository;

        public GetPersonService(IZCPersonalRepository zcPersonalRepository)
        {
            _zcPersonalRepository = zcPersonalRepository;
        }
        /// <summary>
        /// 获取用户信息 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetPerson(int agentId)
        {
            var personInfo = _zcPersonalRepository.GetPerson(agentId);
            if (personInfo != null)
            {
                return BaseViewModel.GetBaseViewModel(1, "查询成功", personInfo);
            }
            return BaseViewModel.GetBaseViewModel(0, "未获取到代理人信息");
        }
    }
}
