using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IUserinfoExpandService
    {
        /// <summary>
        /// 根据拼接的SQL脚本批量添加到表中
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddRangeBySql(List<bx_userinfo_expand> list);
        /// <summary>
        /// 根据buid更新扩展表
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="DeleteType"></param>
        /// <param name="DeleteTime"></param>
        /// <returns></returns>
        bool UpdateUserExpandByBuid(string buids, int DeleteType, DateTime DeleteTime);
        bool UpdateUserExpandByBuid(List<long> buids, int DeleteType, DateTime DeleteTime);
        /// <summary>
        /// 根据buid获得扩展表中存在的buid集合
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<long> GetExistBuidList(string buids);
    }
}
