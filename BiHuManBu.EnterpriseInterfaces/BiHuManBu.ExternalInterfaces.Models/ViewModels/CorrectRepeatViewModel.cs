using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CorrectRepeatViewModel : BaseViewModel
    {
        private int _isHas = 2;
        /// <summary>
        /// 是否存在数据：1有，2无
        /// </summary>
        public int IsHas
        {
            get { return _isHas; }
            set { _isHas = value; }
        }

        /// <summary>
        /// add by qdk 2018-11-09
        /// 交强险批单号
        /// 接收批改车接口直接给前端
        /// </summary>
        public string ForceCheckNo { get; set; }
        /// <summary>
        /// add by qdk 2018-11-09
        /// 商业险批单号
        /// 接收批改车接口直接给前端
        /// </summary>
        public string BizCheckNo { get; set; }

        private List<CorrectCustomerViewModel> _cList = new List<CorrectCustomerViewModel>();

        /// <summary>
        /// 重复数据
        /// </summary>
        public List<CorrectCustomerViewModel> CList
        {
            get { return _cList; }
            set
            {
                if (value != null)
                {
                    _cList = value;
                }
            }
        }


    }
    public class CorrectCustomerViewModel
    {
        /// <summary>
        /// bx_userinfo.id
        /// </summary>
        public long Buid { get; set; }
        /// <summary>
        /// dz_correct_license_plate.id
        /// </summary>
        //public int CId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 客户电话
        /// </summary>
        public string ClientMobile { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }

        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public string LastReviewTime { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 该数据是否删除：主要用于前端勾选删除的数据
        /// 1:删除，2不删除
        /// </summary>
        public int IsDeleted { get; set; }
    }
}
