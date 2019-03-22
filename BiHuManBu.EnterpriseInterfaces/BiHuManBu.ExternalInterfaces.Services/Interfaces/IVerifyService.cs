using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IVerifyService
    {
        /// <summary>
        /// 通用的验证请求方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        BaseResponse Verify(BaseVerifyRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        BaseResponse Verify(string SecCode, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 参数校验
        /// </summary>
        /// <param name="list">参数列表</param>
        /// <param name="checkCode">输入的校验串</param>
        /// <returns></returns>
        BaseResponse ValidateReqestGet(IEnumerable<KeyValuePair<string, string>> list, string checkCode);

        /// <summary>
        /// 参数校验
        /// </summary>
        /// <param name="urlData">json</param>
        /// <param name="checkCode">输入的校验串</param>
        /// <returns></returns>
        BaseResponse ValidateReqestPost(string urlData,string checkCode);
    }
}
