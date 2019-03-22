using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IGroupAuthenRepository
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
        /// <param name="model"></param>
        /// <param name="save"></param>
        /// <returns></returns>
        bool UpdateModel(bx_group_authen model, bool save = true);

        /// <summary>
        /// 添加实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="model"></param>
        /// <param name="save"></param>
        /// <returns></returns>
        bool AddModel(bx_group_authen model, bool save = true);

        /// <summary>
        /// 执行保存 zky 2017-11-14 /crm
        /// </summary>
        /// <returns></returns>
        int SaveChange();

        /// <summary>
        /// 根据条件查询列表 zky 2017-11-14
        /// </summary>
        /// <param name="whereLamda"></param>
        /// <returns></returns>
        IQueryable<bx_group_authen> GetList(Expression<Func<bx_group_authen, bool>> whereLamda);

        /// <summary>
        /// 根据id查询 zky 2017-11-14
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bx_group_authen Get(int id);
        /// <summary>
        /// 根据ahentId查询 sjy 2018-2-4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bx_group_authen GetByAgentId(int id);

        /// <summary>
        /// 查询所有的银行 zky 2018-1-2/crm
        /// </summary>
        /// <returns></returns>
        IList<bx_bank> GetAllBank();

        /// <summary>
        /// 工号管理列表 zky 2018-1-2/crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="ukeyName"></param>
        /// <param name="cityName"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        IList<JobNumberDto> JobNumberList(int agentId, string ukeyName, string cityName, string orgName, int pageIndex, int pageSize, int cityId,int ukeyId,int groupId,out int total);

        IList<JobNumberDto> CityInfoAndUkeyInfo(int agentId);
        /// <summary>
        /// 更新团队等级
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int UpdateGroupLevel(List<TeamLevelViewModel> list);

        /// <summary>
        /// 2018-09-25 张克亮
        /// 根据身份证号或银行卡号查询实体模型
        /// </summary>
        /// <param name="idcard">身份证号</param>
        /// <param name="bankNumber">银行卡号</param>
        /// <returns></returns>
        BaseViewModel GetModel(string idcard = "", string bankCardNumber = "");
        /// <summary>
        /// 获得团队等级信息
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        bx_zc_team_incoming_setting GetTeamIncomingSetByLevelId(int levelId);
    }
}
