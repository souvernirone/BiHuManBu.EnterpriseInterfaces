
using System.Web;

namespace BiHuManBu.ExternalInterfaces.Repository.AppDataContextStorage
{
    public class DataContextStorageFactory
    {
        public static IDataContextStorageContainer _dataContectStorageContainer;

        public static IDataContextStorageContainer CreateStorageContainer()
        {
            if (_dataContectStorageContainer == null)
            {
                if (HttpContext.Current == null)                                    
                    _dataContectStorageContainer = new ThreadDataContextStorageContainer();
                else
                    _dataContectStorageContainer = new HttpDataContextStorageContainer();
            }

            return _dataContectStorageContainer;
        }
    }
}
