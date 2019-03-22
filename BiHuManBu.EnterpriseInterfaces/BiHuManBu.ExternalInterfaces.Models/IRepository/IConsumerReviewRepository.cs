using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IConsumerReviewRepository
    {
        bx_consumer_review Find(int id);
        int AddDetail(bx_consumer_review bxWorkOrderDetail);
        int UpdateDetail(bx_consumer_review bxWorkOrderDetail);
        List<bx_consumer_review> FindDetails(long buid);
        bx_consumer_review FindNewClosedOrder(long buid, int status = 1);
        List<bx_consumer_review> FindNoReadList(int agentId, out int total);

        /// <summary>
        /// 获取本年回访次数
        /// </summary>
        /// <param name="listBuid"></param>
        /// <param name="thisYearBeginTime">今年的起始时间</param>
        /// <returns></returns>
        Task<List<YearReviewCountDto>> GetYearReviewCountAsync(List<long> listBuid,DateTime thisYearBeginTime);
        
        List<ConsumerReviewModel> GetConsumerReview(string buids); 
    }
}
