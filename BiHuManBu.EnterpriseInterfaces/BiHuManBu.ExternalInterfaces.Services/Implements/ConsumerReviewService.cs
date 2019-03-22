using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class ConsumerReviewService : IConsumerReviewService
    {
        private IConsumerReviewRepository _consumerReviewRepository;

        public ConsumerReviewService(IConsumerReviewRepository consumerReviewRepository)
        {
            _consumerReviewRepository = consumerReviewRepository;
        }

        public async Task<List<YearReviewCountDto>> GetYearReviewCountAsync(List<long> listBuid)
        {
            if (listBuid.Count == 0)
                return new List<YearReviewCountDto>();

            var thisYear = Convert.ToDateTime(DateTime.Now.Year + "-01-01");

            return await _consumerReviewRepository.GetYearReviewCountAsync(listBuid, thisYear);
        }
        public List<ConsumerReviewModel> GetConsumerReview(List<long> buids)
        {
            if (buids == null || buids.Count == 0)
            {
                return new List<ConsumerReviewModel>();
            }
            List<ConsumerReviewModel> list = _consumerReviewRepository.GetConsumerReview(string.Join(",", buids));
            
            return list;
        }
        
    }
}
