using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BiHuManBu.Redis;
using MemcachedProviders.Cache;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Caches
{
    public class CacheProvider
    {
        //public static bool Add(string strKey, object objValue)
        //{
        //    return DistCache.Add(strKey, objValue);
        //}
        /// <summary>
        /// 使用add方式增加缓存（已存在则无法更新）
        /// </summary>
        /// <param name="strKey">关键字</param>
        /// <param name="objValue">值</param>
        /// <param name="isSetExpireTime">true：过期时间默认为配置节defaultExpireSeconds；false：永不过期</param>
        /// <returns></returns>
        public static bool Add(string strKey, object objValue, bool isSetExpireTime = true, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Add(strKey, objValue, isSetExpireTime);
            }
            else
            {
                return RedisManager.Add<object>(strKey, objValue, isSetExpireTime);
            }
        }

        /// <summary>
        /// 使用add方式增加缓存（已存在则无法更新）
        /// </summary>
        /// <param name="strKey">关键字</param>
        /// <param name="objValue">值</param>
        /// <param name="lNumofSeconds">过期时间，单位秒</param>
        /// <returns></returns>
        public static bool Add(string strKey, object objValue, long lNumofSeconds, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Add(strKey, objValue, lNumofSeconds);
            }
            else
            {
                return RedisManager.Add<object>(strKey, objValue, lNumofSeconds);
            }
        }
        /// <summary>
        /// 使用set方式增加/更新缓存
        /// </summary>
        /// <param name="strKey">关键字</param>
        /// <param name="objValue">值</param>
        /// <param name="isSetExpireTime">true：过期时间默认为配置节defaultExpireSeconds；false：永不过期</param>
        /// <param name="cacheType">缓存类型，0=memcached，1=redis</param>
        /// <returns></returns>
        //public static bool Set(string strKey, object objValue, bool isSetExpireTime = true, int cacheType = 1)
        //{
        //    if (cacheType == 0)
        //    {
        //        return DistCache.Set(strKey, objValue, isSetExpireTime);
        //    }
        //    else
        //    {
        //        return RedisManager.Set<object>(strKey, objValue, isSetExpireTime);
        //    }
        //}
        public static bool Set<T>(string strKey, T objValue, bool isSetExpireTime = true, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Set(strKey, objValue, isSetExpireTime);
            }
            else
            {
                return RedisManager.Set<T>(strKey, objValue, isSetExpireTime);
            }
        }

        /// <summary>
        /// 使用set方式增加/更新缓存
        /// </summary>
        /// <param name="strKey">关键字</param>
        /// <param name="objValue">值</param>
        /// <param name="lNumofSeconds">过期时间，单位秒</param>
        /// <param name="cacheType">缓存类型，0=memcached，1=redis</param>
        /// <returns></returns>
        public static bool Set(string strKey, object objValue, long lNumofSeconds, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Set(strKey, objValue, lNumofSeconds);
            }
            else
            {
                return RedisManager.Set<object>(strKey, objValue, lNumofSeconds);
            }
        }

        public static long Decrement(string strKey, long lAmount)
        {
            return DistCache.Decrement(strKey, lAmount);
        }

        //public static IDictionary<string, object> Get(params string[] keys)
        //{
        //    return DistCache.Get(keys);
        //}

        public static T Get<T>(string strKey, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Get<T>(strKey);
            }
            else
            {
                return RedisManager.Get<T>(strKey);
            }
        }
        //public static object Get(string strKey, int cacheType = 1)
        //{
        //    if (cacheType == 0)
        //    {
        //        return DistCache.Get(strKey);
        //    }
        //    else
        //    {
        //        return RedisManager.Get<object>(strKey);
        //    }
        //}

        public static long Increment(string strKey, long lAmount)
        {
            return DistCache.Increment(strKey, lAmount);
        }

        public static object Remove(string strKey, int cacheType = 1)
        {
            if (cacheType == 0)
            {
                return DistCache.Remove(strKey);
            }
            else
            {
                return RedisManager.Remove(strKey);
            }
        }

        public static void RemoveAll()
        {
            DistCache.RemoveAll();
        }
        #region 缓存分组排序方法
        /// <summary>
        /// 重新排序，为每个item设置行号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKey"></param>
        /// <param name="list">需要设置行号的list</param>
        /// <param name="sortGroupCount">每组最大个数</param>
        /// <param name="isSetExpireTime"></param>
        /// <returns></returns>
        public static void ReSetSortNo<T>(string strKey, IEnumerable<T> list, int sortGroupCount, bool isSetExpireTime = true, int cacheType = 1)
        {
            string keyCount = string.Format("{0}_sort_groups", strKey);
            var obj = Get<int>(keyCount, cacheType);
            int groupCount = int.Parse(obj.ToString());
            for (int i = 0; i < groupCount; i++)
            {
                Remove(string.Format("{0}_sort_{1}", strKey, i), cacheType);
            }
            Remove(keyCount, cacheType);

            Dictionary<int, T> dic = new Dictionary<int, T>();
            int no = 0;
            int groupNum = 0;
            foreach (var item in list)
            {
                no++;
                if (no > sortGroupCount * (groupNum + 1))
                {
                    Set(string.Format("{0}_sort_{1}", strKey, groupNum), dic, isSetExpireTime, cacheType);
                    dic.Clear();
                    groupNum++;
                }
                dic.Add(no, item);
            }
            Set(string.Format("{0}_sort_{1}", strKey, groupNum), dic, isSetExpireTime, cacheType);
            Set(string.Format("{0}_sort_groups", strKey), groupNum + 1, isSetExpireTime, cacheType);
        }
        /// <summary>
        /// 获取item所在行号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKey"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetSortNo<T>(string strKey, T item, int sortGroupCount, int cacheType = 1)
        {
            var obj = Get<int>(string.Format("{0}_sort_groups", strKey), cacheType);
            int groupNum = int.Parse(obj.ToString());
            for (int i = 0; i < groupNum; i++)
            {
                Dictionary<int, T> dic = Get<Dictionary<int, T>>(string.Format("{0}_sort_{1}", strKey, i), cacheType);
                var keyValue = dic.SingleOrDefault(x => x.Value.Equals(item));
                if (keyValue.Value != null)
                {
                    return keyValue.Key;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取item所在行号，不存在时则插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKey"></param>
        /// <param name="item"></param>
        /// <param name="sortGroupCount"></param>
        /// <returns></returns>
        public static int GetSortNoAndSave<T>(string strKey, T item, int sortGroupCount, bool isSetExpireTime = true, int cacheType = 1)
        {
            var obj = Get<int>(string.Format("{0}_sort_groups", strKey), cacheType);
            int groupNum = obj == null ? 0 : int.Parse(obj.ToString());
            for (int i = 0; i < groupNum; i++)
            {
                Dictionary<int, T> dic = Get<Dictionary<int, T>>(string.Format("{0}_sort_{1}", strKey, i), cacheType);
                var keyValue = dic.SingleOrDefault(x => x.Value.Equals(item));
                if (keyValue.Value != null && keyValue.Key > 0)
                {
                    return keyValue.Key;
                }
            }
            return InsertSortNo(strKey, item, sortGroupCount, isSetExpireTime, cacheType);
        }
        /// <summary>
        /// 插入新行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKey"></param>
        /// <param name="item"></param>
        /// <param name="sortGroupCount"></param>
        /// <param name="isSetExpireTime"></param>
        /// <returns></returns>
        private static int InsertSortNo<T>(string strKey, T item, int sortGroupCount, bool isSetExpireTime = true, int cacheType = 1)
        {
            var obj = Get<int>(string.Format("{0}_sort_groups", strKey), cacheType);
            int groupNum = obj == null ? 0 : int.Parse(obj.ToString());
            Dictionary<int, T> dic = Get<Dictionary<int, T>>(string.Format("{0}_sort_{1}", strKey, groupNum - 1), cacheType);
            if (dic != null)
            {
                int current = dic.Keys.Max();
                if (dic.Count < sortGroupCount)
                {
                    dic.Add(current + 1, item);
                }
                else
                {
                    dic.Clear();
                    dic.Add(current + 1, item);
                    groupNum++;
                    Set(string.Format("{0}_sort_groups", strKey), groupNum, isSetExpireTime, cacheType);
                }
                Set(string.Format("{0}_sort_{1}", strKey, groupNum - 1), dic, isSetExpireTime, cacheType);
                return current + 1;
            }
            return 0;
        }
        /// <summary>
        /// 对业务数据重新进行缓存分组设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="list"></param>
        /// <param name="cacheGroupCount">每组缓存的业务数据个数</param>
        public static void CacheProviderReSet<T>(string cacheKey, List<T> list, int cacheGroupCount, bool isSetExpireTime = true, int cacheType = 1)
        {
            string keyCount = string.Format("{0}_groups", cacheKey);
            var obj = Get<int>(keyCount, cacheType);
            int groupCount = obj == null ? 0 : int.Parse(obj.ToString());
            for (int i = 0; i < groupCount; i++)
            {
                Remove(string.Format("{0}_{1}", cacheKey, i), cacheType);
            }
            Remove(keyCount, cacheType);

            List<T> itemGroup = new List<T>();
            int no = 0;
            int groupNum = 0;
            foreach (var item in list)
            {
                no++;
                if (no >= cacheGroupCount * (groupNum + 1))
                {
                    Set(string.Format("{0}_{1}", cacheKey, groupNum), itemGroup, isSetExpireTime, cacheType);
                    itemGroup.Clear();
                    groupNum++;
                }
                itemGroup.Add(item);
            }
            Set(string.Format("{0}_{1}", cacheKey, groupNum), itemGroup, isSetExpireTime, cacheType);
            Set(string.Format("{0}_groups", cacheKey), groupNum + 1, isSetExpireTime, cacheType);

        }
        /// <summary>
        /// 获取业务数据当前组号，如需扩展则更新业务数据组数
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="sortNo"></param>
        /// <returns></returns>
        public static int GetGroupNumAndSave(string cacheKey, int sortNo, int cacheGroupCount, bool isSetExpireTime = true, int cacheType = 1)
        {
            string keyCount = string.Format("{0}_groups", cacheKey);
            var obj = Get<int>(keyCount, cacheType);
            int groupCount = obj == null ? 0 : int.Parse(obj.ToString());
            int currGroupNum = sortNo / cacheGroupCount;
            if (currGroupNum >= groupCount)
            {
                Set(string.Format("{0}_groups", cacheKey), groupCount + 1, isSetExpireTime, cacheType);
            }
            return currGroupNum;
        }
        public static int CacheProviderGetCity<T>(string cacheKey, int groupNum, int sleepMinSec, int cacheType = 1)
        {
            //var groups = CacheProvider.Get(string.Format("{0}_groups", cacheKey));
            List<T> smallServiceList = new List<T>();
            int count = 0;
            for (int i = 0; i < groupNum; i++)
            {
                var cacheGroupKey = string.Format("{0}_{1}", cacheKey, i);
                var obj = Get<IEnumerable<T>>(cacheGroupKey, cacheType);
                if (obj != null)
                    count += obj.Count();
                Thread.Sleep(sleepMinSec);
            }
            return count;
        }
        public static List<T> CacheProviderGetCity<T>(string cacheKey, int cacheType = 1)
        {
            var groups = Get<int>(string.Format("{0}_groups", cacheKey), cacheType);
            int groupNum = groups == null ? 0 : int.Parse(groups.ToString());
            List<T> list = new List<T>();
            for (int i = 0; i < groupNum; i++)
            {
                var groupKey = string.Format("{0}_{1}", cacheKey, i);
                var obj = Get<IEnumerable<T>>(groupKey, cacheType);
                if (obj != null)
                    list.AddRange(obj.ToList());
            }
            if (list.Count == 0)
                list = null;
            return list;
        }
        #endregion
        //public static double GetExpire(string key)
        //{
        //    return RedisManager.GetExpire(key);
        //}
    }
}
