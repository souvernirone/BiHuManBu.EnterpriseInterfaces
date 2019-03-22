
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UserInfoStatusViewModel : BaseViewModel
    {
        /// <summary>
        /// 系统是否有此数据 1 有 0 没有
        /// </summary>
        public int HasBaoJia { get; set; }
        /// <summary>
        /// Buid
        /// </summary>
        public long Buid { get; set; }

        /// <summary>
        /// 处理该数据的代理人Id
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 处理该数据的代理人姓名
        /// </summary>
        public string AgentName { get; set; }
    }
}
