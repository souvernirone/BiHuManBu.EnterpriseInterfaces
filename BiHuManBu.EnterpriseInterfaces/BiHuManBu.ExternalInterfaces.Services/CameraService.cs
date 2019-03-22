using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
namespace BiHuManBu.ExternalInterfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraService : ICameraService
    {
        private ICameraRepository _cameraRepository;
        private IUserInfoRepository _userInfoRepository;
        private ICustomerTopLevelService _customerTopLevelService;
        private IAgentRepository _agentRepository;
        private ICustomerBusinessService _customerbusinessService;
        private readonly IAgentService _agentService;
        private ILog logError;
        private ILog logInfo;
        private readonly IHashOperator _hashOperator;
        private readonly ILoginRepository _loginRepository;
        private readonly IAuthorityService _authorityService;
        private readonly IBatchRenewalService _batchRenewalService;
        private readonly IUserinfoExpandRepository _userinfoExpandRepository;
        private readonly ICameraConfigRepository _cameraConfigRepository;

        //缓存
        private readonly string _sealmanRedirs;
        private readonly string _sealmanLeaveRedirs;
        private readonly string _sealmanIndexRedirs;
        private readonly string _cameraCarModelsKey;
        private readonly ICameraDetailRepository _cameraDetailRepository;
        private ICustomerListMapperService _customerListMapperService;

        public CameraService(ICameraRepository CameraRepository,
            IUserInfoRepository userInfoRepository
            , ICustomerTopLevelService customerTopLevelService
            , ICustomerBusinessService customerbusinessService
            , IAgentRepository agentRepository
            , IAgentService agentService
            , ICameraDetailRepository cameraDetailRepository
            , ICustomerListMapperService customerListMapperService,
            ILoginRepository loginRepository,
            IAuthorityService authorityService,
            IBatchRenewalService batchRenewalService,
            IUserinfoExpandRepository userinfoExpandRepository, ICameraConfigRepository cameraConfigRepository, IHashOperator hashOperator
            )
        {
            _cameraRepository = CameraRepository;
            _userInfoRepository = userInfoRepository;
            _customerTopLevelService = customerTopLevelService;
            _agentRepository = agentRepository;
            //分配
            _customerbusinessService = customerbusinessService;
            //缓存 db
            this._hashOperator = hashOperator;
            this._cameraCarModelsKey = GetAppSettings("CameraCarModelsKey");
            this._sealmanRedirs = GetAppSettings("sealmanRedirs");
            this._sealmanLeaveRedirs = GetAppSettings("sealmanLeaveRedirs");
            this._sealmanIndexRedirs = GetAppSettings("sealmanIndexRedirs");
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
            _agentService = agentService;
            _cameraDetailRepository = cameraDetailRepository;
            _customerListMapperService = customerListMapperService;
            _loginRepository = loginRepository;
            _authorityService = authorityService;
            _batchRenewalService = batchRenewalService;
            _userinfoExpandRepository = userinfoExpandRepository;
            _cameraConfigRepository = cameraConfigRepository;
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
        #region 摄像头用户列表
        /// <summary>
        /// 摄像头用户列表 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CustomerListViewModel> FindUserListAsync(GetCustomerListRequest request)
        {
            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            _customerTopLevelService.SetPage(ref search, request.PageSize, request.CurPage);
            search.OrderBy = request.OrderBy;

            // 根据以上条件综合查询 
            var list = _userInfoRepository.FindCustomerList(search).ToList();
            var convertVM = await _customerListMapperService.ConvertToViewModelTopLevelAsync(list, search.HasDistribute, search.CurrentAgent);
            var result = new CustomerListViewModel
            {
                BusinessStatus = 1,
                CustomerList = convertVM
            };
            return result;
        }

        /// <summary>
        /// 摄像头进店列表导出
        /// </summary>
        /// <param name="request"></param>
        /// <param name="type"></param>
        /// <param name="isFiles"></param>
        /// <returns></returns>
        public CustomerListExportViewModel GetExportUserList(GetCustomerListRequest request, int type, int isFiles)
        {
            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            search.OrderBy = request.OrderBy;

            //根据以上条件综合查询
            var list = _userInfoRepository.FindCustomerListForCameraExport(search).ToList();
            // 转换模型
            var convertVM = _customerListMapperService.ConvertToViewModelExport(list, request.Agent, search.HasDistribute, search.CurrentAgent);
            var result = new CustomerListExportViewModel
            {
                BusinessStatus = 1,
                CustomerList = convertVM,
                TotalCount = list.Count,
            };
            return result;

        }
        /// <summary>
        /// 获取摄像头进店列表导出总数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DistributedCountViewModel GetExportCount(GetCustomerListRequest request)
        {
            //    string joinWhere = string.Empty;
            //    //拼接where语句
            //    string strWhere = _customerTopLevelService.GetWhereByRequest(request, out joinWhere).ToString();

            //    // 判断关联类型
            //    var joinType = _customerTopLevelService.GetJoinType(request, request.OrderBy);

            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            search.OrderBy = request.OrderBy;

            //根据以上条件综合查询
            var count = _userInfoRepository.GetExportCount(search);
            return count;
        }
        /// <summary>
        /// 摄像头用户列表总数 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DistributedCountViewModel FindUserCount(GetCustomerListRequest request)
        {
            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            //根据以上条件综合查询
            var count = _userInfoRepository.FindCustomerCountContainDistributedCount(search);
            return count;
        }
        //public int FindUserAllCount(GetCustomerListRequest request, int type, int isFiles)
        //{
        //    string joinWhere = string.Empty;
        //    //拼接where语句
        //    string strWhere = _customerTopLevelService.GetWhereByRequest(request, type,  out joinWhere).ToString();

        //    // 判断关联类型
        //    var joinType = _customerTopLevelService.GetJoinType(request, type, request.OrderBy);
        //    //根据以上条件综合查询
        //    int count = _userInfoRepository.FindCustomerAllCount(joinType, strWhere, joinWhere);
        //    return count;
        //}
        /// <summary>
        /// 摄像头用户列表总数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int FindUserCountNew(GetCustomerListRequest request)
        {
            //string joinWhere = string.Empty;
            ////拼接where语句
            //string strWhere = _customerTopLevelService.GetWhereByRequest(request,   out joinWhere).ToString();

            //// 判断关联类型
            //var joinType = _customerTopLevelService.GetJoinType(request,  request.OrderBy);

            var search = _customerTopLevelService.GetWhereAndJoinType(request, request.OrderBy);
            _customerTopLevelService.SetPage(ref search, request.PageSize, request.CurPage, request.ShowPageNum);

            //根据以上条件综合查询
            return _userInfoRepository.FindCustomerCountNew(search);
        }

        #endregion
        #region 业务员信息
        public List<SealmanViewModel> FindSealman(CameraRequest Camera)
        {
            bool isAdmin = _authorityService.IsSystemAdminOrAdmin(Camera.agentId);
            string agents = Camera.agentId.ToString();
            int agentid = Camera.agentId;
            if (isAdmin)
            {
                bx_agent agent = _agentRepository.GetAgent(Camera.agentId);
                agentid = agent != null ? agent.TopAgentId : Camera.agentId;
                //agents = string.Join(",", _agentService.GetSonsListFromRedisToString(agentid));
            }
            return _cameraRepository.FindSealman(agentid, 2, null, string.Empty, agents);
        }
        /// <summary>
        /// 查看是否已设置业务员
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool isExistSealMan(int agentId)
        {
            if (_cameraRepository.isExistSealMan(agentId) > 0)
                return true;
            return false;
        }
        //<summary>
        //保存 业务员信息
        //</summary>
        //<param name="userId"></param>
        //<returns></returns>
        public string SaveSealman(CameraRequest Camera, ref bool isSuccess)
        {
            if (Camera.sealmans == null || !Camera.sealmans.Any())
                return "不存在数据!";
            try
            {
                var insertStr = "insert into bx_Salesman(AgentId,UserId,CreateTime) values ";
                var addstr = "";
                var builder = new StringBuilder();
                foreach (var item in Camera.sealmans)
                {
                    //删除
                    if (item.isAdd == 0)
                    {
                        string delStr = string.Format("delete from bx_salesman where AgentId={0} and UserId={1};",
                            Camera.agentId, item.userId);
                        builder.Append(delStr);
                    }
                    //添加
                    if (item.isAdd == 1)
                    {
                        addstr = addstr +
                            string.Format(
                                "({0},{1},'{2}'),",
                                Camera.agentId, item.userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                if (addstr != "")
                {
                    addstr = insertStr + addstr.Substring(0, addstr.Length - 1) + ";";
                    builder.Append(addstr);
                }
                if (!string.IsNullOrWhiteSpace(builder.ToString()))
                {
                    _cameraRepository.ExecuteNonQuery(builder.ToString());
                    //更新缓存
                    SaveSealToRedirs(Camera.agentId);
                }
                isSuccess = true;
                return "保存成功！";
            }
            catch (Exception e)
            {
                isSuccess = false;
                return e.Message;
            }
        }
        /// <summary>
        /// 移除 业务员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string DelSealman(CameraRequest Camera, ref bool isSuccess)
        {
            var whereSql = string.Empty;
            if (Camera.agentId > 0)
                whereSql += " and AgentId=" + Camera.agentId;
            if (Camera.userId > 0)
                whereSql += " and UserId=" + Camera.userId;
            var data = _cameraRepository.DelSealman(whereSql, ref isSuccess);
            //更新缓存
            SaveSealToRedirs(Camera.agentId);
            return data;
        }
        #region 缓存操作
        public void SaveSealToRedirs(int agentId)
        {
            if (agentId > 0)
            {
                var item = _cameraRepository.FindAgentIdBySealman(agentId);
                item = _agentRepository.GetList(item).Where(ag => ag.IsUsed == 1).Select(ag => (long)ag.Id).ToList();
                _hashOperator.Set(_sealmanRedirs, agentId.ToString(), item);
            }
        }
        #endregion
        #endregion
        #region  业务员请假信息
        /// <summary>
        /// 获取摄像头 业务员请假信息
        /// </summary>
        /// <param name="UserId">代理人Id</param>
        /// <param name="year">年份</param>
        /// <param name="mounth">月份</param>
        /// <returns></returns>
        public List<LeaveDate> FindSealmanLeave(CameraRequest Camera)
        {
            return GetRedirsLeaveByUserId(Camera.userId);
        }
        /// <summary>
        /// 保存 业务员请假信息
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string SaveSealmanLeave(CameraRequest Camera, ref bool isSuccess)
        {
            var result = "操作失败！";
            if (Camera.Leave == null)
                return result;
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    foreach (var item in Camera.Leave)
                    {
                        if (item.status.Equals("del"))
                            _cameraRepository.delSealmanLeave(string.Format(@" and UserId={0} and  LeaveTime={1}", Camera.userId, item.leave), ref isSuccess);
                        if (item.status.Equals("add"))
                            _cameraRepository.SaveSealmanLeave(Camera.userId, item.leave, ref isSuccess);
                    }
                    transactionScope.Complete();
                }
                #region 更新Redirs缓存
                SaveLeaveToRedirs(Camera.userId);
                #endregion
                return "保存成功!";
            }
            catch (Exception e)
            {
                isSuccess = false;
                Transaction.Current.Rollback();
                return e.Message;
            }
        }
        #region 缓存操作
        /// <summary>
        /// 保存请假信息缓存
        /// </summary>
        /// <param name="userId"></param>
        public void SaveLeaveToRedirs(int userId)
        {
            //存在时更新
            var leaves = _cameraRepository.FindSealmanLeave(userId);
            _hashOperator.Set(_sealmanLeaveRedirs, userId.ToString(), leaves);

        }
        /// <summary>
        /// 获取请假信息缓存
        /// </summary>
        /// <param name="userId"></param>
        public List<LeaveDate> GetRedirsLeaveByUserId(int userId)
        {
            var data = _hashOperator.Get<List<LeaveDate>>(_sealmanLeaveRedirs, userId.ToString());
            if (data == null)
            {
                data = _cameraRepository.FindSealmanLeave(userId);
                _hashOperator.Set(_sealmanLeaveRedirs, userId.ToString(), data);
            }
            return data;
        }
        #endregion
        #endregion
        #region  车型设置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<carMold> FindCarType(int agentId)
        {
            if (agentId > 0)
                return _cameraRepository.FindCarModel(agentId);
            return _cameraRepository.FindCarModel(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> FindCarType()
        {
            return _cameraRepository.FindCarType();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Camera"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string SaveCarModel(CameraRequest Camera, ref bool isSuccess)
        {
            if (Camera.carModes.Count <= 0)
            {
                isSuccess = false;
                return "关键词不存在！";
            }
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    var builder = new StringBuilder();
                    var insertStr = "insert into bx_car_mold (AgentId,CarType,CreateTime,IsDel) values";
                    var addstr = "";
                    foreach (var item in Camera.carModes)
                    {

                        //删除
                        if (item.status.Equals("del"))
                        {
                            string delStr = "update bx_car_mold set IsDel=1  where Id=" + item.id + ";";
                            builder.Append(delStr);
                        }
                        //添加
                        if (item.status.Equals("add"))
                        {
                            addstr = addstr +
                                string.Format(
                                    "({0},'{1}','{2}',{3}),",
                                    Camera.agentId, item.name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                        }
                    }
                    if (addstr != "")
                    {
                        addstr = insertStr + addstr.Substring(0, addstr.Length - 1) + ";";
                        builder.Append(addstr);
                    }
                    if (!string.IsNullOrWhiteSpace(builder.ToString()))
                    {
                        _cameraRepository.ExecuteNonQuery(builder.ToString());
                        transactionScope.Complete();
                    }
                }
                //更新缓存
                SaveCarModelToRedirs(Camera.agentId);
                isSuccess = true;
                return "保存成功!";
            }
            catch (Exception e)
            {
                isSuccess = false;
                Transaction.Current.Rollback();
                return e.Message;
            }
        }
        /// <summary>
        /// 更新车型设置缓存
        /// </summary>
        /// <param name="agentId"></param>
        public void SaveCarModelToRedirs(int agentId)
        {
            logInfo.Info(string.Format("保存摄像头进店的设置车型过滤 放入Redis中"));

            var value = FindCarType(agentId);
            _hashOperator.Set(_cameraCarModelsKey, agentId.ToString(), value);
        }
        #endregion
        #region 设置过滤车型标签
        /// <summary>
        /// 设置车型 过滤值
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <param name="carModelKey"></param>
        /// <returns></returns>
        public bool SetCarModlId(int agentId, int userId, string carModelKey)
        {
            var data = _hashOperator.Get<List<carMold>>(_cameraCarModelsKey, agentId.ToString());
            if (data == null)
            {
                data = FindCarType(agentId);
                _hashOperator.Set(_cameraCarModelsKey, agentId.ToString(), data);
            }
            var isSuccess = false;
            //该顶级代理人没有设置摄像头过滤关键词，该车的状态是有效的
            if (data.Any())
            {
                var carModelId = -1;
                foreach (var item in data)
                {
                    //是否包含该数据
                    if (carModelKey.IndexOf(item.name) > -1)
                    {
                        carModelId = item.id;
                        break;
                    }
                }
                //已设置了过滤关键词且不在之内，该车无效被过滤掉放入回收站
                if (carModelId < 0)
                {
                    Remove(userId, 3, ref isSuccess);
                    return false;
                }
                _cameraRepository.SetCarModelId(userId, carModelId);
            }
            isSuccess = RevokeUserInfo(userId);//_cameraRepository.RevokeFiles(userId);
            return true;
        }
        /// <summary>
        /// 是否为手机号
        /// </summary>
        public static bool IsMobile(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            //手机号正则表达式
            Regex _mobileregex = new Regex("^(13|14|15|17|18)[0-9]{9}$");
            return _mobileregex.IsMatch(s);
        }
        #endregion
        #region 删除、回撤
        /// <summary>
        /// 从回收站恢复
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public BaseViewModel RevokeFiles(int userId)
        {
            var result = false;

            // 判断是否有重复数据
            var userinfo = _userInfoRepository.GetUserInfo(userId);
            if (userinfo == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "数据不存在");
            if (userinfo.IsTest == 0)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, "数据已回收，请不要重复操作");
            var agent = int.Parse(userinfo.Agent);
            // 根据代理人找见顶级代理人信息
            var topAgent = _agentRepository.GetTopAgent(agent);
            if (topAgent == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "顶级代理人不存在");
            // 获取顶级所有的代理人
            var listAgent = _agentService.GetSonsListFromRedis(topAgent.Id);
            var searchAgents = StringHandleHelper.ListToString(listAgent);
            // 根据agent和licenseno找见bx_userinfo
            var listUserinfo = _userInfoRepository.FindByAgentsAndLicenseNo(searchAgents, userinfo.LicenseNo);
            if (listUserinfo.Count > 0)
            {
                // 有 判断是否是自己的
                if (listUserinfo.Any(o => o.Agent == userinfo.Agent))
                {
                    // 是自己的，不允许回收
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.RevokeHasData);
                }
                // 不是自己的，判断顶级是否允许重复报价
                if (topAgent.repeat_quote == 0)
                {
                    // 不予许
                    return BaseViewModel.GetBaseViewModel(BusinessStatusType.RevokeHasData);
                }

                result = RevokeUserInfo(userId); //_cameraRepository.RevokeFiles(userId);

            }
            else
            {
                // 直接恢复
                result = RevokeUserInfo(userId);// _cameraRepository.RevokeFiles(userId);
            }

            if (result)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }
        /// <summary>
        /// isTest=3(回收站) isTest=1(删除) 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isTest">isTest=1是删除、isTest=3 进回收站</param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public string Remove(int userId, int isTest, ref bool isSuccess)
        {
            return _cameraRepository.Remove(userId, isTest, ref isSuccess);
        }
        public string RemoveList(List<long> userList, int isTest, ref bool isSuccess)
        {
            string strList = string.Empty;
            if (userList.Any())
            {
                strList = String.Join(",", userList);
            }
            if (string.IsNullOrEmpty(strList))
            {
                return string.Empty;
            }
            return _cameraRepository.RemoveList(strList, isTest, ref isSuccess);
        }


        #endregion

        public BaseViewModel GetCameraHistoryList(CameraHistoryListRequest request)
        {
            var list = _cameraDetailRepository.GetPageList(request.CurPage, request.PageSize, request.ChildAgent, request.CameraStartTime, request.CameraEndTime);

            var transferList = TransferModel(list);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, transferList);
        }

        public BaseViewModel GetCameraHistoryCount(CameraHistoryCountRequest request)
        {
            var count = 0;
            if (!string.IsNullOrEmpty(request.Ids))
            {
                count = request.Ids.Split(',').Length;
            }
            else
            {
                count = _cameraDetailRepository.GetCount(request.ChildAgent, request.CameraStartTime, request.CameraEndTime);
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, new { Count = count });
        }

        public BaseViewModel GetCameraHistoryExport(CameraHistoryExportRequest request)
        {
            var listId = new List<int>();
            if (!string.IsNullOrEmpty(request.Ids))
            {
                var arrlistId = request.Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrlistId.Any())
                {
                    listId = arrlistId.Select(o => Convert.ToInt32(o)).ToList();
                }
            }

            var list = _cameraDetailRepository.GetList(request.ChildAgent, request.CameraStartTime, request.CameraEndTime, listId);
            var transferList = TransferModel(list);

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, transferList);
        }

        private List<CameraHistoryDto> TransferModel(List<CameraDetailModel> list)
        {
            List<CameraHistoryDto> transferList = new List<CameraHistoryDto>();
            foreach (var item in list)
            {
                CameraHistoryDto cameraHistoryDto = new CameraHistoryDto()
                {
                    CameraId = item.camera_id,
                    LicenseNo = item.car_plate,
                    CreateTime = item.createtime.HasValue ? item.createtime.Value.ToString() : "",
                    ID = item.id
                };
                transferList.Add(cameraHistoryDto);
            }
            return transferList;
        }

        /// <summary>
        /// 是否有摄像头
        /// </summary>
        /// <param name="childagent"></param>
        /// <returns></returns>
        public bool HasCamera(int childagent)
        {
            bx_camera_config model = _loginRepository.GetCameraConfig(childagent);
            return model != null;
        }

        /// <summary>
        /// 撤销删除的数据方法
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool RevokeUserInfo(int userId)
        {
            //调用陈亮之前写的撤销方法，并做拆分，将批续模块单独拆出来
            bool reuserinfo = _cameraRepository.RevokeFiles(userId);
            if (reuserinfo)
            {
                //执行成功之后，调用批续模块的恢复
                _batchRenewalService.RevertBatchRenewalItem(userId);
                //将bx_userinfo_expand表中删除时间和删除类型恢复默认
                _userinfoExpandRepository.UpdateUserExpandByBuid(userId.ToString(), -1, DateTime.Parse("1970-01-01"));
            }
            return reuserinfo;
        }
        public BaseViewModel SetCameraConfig(SetCameraConfigRequest request)
        {
            if (request.IsRemind == 1)
            {
                request.IsRemind = 0;



            }

            else if (request.IsRemind == 0)
            {
                request.IsRemind = 1;
            }
            List<bx_camera_config> list = _cameraConfigRepository.Get(request.ChildAgent);
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.Days = request.Days;
                    item.IsRemind = request.IsRemind;
                    _cameraConfigRepository.Update(item);
                }

                return BaseViewModel.GetBaseViewModel(1, "更新成功");
            }
            else
            {
                return BaseViewModel.GetBaseViewModel(0, "未能找到该摄像头配置");
            }
        }
        public List<bx_camera_config> GetCameraConfig(int agentId)
        {
            return _cameraConfigRepository.Get(agentId);
        }
        public List<bx_camera_config> GetCameraConfigSet(int agentId)
        {
            List<bx_camera_config> list = new List<bx_camera_config>();
            var cameraConfigs = _cameraConfigRepository.Get(agentId);
            if (list != null)
            {
                foreach (var item in cameraConfigs)
                {
                    if ((item.Days != 0 && item.Days != null) || item.IsRemind != 0)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }
        public BaseViewModel GetCameraConfigByAgent(BaseRequest2 request)
        {
            List<GetCameraConfigByAgentViewModel> list = new List<GetCameraConfigByAgentViewModel>();
            bool isAdmin = _authorityService.IsAdmin(request.ChildAgent);
            if (isAdmin || request.ChildAgent == request.Agent)
            {
                var topAgentId = _agentRepository.GetAgent(request.ChildAgent).TopAgentId;
                list = _cameraRepository.GetCameraConfigByTopAgent(topAgentId);
            }

            else { list = _cameraRepository.GetCameraConfigByAgent(request.ChildAgent); }

            return BaseViewModel.GetBaseViewModel(1, "获取成功", new { TotalCount = list.Count, List = list });

        }
    }
}

