using System;
using System.Web;
using System.Web.Caching;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Caches
{
    public  class CacheHelper:ICacheHelper
    {
        public object Get(string cacheKey)
        {
            return HttpRuntime.Cache.Get(cacheKey);
        }

        public void Add(string cacheKey, object obj, int cacheMinute)
        {
            //LRU算法
            //CacheStrategy<string>.GetInstance(int.MaxValue).Add(cacheKey,obj.ToString());
            HttpRuntime.Cache.Insert(cacheKey, obj, null, DateTime.Now.AddMinutes(cacheMinute),Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public void Remove(string cacheKey)
        {
            HttpRuntime.Cache.Remove(cacheKey);
        }
    }
}
