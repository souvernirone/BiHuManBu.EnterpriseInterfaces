using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class CorrectCarRequest
    {
        /// <summary>
        /// 车牌号属于的代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ChildAgent参数错误")]
        public int ChildAgent { get; set; }
        /// <summary>
        /// dz_reconciliation.guid
        /// </summary>
        //[Range(1, int.MaxValue, ErrorMessage = "RecGuid参数错误")]
        public string RecGuid { get; set; }
    }
    public class CorrectRepeatRequest
    {
        /// <summary>
        /// 当前代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ChildAgent参数错误")]
        public int ChildAgent { get; set; }
        /// <summary>
        /// 顶级代理人
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }
        /// <summary>
        /// dz_correct_license_plate.id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 保单主键（商业险或者交强险）
        /// </summary>
        public int BDId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 是否再次：1第一次，2第二次
        /// </summary>
        public int IsAgain { get; set; }
        /// <summary>
        /// add by qdk 2018-11-09
        /// 直接传给批改车牌接口
        /// </summary>
        public int IsCorrectSource { get; set; }
       

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
}
