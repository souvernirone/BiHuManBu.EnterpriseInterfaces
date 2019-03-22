using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Repository.AppDataContextStorage
{
    public interface IDataContextStorageContainer
    {
        EntityContext GetDataContext();
        void Store(EntityContext libraryDataContext);
    }
}
