using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Repository;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper
{
    public static class LastDayReInfoTotalMapper
    {
        public static List<ReportReInfo> ConvertViewModel(this List<bx_userinfo> userinfos)
        {
            List<ReportReInfo> list = new List<ReportReInfo>();
            if (userinfos.Any())
            {
                ReportReInfo ui = new ReportReInfo();
                foreach (bx_userinfo rf in userinfos)
                {
                    ui = new ReportReInfo();
                    ui = ConvertToViewModel(rf);
                    list.Add(ui);
                }
            }
            return list;
        }

        public static ReportReInfo ConvertToViewModel(this bx_userinfo userinfo)
        {
            IAgentRepository _agentRepository = new AgentRepository();
            return new ReportReInfo
            {
                LicenseNo = userinfo.LicenseNo,
                AdvAgentName = _agentRepository.GetAgent(int.Parse(userinfo.Agent)).AgentName
            };
        }
    }
}
