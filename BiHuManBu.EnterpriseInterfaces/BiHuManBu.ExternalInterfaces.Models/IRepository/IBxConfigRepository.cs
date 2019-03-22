using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IBxConfigRepository
    {
        //跟新实体
        int Update(bx_config config);

        int Add(bx_config model);

        /// <summary>
        /// 根据配置键 获取配置值
        /// </summary>
        /// <param name="configKey">键</param>
        /// <returns></returns>
        List<bx_config> FindByConfigKey(string configKey);

        /// <summary>
        /// 修改  是否验证版本号
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        bool UpdateByConfigKey_Isupload(RequestKeyConfig request);
    }
}
