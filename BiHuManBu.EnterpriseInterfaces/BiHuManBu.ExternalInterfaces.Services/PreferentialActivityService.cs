using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class PreferentialActivityService : IPreferentialActivityService
    {
        private readonly IPreferentialActivityRepository _preferentialActivityRepository;

        public PreferentialActivityService(IPreferentialActivityRepository preferentialActivityRepository)
        {
            _preferentialActivityRepository = preferentialActivityRepository;
        }

        public List<bx_preferential_activity> GetActivityByBuid(long buid)
        {
            return _preferentialActivityRepository.GetActivityByBuid(buid);
        }

        public List<bx_preferential_activity> GetActivityByIds(string stringId)
        {
            return _preferentialActivityRepository.GetActivityByIds(stringId);
        }

        public List<ResponesActivity> GetActivityByActivityIds(string stringId)
        {
            return _preferentialActivityRepository.GetActivityByActivityIds(stringId);
        }

        public bx_preferential_activity AddActivity(bx_preferential_activity request)
        {
            return _preferentialActivityRepository.AddActivity(request);
        }

        public List<ResponesActivity> AddOrEditActivity(RequestActivityViewModel request)
        {
            return _preferentialActivityRepository.AddOrEditActivity(request);
        }

        public List<ResponesActivity> GetActivityPageList(GetActivityPageListRequest request)
        {
            return _preferentialActivityRepository.GetActivityPageList(request);
        }

        public List<ResponesActivity> GetActivityByType(BaseVerifyRequest request)
        {
            return _preferentialActivityRepository.GetActivityByType(request);
        }

        public BaseViewModel DelActivity(DelPreferentialActivityListRequest request)
        {
            return _preferentialActivityRepository.DelActivity(request);
        }
    }
}
