using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models;
using log4net;
using MySql.Data.MySqlClient;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;

namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class AgentRepository : AppIRepository.IAgentRepository
    {
        private ILog logError;
        public AgentRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        public long Add(bx_agent agent)
        {
            long agentid = 0;
            try
            {
                var ag = DataContextFactory.GetDataContext().bx_agent.Add(agent);
                DataContextFactory.GetDataContext().SaveChanges();
                agentid = ag.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agentid;
        }

        public int Update(long agentId)
        {
            //bx_agent agent=new bx_agent();
            //agent = GetAgent(agentId);
            //agent.ParentShareCode = (agentId + 1000).ToString();
            //DataContextFactory.GetDataContext().bx_agent.AddOrUpdate(agent);
            long ParentShareCode = agentId + 1000;
            int intUpdate = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand("UPDATE bx_agent set ShareCode={0} WHERE Id={1}", ParentShareCode, agentId);
            //DataContextFactory.GetDataContext().SaveChanges();
            return intUpdate;
        }

        public int UpdateModel(bx_agent bxAgent)
        {
            int count = 0;
            try
            {
                DataContextFactory.GetDataContext().bx_agent.AddOrUpdate(bxAgent);
                count = DataContextFactory.GetDataContext().SaveChanges();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public bx_agent GetAgentByMobile(string mobile)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Mobile == mobile);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }
        //对外公开
        public bx_agent GetAgent(int agentId)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == agentId);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 查询当前agentid是否为顶级代理
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bx_agent GetAgentIsTop(int agentId)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.Id == agentId && x.ParentAgent == 0);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 获取代理，有问题
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bx_agent GetAgent(string openid)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.OpenId == openid);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 根据openId获取所有的代理
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public List<bx_agent> GetAllAgent(string openId)
        {
            var item = new List<bx_agent>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.OpenId == openId).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }
        public List<int> GetAgentIds(string openId)
        {
            var item = new List<int>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.OpenId == openId).Select(s => s.Id).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 根据mobile获取所有的代理
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public List<bx_agent> GetAllAgentByPhone(string mobile)
        {
            var item = new List<bx_agent>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.Mobile == mobile).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 根据上一级代理人Id获取代理人
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        public bx_agent GetAgentByParentAgent(string openid, int parentAgent)
        {
            var item = new bx_agent();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent.FirstOrDefault(x => x.OpenId == openid && x.ParentAgent == parentAgent);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }
        /// <summary>
        /// 根据顶级代理人Id获取代理人
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topParentAgent"></param>
        /// <returns></returns>
        public bx_agent GetAgentByTopParentAgent(string openId, int topParentAgent)
        {
            var listAgent = new List<bx_agent>();
            listAgent = GetAllAgent(openId);
            foreach (var item in listAgent)
            {
                if (GetTopAgentId(item.Id).Equals(topParentAgent.ToString()))//if (GetTopAgentId(item.Id).Equals(GetTopAgentId(topParentAgent)))
                {
                    return item;
                }
            }
            return null;
        }
        public int GetAgentId(string openId, string topAgent)
        {
            var listAgent = GetAgentIds(openId);
            if (listAgent.Any())
            {
                for (int i = 0; i < listAgent.Count; i++)
                {
                    if (GetTopAgentId(listAgent[i]).Equals(topAgent))
                    {
                        return listAgent[i];
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 根据手机号获取代理人
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topParentAgent"></param>
        /// <returns></returns>
        public bx_agent GetAgentByPhoneTopAgent(string mobile, int topParentAgent)
        {
            //List<bx_agent> listAgent = new List<bx_agent>();
            //listAgent = GetAllAgentByPhone(mobile);
            //foreach (var item in listAgent)
            //{
            //    if (GetTopAgentId(item.Id).Equals(GetTopAgentId(topParentAgent)))
            //    {
            //        return item;
            //    }
            //}
            //return null;
            
            var queryTopAgentId = int.Parse(GetTopAgentId(topParentAgent));//单个的topAgentId
            var agent = DataContextFactory.GetDataContext().bx_agent.Where(t => t.Mobile == mobile && t.TopAgentId == queryTopAgentId).FirstOrDefault();
            return agent;
        }

        /// <summary>
        /// 根据AgentId获取到所有子集的代理
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        public string GetSonId(int currentAgent)
        {
            MySqlParameter[] parms = new MySqlParameter[1];
            MySqlParameter parentAgentid = new MySqlParameter("parentAgentid", currentAgent);
            parentAgentid.MySqlDbType = MySqlDbType.Int32;
            parentAgentid.Size = 128;
            parms[0] = parentAgentid;
            var ts = DataContextFactory.GetDataContext().Database.SqlQuery<string>("select `getAgentChildList`(@parentAgentid)", parms).FirstOrDefault();
            return ts;
        }

        /// <summary>
        /// 根据AgentId获取所有的子集代理
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        public List<string> GetSonsList(int currentAgent, bool isContainSelf)
        {
            var listAgents = new List<string>();
            try
            {
                MySqlParameter[] parameters =
                {
                    new MySqlParameter("@curAgent", MySqlDbType.String)
                };
                parameters[0].Value = currentAgent;
                #region SQL语句

                var strSql = new StringBuilder();
                if (isContainSelf)
                {
                    strSql.Append("select @curAgent ")
                        .Append(" union all ");
                }
                strSql.Append(" select id from  bx_agent where parentagent=@curAgent ")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent
                            where parentagent in (select id from  bx_agent where parentagent=@curAgent ) ")
                    .Append(" union all ")
                    .Append(@" select id from bx_agent where  parentagent in (
                            select id from bx_agent where  parentagent in (
                            select id from  bx_agent where  parentagent=@curAgent) ) ");
                //#第五级代理
                //.Append(" union all ")
                //.Append(@" select id from bx_agent where  parentagent in (
                //select id from bx_agent where  parentagent in (
                //select id from bx_agent where  parentagent in (
                //select id from  bx_agent where  parentagent=@curAgent) ) ) ");  
                #endregion
                //查询列表
                listAgents = DataContextFactory.GetDataContext().Database.SqlQuery<string>(strSql.ToString(), parameters.ToArray()).ToList();

                return listAgents;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return new List<string>();
        }

        /// <summary>
        /// 查询子集，返回子集字符串 demo：'1','2','3'
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        public string GetSonsIdToString(int curAgent, bool isContainSelf)
        {
            var sbAgent = new StringBuilder();
            //获取子集代理
            var sonIds = GetSonsList(curAgent, isContainSelf);
            if (sonIds.Any())
            {
                foreach (var son in sonIds)
                {//拼接子集代理串
                    sbAgent.Append("'").Append(son).Append("'").Append(',');
                }
                sbAgent.Remove(sbAgent.Length - 1, 1);
            }
            return sbAgent.ToString();
        }
        /// <summary>
        /// 查询子集，返回子集字符串 demo：1,2,3
        /// </summary>
        /// <param name="curAgent"></param>
        /// <param name="isContainSelf">是否包含自己</param>
        /// <returns></returns>
        public string GetSonsIdToInt(int curAgent, bool isContainSelf)
        {
            var sbAgent = new StringBuilder();
            //获取子集代理
            var sonIds = GetSonsList(curAgent, isContainSelf);
            if (sonIds.Any())
            {
                foreach (var son in sonIds)
                {//拼接子集代理串
                    sbAgent.Append(son).Append(',');
                }
                sbAgent.Remove(sbAgent.Length - 1, 1);
            }
            return sbAgent.ToString();
        }

        /// <summary>
        /// 根据AgentId获取顶级代理
        /// </summary>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        public string GetTopAgentId(int currentAgent)
        {
            MySqlParameter[] parms = new MySqlParameter[1];
            var parentAgentid = new MySqlParameter("parentAgentid", currentAgent);
            parentAgentid.MySqlDbType = MySqlDbType.Int32;
            parentAgentid.Size = 128;
            parms[0] = parentAgentid;
            var ts = DataContextFactory.GetDataContext().Database.SqlQuery<string>("select `getAgentTopParent`(@parentAgentid)", parms).FirstOrDefault();
            return ts;
        }

        /// <summary>
        /// 获取父级代理列表
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        public IEnumerable<bx_agent> GetParentID(int parentAgent)
        {
            var query = from c in DataContextFactory.GetDataContext().bx_agent
                        where c.Id == parentAgent
                        select c;
            return query.ToList().Concat(query.ToList().SelectMany(t => GetParentID(t.ParentAgent)));
        }

        /// <summary>
        /// 获取下级代理人
        /// </summary>
        /// <param name="curAgentId"></param>
        /// <returns></returns>
        public IEnumerable<bx_agent> GetSonsAgent(int curAgentId)
        {
            var query = from c in DataContextFactory.GetDataContext().bx_agent
                        where c.ParentAgent == curAgentId
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonsAgent(t.Id)));
        }

        /// <summary>
        /// 根据当前人的ParentAgent获取其代理级别
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        public int GetAgentLevel(int parentAgent)
        {
            var query = GetParentID(parentAgent);
            string a = string.Empty;
            query.ToList().ForEach(q => a += q.ParentAgent + ",");
            return a.Split(',').Length;//此处需要-1(去掉分隔符后面的空字符)+1(增加当前的1级代理)=0，故省略
        }

        /// <summary>
        /// 获取当前及下级代理人
        /// </summary>
        /// <param name="bxAgent">二级或三级代理人对象</param>
        /// <returns></returns>
        public List<bx_agent> GetSonsAndSelfList(bx_agent bxAgent)
        {
            var item = new List<bx_agent>();
            try
            {
                item.Add(bxAgent);
                item = DataContextFactory.GetDataContext().bx_agent.Where(x => x.ParentAgent == bxAgent.Id).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        public List<bx_agent> GetAgentList(int curAgent, int topAgent, string search, int agentStatus, int curPage, int pageSize, out int totalCount)
        {
            var item = new List<bx_agent>();
            //不分页代理人的总条数
            totalCount = 0;
            try
            {
                //按照代理人名搜索
                //如果传值状态为-1，搜索全部状态；否则按照传来的状态搜索
                //存储过程调用子集
                string getSonId = GetSonId(curAgent);
                List<int> reSonId = new List<string>(getSonId.Split(',')).ConvertAll(i => int.Parse(i));
                reSonId.Remove(curAgent);

                var list1 = DataContextFactory.GetDataContext().bx_agent.Where(p => reSonId.Contains(p.Id)
                                                        &&
                                                         (!string.IsNullOrEmpty(search)
                                                             ? p.AgentName.Contains(search)
                                                             : true)
                                                        && (agentStatus != -1 ? p.IsUsed == agentStatus : true)
                    ).ToList();
                totalCount = list1.Count;
                item =
                    list1.OrderByDescending(i => i.Id)
                        .ThenByDescending(i => i.CreateTime)
                        .Take(pageSize * curPage)
                        .Skip(pageSize * (curPage - 1)).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        /// <summary>
        /// 判断顶级下面是否包含该子集代理
        /// </summary>
        /// <param name="topAgent"></param>
        /// <param name="sonAgent"></param>
        /// <returns></returns>
        public bool IsContainSon(int topAgent, int sonAgent)
        {
            return int.Parse(GetTopAgentId(sonAgent)) == topAgent;
        }
    }
}
