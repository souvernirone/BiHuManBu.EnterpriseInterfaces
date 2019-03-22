using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class BatchRefreshRenewalRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest2
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

        private int _refreshType = 2;
        /// <summary>
        /// 1:接收批量刷新续保数据
        /// 2:定时刷新前端续保状态
        /// </summary>
        public int RefreshType
        {
            get { return _refreshType; }
            set
            {
                _refreshType = value;
            }
        }
    }

    public class BatchRefreshRenewalDetailRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest2
    {
        public long Buid { get; set; }
    }
}
