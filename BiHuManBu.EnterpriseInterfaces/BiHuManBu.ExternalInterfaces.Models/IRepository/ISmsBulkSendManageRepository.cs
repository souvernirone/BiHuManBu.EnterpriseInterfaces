using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
   public interface ISmsBulkSendManageRepository
    {
        /// <summary>
        /// 获取目标用户电话
        /// </summary>
        /// <param name="getTargetUsersRequest">获取目标用户电话搜索模型</param>
        /// <param name="totalCount">总数量</param>
        /// <param name="agentIdsStr">代理人集合字符串</param>
        /// <returns></returns>
        List<TargetUsersMobileResult> GetTargetUsersMobile(GetTargetUsersRequest getTargetUsersRequest, string agentIdsStr,out int totalCount);
      /// <summary>
      /// 保存批量发送记录
      /// </summary>
      /// <param name="addSmsBulkSendRecordRequest">批量发送模型</param>
      /// <returns></returns>
        int AddSmsBulkSendRecord(dynamic addSmsBulkSendRecordRequest);
        /// <summary>
        /// 获得批量发送记录
        /// </summary>
        /// <param name="getSmsBulkSendRecordRequest">获取批量发送记录条件模型</param>
        /// <param name="totalCount">总数量</param>
        /// <param name="agentIds">agentIds</param>
        /// <returns></returns>

        List<SmsBulkSendRecordViewModel> GetSmsBulkSendRecord(GetSmsBulkSendRecordRequest getSmsBulkSendRecordRequest,List<int>agentIds,out int totalCount);
        /// <summary>
        /// 删除批量发送记录
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        bool DeleteSmsBulkSendRecord(int id);
        /// <summary>
        /// 改变成立即发送
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <param name="sendTime">发送时间</param>
        /// <param name="status">发送状态</param>
        /// <param name="mobileList">发送号码集合</param>
        /// <param name="smsContent">短信内容</param>
        /// <param name="agentId">代理人编号</param>
        /// <param name="agentName">代理人名称</param>
        /// <returns></returns>
        bool UpdateSmsBulkSendRecord(int id, int agentId, DateTime sendTime, int status, List<string> mobileList, string smsContent, string agentName);
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        bool CancelSend(int id);
        /// <summary>
        /// 根据批次编号获取发送电话号和发送内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SendMobilesAndContentResult GetSendMobilesAndContentById(int id);
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        bool UpdateStatus(int id);
        /// <summary>
        /// 根据批次号获取代理人编号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BulkSendResult GetBulkSendRecordById(int id);
        /// <summary>
        /// 更新可用数量
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="calculatedCount">计算后数量</param>
        /// <param name="operationType">操作类型：1->加，2->减</param>
        /// <returns></returns>
        bool UpdateSmsAccountUseCount(int agentId, int calculatedCount, int operationType);
        /// <summary>
        /// 根据代理人获得可用数量
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        int GetAvailCount(int agentId);
        /// <summary>
        /// 发送失败后处理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ChangeSmsBulkRedcordToFail(int id);
    }
}
