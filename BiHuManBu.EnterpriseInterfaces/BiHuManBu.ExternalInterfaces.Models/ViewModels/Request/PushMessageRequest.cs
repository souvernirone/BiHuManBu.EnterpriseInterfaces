using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class PushMessageRequest : BaseRequest2
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LincenseNo { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string TitleStr { get; set; }

        /// <summary>
        /// buid
        /// </summary>
        public long Buid { get; set; }

        /// <summary>
        /// 消息推送类型  
        /// 消息类型,0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知，8 进店消息 9 新分配消息 10 订单
        /// 当前只支持  10订单消息   其他消息业务不熟悉  未做兼容
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }

    }
}
