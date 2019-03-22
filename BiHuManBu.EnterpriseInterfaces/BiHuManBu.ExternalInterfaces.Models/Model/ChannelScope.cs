namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 发送渠道和范围
    /// </summary>
    public  class ChannelScope
    {
        /// <summary>
        /// 发送渠道 1:PC端 2:微信端  3:短信
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 发送范围 1:仅对顶级 2:所有账号
        /// </summary>
        public int Scope { get; set; }
    }
}
