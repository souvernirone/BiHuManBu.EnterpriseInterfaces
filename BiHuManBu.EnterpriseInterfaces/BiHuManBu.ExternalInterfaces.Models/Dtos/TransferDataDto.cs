using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 转移数据成功，发送通知模型
    /// </summary>
    public class TransferDataDto
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 转移数据的详情集合
        /// </summary>
        public List<TransferDataDetail> TransferDataDetailList { get; set; }
    }

    public class TransferDataDetail
    {
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
    }
}
