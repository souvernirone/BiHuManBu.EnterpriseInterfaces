using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class BatchRefreshRenewalViewModel:BaseViewModel
    {       
        /// <summary>
        /// 1:接收批量刷新续保数据
        /// 2:定时刷新前端续保状态
        /// </summary>
        public int RefreshType { get; set; }

        private List<BatchRefreshRenewalModel> _batchRefRenewalList = new List<BatchRefreshRenewalModel>();
        public List<BatchRefreshRenewalModel> BatchRefRenewalList
        {
            get { return _batchRefRenewalList; }
            set
            {
                if (value != null)
                    _batchRefRenewalList = value;
            }
        }         
    }

    public class BatchRefreshRenewalDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// 续保状态
        /// 1:成功，4：失败，6:排队中,0:该数据没有进行操作批量刷新续保
        /// </summary>
        public int RenewalType { get; set; }
    }
}
