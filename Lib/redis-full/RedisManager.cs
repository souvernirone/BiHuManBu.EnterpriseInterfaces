using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BiHuManBu.Redis
{
    //IDatabase 返回的db对象是很轻量级别的，不需要被缓存起来，每次用每次拿即可 IDatabase 的所有方法都有同步和异步的实现。其中的异步实现都是可以await的。
    public class RedisManager
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static RedisConfig redisConfigInfo = RedisConfig.GetConfig();

        private static readonly object _lockerMaster = new object();
        private static readonly object _lockerAssistant = new object();

        #region 主写
        private static ConnectionMultiplexer _connmaster;
        public static ConnectionMultiplexer ManagerMaster
        {
            get
            {
                if (_connmaster == null || _connmaster.IsConnected == false)
                {
                    lock (_lockerMaster)
                    {
                        if (_connmaster != null && _connmaster.IsConnected) return _connmaster;

                        _connmaster = CreateManagerMaster();
                        return _connmaster;
                    }
                }

                return _connmaster;
            }
        }
        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static ConnectionMultiplexer CreateManagerMaster()
        {
            string connStr = "";
            if (!string.IsNullOrEmpty(redisConfigInfo.WriteServerList))
            {
                connStr += redisConfigInfo.WriteServerList;
            }
            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException("Redis配置的连接地址不能为空");
            }
            return ConnectionMultiplexer.Connect(connStr);
        }
        #endregion

        #region 副写
        private static ConnectionMultiplexer _connassistant;
        public static ConnectionMultiplexer ManagerAssistant
        {
            get
            {
                if (_connassistant == null || _connassistant.IsConnected == false)
                {
                    lock (_lockerAssistant)
                    {
                        if (_connassistant != null && _connassistant.IsConnected) return _connassistant;

                        _connassistant = CreateManagerAssistant();
                        return _connassistant;
                    }
                }

                return _connassistant;
            }
        }
        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static ConnectionMultiplexer CreateManagerAssistant()
        {
            //主副相同时，不再实例连接
            if (redisConfigInfo.WriteServerList == redisConfigInfo.ReadServerList)
            {
                return null;
            }

            string connStr = "";
            if (!string.IsNullOrEmpty(redisConfigInfo.ReadServerList))
            {
                connStr += redisConfigInfo.ReadServerList;
            }
            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException("Redis配置的连接地址不能为空");
            }
            return ConnectionMultiplexer.Connect(connStr);
        }
        #endregion

        #region 副写开关

        private const string _ConfigCenterUrl = "http://config.91bihu.com/api/config/get?type=%E7%8B%AC%E7%AB%8BKV&identifie=&key=OpenAssistant";
        private static DateTime _lastUpdateTime;
        /// <summary>
        /// 是否打开副写，1是 0否
        /// 默认打开
        /// </summary>
        private static int _openAssistant = 1;
        private static bool OpenAssistant
        {
            get
            {
                if ((DateTime.Now - _lastUpdateTime).Seconds >= 5)
                {
                    ReBuildConfig();
                    _lastUpdateTime = DateTime.Now;
                }
                return _openAssistant == 1;
            }

        }
        private static void ReBuildConfig()
        {
            var value = ConfigHelper.ByHttp(_ConfigCenterUrl);
            var tryresult = int.TryParse(value, out _openAssistant);
            //如果获取开关失败，按单写处理
            if (!tryresult)
                return;
        }

        #endregion

        #region 私有工具方法

        private static string[] SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }

        private static string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private static byte[] Serialize(object o)
        {
            if (o == null) return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                var arrays = memoryStream.ToArray();
                return arrays;
            }
        }

        #endregion

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IDatabase GetClient(long? db = -1)
        {
            IDatabase database;
            if (db < 0)
                database = ManagerMaster.GetDatabase((int)redisConfigInfo.DefaultDb);
            else
                database = ManagerMaster.GetDatabase((int)db);

            return database;
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IDatabase GetClientAssistant(long? db = -1)
        {
            IDatabase database;
            if (db < 0)
                database = ManagerAssistant.GetDatabase((int)redisConfigInfo.DefaultDb);
            else
                database = ManagerAssistant.GetDatabase((int)db);

            return database;
        }

        private static void WriteAssistant(Action writeAction)
        {
            if (OpenAssistant && ManagerAssistant != null && writeAction != null)
            {
                Task.Factory.StartNew(writeAction);
            }
        }

        public static bool Add<T>(string key, T objectValue, bool isSetExpireTime = true)
        {
            key = redisConfigInfo.KeySuffix + key;
            if (isSetExpireTime)
            {
                WriteAssistant(() =>
                {
                    ManagerAssistant.GetDatabase()
                       .StringSet(key, ConvertJson(objectValue), TimeSpan.FromMilliseconds(redisConfigInfo.LocalCacheTime));
                });
                return ManagerMaster.GetDatabase()
                    .StringSet(key, ConvertJson(objectValue), TimeSpan.FromMilliseconds(redisConfigInfo.LocalCacheTime));

            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase()
                   .StringSet(key, ConvertJson(objectValue));
            });
            return ManagerMaster.GetDatabase().StringSet(key, ConvertJson(objectValue));
        }

        public static bool Add<T>(string key, T objectValue, long lNumofSeconds = 0)
        {
            key = redisConfigInfo.KeySuffix + key;
            if (lNumofSeconds > 0)
            {
                WriteAssistant(() =>
                {
                    ManagerAssistant.GetDatabase()
                        .StringSet(key, ConvertJson(objectValue), TimeSpan.FromSeconds(lNumofSeconds));
                });
                return ManagerMaster.GetDatabase()
                    .StringSet(key, ConvertJson(objectValue), TimeSpan.FromSeconds(lNumofSeconds));
            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase()
                    .StringSet(key, ConvertJson(objectValue));
            });
            return ManagerMaster.GetDatabase().StringSet(key, ConvertJson(objectValue));
        }

        public static bool Set<T>(string key, T objectValue, bool isSetExpireTime = true)
        {
            key = redisConfigInfo.KeySuffix + key;
            if (isSetExpireTime)
            {
                WriteAssistant(() =>
                {
                    ManagerAssistant.GetDatabase().StringSet(key, ConvertJson(objectValue),
                        TimeSpan.FromMilliseconds(redisConfigInfo.LocalCacheTime));
                });
                return ManagerMaster.GetDatabase().StringSet(key, ConvertJson(objectValue),
                    TimeSpan.FromMilliseconds(redisConfigInfo.LocalCacheTime));
            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().StringSet(key, ConvertJson(objectValue));
            });
            return ManagerMaster.GetDatabase().StringSet(key, ConvertJson(objectValue));
        }

        public static bool Set<T>(string key, T objectValue, long lNumofSeconds = 0)
        {
            key = redisConfigInfo.KeySuffix + key;
            if (lNumofSeconds > 0)
            {
                WriteAssistant(() =>
                {
                    ManagerAssistant.GetDatabase()
                .StringSet(key, ConvertJson(objectValue), TimeSpan.FromSeconds(lNumofSeconds));
                });
                return ManagerMaster.GetDatabase()
                    .StringSet(key, ConvertJson(objectValue), TimeSpan.FromSeconds(lNumofSeconds));
            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().StringSet(key, ConvertJson(objectValue));
            });
            return ManagerMaster.GetDatabase().StringSet(key, ConvertJson(objectValue));
        }

        public static T Get<T>(string key)
        {
            key = redisConfigInfo.KeySuffix + key;
            var value = ManagerMaster.GetDatabase().StringGet(key);
            if (string.IsNullOrEmpty(value)) return default(T);

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static bool Remove(string key)
        {
            key = redisConfigInfo.KeySuffix + key;
            WriteAssistant(() =>
            {
                if (ManagerAssistant.GetDatabase().KeyExists(key))
                {
                    ManagerAssistant.GetDatabase().KeyDelete(key);
                }
            });
            if (ManagerMaster.GetDatabase().KeyExists(key))
            {
                return ManagerMaster.GetDatabase().KeyDelete(key);
            }
            return true;
        }

        public static bool Hash_Exist<T>(string key, string dataKey)
        {
            key = redisConfigInfo.KeySuffix + key;
            return ManagerMaster.GetDatabase().HashExists(key, dataKey);
        }

        public static void List_Add<T>(string key, T t)
        {
            key = redisConfigInfo.KeySuffix + key;
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().ListRightPush(key, ConvertJson(t));
            });
            ManagerMaster.GetDatabase().ListRightPush(key, ConvertJson(t));
        }

        public static bool List_Remove<T>(string key, T t)
        {
            key = redisConfigInfo.KeySuffix + key;
            WriteAssistant(() =>
            {
                if (ManagerAssistant.GetDatabase().KeyExists(key))
                {
                    ManagerAssistant.GetDatabase().ListRemove(key, ConvertJson(t));
                }
            });
            if (ManagerMaster.GetDatabase().KeyExists(key))
            {
                return ManagerMaster.GetDatabase().ListRemove(key, ConvertJson(t)) > 0;
            }
            return true;
        }

        public static void List_RemoveAll<T>(string key)
        {
            key = redisConfigInfo.KeySuffix + key;
            WriteAssistant(() =>
            {
                if (ManagerAssistant.GetDatabase().KeyExists(key))
                {
                    ManagerAssistant.GetDatabase().KeyDelete(key);
                }
            });
            if (ManagerMaster.GetDatabase().KeyExists(key))
            {
                ManagerMaster.GetDatabase().KeyDelete(key);
            }
        }

        public static long List_Count(string key)
        {
            key = redisConfigInfo.KeySuffix + key;
            return ManagerMaster.GetDatabase().ListLength(key);
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SetExpire(string key, DateTime datetime)
        {
            key = redisConfigInfo.KeySuffix + key;
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().KeyExpire(key, datetime);
            });
            ManagerMaster.GetDatabase().KeyExpire(key, datetime);
        }

        public static double GetExpire(string key)
        {
            key = redisConfigInfo.KeySuffix + key;
            var timeStamp = ManagerMaster.GetDatabase().KeyTimeToLive(key);
            if (timeStamp.HasValue)
            {
                return timeStamp.Value.TotalMilliseconds;
            }

            return 0;
        }

        #region qpp

        /// <summary>
        /// 进入hash表中
        /// </summary>
        public static bool SetEntryInHash(string hashId, string key, string value)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().HashSet(hashId, key, value);
            });
            return ManagerMaster.GetDatabase().HashSet(hashId, key, value);
        }

        /// <summary>
        /// 从hash表中移除
        /// </summary>
        public static bool RemoveEntryFromHash(string hashId, string key)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().HashDelete(hashId, key);
            });
            return ManagerMaster.GetDatabase().HashDelete(hashId, key);
        }

        /// <summary>
        /// 得到hash表中的单个键值
        /// </summary>
        public static string GetValueFromHash(string hashId, string key)
        {
            var value = ManagerMaster.GetDatabase().HashGet(hashId, key);
            if (!string.IsNullOrEmpty(value))
            {
                return ManagerMaster.GetDatabase().HashGet(hashId, key).ToString();
            }
            return null;
        }

        /// <summary>
        /// 获取hash的key集合
        /// </summary>
        public static List<string> GetHashKeys(string hashId)
        {
            var keys = ManagerMaster.GetDatabase().HashKeys(hashId);
            if (keys != null && keys.Length > 0)
            {
                return keys.Select(x => x.ToString()).ToList();
            }

            return null;
        }

        /// <summary>
        /// 获取hash的value集合
        /// </summary>
        public static List<string> GetHashValues(string hashId)
        {
            var values = ManagerMaster.GetDatabase().HashGetAll(hashId);
            if (values != null && values.Length > 0)
            {
                return values.Select(x => x.ToString()).ToList();
            }

            return null;
        }

        /// <summary>
        /// 得到hash表中的数量
        /// </summary>
        public static long GetHashCount(string hashId)
        {
            return ManagerMaster.GetDatabase().HashLength(hashId);
        }

        #endregion

        #region hjj

        /// <summary>
        /// 当存储的字符串是整数时，redis提供了一个实用的命令INCR，其作用是让当前键值递增，并返回递增后的值。如果key不存在，则自动会创建，如果存在自动+1
        /// </summary>
        public static long Increment(string key)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().StringIncrement(key);
            });
            return ManagerMaster.GetDatabase().StringIncrement(key);
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="lNumofMilliseconds"></param>
        /// <returns></returns>
        public static bool GetLock(string key, string token, long lNumofMilliseconds = 0)
        {
            if (lNumofMilliseconds > 0)
            {
                return ManagerMaster.GetDatabase()
                    .LockTake(key, ConvertJson(token), TimeSpan.FromMilliseconds(lNumofMilliseconds));
            }
            return ManagerMaster.GetDatabase()
                    .LockTake(key, ConvertJson(token), TimeSpan.FromMilliseconds(redisConfigInfo.LocalCacheTime));
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool RemoveLock(string key, string token)
        {
            return ManagerMaster.GetDatabase().LockRelease(key, token);
        }

        #endregion

        #region 集合

        /// <summary>
        /// 无序集合添加成员
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static bool SetAdd(string key, string member)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetAdd(key, member);
            });
            return ManagerMaster.GetDatabase().SetAdd(key, member);
        }

        /// <summary>
        /// 获取无序集合值的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>long</returns>
        public static long SetLength(string key)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetLength(key);
            });
            return ManagerMaster.GetDatabase().SetLength(key);
        }

        /// <summary>
        /// 判断无序集合是否包含指定值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <returns>bool</returns>
        public static bool SetContains(string key, string val)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetLength(key);
            });
            return ManagerMaster.GetDatabase().SetContains(key, val);
        }

        /// <summary>
        /// 无序集合随机获取一个成员
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>string</returns>
        public static string SetRandomMember(string key)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetRandomMember(key);
            });
            return ManagerMaster.GetDatabase().SetRandomMember(key);
        }

        /// <summary>
        /// 获取无序集合全部成员
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static List<string> SetMembers(string key)
        {
            return ManagerMaster.GetDatabase().SetMembers(key).Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// 无序集合删除指定的value
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">需要删除的值</param>
        /// <returns></returns>
        public static long SetRemove(string key, params string[] value)
        {
            var valueList = new RedisValue[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                valueList[i] = value[i];
            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetRemove(key, valueList);
            });
            return ManagerMaster.GetDatabase().SetRemove(key, valueList);
        }

        /// <summary>
        /// 无序集合随机弹出一个键，并删除该键，会将删除的值返回
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string SetPop(string key)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SetPop(key);
            });
            return ManagerMaster.GetDatabase().SetPop(key);
        }

        /// <summary>
        /// 扫描无序集合
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="pattern">模式，通配符</param>
        /// <param name="pagesize">页大小</param>
        /// <returns>List<string/></returns>
        public static List<string> SetScan(string key, string pattern, int pagesize)
        {
            var result = ManagerMaster.GetDatabase().SetScan(key, pattern, pagesize);
            return result.Select(x => x.ToString()).ToList();
        }

        #endregion 有序集合

        #region 有序集合

        /// <summary>
        /// 有序集合添加成员
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="member">成员</param>
        /// <param name="score">排序数字</param>
        /// <returns></returns>
        public static bool SortedSetAdd(string key, string member, double score)
        {
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SortedSetAdd(key, member, score);
            });
            return ManagerMaster.GetDatabase().SortedSetAdd(key, member, score);
        }
        /// <summary>
        /// 获取有序集合值的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>long</returns>
        public static long SortedSetLength(string key)
        {
            return ManagerMaster.GetDatabase().SortedSetLength(key);
        }

        /// <summary>
        /// 有序集合删除指定的value
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">需要删除的值</param>
        /// <returns></returns>
        public static long SortedSetRemove(string key, params string[] value)
        {
            var valueList = new RedisValue[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                valueList[i] = value[i];
            }
            WriteAssistant(() =>
            {
                ManagerAssistant.GetDatabase().SortedSetRemove(key, valueList);
            });
            return ManagerMaster.GetDatabase().SortedSetRemove(key, valueList);
        }

        /// <summary>
        ///  有序集合 获取从 start 开始的 stop 条数据包含Score
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">获取条数</param>
        /// <returns>List<string/></returns>
        public static List<string> SortedSetRangeByRank(string key, long start, long stop)
        {
            var result = ManagerMaster.GetDatabase().SortedSetRangeByRank(key, start, stop);
            return result.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// 有序集合 获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">开始索引</param>
        /// <param name="stop">获取条数</param>
        /// <param name="sort">默认：0升序，其他任意数据为倒序</param>
        /// <returns>List<string/></returns>
        public static List<string> SortedSetRangeByRankWithScores(string key, long start, long stop, int sort = 0)
        {
            var sortval = Order.Ascending;
            if (sort != 0)
                sortval = Order.Descending;
            var result = ManagerMaster.GetDatabase().SortedSetRangeByRankWithScores(key, start, stop, sortval);
            return result.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// 有序集合扫描
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="pattern">模式，通配符</param>
        /// <param name="pagesize">页大小</param>
        /// <returns>List<string/></returns>
        public static List<string> SortedSetScan(string key, string pattern, int pagesize)
        {
            var result = ManagerMaster.GetDatabase().SortedSetScan(key, pattern, pagesize);
            return result.Select(x => x.Element.ToString()).ToList();
        }

        #endregion 有序集合

    }
}
