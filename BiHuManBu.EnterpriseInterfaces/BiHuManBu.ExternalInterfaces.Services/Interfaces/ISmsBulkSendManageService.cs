using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
   public interface ISmsBulkSendManageService
    {
        /// <summary>
        /// 获取目标用户电话
        /// </summary>
        /// <param name="getTargetUsersRequest">获取目标用户电话搜索模型</param>
        /// <param name="totalCount">总数量</param>
        /// <returns></returns>
        List<TargetUsersMobileResult> GetTargetUsersMobile(GetTargetUsersRequest getTargetUsersRequest, out int totalCount);
        /// <summary>
        /// 保存批量发送记录
        /// </summary>
        /// <param name="addSmsBulkSendRecordRequest">批量发送模型</param>
        /// <param name="batchId">批次编号</param>
        /// <returns></returns>
        SmsResultModel AddSmsBulkSendRecord(AddSmsBulkSendRecordRequest addSmsBulkSendRecordRequest, out int batchId);
        /// <summary>
        /// 获得批量发送记录
        /// </summary>
        /// <param name="getSmsBulkSendRecordRequest">获取批量发送记录条件模型</param>
        /// <param name="totalCount">总数量</param>
        /// <returns></returns>

        List<SmsBulkSendRecordViewModel> GetSmsBulkSendRecord(GetSmsBulkSendRecordRequest getSmsBulkSendRecordRequest, out int totalCount);
        /// <summary>
        /// 删除批量发送记录
        /// </summary>
        /// <param name="id">记录编号</param>
        /// <returns></returns>
        bool DeleteSmsBulkSendRecord(int id);
        /// <summary>
        /// 改变成立即发送
        /// </summary>
        /// <param name="request">更新</param>
        /// <param name="oldSendResult">原纪录</param>
        /// <returns></returns>
        SmsResultModel UpdateSmsBulkSendRecord(UpdateSmsBulkSendRecordRequest request, BulkSendResult oldSendResult);
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
        /// 服务发送
        /// </summary>
        /// <param name="request">发送请求集合</param>
        /// <returns></returns>
        void BulkSend(List<SendRequest> request);

        /// <summary>
        /// 根据代理人获得可用数量
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <returns></returns>
        int GetAvailCount(int agentId,int topAgentId);

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
        /// <param name="calculatedCount">计算后得数量</param>
        /// <param name="operationType">操作类型：1->加，2->减</param>

        /// <returns></returns>
        bool UpdateSmsAccountUseCount(int agentId, int calculatedCount, int operationType);
        /// <summary>
        /// 计算短信条数
        /// </summary>
        /// <param name="message">发送短信内容</param>
        /// <param name="isBatch">是否批量 1是批量 0单个短信</param>
        /// <returns></returns>
        int SDmessageCount(string message, int isBatch = 1);

    }
}
