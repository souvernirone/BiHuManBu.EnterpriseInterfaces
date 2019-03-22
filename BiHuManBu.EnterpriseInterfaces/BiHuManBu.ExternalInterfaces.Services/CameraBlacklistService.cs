using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class CameraBlacklistService : ICameraBlacklistService
    {

        private readonly ICameraBlacklistRepository _cameraBlacklistRepository;

        public CameraBlacklistService(ICameraBlacklistRepository cameraBlacklistRepository)
        {
            _cameraBlacklistRepository = cameraBlacklistRepository;
        }

        public List<CameraBlackModel> AddCameraBlack(CameraBlackListRequest request)
        {
            var sqladd = "INSERT INTO bx_camera_blacklist (license_no,agent_id,IsDelete,create_time,update_time) VALUES ";
            var sqldel = "UPDATE bx_camera_blacklist SET IsDelete = 1 WHERE Id IN ";
            var listLincenseNoAdd = new List<string>();
            var listLincenseNoDel = new List<string>();
            foreach (var item in request.ListCameraBlack)
            {
                if (item.Status == "add")
                {
                    listLincenseNoAdd.Add("('" + item.LicenseNo + "', " + request.ChildAgent + ", 0, NOW(), NOW())");
                }
                if (item.Status == "del")
                {
                    listLincenseNoDel.Add(item.Id.ToString());
                }
            }

            if (listLincenseNoAdd.Count == 0 && listLincenseNoDel.Count == 0)
            {

            }
            if (listLincenseNoAdd.Count > 0)
            {
                sqladd = sqladd + string.Join(",", listLincenseNoAdd) + ";";
            }
            else
            {
                sqladd = "";
            }
            if (listLincenseNoDel.Count > 0)
            {
                sqldel = sqldel + "(" + string.Join(",", listLincenseNoDel) + ");";
            }
            else
            {
                sqldel = "";
            }
            var sql = sqladd + sqldel;
            if (!string.IsNullOrEmpty(sql))
            {
                _cameraBlacklistRepository.AddCameraBlack(sql);
            }
            return GetCameraBlackList(new BaseRequest2() { Agent = request.Agent, ChildAgent = request.ChildAgent });
        }

        public int DeleteCameraBlack(DelCameraBlackRequest request)
        {
            return _cameraBlacklistRepository.DeleteCameraBlack(request);
        }

        public bx_camera_blacklist GetCameraBlack(CameraBlackRequest request)
        {
            return _cameraBlacklistRepository.GetCameraBlack(request);
        }

        public List<CameraBlackModel> GetCameraBlackList(BaseRequest2 request)
        {
            return _cameraBlacklistRepository.GetCameraBlackList(request);
        }
    }
}
