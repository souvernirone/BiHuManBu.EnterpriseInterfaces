using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{

    public class ManagerModuleListViewModel : BaseViewModel
    {
        public List<ManagerModuleViewModel> ModuleList { get; set; }

     
    }

    public class ManagerModuleViewModel
    {
        public string module_code { get; set; }
        public string text { get; set; }
        public string pater_code { get; set; }
        public int roleId { get; set; }
        public string status { get; set; }
        public object state { get; set; }
        public string action_url { get; set; }
        public decimal? orderby { get; set; }
        public int buttonId { get; set; }
        public string buttonCode { get; set; }
        public int? moduleType { get; set; }

        public int? crm_module_type { get; set; }




        public    List<manager_function_db> listFunction { get; set; }

        public List<string> SelectFuncCode { get; set; }

        public List<ManagerModuleViewModel> attrs { get; set; }
        public List<ManagerModuleViewModel> nodes { get; set; }





    }













   








}
