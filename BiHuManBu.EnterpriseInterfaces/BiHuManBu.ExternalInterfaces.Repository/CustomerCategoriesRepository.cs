using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CustomerCategoriesRepository : ICustomerCategoriesRepository
    {
        readonly ILog _log;
        public CustomerCategoriesRepository()
        {
            _log = LogManager.GetLogger("ERROR");
        }
        ///// <summary>
        ///// 保存客户类别
        ///// </summary>
        ///// <param name="customercategoriesModel"></param>
        ///// <returns></returns>
        //public bool SaveCustomerCategories(List<bx_customercategories> customercategoriesModel)
        //{
        //    using (var DataContextFactory.GetDataContext() = new EntityContext())
        //    {
        //        int InsertCount = customercategoriesModel.Count;
        //        int Count = 0;
        //        foreach (var item in customercategoriesModel)
        //        {
        //            //判断是否存在
        //            var isown = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == item.AgentId && x.CategoryInfo == item.CategoryInfo && !x.Deleted).Select(x => x.Id).FirstOrDefault();

        //            if (isown > 0)
        //            {
        //                Count++;
        //            }
        //            else
        //            {
        //                DataContextFactory.GetDataContext().bx_customercategories.Add(item);
        //                if (DataContextFactory.GetDataContext().SaveChanges() > 0)
        //                {
        //                    Count++;
        //                }
        //            }

        //        }
        //        return InsertCount == Count ? true : false;

        //    }
        //}

        /// <summary>
        /// 保存客户类别
        /// </summary>
        /// <param name="customercategoriesModel"></param>
        /// <returns></returns>
        public int SaveCustomerCategories(bx_customercategories customercategoriesModel)
        {
            //判断是否存在
            var isown = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == customercategoriesModel.AgentId && x.CategoryInfo == customercategoriesModel.CategoryInfo && !x.Deleted).Select(x => x.Id).FirstOrDefault();
            if (isown == 0)
            {
                DataContextFactory.GetDataContext().bx_customercategories.Add(customercategoriesModel);
                DataContextFactory.GetDataContext().SaveChanges();
                return customercategoriesModel.Id;
            }
            else
            {
                return 10002;
            }
        }


        /// <summary>
        /// 刷库
        /// </summary>
        /// <returns></returns>
        public bool MakeCustomerCategories()
        {
            List<bx_customercategories> bx_cu = new List<bx_customercategories>();
            var isown = DataContextFactory.GetDataContext().bx_agent.Where(x => x.IsDaiLi == 1).Select(x => x.Id).ToList();
            foreach (var item in isown)
            {
                //var A1 = new bx_customercategories
                //{

                //    AgentId = item,
                //    CategoryInfo = "新车",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                //var A2 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "在修不在保",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                //var A3 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "不在修不在保",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};

                //var A4 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "往年战败",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                //var A5 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "续保客户",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                var A6 = new bx_customercategories
                {
                    AgentId = item,
                    CategoryInfo = "新转续",
                    CreateTime = DateTime.Now,
                    DefeatTrans = 0,
                    Deleted = false,
                    IsStart = 1,
                    IssuingTrans = 0
                };
                var A7 = new bx_customercategories
                {
                    AgentId = item,
                    CategoryInfo = "潜转续",
                    CreateTime = DateTime.Now,
                    DefeatTrans = 0,
                    Deleted = false,
                    IsStart = 1,
                    IssuingTrans = 0
                };
                //var A8 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "间转续",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                var A9 = new bx_customercategories
                {
                    AgentId = item,
                    CategoryInfo = "续转续",
                    CreateTime = DateTime.Now,
                    DefeatTrans = 0,
                    Deleted = false,
                    IsStart = 1,
                    IssuingTrans = 0
                };
                //var A10 = new bx_customercategories
                //{
                //    AgentId = item,
                //    CategoryInfo = "三年联保",
                //    CreateTime = DateTime.Now,
                //    DefeatTrans = 0,
                //    Deleted = false,
                //    IsStart = 1,
                //    IssuingTrans = 0
                //};
                //DataContextFactory.GetDataContext().bx_customercategories.Add(A1);
                //DataContextFactory.GetDataContext().bx_customercategories.Add(A2);
                //DataContextFactory.GetDataContext().bx_customercategories.Add(A3);
                //DataContextFactory.GetDataContext().bx_customercategories.Add(A4);
                //DataContextFactory.GetDataContext().bx_customercategories.Add(A5);
                DataContextFactory.GetDataContext().bx_customercategories.Add(A6);
                DataContextFactory.GetDataContext().bx_customercategories.Add(A7);
              //  DataContextFactory.GetDataContext().bx_customercategories.Add(A8);
                DataContextFactory.GetDataContext().bx_customercategories.Add(A9);
              //  DataContextFactory.GetDataContext().bx_customercategories.Add(A10);

                DataContextFactory.GetDataContext().SaveChanges();

                //var UA1 = new bx_customercategories
                //{
                //    Id = A1.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A6.Id
                //};
                //var UA2 = new bx_customercategories
                //{
                //    Id = A2.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A7.Id
                //};
                //var UA3 = new bx_customercategories
                //{
                //    Id = A3.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A7.Id
                //};
                //var UA4 = new bx_customercategories
                //{
                //    Id = A4.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A8.Id
                //};
                //var UA5 = new bx_customercategories
                //{
                //    Id = A5.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A9.Id
                //};
                //var UA6 = new bx_customercategories
                //{
                //    Id = A6.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A9.Id
                //};
                //var UA7 = new bx_customercategories
                //{
                //    Id = A7.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A9.Id
                //};
                //var UA8 = new bx_customercategories
                //{
                //    Id = A8.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A9.Id
                //};
                //var UA9 = new bx_customercategories
                //{
                //    Id = A9.Id,
                //    DefeatTrans = A4.Id,
                //    IssuingTrans = A9.Id
                //};

                //Updatedata(UA1);
                //Updatedata(UA2);
                //Updatedata(UA3);
                //Updatedata(UA4);
                //Updatedata(UA5);
                //Updatedata(UA6);
                //Updatedata(UA7);
                //Updatedata(UA8);
                //Updatedata(UA9);
            }
            return DataContextFactory.GetDataContext().SaveChanges() > 0 ? true : false;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        public bool Updatedata(bx_customercategories customercategories)
        {
            DataContextFactory.GetDataContext().Set<bx_customercategories>().Attach(customercategories);
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("IssuingTrans").IsModified = true;
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("DefeatTrans").IsModified = true;
            //如果删除
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        public bool UpdateCustomerCategories(bx_customercategories customercategories)
        {
            DataContextFactory.GetDataContext().Set<bx_customercategories>().Attach(customercategories);
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("CategoryInfo").IsModified = true;
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("IssuingTrans").IsModified = true;
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("DefeatTrans").IsModified = true;
            //如果删除
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        public bool SetCustomerCategories(int agentId)
        {
            //设置
            var CustomerCategories = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == agentId && !x.Deleted).ToList();
            //循环设置
            foreach (var item in CustomerCategories)
            {
                item.IsStart = 1;
            }
            //如果删除
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="customercategories"></param>
        /// <param name="removeWay"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        public bool DeleteCustomerCategories(string strBuids, bx_customercategories customercategories, int removeWay, int moveTo)
        {
            DataContextFactory.GetDataContext().Set<bx_customercategories>().Attach(customercategories);
            DataContextFactory.GetDataContext().Entry<bx_customercategories>(customercategories).Property("Deleted").IsModified = true;
            string sql = string.Empty;
            Stopwatch sw = new Stopwatch();
            //校验
            if (customercategories.Id == 0)
                return false;
            //如果删除
            if (customercategories.Deleted)
            {
                //  string agentid = customercategories.AgentId.ToString();
                // 1 直接删除
                if (removeWay == 1)
                {
                    try
                    {
                        sw.Start();
                        //var _customer = DataContextFactory.GetDataContext().bx_userinfo.Where(x => x.CategoryInfoId == customercategories.Id).ToList();
                        sql = string.Format("update bx_userinfo set UpdateTime=now(), CategoryInfoId=0 where agent IN (" + strBuids + ") and CategoryInfoId={0} ", customercategories.Id);
                        DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql.ToString());
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        _log.Error(string.Format("更新客户类别Sql：{0}；耗时：{1}", sql, sw.ElapsedMilliseconds));
                    }
                    //出单后
                    var _customerown = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == customercategories.AgentId && x.IssuingTrans == customercategories.Id).ToList();
                    foreach (var item in _customerown)
                    {
                        item.IssuingTrans = 0;
                    }
                    //战败后
                    var _customerzbh = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == customercategories.AgentId && x.DefeatTrans == customercategories.Id).ToList();
                    foreach (var item in _customerzbh)
                    {
                        item.DefeatTrans = 0;
                    }
                }
                // 2 转移数据
                else
                {
                    //var _customer = DataContextFactory.GetDataContext().bx_userinfo.Where(x => x.CategoryInfoId == customercategories.Id).ToList();
                    //foreach (var item in _customer)
                    //{
                    //    item.CategoryInfoId = moveTo;
                    //}
                    sql = string.Format("update bx_userinfo set UpdateTime=now(), CategoryInfoId=" + moveTo + " where agent IN (" + strBuids + ") and CategoryInfoId={0} ", customercategories.Id);
                    DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql.ToString());
                }
                // DataContextFactory.GetDataContext().SaveChanges();
            }
            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<bx_customercategories> GetCustomerCategories(int agentId)
        {
            List<bx_customercategories> lsbxcustomerca = new List<bx_customercategories>();
            lsbxcustomerca = DataContextFactory.GetDataContext().bx_customercategories.Where(x => x.AgentId == agentId && !x.Deleted).ToList();
            return lsbxcustomerca;
        }
        public bx_customercategories GetCirculationCategoryRecord(int categoryId)
        {
            return DataContextFactory.GetDataContext().bx_customercategories.FirstOrDefault(x => x.Id == categoryId);
        }

        public List<bx_customercategories> GetCategoriesList(int agentId)
        {
            return DataContextFactory.GetDataContext().bx_customercategories.Where(t => t.AgentId == agentId && !t.Deleted).ToList();
        }


        public bool BatchUpdateCustomerCategories(List<long> list,bx_customercategories model)
        {
            string buidStr = string.Join(",", list);
            using (var _dbContext = new EntityContext())
            {
                string sql =string.Format( @"   
             UPDATE  bx_userinfo RIGHT JOIN bx_customercategories ON bx_userinfo.CategoryInfoId=bx_customercategories.Id   SET  bx_customercategories.CategoryInfo='{0}' WHERE bx_userinfo.Id IN({1})
             ",model.CategoryInfo, buidStr);
                return _dbContext.Database.ExecuteSqlCommand(sql) > 0;
            }
        }

      


    }
}
