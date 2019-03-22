using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ConsumerReviewRepository : IConsumerReviewRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        public DbContext _context;

        public ConsumerReviewRepository(DbContext context)
        {
            logError = LogManager.GetLogger("ERROR");
            _context = context;
        }
        public bx_consumer_review Find(int id)
        {
            bx_consumer_review model = new bx_consumer_review();
            try
            {
                model = DataContextFactory.GetDataContext().bx_consumer_review.Where(i => i.id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
        public int AddDetail(bx_consumer_review bxWorkOrderDetail)
        {
            int workOrderDetailId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_consumer_review.Add(bxWorkOrderDetail);
                DataContextFactory.GetDataContext().SaveChanges();
                workOrderDetailId = t.id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                workOrderDetailId = 0;
            }
            return workOrderDetailId;
        }
        public int UpdateDetail(bx_consumer_review bxWorkOrderDetail)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_consumer_review.AddOrUpdate(bxWorkOrderDetail);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public List<bx_consumer_review> FindDetails(long buid)
        {
            List<bx_consumer_review> list = new List<bx_consumer_review>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_consumer_review.Where(x => x.b_uid == buid).OrderByDescending(o => o.create_time).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        /// <summary>
        /// 查找最后一条已出单记录
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bx_consumer_review FindNewClosedOrder(long buid, int status = 1)
        {
            bx_consumer_review model = new bx_consumer_review();
            try
            {
                model = DataContextFactory.GetDataContext().bx_consumer_review.Where(x => x.b_uid == buid && x.result_status == status).OrderByDescending(o => o.id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return model;
        }
        /// <summary>
        /// 获取所有的未读续保记录，未排序，消息列表用
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<bx_consumer_review> FindNoReadList(int agentId, out int total)
        {
            List<bx_consumer_review> list = new List<bx_consumer_review>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_consumer_review.Where(i => i.operatorId == agentId && i.read_status == 0 && i.next_review_date.HasValue && i.next_review_date.Value <= DateTime.Now).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }

        public async Task<List<YearReviewCountDto>> GetYearReviewCountAsync(List<long> listBuid, DateTime thisYearBeginTime)
        {
            var buids = string.Join(",", listBuid);
            var sqlPlaceHolder = @"
                            SELECT
                              b_uid AS BuId,
                              COUNT(1) AS ReviewCount
                            FROM bx_consumer_review
                            WHERE create_time >= '{1}'
                                AND b_uid IN({0})
                            GROUP BY b_uid;
                            ";
            var sql = string.Format(sqlPlaceHolder, buids, thisYearBeginTime.ToShortDateString());

            return await Task.FromResult(_context.Database.SqlQuery<YearReviewCountDto>(sql).ToList());
        }
        public List<ConsumerReviewModel> GetConsumerReview(string buids)
        {
            try
            {
                StringBuilder sqlBuilder = new StringBuilder();
                //sqlBuilder.Append("SELECT * FROM bx_consumer_review ");
                //sqlBuilder.Append("WHERE Id IN (SELECT MAX(Id) FROM bx_consumer_review WHERE b_uid IN(" + buids + ") GROUP BY b_uid )");
                sqlBuilder.Append("SELECT ");
                sqlBuilder.Append("  t.b_uid,");
                sqlBuilder.Append("  t.next_review_date");
                sqlBuilder.Append(" FROM (SELECT");
                sqlBuilder.Append("        b_uid,");
                sqlBuilder.Append("        next_review_date,");
                sqlBuilder.Append("        IF(@grp=b_uid,@rank:=@rank+1,1) AS rank,");
                sqlBuilder.Append("        @grp:=b_Uid");
                sqlBuilder.Append("      FROM ((SELECT id,b_uid,next_review_date");
                sqlBuilder.Append("             FROM bx_consumer_review");
                sqlBuilder.Append("             WHERE b_uid IN(" + buids + ")");
                sqlBuilder.Append("             ORDER BY b_uid,id DESC) AS a,");
                sqlBuilder.Append("         (SELECT");
                sqlBuilder.Append("            @grp:=0,");
                sqlBuilder.Append("            @rank:=0) AS b)) AS t");
                sqlBuilder.Append(" WHERE t.rank = 1");
                return _context.Database.SqlQuery<ConsumerReviewModel>(sqlBuilder.ToString()).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("BUIDS=" + buids + "；发生异常：" + ex);
            }
            return new List<ConsumerReviewModel>();
        }

        
    }
}
