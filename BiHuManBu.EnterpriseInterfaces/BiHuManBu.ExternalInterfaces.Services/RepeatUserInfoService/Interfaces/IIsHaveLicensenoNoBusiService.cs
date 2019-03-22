using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces
{
    public interface IIsHaveLicensenoNoBusiService
    {
        bx_userinfo IsHaveLicenseno(int agent, int childagent, string licenseNo, string vinNo, string engineNo, int type);
    }
}
