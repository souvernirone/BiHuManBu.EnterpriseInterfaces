using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ITagsettingRepository
    {
        List<bx_tagsetting> GetTagsetting(List<int> agentId);
        List<bx_tagsetting_agent_relationship> GetTagsettingAgentRelationship(List<int> agentid);
        List<bx_tagsetting> AddTagsetting(int agentId);
    }
}
