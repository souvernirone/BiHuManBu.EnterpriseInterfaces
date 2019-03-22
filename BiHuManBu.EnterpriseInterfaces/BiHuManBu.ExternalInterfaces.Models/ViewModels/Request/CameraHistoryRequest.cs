using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class CameraHistoryRequest : BaseRequest2
    {
        /// <summary>
        /// 进店开始时间
        /// </summary>
        public DateTime CameraStartTime { get; set; }

        private DateTime _cameraEndTime;
        /// <summary>
        /// 进店结束时间
        /// </summary>
        public DateTime CameraEndTime
        {
            get { return _cameraEndTime; }
            set
            {
                _cameraEndTime = value;
                if (_cameraEndTime.Hour == 0 && _cameraEndTime.Minute == 0 && _cameraEndTime.Second == 0)
                {
                    var date = _cameraEndTime.ToShortDateString();
                    date += " 23:59:59";
                    _cameraEndTime = Convert.ToDateTime(date);
                }
            }
        }
    }

    /// <summary>
    /// 获取摄像头进店列表请求模型
    /// </summary>
    public class CameraHistoryListRequest : CameraHistoryRequest, IPageRequest
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

    public class CameraHistoryExportRequest : CameraHistoryRequest
    {
        /// <summary>
        /// 选中的记录id，用逗号分隔
        /// </summary>
        public string Ids { get; set; }
    }

    public class CameraHistoryCountRequest : CameraHistoryRequest
    {
        /// <summary>
        /// 选中的记录id，用逗号分隔
        /// </summary>
        public string Ids { get; set; }
    }

}
