using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IPushSpecialMessageService
    {
        /// <summary>
        /// 把buid推送给pc端客户
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="childagent"></param>
        /// <param name="agent"></param>
        /// <param name="agents"></param>
        void PushMsg(long buid, int childagent,int agent, List<int> agents);
    }
}
