using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using log4net;
using MySql.Data.MySqlClient;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System.Data;
using System.Data.Entity.Validation;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;


namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class TagFlagRepository : ITagFlagRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public List<bx_tagflag> GetByAgentId(int agentId)
        {
            List<bx_tagflag> tagFlags = new List<bx_tagflag>();
            try
            {
                tagFlags = DataContextFactory.GetDataContext().bx_tagflag.Where(x => (x.AgentId == agentId || x.AgentId == 0) && x.ValidFlag == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tagFlags;
        }

        public List<bx_tagflag> GetById(long[] ids)
        {
            List<bx_tagflag> tagFlags = new List<bx_tagflag>();
            try
            {
                tagFlags = DataContextFactory.GetDataContext().bx_tagflag.Where(x => ids.Contains(x.Id) && x.ValidFlag == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tagFlags;
        }
        public List<bx_tagflag> GetByContent(string[] tag, int agentId)
        {
            List<bx_tagflag> tagFlags = new List<bx_tagflag>();
            try
            {
                tagFlags = DataContextFactory.GetDataContext().bx_tagflag.Where(x => tag.Contains(x.Content) && x.ValidFlag == 1 && x.AgentId == agentId).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tagFlags;
        }
        public bool AddTagFlag(List<bx_tagflag> addTagList)
        {
            IEnumerable<bx_tagflag> tagFlags = new List<bx_tagflag>();
            try
            {
                tagFlags = DataContextFactory.GetDataContext().bx_tagflag.AddRange(addTagList);
                DataContextFactory.GetDataContext().SaveChanges();
                if (tagFlags.Count() == addTagList.Count)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
        public bx_tagflag GetById(int Id)
        {
            bx_tagflag tagFlag = new bx_tagflag();
            try
            {
                tagFlag = DataContextFactory.GetDataContext().bx_tagflag.Where(x => x.Id == Id && x.ValidFlag == 1).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tagFlag;
        }
        public int Update(bx_tagflag tag)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_tagflag.AddOrUpdate(tag);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }
        public List<bx_tagflag> GetByIds(long[] array)
        {
            List<bx_tagflag> tagFlags = new List<bx_tagflag>();
            try
            {
                tagFlags = DataContextFactory.GetDataContext().bx_tagflag.Where(x => array.Contains(x.Id) && x.ValidFlag == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return tagFlags;
        }
    }
}
