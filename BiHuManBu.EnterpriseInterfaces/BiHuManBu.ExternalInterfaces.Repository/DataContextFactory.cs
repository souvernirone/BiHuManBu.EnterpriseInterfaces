using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Repository.AppDataContextStorage;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class DataContextFactory
    {      
        public static EntityContext GetDataContext()
        {
            IDataContextStorageContainer _dataContextStorageContainer = DataContextStorageFactory.CreateStorageContainer();

            EntityContext libraryDataContext = _dataContextStorageContainer.GetDataContext();
            if (libraryDataContext == null)
            {
                libraryDataContext = new EntityContext();
                _dataContextStorageContainer.Store(libraryDataContext);
            }

            return libraryDataContext;            
        }       
    }
}
