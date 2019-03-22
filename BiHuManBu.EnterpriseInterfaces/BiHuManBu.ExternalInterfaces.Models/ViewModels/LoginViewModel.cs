
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public int ParentAgent { get; set; }

        private int _isWChat = 0;
        private int _topAgentId = 0;
        private int _agentId = 0;
        private int _fromMethod = -1;

        public int AgentId
        {
            get { return _agentId; }
            set { _agentId = value; }
        }
        public int IsWChat
        {
            get { return _isWChat; }
            set { _isWChat = value; }
        }
        public int TopAgentId
        {
            get { return _topAgentId; }
            set { _topAgentId = value; }
        }

        /// <summary>
        /// 1:PC 2:微信 4:APP
        /// </summary>
        public int FromMethod
        {
            get { return _fromMethod; }
            set { _fromMethod = value; }
        }
        /// <summary>
        /// 集团Id
        /// </summary>
        public int GroupId { get; set; }
    }
}
