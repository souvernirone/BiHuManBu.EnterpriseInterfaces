using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentSMSRequest
    {
        public int TopAgentId { get; set; }

        public int AgentId { get; set; }

        public string Mobile { get; set; }

        public string SMSContent { get; set; }
    }


    public class TxSendVerificationCodeRequest
    {
   
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 手机码
        /// </summary>
        public string MobileCode { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string SecCode { get; set; }

    }



    public class TxSendVerificationCodeResponse:TXBaseResponse
    {

        public TxSendVerificationCodeModel Data { get; set; }


    }


    public class TxSendVerificationCodeModel {

    }


}
