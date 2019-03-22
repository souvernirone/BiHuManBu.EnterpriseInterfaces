using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class NoticexbRepository : INoticexbRepository
    {
        private ILog logError;

        public NoticexbRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }
        public bx_notice_xb Find(int id)
        {
            bx_notice_xb bxNoticeXb = new bx_notice_xb();
            try
            {
                bxNoticeXb = DataContextFactory.GetDataContext().bx_notice_xb.FirstOrDefault(i => i.id == id);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxNoticeXb;
        }
        public bx_notice_xb FindByBuid(long buid)
        {
            bx_notice_xb bxNoticeXb = new bx_notice_xb();
            try
            {
                bxNoticeXb = DataContextFactory.GetDataContext().bx_notice_xb.OrderByDescending(o => o.id).FirstOrDefault(i => i.b_uid == buid);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return bxNoticeXb;
        }
        public int Update(bx_notice_xb bxNoticeXb)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_notice_xb.AddOrUpdate(bxNoticeXb);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public long Add(bx_notice_xb bxNoticeXb)
        {
            long noticexb = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_notice_xb.Add(bxNoticeXb);
                DataContextFactory.GetDataContext().SaveChanges();
                noticexb = t.id;
            }
            catch (Exception ex)
            {
                noticexb = 0;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return noticexb;
        }
        /// <summary>
        /// 获取所有的未读续保记录，未排序，消息列表用
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<bx_notice_xb> FindNoReadList(int agentId, out int total)
        {
            List<bx_notice_xb> list = new List<bx_notice_xb>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_notice_xb.Where(i => i.agent_id == agentId && i.stauts == 0).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            total = list.Count;
            return list;
        }
        public List<bx_notice_xb> FindByAgentId(int agentId)
        {
            List<bx_notice_xb> list = new List<bx_notice_xb>();
            try
            {
                list = DataContextFactory.GetDataContext().bx_notice_xb.Where(i => i.agent_id == agentId && i.stauts == 0).OrderByDescending(o => o.create_time).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
    }
}
