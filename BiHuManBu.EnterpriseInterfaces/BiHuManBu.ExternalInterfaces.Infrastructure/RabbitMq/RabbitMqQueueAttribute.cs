using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq
{
    public class RabbitMqQueueAttribute: Attribute
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get { return "vmeng_exc"; } private set { } }
        /// <summary>
        /// 队列名称【必填】
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get { return true; } private set { } }
        /// <summary>
        /// 路由名称【必填】
        /// </summary>
        public string RouteKey { get; set; }
        /// <summary>
        /// 交换机类型
        /// </summary>
        public string ExchangeType { get { return "direct"; } private set { } }


        #region 插件延时使用
        /// <summary>
        /// 插件延迟消息交换机名称 默认和ExchangeName一样
        /// </summary>
        public string DelayExchangeName { get { return "tx_exc"; } private set { } }
        /// <summary>
        /// 插件延时消息队列名称  【必填】
        /// </summary>
        public string DelayQueueName { get { return "tx_queue_delay"; } set { } }

        /// <summary>
        /// 插件延时消息路由Key名称 【必填】
        /// </summary>
        public string DelayRouteKey { get { return "tx_delay_routekey"; } set { } }

        /// <summary>
        /// 插件延时交换机类型
        /// </summary>
        public string DelayExchangeType { get { return "x-delayed-message"; } private set { } }

        #endregion

        #region 死信延时使用

        /// <summary>
        /// 死信延时消息交换机名称 默认和ExchangeName一样
        /// </summary>
        public string DeadExchangeName { get { return "vmeng_dead"; } set { } }

        /// <summary>
        /// 死信延时消息队列名称 【必填】
        /// </summary>
        public string DeadQueueName { get { return "vmeng_queue_dead"; } set { } }

        /// <summary>
        /// 死信延时消息路由Key名称 【必填】
        /// </summary>
        public string DeadRouteKey { get { return "bind_dead_routekey"; } set { } }

        /// <summary>
        /// 死信延时交换机类型
        /// </summary>
        public string DeadExchangeType { get { return "direct"; } private set { } }

        #endregion
    }
}
