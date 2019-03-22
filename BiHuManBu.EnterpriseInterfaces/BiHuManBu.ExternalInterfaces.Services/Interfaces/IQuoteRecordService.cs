using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IQuoteRecordService
    {
        Task<BaseViewModel> AddQuoteRecord(AddQuoteRecordRequest request);
    }
}
