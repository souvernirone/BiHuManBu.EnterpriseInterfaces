using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
  /// <summary>
  /// 代理人开通城市请求对象
  /// </summary>
  public  class AegntDredgeCityResult
    {
      /// <summary>
      /// 顶级代理人
      /// </summary>
      [Range(1, int.MaxValue, ErrorMessage = "顶级代理人Id不能小于1")]
      public int TopAgentId { get; set; }
      /// <summary>
      /// 
      /// </summary>
      [Required(ErrorMessage ="SecCode不能为空")]
      public string SecCode { get; set; }
      /// <summary>
      /// 
      /// </summary>
      [Required(ErrorMessage = "CustKey不能为空")]
      public string CustKey { get; set; }
    }
}
