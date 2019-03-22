using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ITagsettingAgentRelationshipRepository
    {
        void AddTagsettingAgent(bx_agent agent, List<bx_tagsetting> tagsetting);
    }
}
