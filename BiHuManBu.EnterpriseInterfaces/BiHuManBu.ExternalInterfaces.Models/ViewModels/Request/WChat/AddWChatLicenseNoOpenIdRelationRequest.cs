using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.WChat
{
  /// <summary>
  /// 
  /// </summary>
  public  class AddWChatLicenseNoOpenIdRelationRequest:BaseRequestViewModel
    {
      /// <summary>
      /// 微信的openid
      /// </summary>
      public string OpenId { get; set; }
      /// <summary>
      /// 顶级代理人
      /// </summary>
      [Range(1, int.MaxValue, ErrorMessage = "顶级代理人Id不能小于1")]
      public int TopAgentId { get; set; }
      /// <summary>
      /// 车牌号
      /// </summary>
      public string LicenseNo { get; set; }

      /// <summary>
      /// 报价地区
      /// </summary>
      public int CityCode { get; set; }

    }
}
