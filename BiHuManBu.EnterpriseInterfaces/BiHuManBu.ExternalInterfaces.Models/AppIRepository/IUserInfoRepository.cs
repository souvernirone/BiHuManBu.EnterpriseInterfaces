using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_userinfo;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IUserInfoRepository
    {
        bx_userinfo FindByBuid(long buid);
        bx_userinfo FindByOpenIdAndLicense(string openid, string licenseno, string agent, int cartype = 0, int isTest = 0);
        long Add(bx_userinfo userinfo);
        int Update(bx_userinfo userinfo);
    }
}
