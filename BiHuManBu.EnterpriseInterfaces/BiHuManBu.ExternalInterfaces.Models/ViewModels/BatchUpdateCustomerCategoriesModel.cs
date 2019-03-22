using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
  public   class BatchUpdateCustomerCategoriesModel
    {

        /// <summary>
        /// 用户ID列表
        /// </summary>
        public List<long> UserIdList{get;set;}

    

        public UpdateCustomerCategoriesModel updateCustomerCategoriesModel;



        public bx_customercategories ConvertEFModel()
        { 
         bx_customercategories  bx_cust=new bx_customercategories();

         bx_cust.CategoryInfo = updateCustomerCategoriesModel.CategoryInfo;
         bx_cust.DefeatTrans = updateCustomerCategoriesModel.DefeatTrans;
         bx_cust.IssuingTrans = updateCustomerCategoriesModel.IssuingTrans;
         return bx_cust;
        }
    }








}
