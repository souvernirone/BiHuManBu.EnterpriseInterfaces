using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
   public  class AnalysisSmsResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
      public AnalysisSms Data { get; set; }
        /// <summary>
        /// 1上传成功3非法请求
        /// </summary>
        public int Code { get; set; }

        public string Message { get; set; }

    }

    public class AnalysisSms
    {


    }


    }
