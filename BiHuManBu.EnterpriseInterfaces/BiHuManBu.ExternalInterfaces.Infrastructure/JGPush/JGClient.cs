using Jiguang.JPush;
using Jiguang.JPush.Model;
using System;
using System.Collections.Generic;
namespace BiHuManBu.ExternalInterfaces.Infrastructure.JGPush
{
    public class JGClient
    {
        private string _appkey;
        private string _mastersecret;
        private bool _apnsproduction;
        public JGClient(string _appkey, string _mastersecret, bool _apnsproduction = true)
        {
            if (string.IsNullOrWhiteSpace(_appkey))
            {
                throw new Exception("极光推送：_appkey参数不能空");
            }
            if (string.IsNullOrWhiteSpace(_mastersecret))
            {
                throw new Exception("极光推送：_mastersecret参数不能空");
            }
            this._appkey = _appkey;
            this._mastersecret = _mastersecret;
            this._apnsproduction = _apnsproduction;
        }
        public Dictionary<int, bool> PushNotification(IEnumerable<JGPushPayLoadModel> payLoadModels,int messageType)
        {
            string sound = "1117";
            if (messageType==0) {
                sound = "prompt.caf";
            }
            var resultDic = new Dictionary<int, bool>();
            var jgProvider = new JPushClient(_appkey, _mastersecret);
            foreach (var item in payLoadModels)
            {
                var jgpushPayload = new PushPayload() { Audience = new { alias = new List<string> { item.JGAccount } }, Notification = new Notification { IOS = new IOS { Alert = item.Content, Sound = sound, Badge = "+1", Extras = item.ParameterDic }, Android = new Android { Title = item.Title, Alert = item.Content, Extras = item.ParameterDic } }, Platform = "all", Options = new Options { IsApnsProduction = _apnsproduction } };
                var isSuccessfulDelivery = jgProvider.SendPush(jgpushPayload).StatusCode == System.Net.HttpStatusCode.OK;
                resultDic.Add(item.MessageId, isSuccessfulDelivery);
            }
            return resultDic;
        }
    }
}
