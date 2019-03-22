using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TagsettingAgentRelationshipRepository : ITagsettingAgentRelationshipRepository
    {
        private ILog logError;
        public TagsettingAgentRelationshipRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        /// <summary>
        /// 添加经纪人跟组关系
        /// </summary>
        /// <param name="agent"></param>
        public void AddTagsettingAgent(bx_agent agent, List<bx_tagsetting> tagsetting)
        {
            try
            {
                var items = tagsetting.FirstOrDefault();
                bx_tagsetting_agent_relationship item = new bx_tagsetting_agent_relationship()
                {
                    AgentId = agent.Id,
                    TagId = items.Id,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                DataContextFactory.GetDataContext().bx_tagsetting_agent_relationship.Add(item);
                DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}
