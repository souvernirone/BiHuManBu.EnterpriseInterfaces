using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg
{
    public class UploadFileResult
    {
        public string FileName { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }

        public int ResultCode { get; set; }

    }
}
