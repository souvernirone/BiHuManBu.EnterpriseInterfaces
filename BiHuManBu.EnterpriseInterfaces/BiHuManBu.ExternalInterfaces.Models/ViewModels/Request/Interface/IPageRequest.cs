namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.Interface
{
    public interface IPageRequest
    {
        int CurPage { get; set; }

        int PageSize { get; set; }
    }
}
