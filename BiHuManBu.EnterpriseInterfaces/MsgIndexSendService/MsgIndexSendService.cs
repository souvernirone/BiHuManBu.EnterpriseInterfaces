using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Timers;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using log4net;
using Quartz;
using ServiceStack;
using Topshelf;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace MsgIndexSendService
{
    public class MsgIndexSendService : ServiceControl
    {//创建日志记录组件实例  

        private readonly IScheduler scheduler;
        ILog log = LogManager.GetLogger("ErrorAppender");
        readonly Timer _timer;
        public MsgIndexSendService()
        {
            int msec = int.Parse(ConfigurationManager.AppSettings["SysTimes"]);
            _timer = new Timer(msec);
            //初始化Quartz任务器

            _timer.Elapsed += new ElapsedEventHandler(InsertMsgIndex);
        }

        public void InsertMsgIndex(object sender, ElapsedEventArgs e)
        {
            bool result = MesageService();
            if (result)
            {
                Console.WriteLine("任务执行成功," + DateTime.Now);
            }
            else
            {
                Console.WriteLine("任务执行失败," + DateTime.Now);
            }
        }
        public bool MesageService()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //查满足条件的消息Id
            string sqlMsg = "SELECT * FROM bx_message WHERE Msg_Type=0 AND MsgStatus='2' AND Send_Time < CURTIME() ORDER BY id DESC";
            try
            {
                bx_message msg = new bx_message();
                using (var _dbContext = new EntityContext())
                {
                    msg = _dbContext.Database.SqlQuery<bx_message>(sqlMsg).FirstOrDefault();
                }
                if (msg == null)
                { //**执行失败
                    log.Info("时间为：" + DateTime.Now + "，未查询到数据");
                    return false;
                }
                //查该消息是否已发送过
                string sqlCount = string.Format("SELECT COUNT(1) FROM bx_msgindex WHERE MsgId={0} AND ReadStatus=0", msg.Id);
                int count;
                using (var _dbContext = new EntityContext())
                {
                    count = _dbContext.Database.SqlQuery<int>(sqlCount).FirstOrDefault();
                }
                if (count > 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "已执行过");
                    //return false;
                }
                //更新消息状态
                string sqlUpdate = string.Format("UPDATE bx_message SET MsgStatus='1' WHERE id={0}", msg.Id);
                using (var _dbContext = new EntityContext())
                {
                    count = _dbContext.Database.ExecuteSqlCommand(sqlUpdate);
                }
                if (count == 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "更新失败");
                    return false;
                }
                List<ChannelScope> list = new List<ChannelScope>();
                if (string.IsNullOrEmpty(msg.ChannelAndScope))
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "配置无需发送消息");
                    return false;
                }
                list = msg.ChannelAndScope.FromJson<List<ChannelScope>>();
                #region 批量发送短信
                var smsObj = list.FirstOrDefault(l => l.Channel == 3);
                if (smsObj != null)
                {
                    //批量发送短信
                    List<bx_agent> agentlists = new List<bx_agent>();
                    string istest = ConfigurationManager.AppSettings["IsTest"];
                    using (var _dbContext = new EntityContext())
                    {
                        string sqlAgents = string.Format("SELECT * FROM bx_agent WHERE isused=1 and ParentAgent=0 {0}", istest.Equals("1") ? " and id=102" : string.Empty);
                        agentlists = _dbContext.Database.SqlQuery<bx_agent>(sqlAgents).ToList();
                    }
                    if (agentlists.Any())
                    {
                        string smsurl = string.Format("{0}/SubmitSms", ConfigurationManager.AppSettings["SmsCenter"]);
                        string smsdata = string.Empty;
                        string smsresult = string.Empty;
                        int x = 0;
                        foreach (var agent in agentlists.Where(l => l.Mobile != null && l.Mobile.Length == 11))
                        {
                            smsdata = string.Format("account={0}&password={1}&mobile={2}&smscontent={3}&businessType={4}", "102-bihu", "16857e4cc146f05cbdf9e7198ba1dffd", agent.Mobile, msg.Url, 3);
                            HttpWebAsk.Post(smsurl, smsdata, out smsresult);
                            x++;
                        }
                        //记录最后一条发短信返回值
                        log.Info("发送" + x + "次短信，调用接口：Url:" + smsurl + "，请求：" + smsdata + "，最后一次返回值：" + smsresult);
                    }
                }
                #endregion
                #region 往crm推消息
                var crmObj = list.FirstOrDefault(l => l.Channel != 3);
                if (crmObj != null)
                {
                    //如果消息记录没在msgindex表中存在，则进行插入操作
                    string msgurl = string.Format("{0}/api/Message/SetMsgAgent", ConfigurationManager.AppSettings["SystemCrmUrl"]);
                    string tmpData =
                        string.Format("MsgId={0}&Agent={1}", msg.Id, msg.Create_Agent_Id);
                    var postData = new
                    {
                        MsgId = msg.Id,
                        Agent = msg.Create_Agent_Id,
                        SecCode = tmpData.GetMd5()
                    };
                    string msgjson = postData.ToJson();
                    var msgresult = HttpWebAsk.HttpClientPostAsync(msgjson, msgurl);
                    log.Info("发送生成消息接口：Url:" + msgurl + "，请求：" + msgjson + "，返回值：" + msgresult);
                }
                return true;

                #endregion
            }
            catch (Exception ex)
            {
                log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            return false;
        }
        public bool Start(HostControl hostControl)
        {
            //_log.Info("TopshelfDemo is Started");
            _timer.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _timer.Stop();
            throw new NotImplementedException();
        }


        //public bool Start(HostControl hostControl)
        //{
        //    scheduler.Start();
        //    return true;
        //}

        //public bool Stop(HostControl hostControl)
        //{
        //    scheduler.Shutdown(false);
        //    return true;
        //}

        //public bool Continue(HostControl hostControl)
        //{
        //    scheduler.ResumeAll();
        //    return true;
        //}

        //public bool Pause(HostControl hostControl)
        //{
        //    scheduler.PauseAll();
        //    return true;
        //}
    }
}
