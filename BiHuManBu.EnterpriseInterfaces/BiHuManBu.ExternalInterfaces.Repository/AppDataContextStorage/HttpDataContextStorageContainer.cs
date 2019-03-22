using System.Web;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Repository.AppDataContextStorage
{
    public class HttpDataContextStorageContainer : IDataContextStorageContainer 
    {
        private string _dataContextKey = "DataContext";
        
        public EntityContext GetDataContext()
        {
            EntityContext objectContext = null;
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                objectContext = (EntityContext)HttpContext.Current.Items[_dataContextKey];

            return objectContext;
        }

        public void Store(EntityContext libraryDataContext)
        {
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                HttpContext.Current.Items[_dataContextKey] = libraryDataContext;
            else
                HttpContext.Current.Items.Add(_dataContextKey, libraryDataContext);  
        }

    }
}
