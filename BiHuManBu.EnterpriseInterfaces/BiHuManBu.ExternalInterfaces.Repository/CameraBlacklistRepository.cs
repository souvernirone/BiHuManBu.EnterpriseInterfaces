using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CameraBlacklistRepository : ICameraBlacklistRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        /// <summary>
        /// 新增摄像头进店黑名单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int AddCameraBlack(string sql)
        {
            var data = _dbHelper.ExecuteDataSet(sql);
            return 1;
        }

        /// <summary>
        /// 获取摄像头进店黑名单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bx_camera_blacklist GetCameraBlack(CameraBlackRequest request)
        {
            bx_camera_blacklist response = DataContextFactory.GetDataContext().bx_camera_blacklist.Where(x => x.agent_id == request.ChildAgent && x.license_no == request.LicenseNo && x.IsDelete == 0).FirstOrDefault();
            return response;
        }

        /// <summary>
        /// 获取摄像头进店黑名单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int DeleteCameraBlack(DelCameraBlackRequest request)
        {
            bx_camera_blacklist response = DataContextFactory.GetDataContext().bx_camera_blacklist.Where(x => x.Id == request.CameraBlackId && request.ChildAgent == x.agent_id).FirstOrDefault();
            response.IsDelete = 1;
            return DataContextFactory.GetDataContext().SaveChanges();
        }


        /// <summary>
        /// 获取摄像头进店黑名单列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<CameraBlackModel> GetCameraBlackList(BaseRequest2 request)
        {
            var response = DataContextFactory.GetDataContext().bx_camera_blacklist.Where(x => x.agent_id == request.ChildAgent && x.IsDelete == 0).ToList();
            var list = new List<CameraBlackModel>();
            foreach (var item in response)
            {
                list.Add(new CameraBlackModel() { Id = item.Id, LicenseNo = item.license_no, Status = "select" });
            }
            return list;
        }
    }
}
