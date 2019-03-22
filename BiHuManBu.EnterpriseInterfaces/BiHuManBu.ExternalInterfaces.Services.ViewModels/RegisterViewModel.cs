using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }

        public string Mobile { get; set; }

        public string AgentName { get; set; }
        public int AgentType { get; set; }
        public string Region { get; set; }
        public int IsDaiLi { get; set; }

        public int ParentAgent { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }

        private int _RegType = 0;

        public int RegType { get { return _RegType; } set { _RegType = value; } }
        public string AgentAddress { get; set; }

        private bool IsCheckCode = false;
        public bool _IsCheckCode
        {
            get { return IsCheckCode; }
            set { IsCheckCode = value; }
        }
        private bool IsUsed = false;
        public bool _IsUsed
        {
            get { return IsUsed; }
            set { IsUsed = value; }
        }

    }
}
