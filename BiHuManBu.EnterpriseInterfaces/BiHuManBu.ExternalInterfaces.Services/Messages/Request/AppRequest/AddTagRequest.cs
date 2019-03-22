using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AddTagRequest : AppBaseRequest
    {
        public string Tags { get; set; }
    }
    public class DelTagRequest : AppBaseRequest
    {
        public int TagId { get; set; }
    }
}
