using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICustomerCategories
    {
        /// <summary>
        /// 查询客户类别信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<CustomerCategoriesModel> GetCustomerCategories(int agentId);

        /// <summary>
        /// 刷库
        /// </summary>
        /// <returns></returns>
        bool MakeCustomerCategories();

        ///// <summary>
        ///// 保存客户类别信息
        ///// </summary>
        ///// <param name="customerCategoriesModel"></param>
        ///// <returns></returns>
        //bool SaveCustomerCategories(List<CustomerCategoriesModel> customerCategoriesModel);

        /// <summary>
        /// 保存客户类别信息
        /// </summary>
        /// <param name="customerCategoriesModel"></param>
        /// <returns></returns>
        int SaveCustomerCategories(CustomerCategoriesModel customerCategoriesModel);
        /// <summary>
        /// 设置客户类别
        /// </summary>
        /// <param name="updateCustomerCategoriesModel"></param>
        /// <returns></returns>
        bool SetCustomerCategories(int agentId);
        /// <summary>
        /// 修改客户类别
        /// </summary>
        /// <param name="updateCustomerCategoriesModel"></param>
        /// <returns></returns>
        bool UpdateCustomerCategories(UpdateCustomerCategoriesModel updateCustomerCategoriesModel);


        /// <summary>
        /// 批量修改客户类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
         bool BatchUpdateCustomerCategories(BatchUpdateCustomerCategoriesModel model);
        /// <summary>
        /// 删除客户类别
        /// </summary>
        /// <param name="removeModel"></param>
        /// <returns></returns>
        bool DeleteCustomerCategories(RemoveModel removeModel);
        /// <summary>
        /// 根据获取流转后相应类型的Id
        /// </summary>
        /// <param name="categoryId">本身的类别编号</param>
        /// <param name="categoryType">0->流转已出单编号，1->流转战败编号</param>
        /// <returns></returns>
        int GetCirculationCategoryId(int categoryId, int categoryType);

        /// <summary>
        /// 获取所有的客户类型
        /// </summary>
        /// <returns></returns>
        List<bx_customercategories> GetCategoriesList(int agentId);

    
    }
}
