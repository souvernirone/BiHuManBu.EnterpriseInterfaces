using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class UserinfoExpandRepository : IUserinfoExpandRepository
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private EntityContext _context;
        public UserinfoExpandRepository()
        {
            _context = DataContextFactory.GetDataContext();
        }
        /// <summary>
        /// 生成插入的sql脚本
        /// 因为插入的数据量大超过了max_allowed_packet，所以这里分批生成sql
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<string> GenerateInsertSql(List<bx_userinfo_expand> list, int scope = 2000)
        {
            var listSql = new List<string>();

            var listCount = list.Count;
            var loopCount = Math.Ceiling(listCount * 1.0 / scope);
            var now = DateTime.Now.ToString();
            for (int i = 0; i < loopCount; i++)
            {
                //is_temp_mobile = -1, is_temp_email = -1, update_time = DateTime.Now, delete_type = 1, delete_time = DateTime.Now
                StringBuilder builder = new StringBuilder();
                builder.Append("INSERT INTO bx_userinfo_expand (b_uid,is_temp_mobile,is_temp_email,update_time,delete_type,delete_time) VALUES ");
                for (int j = i * scope; j < ((i + 1) * scope) && j < listCount; j++)
                {
                    builder.Append(string.Format("({0},{1},{2},'{3}',{4},'{5}'),", list[j].b_uid, list[j].is_temp_mobile, list[j].is_temp_email, list[j].update_time.ToString("yyyy-MM-dd HH:mm:ss"), list[j].delete_type, list[j].delete_time.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                builder.Remove(builder.Length - 1, 1);
                listSql.Add(builder.ToString());
            }

            return listSql;
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddRangeBySql(List<bx_userinfo_expand> list)
        {
            var listSql = GenerateInsertSql(list);
            if (listSql.Count == 0)
                return true;
            // 执行sql
            foreach (var item in listSql)
            {
                _context.Database.ExecuteSqlCommand(item);
            }
            return true;
        }
        /// <summary>
        /// 根据buid批量更新扩展表
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public bool UpdateUserExpandByBuid(string buids, int DeleteType, DateTime DeleteTime)
        {
            try
            {
                var param = new[]
                                {
                                    new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.Int32,
                                        ParameterName = "delete_type",
                                        Value = DeleteType
                                    },
                                    new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.DateTime,
                                        ParameterName = "delete_time",
                                        Value = DeleteTime
                                    }
                                };
                string sql = "UPDATE bx_userinfo_expand SET delete_type=@delete_type,delete_time=@delete_time WHERE b_uid IN (" + buids + ");";
                return _context.Database.ExecuteSqlCommand(sql, param) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateUserExpandByBuid发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }

        /// <summary>
        /// 根据buid批量更新扩展表
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public bool UpdateUserExpandByBuid(List<long> buids, int DeleteType, DateTime DeleteTime)
        {
            if (buids==null||buids.Count==0)
            {
                return true;
            }
            try
            {
                var param = new[]
                                {
                                    new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.Int32,
                                        ParameterName = "delete_type",
                                        Value = DeleteType
                                    },
                                    new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.DateTime,
                                        ParameterName = "delete_time",
                                        Value = DeleteTime
                                    }
                                };
                bool result = false;
                foreach (string item in CutBuid(buids, 2500))
                {
                    string sql = "UPDATE bx_userinfo_expand SET delete_type=@delete_type,delete_time=@delete_time WHERE b_uid IN (" + item + ");";
                    result = _context.Database.ExecuteSqlCommand(sql, param) > 0;
                }
                return result;
                //string sql = "UPDATE bx_userinfo_expand SET delete_type=@delete_type,delete_time=@delete_time WHERE b_uid IN (" + buids + ");";
                //return _context.Database.ExecuteSqlCommand(sql, param) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateUserExpandByBuid:" +string.Join(",", buids) + "发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return false;
        }
        /// <summary>
        /// 截取buid集合
        /// </summary>
        /// <param name="listBuid">要截取的buid集合</param>
        /// <param name="tempNum">根据多长截取</param>
        /// <returns></returns>
        private List<string> CutBuid(List<long> listBuid, int tempNum)
        {
            List<string> cycleBuids = new List<string>();
            //if (listBuid.Count < 5000)
            if (listBuid.Count <= tempNum)
            {
                cycleBuids.Add(string.Join(",", listBuid));
                return cycleBuids;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listBuid.Count; i++)
            {
                sb.Append(listBuid[i]).Append(",");
                if ((i + 1) % tempNum == 0)
                {
                    cycleBuids.Add(sb.ToString().TrimEnd(','));
                    sb.Clear();
                    continue;
                }
                if ((i + 1) == listBuid.Count)
                {
                    cycleBuids.Add(sb.ToString().TrimEnd(','));
                    sb.Clear();
                }
            }
            return cycleBuids;
        }
        /// <summary>
        /// 根据buids获得存在的buid
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        public List<long> GetExistBuidList(string buids)
        {
            try
            {
                string sql = "SELECT b_uid FROM bx_userinfo_expand WHERE b_uid IN (" + buids + ")";
                return _context.Database.SqlQuery<long>(sql).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetExistBuidList发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }
        public async Task<int> Update(bx_userinfo_expand entity)
        {
            int result = 0;
            try
            {
                using (var _dbContext = new EntityContext())
                {
                    result = await Task.Run(() =>
                    {
                        _dbContext.bx_userinfo_expand.AddOrUpdate(entity);
                        return _dbContext.SaveChanges();
                    });
                }

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
        public async Task<bx_userinfo_expand> GetAsync(long buid)
        {
            bx_userinfo_expand expand = null;
            using (var _dbContext = new EntityContext())
            {
                try
                {
                    expand = await Task.Run(() =>
                    {
                        return _dbContext.bx_userinfo_expand.Where(ex => ex.b_uid == buid).FirstOrDefault();
                    });
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                return expand;
            }
        }
        public bx_userinfo_expand Get(long buid)
        {
            bx_userinfo_expand expand = null;
            using (var _dbContext = new EntityContext())
            {
                try
                {
                    return _dbContext.bx_userinfo_expand.Where(ex => ex.b_uid == buid).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
                return expand;
            }

        }
    }
}
