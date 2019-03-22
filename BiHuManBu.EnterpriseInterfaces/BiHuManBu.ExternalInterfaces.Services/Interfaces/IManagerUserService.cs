using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public  interface IManagerUserService
    {
        /// <summary>
        /// 更新经纪人平台账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateManagerUserAccount(UpdateManagerUserAccountRequest request);

        /// <summary>
        /// 用户列表查询 zky 2017-08-31 /运营后台
        /// </summary>
        /// <param name="accountType">0运营后台用户，1CRM用户</param>
        /// <param name="mobile">手机号</param>
        /// <param name="account">用户名</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IList<ManageruserViewModel> GetManageruserList(int accountType, string mobile, string account, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 查询菜单列表
        /// </summary>
        /// <param name="platform">0运营后台菜单 1CRM菜单</param>
        /// <returns></returns>
        List<ManagerModuleViewModel> GetModuleList(int platform);
    }
}
