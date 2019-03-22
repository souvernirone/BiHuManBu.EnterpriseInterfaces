using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class BusiuserSettingDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 保险公司（1人保，2太平洋，3平安,4太平,5华泰,6平安固定,7英大,8人保固定,9国寿财固定,10国寿财移动）
        /// </summary>
        public int InsuranceType { get; set; }
        public string MachineCode { get; set; }

        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string SourceName
        {
            get
            {
                return ConvertHelper.ConvertSource(ConvertHelper.GetOldSourceByInsuranceType(InsuranceType));
            }
        }
    }

    /// <summary>
    /// 绑定采集器模型
    /// </summary>
    public class BandBusiuserSettingDto
    {
        /// <summary>
        /// bx_busiusersetting.id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// bx_busiusersetting.InsuranceType
        /// 设备类型：1人保，2太平洋，3平安，10国寿财移动
        /// </summary>
        public int InsuranceType { get; set; }

        /// <summary>
        /// InsuranceType对应的老的Source
        /// </summary>
        public int Source
        {
            get
            {
                return ConvertHelper.GetOldSourceByInsuranceType(InsuranceType);
            }
        }

        /// <summary>
        /// bx_busiuser.Id
        /// </summary>
        public int BusiuserId { get; set; }
    }

    /// <summary>
    /// 获取采集器可以使用的渠道
    /// </summary>
    public class BusiuserAgentConfigDto
    {
        public int id { get; set; }
        public string config_name { get; set; }
    }
}
