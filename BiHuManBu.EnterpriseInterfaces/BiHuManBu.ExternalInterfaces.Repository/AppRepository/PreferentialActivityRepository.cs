using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;




namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class PreferentialActivityRepository : AppIRepository.IPreferentialActivityRepository
    {
        private readonly IAgentRepository _agentRepository;
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRepository"></param>
        public PreferentialActivityRepository(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        /// <summary>
        /// 根据buid获取活动信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public List<bx_preferential_activity> GetActivityByBuid(long buid)
        {
            var result = from pa in DataContextFactory.GetDataContext().bx_preferential_activity
                         where pa.activity_status == 1 &&
                         (from ba in DataContextFactory.GetDataContext().bx_buid_activity
                          where ba.B_Uid == buid
                          select ba.activity_id).Contains(pa.id)
                         select pa;
            return result.ToList();
        }

        /// <summary>
        /// 根据传过来的id查询活动内容
        /// </summary>
        /// <param name="stringId"></param>
        /// <returns></returns>
        public List<bx_preferential_activity> GetActivityByIds(string stringId)
        {
            string[] strArray = stringId.Split(',');
            if (strArray.Length > 0)
            {
                int[] intArray = Array.ConvertAll<string, int>(strArray, int.Parse);
                var result = DataContextFactory.GetDataContext().bx_preferential_activity.Where(
                    i => intArray.Contains(i.id) && i.activity_status == 1)
                    .ToList().OrderByDescending(o => o.create_time);
                return result.ToList();
            }
            return new List<bx_preferential_activity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringId"></param>
        /// <returns></returns>
        public List<ResponesActivity> GetActivityByActivityIds(string stringId)
        {
            string sql = string.Format(@"SELECT Id,activity_content ActivityContent,'select' AS Status,IdEdit false  FROM bx_preferential_activity WHERE activity_status = 1 AND activity_type = 3 AND Id IN ( " + stringId + ")");
            var list = _dbHelper.ExecuteDataTable(sql).ToList<ResponesActivity>().ToList();
            return list;
        }

        /// <summary>
        /// 获取优惠活动通过活动类别
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ResponesActivity> GetActivityByType(BaseVerifyRequest request)
        {
            var agent = _agentRepository.GetAgent(request.ChildAgent);
            string agentids = request.ChildAgent + "," + request.Agent + "," + agent.ParentAgent;
            string sql = string.Format(@"SELECT Id,activity_content ActivityContent,'select' AS STATUS,FALSE AS IdEdit,agent_id AS IsMine FROM bx_preferential_activity WHERE activity_status = 1 AND activity_type = 3 AND top_agent_id = {0} AND agent_id IN ({1})  ORDER BY ID DESC", request.Agent, agentids);
            var list = _dbHelper.ExecuteDataTable(sql).ToList<ResponesActivity>().ToList();
            foreach (var responesActivity in list)
            {
                if (responesActivity.IsMine == request.ChildAgent)
                {
                    responesActivity.IsMine = 1;
                }
            }
            return list;
        }

        /// <summary>
        /// 通过当前代理人ID查询上次记录优惠活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponesActivity GetActivityByAgentId(BaseVerifyRequest request)
        {

            var activity = new ResponesActivity();
            string sqlActivity =
                string.Format(
                    "SELECT * FROM bx_crm_steps WHERE agent_id = {0} AND TYPE IN (2,9) ORDER BY create_time DESC LIMIT 0,1",
                    request.ChildAgent);
            bx_crm_steps preferentialActivity = _dbHelper.ExecuteDataTable(sqlActivity).ToList<bx_crm_steps>().ToList().FirstOrDefault();
            if (preferentialActivity == null)
            {
                return activity;
            }

            CrmTimeLineForSmsViewModel smsViewModel =
                CommonHelper.ToListT<CrmTimeLineForSmsViewModel>("[" + preferentialActivity.json_content + "]")
                    .FirstOrDefault();
            if (smsViewModel == null || smsViewModel.ActivityId == 0)
            {
                return activity;
            }

            string sql = string.Format(@"SELECT Id,activity_content ActivityContent,'select' AS STATUS,FALSE AS IdEdit,agent_id AS IsMine FROM bx_preferential_activity WHERE id = {0}", smsViewModel.ActivityId);
            activity = _dbHelper.ExecuteDataTable(sql).ToList<ResponesActivity>().ToList().FirstOrDefault();
            if (activity != null)
            {
                activity.IsMine = activity.IsMine == request.ChildAgent ? 1 : 0;
            }
            
            return activity;
        }

        /// <summary>
        /// 通过活动类别获取优惠活动列别分页
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ResponesActivity> GetActivityPageList(GetActivityPageListRequest request)
        {
            string sql = string.Format(@"SELECT Id,activity_content ActivityContent,'select' AS Status,IdEdit false  FROM bx_preferential_activity WHERE activity_status = 1 AND activity_type = 3 AND top_agent_id = {0} AND agent_id = {1} ORDER BY ID DESC LIMIT {2},{3}", request.Agent, request.ChildAgent, (request.CurPage - 1) * request.PageSize, request.PageSize);
            var list = _dbHelper.ExecuteDataTable(sql).ToList<ResponesActivity>().ToList();
            return list;
        }

        /// <summary>
        /// 新增编辑优惠活动集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<ResponesActivity> AddOrEditActivity(RequestActivityViewModel request)
        {
            var lit = new List<ResponesActivity>();
            foreach (var item in request.Activitys)
            {
                var modelActivity =
                       DataContextFactory.GetDataContext()
                           .bx_preferential_activity.FirstOrDefault(n => n.id == item.Id);
                var rModel = new ResponesActivity();
                if (item.Status == "select")
                {
                    if (modelActivity != null)
                    {
                        rModel.IsMine = modelActivity.agent_id == request.ChildAgent ? 1 : 0;
                    }
                    rModel.ActivityContent = item.ActivityContent;
                    rModel.IsEdit = 0;
                    rModel.Status = "select";
                    rModel.Id = item.Id;
                    lit.Add(rModel);
                }
                if (item.Status == "add")
                {
                    var model = new bx_preferential_activity();
                    model.top_agent_id = request.Agent;
                    model.agent_id = request.ChildAgent;
                    model.activity_type = 3;
                    model.activity_name = "优惠活动";
                    model.activity_content = item.ActivityContent;
                    model.activity_status = 1;
                    model.create_time = DateTime.Now;
                    model.create_name = request.ModifyName;
                    model.modify_time = DateTime.Now;
                    model.modify_name = request.ModifyName;
                    model = AddActivity(model);

                    rModel.ActivityContent = item.ActivityContent;
                    rModel.IsEdit = 0;
                    rModel.Status = "select";
                    rModel.Id = model.id;
                    rModel.IsMine = 1;
                    lit.Add(rModel);
                }
                if (item.Status == "del")
                {
                    string sql = string.Format(@"UPDATE bx_preferential_activity SET activity_status=0,modify_time='{1}',modify_name='{2}' WHERE id = {0} and agent_id = {3} and top_agent_id = {4};", item.Id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.ModifyName, request.ChildAgent, request.Agent);
                    bool result = _dbHelper.ExecuteNonQuery(sql) > 0 ? true : false;
                    if (!result)
                    {
                        if (modelActivity != null)
                        {
                            rModel.IsMine = modelActivity.agent_id == request.ChildAgent ? 1 : 0;
                        }
                        rModel.ActivityContent = item.ActivityContent;
                        rModel.IsEdit = 0;
                        rModel.Status = "select";
                        rModel.Id = item.Id;
                        lit.Add(rModel);
                    }
                }
                if (item.Status == "modify")
                {
                    
                    string sql = string.Format(@"UPDATE bx_preferential_activity SET activity_content='{5}',modify_time='{1}',modify_name='{2}' WHERE id = {0} and agent_id = {3} and top_agent_id = {4};", item.Id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.ModifyName, request.ChildAgent, request.Agent, item.ActivityContent);
                    if (modelActivity.agent_id == request.ChildAgent)
                    {
                        bool result = _dbHelper.ExecuteNonQuery(sql) > 0 ? true : false;
                    }

                    if (modelActivity != null)
                    {
                        rModel.IsMine = modelActivity.agent_id == request.ChildAgent ? 1 : 0;
                    }

                    rModel.ActivityContent = item.ActivityContent;
                    rModel.IsEdit = 0;
                    rModel.Status = "select";
                    rModel.Id = item.Id;
                    lit.Add(rModel);
                }
            }
            return lit;
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bx_preferential_activity AddActivity(bx_preferential_activity request)
        {
            bx_preferential_activity response = DataContextFactory.GetDataContext().bx_preferential_activity.Add(request);
            DataContextFactory.GetDataContext().SaveChanges();
            return response;
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel DelActivity(DelPreferentialActivityListRequest request)
        {
            if (string.IsNullOrEmpty(request.Ids))
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);


            var result = false;
            string sql = string.Format(@"UPDATE bx_preferential_activity SET activity_status=0,modify_time='{1}',modify_name='{2}' WHERE id IN({0}) and agent_id = {3} and top_agent_id = {4};", request.Ids, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.ModifyName, request.ChildAgent, request.Agent);
            result = _dbHelper.ExecuteNonQuery(sql) > 0 ? true : false;

            if (result)
            {
                return BaseViewModel.GetBaseViewModel(1, "删除成功");
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError, "删除失败");
            }
        }
    }
}
