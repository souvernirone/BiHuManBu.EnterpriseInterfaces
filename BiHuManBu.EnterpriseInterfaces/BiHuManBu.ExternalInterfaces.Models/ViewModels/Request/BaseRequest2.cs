using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 请求基类2.0版本，去掉了SecCode，分离出了接口验证逻辑
    /// </summary>
    public class BaseRequest2
    { 
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        [Range(1,int.MaxValue,ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }

        /// <summary>
        /// 当前登录人的AgentId
        /// </summary>
        [Range(1,int.MaxValue,ErrorMessage = "ChildAgent参数错误")]
        public int ChildAgent { get; set; }
    }
}
