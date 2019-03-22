using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 客户状态
    /// </summary>
    public class CustomerStatusService : ICustomerStatusService
    {
        readonly ICustomerStatusRepository _customerStatusRepository;
        readonly IAgentRepository _agentRepository;
        private readonly IAgentService _agentService;
        //ICustomerTopLevelService _customerTopLevelService;
        //IConsumerDetailService _consumerDetailService;
        //IUserInfoRepository _userInfoRepository;
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="CustomerStatusRepository"></param>
        /// <param name="agentRepository"></param>
        /// <param name="agentService"></param>
        public CustomerStatusService(ICustomerStatusRepository customerStatusRepository, IAgentRepository agentRepository, IAgentService agentService)
        {
            this._customerStatusRepository = customerStatusRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
            //_consumerDetailService = consumerDetailService;
            //_customerTopLevelService = customerTopLevelService;
            //_userInfoRepository = userInfoRepository;
        }

        /// <summary>
        /// 保存客户状态
        /// </summary>
        /// <param name="customerStatusModel"></param>
        /// <returns></returns>
        public int SaveCustomerStatus(CustomerStatusModel customerStatusModel)
        {
            //定义
            //   List<bx_CustomerStatus> _lscc = new List<bx_CustomerStatus>();
            //判断
            if (customerStatusModel != null)
            {

                var customerStatusInfo = new bx_customerstatus() { CreateTime = DateTime.Now };
                customerStatusInfo.AgentId = customerStatusModel.Agent;
                customerStatusInfo.Deleted = customerStatusModel.Deleted;
                customerStatusInfo.StatusInfo = customerStatusModel.StatusInfo;
                customerStatusInfo.Id = customerStatusModel.Id;
                customerStatusInfo.IsSystemData = customerStatusModel.IsSystemData;
                return _customerStatusRepository.SaveCustomerStatus(customerStatusInfo);
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 查询客户状态
        /// </summary>
        /// <param name="customerStatusModel"></param>
        /// <returns></returns>
        public List<CustomerStatusModel> GetCustomerStatus(int agentid, int t_Id, bool isDeleteData, bool isGetReView)
        {
            var customerStatusModels = new List<CustomerStatusModel>();

            var customerCasInfo = _customerStatusRepository.GetCustomerStatus(agentid, t_Id, isDeleteData, isGetReView);
            if (customerCasInfo.Any())
            {
                foreach (var item in customerCasInfo)
                {
                    CustomerStatusModel customerStatusModel = new CustomerStatusModel();
                    customerStatusModel.AgentId = item.AgentId;
                    customerStatusModel.StatusInfo = item.StatusInfo;
                    customerStatusModel.Deleted = item.Deleted;
                    customerStatusModel.Id = item.Id;
                    customerStatusModel.T_Id = item.T_Id;
                    customerStatusModel.IsSystemData = item.IsSystemData;
                    customerStatusModels.Add(customerStatusModel);
                }
            }
            return customerStatusModels;
        }

        /// <summary>
        /// 修改客户状态
        /// </summary>
        /// <param name="customerStatusModel"></param>
        /// <returns></returns>
        public bool UpdateCustomerStatus(UpdateCustomerStatusModel updateCustomerStatusModel)
        {
            bool isupdate = false;
            if (updateCustomerStatusModel != null)
            {
                bx_customerstatus _CustomerStatus = new bx_customerstatus();
                _CustomerStatus.StatusInfo = updateCustomerStatusModel.StatusInfo;
                _CustomerStatus.Id = updateCustomerStatusModel.Id;
                isupdate = _customerStatusRepository.UpdateCustomerStatus(_CustomerStatus);
            }
            return isupdate;
        }

        public bool BatchUpdateCustomerStatusAndCategories(BatchUpdateCustomerStatusAndCategoriesModel model)
        {



            //if (model.IsAll)
            //{
            //    var search = _customerTopLevelService.GetWhereByRequest2(model);
            //    // 这里写死要关联bx_consumer_review表，因为要查几个回访字段 
            //    search.JoinType = 1;
            //    var list = _userInfoRepository.FindCustomerListJoinConsumerReview(search);
            //    list.ForEach(l => model.UserIdList.Add(l.Id));

            //}

            return _customerStatusRepository.BatchUpdateCustomerStatusAndCategories(model.UserIdList, model.Status, model.CategoryInfo);



        }


        public string GetCustomerStatusInfo(int agentId, int t_Id)
        {
            var customerStatusInfo = _customerStatusRepository.GetCustomerStatusInfo(agentId, t_Id);
            return customerStatusInfo;
        }

        public bool MakeCustomerStatus()
        {
            var isSucessData = _customerStatusRepository.MakeCustomerStatus();
            return isSucessData;
        }
        //DeleteCustomerStatus

        /// <summary>
        /// 删除客户状态
        /// </summary>
        /// <param name="removeCustomerStatusModel"></param>
        /// <returns></returns>
        public bool DeleteCustomerStatus(RemoveCustomerStatusModel removeCustomerStatusModel)
        {
            bool isupdate = false;
            if (removeCustomerStatusModel != null)
            {
                bx_customerstatus _CustomerStatus = new bx_customerstatus();
                _CustomerStatus.Deleted = true;
                _CustomerStatus.Id = removeCustomerStatusModel.Id;
                _CustomerStatus.AgentId = removeCustomerStatusModel.Agent;
                _CustomerStatus.T_Id = removeCustomerStatusModel.T_Id;
                List<string> sonIds = _agentService.GetSonsListFromRedisToString(removeCustomerStatusModel.Agent);
                //_agentRepository.GetSonsList(removeCustomerStatusModel.agentId);
                var sbAgent = new StringBuilder();
                //获取子集代理
                //var sonIds = GetSonsList(curAgent, isContainSelf);
                if (sonIds.Any())
                {
                    foreach (var son in sonIds)
                    {//拼接子集代理串
                        sbAgent.Append("'").Append(son).Append("'").Append(',');
                    }
                    sbAgent.Remove(sbAgent.Length - 1, 1);
                }
                sbAgent.ToString();
                isupdate = _customerStatusRepository.DeleteCustomerStatus(sbAgent.ToString(), _CustomerStatus, removeCustomerStatusModel.removeWay, removeCustomerStatusModel.moveTo);
            }
            return isupdate;
        }

        public bool NewDailiStatus(int agentid)
        {
            var isSucessData = _customerStatusRepository.NewDailiStatus(agentid);
            return isSucessData;
        }

    }
}
