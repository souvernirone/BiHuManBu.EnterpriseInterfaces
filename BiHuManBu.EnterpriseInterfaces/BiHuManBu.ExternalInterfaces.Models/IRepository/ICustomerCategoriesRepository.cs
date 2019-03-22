using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ICustomerCategoriesRepository
    {
        ///// <summary>
        ///// 保存客户类别
        ///// </summary>
        ///// <param name="customerCategories"></param>
        ///// <returns></returns>
        //bool SaveCustomerCategories(List<bx_customercategories> customerCategories);

        /// <summary>
        /// 保存客户类别
        /// </summary>
        /// <param name="customerCategories"></param>
        /// <returns></returns>
        int SaveCustomerCategories(bx_customercategories customerCategories);

        /// <summary>
        /// 刷库客户类别
        /// </summary>
        /// <returns></returns>
        bool MakeCustomerCategories();

        /// <summary>
        /// 获取客户类别
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        List<bx_customercategories> GetCustomerCategories(int agentid);

        /// <summary>
        /// 修改删除
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        bool UpdateCustomerCategories(bx_customercategories customercategories);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        bool BatchUpdateCustomerCategories(List<long> list,bx_customercategories customercategories);
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool SetCustomerCategories(int agentId);

        /// <summary>
        /// 删除客户类别
        /// </summary>
        /// <param name="strBuids"></param>
        /// <param name="customercategories"></param>
        /// <param name="removeWay"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        bool DeleteCustomerCategories(string strBuids, bx_customercategories customercategories, int removeWay, int moveTo);
        /// <summary>
        /// 根据获取流转后相应类型的Id
        /// </summary>
        /// <param name="categoryId"></param>

        /// <returns></returns>
        bx_customercategories GetCirculationCategoryRecord(int categoryId);

        /// <summary>
        /// 获取所有的客户类型
        /// </summary>
        /// <returns></returns>
        List<bx_customercategories> GetCategoriesList(int agentId);
    }
}
