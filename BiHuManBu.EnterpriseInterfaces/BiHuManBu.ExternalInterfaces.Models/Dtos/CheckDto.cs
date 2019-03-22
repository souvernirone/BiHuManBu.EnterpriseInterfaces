namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    /// <summary>
    /// 校验的模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckDto<T>
    {
        #region 属性
        /// <summary>
        /// 只有返回1时是成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 校验的消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 校验返回的数据
        /// </summary>
        public T Data { get; set; }
        #endregion

        #region 构造函数
        public CheckDto()
        {
            
        }

        public CheckDto(int status,string msg)
        {
            Status = status;
            Msg = msg;
        }

        public CheckDto(int status, T data)
        {
            Status = status;
            Data =data;
        }

        #endregion

        public static CheckDto<T> GetModel(int status, T data)
        {
            return new CheckDto<T>(status, data);
        }
    }
}
