using System;
using ServiceStack.Redis;

namespace CacheMakerServiceDemo.RedisOperator
{
    /// <summary>
    /// RedisBase类，是redis操作的基类，继承自IDisposable接口，主要用于释放内存
    /// </summary>
    public abstract class RedisBase : IDisposable
    {
        public static IRedisClient Core { get; private set; }
        private bool _disposed = false;
        static RedisBase()
        {
            Core = RedisManager.GetClient();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Core.Dispose();
                    Core = null;
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
        public void Save()
        {
            Core.Save();
        }
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            Core.SaveAsync();
        }
    }
}


//static void Main(string[] args)
//        {
//            //清空数据库
//            DoRedisBase.Core.FlushAll();
//            //声明事务
//            using (var tran = RedisManager.GetClient().CreateTransaction())
//            {
//                try
//                {
//                    tran.QueueCommand(p =>
//                    {
//                        //操作redis数据命令
//                        DoRedisBase.Core.Set<int>("name", 30);
//                        long i = DoRedisBase.Core.IncrementValueBy("name", 1);
//                    });
//                    //提交事务
//                    tran.Commit();
//                }
//                catch
//                {
//                    //回滚事务
//                    tran.Rollback();
//                }
//                ////操作redis数据命令
//                //RedisManager.GetClient().Set<int>("zlh", 30);
//                ////声明锁，网页程序可获得锁效果
//                //using (RedisManager.GetClient().AcquireLock("zlh"))
//                //{
//                //    RedisManager.GetClient().Set<int>("zlh", 31);
//                //    Thread.Sleep(10000);
//                //}
//            }
//            Console.ReadKey();
//        }