using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class UserinfoExpandService : IUserinfoExpandService
    {
        private readonly IUserinfoExpandRepository _userinfoExpandRepository;
        public UserinfoExpandService(IUserinfoExpandRepository userInfoExpandRepository)
        {
            _userinfoExpandRepository = userInfoExpandRepository;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddRangeBySql(List<bx_userinfo_expand> list)
        {
            return _userinfoExpandRepository.AddRangeBySql(list);
        }

        /// <summary>
        /// 批量恢复扩展表
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="DeleteType"></param>
        /// <param name="DeleteTime"></param>
        /// <returns></returns>
        public bool UpdateUserExpandByBuid(string buids, int DeleteType, DateTime DeleteTime)
        {
            return _userinfoExpandRepository.UpdateUserExpandByBuid(buids, DeleteType, DeleteTime);
        }
        public bool UpdateUserExpandByBuid(List<long> buids, int DeleteType, DateTime DeleteTime)
        {
            return _userinfoExpandRepository.UpdateUserExpandByBuid(buids, DeleteType, DeleteTime);
        }
        /// <summary>
        /// 根据buid获得扩展表中存在的buid集合
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<long> GetExistBuidList(string buids)
        {
            return _userinfoExpandRepository.GetExistBuidList(buids);
        }
    }
}
