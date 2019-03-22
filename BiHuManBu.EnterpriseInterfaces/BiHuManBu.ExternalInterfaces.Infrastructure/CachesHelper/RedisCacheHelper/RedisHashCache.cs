﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper
{
    //redis hash 不同步
    public class RedisHashCache : BaseCache
    {

        private readonly ConnectionMultiplexer _conn;
        public static readonly string sysRedisKey = ConfigurationManager.AppSettings["redisKey"] ?? "";
        private IDatabase _db;


        #region 构造函数

        public RedisHashCache(int dbNum = 0)
            : this(dbNum, null)
        {
        }

        public RedisHashCache(int dbNum, string readWriteHosts)
        {
            _conn =
                string.IsNullOrWhiteSpace(readWriteHosts) ?
                RedisConnectionManager.Instance :
                RedisConnectionManager.GetConnectionMultiplexer(readWriteHosts);
            _db = _conn.GetDatabase(dbNum);
        }
        #endregion 构造函数

        /// <summary>
        /// wyy add 2017/08/23 19:50
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public override T Get<T>(string key)
        {
            var redisValue = _db.StringGet(key);
            if (string.IsNullOrWhiteSpace(redisValue)) return default(T);
            return CacheCommon.ConvertObj<T>(redisValue);
        }


        public override T Get<T>(string key, string dataKey)
        {

            var redisValue = _db.HashGet(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey);
            return CacheCommon.ConvertObj<T>(redisValue);
        }

        public override bool Set<T>(string key, string dataKey, T value, TimeSpan? expiry = default(TimeSpan?))
        {
            var newValue = CacheCommon.ConvertJson<T>(value);
            return _db.HashSet(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey, newValue);
        }
        public override bool Remove(string key)
        {
            return _db.KeyExists(CacheCommon.AddSysCustomKey(sysRedisKey, key)) ? _db.KeyDelete(CacheCommon.AddSysCustomKey(sysRedisKey, key)) : false;
        }
        public override bool Remove(string key, string dataKey)
        {
            return KeyExists(key, dataKey) ? _db.HashDelete(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey) : false;

        }
        public override bool KeyExists(string key)
        {
            return _db.KeyExists(CacheCommon.AddSysCustomKey(sysRedisKey, key));
        }
        public override bool KeyExists(string key, string dataKey)
        {
            return _db.HashExists(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey);
        }

        public override async Task<T> GetAsync<T>(string key, string dataKey)
        {
            var redisValue = await _db.HashGetAsync(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey);
            if (string.IsNullOrWhiteSpace(redisValue)) return default(T);
            return CacheCommon.ConvertObj<T>(redisValue);
        }

        public override async Task<bool> SetAsync<T>(string key, string dataKey, T value, TimeSpan? expiry = default(TimeSpan?))
        {
            if (KeyExists(key)) Remove(key);
            var newValue = CacheCommon.ConvertJson<T>(value);
            return await _db.HashSetAsync(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey, newValue);
        }

        public override async Task<bool> RemoveAsync(string key, string dataKey)
        {
            return KeyExists(key) ? await _db.HashDeleteAsync(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey) : false;

        }

        public override async Task<bool> KeyExistsAsync(string key, string dataKey)
        {

            return await _db.HashExistsAsync(CacheCommon.AddSysCustomKey(sysRedisKey, key), dataKey);
        }
    }
}
