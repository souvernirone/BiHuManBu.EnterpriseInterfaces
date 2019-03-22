using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public class ReWriteRelationInfoService : IReWriteRelationInfoService
    {
        private readonly IGetCenterValueService _getCenterValue;
        public ReWriteRelationInfoService(IGetCenterValueService getCenterValue)
        {
            _getCenterValue = getCenterValue;
        }

        public RenewalInfo ReWriteUserInfoService(RenewalInfo renewalInfo, int topAgentId)
        {
            string getvalue = _getCenterValue.GetValue("S_S_RenewalTask_version", "", "pa_unfilter_agents");
            RenewalInfo viewmodel = renewalInfo;
            getvalue = GetString(getvalue);
            if (!string.IsNullOrEmpty(getvalue) && !getvalue.Contains("," + topAgentId + ","))
            {
                viewmodel.CarInfo.OwnerIdCard = GetRelationInfo(viewmodel.CarInfo.OwnerIdCard);//车主
                viewmodel.CarInfo.LicenseOwner = GetRelationInfo(viewmodel.CarInfo.LicenseOwner);//车主
                viewmodel.PreRenewalInfo.InsuredName = GetRelationInfo(viewmodel.PreRenewalInfo.InsuredName);//被保
                viewmodel.PreRenewalInfo.HolderName = GetRelationInfo(viewmodel.PreRenewalInfo.HolderName);//投保
                viewmodel.PreRenewalInfo.InsuredMobile = GetRelationInfo(viewmodel.PreRenewalInfo.InsuredMobile);//被保
                viewmodel.PreRenewalInfo.InsuredIdCard = GetRelationInfo(viewmodel.PreRenewalInfo.InsuredIdCard);

            }
            return viewmodel;
        }

        /// <summary>
        /// 信息包含星号，就返回空
        /// </summary>
        /// <param name="strInfo"></param>
        /// <returns></returns>
        private string GetRelationInfo(string strInfo)
        {
            if (!string.IsNullOrEmpty(strInfo) && strInfo.Contains("*"))
            {
                return "";
            }
            return strInfo;
        }

        /// <summary>
        /// 截前后字符
        /// </summary>
        /// <param name="val">原字符串</param>
        /// <param name="c">要截取的字符</param>
        /// <returns></returns>
        private string GetString(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return "";
            }
            return val.Replace('[', ',').Replace(']', ',');
        }
    }
}
