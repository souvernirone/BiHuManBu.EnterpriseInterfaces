using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CqaLoginResultModel:BaseViewModel
    {

        public int type { get; set; }

        public List<int> agents { get; set; }

        /// <summary>
        /// 销售职位以下的用户Id
        /// </summary>
        public List<int> sons { get; set; }

        public List<CqaUser> cqaSons { get; set; }

        public List<int> citys { get; set; }

        public string CurrentMobile { get; set; }

    }


    public class CqaKv
    {
        public int id { get; set; }
        public int pid { get; set; }
    }

    public class CqaCA
    {
        public int cqaId { get; set; }
        public int agentId { get; set; }
    }

    public class CqaUser
    {
        #region Model
        /// <summary>
        /// 主键
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 区域id，枚举类型
        /// </summary>
        public int? areaid { get; set; }

        public string areaName { get; set; }
        /// <summary>
        /// 0销售，1城市经理，2大区总
        /// </summary>
        public int? position { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string positionName { get; set; }
        /// <summary>
        /// 上级销售id
        /// </summary>
        public int? parentId { get; set; }
        /// <summary>
        /// 角色 1、销售 2、运营
        /// </summary>
        public int role { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }
        #endregion
    }
}
