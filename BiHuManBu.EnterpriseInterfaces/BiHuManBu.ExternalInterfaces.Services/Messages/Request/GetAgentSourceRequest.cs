
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetAgentSourceRequest:BaseVerifyRequest
    {
        private int _isBj = 1;
        /// <summary>
        /// 是否报价渠道 1是 0否 2全部数据
        /// </summary>
        public int IsBj
        {
            get { return _isBj; }
            set { _isBj = value; }

        } 
    }
}
