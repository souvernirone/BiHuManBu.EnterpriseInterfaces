using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using log4net;
using MySql.Data.MySqlClient;
using System;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CameraRepository : ICameraRepository
    {
        private ILog logError;
        public CameraRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        #region  业务员设置

        /// <summary>
        /// 获取摄像头 业务员信息
        /// </summary>
        /// <param name="agentId">代理人Id</param>
        /// <param name="year">年份</param>
        /// <param name="mounth">月份</param>
        /// <param name="type">数据类型 1=已选业务员、2=全部业务员</param>
        /// <returns></returns>
        public List<SealmanViewModel> FindSealman(int agentId, int type, string mobile, string name, string agents)
        {
            #region 查询参数
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int32)
                };
            parameters[0].Value = agentId;
            #endregion
            var querySql = string.Empty;
            if (type == 1)
            {
                //返回已设置业务员  返回上班状态  上班中/请假中 
                querySql = string.Format(@" select S.UserId,a.AgentName as UserName,a.Mobile as Mobile,(CASE WHEN SL.Id>0 THEN '请假中' ELSE '上班中' END) workStatus   from bx_Salesman S LEFT JOIN bx_agent a on a.id=s.UserId  LEFT JOIN bx_Salesman_leave SL ON S.UserId=SL.UserId and SL.LeaveTime between '{0} 00:00:00' and '{0} 23:59:59'  where AgentId=@agentId", DateTime.Now.ToString("yyyy/MM/dd"));
            }
            else
            {
                //返回全部业务员  是否已添加
                //querySql = string.Format(@"select tAt.Id as UserId,tAt.AgentName as UserName,tAt.Mobile,tAt.role_name as roleName,CASE WHEN (select DISTINCT UserId from bx_Salesman as sman where sman.UserId=tAt.Id )>0 THEN 2 ELSE  0  END isAdd from  
                //                          (select a.Id,a.AgentName,a.Mobile,r.role_name from bx_agent a 
                //                           INNER JOIN  bx_agent_distributed as b on b.AgentId=a.id and  b.ParentAgentId=@agentId AND b.AgentType=0 and a.IsUsed=1 and LENGTH        (a.AgentAccount)>0 and b.Deteled=0
                //                           INNER JOIN manager_role_db as r ON r.id=a.ManagerRoleId
                //                           where a.id in ({0}) ) tAt where 1=1 ", agents);
                querySql = string.Format(@"select tAt.Id as UserId,tAt.AgentName as UserName,tAt.Mobile,tAt.role_name as roleName,CASE WHEN (select DISTINCT UserId from bx_Salesman as sman where sman.UserId =tAt.id AND sman.AgentId in ({0}) )>0 THEN 2 ELSE  0  END isAdd from  
                                          (select a.Id,a.AgentName,a.Mobile,r.role_name from bx_agent a 
                                           INNER JOIN  bx_agent_distributed as b on b.AgentId=a.id and  b.ParentAgentId in (@agentId) AND b.AgentType=0 and a.IsUsed=1 and LENGTH (a.AgentAccount)>0 and b.Deteled=0
                                           INNER JOIN manager_role_db as r ON r.id=a.ManagerRoleId ) tAt where 1=1 ", agents);

                if (!string.IsNullOrEmpty(mobile))
                    querySql += string.Format(" and  tAt.Mobile='{0}'", mobile);

                if (!string.IsNullOrEmpty(name))
                    querySql += string.Format(" and tAt.AgentName='{0}'", name);

                querySql = string.Format("{0} GROUP BY UserId", querySql);
            }
            return DataContextFactory.GetDataContext().Database.SqlQuery<SealmanViewModel>(querySql.ToString(), parameters.ToArray()).ToList();
        }

        public List<long> FindAgentIdBySealman(int agentId)
        {
            var querySql = string.Format(@" select AgentId from bx_Salesman group by AgentId");
            if (agentId > 0)
                querySql = string.Format(@" select UserId from bx_Salesman where  AgentId={0}", agentId);
            return DataContextFactory.GetDataContext().Database.SqlQuery<long>(querySql.ToString()).ToList();
        }

        public List<long> FindAgentIdByMenber(int agentId)
        {
            var querySql = string.Format(@" SELECT AgentId FROM bx_agent_distributed WHERE Deteled=0 AND AgentType = 5");
            if (agentId > 0)
                querySql = string.Format(@" SELECT AgentId FROM bx_agent_distributed WHERE ParentAgentId = {0} AND Deteled=0 AND AgentType = 5 ", agentId);
            return DataContextFactory.GetDataContext().Database.SqlQuery<long>(querySql.ToString()).ToList();
        }

        /// <summary>
        /// 查看是否已设置业务员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int isExistSealMan(int agentId)
        {
            #region 查询参数
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int32)
                };
            parameters[0].Value = agentId;
            #endregion
            var querySql = string.Format(@" select count(id) from bx_Salesman where AgentId=@agentId");
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(querySql.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        public int isExistSealMan(int agentId, int userId)
        {
            #region 查询参数
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentId", MySqlDbType.Int32),
                    new MySqlParameter("@userId", MySqlDbType.Int32)
                };
            parameters[0].Value = agentId;
            #endregion
            var querySql = string.Format(@" select count(id) from bx_Salesman where AgentId=@agentId and UserId=@userId");
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(querySql.ToString(), parameters.ToArray()).FirstOrDefault();
        }

        //<summary>
        //保存 新增业务员信息
        //</summary>
        //<param name="userId"></param>
        //<returns></returns>
        public string SaveSealman(int agentId, int userId, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@AgentId", MySqlDbType.Int32),
                               new MySqlParameter("@UserId", MySqlDbType.Int32),
                               new MySqlParameter("@createTime", MySqlDbType.DateTime)
                        };
                parameters[0].Value = agentId;
                parameters[1].Value = userId;
                parameters[2].Value = DateTime.Now;
                #endregion
                var insertSql = string.Format(@"insert into bx_Salesman(AgentId,UserId,CreateTime) values(@AgentId,@UserId,@createTime)");
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(insertSql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        public string DelSealman(int agentId, int userId)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@AgentId", MySqlDbType.Int32),
                               new MySqlParameter("@UserId", MySqlDbType.Int32)
                        };
                parameters[0].Value = agentId;
                parameters[1].Value = userId;
                #endregion
                var insertSql = string.Format(@"delete from bx_salesman where AgentId=@AgentId and UserId=@UserId");
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(insertSql, parameters);
            }
            catch (Exception e)
            {
                logError.Error("" + e);
                result = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除 业务员
        /// </summary>
        /// <param name="whereStr">删除的条件</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string DelSealman(string whereStr, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                var delSql = string.Format(@"delete from bx_salesman where 1=1 ");
                if (string.IsNullOrEmpty(whereStr))
                {
                    result = "清除业务员请假信息时缺少条件！";
                }
                else
                {
                    delSql += whereStr;
                    DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(delSql);
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取摄像头 业务员请假信息
        /// </summary>
        /// <param name="UserId">代理人Id</param>
        /// <param name="year">年份</param>
        /// <param name="mounth">月份</param>
        /// <returns></returns>
        public List<LeaveDate> FindSealmanLeave(int userId)
        {
            #region 查询参数
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@UserId", MySqlDbType.Int32)
                };
            parameters[0].Value = userId;
            #endregion

            var querySql = string.Format(@" select leaveTime as 'leave','modify' as status from bx_salesman_leave where UserId=@UserId ");
            return DataContextFactory.GetDataContext().Database.SqlQuery<LeaveDate>(querySql.ToString(), parameters.ToArray()).ToList();
        }

        /// <summary>
        /// 获取所有请假人的id
        /// </summary>
        /// <returns></returns>
        public List<long> FindSealmanLeave()
        {
            var querySql = string.Format(@" select UserId from bx_salesman_leave group by UserId");
            return DataContextFactory.GetDataContext().Database.SqlQuery<long>(querySql.ToString()).ToList();
        }

        /// <summary>
        /// 保存 业务员请假信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string SaveSealmanLeave(int userId, DateTime leave, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@UserId", MySqlDbType.Int32),
                               new MySqlParameter("@leaveTime", MySqlDbType.DateTime),
                               new MySqlParameter("@createTime", MySqlDbType.DateTime)
                        };
                parameters[0].Value = userId;
                parameters[1].Value = leave;
                parameters[2].Value = DateTime.Now;
                #endregion
                var insertSql = string.Format(@"insert into bx_salesman_leave(UserId,leaveTime,CreateTime) values(@UserId,@leaveTime,@createTime)");
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(insertSql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除业务员的请假信息
        /// </summary>
        /// <param name="whereStr"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string delSealmanLeave(string whereStr, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                var delSql = string.Format(@" delete from bx_salesman_leave where 1=1 ");
                if (string.IsNullOrEmpty(whereStr))
                {
                    result = "清除业务员请假信息时缺少条件！";
                }
                else
                {

                    delSql += whereStr;
                    DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(delSql);
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        #endregion

        #region 车型设置

        public List<carMold> FindCarModel(int agentId)
        {
            #region 参数
            MySqlParameter[] parameters =
                        {
                               new MySqlParameter("@agentId", MySqlDbType.Int32)
                        };
            parameters[0].Value = agentId;
            #endregion

            var querySql = string.Format("select Id as id,CarType as name,'modify' as status from bx_car_mold where IsDel=0 ");
            if (agentId > 0)
                querySql += " AND AgentId=@agentId";

            return DataContextFactory.GetDataContext().Database.SqlQuery<carMold>(querySql.ToString(), parameters.ToArray()).ToList();
        }

        public List<int> FindCarType()
        {
            var querySql = string.Format("select AgentId from bx_car_mold where IsDel=0 GROUP BY AgentId  ");
            return DataContextFactory.GetDataContext().Database.SqlQuery<int>(querySql.ToString()).ToList();
        }

        /// <summary>
        /// 保存车型过滤关键字
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="carModes"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string SaveCarModel(int agentId, string carModes, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                        {
                                   new MySqlParameter("@agentId", MySqlDbType.Int32),
                                   new MySqlParameter("@carType", MySqlDbType.VarChar),
                                   new MySqlParameter("@createTime", MySqlDbType.DateTime),
                                   new MySqlParameter("@isDel", MySqlDbType.Int32)
                            };
                parameters[0].Value = agentId;
                parameters[1].Value = carModes;
                parameters[2].Value = DateTime.Now;
                parameters[3].Value = 0;
                #endregion
                var insertSql = string.Format("insert into bx_car_mold (AgentId,CarType,CreateTime,IsDel) values(@agentId,@carType,@createTime,@isDel)");
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(insertSql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                result = "操作失败！";
            }
            return result;
        }


        /// <summary>
        /// 删除车型过滤关键字
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="carModes"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string delCarModel(int Id, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@Id", MySqlDbType.Int32)
                        };
                parameters[0].Value = Id;
                #endregion
                var delSql = "update bx_car_mold set IsDel=1  where Id=@Id";
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(delSql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                result = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 修改车型过滤关键字
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="carModes"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string ModifyCarModel(int agentId, string carModes, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@agentId", MySqlDbType.Int32),
                               new MySqlParameter("@carType", MySqlDbType.VarChar)
                        };
                parameters[0].Value = agentId;
                parameters[1].Value = carModes;
                #endregion
                var modifySql = "update bx_car_mold set CarType=@carType where AgentId=@agentId";
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(modifySql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                result = e.Message;
            }
            return result;
        }

        #endregion

        #region
        /// <summary>
        /// 检查是否是摄像头进店 是否分配
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="renewalType"></param>
        /// <returns>true=未分配 false=已分配</returns>
        public bool IsExistUser(int uid, int renewalType)
        {
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                    {
                               new MySqlParameter("@userId", MySqlDbType.Int32),
                               new MySqlParameter("@renewalType", MySqlDbType.VarChar)
                        };
                parameters[0].Value = uid;
                parameters[1].Value = renewalType;
                #endregion
                var insertSql = string.Format("select count(1) from  bx_userinfo  where Id=@userId AND RenewalType=@renewalType");
                if (DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(insertSql, parameters) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region  v1.2

        /// <summary>
        /// 从回收站 撤销
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RevokeFiles(int userId)
        {
            var result = false;
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                        {
                                   new MySqlParameter("@userId", MySqlDbType.VarChar)
                            };
                parameters[0].Value = userId;
                #endregion
                var modifySql = string.Format("update bx_userinfo set isTest=0,UpdateTime=now() where Id=@userId");
                var effectRow = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(modifySql, parameters);
                result = effectRow > 0;
                #region bygpj20180209将此处逻辑删除，并改到service层，并且调用东亮的恢复批续数据功能
                //if (result)
                //{
                //    new BatchRenewalRepository().RevokeDelete(userId);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// isTest=3(回收站) isTest=1(删除) 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isTest">isTest=1是删除、isTest=3 进回收站</param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public string Remove(int userId, int isTest, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                #region 参数
                MySqlParameter[] parameters =
                             {
                                   new MySqlParameter("@userId", MySqlDbType.Int32),
                                   new MySqlParameter("@IsTest", MySqlDbType.Int32)
                                 };
                parameters[0].Value = userId;
                parameters[1].Value = isTest;
                #endregion
                var modifySql = string.Format("update bx_userinfo set UpdateTime=now(),IsTest=@IsTest where Id=@userId");
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(modifySql, parameters);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                result = "操作失败！";
            }
            return result;
        }

        public string RemoveList(string userIds, int isTest, ref bool isSuccess)
        {
            var result = "操作成功！";
            try
            {
                var modifySql = string.Format("update bx_userinfo set UpdateTime=now(), IsTest={0} where Id in ({1})",isTest,userIds);
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(modifySql);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                result = "操作失败！";
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取该代理人的顶级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int GetToAgentIdByAgentId(int agentId)
        {
            #region 参数
            MySqlParameter[] parameters =
                        {
                               new MySqlParameter("@agentId", MySqlDbType.Int32)
                        };
            parameters[0].Value = agentId;
            #endregion

            var querySql = string.Format("select TopAgentId from bx_agent where Id=@agentId");
            var data = DataContextFactory.GetDataContext().Database.SqlQuery<int>(querySql.ToString(), parameters.ToArray()).ToList();
            if (data.Count < 1)
                return agentId;

            return data[0];
        }

        /// <summary>
        /// 设置车型Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carModlId"></param>
        /// <returns></returns>
        public bool SetCarModelId(int userId, int carModlId)
        {
            #region 参数
            MySqlParameter[] parameters =
                        {
                               new MySqlParameter("@userId", MySqlDbType.Int32),
                               new MySqlParameter("@carModlId", MySqlDbType.Int32)
                        };
            parameters[0].Value = userId;
            parameters[1].Value = carModlId;
            #endregion

            var querySql = string.Format("update bx_userinfo set UpdateTime=now(), CarMoldId=@carModlId where Id=@userId");
            if (DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(querySql.ToString(), parameters.ToArray()) > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 执行批量增删 操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string sql)
        {
            var result = false;
            try
            {
                DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sql);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
        public List<GetCameraConfigByAgentViewModel> GetCameraConfigByTopAgent(int Agent)
        {
            var querySql = string.Format(@" SELECT config.id AS Id ,config.camera_id AS CameraId,config.park_id AS CameraAgentId,agent.AgentName AS CameraAgentName,config.cityid AS CityId ,
                                         config.seccode AS TopAgent,config.CreateTime AS CreateTime,config.IsFilteringOld ,config.IsDeleteFailed ,config.Days ,config.IsRemind ,config.IsRemind  ,config.CameraName 
                                         FROM bx_camera_config  config INNER JOIN  bx_agent  agent  ON config.park_id=agent.id where config.seccode ='" + Agent.ToString() + "'");
            return DataContextFactory.GetDataContext().Database.SqlQuery<GetCameraConfigByAgentViewModel>(querySql.ToString()).ToList();
        }
        public List<GetCameraConfigByAgentViewModel> GetCameraConfigByAgent(int Agent)
        {
            var querySql = string.Format(@" SELECT config.id AS Id ,config.camera_id AS CameraId,config.park_id AS CameraAgentId,agent.AgentName AS CameraAgentName,config.cityid AS CityId ,
                                         config.seccode AS TopAgent,config.CreateTime AS CreateTime,config.IsFilteringOld ,config.IsDeleteFailed ,config.Days ,config.IsRemind ,config.IsRemind  ,config.CameraName 
                                         FROM bx_camera_config  config INNER JOIN  bx_agent  agent  ON config.park_id=agent.id where config.park_id ='" + Agent.ToString() + "'");
            return DataContextFactory.GetDataContext().Database.SqlQuery<GetCameraConfigByAgentViewModel>(querySql.ToString()).ToList();
        }
    }
}
