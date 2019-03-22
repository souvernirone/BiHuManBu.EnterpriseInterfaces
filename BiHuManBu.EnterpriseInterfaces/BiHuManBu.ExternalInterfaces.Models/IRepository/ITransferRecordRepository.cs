using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{   
   public interface ITransferRecordRepository
    {
       /// <summary>
       /// 保存记录
       /// </summary>
       /// <param name="buId">bx_userinfo.id</param>
       /// <param name="fromAgentId">来源代理人编号</param>
       /// <param name="ToAgentId">接受代理人编号</param>
       /// <param name="stepType">当前步骤类型</param>
       /// <returns></returns>
       bool SaveTransferRecord(long buId, int fromAgentId, int stepType, int? ToAgentId);

       long Add(bx_transferrecord record);

       List<bx_transferrecord> FindListByBuidList(List<long> buids);

       bx_transferrecord FindFirstSaByBuid(long buid);

       List<bx_transferrecord> FindByBuid(long buid);
    }
}
