using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper
{
    /// <summary>
    /// redis hash同步
    /// </summary>
    public class HashOperator : RedisOperatorBase, IHashOperator
    {
        public HashOperator() : base() { }
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool Exist<T>(string hashId, string key)
        {
            return Redis.RedisManager.Hash_Exist<T>(hashId, key);
            //return Redis.HashContainsEntry(hashId, key);
        }
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public bool Set<T>(string hashId, string key, T t)
        {
            var value = JsonSerializer.SerializeToString<T>(t);
            return Redis.RedisManager.SetEntryInHash(hashId, key, value);
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool Remove(string hashId, string key)
        {
            return Redis.RedisManager.RemoveEntryFromHash(hashId, key);
        }
        /// <summary>
        /// 移除整个hash
        /// </summary>
        public bool Remove(string key)
        {
            return Redis.RedisManager.Remove(key);
        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        public T Get<T>(string hashId, string key)
        {
            string value = Redis.RedisManager.GetValueFromHash(hashId, key);
            return JsonSerializer.DeserializeFromString<T>(value);
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        public List<T> GetAll<T>(string hashId)
        {
            var result = new List<T>();
            var list = Redis.RedisManager.GetHashValues(hashId);
            if (list != null && list.Count > 0)
            {
                list.ForEach(x =>
                {
                    var value = JsonSerializer.DeserializeFromString<T>(x);
                    result.Add(value);
                });
            }
            return result;
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public void SetExpire(string key, DateTime datetime)
        {
            Redis.RedisManager.SetExpire(key, datetime);
        }
    }
}
