using System.Collections;
using System.Threading;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Repository.AppDataContextStorage
{
    public class ThreadDataContextStorageContainer : IDataContextStorageContainer 
    {    
        private static readonly Hashtable _libraryDataContexts = new Hashtable();

        public EntityContext GetDataContext()
        {
            EntityContext libraryDataContext = null;

            if (_libraryDataContexts.Contains(GetThreadName()))
                libraryDataContext = (EntityContext)_libraryDataContexts[GetThreadName()];           

            return libraryDataContext;
        }

        public void Store(EntityContext libraryDataContext)
        {
            if (_libraryDataContexts.Contains(GetThreadName()))
                _libraryDataContexts[GetThreadName()] = libraryDataContext;
            else
                _libraryDataContexts.Add(GetThreadName(), libraryDataContext);           
        }

        private static string GetThreadName()
        {
            return Thread.CurrentThread.Name??"adsada";
        }     
    }
}
