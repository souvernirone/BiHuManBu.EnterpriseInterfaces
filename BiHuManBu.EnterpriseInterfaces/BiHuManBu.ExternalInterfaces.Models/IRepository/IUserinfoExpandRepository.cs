using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IUserinfoExpandRepository
    {
        /// <summary>
        /// 根据拼接的SQL脚本批量添加到表中
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddRangeBySql(List<bx_userinfo_expand> list);
        /// <summary>
        /// 根据buid批量更新扩展表
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="DeleteType"></param>
        /// <param name="DeleteTime"></param>
        /// <returns></returns>
        bool UpdateUserExpandByBuid(string buids, int DeleteType, DateTime DeleteTime);
        bool UpdateUserExpandByBuid(List<long> buids, int DeleteType, DateTime DeleteTime);
        /// <summary>
        /// 根据buid查询扩展表中存在的buid
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<long> GetExistBuidList(string buids);
        Task<int> Update(bx_userinfo_expand entity);
        Task<bx_userinfo_expand> GetAsync(long buid);
        bx_userinfo_expand Get(long buid);
    }
}
