namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICityQuoteDayRepository:IRepositoryBase<bx_cityquoteday>
    {
        int GetDaysNum(int cityId);
    }
}
