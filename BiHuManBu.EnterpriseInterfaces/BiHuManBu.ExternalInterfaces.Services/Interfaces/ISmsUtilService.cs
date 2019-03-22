using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
   public interface ISmsUtilService
    {
       /// <summary>
       /// 初始化黑词
       /// </summary>
       /// <returns></returns>
       Task<string[]> InitBadWords(string txtPath);
       /// <summary>
       /// 移除缓存中的黑词
       /// </summary>
       /// <returns></returns>
       Task<bool> RemoveBadWordsCache();
       /// <summary>
       /// 获取缓存中的黑词
       /// </summary>
       /// <returns></returns>
       Task<string[]> GetBadWordsCache();
     /// <summary>
     /// 获取短信中的黑词
     /// </summary>
     /// <param name="badWords">黑词库</param>
     /// <param name="content">需要被检验的短信内容</param>
     /// <returns></returns>
       List<string> BadWordsFilter(string[] badWords, string content);
    }
}
