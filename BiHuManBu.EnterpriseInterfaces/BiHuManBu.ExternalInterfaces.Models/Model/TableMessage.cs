using System;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class TableMessage
    {
        public string StrId { get; set; }
        public long Id { get; set; }
        public int Msg_Level { get; set; }
        public int Msg_Status { get; set; }
        public int Msg_Type { get; set; }
        public string SendTime { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Update_Time { get; set; }
        public string Url { get; set; }
        public int Agent_Id { get; set; }
        public string AgentName { get; set; }
        public int Create_Agent_Id { get; set; }
        public string CreateAgentName { get; set; }
        public DateTime Create_Time { get; set; }
        public string LicenseNo { get; set; }
        public string Last_Force_End_Date { get; set; }
        public string Last_Biz_End_Date { get; set; }
        public string Next_Force_Start_Date { get; set; }
        public string Next_Biz_Start_Date { get; set; }
        public int? Source { get; set; }
        public int? Days { get; set; }
        public int? OwnerAgent { get; set; }
        public long? Buid { get; set; }
    }
}
