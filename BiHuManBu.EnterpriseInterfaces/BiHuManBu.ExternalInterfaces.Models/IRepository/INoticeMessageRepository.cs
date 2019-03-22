using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
   public interface INoticeMessageRepository
    {
        /// <summary>
        /// 获取推修消息列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentId">app登录人id</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<AccidentNoticeMessageModel> GetMessageListV23(int pageIndex, int pageSize, int agentId, out int totalCount);

        /// <summary>
        /// V 2.2.0及以前
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentId"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<AccidentNoticeMessageModel> GetMessageList(int pageIndex, int pageSize, int agentId, out int totalCount);

        /// <summary>
        /// 抢单信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        List<RobbingModel> GetRobbingMessageList(int agentId);
    }
}
