using System;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Repository;
using CacheMakerServiceDemo.RedisOperator;
using ServiceStack.Text;

namespace CacheMakerServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
             DataSetUp();
             Test();

        }

        static void DataSetUp()
        {
            //agent 缓存设置



            IAgentRepository _agentRepository = new AgentRepository();
            var items = _agentRepository.FindList();

            using (var client = RedisManager.GetClient())
            {
                client.FlushAll();
                using (var pile = client.CreatePipeline())
                {
                    foreach (var item in items)
                    {
                        bx_agent item1 = item;
                        pile.QueueCommand(p => p.Set(string.Format("agent:{0}", item1.Id), item1.ToJson()));
                        // pile.Set(string.Format("agent:{0}", item.Id), item.ToJson());
                    }

                    pile.Flush();
                }
            }

            Console.WriteLine("正在处理经纪人");

            var topAgents = items.Where(x => x.ParentAgent == 0).ToList();
            using (var client = RedisManager.GetClient())
            {
                using (var pipe = client.CreatePipeline())
                {
                    //添加顶级
                    foreach (var agent in topAgents)
                    {
                        bx_agent agent1 = agent;
                        pipe.QueueCommand(p => p.AddItemToSet("agent:top:", agent1.Id.ToString()));//所有顶级经纪人


                    }
                    foreach (bx_agent item in items)
                    {
                        var fastChild = items.Where(x => x.ParentAgent == item.Id).ToList();
                        foreach (bx_agent agent in fastChild)
                        {
                            pipe.QueueCommand(p => p.AddItemToSet(string.Format("agent:{0}:", item.Id.ToString()), agent.Id.ToString()));
                        }
                    }

                    pipe.Flush();
                }
            }

            //报价数据

            Console.WriteLine("正在处理报价信息");

            int curAgent = 1;

            foreach (var agent in items)
            {
                Console.Write("当前的经纪人是 :" + agent.Id);
                using (var client = RedisManager.GetClient())
                {
                    using (var pipe = client.CreatePipeline())
                    {
                        var userinfoRepository = new UserInfoRepository();
                     
                        var userinfos = userinfoRepository.FindList(agent.Id.ToString());
                        if (userinfos.Count > 0)
                        {
                            foreach (var userinfo in userinfos)
                            {
                                //1.添加基础数据  key:  bx_userinfo:b_Uid
                                pipe.QueueCommand(p => p.Set(string.Format("base:bx_userinfo:{0}", userinfo.Id), userinfo.ToJson()));

                                //2.搜集每个人的 报价集合 格式：  agent:bx_userinfo:agentid   集合
                                pipe.QueueCommand(p => p.AddItemToSet(string.Format("agent:{0}:bx_userinfo", agent.Id), userinfo.Id.ToString()));

                                //3.基于车牌号索引
                                pipe.QueueCommand(p => p.AddItemToSet(string.Format("agent:{0}:bx_userinfo:licenseno:{1}", agent.Id, userinfo.LicenseNo), userinfo.Id.ToString()));

                                //4.基于品牌型号
                                pipe.QueueCommand(p => p.AddItemToSet(string.Format("agent:{0}:bx_userinfo:moldname:{0}", userinfo.MoldName), userinfo.Id.ToString()));

                                //5.基于客户来源
                                pipe.QueueCommand(p => p.AddItemToSet(string.Format("agent:{0}:bx_userinfo:fromsource:{0}", userinfo.RenewalType.ToString()), userinfo.Id.ToString()));
                                //6.基于录入时间
                                pipe.QueueCommand(p => p.AddItemToSortedSet(string.Format("agent:{0}:bx_userinfo:updatetime:{0}", userinfo.RenewalType.ToString()), userinfo.Id.ToString(), userinfo.UpdateTime.Value.Ticks));
                                //7.基于注册时间
                                pipe.QueueCommand(p => p.AddItemToSortedSet(string.Format("agent:{0}:bx_userinfo:registerdate:{0}", userinfo.RenewalType.ToString()), userinfo.Id.ToString(), Convert.ToDateTime(userinfo.RegisterDate).Ticks));
                                //8.
                            }

                            pipe.Flush();
                        }

                    }
                }
                curAgent++;
            }
        }
        //续保数据
        public void DataSetBxCarRenewal()
        {
        }
        //

        static void Test()
        {
            //1.读取 agent=102 的 所有报价信息

            using (var  client=RedisManager.GetClient())
            {
                var items_102 = client.GetAllItemsFromSet("agent:102:bx_userinfo");
                var nextAgents = client.GetAllItemsFromSet("agent:102");
                foreach (var bxAgent in nextAgents)
                {
                    var Agents = client.GetAllItemsFromSet(string.Format("agent:{0}", bxAgent));
                }
            }
            //2.针对车牌号查询

            using (var client=RedisManager.GetClient())
            {

                var item_licenseno = client.GetIntersectFromSets("agent:102:bx_userinfo", "agent:102:licenseno:京FF1234","");
                
                
              
            }
        }
    }
}
