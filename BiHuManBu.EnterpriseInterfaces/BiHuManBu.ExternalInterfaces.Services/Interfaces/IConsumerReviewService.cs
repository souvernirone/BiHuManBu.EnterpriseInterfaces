using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IConsumerReviewService
    {
        /// <summary>
        /// 获取本年回访次数
        /// </summary>
        /// <param name="listBuid"></param>
        /// <returns></returns>
        Task<List<YearReviewCountDto>> GetYearReviewCountAsync(List<long> listBuid);
        List<ConsumerReviewModel> GetConsumerReview(List<long> buids);
    }
}
