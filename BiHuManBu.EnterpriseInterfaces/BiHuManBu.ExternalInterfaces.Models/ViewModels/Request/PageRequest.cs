using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 默认的分页请求串
    /// </summary>
    public class PageRequest : BaseRequest2, IPageRequest
    {
        private int _pageSize = 15;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _curPage = 1;
        public int CurPage
        {
            get { return _curPage; }
            set { _curPage = value; }
        }
    }
}
