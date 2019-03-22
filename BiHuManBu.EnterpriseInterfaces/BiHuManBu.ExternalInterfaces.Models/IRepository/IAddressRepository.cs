using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAddressRepository
    {
        int Add(bx_address bxAddress);
        bx_address Find(int addressId, int userid=0);
        int Delete(int addressId, int userid);
        int Update(bx_address bxAddress);
        List<bx_address> FindByBuidAndAgentId(int userid,bool isGetDefaultAddress=false);//, int agentId
    }
}
