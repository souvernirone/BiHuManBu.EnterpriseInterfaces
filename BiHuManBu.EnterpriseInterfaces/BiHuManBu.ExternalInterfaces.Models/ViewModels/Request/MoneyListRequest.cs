using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetMoneyListRequest : PageRequest
    {
        public string AgentName { get; set; }

        public string Mobile { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BillInfoRequest : PageRequest
    {
        /// <summary>
        /// 要查看的代理人ID
        /// </summary>
        public int SearchAgentId { get; set; }

        public DateTime StartTime { get; set; }

        private DateTime _endTime;

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                if (_endTime == DateTime.MinValue)
                {
                    _endTime = DateTime.MaxValue;
                }
                if (_endTime.Hour == 0 && _endTime.Minute == 0 && _endTime.Second == 0)
                {
                    var date = _endTime.ToShortDateString();
                    date += " 23:59:59";
                    _endTime = Convert.ToDateTime(date);
                }
            }
        }
    }
}
