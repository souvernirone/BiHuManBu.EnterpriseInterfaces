using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class WorkOrderDetailListResponse:BaseResponse
    {
        public bx_userinfo_renewal_info WorkOrder { get; set; }
        public List<bx_consumer_review> WorkOrderDetailList { get; set; }
    }
}
