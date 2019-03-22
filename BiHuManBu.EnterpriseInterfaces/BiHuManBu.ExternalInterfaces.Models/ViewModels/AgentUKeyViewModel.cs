using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 获取可用渠道的城市
    /// </summary>
    public class GetCanUseUkeyCityViewModel : BaseViewModel<GetCanUseUkeyCityViewModel>
    {
        /// <summary>
        /// 城市信息
        /// </summary>
        public List<MinCity> ListCity { get; set; }
    }

    /// <summary>
    /// 缩减版的bx_city
    /// 只有id，cityname
    /// </summary>
    public class MinCity
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CityName { get; set; }
    }

    /// <summary>
    /// 获取ukey模型
    /// </summary>
    public class GetPageUKeyViewModel : BaseViewModel<GetPageUKeyViewModel>
    {
        private List<GetUKeyModel> listUkey = new List<GetUKeyModel>();

        /// <summary>
        /// 
        /// </summary>
        public List<GetUKeyModel> ListUkey { get { return listUkey; } set { listUkey = value; } }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetUKeyModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UkeyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IsUsed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// ukey的代理人
        /// </summary>
        public int UkeyOwnerAgentId { get; set; }


        private string insuranceUserName = string.Empty;
        /// <summary>
        /// 保险系统用户名
        /// </summary>
        public string InsuranceUserName
        {
            get
            {
                if (string.IsNullOrEmpty(insuranceUserName))
                {
                    return "";
                }
                return insuranceUserName;
            }
            set
            {
                this.insuranceUserName = value;
            }
        }
    }

    public class UkeyBackupPwdViewModel : BaseViewModel
    {
        public BackupPwd BackupPwd { get; set; }
        public int HasPassword { get; set; }
    }

    /// <summary>
    ///缓存渠道模型
    /// </summary>
    public partial class AgentCacheChannelModel
    {

        /// <summary>
        /// 渠道ID
        /// </summary>
        public long ChannelId { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        ///*保险公司类型
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        ///*所属城市
        /// </summary>
        public int City { get; set; }

        /// <summary>
        ///*1为url 2为macurl
        /// </summary>
        public int IsUrl { get; set; }

        /// <summary>
        ///*url请求地址,网站需要
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///*mac请求地址,服务需要
        /// </summary>
        public string MacUrl { get; set; }

        /// <summary>
        ///*是否加入调度
        /// </summary>
        public int IsUseDeploy { get; set; }

        /// <summary>
        ///当前渠道是否可用
        /// </summary>
        public int IsUse { get; set; }

        /// <summary>
        ///当前已占用
        /// </summary>
        public int CurrentOccupancy { get; set; }

        /// <summary>
        ///累计执行次数-自己走自己的
        /// </summary>
        public int ExcuteTimesMyself { get; set; }

        /// <summary>
        ///累计执行次数-别人走自己的
        /// </summary>
        public int ExcuteTimesHimself { get; set; }

    }

    public class BackupPwd
    {
        /// <summary>
        /// 备用密码1
        /// </summary>
        public string BackupPwdOne { get; set; }
        /// <summary>
        /// 备用密码2
        /// </summary>
        public string BackupPwdTwo { get; set; }
        /// <summary>
        /// 备用密码3
        /// </summary>
        public string BackupPwdThree { get; set; }
    }
}
