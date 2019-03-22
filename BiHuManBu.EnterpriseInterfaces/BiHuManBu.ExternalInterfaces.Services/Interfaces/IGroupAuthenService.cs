using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IGroupAuthenService
    {
        /// <summary>
        /// 根据id获取实体对象 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        GroupAuthenModel GetModel(int id);

        /// <summary>
        /// 更新实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateModel(UpdateGroupAuthenRequest request);

        /// <summary>
        /// 添加实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel AddModel(UpdateGroupAuthenRequest request);

        BaseViewModel ZhenBangUpdate(int TopAgentId, int AgentId, int IsUsed, int IsSubmit, int IsShowCalc, int MessagePayType, int RoleId, int ZhenBangType, string UpdateFrom,int ParentShareCode);

        /// <summary>
        /// 查询所有的银行 zky 2018-1-2
        /// </summary>
        /// <returns></returns>
        IList<BankModel> GetAllBank();

        /// <summary>
        /// 更新机构账号信息 zky 2018-1-2 /crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateOrgInfo(SaveOrgInfoRequest request);

        /// <summary>
        /// 工号管理列表 zky 2018-1-2/crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="ukeyName"></param>
        /// <param name="cityName"></param>
        /// <param name="orgName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IList<JobNumberDto> JobNumberList(int agentId, string ukeyName, string cityName, string orgName, int pageIndex, int pageSize, int cityId,int ukeyId,int groupId,out int total);

        JobNumberListViewModel CityInfoAndUkeyInfo(int agentId);

        /// <summary>
        /// 认证审核不通过 zky 2018-2-4/crm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel AuditFailed(AuditFailedRequest request);

        /// <summary>
        /// 2108-09-19 张克亮 小V盟项目时加入 根据经济人查询头像及证件信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        BaseViewModel GetGroupAuthen(int agentId);
    }
}
