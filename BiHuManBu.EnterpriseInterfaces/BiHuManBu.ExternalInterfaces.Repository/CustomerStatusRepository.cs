using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CustomerStatusRepository : ICustomerStatusRepository
    {
        readonly ILog _log;
        public CustomerStatusRepository()
        {
            _log = LogManager.GetLogger("ERROR");
        }


        /// <summary>
        /// 保存客户状态
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public int SaveCustomerStatus(bx_customerstatus customerStatusModel)
        {
            //判断是否存在
            var isown = DataContextFactory.GetDataContext().bx_customerstatus.Where(x => x.AgentId == customerStatusModel.AgentId && x.StatusInfo == customerStatusModel.StatusInfo && !x.Deleted).Select(x => x.Id).FirstOrDefault();
            if (isown == 0)
            {
                DataContextFactory.GetDataContext().bx_customerstatus.Add(customerStatusModel);
                DataContextFactory.GetDataContext().SaveChanges();
                int t_id = customerStatusModel.Id;
                string sql = string.Format("update bx_customerstatus set T_Id=" + t_id + " where Id={0} ", t_id);
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql.ToString());
                return t_id;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="CustomerStatus"></param>
        /// <returns></returns>
        public bool UpdateCustomerStatus(bx_customerstatus CustomerStatus)
        {
            DataContextFactory.GetDataContext().Set<bx_customerstatus>().Attach(CustomerStatus);
            DataContextFactory.GetDataContext().Entry<bx_customerstatus>(CustomerStatus).Property("StatusInfo").IsModified = true;
            //如果删除
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="CustomerStatus"></param>
        /// <param name="removeWay"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        public bool DeleteCustomerStatus(string strBuids, bx_customerstatus customerStatus, int removeWay, int moveTo)
        {
            DataContextFactory.GetDataContext().Set<bx_customerstatus>().Attach(customerStatus);
            DataContextFactory.GetDataContext().Entry<bx_customerstatus>(customerStatus).Property("Deleted").IsModified = true;
            string sql = string.Empty;
            Stopwatch sw = new Stopwatch();
            //校验
            if (customerStatus.Id == 0)
                return false;
            //如果删除
            if (customerStatus.Deleted)
            {

                // 1 直接删除
                if (removeWay == 1)
                {
                    try
                    {
                        sw.Start();
                        sql = string.Format("update bx_userinfo set UpdateTime=now(), IsReView=0 where agent IN (" + strBuids + ") and  IsReView={0} ", customerStatus.Id);
                        DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql.ToString());
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        _log.Error(string.Format("更新客户状态Sql：{0}；耗时：{1}", sql, sw.ElapsedMilliseconds));
                    }

                }
                // 2 转移数据
                else
                {
                    sql = string.Format("update bx_userinfo set UpdateTime=now(), IsReView=" + moveTo + " where agent IN (" + strBuids + ") and  IsReView={0} ", customerStatus.T_Id);
                    DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_customerstatus> GetCustomerStatus(int agentId, int t_Id, bool isDeleteData, bool isGetReView)
        {
            List<bx_customerstatus> lsbxcustomerca = new List<bx_customerstatus>();
            //查询全部
            if (isDeleteData)
            {
                if (t_Id == -1)
                {
                    lsbxcustomerca = DataContextFactory.GetDataContext().bx_customerstatus.Where(x => x.AgentId == agentId).ToList();
                }
                else
                {
                    lsbxcustomerca = DataContextFactory.GetDataContext().bx_customerstatus.Where(t => t.AgentId == agentId && t.T_Id == t_Id).ToList();
                }
            }
            else //查询未删除的数据
            {
                if (t_Id == -1)
                {
                    lsbxcustomerca = DataContextFactory.GetDataContext().bx_customerstatus.Where(x => x.AgentId == agentId && !x.Deleted).ToList();
                }
                else
                {
                    lsbxcustomerca = DataContextFactory.GetDataContext().bx_customerstatus.Where(t => t.AgentId == agentId && !t.Deleted && t.T_Id == t_Id).ToList();
                }
            }
            if (!isGetReView)
            {
                lsbxcustomerca = lsbxcustomerca.Where(x => x.IsSystemData > 0).ToList();
            }
            //返回
            //return lsbxcustomerca.OrderBy(x => x.T_Id).ToList();
            return lsbxcustomerca;
        }
        public bx_customerstatus GetCirculationCategoryRecord(int categoryId)
        {
            return DataContextFactory.GetDataContext().bx_customerstatus.FirstOrDefault(x => x.Id == categoryId);
        }

        public string GetCustomerStatusInfo(int agentId, int t_Id)
        {
            var customerStatusInfo = DataContextFactory.GetDataContext().bx_customerstatus.Where(t => t.AgentId == agentId && t.T_Id == t_Id).Select(x => x.StatusInfo).FirstOrDefault();
            return customerStatusInfo;
        }


        /// <summary>
        /// 刷库
        /// </summary>
        /// <returns></returns>
        public bool MakeCustomerStatus()
        {
            var isown = DataContextFactory.GetDataContext().bx_agent.Where(x => x.IsDaiLi == 1).Select(x => x.Id).ToList();
            foreach (var item in isown)
            {
                var A0 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "未回访",
                    CreateTime = DateTime.Now,
                    IsSystemData = 0,
                    Deleted = false,
                    T_Id = 0
                };
                var A1 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "忙碌中待联系",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 5
                };
                var A2 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "已报价考虑中（普通）",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 17
                };
                var A3 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "已报价考虑中（重点）",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 13
                };
                var A4 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "其他",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 14
                };

                var A5 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "预约到店",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 20
                };
                var A6 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "已预约出单",
                    CreateTime = DateTime.Now,
                    IsSystemData = 10001,
                    Deleted = false,
                    T_Id = 6
                };
                var A7 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "成功出单",
                    CreateTime = DateTime.Now,
                    IsSystemData = 9,
                    Deleted = false,
                    T_Id = 9
                };
                var A8 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "申请投保",
                    CreateTime = DateTime.Now,
                    IsSystemData = 21,
                    Deleted = false,
                    T_Id = 21
                };
                var A9 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "战败",
                    CreateTime = DateTime.Now,
                    IsSystemData = 4,
                    Deleted = false,
                    T_Id = 4
                };
                var A10 = new bx_customerstatus
                {

                    AgentId = item,
                    StatusInfo = "无效数据（停机、空号）",
                    CreateTime = DateTime.Now,
                    IsSystemData = 16,
                    Deleted = false,
                    T_Id = 16
                };
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A0);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A1);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A2);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A3);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A4);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A5);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A6);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A7);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A8);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A9);
                DataContextFactory.GetDataContext().bx_customerstatus.Add(A10);
            }
            return DataContextFactory.GetDataContext().SaveChanges() > 0 ? true : false;
        }

        /// <summary>
        /// 新代理
        /// </summary>
        /// <returns></returns>
        public bool NewDailiStatus(int agentid)
        {
            var item = agentid;
            var A0 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "未回访",
                CreateTime = DateTime.Now,
                IsSystemData = 0,
                Deleted = false,
                T_Id = 0
            };
            var A1 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "忙碌中待联系",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 5
            };
            var A2 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "已报价考虑中（普通）",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 17
            };
            var A3 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "已报价考虑中（重点）",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 13
            };
            var A4 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "其他",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 14
            };

            var A5 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "预约到店",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 20
            };
            var A6 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "已预约出单",
                CreateTime = DateTime.Now,
                IsSystemData = 10001,
                Deleted = false,
                T_Id = 6
            };
            var A7 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "成功出单",
                CreateTime = DateTime.Now,
                IsSystemData = 9,
                Deleted = false,
                T_Id = 9
            };
            var A8 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "申请投保",
                CreateTime = DateTime.Now,
                IsSystemData = 21,
                Deleted = false,
                T_Id = 21
            };
            var A9 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "战败",
                CreateTime = DateTime.Now,
                IsSystemData = 4,
                Deleted = false,
                T_Id = 4
            };
            var A10 = new bx_customerstatus
            {

                AgentId = item,
                StatusInfo = "无效数据（停机、空号）",
                CreateTime = DateTime.Now,
                IsSystemData = 16,
                Deleted = false,
                T_Id = 16
            };
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A0);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A1);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A2);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A3);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A4);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A5);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A6);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A7);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A8);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A9);
            DataContextFactory.GetDataContext().bx_customerstatus.Add(A10);
            return DataContextFactory.GetDataContext().SaveChanges() > 0 ? true : false;
        }


        public bool BatchUpdateCustomerStatusAndCategories(List<long> list, int status, int category)//改成参数化查询
        {

            string buidStr = string.Join(",", list);
            string sql = string.Empty;
            using (var _dbContext = new EntityContext())
            {
                if (status != -1)
                {
                    sql = string.Format(@"      
      UPDATE bx_userinfo SET bx_userinfo.IsReView={0},UpdateTime=now() WHERE bx_userinfo.Id IN ({1});", (int)status, buidStr);
                    //qidakang  2018-08-30 
                    //只有修改客户状态的时候才将计划回访时间删除。之前无论修改客户状态还是
                    //修改客户类别都将计划回访时间删除，导致计划回访标签里面没有该数据
                    sql += string.Format(@"
UPDATE bx_consumer_review SET bx_consumer_review.next_review_date=NULL  WHERE  bx_consumer_review.b_uid IN ({0}); ", buidStr);

                }
                if (category != -1)
                {
                    sql += string.Format(@"UPDATE  bx_userinfo set bx_userinfo.CategoryInfoId={0},UpdateTime=now() WHERE bx_userinfo.Id IN({1});"
                     , (int)category, buidStr);
                }
                if (sql == "")
                {
                    return false;
                }
                 
                return _dbContext.Database.ExecuteSqlCommand(sql) > 0;
            }



        }





    }
}
