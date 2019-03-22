namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISmsContentRepository : IRepositoryBase<bx_sms_account>
    {    /// <summary>
         /// 
         /// </summary>
        bx_sms_account Find(int agent);
        /// <summary>
        /// 
        /// </summary>
        int Add(bx_sms_account bxSmsAccount);
        /// <summary>
        /// 
        /// </summary>
        int Update(bx_sms_account bxSmsAccount);
    }
}
