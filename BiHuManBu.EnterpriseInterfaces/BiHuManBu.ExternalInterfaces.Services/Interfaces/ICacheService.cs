using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// ukey更新后清除缓存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel ClearUKeyCache(ClearUKeyCacheRequest request);
    }
}
