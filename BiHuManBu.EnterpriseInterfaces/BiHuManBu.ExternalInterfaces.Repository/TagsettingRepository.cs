using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TagsettingRepository : ITagsettingRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");

        /// <summary>
        /// 获取组跟经理人关系
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public List<bx_tagsetting_agent_relationship> GetTagsettingAgentRelationship(List<int> agentid)
        {
            List<bx_tagsetting_agent_relationship> item = new List<bx_tagsetting_agent_relationship>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_tagsetting_agent_relationship.Where(x => agentid.Contains(x.AgentId) && !x.Deleted).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 获取组
        /// </summary>
        /// <param name="angentid">经理人id</param>
        /// <returns>返回组信息</returns>
        public List<bx_tagsetting> GetTagsetting(List<int> angentid)
        {
            List<bx_tagsetting> result = new List<bx_tagsetting>();
            try
            {
                var item = GetTagsettingAgentRelationship(angentid);
                if (item != null && item.Count > 0)
                {
                    var tagIds = item.Select(x => x.TagId).ToList();
                    result = DataContextFactory.GetDataContext().bx_tagsetting.Where(x => tagIds.Contains(x.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="agentId">创建标签代理人编号</param>
        /// <returns></returns>
        public List<bx_tagsetting> AddTagsetting(int agentId)
        {
            List<bx_tagsetting> items = new List<bx_tagsetting>()
            {
                #region
                new bx_tagsetting(){
                    TagName="普通员工组",
                    AgentId=agentId,
                    CreateTime=DateTime.Now,
                    UpdateTime=DateTime.Now,
                   
                    TopAgent=agentId
                },
                new bx_tagsetting(){
                    TagName="销售顾问组",
                    AgentId=agentId,
                    CreateTime=DateTime.Now,
                    UpdateTime=DateTime.Now,
                   
                    TopAgent=agentId
                },
                new bx_tagsetting(){
                    TagName="续保顾问组",
                    AgentId=agentId,
                    CreateTime=DateTime.Now,
                    UpdateTime=DateTime.Now,
                
                    TopAgent=agentId
                },
                new bx_tagsetting(){
                    TagName="管理员组",
                    AgentId=agentId,
                    CreateTime=DateTime.Now,
                    UpdateTime=DateTime.Now,
                   
                    TopAgent=agentId
                }
                #endregion
            };
            try
            {
                foreach (var item in items)
                {
                    DataContextFactory.GetDataContext().bx_tagsetting.Add(item);
                    DataContextFactory.GetDataContext().SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return items;
        }
    }
}
