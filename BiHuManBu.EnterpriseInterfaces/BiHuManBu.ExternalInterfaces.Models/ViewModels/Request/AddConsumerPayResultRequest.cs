using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 净费支付
    /// </summary>
    public class AddConsumerPayResultRequest
    {
        /// <summary>
        /// OrderNum
        /// </summary>
        [Required(ErrorMessage ="OrderNum不正确")]
        public string OrderNum { get; set; }

        [Range(0.01, int.MaxValue, ErrorMessage = "金额不正确")]
        public Decimal Money { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255,ErrorMessage ="备注不能超过255个字符")]
        public string Remarks { get; set; }

        private List<string> listImgs = new List<string>();
        /// <summary>
        /// 照片列表
        /// </summary>
        public List<string> ListImgs { get { return listImgs; } set { listImgs = value; } }

        /// <summary>
        /// 收款时间
        /// </summary>
        public DateTime PayDateTime { get; set; }

        /// <summary>
        /// 支付方式（-1默认值 0微信支付 1支付宝支付 2现金支付 3POS机刷卡 4银行卡转账 5支票支付）
        /// </summary>
        public int PayType { get; set; }
    }
}
