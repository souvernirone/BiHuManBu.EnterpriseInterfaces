using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class WorkOrderDetailListViewModel : BaseViewModel
    {
        public WorkOrderViewModel WorkOrder { get; set; }
        public List<WorkOrderDetail> DetailList { get; set; }
    }

    public class WorkOrderViewModel
    {
        public int Id { get; set; }
        public Nullable<int> Intention_View { get; set; }
        public string Customer_Name { get; set; }
        public string Phone { get; set; }
        public Nullable<int> Sa_Agent_Id { get; set; }
        public string SaAgentName { get; set; }
        public Nullable<int> Status { get; set; }
        public string Create_Time { get; set; }
        public string Update_Time { get; set; }
        public Nullable<int> Adv_Agent_Id { get; set; }
        public string AdvAgentName { get; set; }
        public long Buid { get; set; }
        public long? IntentionCompany { get; set; }
        public string Remark { get; set; }
    }

    public class WorkOrderDetail
    {
        public int Id { get; set; }
        public Nullable<int> Result_Status { get; set; }
        public string Recall_Time { get; set; }
        public Nullable<int> Agent_Id { get; set; }
        public string AgentName { get; set; }
        public string Create_Time { get; set; }
        public Nullable<int> IntentionCompany { get; set; }
        public string Remark { get; set; }
    }
}
