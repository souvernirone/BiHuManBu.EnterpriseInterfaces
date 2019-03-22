using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ITempInsuredRepository
    {
        /// <summary>
        /// 获取临时被保险人信息
        /// </summary>
        /// <param name="agentId">bx_agent.id</param>
        ///<param name="buId">bx_userinfo.buid</param>
        /// <returns></returns>
        Task<bx_tempinsuredinfo> GetTempInsuredInfoAsync(int agentId,long buId);
        /// <summary>
        /// 保存临时被保险信息
        /// </summary>
        /// <param name="tempInsuredInfo">被保险人信息</param>
        /// <param name="isEdit">是否为编辑</param>
        /// <returns></returns>
        Task<bool> SaveTempInsuredInfoAsync(bx_tempinsuredinfo tempInsuredInfo, bool isEdit);

        /// <summary>
        /// 报价用来存放临时邮箱电话-新增
        /// </summary>
        /// <param name="userinfoExpand"></param>
        /// <returns></returns>
        bool AddUserExpand(bx_userinfo_expand userinfoExpand);

        /// <summary>
        /// 报价用来存放临时邮箱电话-修改
        /// </summary>
        /// <param name="userinfoExpand"></param>
        /// <returns></returns>
        bool UpdateUserExpand(bx_userinfo_expand userinfoExpand);

        /// <summary>
        /// 通过buid获取最新的临时信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        bx_userinfo_expand GetUserExpand(long buid);

    }
}
