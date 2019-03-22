using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class BatchUpdateCustomerStatusAndCategoriesModel : BaseCustomerSearchRequest
    {
     
        /// <summary>
        /// 用户ID列表
        /// </summary>
   
        public List<long> UserIdList{get;set;}
      /// <summary>
        /// 客户状态 0未回访,4战败,5忙碌中待联系,6已预约出单,9已出单,13已报价考虑中（重点）,14其他,16无效数据（空号、停机）,17已报价考虑中（普通）,20预约到店,21申请投保
      /// </summary>
    
        public int Status { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public int CategoryInfo { get; set; }


        /// <summary>
        /// 是否全部修改
        /// </summary>
        public bool IsAll { get; set; }


      
    }


    public class UpdateCustomerStatusAndCategoriesJson {

        public long UserId { get; set; }
        /// <summary>
        /// 客户状态 0未回访,4战败,5忙碌中待联系,6已预约出单,9已出单,13已报价考虑中（重点）,14其他,16无效数据（空号、停机）,17已报价考虑中（普通）,20预约到店,21申请投保
        /// </summary>

        public int OldStatus { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string  OldCategoryInfo { get; set; }

    
        public CustomerStatus OldStatusInfo { get { return (CustomerStatus)OldStatus; }  }


        public string Placeholder { get { return "$"; } }
    }




   public enum CustomerStatus
   {
       未回访 = 0, 战败 = 4, 忙碌中待联系 = 5, 已预约出单 = 6, 已出单 = 9, 已报价考虑中重点 = 13, 其他 = 14, 无效数据空号停机 = 16, 已报价考虑中普通 = 17, 预约到店 = 20, 申请投保 = 21
    }
    

}
