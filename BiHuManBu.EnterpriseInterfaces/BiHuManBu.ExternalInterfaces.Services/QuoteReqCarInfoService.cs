using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class QuoteReqCarInfoService : IQuoteReqCarInfoService
    {
        private IQuoteReqCarInfoRepository _quoteReqCarInfoRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteReqCarInfoRepository"></param>
        public QuoteReqCarInfoService(IQuoteReqCarInfoRepository quoteReqCarInfoRepository)
        {
            _quoteReqCarInfoRepository = quoteReqCarInfoRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public List<IsNewCarViewModel> GetIsNewCarList(List<long> buid)
        {
            if (buid.Count <= 0)
                return new List<IsNewCarViewModel>();
            return _quoteReqCarInfoRepository.GetIsNewCarList(buid);
        }

        public List<IsNewCarViewModel> GetIsNewCar(List<long> buid)
        {
            if (buid.Count <= 0)
                return new List<IsNewCarViewModel>();
            return _quoteReqCarInfoRepository.GetIsNewCar(buid);
        }
    }
}
