using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetClusDetailsRequest: TXBaseRequest
    {
        /// <summary>
        /// 线索ID
        /// </summary>
        public int clueId { get; set; }


      


    }


    public class GetClusDetailsResponse: TXBaseResponse
    {
        public AccidentClueModel Data { get; set; }

       

    }
}
