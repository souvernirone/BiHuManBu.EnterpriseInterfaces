using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Repository;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class RenewalQueue
    {

        private static BatchrefreshrenewalRepository _batchrefreshrenewalRepository;
        private static BatchRefreshRenewalConsumerRepository _batchRefreshRenewalConsumerRepository;
        //静态构造函数必须无参？
        static RenewalQueue()
        {
            _batchrefreshrenewalRepository = new BatchrefreshrenewalRepository();
            _batchRefreshRenewalConsumerRepository = new BatchRefreshRenewalConsumerRepository();
        }
        //线程安全的字典
        //private static System.Collections.Concurrent.ConcurrentDictionary<DateTime, int> dic = new System.Collections.Concurrent.ConcurrentDictionary<DateTime, int>();  
        //线程安全的队列
        private static System.Collections.Concurrent.ConcurrentQueue<bx_batchrefreshrenewal> queue = new System.Collections.Concurrent.ConcurrentQueue<bx_batchrefreshrenewal>();

        /// <summary>
        /// 实时续保
        /// </summary>
        public static void RealTimeRenewal()
        {
            //生产者
            Thread producerThread = new Thread(new ThreadStart(ThreadMain));
            producerThread.Name = "ProducerThread";
            //调用Start方法执行线程
            producerThread.Start();

            //消费者
            Thread consumerThread = new Thread(new ThreadStart(ConsumerMain));
            consumerThread.Name = "ConsumerThread";
            consumerThread.Start();

        }
        /// <summary>
        /// 生成者
        /// </summary>
        static void ThreadMain()//object obj
        {

            while (true)
            {
                try
                {
                    //判断队列，如果数量=0，则去数据库中取值，然后发送续保
                    if (queue.Count == 0)
                    {
                        GetBatchRefreshRenewalByAgentList();
                    }
                    if (queue.Count <= 15)
                    {
                        GetBatchRefreshRenewalByTimes();
                    }

                    if (queue.Count <= 15)
                    {
                        GetBatchRefreshRenewalLimit();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("将数据存储到队列发生异常：" + ex);
                }
            }
        }

        /// <summary>
        /// 消费者，主要负责进行续保
        /// </summary>
        static void ConsumerMain()
        {
            while (true)
            {
                bx_batchrefreshrenewal errModel = null;
                try
                {
                    GetInRenewal();
                    if (queue.Count == 0)
                    {
                        continue;
                    }

                    bx_batchrefreshrenewal temp = new bx_batchrefreshrenewal();
                    temp = queue.FirstOrDefault();
                    //判断是否已经完成刷新续保
                    if (_batchRefreshRenewalConsumerRepository.IsCompleteRenewal(temp.id))
                    {
                        //避免重复进入数据
                        queue.TryDequeue(out temp);
                        continue;
                    }
                    errModel = temp;
                    string url = CreateUrl(temp);
                    //更新状态：续保中
                    _batchRefreshRenewalConsumerRepository.UpdateRefRenewalStatusById(temp.id, 5);

                    //调用续保接口
                    string result = HttpHelper.GetHttpClient(url);// GetHttp(url);                
                    if (string.IsNullOrEmpty(result) || result == "-429")
                    {
                        temp.refrenewalstatus = 4;
                        _batchRefreshRenewalConsumerRepository.UpdateBatchRefreshRenewalStatus(temp);
                        queue.TryDequeue(out temp);
                        continue;
                    }
                    GetReInfoResponse2 responseModel = JsonHelper.DeSerialize<GetReInfoResponse2>(result);

                    //去库中去状态，然后判断续保状态，更新表
                    if (responseModel.BusinessStatus == 3)
                    {
                        temp.refrenewalstatus = 3;//3:只取到行驶本
                    }
                    else if (responseModel.BusinessStatus == 1)
                    {
                        temp.refrenewalstatus = 1;//1:刷新成功
                    }
                    else if (responseModel.BusinessStatus == 2 || responseModel.BusinessStatus <= 0)
                    {
                        temp.refrenewalstatus = 4;//4:刷新失败
                    }
                    _batchRefreshRenewalConsumerRepository.UpdateBatchRefreshRenewalStatus(temp);
                    //避免重复进入数据
                    queue.TryDequeue(out temp);
                }
                catch (Exception ex)
                {
                    errModel.refrenewalstatus = 4;
                    _batchRefreshRenewalConsumerRepository.UpdateBatchRefreshRenewalStatus(errModel);
                    bx_batchrefreshrenewal temp2 = new bx_batchrefreshrenewal();
                    queue.TryDequeue(out temp2);
                    LogHelper.Error("将队列中批量续保进行刷新发生异常：" + ex);
                }
            }
        }

        /// <summary>
        /// 拼接url
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private static string CreateUrl(bx_batchrefreshrenewal temp)
        {
            bx_userinfo userModel = _batchRefreshRenewalConsumerRepository.GetUserModel(temp.buid);
            string url = ConfigurationManager.AppSettings["BaoJiaJieKou"];
            Dictionary<string, string> queryparm = new Dictionary<string, string>();
            #region 拼接参数
            queryparm.Add("ChildAgent", temp.agentid.ToString());
            queryparm.Add("CustKey", temp.openid);
            if (!string.IsNullOrWhiteSpace(userModel.LicenseNo))
            {
                queryparm.Add("LicenseNo", userModel.LicenseNo.ToUpper());
                if (!string.IsNullOrWhiteSpace(userModel.SixDigitsAfterIdCard))
                {
                    queryparm.Add("SixDigitsAfterIdCard", userModel.SixDigitsAfterIdCard);
                }
                //号牌种类
                queryparm.Add("RenewalCarType", userModel.RenewalCarType.ToString());
            }
            else
            {
                queryparm.Add("LicenseNo", temp.licenseno.ToUpper());
            }
            queryparm.Add("ShowRenewalCarType", "1");
            queryparm.Add("CityCode", temp.citycode);
            queryparm.Add("Group", "1");//续保新接口
            if (!string.IsNullOrEmpty(userModel.CarVIN))
            {
                queryparm.Add("CarVin", userModel.CarVIN.ToUpper());
            }
            if (!string.IsNullOrEmpty(userModel.EngineNo))
            {
                queryparm.Add("EngineNo", userModel.EngineNo.ToUpper());
            }
            queryparm.Add("CanShowNo", "1");
            queryparm.Add("Agent", temp.topagentid.ToString());
            queryparm.Add("RenewalType", "4");
            queryparm.Add("ShowXiuLiChangType", "1");
            queryparm.Add("ShowInnerInfo", "1");
            queryparm.Add("ShowAutoMoldCode", "1");
            queryparm.Add("ShowFybc", "1");//修理期间费用补偿险：0:（默认）:否  1：是
            queryparm.Add("ShowSheBei", "1");//新增设备险种：0:（默认）:否  1：是         
            queryparm.Add("ShowRelation", "1");
            //添加是否展示 三责险附加法定节假日限额翻倍险 
            queryparm.Add("ShowSanZheJieJiaRi", "1");
            //按照实时起保返回到期日期
            queryparm.Add("TimeFormat", "1");
            #endregion
            url = url + "api/CarInsurance/getreinfo" + ToQueryString(queryparm);
            return url;
        }
        /// <summary>
        /// 将Dictionary对象转换为queryString, 并计算SecCode值。
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>返回http请求querystring字符串</returns>
        private static string ToQueryString(Dictionary<string, string> dict)
        {
            var tmp = dict.Where(x => !string.IsNullOrWhiteSpace(x.Value)).OrderBy(y => y.Key);
            StringBuilder data = new StringBuilder();
            string result = string.Join("&", tmp.Select(p => p.Key + '=' + p.Value.Trim()).ToArray());
            return '?' + result + "&SecCode=" + result.GetMd5().ToLower();
        }

        /// <summary>
        /// 读取续保中的数据重新进行续保
        /// </summary>
        private static void GetInRenewal()
        {
            //LogHelper.Info("重新读取续保中数据：" + DateTime.Now);
            try
            {
                //读取续保中的数据重新进行续保
                if (queue.Count == 0)
                {
                    List<bx_batchrefreshrenewal> inList = _batchRefreshRenewalConsumerRepository.GetInRenewalList();
                    if (inList != null && inList.Count > 0)
                    {
                        foreach (bx_batchrefreshrenewal model in inList)
                        {
                            bx_batchrefreshrenewal inTemp = queue.Where(a => a.id == model.id).FirstOrDefault();
                            if (inTemp == null || inTemp.id <= 0)
                            {
                                if (queue.Count <= 20)
                                {
                                    queue.Enqueue(model);
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("重新续保状态为续保中的数据发生异常：" + ex);
            }
        }

        /// <summary>
        /// 获取排队总最早的5条数据
        /// </summary>
        private static void GetBatchRefreshRenewalLimit()
        {
            //获取数据，并更新状态为续保中
            List<bx_batchrefreshrenewal> limitList = _batchrefreshrenewalRepository.GetBatchRefreshRenewalLimit(5);
            if (limitList != null && limitList.Count > 0)
            {
                foreach (bx_batchrefreshrenewal limitModel in limitList)
                {
                    bx_batchrefreshrenewal temp3 = queue.Where(a => a.id == limitModel.id).FirstOrDefault();
                    if (temp3 == null || temp3.id <= 0)
                    {
                        queue.Enqueue(limitModel);
                    }
                }
            }
        }

        /// <summary>
        /// 获取多次续保的数据
        /// </summary>
        private static void GetBatchRefreshRenewalByTimes()
        {
            //多次续保数据
            List<bx_batchrefreshrenewal> timesList = _batchrefreshrenewalRepository.GetBatchRefreshRenewalByTimes();
            if (timesList != null && timesList.Count > 0)
            {
                foreach (bx_batchrefreshrenewal timesModel in timesList)
                {
                    bx_batchrefreshrenewal temp2 = queue.Where(a => a.id == timesModel.id).FirstOrDefault();
                    if (temp2 == null || temp2.id <= 0)
                    {
                        queue.Enqueue(timesModel);
                    }

                }
            }
        }

        /// <summary>
        /// 获取前20条数据
        /// </summary>
        private static void GetBatchRefreshRenewalByAgentList()
        {
            //1.1获取续保中数据                    
            List<bx_batchrefreshrenewal> list = _batchrefreshrenewalRepository.GetBatchRefreshRenewalByAgentList();

            if (list != null && list.Count > 0)
            {
                foreach (bx_batchrefreshrenewal model in list)
                {
                    bx_batchrefreshrenewal temp1 = queue.Where(a => a.id == model.id).FirstOrDefault();
                    if (temp1 == null || temp1.id <= 0)
                    {
                        if (queue.Count <= 20)
                        {
                            queue.Enqueue(model);
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }
        }
    }
}
