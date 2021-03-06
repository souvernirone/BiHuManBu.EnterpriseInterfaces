﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.Redis;
using StackExchange.Redis;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper
{
    public class RedisOperatorBase //: IDisposable
    {
        private IRedis Redis { get; set; }
        //protected IRedisClient Redis { get; private set; }
        private bool _disposed = false;
        protected RedisOperatorBase()
        {
            Redis = RedisManager.GetClient(0);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //Redis.Dispose();
                    Redis = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        //public void Save()
        //{
        //    Redis.Save();
        //}
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        //public void SaveAsync()
        //{
        //    Redis.SaveAsync();
        //}
    }
}
