using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ILoginService
    {
        Task<Account> LoginAccount(string mobile);
        GetLoginViewModel Login(string name, string pwd, string uniqueIdentifier, int FromMethod, int agentId = 0, int isWChat = 0, int topAgentId = 0, bool checkPwd = true, int GroupId = 0);

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="isDaiLi">是否是顶级经纪人</param>
        /// <param name="mobile">手机</param>
        /// <param name="agentName">名称(公司名称或姓名)</param>
        /// <param name="agentType">类型(修理厂、4s店)</param>
        /// <param name="region">地域</param>
        /// <param name="shareCode">邀请码</param>
        /// <param name="isCheck"></param>
        /// <param name="regType">单店集团</param>
        /// <param name="address">地址</param>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="isUsed">是否启用</param>
        /// <param name="commodity">已购商品</param>
        /// <param name="addRenBao"></param>
        /// <param name="hidePhone">手机号加星</param>
        /// <param name="platfrom">可以使用的平台</param>
        /// <param name="repeatQuote">重复报价</param>
        /// <param name="loginType">0普通注册登陆,1联合注册登陆</param>
        /// <param name="robotCount">安装机器人数量</param>
        /// <param name="brand">4S店品牌</param>
        /// <param name="contractEnd">收费账号合同截止日期</param>
        /// <param name="registedAgent">是否是人保专用 1专用 0不专用</param>
        /// <param name="accountType">0测试账号，1付费账号</param>
        /// <param name="endDate">账号有效期</param>
        /// <param name="openQuote">开通新车报价</param>
        /// <param name="quoteCompany">可报价保险公司</param>
        GetLoginViewModel Register(string name, string pwd, string mobile, string agentName, int agentType, string region, int isDaiLi, int shareCode, bool isCheck, int regType, string address, string uniqueIdentifier, bool isUsed, int commodity, int platfrom, int repeatQuote, int loginType, int robotCount, string brand, DateTime? contractEnd, int quoteCompany, int addRenBao, int hidePhone, out bx_agent registedAgent, int accountType = 0, string endDate = "", int openQuote = 0, int zhenBangType = 0, Dictionary<int, int> dicSource = null, int configCityId = -1, int openMultiple = 0, int settlement = 0, int structType = 0, int desensitization = 0,int peopleType=0,int ceditOpenTuiXiu=0);

        List<ManagerModule> GetModuleChild(List<manager_module_db> moduleAll, string parentModuleCode);
        SmsResultViewModel SendSms(string mobile, string smsContent, EnumSmsBusinessType businessType);
        GetLoginViewModel UpdateInfo();
        int IsInvitationCode(int parentAgent);
        CqaLoginResultModel CqaLogin(string name, string pwd);

        /// <summary>
        /// 外部登录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        GetLoginViewModel ExternalLogin(ExternalLoginRequest request, string keyCode);

        GetTopAgentInfoResult TopAgentInfoByAccount(string agentAccount, int topAgentId);
        IsUpdateAccountResult IsUpdateAccount(string openId, int topAgentId);

        CreateWCahtAgentAccountResult CreateWCahtAgentAccount(string opendId, string account,
            string passWord, int agentId);

        /// <summary>
        /// 获取城市的商业交强的报价有效时间
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        AgentDredgeCityRequest GetAgentDredgeCity(int topAgentId);
        int IsExistMobile(int topAgentId, string mobile);
        GetUserInfoByShareCodeResult GettUserInfoByShareCode(int shareCode);
        void DataUpdate();
        void UpdateCarRenewalIndex();

        /// <summary>
        /// 根据手机号和代理查是否重复手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="parentAgent"></param>
        /// <returns></returns>
        bool IsExistMobile(string mobile, int parentAgent);

        SfMobileLoginViewModel SfMobileLogin(string agentAccount, string agentPassWord);

        /// <summary>
        /// 微信授权登录 2018-09-18 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="topAgentId">顶级经济人ID</param>
        /// <returns></returns>
        GetLoginViewModel WeChatLogin(string uniqueIdentifier, int topAgentId=0);

        /// <summary>
        /// 生成经济人授权信息 2018-09-19 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="topAgentId">顶级经济人ID</param>
        /// <returns>授权成功后生成的token</returns>
        BaseViewModel SetAgentToken(string uniqueIdentifier, int topAgentId);
    }
}
