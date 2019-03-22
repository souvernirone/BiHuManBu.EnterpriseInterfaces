using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ICameraConfigRepository
    {
        int Update(bx_camera_config model);
        List<bx_camera_config> Get(int agentId);
    }
}
