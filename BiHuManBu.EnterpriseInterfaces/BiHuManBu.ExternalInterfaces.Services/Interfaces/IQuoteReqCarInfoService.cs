using BiHuManBu.ExternalInterfaces.Models.ViewModels;
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
    public interface IQuoteReqCarInfoService
    {
        /// <summary>
        /// 根据给定的buid查找是否是新车
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<IsNewCarViewModel> GetIsNewCarList(List<long> buid);

        List<IsNewCarViewModel> GetIsNewCar(List<long> buid);
    }
}
