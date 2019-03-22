using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAreaRepository
    {
        List<bx_area> Find();
        List<bx_area> FindByPid(int pid);
        List<bx_area> GetAll();
    }
}
