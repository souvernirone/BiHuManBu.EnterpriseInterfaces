using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using AppRequest=BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppViewModels= BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IAddressService
    {
        int Add(AppRequest.AddressRequest bxAddress);
        AppViewModels.AddressModel Find(int addressId, string openid);
        int Delete(int addressId, string openid);
        int Update(AppRequest.AddressRequest bxAddress);
        List<AppViewModels.AddressModel> FindByBuidAndAgentId(string openid, int agentId,bool  isGetDefaultAddress=false);
        int SetDefaultAddress(int addressId, string openId);
    }

}
