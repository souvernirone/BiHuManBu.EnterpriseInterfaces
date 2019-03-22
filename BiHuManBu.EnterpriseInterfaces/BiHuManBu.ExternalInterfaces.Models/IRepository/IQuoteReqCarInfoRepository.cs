using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQuoteReqCarInfoRepository
    {
        bx_quotereq_carinfo Add(bx_quotereq_carinfo item);

        int Update(bx_quotereq_carinfo item);

        /// <summary>
        /// 根据给定的buid查找是否是新车
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<IsNewCarViewModel> GetIsNewCarList(List<long> buid);

        List<IsNewCarViewModel> GetIsNewCar(List<long> buid);

        bx_quotereq_carinfo Find(long buid);

        Task<bx_quotereq_carinfo> FindAsync(long buid);

    }
}
