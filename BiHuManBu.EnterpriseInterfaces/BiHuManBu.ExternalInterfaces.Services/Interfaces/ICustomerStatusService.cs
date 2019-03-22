using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICustomerStatusService
    {
        /// <summary>
        /// 查询客户状态信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<CustomerStatusModel> GetCustomerStatus(int agentId, int t_Id, bool isDeleteData, bool isGetReView);
        /// <summary>
        /// 保存客户状态信息
        /// </summary>
        /// <param name="customerStatusModel"></param>
        /// <returns></returns>
        int SaveCustomerStatus(CustomerStatusModel customerStatusModel);
      
        /// <summary>
        /// 修改客户状态
        /// </summary>
        /// <param name="updateCustomerStatusModel"></param>
        /// <returns></returns>
        bool UpdateCustomerStatus(UpdateCustomerStatusModel updateCustomerStatusModel);


        /// <summary>
        /// 批量修改客户状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool BatchUpdateCustomerStatusAndCategories(BatchUpdateCustomerStatusAndCategoriesModel model);
        

        /// <summary>
        /// 删除客户状态
        /// </summary>
        /// <param name="rmoveCustomerStatusModel"></param>
        /// <returns></returns>
        bool DeleteCustomerStatus(RemoveCustomerStatusModel rmoveCustomerStatusModel);

        string GetCustomerStatusInfo(int agentId, int t_Id);

        bool MakeCustomerStatus();

        bool NewDailiStatus(int agentid);
    }
}
