using log4net;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper
{
    public interface IHashOperator
    {
        bool Exist<T>(string hashId, string key);
        bool Set<T>(string hashId, string key, T t);
        bool Remove(string hashId, string key);
        bool Remove(string key);
        T Get<T>(string hashId, string key);
        List<T> GetAll<T>(string hashId);
        void SetExpire(string key, DateTime datetime);
    }
}
