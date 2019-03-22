using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System.Data.Entity;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CameraHistoryRepository : EfRepositoryBase<bx_camera_history>, ICameraHistoryRepository
    {
        public CameraHistoryRepository(DbContext context) : base(context)
        {
        }
    }
}
