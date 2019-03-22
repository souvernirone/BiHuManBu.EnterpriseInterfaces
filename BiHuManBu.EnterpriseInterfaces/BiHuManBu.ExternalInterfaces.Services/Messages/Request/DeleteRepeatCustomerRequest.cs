using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class DeleteRepeatCustomerRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest2
    {
        private List<long> _buids = new List<long>();
        /// <summary>
        /// 根据buid批量查询
        /// </summary>
        public List<long> Buids
        {
            get { return _buids; }
            set
            {
                if (value != null)
                    _buids = value;
            }
        }
    }
}
