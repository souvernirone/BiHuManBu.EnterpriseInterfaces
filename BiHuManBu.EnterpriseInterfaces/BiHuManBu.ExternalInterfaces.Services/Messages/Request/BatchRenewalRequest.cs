using BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{

    /// <summary>
    /// 上传批量续保的请求类
    /// </summary>
    public class BatchRenewalRequest : BaseRequest2
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { set; get; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { set; get; }

        /// <summary>
        /// 代理人编号
        /// </summary>
        public int agentId { get; set; }
        /// <summary>
        /// 车型类别: 1大车 2是小车
        /// </summary>
        public int renewalCarType { get; set; }
        /// <summary>
        /// 续保城市编号
        /// </summary>
        public int cityId { get; set; }
        /// <summary>
        /// 渠道模式
        /// </summary>
        ///  /// <summary>
        /// 续保动态配置的时间数组
        /// </summary>
        public List<string> timeSetting { get; set; }
        //public int ChannelType { get; set; }
        //public List<int> SelectedSources { get; set; }
        public ChannelPattern ChannelPattern { get; set; }
        public bool isAuthorization { get; set; }
        /// <summary>
        /// 批量续保类型：0是需要续保的批次  1是不需要续保的批次
        /// </summary>
        public int batchRenewalType { get; set; }

    }
    public class UpdateBatchRenewalRequest : BaseRequest2
    {
        public ChannelPatternModel channelPattern { get; set; }
        public long batchRenewalId { get; set; }
        public int operateType { get; set; }
    }
    public class ChannelPattern
    {
        public int ChannelType;
        public List<int> SelectedSources;
        /// <summary>
        /// 是否历史承保
        /// </summary>
        public int IsHistoryRenewal;
    }

    /// <summary>
    /// 续保列表请求类
    /// </summary>

    /// <summary>
    ///导出错误数据请求类
    /// </summary>
    public class BatchRenewalExportErrorRequest : BaseRequest2
    {
        /// <summary>
        /// 代理人编号 根据代理人类型传入相对应的代理人编号 如：roletype是3或者4，就是顶级代理人编号。
        /// </summary>
        public long batchRenewalId { get; set; }
    }
    /// <summary>
    /// 续保结果请求类
    /// </summary>
    public class BatchRenewalDeleteDataRequest : BaseRequest2
    {
        /// <summary>
        /// BatchrenewalIdList：批续文件主键编号数组，每次只允许删除一条
        /// </summary>
        public List<int> batchrenewalIdList { get; set; }
    }
    /// <summary>
    /// 批量续保数量请求类
    /// </summary>
    public class BatchRenewalGetCountRequest : BaseRequest2
    {
        /// <summary>
        /// 代理人编号 根据代理人类型传入相对应的代理人编号 如：roletype是3或者4，就是顶级代理人编号。
        /// </summary>
        public int agentId { get; set; }
    }
    /// <summary>
    /// 获取批量续保选择续保城市接口
    /// </summary>
    public class BatchRenewalGetSourceRequest : BaseRequest2
    {
        /// <summary>
        /// 投保城市编号
        /// </summary>
        public int cityId { get; set; }
    }

    /// <summary>
    /// 通用请求
    /// </summary>
    public class BatchRenewalCommonRequest : BaseRequest2
    {
        /// <summary>
        /// 批续编号
        /// </summary>
        public int BatchId { get; set; }
        public int agentId { get; set; }
    }
}
