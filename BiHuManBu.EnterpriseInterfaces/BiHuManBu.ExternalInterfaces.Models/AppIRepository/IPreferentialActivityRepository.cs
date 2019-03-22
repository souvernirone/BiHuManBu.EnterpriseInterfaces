using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface IPreferentialActivityRepository
    {
        List<bx_preferential_activity> GetActivityByBuid(long buid);

        List<bx_preferential_activity> GetActivityByIds(string stringId);

        /// <summary>
        /// 通过ID集合字符串查询  李金友 20170906 /PC
        /// </summary>
        /// <param name="stringId"></param>
        /// <returns></returns>
        List<ResponesActivity> GetActivityByActivityIds(string stringId);



        /// <summary>
        /// 新增优惠活动  李金友 20170906 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bx_preferential_activity AddActivity(bx_preferential_activity request);


        /// <summary>
        /// 新增编辑优惠活动集合  李金友 20170906 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<ResponesActivity> AddOrEditActivity(RequestActivityViewModel request);

        /// <summary>
        /// 查询分页，通过ActivityType活动类别  李金友 20170906 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<ResponesActivity> GetActivityPageList(GetActivityPageListRequest request);

       /// <summary>
        /// 通过活动类别查询活动列表  李金友 20170906 /PC
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        List<ResponesActivity> GetActivityByType(BaseVerifyRequest request);

        /// <summary>
        /// 通过当前代理人ID查询上次记录优惠活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponesActivity GetActivityByAgentId(BaseVerifyRequest request);
        
        /// <summary>
        /// 删除/禁用优惠活动  李金友 20170906 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel DelActivity(DelPreferentialActivityListRequest request);
    }
}
