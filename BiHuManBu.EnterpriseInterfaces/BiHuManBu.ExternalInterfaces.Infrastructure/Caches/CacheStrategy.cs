using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Caches
{
    public class CacheStrategy<TValue>:IEnumerable<KeyValuePair<string,TValue>>
    {
        private static CacheStrategy<TValue> instance; 
        private long ageToDiscard = 0;
        public long currentAge = 0;
        private int maxSize = 0;
        private readonly ConcurrentDictionary<string, TrackValue<TValue>> cache;

        public static CacheStrategy<TValue> GetInstance(int maxSize)
        {
            if (instance == null)
            {
                instance=new CacheStrategy<TValue>(maxSize);
            }
            return instance;
        } 
        public CacheStrategy(int maxKeySize)
        {
            cache=new ConcurrentDictionary<string, TrackValue<TValue>>();
            maxSize = maxKeySize;
        }

        public void Add(string key, TValue value)
        {
            Adjust(key);
            var result = new TrackValue<TValue>(this, value);
            cache.AddOrUpdate(key, result, (k, o) => result);
        }

        public void Adjust(string key)
        {
            while (cache.Count>=maxSize)
            {
                long agentToDelete = Interlocked.Increment(ref ageToDiscard);
                var toDiscard = cache.FirstOrDefault(p => p.Value.Age == agentToDelete);
                if (toDiscard.Key == null)
                {
                    continue;
                }
                TrackValue<TValue> old;
                cache.TryRemove(toDiscard.Key, out old);
            }
        }


        public TValue Get(string key)
        {
            TrackValue<TValue> value = null;
            if (cache.TryGetValue(key, out value))
            {
                value.Age = Interlocked.Increment(ref currentAge);
            }

            return value.Value;
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TrackValue<TValue>
    {
        public readonly TValue Value;
        public long Age;
        public TrackValue(CacheStrategy<TValue> lv,TValue tv)
        {
            Age = Interlocked.Increment(ref lv.currentAge);
            Value = tv;
        } 
    }
}
