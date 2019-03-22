using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class LoginRepository : ILoginRepository, IDisposable
    {
        private EntityContext _entityContext = new EntityContext();
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private readonly string _autoOpenUsedId = ConfigurationManager.AppSettings["autoOpenUsedId"];
        private IAgentConfigRepository _agentConfigRepository;

        public LoginRepository(IAgentConfigRepository agentConfigRepository)
        {
            _agentConfigRepository = agentConfigRepository;
        }
        public LoginRepository()
        {
        }

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public manageruser Find(string username, string userpwd, bool checkPwd)
        {
            manageruser user = new manageruser();
            try
            {
                if (checkPwd)
                {
                    user = _entityContext.manageruser.FirstOrDefault(x => x.Name == username && x.PwdMd5 == userpwd && x.AccountType == 1);
                }
                else
                {
                    user = _entityContext.manageruser.FirstOrDefault(x => x.Name == username && x.AccountType == 1);
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return user;
        }

        /// <summary>
        /// 微信用户授权登录 2018-09-18 张克亮 小V盟项目加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="agentId">顶级经济人Id</param>
        /// <returns></returns>
        public BaseViewModel WeChatFind(string uniqueIdentifier,int agentId=0)
        {
            BaseViewModel userInfo = new BaseViewModel();
            
            try
            {
                //bx_agent_token表中进行验证授权
                string tokenSql = "select agentid from bx_agent_token where uniqueIdentifier=@uniqueIdentifier";
                MySqlParameter[] sqlParams = new MySqlParameter[1];
                sqlParams[0] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "uniqueIdentifier",
                    Value = uniqueIdentifier
                };
                var tokenAgentId=DataContextFactory.GetDataContext().Database.SqlQuery<int>(tokenSql, sqlParams).FirstOrDefault();

                //无授权信息
                if (tokenAgentId <= 0)
                {
                    userInfo.StatusMessage = "无授权信息";
                    userInfo.BusinessStatus = -10;
                    return userInfo;
                }

                //bx_agent表中获取到agentaccount
                tokenSql = "select agentaccount from bx_agent where id=@agentId";
                sqlParams = new MySqlParameter[1];
                sqlParams[0] = new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = tokenAgentId
                };

                var agentAccount= DataContextFactory.GetDataContext().Database.SqlQuery<string>(tokenSql, sqlParams).FirstOrDefault();

                //无经济人信息
                if (string.IsNullOrEmpty(agentAccount))
                {
                    userInfo.StatusMessage = "此授权的用户不存在";
                    userInfo.BusinessStatus = -11;
                    return userInfo;
                }

                //manageruser 获取用户信息
                var user = _entityContext.manageruser.FirstOrDefault(x => x.Name == agentAccount && x.AccountType == 1);
                userInfo.Data = user;
                userInfo.BusinessStatus = 1;

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return userInfo;
        }

        /// <summary>
        /// 经纪人
        /// </summary>
        /// <param name="agentAccount"></param>
        /// <returns></returns>
        public BiHuManBu.ExternalInterfaces.Models.ReportModel.AagentGroupAuthen GetAgentItemByAgentAccount(string agentAccount)
        {
            return new AgentRepository().GetAgentItemByAgentAccount(agentAccount);
        }

        public List<bx_agent> GetAgentByAgentAccount(string agentAccount, string pwd)
        {
            return new AgentRepository().GetAgentByAgentAccount(agentAccount, pwd);
        }

        public manager_role_db GetRoleInfo(int roleId)
        {
            return new ManagerRoleRepository().GetRoleInfo(roleId) ?? new manager_role_db();
        }


        /// <summary>
        /// 经纪人
        /// </summary>
        /// <param name="agentAccount"></param>
        /// <returns></returns>
        public List<bx_agent> FindAgent(string agentAccount, string pwd)
        {
            return new AgentRepository().GetAgentByAgentAccount(agentAccount, pwd);
        }


        /// <summary>
        /// 角色跟模块信息
        /// </summary>
        /// <param name="angentId"></param>
        /// <returns></returns>
        public List<manager_role_module_relation> GetManagerRoleModuleRelation(List<int> ids)
        {
            List<manager_role_module_relation> roleModule = new List<manager_role_module_relation>();
            try
            {
                roleModule = _entityContext.manager_role_module_relation.Where(x => ids.Contains(x.role_id.Value)).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return roleModule;
        }

        /// <summary>
        /// 模块信息
        /// </summary>
        /// <param name="angentId"></param>
        /// <returns></returns>
        public List<manager_module_db> GetManagerModule(List<string> moduleCode)
        {
            List<manager_module_db> managerModule = new List<manager_module_db>();
            try
            {
                managerModule = _entityContext.manager_module_db.Where(x => moduleCode.Contains(x.module_code) && x.module_status == 1).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return managerModule;
        }

        public List<manager_module_db> GetManagerModuleAlls()
        {
            return new ManagerModuleRepository().GetManagerModuleAll();
        }
        public List<string> GetModuleCodes(string paterCode, int type)
        {
            return new ManagerModuleRepository().GetManagerModuleCodes(paterCode, type);
        }

        public List<bx_agent_ukey> GetUkeyList(int agentId)
        {
            try
            {
                return _entityContext.bx_agent_ukey.Where(x => x.agent_id == agentId).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return null;
        }


        public int CreateUserToken(int agentid, string uniqueIdentifier, out string token)
        {
            token = string.Empty;
            try
            {

                var item = _entityContext.bx_agent_token.FirstOrDefault(x => x.uniqueIdentifier == uniqueIdentifier && x.agentid == agentid);
                if (item == null)
                {
                    bx_agent_token tokenItem = new bx_agent_token()
                    {
                        agentid = agentid,
                        createTime = DateTime.Now,
                        uniqueIdentifier = uniqueIdentifier
                    };
                    _entityContext.bx_agent_token.Add(tokenItem);
                    if (_entityContext.SaveChanges() > 0)
                    {
                        token = RSACryptionHelper.RSAEncrypt(string.Format(@"agentId={0}&createTime={1}&uniqueIdentifier={2}", agentid, tokenItem.createTime, uniqueIdentifier));
                        tokenItem.token = token;
                        _entityContext.SaveChanges();
                        return 1;
                    }
                }
                else
                {
                    item.createTime = DateTime.Now;
                    token = RSACryptionHelper.RSAEncrypt(string.Format(@"agentId={0}&createTime={1}&uniqueIdentifier={2}", agentid, item.createTime, uniqueIdentifier));
                    item.token = token;

                    if (_entityContext.SaveChanges() > 0)
                    {

                        //_entityContext.SaveChanges();
                        return 1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return 0;
        }

        public List<AgentIdParentIdTopIdViewModel> GetAgentHaveCamera()
        {
            string sql = string.Format(@"SELECT 	bx_agent.Id, 
	                                        bx_agent.ParentAgent, 
	                                        bx_agent.TopAgentId
	                                        FROM bx_camera_config  INNER JOIN 
	                                        bx_agent  ON bx_camera_config.park_id=bx_agent.id");

            using (var db = new EntityContext())
            {
                return db.Database.SqlQuery<AgentIdParentIdTopIdViewModel>(sql).ToList();
            }

        }

        /// <summary>
        /// 根据当前代理人获取摄像头数据，子集取park_id，顶级取seccode
        /// </summary>
        /// <param name="childagent"></param>
        /// <returns></returns>
        public bx_camera_config GetCameraConfig(int childagent)
        {
            string sql = string.Format(@"SELECT * from bx_camera_config where park_id='{0}'", childagent);
            using (var db = new EntityContext())
            {
                return db.Database.SqlQuery<bx_camera_config>(sql).FirstOrDefault();
            }
        }

        #endregion

        #region 用户注册
        /// <summary>
        /// 判断用户是否已经注册过
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        public bool IsExist(string name)
        {
            bool isExis = false;//表示不存在此用户可以进行注册
            try
            {
                var user = _entityContext.manageruser.FirstOrDefault(x => x.Name == name);
                if (user != null && !string.IsNullOrEmpty(user.Name))
                {
                    isExis = true;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return isExis;
        }
       

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="customercategories"></param>
        /// <returns></returns>
        public bool Updatedata(bx_customercategories customercategories)
        {
            using (var _dbContext = new EntityContext())
            {
                _dbContext.Set<bx_customercategories>().Attach(customercategories);
                _dbContext.Entry<bx_customercategories>(customercategories).Property("IssuingTrans").IsModified = true;
                _dbContext.Entry<bx_customercategories>(customercategories).Property("DefeatTrans").IsModified = true;
                //如果删除
                return _dbContext.SaveChanges() >= 0;
            }
        }

        /// <summary>
        /// 校验邀请码适合合法
        /// </summary>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        public bool IsInvitationCode(int parentAgent)
        {
            var result = true;
            try
            {
                result = new AgentRepository().IsExistAgentInfo(parentAgent - 1000);//false表示邀请码合法，true反之
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        #endregion

        #region 微信账号统一
        /// <summary>
        /// 根据账号查找代理人信息
        /// </summary>
        /// <param name="account"></param>

        /// <returns></returns>
        public bx_agent GetAgentByAccount(string account)
        {
            using (var db = new EntityContext())
            {

                return db.bx_agent.FirstOrDefault(c => c.AgentAccount == account);
            }
        }
        /// <summary>
        /// openid是否有机器人账号
        /// </summary>
        /// <param name="openId"></param>
        /// <returns>false 有 true 没有</returns>
        public List<bx_agent> CheckWChantUser(string openId)
        {
            using (var db = new EntityContext())
            {
                List<bx_agent> bxAgent = db.bx_agent.Where(c => c.OpenId == openId).ToList();

                return bxAgent;
            }
        }


        public int CreateWCahtAgentAccount(string opendId, string account, string passWord, manageruser manageruser, int agentId)
        {
            using (var db = new EntityContext())
            {
                bx_agent bxAgent = db.bx_agent.FirstOrDefault(c => c.OpenId == opendId && c.Id == agentId);
                if (bxAgent != null)
                {
                    bxAgent.AgentAccount = account;
                    bxAgent.AgentPassWord = passWord;
                    bxAgent.OpenId = bxAgent.Id.ToString().GetMd5();
                }
                db.manageruser.Add(manageruser);
                int result = db.SaveChanges();
                return result;
            }
        }
        /// <summary>
        /// 同一个顶级是否存在相同的手机号
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public int IsExistMobile(int topAgentId, string mobile)
        {
            using (var db = new EntityContext())
            {
                return db.bx_agent.Count(c => c.Mobile == mobile && c.IsUsed != 3 && c.TopAgentId == topAgentId);
            }
        }

        #endregion
        #region 更新数据库权限信息，顶级代理人id


        public int UpdateTopAgentId(int agentid, int topagentid)
        {
            using (var db = new EntityContext())
            {
                var bxagent = db.bx_agent.FirstOrDefault(c => c.Id == agentid);
                bxagent.TopAgentId = topagentid;
                return db.SaveChanges();
            }
        }

        #endregion
        public void Dispose()
        {
            this._entityContext.Dispose();
        }
        public List<bx_agent_config> GetAgentConfigs(int agentId)
        {
            using (var db = new EntityContext())
            {
                return db.bx_agent_config.Where(c => c.agent_id == agentId && c.is_used == 1).DistinctBy(c => c.city_id).ToList();
            }
        }

        /// <summary>
        /// 获取城市的有效报价时间
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public bx_cityquoteday GetBxCityquoteday(int cityId)
        {
            using (var db = new EntityContext())
            {
                return db.bx_cityquoteday.FirstOrDefault(c => c.cityid == cityId);
            }
        }

        /// <summary>
        /// 获取顶级代理人的城市较强和商业的有效报价时间
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        public List<AgentDredgeCity> AgentDredgeCityList(int topAgentId)
        {
            string sql = string.Format(@"
 SELECT 	
	bx_cityquoteday.cityid, 
	bx_cityquoteday.cityname, 
	bx_cityquoteday.quotedays, 
	bx_cityquoteday.bizquotedays,
bx_city.provice_short_name
	FROM 
	bx_cityquoteday
left join bx_city
on bx_cityquoteday.cityid=bx_city.id


	 ");
            using (var db = new EntityContext())
            {
                return db.Database.SqlQuery<AgentDredgeCity>(sql).ToList();
            }
        }



        ///// <summary>
        ///// 获取顶级代理
        ///// </summary>
        ///// <param name="agentId"></param>
        ///// <returns></returns>
        //public int GetTopAgent(int agentId)
        //{
        //    string sql = string.Format(@"select getAgentTopParent({0})", agentId);
        //    using (var db = new EntityContext())
        //    {
        //        return db.Database.SqlQuery<int>(sql).FirstOrDefault();
        //    }
        //}

        #region  CQA销售&运营登录
        /// <summary>
        /// 销售&运营登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public CqaLoginResultModel FindCqa(string username, string userpwd)
        {
            CqaLoginResultModel viewModel = new CqaLoginResultModel();

            #region 登录查询条件 & sql
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@account", MySqlDbType.VarChar),
                    new MySqlParameter("@password", MySqlDbType.VarChar)
                };
            parameters[0].Value = username;
            parameters[1].Value = userpwd;
            var querySql = @"SELECT id ,`name`,account,mobile,create_time,role FROM bx_cqa_user WHERE account=@account AND `password`=@password";
            //查询列表
            var listuserinfo = _entityContext.Database.SqlQuery<CqaUserModel>(querySql, parameters.ToArray()).ToList();
            #endregion

            //mysql 查询条件值不区分大小写
            if (listuserinfo.Count > 0 && listuserinfo[0].account == username)
            {
                var user = listuserinfo.FirstOrDefault();
                var querySqlCitys = @"SELECT city_id FROM bx_cqa_city WHERE cqa_id=@cqaId";
                viewModel.type = user.role;

                MySqlParameter[] parameters2 =
                {
                    new MySqlParameter("@cqaId", MySqlDbType.Int32)
                };
                parameters2[0].Value = user.id;
                if (user.role == 1)
                {
                    var selects = " id,`name`,`password`,mobile,account,sell_area_id AS areaid,sell_position as sposition,sell_parent_id as parentId,role ";
                    //查询下级销售信息<支持三级>
                    var querySqlSons = string.Format(@"select {0}  from bx_cqa_user where id=@cqaId
                                        UNION all
                                        select {0} from bx_cqa_user where sell_parent_id=@cqaId
                                        UNION all
                                        select {0} from bx_cqa_user where sell_parent_id in (select id from bx_cqa_user where sell_parent_id=@cqaId)", selects);

                    var cqaSons = _entityContext.Database.SqlQuery<CqaUser>(querySqlSons, parameters2.ToArray()).ToList();
                    var sons = string.Empty;

                    foreach (CqaUser us in cqaSons)
                    {
                        sons += us.id.ToString() + ",";
                    }
                    sons = sons.Substring(0, sons.Length - 1);

                    viewModel.cqaSons = ConvertCqa(cqaSons);
                    //销售<返回关联的AgentId>
                    #region sql
                    var querySqlAgents = string.Format("SELECT agent_id FROM bx_cqa_agent  WHERE cqa_id in ({0})", sons);
                    viewModel.agents = _entityContext.Database.SqlQuery<int>(querySqlAgents).ToList();
                    #endregion

                }
                else
                    //运营<返回关联的AgentId&城市Id>
                    if (user.role >= 2 && user.role <= 5)
                {
                    #region
                    var querySqlAgents = string.Empty;
                    if (user.role > 2)
                        querySqlCitys = "SELECT id FROM bx_city";
                    viewModel.citys = _entityContext.Database.SqlQuery<int>(querySqlCitys, parameters2.ToArray()).ToList();

                    if (viewModel.citys.Count > 0)
                    {
                        querySqlAgents = string.Format("SELECT DISTINCT agent_id FROM bx_agent_config  WHERE city_id in ({0}) and  agent_id>0;", string.Join(",", viewModel.citys));
                        viewModel.agents = _entityContext.Database.SqlQuery<int>(querySqlAgents).ToList();
                    }
                    #endregion
                }
                viewModel.CurrentMobile = listuserinfo[0].mobile;
                viewModel.BusinessStatus = 200;
                viewModel.StatusMessage = "登录成功";
            }
            else
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "账户不存在或者密码错误！";
            }
            return viewModel;
        }

        /// <summary>
        /// 角色
        /// </summary>
        public enum EnumRole
        {
            [Description("销售")]
            XS = 1,
            [Description("运营")]
            YY = 2,
            [Description("研发")]
            YF = 3,
            [Description("运维")]
            YW = 4,
            [Description("系统管理员")]
            GL = 5,
            [Description("CEO")]
            CEO = 6
        }
        public enum EnumArea
        {
            [Description("华中")]
            HuaZhong = 1,
            [Description("华南")]
            HuaNan = 2,
            [Description("华北")]
            HuaBei = 3,
            [Description("东北")]
            DongBei = 4,
            [Description("华东")]
            HuaDong = 5,
            [Description("西南")]
            XiNan = 6,
            [Description("山东")]
            ShanDong = 7
        }

        /// <summary>
        /// 销售职位
        /// </summary>
        public enum EnumSellPosition
        {
            [Description("销售")]
            HuaZhong = 0,
            [Description("城市经理")]
            HuaNan = 1,
            [Description("大区总")]
            HuaBei = 2
        }
        public List<CqaUser> ConvertCqa(List<CqaUser> CqaUserList)
        {
            var userList = new List<CqaUser>();
            foreach (CqaUser user in CqaUserList)
            {
                CqaUser item = new CqaUser();
                item.name = user.name;
                item.mobile = user.mobile;
                item.account = user.account;
                item.id = user.id;
                item.role = user.role;
                if (user.role > 0)
                    item.roleName = CommonHelper.GetEnumDescription((EnumRole)user.role);
                if (user.areaid != null && user.areaid > 0)
                    item.areaName = CommonHelper.GetEnumDescription((EnumArea)user.areaid);
                if (user.position != null && user.position > -1)
                    item.positionName = CommonHelper.GetEnumDescription((EnumSellPosition)user.position);
                userList.Add(item);
            }
            return userList;
        }
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="isDaiLi">是否是顶级经纪人0 不是 1 是顶级</param>
        /// <param name="mobile">手机</param>
        /// <param name="agentName">名称(公司名称或姓名)</param>
        /// <param name="agentType">类型(修理厂、4s店)</param>
        /// <param name="region">地域</param>
        /// <param name="parentAgent">邀请码</param>
        /// <returns></returns>
        public bool AddManagerUser(string name, string pwd, string mobile, string agentName, int agentType, string region, int isDaiLi, int parentAgent, int regType, string address, bool isUsed, out string isExistAgent, int commodity, int platfrom, int repeatQuote, int accountType, string endDate, int openQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, int zhenBangType, Dictionary<int, int> dicSource, int configCityId, int openMultiple, int settlement, int structType, int desensitization, out bx_agent registedAgent, int peopleType,int ceditOpenTuiXiu)
        {
            registedAgent = new bx_agent();
            bool result = false;//注册失败
            isExistAgent = string.Empty;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bx_agent item = new bx_agent();
                    AgentRepository agent = new AgentRepository();
                    DateTime? endTime = new DateTime();
                    var agentLevel = 0;
                    if (isDaiLi == 0)
                    {
                        bx_agent bxParentAgent = agent.GetAgent(parentAgent - 1000);

                        agentLevel = bxParentAgent.agent_level + 1;
                        platfrom = bxParentAgent.platform;
                        openQuote = bxParentAgent.openQuote;
                        agentType = bxParentAgent.AgentType != null ? Convert.ToInt32(bxParentAgent.AgentType) : -1;
                        repeatQuote = bxParentAgent.repeat_quote;
                        accountType = bxParentAgent.accountType;
                        commodity = bxParentAgent.commodity;
                        if (bxParentAgent.accountType == 0)
                        {
                            endTime = Convert.ToDateTime(bxParentAgent.endDate);
                        }
                        else
                        {
                            endTime = null;
                        }
                        if (bxParentAgent.TopAgentId.ToString() == _autoOpenUsedId|| peopleType == 1)
                        {
                            isUsed = true;
                        }
                        robotCount = bxParentAgent.robotCount;
                        brand = bxParentAgent.agentBrand;
                        contractEnd = bxParentAgent.contractEndDate;
                        quoteCompany = bxParentAgent.quoteCompany;
                        hidePhone = bxParentAgent.hide_phone;

                        if (bxParentAgent.zhen_bang_type == 1)//注册到机构下默认是外部代理
                        {
                            zhenBangType = 4;
                        }
                        else if (bxParentAgent.zhen_bang_type == 2)//注册到网点下默认内部员工
                        {
                            zhenBangType = 3;
                        }
                    }
                    else if (isDaiLi == 1)
                    {//注册顶级
                        agentLevel = 1;
                        if (accountType == 0)
                        {
                            endTime = Convert.ToDateTime(endDate);
                        }
                        else
                        {
                            endTime = null;
                        }
                    }

                    #region
                    //添加经纪人
                    
                    bool isOk = agent.AddAgent(agentName, mobile, agentType, region, name, pwd, isDaiLi, parentAgent, regType, address, isUsed, agentLevel, out item, 0, commodity, platfrom, repeatQuote, accountType, endTime, openQuote, loginType, robotCount, brand, contractEnd, quoteCompany, addRenBao, hidePhone, zhenBangType,peopleType);
                    registedAgent = item;
                    if (!isOk)
                    {
                        return isOk;
                    }

                    //添加用户
                    var manageruser = new ManagerUserRepository().AddManagerUser(name, pwd, mobile, peopleType);
                    if (manageruser == null)
                    {
                        // 添加manageruser出错
                        logError.Error("注册时出错，参数：name=" + name + "||pwd=" + pwd + "||mobile=" + mobile);
                        return false;
                    }
                    if (isDaiLi == 1)//顶级经纪人
                    {
                        #region 创建相应的角色信息
                        var role = new ManagerRoleRepository().AddManagerRole(item.Id, name, item.zhen_bang_type);
                        var roleId = role.Where(x => x.role_type == 3).Select(x => x.id).FirstOrDefault();
                        if (manageruser != null)
                        {
                            new ManagerUserRepository().EditManagerUserRoleId(manageruser.ManagerUserId, roleId);
                            new ManagerUserRepository().EditAgentRoleId(item.Id, roleId);
                        }
                        #endregion
                        #region 顶级初始化战败原因
                        List<bx_defeatreasonsetting> defeatReasonSettingList = new List<bx_defeatreasonsetting>() {
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="无效数据（停机、空号）", Deleted=false, IsChange=false, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="其他", Deleted=false, IsChange=false, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="已在别处投保", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="有专人代办", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="车行距离太远", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="车行服务问题", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="外地投保", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="价格太贵", Deleted=false, IsChange=true, UpdateTime=DateTime.Now},
                        new bx_defeatreasonsetting {  AgentId=item.Id, CreateTime=DateTime.Now, DefeatReason="礼品没有吸引力", Deleted=false, IsChange=true, UpdateTime=DateTime.Now}

                            };
                        foreach (var defeatReasonSetting in defeatReasonSettingList)
                        {
                            _entityContext.bx_defeatreasonsetting.Add(defeatReasonSetting);
                        }
                        _entityContext.SaveChanges();
                        #endregion
                        #region 顶级代理人创建客户类别
                        // List<bx_customercategories> _customercategoriesList = new List<bx_customercategories>() 
                        // {
                        // new bx_customercategories {  AgentId=item.Id, CategoryInfo="新车", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="在修不在保", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="不在修不在保", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="往年战败", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="续保客户", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="新转续", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="潜转续", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="间转续", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //new bx_customercategories {  AgentId=item.Id, CategoryInfo="续转续", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        // new bx_customercategories {  AgentId=item.Id, CategoryInfo="三年联保", CreateTime=DateTime.Now, DefeatTrans=0,Deleted=false,IsStart=0,IssuingTrans=0},
                        //     };
                        // foreach (var customercategories in _customercategoriesList)
                        // {
                        //     _entityContext.bx_customercategories.Add(customercategories);
                        // }
                        // _entityContext.SaveChanges();
                        //var A1 = new bx_customercategories
                        //{

                        //    AgentId = item.Id,
                        //    CategoryInfo = "新车",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        //var A2 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "在修不在保",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        //var A3 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "不在修不在保",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};

                        //var A4 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "往年战败",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        //var A5 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "续保客户",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        var A6 = new bx_customercategories
                        {
                            AgentId = item.Id,
                            CategoryInfo = "新转续",
                            CreateTime = DateTime.Now,
                            DefeatTrans = 0,
                            Deleted = false,
                            IsStart = 1,
                            IssuingTrans = 0
                        };
                        var A7 = new bx_customercategories
                        {
                            AgentId = item.Id,
                            CategoryInfo = "潜转续",
                            CreateTime = DateTime.Now,
                            DefeatTrans = 0,
                            Deleted = false,
                            IsStart = 1,
                            IssuingTrans = 0
                        };
                        //var A8 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "间转续",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        var A9 = new bx_customercategories
                        {
                            AgentId = item.Id,
                            CategoryInfo = "续转续",
                            CreateTime = DateTime.Now,
                            DefeatTrans = 0,
                            Deleted = false,
                            IsStart = 1,
                            IssuingTrans = 0
                        };
                        //var A10 = new bx_customercategories
                        //{
                        //    AgentId = item.Id,
                        //    CategoryInfo = "三年联保",
                        //    CreateTime = DateTime.Now,
                        //    DefeatTrans = 0,
                        //    Deleted = false,
                        //    IsStart = 0,
                        //    IssuingTrans = 0
                        //};
                        //_entityContext.bx_customercategories.Add(A1);
                        //_entityContext.bx_customercategories.Add(A2);
                        //_entityContext.bx_customercategories.Add(A3);
                        //_entityContext.bx_customercategories.Add(A4);
                        //_entityContext.bx_customercategories.Add(A5);
                        _entityContext.bx_customercategories.Add(A6);
                        _entityContext.bx_customercategories.Add(A7);
                        // _entityContext.bx_customercategories.Add(A8);
                        _entityContext.bx_customercategories.Add(A9);
                        // _entityContext.bx_customercategories.Add(A10);
                        _entityContext.SaveChanges();

                        //var UA1 = new bx_customercategories
                        //{
                        //    Id = A1.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A6.Id
                        //};
                        //var UA2 = new bx_customercategories
                        //{
                        //    Id = A2.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A7.Id
                        //};
                        //var UA3 = new bx_customercategories
                        //{
                        //    Id = A3.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A7.Id
                        //};
                        //var UA4 = new bx_customercategories
                        //{
                        //    Id = A4.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A8.Id
                        //};
                        //var UA5 = new bx_customercategories
                        //{
                        //    Id = A5.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A9.Id
                        //};
                        //var UA6 = new bx_customercategories
                        //{
                        //    Id = A6.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A9.Id
                        //};
                        //var UA7 = new bx_customercategories
                        //{
                        //    Id = A7.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A9.Id
                        //};
                        //var UA8 = new bx_customercategories
                        //{
                        //    Id = A8.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A9.Id
                        //};
                        //var UA9 = new bx_customercategories
                        //{
                        //    Id = A9.Id,
                        //    DefeatTrans = A4.Id,
                        //    IssuingTrans = A9.Id
                        //};
                        //Updatedata(UA1);
                        //Updatedata(UA2);
                        //Updatedata(UA3);
                        //Updatedata(UA4);
                        //Updatedata(UA5);
                        //Updatedata(UA6);
                        //Updatedata(UA7);
                        //Updatedata(UA8);
                        //Updatedata(UA9);
                        #endregion

                        #region 角色跟模块关系
                        if (regType != 1)
                        {
                            new ManagerRoleModuleRelationRepository().AddRoleModuleRelation(role, name, regType, registedAgent.zhen_bang_type);
                        }
                        #endregion

                        #region 代理人配置信息(bx_agent_setting表)
                        bx_agent_setting setting = new bx_agent_setting();
                        setting.agent_id = registedAgent.Id;
                        setting.can_multiple_quote = openMultiple;
                        setting.create_time = System.DateTime.Now;
                        setting.desensitization = desensitization;
                        setting.is_open_tuixiu = ceditOpenTuiXiu;
                        if (registedAgent.RegType == 1)
                        {
                            setting.struct_type = structType;
                            setting.settlement_type = settlement;
                        }
                        new AgentRepository().AddAgentSetting(setting);
                        #endregion
                        #region 多渠道报价(可以开启的渠道数量)
                        if (openMultiple == 1)
                        {
                            _agentConfigRepository.AddSouceCount(registedAgent.Id, configCityId, dicSource);
                        }
                        #endregion
                    }
                    else//非顶级经纪人
                    {
                        if (parentAgent > -1)
                        {
                            #region
                            //顶级经纪人id
                            var topPatentAgent = new AgentRepository().GetAgentTopParent(parentAgent - 1000);
                            var roleId = _entityContext.manager_role_db.Where(x => x.top_agent_id == topPatentAgent && x.role_type == 0).Select(x => x.id).FirstOrDefault();
                            if (manageruser != null)
                            {
                                new ManagerUserRepository().EditManagerUserRoleId(manageruser.ManagerUserId, roleId);
                                new ManagerUserRepository().EditAgentRoleId(item.Id, roleId);
                            }
                            #endregion
                        }
                    }
                    result = true;//注册成功
                    scope.Complete();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result = false;//注册失败
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
        #endregion


        public SfMobileLoginViewModel SfMobileLogin(string agentAccount, string agentPassWord)
        {
            SfMobileLoginViewModel viewModel = new SfMobileLoginViewModel();
            var sql = @"SELECT * FROM bx_sf_agent t WHERE BINARY t.AgentAccount = @agentAccount AND AgentPassWord = @agentPassWord";
            MySqlParameter[] parameters =
                {
                    new MySqlParameter("@agentAccount", MySqlDbType.VarChar),
                    new MySqlParameter("@agentPassWord", MySqlDbType.VarChar)
                };
            parameters[0].Value = agentAccount;
            parameters[1].Value = agentPassWord;
            bx_sf_agent sfAgent = _entityContext.Database.SqlQuery<bx_sf_agent>(sql, parameters).FirstOrDefault();
            if (sfAgent != null)
            {
                if (sfAgent.is_used == 0)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "用户被禁用";
                }
                else
                {
                    viewModel.AgentId = sfAgent.Id;
                    viewModel.AgentName = sfAgent.AgentName;
                    viewModel.AgentAccount = sfAgent.AgentAccount;
                    viewModel.IsViewAllData = sfAgent.is_view_all_data.Value;
                    viewModel.BusinessStatus = 200;
                    viewModel.StatusMessage = "登录成功";
                }
            }
            else
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "用户名或密码有误";
            }
            return viewModel;
        }
    }
}