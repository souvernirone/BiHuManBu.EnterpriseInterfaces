using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class UserExpandRequest
    {
        /// <summary>
        /// bx_userinfo_expand.Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Buid { get; set; }

        private int _isTempMobile = -1;

        /// <summary>
        /// 手机号
        /// </summary>
        public int IsTempMobile
        {
            get { return _isTempMobile; }
            set { _isTempMobile = value; }
        }

        private int _isTempEmail = -1;
        /// <summary>
        /// 邮箱
        /// </summary>
        public int IsTempEmail
        {
            get { return _isTempEmail; }
            set { _isTempEmail = value; }
        }

        private bool _isEdit = false;

        /// <summary>
        /// 是否修改
        /// </summary>
        public bool IsEdit
        {
            get { return _isEdit; }
            set { _isEdit = value; }
        }
    }
}
