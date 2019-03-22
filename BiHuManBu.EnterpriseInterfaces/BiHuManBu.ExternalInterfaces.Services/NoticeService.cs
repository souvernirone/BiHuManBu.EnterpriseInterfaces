using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class NoticeService : INoticeService
    {
        private INoticeRepository _noticeRepository;

        public NoticeService(INoticeRepository noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }
        public int ShareBatchRenewal(string buids)
        {
            return _noticeRepository.ShareBatchRenewal(buids);
        }
    }
}
