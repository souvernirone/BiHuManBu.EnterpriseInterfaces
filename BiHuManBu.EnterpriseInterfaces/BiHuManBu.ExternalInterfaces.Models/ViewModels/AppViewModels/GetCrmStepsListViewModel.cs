using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetCrmStepsListViewModel : AppBaseViewModel
    {
        /// <summary>
        /// 返回步骤列表模型
        /// </summary>
        public List<ConsumerDetailCrmListResponsedeatil> BxCrmStepses { get; set; }
    }

    /// <summary>
    /// 返回步骤列表模型
    /// </summary>
    public class ConsumerDetailCrmListResponsedeatil
    {
        /// <summary>
        /// 创建时间（似乎可以不用，不知道当时为啥这么写）
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 返回步骤列表模型（直接调用即可，不知道当时为啥这么写）
        /// </summary>
        public List<ConsumerDetailCrmListResponseStep> JsonStepses { get; set; }
    }
    /// <summary>
    /// 返回步骤列表模型
    /// </summary>
    public class ConsumerDetailCrmListResponseStep
    {
        /// <summary>
        /// 对应记录Id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// Json模型，请解析里面的Json
        /// </summary>
        public object json_content { get; set; }
        /// <summary>
        /// 操作记录的当前AgentId
        /// </summary>
        public int agent_id { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 操作类型（目前暂时只用到123）1回访，2短信报价，3预约单，4保单，5 bx_userinfo批量删除
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 对应的Buid
        /// </summary>
        public long b_uid { get; set; }
        /// <summary>
        /// （不要取此值）跟上面的create_time是一样的
        /// </summary>
        public string stime { get; set; }
    }
}
