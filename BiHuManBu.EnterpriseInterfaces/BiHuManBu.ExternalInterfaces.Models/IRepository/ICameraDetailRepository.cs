using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ICameraDetailRepository : IRepositoryBase<bx_camera_detail>
    {
        /// <summary>
        /// 分页获取进店历史记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agent">绑定摄像头的代理人</param>
        /// <param name="createStartTime">进店开始时间</param>
        /// <param name="createEndTime">进店结束时间</param>
        /// <returns></returns>
        List<CameraDetailModel> GetPageList(int pageIndex, int pageSize, int agent, DateTime createStartTime, DateTime createEndTime);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="createStartTime"></param>
        /// <param name="createEndTime"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
        List<CameraDetailModel> GetList(int agent, DateTime createStartTime, DateTime createEndTime, List<int> listId);

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="createStartTime"></param>
        /// <param name="createEndTime"></param>
        /// <returns></returns>
        int GetCount(int agent, DateTime createStartTime, DateTime createEndTime);


    }
}
