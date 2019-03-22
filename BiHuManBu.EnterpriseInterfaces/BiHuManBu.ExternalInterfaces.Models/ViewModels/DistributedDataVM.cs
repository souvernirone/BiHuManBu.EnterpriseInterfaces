using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /**
     * 
     *  请求通知接口对象 
     * 
     */
    public class DistributedDataVm : BaseViewModel
    {
        /// <summary>
        /// 是否需要特殊提醒
        /// </summary>
        public bool IsManager { get; set; }

        public int AgentId { set; get; }

        private List<CompositeBuldLicenseNo> data = new List<CompositeBuldLicenseNo>();

        public new List<CompositeBuldLicenseNo> Data
        {
            get
            {
                return data;
            }
            set
            {
                if (value != null)
                    data = value;
            }
        }

        public string BuidsString { get; set; }
        public string MessageIds { get; set; }


    }

    public class CompositeBuldLicenseNo
    {

        public long BuId { get; set; }

        public string LicenseNo { get; set; }
    }

}