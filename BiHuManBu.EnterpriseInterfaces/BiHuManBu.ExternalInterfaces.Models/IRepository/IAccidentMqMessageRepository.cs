using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAccidentMqMessageRepository
    {
        /// <summary>
        /// 批量添加消息
        /// </summary>
        /// <param name="tx_Noticemessages"></param>
        /// <returns></returns>
        List<int> AddMessages(IEnumerable<tx_noticemessage> tx_Noticemessages);
        /// <summary>
        /// 批量更新消息发送状态
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        bool UpdateSendState(Dictionary<int, bool> dictionary);


        List<tx_noticemessage> GetPollingPeople(string ids, int count);
    }
}
