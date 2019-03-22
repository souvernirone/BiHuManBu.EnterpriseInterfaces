using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ICustomerStatusRepository
    {
        /// <summary>
        /// 保存客户状态
        /// </summary>
        /// <param name="CustomerStatus"></param>
        /// <returns></returns>
        int SaveCustomerStatus(bx_customerstatus CustomerStatus);
        /// <summary>
        /// 获取客户状态
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        List<bx_customerstatus> GetCustomerStatus(int agentid, int t_Id, bool isDeleteData, bool isGetReView);

        /// <summary>
        /// 修改删除
        /// </summary>
        /// <param name="CustomerStatus"></param>
        /// <returns></returns>
        bool UpdateCustomerStatus(bx_customerstatus CustomerStatus);



        bool BatchUpdateCustomerStatusAndCategories(List<long> list, int status, int category);
        /// <summary>
        /// 删除客户状态
        /// </summary>
        /// <param name="CustomerStatus"></param>
        /// <param name="removeWay"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        bool DeleteCustomerStatus(string strBuids, bx_customerstatus CustomerStatus, int removeWay, int moveTo);
        string GetCustomerStatusInfo(int agentId, int t_Id);

        bool MakeCustomerStatus();


        bool NewDailiStatus(int agentid);  
    }
}
