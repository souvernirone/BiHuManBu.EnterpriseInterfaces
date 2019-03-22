using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TempUserRepository : ITempUserRepository
    {
        /// <summary>
        /// 获取临时关系人信息
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="buId"></param>
        /// <param name="TempUserType"></param>
        /// <returns></returns>
        public List<bx_tempuserinfo> GetTempRelationAsync(int agentId, long buId, bool? TempUserType, int temptype)
        {
            //个人
            bx_tempuserinfo bx = new bx_tempuserinfo();
            //公户
            bx_tempuserinfo bx_1 = new bx_tempuserinfo();

            List<bx_tempuserinfo> lt = new List<bx_tempuserinfo>();
            if (agentId != -1)
            {
                if (TempUserType == null)
                {
                    //bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    //if (bx != null)
                    //{
                    //    lt.Add(bx);
                    //}
                    //bx_1 = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && !x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    //if (bx_1 != null)
                    //{
                    //    lt.Add(bx_1);
                    //}
                    //return lt;
                    string Getsql =string.Format(" SELECT s.* FROM (SELECT MAX(id) AS id FROM bx_tempuserinfo WHERE AgentId={0} AND Deleted=0 GROUP BY TempUserType ) t LEFT JOIN bx_tempuserinfo AS s ON t.id=s.id  ",agentId);
                 //string.Format("   SELECT * FROM (SELECT * FROM  bx_tempuserinfo  ORDER BY id DESC ) AS s WHERE s.AgentId={0} AND s.Deleted=0  GROUP BY s.TempUserType ", agentId);
                    return DataContextFactory.GetDataContext().bx_tempuserinfo.SqlQuery(Getsql).ToList();

                }
                else
                {
                    ////&& x.BuId != -1
                    //bool? _boolType = TempUserType;
                    //bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && x.TempUserType == _boolType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    //if (bx != null)
                    //{
                    //    lt.Add(bx);
                    // }

                    string Getsql = string.Format(" SELECT * FROM bx_tempuserinfo  WHERE  AgentId ={0} AND Deleted=0 AND TempUserType ={1}    ORDER  BY   UpdateTime DESC  LIMIT 1 ", agentId, TempUserType);
                    lt = DataContextFactory.GetDataContext().bx_tempuserinfo.SqlQuery(Getsql).ToList();
                    return lt;
                }

            }
            else
            {
                //var _selbybuid = (from _bxtiuser in DataContextFactory.GetDataContext().bx_temprelation_intermediate_userinfo
                //                  join _bxtemp in DataContextFactory.GetDataContext().bx_tempuserinfo
                //                  on _bxtiuser.TuId equals _bxtemp.Id
                //                  where _bxtiuser.BuId == buId && _bxtiuser.TempType == temptype
                //                  orderby _bxtiuser.Id
                //                  select new { _bxtiuser.TuId, _bxtiuser.Id, _bxtemp.TempUserType }).Distinct();

                //var _selbytuid = _selbybuid.OrderByDescending(x => x.Id).FirstOrDefault();
                //if (_selbytuid != null)
                //{
                //    bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.Id == _selbytuid.TuId).FirstOrDefault();
                //}
                string Getsql = string.Format(" SELECT _bxtemp.* FROM bx_temprelation_intermediate_userinfo _bxtiuser  INNER JOIN  bx_tempuserinfo  _bxtemp  ON _bxtemp.Id=_bxtiuser.TuId  WHERE _bxtiuser.BuId ={0} AND  _bxtiuser.TempType ={1}    ORDER  BY  _bxtiuser.Id  DESC  LIMIT 1 ", buId, temptype);
                return DataContextFactory.GetDataContext().bx_tempuserinfo.SqlQuery(Getsql).ToList();
                //if (bx != null)
                //{
                //    if (bx.Id > 0)
                //    {
                //        lt.Add(bx);
                //    }
                //}

            }
        }

        #region 注释的代码
        //public List<bx_tempuserinfo> GetTempUserInfoAsync(int agentId, long buId, bool? TempUserType)
        //{
        //        bx_tempuserinfo bx = new bx_tempuserinfo();
        //        List<bx_tempuserinfo> LSS = new List<bx_tempuserinfo>();
        //        List<bx_tempuserinfo> lt = new List<bx_tempuserinfo>();
        //        if (agentId != -1)
        //        {
        //            if (TempUserType == null)
        //            {

        //                //var asd = from a in DataContextFactory.GetDataContext().bx_tempuserinfo
        //                //          group new { a.AgentId, a.BuId, a.TempUserType, a.UpdateTime, a.TempUserName, a.TempIdCard, a.TempIdCardType, a.Deleted, a.CreateTime, a.Id }  or g.FirstOrDefault().UpdateTime by a.TempUserType  into g

        //                //          select new
        //                //          {
        //                //              AgentId = g.FirstOrDefault().AgentId,
        //                //              BuId = g.FirstOrDefault().BuId,
        //                //              UpdateTime = g.FirstOrDefault().UpdateTime,
        //                //              TempUserName = g.FirstOrDefault().TempUserName,
        //                //              TempIdCard = g.FirstOrDefault().TempIdCard,
        //                //              TempIdCardType = g.FirstOrDefault().TempIdCardType,
        //                //              Deleted = g.FirstOrDefault().Deleted,
        //                //              Id = g.FirstOrDefault().Id,
        //                //              TempUserType = g.FirstOrDefault().TempUserType
        //                //          };

        //                //var aaa = asd.OrderByDescending(x => x.UpdateTime);
        //                //var ss = aaa.Take(2).ToList();


        //                //  lt = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted).GroupBy(y => new { y.TempUserType }).SelectMany(x => x.OrderByDescending(z => z.UpdateTime).Take(2)).ToList() ;

        //                //  var wflist = from u in DataContextFactory.GetDataContext().bx_tempuserinfo
        //                //               where u.AgentId == agentId && !u.Deleted
        //                //               group new { TempUserType = u.TempUserType, UpdateTime = u.UpdateTime } by u.TempUserType into g

        //                //               select new { aa=, g.Key.UpdateTime };
        //                ////  wflist = wflist.OrderByDescending(x => x.UpdateTime);
        //                //  var hotWord = wflist.ToList().Take(2).ToList();
        //                //  var aa = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted).GroupBy(x => x.TempUserType).FirstOrDefault();
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && !x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                return lt;

        //            }
        //            else
        //            {
        //                bool? _boolType = TempUserType;
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.AgentId == agentId && !x.Deleted && x.TempUserType == _boolType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                return lt;
        //            }

        //        }
        //        else
        //        {
        //            if (TempUserType == null)
        //            {
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => !x.Deleted && x.BuId == buId && x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => !x.Deleted && x.BuId == buId && !x.TempUserType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                return lt;
        //            }
        //            else
        //            {
        //                bool? _boolType = TempUserType;
        //                bx = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.BuId == buId && !x.Deleted && x.TempUserType == _boolType).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        //                if (bx != null)
        //                {
        //                    lt.Add(bx);
        //                }
        //                return lt;
        //            }
        //        }
        //}

        //public async Task<bool> SaveTempUserInfoAsync(List<bx_tempuserinfo> tempUserInfo, bool isEdit)
        //{
        //        if (!isEdit)
        //        {
        //            foreach (var item in tempUserInfo)
        //            {
        //                DataContextFactory.GetDataContext().bx_tempuserinfo.Add(item);
        //            }

        //        }
        //        else
        //        {
        //            foreach (var item in tempUserInfo)
        //            {
        //                DataContextFactory.GetDataContext().Set<bx_tempuserinfo>().Attach(item);
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("AgentId").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("BuId").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("TempUserType").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("TempUserName").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("TempIdCardType").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("TempIdCard").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("UpdateTime").IsModified = true;
        //                DataContextFactory.GetDataContext().Entry<bx_tempuserinfo>(item).Property("Deleted").IsModified = true;
        //            }
        //        }
        //        return await Task.Run(() =>
        //        {
        //            return DataContextFactory.GetDataContext().SaveChanges() >= 0;
        //        });
        //}
        #endregion



        /// <summary>
        /// 临时关系人
        /// </summary>
        /// <param name="tempRelation"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        public bool SaveoldTempUser(List<bx_tempuserinfo> tempRelation)
        {
            foreach (var item in tempRelation)
            {
                if (DataContextFactory.GetDataContext().bx_temprelation_intermediate_userinfo.Where(x => x.BuId == item.BuId && x.TempType == 2).Select(x => x.Id).FirstOrDefault() <= 0)
                {
                    DataContextFactory.GetDataContext().bx_tempuserinfo.Add(item);
                    var bxInfo = new bx_temprelation_intermediate_userinfo();
                    bxInfo.BuId = Convert.ToInt32(item.BuId);
                    bxInfo.TuId = item.Id;
                    bxInfo.TempType = 2;
                    bxInfo.Deleted = false;
                    DataContextFactory.GetDataContext().bx_temprelation_intermediate_userinfo.Add(bxInfo);
                }
            }
            return DataContextFactory.GetDataContext().SaveChanges() > 0;
        }

        /// <summary>
        /// 临时关系人
        /// </summary>
        /// <param name="tempRelation"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        public bool SaveTempRelationAsync(List<bx_tempuserinfo> tempRelation, List<RelationDetailInfo> relation, int step)
        {
            //记录数
            int noteNum = 0;
            if (step == 1)
            {
                List<int> Tuid = new List<int>();
                foreach (var item in tempRelation)
                {
                    DataContextFactory.GetDataContext().bx_tempuserinfo.Add(item);
                }
            }
            else
            {
                
                foreach (var item in relation)
                {

                    var temptype = DataContextFactory.GetDataContext().bx_temprelation_intermediate_userinfo.Where(x => x.BuId == item.BuId && x.TempType == item.TempType).FirstOrDefault();

                    if (temptype != null)
                    {
                        if (temptype.TuId != item.TuId)
                        {
                            temptype.TuId = item.TuId;
                        }
                        else
                        {
                            //如果存在这种数
                            noteNum++;
                        }
                    }
                    else
                    {
                        var bxInfo = new bx_temprelation_intermediate_userinfo();
                        bxInfo.BuId = Convert.ToInt32(item.BuId);
                        bxInfo.TuId = item.TuId;
                        bxInfo.TempType = item.TempType;
                        bxInfo.Deleted = false;
                        DataContextFactory.GetDataContext().bx_temprelation_intermediate_userinfo.Add(bxInfo);
                    }
                }
            }
            if (noteNum > 0 || DataContextFactory.GetDataContext().SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<bx_tempinsuredinfo> GetOldData()
        {
            //   var aa=(from _oldData in DataContextFactory.GetDataContext().bx_tempinsuredinfo  )
            string sql = "SELECT * FROM (SELECT * FROM bx_tempinsuredinfo WHERE Deleted=FALSE  ORDER BY UpdateTime DESC) AS old GROUP BY old.BuId";
            return DataContextFactory.GetDataContext().bx_tempinsuredinfo.SqlQuery(sql).ToList();
            //  DataContextFactory.GetDataContext().bx_tempinsuredinfo.Where(x => !x.Deleted).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
        }

        public bool UpdateTempUserInfo(bx_tempuserinfo tempUserInfo, bool isEdit)
        {
            var selTempUserInfo = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.TempUserType == tempUserInfo.TempUserType && (x.BuId == tempUserInfo.BuId || x.AgentId == tempUserInfo.AgentId) && !x.Deleted).Select(x => x.Id).FirstOrDefault();
            if (selTempUserInfo > 0)
            {
                var updateTemp = DataContextFactory.GetDataContext().bx_tempuserinfo.Where(x => x.Id == selTempUserInfo).FirstOrDefault();
                updateTemp.Deleted = true;
                return DataContextFactory.GetDataContext().SaveChanges() >= 0;
            }
            return true;
        }
    }
}
