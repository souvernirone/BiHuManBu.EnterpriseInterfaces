namespace BiHuManBu.ExternalInterfaces.Infrastructure.Caches
{
    public interface ICacheHelper
    {
        object Get(string cacheKey);
        void Add(string cacheKey, object obj, int cacheMinute);

        void Remove(string cacheKey);
    }
}
