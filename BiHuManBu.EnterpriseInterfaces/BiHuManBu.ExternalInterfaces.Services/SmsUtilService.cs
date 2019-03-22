using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.NetCacheHelper;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class SmsUtilService : ISmsUtilService
    {
        readonly CacheClient _cacheClient;
        readonly string customerKey = "_netBadWordsKey";

        private HashSet<string> hash = new HashSet<string>();
        public SmsUtilService()
        {
            this._cacheClient = new CacheClient(new NetCache());
        }
        public async Task<string[]> InitBadWords(string txtPath)
        {
            return await Task.Run(() =>
            {

                var badWordsArray = File.ReadAllLines(txtPath, Encoding.UTF8).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (!_cacheClient.Set<string[]>(customerKey, badWordsArray, txtPath))
                {
                    return default(string[]);
                }
                return badWordsArray;
            });
        }

        public async Task<bool> RemoveBadWordsCache()
        {
            return await Task.Run(() =>
            {
                return _cacheClient.Remove(customerKey);
            });
        }

        public async Task<string[]> GetBadWordsCache()
        {
            return await Task.Run(() =>
            {
                return _cacheClient.Get<string[]>(customerKey);
            });
        }
        public List<string> BadWordsFilter(string[] badWords, string content)
        {
            Init(badWords);
            return HasBadWords(content).Distinct().ToList();
        }
        private void Init(string[] badWords)
        {
            foreach (string word in badWords)
            {

                if (word.Length != 1)
                {
                    hash.Add(word);
                }
            }
        }
        private List<string> HasBadWords(string text)
        {
            Dictionary<char, List<string>> dicList = new Dictionary<char, List<string>>();
            var strList = new List<string>();
            foreach (var item in hash)
            {
                char value = item[0];
                if (dicList.ContainsKey(value))
                    dicList[value].Add(item);
                else
                    dicList.Add(value, new List<string>() { item });
            }
            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                char word = text[i];
                if (dicList.ContainsKey(word))//如果在字典表中存在这个key
                {
                    var data = dicList[word].OrderBy(g => g.Length);
                    //把该key的字典集合按 字符数排序(方便下面从少往多截取字符串查找)
                    foreach (var wordbook in data)
                    {
                        if (i + wordbook.Length <= count)
                        //如果需截取的字符串的索引小于总长度 则执行截取
                        {
                            string result = text.Substring(i, wordbook.Length);
                            //根据关键字长度往后截取相同的字符数进行比较
                            if (result == wordbook)
                            {
                                strList.Add(result);
                                i = i + wordbook.Length - 1;
                                //比较成功 同时改变i的索引
                                break;
                            }
                        }
                    }
                }

            }
            return strList;
        }
    }
}
