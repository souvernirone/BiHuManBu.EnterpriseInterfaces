using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class UpdateGroupLevelService: IUpdateGroupLevelService
    {
        private readonly IGroupAuthenRepository groupAuthenRepository;

        public UpdateGroupLevelService(IGroupAuthenRepository groupAuthenRepository)
        {
            this.groupAuthenRepository = groupAuthenRepository;
        }
        /// <summary>
        /// 更新团队等级
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateGroupLevel(List<TeamLevelViewModel> list)
        {
            var rusult = false;
            using (TransactionScope ts = new TransactionScope())
            {
                var count=groupAuthenRepository.UpdateGroupLevel(list);
                if (count==1){
                    ts.Complete();
                    rusult = true;
                }
            }
            return rusult;
        }
    }
}
