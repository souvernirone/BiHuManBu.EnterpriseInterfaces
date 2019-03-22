using System;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ServerCenter.Infrastructure.Helpers;

namespace BiHuManBu.ExternalInterfaces.Cache
{
    public class CarInsuranceCache:ICarInsuranceCache
    {
        public GetReInfoResponse GetReInfo(GetReInfoRequest request)
        {
            var xubaokey = new
            {
                LicenseNo = request.LicenseNo,
                Agent=request.Agent

            };

            var xuBaoCacheKey= CommonCacheKeyFactory.MakeCacheKey(xubaokey);

            var xuBaoKey = string.Format("{0}-{1}", xuBaoCacheKey, "key");
             var cacheKey= CacheProvider.Get<string>(xuBaoKey);
            if (cacheKey == null)
            {
                for (int i = 0; i < 30; i++)
                {

                    cacheKey = CacheProvider.Get<string>(xuBaoKey);
                    if (!string.IsNullOrWhiteSpace(cacheKey))
                    {
                        break;
                    }
                    else
                    {
                        Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }
          
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                GetReInfoResponse response = new GetReInfoResponse();

                //续保 需要的表
                //bx_userinfo
                //bx_renewalquote
                //bx_carinfo

                //步骤1  续保的时候 会发送消息队列 ，这个时候 会把 key传过去eg:aaaa。
                
                //步骤2   中心在续保的时候 ，需要根据这个key 设置一个开关 eg:aaaa-key：1,放在缓存中,成功的时候要置1，刚开始是空值
                          //等续保结束后，先将上面列出的表写入缓存 
                          //其中： 键值分别是：
                          //bx_userinfo        :aaaa-userinfo
                          //bx_renewalquote    :aaaa-renewal
                          //bx_carinfo         :aaaa-carinfo
                //步骤3： 讲开关缓存设置续保完成标识：aaaa-key：1


                //续保缓存标示（是否成功）


                if (cacheKey == "1")
                {
                    response.UserInfo =
                        CacheProvider.Get<bx_userinfo>(string.Format("{0}-{1}", xuBaoCacheKey, "userinfo"));
                    response.SaveQuote =
                        CacheProvider.Get<bx_renewalquote>(string.Format("{0}-{1}", xuBaoCacheKey, "renewal"));
                    response.CarInfo = CacheProvider.Get<bx_carinfo>(string.Format("{0}-{1}", xuBaoCacheKey, "carinfo"));
                    response.BusinessStatus = 1;
                }
                else
                {
                    response = null;
                }
                
                return response;
            }
            else
            {
                return null;
            }
        }

        public GetPrecisePriceReponse GetPrecisePrice(GetPrecisePriceRequest request)
        {
            ///获取报价信息和 核保信息，我发送的key都是一样的 ，不能按照险种信息hash，因为我取的时候 ，请求参数是没有险种的 ，
            /// 如果换成的key跟险种信息有关，导致缓存中 会有过的垃圾数据，客户端用不到 。
            /// 也就是说  缓存只存放最新的一条报价信息和核保信息
            GetPrecisePriceReponse response = new GetPrecisePriceReponse();

            var baojiaKey = new
            {
                LicenseNo = request.LicenseNo,
                IntentionCompany = request.IntentionCompany,
                Agent = request.Agent
            };
            var baojiaCacheKey = CommonCacheKeyFactory.MakeCacheKey(baojiaKey);
            var xuBaoKey = string.Format("{0}-bj-{1}-{2}", baojiaCacheKey, request.IntentionCompany,"key");
            //获取报价信息
            //需要的表
            //bx_userinfo
            //bx_LastInfo
            //bx_SaveQuote
            //bx_QuoteResult
            //bx_SubmitInfo
            //bx_renewalquote

            //步骤1  续保的时候 会发送消息队列 ，这个时候 会把 key传过去eg:bbbb。

            //步骤2   中心在续保的时候 ，需要根据这个key 设置3个开关(人太平) 
            //eg:
            //bbbb-bj-0-key：,
            //bbbb-bj-1-key：,
            //bbbb-bj-2-key：,
            //放在缓存中,人太平报价成功的时候分别要置1，刚开始是空值
            //等报价结束后，先将上面列出的表写入缓存 
            //其中： 键值分别是：
            //bx_userinfo        :bbbb-userinfo
            //bx_LastInfo        :bbbb-lastinfo
            //bx_renewalquote    :bbbb-renewal
            //bx_SaveQuote       :bbbb-savequote

            //bx_QuoteResult     :bbbb-0-quoteresult
            //bx_SubmitInfo      :bbbb-0-submitinfo
            

            

            return response;
        }

        public GetSubmitInfoResponse GetSubmitInfo(GetSubmitInfoRequest request)
        {
            GetSubmitInfoResponse response = new GetSubmitInfoResponse();

            //获取核保信息
            //需要的表
            //bx_userinfo
            //bx_SubmitInfo
        
            //步骤1  续保的时候 会发送消息队列 ，这个时候 会把 key传过去eg:cccc。

            //步骤2   中心在续保的时候 ，需要根据这个key 设置1（因为最多只有一家核保）个开关 eg:cccc-hb-key：1,放在缓存中,成功的时候要置1，刚开始是空值
            //等续保结束后，先将上面列出的表写入缓存 
            //其中： 键值分别是：
            //bx_userinfo        :cccc-userinfo
            //bx_SubmitInfo      :cccc-submitinfo

            //步骤3： 讲开关缓存设置核保完成标识：cccc-hb-key：1

            return response;
        }
    }
}
