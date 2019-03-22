using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IManagerModuleRepository
    {
        List<manager_module_db> GetManagerModuleAll();
        List<string> GetManagerModuleCodes(string paterCode, int type);

        /// <summary>
        /// 查询所有的菜单 zky 2017-08-04
        /// </summary>
        /// <returns></returns>
        IList<manager_module_db> GetAllModule();

        IQueryable<manager_module_db> GetList(Expression<Func<manager_module_db, bool>> whereLamda);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Add(manager_module_db entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(manager_module_db entity);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        bool Delete(string moduleCode);






    }
}
