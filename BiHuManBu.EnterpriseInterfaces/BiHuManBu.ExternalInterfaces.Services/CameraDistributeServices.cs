using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using IAgentRepository = BiHuManBu.ExternalInterfaces.Models.IAgentRepository;
using ICityQuoteDayRepository = BiHuManBu.ExternalInterfaces.Models.IRepository.ICityQuoteDayRepository;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;

namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraDistributeServices : ICameraDistributeServices
    {
        private IDistributedHistoryService _iDistributedHistoryService;
        private IConsumerDetailService _iConsumerDetailService;
        private ICameraRepository _cameraRepository;
        private IUserInfoRepository _userInfoRepository;
        private ICustomerTopLevelService _customerTopLevelService;

        private ICameraService _cameraService;
        private IAgentRepository _agentRepository;
        private readonly IAgentService _agentService;

        //分配接口
        private ICustomerBusinessService _customerbusinessService;
        private ICityQuoteDayRepository _cityQuoteDayRepository;
        private IMessageRepository _messageRepository;

        private ILog logError;
        private ILog logInfo;
        private readonly IHashOperator _hashOperator;

        private IBatchRenewalService _batchRenewalService;

        private readonly IAuthorityService _authorityService;

        //缓存
        private readonly string _sealmanRedirs;
        private readonly string _sealmanLeaveRedirs;
        private readonly string _sealmanIndexRedirs;
        private readonly string _cameraCarModelsKey;
        private readonly string _messageCenterHost;
        private readonly string _crmCenterHost;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CameraRepository"></param>
        /// <param name="userInfoRepository"></param>
        /// <param name="customerTopLevelService"></param>
        /// <param name="customerbusinessService"></param>
        /// <param name="cityQuoteDayRepository"></param>
        /// <param name="agentRepository"></param>
        /// <param name="cameraService"></param>
        /// <param name="messageRepository"></param>
        /// <param name="agentService"></param>
        public CameraDistributeServices(IDistributedHistoryService IDistributedHistoryService, IConsumerDetailService IConsumerDetailService,
            ICameraRepository CameraRepository, IUserInfoRepository userInfoRepository, ICustomerTopLevelService customerTopLevelService,
            ICustomerBusinessService customerbusinessService, ICityQuoteDayRepository cityQuoteDayRepository, IAgentRepository agentRepository,
            ICameraService cameraService, IMessageRepository messageRepository, IAgentService agentService, IBatchRenewalService batchRenewalService,
            IAuthorityService authorityService, IHashOperator hashOperator)
        {
            _iDistributedHistoryService = IDistributedHistoryService;
            _iConsumerDetailService = IConsumerDetailService;
            _cameraRepository = CameraRepository;
            _userInfoRepository = userInfoRepository;
            _customerTopLevelService = customerTopLevelService;
            _cityQuoteDayRepository = cityQuoteDayRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
            _cameraService = cameraService;
            _messageRepository = messageRepository;
            //分配
            _customerbusinessService = customerbusinessService;
            //缓存 db
            this._hashOperator = hashOperator;
            this._cameraCarModelsKey = GetAppSettings("CameraCarModelsKey");
            this._sealmanRedirs = GetAppSettings("sealmanRedirs");
            this._sealmanLeaveRedirs = GetAppSettings("sealmanLeaveRedirs");
            this._sealmanIndexRedirs = GetAppSettings("sealmanIndexRedirs");
            //发送通知地址
            this._messageCenterHost = GetAppSettings("SendMessage");

            this._crmCenterHost = GetAppSettings("SystemCrmUrl");

            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            _batchRenewalService = batchRenewalService;
            _authorityService = authorityService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }

        #region  主方法
        /// <summary>
        /// 分配人员及发送通知
        /// </summary>
        /// <param name="request"></param>
        /// <param name="carMoldTmp"></param>
        /// <param name="agentId"></param>
        /// <param name="isTopAgent"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string DistributeAndSendMsg(CameraDistributeModel request, bool carMoldTmp, int agentId, Tuple<bool, bool, bool> isTopAgent, ref bool isSuccess, List<bx_userinfo> ListexistUserInfo)
        {
            var result = "操作成功!";
            try
            {
                string businessExpireDate = request.businessExpireDate;//商业险到期时间初始化
                string forceExpireDate = request.forceExpireDate;//交强险到期时间初始化
                var deleteAgentId = ConfigurationManager.AppSettings["crmMultipleDelete"];//crm删除记录放在哪个代理下
                int msgIsDistributed = 0;//消息用_分配状态
                bx_userinfo existUserInfo = null;//老数据，执行分配操作
                bool isExitUserinfo = false;//是否存在userifo
                bool isDistributedUserInfo = false;//userinfo是否已分配
                int endDays;//原交强险到期天数,现改为商业和交强判断
                long distributedAgentId = 0;//分配代理人
                //是否在到期时间范围内
                bool intime = IsInTime(request.cityCode, businessExpireDate, forceExpireDate, out endDays);

                //carMoldTmp为true符合进店车型标准的车辆，不符合的直接改为false
                if (request.reqRenewalType == 3)
                {
                    existUserInfo = ListexistUserInfo.Where(x => x.Id != Convert.ToInt64(request.buId)).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    //如果已存在
                    if (existUserInfo != null && existUserInfo.Id > 0)
                    {
                        logInfo.Info(string.Format("buid为：{0}，已存在执行分配", request.buId));
                        isExitUserinfo = true;
                        //第一步，删除新数据
                        if (ListexistUserInfo.Count > 1)
                        {
                            _userInfoRepository.DeleteUserinfo(request.buId, deleteAgentId);
                        }
                        //第二步，修改老数据字段
                        //车型判断：是不是期望车型
                        if (!carMoldTmp)
                        {
                            logInfo.Info(string.Format("执行分配", request.buId));
                            //模型匹配失败，放入回收站
                            existUserInfo.IsTest = 3;
                        }
                        else
                        {
                            if (intime)
                            {
                                //到期范围内，取自动分配的业务员
                                distributedAgentId = GerRedirsSealman(agentId);
                            }
                        }
                        //判断记录未分配，需要分配人
                        if (existUserInfo.IsDistributed == 0 && distributedAgentId > 0)
                        {
                            isDistributedUserInfo = true;
                            existUserInfo.Agent = distributedAgentId.ToString();
                            existUserInfo.agent_id = int.Parse(distributedAgentId.ToString());//2018-09-07 更新Agent字段同时更新agent_id
                            existUserInfo.OpenId = distributedAgentId.ToString().GetMd5();
                            existUserInfo.IsDistributed = 3;
                        }
                        existUserInfo.UpdateTime = DateTime.Now;
                        existUserInfo.IsCamera = true;
                        existUserInfo.CameraTime = DateTime.Now;
                        logInfo.Info("执行更新操作的时候用户的状态：" + existUserInfo.IsTest + ",buid=" + existUserInfo.Id);
                        var a = _userInfoRepository.Update(existUserInfo);
                        //重置传入的参数
                        request.buId = (int)existUserInfo.Id;
                        request.uiRenewalType = existUserInfo.RenewalType.Value;
                        request.childAgent = int.Parse(existUserInfo.Agent);
                        //request.cityCode = string.IsNullOrEmpty(existUserInfo.CityCode) ? 1 : int.Parse(existUserInfo.CityCode);
                        request.uiCustKey = existUserInfo.OpenId;
                        msgIsDistributed = existUserInfo.IsDistributed;
                        #region 重写到期时间
                        //判断修改到期时间；
                        //上文已经将续保回来的2个时间赋值给变量，此处要做的就是判断是否需要修改这2个值
                        var renewalItem = _batchRenewalService.GetItemByBuId(existUserInfo.Id);//获取批续的续保对象
                        //判断批续是否为空
                        if (renewalItem != null && renewalItem.BizEndDate.HasValue && !string.IsNullOrWhiteSpace(businessExpireDate))
                        {
                            //批续的年份>续保年份，返回批续的时间
                            if (renewalItem.BizEndDate.Value.Year != 1900 &&
                                (DateTime.Parse(businessExpireDate).Year < renewalItem.BizEndDate.Value.Year ||
                                 string.IsNullOrEmpty(businessExpireDate)))
                            {
                                businessExpireDate = renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd");
                                forceExpireDate = renewalItem.ForceEndDate.HasValue ? (renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd")) : "";
                            }
                            intime = IsInTime(request.cityCode, businessExpireDate, forceExpireDate, out endDays);
                        }
                        #endregion
                    }
                    else
                    {
                        logInfo.Info(string.Format("buid为：{0}，未重复执行分配", request.buId));
                        //有分配的人
                        bx_userinfo noexistUserInfo = _userInfoRepository.GetUserInfo(request.buId);
                        if (!carMoldTmp)
                        {
                            noexistUserInfo.IsTest = 3;
                        }
                        else
                        {
                            if (intime)
                            {
                                //到期范围内，取自动分配的业务员
                                distributedAgentId = GerRedirsSealman(agentId);
                            }
                            if (distributedAgentId > 0)
                            {
                                logInfo.Info(string.Format("buid为：{0}，分配的代理人Id是：{1}", request.buId, distributedAgentId));
                                isDistributedUserInfo = true;
                                noexistUserInfo.Agent = distributedAgentId.ToString();
                                noexistUserInfo.agent_id = int.Parse(distributedAgentId.ToString());//2018-09-07 更新Agent字段同时更新agent_id
                                noexistUserInfo.OpenId = distributedAgentId.ToString().ToMd5();
                                noexistUserInfo.IsCamera = true;
                                noexistUserInfo.CameraTime = DateTime.Now;
                                noexistUserInfo.IsDistributed = 3;
                                //添加回收数据
                                Task<int> resultNum2 = _iDistributedHistoryService.AddDistributedHistoryAsync(new bx_distributed_history
                                {
                                    b_uid = request.buId,
                                    batch_id = 0,
                                    now_agent_id = request.childAgent,
                                    operate_agent_id = request.childAgent,
                                    top_agent_id = request.Agent,
                                    type_id = 2,
                                    create_time = DateTime.Now
                                });
                            }
                        }

                        var updateResult = _userInfoRepository.Update(noexistUserInfo);
                        //重置传入的参数
                        request.childAgent = int.Parse(noexistUserInfo.Agent);//(int)distributedAgentId;
                        request.uiCustKey = noexistUserInfo.OpenId;
                        msgIsDistributed = noexistUserInfo.IsDistributed;
                    }

                    //添加步骤记录
                    Task<int> resultNum = _iConsumerDetailService.AddCrmStepsAsync(new bx_crm_steps { agent_id = request.childAgent, b_uid = request.buId, create_time = DateTime.Now, json_content = "{\"camertime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}", type = 6 });
                }

                int isDistributed = 0;
                #region 注释掉以前逻辑
                ////原先摄像头进来的是0；非摄像头的顶级是0，非顶级是2
                ////20180105改：摄像头->顶级+管理员是0，非顶级+管理员是3；非摄像头->顶级+管理员是0，非顶级+管理员是2
                //if (isTopAgent.Item1)
                //{//如果顶级或管理员直接改为0
                //    isDistributed = 0;
                //}
                //else
                //{
                //    //非顶级和管理员，摄像头改3，非摄像头改2
                //    isDistributed = request.reqRenewalType == 3 ? 3 : 2;
                //}
                #endregion
                //摄像头 顶级和管理员数据为未分配，其他为已分配
                if (request.reqRenewalType == 3)
                {
                    if (isTopAgent.Item1)
                    {
                        isDistributed = 0;
                    }
                    else
                    {
                        isDistributed = 2;
                    }
                }
                //非摄像头 顶级为未分配，其他为已分配
                else
                {
                    if (isTopAgent.Item2)
                    {//顶级
                        isDistributed = 0;
                    }
                    else
                    {
                        isDistributed = 2;
                    }
                }

                if (msgIsDistributed == 0)
                    msgIsDistributed = isDistributed;
                //更新数据库状态：分配状态、是否摄像头进店、进店时间
                _userInfoRepository.UpdateUserRenewalTypeAndDistributed(request.buId, request.reqRenewalType, isDistributed, isExitUserinfo, isDistributedUserInfo);
                //放入回收站的数据不在分配
                if (!isSuccess)
                {
                    isSuccess = true;
                    return result;
                }

                //重新续保
                if (isExitUserinfo)
                {
                    //提供车牌、openid、agent、citycode、走renewaltype=2这样不用重新分配操作
                    Task.Factory.StartNew(() =>
                    {
                        string strurl =
                            string.Format("LicenseNo={0}&CityCode={1}&ChildAgent={2}&CustKey={3}&NeedCarMoldFilter=1&CameraAgent={4}&Agent={5}", request.licenseNo, request.cityCode, request.childAgent, request.uiCustKey, request.CameraAgent, request.Agent);
                        string strseccode = strurl.GetMd5();
                        string strReInfoUrl = string.Format("{0}/api/CarInsurance/GetReInfo?{1}&SecCode={2}", ApplicationSettingsFactory.GetApplicationSettings().BaoJiaJieKou, strurl, strseccode);
                        HttpWebAsk.Get(strReInfoUrl);
                    });
                }

                if (request.reqRenewalType == 3)
                {//摄像头进店的、非顶级账号，发送signalr到期通知
                    if (!string.IsNullOrEmpty(forceExpireDate))
                    {
                        //城市Id、车牌号、商业险到期时间、交强险到期时间、下级代理人Id、顶级代理人Id、用户Id
                        AddNoticexb(request.licenseNo, request.carModelKey, request.childAgent, request.Agent, request.buId, msgIsDistributed, intime, endDays);
                    }
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", e.Source, e.StackTrace, e.Message, e.InnerException));
                isSuccess = false;
                result = e.Message;
            }
            return result;
        }

        #region 缓存操作

        /// <summary>
        /// 该业务员今天是否存在请假
        /// </summary>
        /// <param name="agentId">业务员Id</param>
        /// <returns></returns>
        public bool isLeave(long agentId)
        {
            var leaves = _cameraRepository.FindSealmanLeave((int)agentId);
            if (leaves == null)
                return false;
            //查询盖下级代理人今天是否有请假
            foreach (var item in leaves)
            {
                if (item.leave.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                    return true;
            }
            return false;
        }
        public List<LeaveDate> FindSealmanLeave(int agentId)
        {
            var data = _hashOperator.Get<List<LeaveDate>>(this._sealmanLeaveRedirs, agentId.ToString());
            if (data == null)
            {
                data = _cameraRepository.FindSealmanLeave(agentId).Distinct().ToList();
                _hashOperator.Set(_sealmanRedirs, agentId.ToString(), data);
            }
            return data;
        }
        /// <summary>
        /// 获取缓存的销售人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<long> GetRedirsSealmans(int agentId)
        {
            ////redis做了修改
            var data = _hashOperator.Get<List<long>>(_sealmanRedirs, agentId.ToString());
            if (data == null)
            {
                data = _cameraRepository.FindAgentIdBySealman(agentId).Distinct().ToList();
                data = _agentRepository.GetList(data).Where(ag => ag.IsUsed == 1).Select(ag => (long)ag.Id).ToList();
                _hashOperator.Set(_sealmanRedirs, agentId.ToString(), data);
            }
            return data;
        }

        public List<long> GetRedirsMenber(int agentId)
        {
            string keyRedirs = "redisSealmanMenberKey";

            var data = _hashOperator.Get<List<long>>(keyRedirs, agentId.ToString());

            if (data == null)
            {
                data = _cameraRepository.FindAgentIdByMenber(agentId).Distinct().ToList();
                data = _agentRepository.GetList(data).Where(ag => ag.IsUsed == 1).Select(ag => (long)ag.Id).ToList();
                _hashOperator.Set(_sealmanRedirs, agentId.ToString(), data);
            }
            return data;
        }

        /// <summary>
        /// 查询取出分配的下级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public long GerRedirsSealman(int agentId, long index = 0, long _index = 0)
        {
            //获取设置的下级代理人Id列表
            var data = GetRedirsSealmans(agentId);
            //var data = _agentRepository.GetUsedSons(agentId);
            if (!data.Any())
                return 0;

            //获取当前摄像头的索引值并赋初始值
            try
            {
                if (_index == 0)
                    _index = index = _hashOperator.Get<long>(_sealmanIndexRedirs, agentId.ToString());
            }
            catch
            {
                _index = index = 0;
            }
            //循环的次数超过了设置的代理人数说明没有符合的代理人，因此返回0，,防止死循环
            if (index - _index > data.Count)
                return 0;

            //求余获取所分配代理人的Id
            var userId = data[(int)index % data.Count()];

            //查询该代理人今天是否请假
            if (!isLeave(userId))
            {
                ////删除摄像头索引值
                //if (_cacheClient.KeyExists(_sealmanIndexRedirs, agentId.ToString()))
                //    _cacheClient.Remove(_sealmanIndexRedirs, agentId.ToString());

                //自增1次
                _index++;
                //索引值大于2W是归0，太大了
                if (_index > 20000) _index = 0;
                //设置摄像头索引值
                _hashOperator.Set(_sealmanIndexRedirs, agentId.ToString(), _index);
                return userId;
            }
            //该代理今天请假,查询下一位代理人
            index++;
            return GerRedirsSealman(agentId, index, _index);
        }

        /// <summary>
        /// 查询取出分配的下级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public long GerRedirsMenber(int agentId, long index = 0, long _index = 0)
        {
            string readisIndex = "redisSealmanMenberIndexKey";
            //获取设置的下级代理人Id列表
            var data = GetRedirsMenber(agentId);
            //var data = _agentRepository.GetUsedSons(agentId);
            if (!data.Any())
                return 0;

            //获取当前摄像头的索引值并赋初始值
            try
            {
                if (_index == 0)
                    _index = index = _hashOperator.Get<long>(readisIndex, agentId.ToString());
            }
            catch
            {
                _index = index = 0;
            }
            //循环的次数超过了设置的代理人数说明没有符合的代理人，因此返回0，,防止死循环
            if (index - _index > data.Count)
                return 0;

            //求余获取所分配代理人的Id
            var userId = data[(int)index % data.Count()];

            //查询该代理人今天是否请假
            if (!isLeave(userId))
            {
                ////删除摄像头索引值
                //if (_cacheClient.KeyExists(_sealmanIndexRedirs, agentId.ToString()))
                //    _cacheClient.Remove(_sealmanIndexRedirs, agentId.ToString());

                //自增1次
                _index++;
                //索引值大于2W是归0，太大了
                if (_index > 20000) _index = 0;
                //设置摄像头索引值
                _hashOperator.Set(readisIndex, agentId.ToString(), _index);
                return userId;
            }
            //该代理今天请假,查询下一位代理人
            index++;
            return GerRedirsSealman(agentId, index, _index);
        }
        #endregion

        #endregion

        #region 业务员进店通知&分配公共方法
        /// <summary>
        /// 只推送到期通知
        /// </summary>
        /// <param name="cityCode">城市</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="businessExpireDate"></param>
        /// <param name="forceExpireDate"></param>
        /// <param name="source"></param>
        /// <param name="childAgent">子集agent</param>
        /// <param name="agentId">顶级agent</param>
        /// <param name="buid">车辆Id</param>
        /// <param name="isRead"></param>
        /// <returns>0未推送1已推送</returns>       
        public long AddNoticexb(int cityCode, string licenseNo, string carMold, string businessExpireDate, string forceExpireDate, int childAgent, int agentId, long buid, int distributed)
        {
            //交强险剩余天数
            int forceDays = 0;
            //不符合90内插入条件的不执行插入
            bool isInTime = IsInTime(cityCode, businessExpireDate, forceExpireDate, out forceDays);

            #region 开始发消息
            //APP推
            PushApp(buid, licenseNo, forceDays, childAgent, agentId, distributed, carMold);

            //如果不在续保期，直接退出
            if (!isInTime)
                return 0;

            if (childAgent != agentId)
            {//只有下级账号才推
                //Signal推
                PushSignal(buid, licenseNo, forceDays, childAgent);
            }
            #endregion
            return 1;
        }
        public long AddNoticexb(string licenseNo, string carMold, int childAgent, int agentId, long buid, int distributed, bool isInTime, int forceDays)
        {
            #region 开始发消息
            //如果不在续保期，直接退出
            if (!isInTime)
                return 0;

            //APP推 //20170722修改，只有到期才发通知
            PushApp(buid, licenseNo, forceDays, childAgent, agentId, distributed, carMold);

            if (childAgent != agentId)
            {//只有下级账号才推
                //Signal推
                PushSignal(buid, licenseNo, forceDays, childAgent);
            }
            #endregion
            return 1;
        }

        /// <summary>
        /// 往Signal平台推消息
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="licenseNo"></param>
        /// <param name="forceDays"></param>
        /// <param name="childAgent"></param>
        public void PushSignal(long buid, string licenseNo, int forceDays, int childAgent)
        {
            //发消息的基础模型1
            var model = new CompositeBuIdLicenseNo
            {
                BuId = buid,
                LicenseNo = licenseNo,
                Days = forceDays
            };
            //发消息基础模型2
            var list = new List<CompositeBuIdLicenseNo> { model };
            //发消息基础模型3
            var sendModel = new DuoToNoticeViewModel
            {
                AgentId = childAgent,
                Data = list,
                BuidsString = buid.ToString()
            };
            //发消息的最终模型
            var sendList = new List<DuoToNoticeViewModel> { sendModel };
            string url = string.Format("{0}/api/Message/SendDuetToNotice", _messageCenterHost);
            string data = sendList.ToJson();
            logInfo.Info(string.Format("消息发送SendDuetToNotice请求串: url:{0}/api/Message/SendDuetToNotice ; data:{1}", _messageCenterHost, data));
            //post消息发送
            string resultMessage = HttpWebAsk.HttpClientPostAsync(data, url);
            logInfo.Info(string.Format("消息发送SendDuetToNotice返回值:{0}", resultMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buid"></param>
        /// <param name="licenseNo"></param>
        /// <param name="forceDays"></param>
        /// <param name="childAgent"></param>
        /// <param name="agentId"></param>
        /// <param name="distributed">分配状态</param>
        /// <param name="moldName">车型</param>
        public void PushApp(long buid, string licenseNo, int forceDays, int childAgent, int agentId, int distributed, string moldName)
        {
            if (forceDays < 0)
            {
                //如果脱保，不执行以下操作
                return;
            }
            string url = string.Format("{0}/api/MessagePush/PushMessageToApp", _crmCenterHost);
            string strgenjin = string.Empty;
            bx_agent bxAgent = _agentRepository.GetAgent(childAgent);
            //顶级代理
            int topAgent = bxAgent != null ? bxAgent.TopAgentId : 0;
            int topAgent2 = bxAgent != null ? bxAgent.ParentAgent : 0;
            if (distributed > 0)
            {
                strgenjin = string.Format("，由业务员{0}跟进", bxAgent != null ? bxAgent.AgentName : "");
            }
            else
            {
                strgenjin = "，请点击分配给业务员";
            }
            string strmsg = string.Format("{0}已进店，车险到期还有{1}天{2}。", licenseNo, forceDays, strgenjin);
            //消息表插入消息
            //bx_message
            bx_message bxMessage = new bx_message()
            {
                Title = strmsg,
                //Body = strmsg,
                Msg_Type = 8,
                Create_Time = DateTime.Now,
                Update_Time = DateTime.Now,
                Msg_Status = 1,
                Msg_Level = 0,
                Send_Time = DateTime.Now,
                Create_Agent_Id = childAgent,
                License_No = licenseNo,
                Buid = buid,
                MsgStatus = "1"
            };
            //bx_msgindex
            int msgId = _messageRepository.Add(bxMessage);
            if (msgId < 1)
            {
                //如果message插入失败，就不执行以下操作了
                return;
            }
            #region 第一次给直接分配人推消息
            bx_msgindex bxMsgindex = new bx_msgindex()
            {
                AgentId = childAgent,
                Deleted = 0,
                Method = 4,//APP
                MsgId = msgId,
                ReadStatus = 0,
                SendTime = DateTime.Now
            };
            long msgIdxId = _messageRepository.AddMsgIdx(bxMsgindex);
            if (msgIdxId < 1)
            {
                //如果msgindex插入失败，就不执行以下操作了
                return;
            }
            //给APP推消息
            PushedMessage sendApp;
            string pushData = string.Empty;
            string resultMessage = string.Empty;
            bx_agent_xgaccount_relationship bxXgAccount = _messageRepository.GetXgAccount(childAgent);
            if (bxXgAccount != null && !string.IsNullOrEmpty(bxXgAccount.Account))
            {
                //如果没有账号，不执行以下操作

                //bx_msgindex
                //消息内容
                sendApp = new PushedMessage
                {
                    Title = GetStrMsg(topAgent2, strmsg),
                    Content = GetStrMsg(topAgent2, strmsg),
                    MsgId = msgId,
                    Account = bxXgAccount.Account,
                    BuId = buid,
                    MsgType = 8
                };
                pushData = sendApp.ToJson();
                logInfo.Info(string.Format("消息发送PushMessageToApp请求串: url:{0}/api/MessagePush/PushMessageToApp ; data:{1}", _crmCenterHost, pushData));
                resultMessage = HttpWebAsk.HttpClientPostAsync(pushData, url);
                logInfo.Info(string.Format("消息发送PushMessageToApp返回值:{0}", resultMessage));
            }
            #endregion

            #region 给顶级推消息
            msgIdxId = 0;//第一次存消息自定义的参数重新初始化
            if (bxAgent != null && topAgent != bxAgent.Id && topAgent != 0)
            {
                bx_msgindex bxMsgindex2 = new bx_msgindex()
                {
                    AgentId = topAgent,
                    Deleted = 0,
                    Method = 4,//APP
                    MsgId = msgId,
                    ReadStatus = 0,
                    SendTime = DateTime.Now
                };
                msgIdxId = _messageRepository.AddMsgIdx(bxMsgindex2);
            }
            if (msgIdxId < 1)
            {
                //如果msgindex插入失败，就不执行以下操作了
                return;
            }
            //给APP推消息
            bxXgAccount = new bx_agent_xgaccount_relationship();
            bxXgAccount = _messageRepository.GetXgAccount(topAgent);
            if (bxXgAccount == null || string.IsNullOrEmpty(bxXgAccount.Account))
            {
                //如果没有账号，不执行以下操作
                return;
            }
            //bx_msgindex
            //消息内容
            sendApp = new PushedMessage
            {
                Title = strmsg,
                Content = strmsg,
                MsgId = msgId,
                Account = bxXgAccount.Account,
                BuId = buid,
                MsgType = 8
            };
            pushData = sendApp.ToJson();
            logInfo.Info(string.Format("消息发送PushMessageToApp请求串: url:{0}/api/MessagePush/PushMessageToApp ; data:{1}", _crmCenterHost, pushData));
            resultMessage = HttpWebAsk.HttpClientPostAsync(pushData, url);
            logInfo.Info(string.Format("消息发送PushMessageToApp返回值:{0}", resultMessage));
            #endregion
        }

        /// <summary>
        /// 是否到期
        /// 先判断交强险到期时间，再判断商业险到期时间；只要有任意一个时间在续保期（不含脱保）就弹出进店提醒；
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <param name="businessExpireDate">商业险到期时间</param>
        /// <param name="forceExpireDate">交强险到期时间</param>
        /// <param name="forceDays"></param>
        /// <returns></returns>
        public bool IsInTime(int cityCode, string businessExpireDate, string forceExpireDate, out int endDays)
        {
            endDays = 0;
            try
            {
                var cityQuoteDay = _cityQuoteDayRepository.FirstOrDefault(o => o.cityid == cityCode);
                if (cityQuoteDay == null)
                {
                    logError.Error("cityid=" + cityCode + " 的城市没有设置交强和商业险有效报价时间");
                    return false;
                }
                //获取代理区域的城市到期天数设置
                int forceNum = cityQuoteDay.quotedays ?? 90;
                int bizNum = cityQuoteDay.bizquotedays ?? 90;
                //先判断交强险是否符合xx天到期
                if (!string.IsNullOrWhiteSpace(forceExpireDate))
                {
                    int forceDays = 0;
                    if (DateDiff(DateTime.Parse(forceExpireDate), DateTime.Now, out forceDays))
                    {
                        endDays = forceDays;
                        if (forceDays <= forceNum)
                        {//如果交强险满足条件直接返回true
                            return true;
                        }
                    }
                }
                //再判断商业险是否符合xx天到期，既然走到此步，说明交强险不满足条件，所以开始判断商业险只要满足即可返回true
                if (!string.IsNullOrWhiteSpace(businessExpireDate))
                {
                    int bizDays = 0;
                    if (DateDiff(DateTime.Parse(businessExpireDate), DateTime.Now, out bizDays))
                    {
                        endDays = bizDays;
                        if (bizDays > bizNum)
                        {//商业险时间跟数据规定的时间比较，超过，则不满足
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logError.Info("计算到期时间发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool DateDiff(DateTime dateTime1, DateTime dateTime2, out int days)
        {
            TimeSpan ts = dateTime1 - dateTime2;
            days = ts.Days;
            if (days < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取顶级代理人Id
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int GetTopAgentIdByAgentId(int agentId)
        {
            return _agentRepository.GetTopAgentIdByAgentId(agentId);
        }

        /// <summary>
        /// 是否位管理员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool isAdmin(int agentId)
        {
            if (_agentRepository.GetRoleNameByAgentId(agentId).Equals("管理员"))
                return true;
            return false;
        }

        /// <summary>
        /// 车辆是否已在bx_userinfo中存在
        /// </summary>
        /// <param name="agent">顶级代理人</param>
        /// <param name="buid">buid</param>
        /// <param name="licenseno">车牌号</param>
        /// <returns></returns>
        public bx_userinfo IsExistLicense(long buid, int agent, string licenseno)
        {
            //取出代理下面所有的经纪人
            var agentLists = _agentService.GetSonsListFromRedisToString(agent);
            //_agentRepository.GetSonsList(agent);
            //根据经纪人和车牌号取最新的一条数据
            bx_userinfo userinfo = _userInfoRepository.FindAgentListByLicenseNo(buid, licenseno, agentLists);
            return userinfo;
        }


        /// <summary>
        /// 根据buid，agent，车牌号获取userinfo集合
        /// </summary>
        /// <param name="agent">顶级代理人</param>
        /// <param name="buid">buid</param>
        /// <param name="licenseno">车牌号</param>
        /// <returns></returns>
        public List<bx_userinfo> GetUserinfoByLicenseAndAgent(long buid, int agent, string licenseno)
        {
            //取出代理下面所有的经纪人
            var agentLists = _agentService.GetSonsListFromRedisToString(agent);
            //_agentRepository.GetSonsList(agent);
            //根据经纪人和车牌号取最新的一条数据
            List<bx_userinfo> listuserinfo = _userInfoRepository.GetUserinfoByLicenseAndAgent(buid, licenseno, agentLists);
            return listuserinfo;
        }
        #endregion

        /// <summary>
        /// 加工strmsg，取前两位
        /// </summary>
        /// <param name="strmsg"></param>
        /// <returns></returns>
        public string GetStrMsg(int topAgent, string strmsg)
        {
            if (topAgent == 0) return strmsg;
            string[] msg = strmsg.Split('，');
            if (msg.Length > 2)
            {
                return msg[0] + "，" + msg[1] + "。";
            }
            return strmsg;
        }

        public void SetOrderAgentRedis(GetOrderAgentRequest request)
        {
            string keyRedirs = "redisSealmanMenberKey";
            var item = _cameraRepository.FindAgentIdByMenber(request.Agent);
            _hashOperator.Set(keyRedirs, request.Agent.ToString(), item);
        }
    }
}
