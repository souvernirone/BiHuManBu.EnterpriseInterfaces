using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
    /// <summary>
    /// 禁呼客户请求对象
    /// </summary>
    public class ForbidCallresult
    {

        private int _cityid = 8;
        /// <summary>
        /// 城市Id (选填)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "城市Id不能小于1")]
        public int CityId { get { return _cityid; } set { _cityid = value; } }

        private int _source = 2;
        /// <summary>
        /// 保险公司id0,1,2,3 平安，太平洋 人保，国寿财 (选填)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "保险公司Id不能小于0")]
        public int Source { get { return _source; } set { _source = value; } }

        /// <summary>
        /// 车牌号
        /// </summary>
        [Required(ErrorMessage = "licenseno不能为空")]
        public string Licenseno { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "CustKey不能为空")]
        public string CustKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "SecCode不能为空")]
        public string SecCode { get; set; }

        /// <summary>
        /// 当前操作业务员代理Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "业务员代理Id有误")]
        public int ChildAgent { get; set; }
    }
}
