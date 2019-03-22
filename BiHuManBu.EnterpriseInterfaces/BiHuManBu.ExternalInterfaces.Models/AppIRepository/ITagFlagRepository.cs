using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.AppIRepository
{
    public interface ITagFlagRepository
    {
        List<bx_tagflag> GetByAgentId(int agentId);
        List<bx_tagflag> GetById(long[] ids);
        List<bx_tagflag> GetByContent(string[] tag, int agentId);
        bool AddTagFlag(List<bx_tagflag> addTagFlag);
        bx_tagflag GetById(int Id);
        int Update(bx_tagflag tag);
        List<bx_tagflag> GetByIds(long[] array);
    }
}
