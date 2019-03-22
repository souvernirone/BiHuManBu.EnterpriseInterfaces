using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class BatchRefreshRenewalService
    {
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logError = LogManager.GetLogger("ERROR");        
        DbCommon _dbCommon = new DbCommon();       
        public void ExcuteTask()
        {
            int times = 0;
            bool flag = true;
            while (flag)
            {               
                try
                {
                    logInfo.Info("开始获取数据:"+ DateTime.Now);
                    DataTable dt = _dbCommon.GetBatchRefreshRenewal();
                    logInfo.Info("获取数据完毕:" + DateTime.Now);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        Thread.Sleep(5000);
                    }
                    logInfo.Info(JsonConvert.SerializeObject(dt));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        logInfo.Info("数量"+dt.Rows.Count+";开始：" + DateTime.Now);
                        int totalCount = dt.Rows.Count;
                        //更新表状态
                        UpdateRefRenewalStatusById(dt);
                        try
                        {
                            Parallel.For(0, totalCount, new ParallelOptions { MaxDegreeOfParallelism = 20 }, (i) =>
                             {
                                 string result = HttpHelper.GetHttpClientAsync(CreateReInfoUrl(dt.Rows[i]));
                                 if (string.IsNullOrEmpty(result) || result == "-429")
                                 {
                                     _dbCommon.UpdateBatchRefreshRenewalStatus(dt.Rows[i], 4);
                                 }
                                 else
                                 {
                                     UpdateStatus(dt.Rows[i], i, result);
                                 }
                             }
                             );
                        }
                        catch (AggregateException err)
                        {
                           MetricsLibrary.MetricUtil.UnitReports("BatchRefreshRenewalService.Parallel.For_service");
                            times = times + 1;
                            foreach (Exception item in err.InnerExceptions)
                            {
                                logError.Error(string.Format("异常类型：{0}{1}，来自：{2}{3}异常内容：{4}", item.InnerException.GetType(), Environment.NewLine, item.InnerException.Source, Environment.NewLine, item.InnerException.Message));
                            }
                            if (times == 2)
                            {
                                flag = false;
                            }

                        }
                        logInfo.Info("结束：" + DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    MetricsLibrary.MetricUtil.UnitReports("BatchRefreshRenewalService.ExcuteTask_service");
                    times = times + 1;
                    logError.Error("发生异常：" + ex);
                    if (times == 2)
                    {
                        flag = false;
                    }
                }
            }
        }

        private bool UpdateRefRenewalStatusById(DataTable dt)
        {
            List<string> idList = dt.AsEnumerable().Select(t => t.Field<int>("Id").ToString()).ToList();
            string ids = string.Join(",", idList);
            return _dbCommon.UpdateRefRenewalStatusById(ids, 5);
        }

        private void UpdateStatus(DataRow row, int i, string result)
        {
            GetReInfoResponse2 responseModel = JsonConvert.DeserializeObject<GetReInfoResponse2>(result);
            int refrenewalstatus = 4;
            //去库中去状态，然后判断续保状态，更新表
            if (responseModel.BusinessStatus == 3)
            {
                refrenewalstatus = 3;//3:只取到行驶本
            }
            else if (responseModel.BusinessStatus == 1)
            {
                refrenewalstatus = 1;//1:刷新成功
            }
            else if (responseModel.BusinessStatus == 2 || responseModel.BusinessStatus <= 0)
            {
                refrenewalstatus = 4;//4:刷新失败
            }
            _dbCommon.UpdateBatchRefreshRenewalStatus(row, refrenewalstatus);
        }

        /// <summary>
        /// 拼接url
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private string CreateReInfoUrl(DataRow row)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["BaoJiaJieKou"];
            try
            {
                Dictionary<string, string> queryparm = new Dictionary<string, string>();
                #region 拼接参数
                queryparm.Add("ChildAgent", row["AgentId"].ToString());
                queryparm.Add("CustKey", row["OpenId"].ToString());
                if (row["LicenseNo"] != null && !string.IsNullOrWhiteSpace(row["LicenseNo"].ToString()))
                {
                    queryparm.Add("LicenseNo", row["LicenseNo"].ToString().ToUpper());
                    if (row["SixDigitsAfterIdCard"] != null && !string.IsNullOrWhiteSpace(row["SixDigitsAfterIdCard"].ToString()))
                    {
                        queryparm.Add("SixDigitsAfterIdCard", row["SixDigitsAfterIdCard"].ToString());
                    }
                    //号牌种类
                    queryparm.Add("RenewalCarType", row["RenewalCarType"].ToString());
                }
                queryparm.Add("ShowRenewalCarType", "1");
                queryparm.Add("CityCode", row["CityCode"].ToString());
                queryparm.Add("Group", "1");//续保新接口
                if (row["CarVIN"] != null && !string.IsNullOrEmpty(row["CarVIN"].ToString()))
                {
                    queryparm.Add("CarVin", row["CarVIN"].ToString().ToUpper());
                }
                if (row["EngineNo"] != null && !string.IsNullOrEmpty(row["EngineNo"].ToString()))
                {
                    queryparm.Add("EngineNo", row["EngineNo"].ToString().ToUpper());
                }
                queryparm.Add("CanShowNo", "1");
                queryparm.Add("Agent", row["TopAgentId"].ToString());
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
            }
            catch (Exception ex)
            {
                logError.Error(ex);
            }
            return url;
        }
        /// <summary>
        /// 将Dictionary对象转换为queryString, 并计算SecCode值。
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>返回http请求querystring字符串</returns>
        private string ToQueryString(Dictionary<string, string> dict)
        {
            var tmp = dict.Where(x => !string.IsNullOrWhiteSpace(x.Value)).OrderBy(y => y.Key);
            StringBuilder data = new StringBuilder();
            string result = string.Join("&", tmp.Select(p => p.Key + '=' + p.Value.Trim()).ToArray());
            return '?' + result + "&SecCode=" + GetMd5(result).ToLower();
        }
        public string GetMd5(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                byte[] md5Bytes = md5.ComputeHash(bytes);
                foreach (byte item in md5Bytes)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
            }
            return stringBuilder.ToString();

        }
    }
}
