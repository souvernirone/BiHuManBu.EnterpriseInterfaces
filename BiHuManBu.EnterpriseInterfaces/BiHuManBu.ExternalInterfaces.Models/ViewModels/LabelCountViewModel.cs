using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 获取标签下客户数量的模型
    /// </summary>
    public class LabelCountViewModel : BaseViewModel<LabelCountViewModel>
    {
        ///// <summary>
        ///// 总数量
        ///// </summary>
        //public int TotalCount { get; set; }

        ///// <summary>
        ///// 上级新分配数量
        ///// </summary>
        //public int NewDistribution { get; set; }

        ///// <summary>
        ///// 今日计划回访数量
        ///// </summary>
        //public int TodayReView { get; set; }

        ///// <summary>
        ///// 今日摄像头进店数量
        ///// </summary>
        //public int TodayCamera { get; set; }

        ///// <summary>
        ///// 预约到店数量
        ///// </summary>
        //public int OrderEnter { get; set; }

        public Dictionary<int, int> _counts = new Dictionary<int, int>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int,int> Counts
        {
            get { return _counts; }
            set { _counts = value; }
        }
    }
}
