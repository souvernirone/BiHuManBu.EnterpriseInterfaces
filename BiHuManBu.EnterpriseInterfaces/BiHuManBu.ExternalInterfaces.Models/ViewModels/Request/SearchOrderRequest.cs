using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.Interface;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SearchOrderRequest : BaseOrderSearchRequest, IPageRequest
    {

        private int _fromMethod = 1;
        /// <summary>
        /// *平台来源 1crm2微信4app8对外
        /// </summary>
        [Range(1, 8)]
        public int FromMethod
        {
            get { return _fromMethod; }
            set { _fromMethod = value; }
        }
        public int CurPage { get; set; }

        public int PageSize { get; set; }

        public int? CarOwnerId { get; set; }
    }
}
