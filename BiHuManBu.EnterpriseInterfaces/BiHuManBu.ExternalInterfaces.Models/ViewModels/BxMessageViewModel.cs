using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class BxMessageViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Nullable<int> Msg_Type { get; set; }
        public Nullable<int> Agent_Id { get; set; }
        public Nullable<System.DateTime> Create_Time { get; set; }
        public Nullable<System.DateTime> Update_Time { get; set; }
        public Nullable<int> Msg_Status { get; set; }
        public Nullable<int> Msg_Level { get; set; }
        public Nullable<System.DateTime> Send_Time { get; set; }
        public int Create_Agent_Id { get; set; }
        public string Url { get; set; }
        public string License_No { get; set; }
        public Nullable<long> Buid { get; set; }
        public Nullable<int> ShowType { get; set; }
        public string ChannelAndScope { get; set; }
        public string MsgStatus { get; set; }
    }
}
