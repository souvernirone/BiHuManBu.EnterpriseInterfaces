using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ResponesActivityViewModel : BaseViewModel
    {
        /// <summary>
        /// 优惠活动集合
        /// </summary>
        public List<ResponesActivity> ActivitysList { get; set; }

        /// <summary>
        /// 返回总条数
        /// </summary>
        public int TotalCount { get; set; }
    }

    public class RequestActivityViewModel : BaseVerifyRequest
    {
        /// <summary>
        /// 优惠活动集合
        /// </summary>
        public List<ResponesActivity> Activitys { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyName { get; set; }
    }

    public class RequestAddActivityViewModel 
    {
        /// <summary>
        /// 顶级代理人的AgentId
        /// </summary>
        [Range(1, 1000000)]
        public int Agent { get; set; }


        private int _childAgent = 0;

        /// <summary>
        /// 子级代理人id
        /// </summary>
        public int ChildAgent
        {
            get { return _childAgent; }
            set { _childAgent = value; }

        }

        /// <summary>
        /// 优惠活动内容
        /// </summary>
        public string ActivityContent { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateName { get; set; }
    }

    public class ResponesActivity 
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 活动内容
        /// </summary>
        public string ActivityContent { get; set; }

        /// <summary>
        /// 状态： selected 不做任何处理； modify 保存时需要修改的； add 新增；  del 删除
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 编辑状态
        /// </summary>
        public int IsEdit { get; set; }

        /// <summary>
        /// 是否是自己的活动
        /// </summary>
        public int IsMine { get; set; }
    }

    public class ResponesAddActivity : BaseViewModel
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
    }


    public class ResponseAddActivityViewModel : BaseViewModel 
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int? Id { get; set; }
    }
}
