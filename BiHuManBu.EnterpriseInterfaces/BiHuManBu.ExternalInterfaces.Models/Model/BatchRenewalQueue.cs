using BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{

    public class BatchRenewalQueue
    {
    }
    public class QueueInfo
    {
        public List<BatchRenewalItemViewModel> batchRenewalItemViewModels { get; set; }
        public long batchrenewalId { get; set; }
        public string agentId { get; set; }
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        public int Agent { get; set; }
        /// <summary>
        /// 当前登录人的AgentId
        /// </summary>
        public int ChildAgent { get; set; }
        public long firstBuid { get; set; }
        public int cityId { get; set; }
        /// <summary>
        ///类别ID
        /// </summary>
        public int categoryId { get; set; }
        public UploadFileResult result { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { set; get; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { set; get; }
        public int renewalCarType { get; set; }
        public IList<CheckBackModel> checkUserModels { get; set; }

        public List<string> timeSetting { get; set; }
        public int dataGrowNum { get; set; }
        public bool isAuthorization { get; set; }

        public int batchRenewalType { get; set; }
    }


    public class ListAgent
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public int ParentAgent { get; set; }
        public string AgentAccount { get; set; }
    }
}
