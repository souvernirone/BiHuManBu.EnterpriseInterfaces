using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 客户类别
    /// </summary>
    public class CustomerCategoriesService : ICustomerCategories
    {
        readonly ICustomerCategoriesRepository _customerCategoriesRepository;
        readonly IAgentRepository _agentRepository;
        private readonly IAgentService _agentService;


        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="customerCategoriesRepository"></param>
        /// <param name="agentRepository"></param>
        /// <param name="agentService"></param>
        public CustomerCategoriesService(ICustomerCategoriesRepository customerCategoriesRepository, IAgentRepository agentRepository, IAgentService agentService)
        {
            this._customerCategoriesRepository = customerCategoriesRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
        }
        ///// <summary>
        ///// 保存客户类别
        ///// </summary>
        ///// <param name="customercategoriesModel"></param>
        ///// <returns></returns>
        //public bool SaveCustomerCategories(List<CustomerCategoriesModel> customercategoriesModel)
        //{
        //    //定义
        //    List<bx_customercategories> _lscc = new List<bx_customercategories>();
        //    //判断
        //    if (customercategoriesModel.Any())
        //    {

        //        foreach (var item in customercategoriesModel)
        //        {
        //            var customerCaInfo = new bx_customercategories() { CreateTime = DateTime.Now };
        //            customerCaInfo.AgentId = item.AgentId;
        //            customerCaInfo.IssuingTrans = item.IssuingTrans;
        //            customerCaInfo.IsStart = item.IsStart;
        //            customerCaInfo.Deleted = item.Deleted;
        //            customerCaInfo.DefeatTrans = item.DefeatTrans;
        //            customerCaInfo.CategoryInfo = item.CategoryInfo;
        //            customerCaInfo.Id = item.Id;
        //            _lscc.Add(customerCaInfo);
        //        }
        //    }
        //    return _customerCategoriesRepository.SaveCustomerCategories(_lscc);
        //}

        /// <summary>
        /// 保存客户类别
        /// </summary>
        /// <returns></returns>
        public bool MakeCustomerCategories()
        {
           //判断
            return _customerCategoriesRepository.MakeCustomerCategories();

        }  

        /// <summary>
        /// 保存客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public int SaveCustomerCategories(CustomerCategoriesModel customercategoriesModel)
        {
            //定义
            //   List<bx_customercategories> _lscc = new List<bx_customercategories>();
            //判断
            if (customercategoriesModel != null)
            {

                var customerCaInfo = new bx_customercategories() { CreateTime = DateTime.Now };
                customerCaInfo.AgentId = customercategoriesModel.Agent;//新增客户类别都放在顶级下面
                customerCaInfo.IssuingTrans = customercategoriesModel.IssuingTrans;
                customerCaInfo.IsStart = customercategoriesModel.IsStart;
                customerCaInfo.Deleted = customercategoriesModel.Deleted;
                customerCaInfo.DefeatTrans = customercategoriesModel.DefeatTrans;
                customerCaInfo.CategoryInfo = customercategoriesModel.CategoryInfo;
                customerCaInfo.Id = customercategoriesModel.Id;
                return _customerCategoriesRepository.SaveCustomerCategories(customerCaInfo);
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 查询客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public List<CustomerCategoriesModel> GetCustomerCategories(int agentid)
        {
            var customerCategoriesModels = new List<CustomerCategoriesModel>();

            var customerCasInfo = _customerCategoriesRepository.GetCustomerCategories(agentid);
            if (customerCasInfo.Any())
            {
                foreach (var item in customerCasInfo)
                {
                    CustomerCategoriesModel customercategoriesModel = new CustomerCategoriesModel();
                    customercategoriesModel.Agent = item.AgentId;
                    customercategoriesModel.CategoryInfo = item.CategoryInfo;
                    customercategoriesModel.Deleted = item.Deleted;
                    customercategoriesModel.IssuingTrans = item.IssuingTrans;
                    customercategoriesModel.DefeatTrans = item.DefeatTrans;
                    customercategoriesModel.IsStart = item.IsStart;
                    customercategoriesModel.Id = item.Id;
                    customerCategoriesModels.Add(customercategoriesModel);
                }
            }
            return customerCategoriesModels;
        }

        /// <summary>
        /// 设置客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public bool SetCustomerCategories(int agentId)
        {
            var customerCasInfo = _customerCategoriesRepository.SetCustomerCategories(agentId);
            return customerCasInfo;
        }

        /// <summary>
        /// 修改客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public bool UpdateCustomerCategories(UpdateCustomerCategoriesModel customercategories)
        {
            bool isupdate = false;
            if (customercategories != null)
            {
                bx_customercategories _customercategories = new bx_customercategories();
                _customercategories.CategoryInfo = customercategories.CategoryInfo;
                _customercategories.IssuingTrans = customercategories.IssuingTrans;
                _customercategories.DefeatTrans = customercategories.DefeatTrans;
                _customercategories.IsStart = customercategories.IsStart;
                _customercategories.Id = customercategories.Id;
                isupdate = _customerCategoriesRepository.UpdateCustomerCategories(_customercategories);
            }
            return isupdate;
        }



        /// <summary>
        /// 批量修改客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public bool BatchUpdateCustomerCategories(BatchUpdateCustomerCategoriesModel  model)
        {
           return  _customerCategoriesRepository.BatchUpdateCustomerCategories(model.UserIdList, model.ConvertEFModel());
        }





        //DeleteCustomerCategories

        /// <summary>
        /// 删除客户类别
        /// </summary>
        /// <param name="removeModel"></param>
        /// <returns></returns>
        public bool DeleteCustomerCategories(RemoveModel removeModel)
        {


            bool isupdate = false;
            if (removeModel != null)
            {
                bx_customercategories _customercategories = new bx_customercategories();
                _customercategories.Deleted = true;
                _customercategories.Id = removeModel.Id;
                _customercategories.AgentId = removeModel.agentId;
                List<string> sonIds = _agentService.GetSonsListFromRedisToString(removeModel.agentId);
                    //_agentRepository.GetSonsList(removeModel.agentId);



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


                 isupdate = _customerCategoriesRepository.DeleteCustomerCategories(sbAgent.ToString(), _customercategories, removeModel.removeWay, removeModel.moveTo);
            }
            return isupdate;
        }

        public int GetCirculationCategoryId(int categoryId, int categoryType)
        {
           int circulationCategoryId = 0;
           var circulationCategoryRecord= _customerCategoriesRepository.GetCirculationCategoryRecord(categoryId);
            if (circulationCategoryRecord == null)
            {
                circulationCategoryId= categoryId;
            } else {
                if (categoryType == 0)
                {
                    circulationCategoryId= circulationCategoryRecord.IssuingTrans==0? categoryId: circulationCategoryRecord.IssuingTrans;
                }
                else {
                    circulationCategoryId=circulationCategoryRecord.DefeatTrans==0? categoryId: circulationCategoryRecord.DefeatTrans;
                }
            }
            return circulationCategoryId;
        }

        /// <summary>
        /// 获取所有的客户类型
        /// </summary>
        /// <returns></returns>
        public List<bx_customercategories> GetCategoriesList(int agentId)
        {
            return _customerCategoriesRepository.GetCategoriesList(agentId);
        }
    }
}
