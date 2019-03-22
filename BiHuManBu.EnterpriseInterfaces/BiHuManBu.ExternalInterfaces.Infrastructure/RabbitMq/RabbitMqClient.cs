using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq
{
    public class RabbitMqClient
    {
        private static IConnection _conn;
        private static ConcurrentDictionary<string, IConnection> ConnDic = new ConcurrentDictionary<string, IConnection>();
        private static readonly object _lockObj = new object();
        private static readonly ConcurrentDictionary<string, IModel> ModelDic =
            new ConcurrentDictionary<string, IModel>();
        private static ILog logError = LogManager.GetLogger("ERROR");
        #region 消息无法消费时使用的专用队列
        static int tryTimes = 3;
        static string ExchangeName = "vmeng_exc";
        static string QueueName = "vmeng_UnConsumer";
        static string RouteKey = "bind_UnConsumer";
        static string ExchangeType = "direct";
        #endregion

        private static void Open()
        {
            if (_conn != null) return;
            lock (_lockObj)
            {
                if (_conn != null) return;
                var factory = new ConnectionFactory
                {
                    HostName = ConfigurationManager.AppSettings["RMQHostName"],
                    UserName = ConfigurationManager.AppSettings["RMQUserName"],
                    Password = ConfigurationManager.AppSettings["RMQPassword"],
                    AutomaticRecoveryEnabled = RabbitMqConfig.AutomaticRecoveryEnabled,
                    RequestedHeartbeat = 15
                };
                _conn = _conn ?? factory.CreateConnection();
            }

        }

        public RabbitMqClient()
        {
            Open();
        }
        private RabbitMqQueueAttribute GetRabbitMqAttribute<T>()
        {
            return (RabbitMqQueueAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(RabbitMqQueueAttribute));
        }

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey"></param>
        /// <param name="exchangeType">匹配规则</param>
        /// <param name="durable">是否持久化</param>
        /// <returns></returns>
        private static IModel GetModel(string exchange, string queue, string routingKey, string exchangeType, bool durable = true,
            bool autoDelete = false, IDictionary<string, object> exchangeArgs = null, IDictionary<string, object> queueArgs = null)
        {
            return ModelDic.GetOrAdd(queue, key =>
            {

                var model = _conn.CreateModel();

                ExchangeDeclare(model, exchange, exchangeType, durable, autoDelete, exchangeArgs);
                QueueDeclare(model, queue, durable, autoDelete, queueArgs);
                model.QueueBind(queue, exchange, routingKey);
                ModelDic[queue] = model;
                return model;
            });
        }

        private static void ExchangeDeclare(IModel iModel, string exchange, string type, bool durable = true,
            bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            exchange = string.IsNullOrWhiteSpace(exchange) ? "" : exchange.Trim();
            iModel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }
        private static void QueueDeclare(IModel channel, string queue, bool durable = true, bool autoDelete = false, IDictionary<string, object> arguments = null, bool exclusive = false)
        {
            queue = string.IsNullOrWhiteSpace(queue) ? "UndefinedQueueName" : queue.Trim();
            channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }
        public void SendMessage<T>(T messageObj) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            var channel = GetModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey, queueInfo.ExchangeType, queueInfo.Durable);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
        }
        public void SendMessage(byte[] body)
        {
            var channel = GetModel(ExchangeName, QueueName, RouteKey, ExchangeType, true);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            channel.BasicPublish(ExchangeName, RouteKey, properties, body);
        }
        /// <summary>
        /// 发送延迟消息 通过插件rabbitmq-delayed-message-exchange实现
        /// http://blog.csdn.net/u014308482/article/details/53036770 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>

        public void SendDelayMessageByPlugin<T>(T messageObj, DateTime publishTime) where T : class
        {
            long delay = (long)(publishTime - DateTime.Now).TotalMilliseconds;
            SendDelayMessageByPlugin<T>(messageObj, delay);
        }

        /// <summary>
        /// 发送延迟消息 通过插件rabbitmq-delayed-message-exchange实现
        /// http://blog.csdn.net/u014308482/article/details/53036770 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>
        /// <param name="delay">延时的毫秒数</param>
        public void SendDelayMessageByPlugin<T>(T messageObj, long delay) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");

            //延迟队列参数，必须
            IDictionary<string, object> args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };
            var channel = GetModel(queueInfo.DelayExchangeName, queueInfo.DelayQueueName, queueInfo.DelayRouteKey, queueInfo.DelayExchangeType, queueInfo.Durable, false, args);
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                {"x-delay", delay}
            };
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.DelayExchangeName, queueInfo.DelayRouteKey, properties, body);
        }

        /// <summary>
        /// 发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>

        public void SendDelayMessageByDeadLetter<T>(T messageObj, DateTime publishTime) where T : class
        {
            long delay = (long)(publishTime - DateTime.Now).TotalMilliseconds;
            SendDelayMessageByDeadLetter<T>(messageObj, delay);
        }

        /// <summary>
        /// 发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="delay">延时的毫秒数</param>

        public void SendDelayMessageByDeadLetter<T>(T messageObj, long delay) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");

            //延迟队列参数，必须
            IDictionary<string, object> queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", queueInfo.DeadExchangeName},
                {"x-dead-letter-routing-key", queueInfo.DeadRouteKey}
            };

            var channel = ModelDic.GetOrAdd(queueInfo.QueueName, key =>
            {
                //声明原交换机，队列，并绑定
                var model = _conn.CreateModel();
                ExchangeDeclare(model, queueInfo.ExchangeName, queueInfo.ExchangeType, queueInfo.Durable, false, null);
                QueueDeclare(model, queueInfo.QueueName, queueInfo.Durable, false, queueArgs);
                model.QueueBind(queueInfo.QueueName, queueInfo.ExchangeName, queueInfo.RouteKey, null);

                //声明死信交换机，死信队列，并绑定
                ExchangeDeclare(model, queueInfo.DeadExchangeName, queueInfo.DeadExchangeType, queueInfo.Durable, false, null);
                QueueDeclare(model, queueInfo.DeadQueueName, queueInfo.Durable, false, null);
                model.QueueBind(queueInfo.DeadQueueName, queueInfo.DeadExchangeName, queueInfo.DeadRouteKey, null);

                ModelDic[queueInfo.QueueName] = model;
                return model;
            });

            var properties = channel.CreateBasicProperties();
            properties.Expiration = delay.ToString();//设置消息过期时间
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            //发送消息到原队列
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
        }


        public void ReceiveMessage<T>(Func<T, CancellationToken, Task<bool>> handler)
        {


            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null)
            {
                throw new NullReferenceException("处理事件为null");
            }
            var channel = GetModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey, queueInfo.ExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.QueueName, false, consumer);
            consumer.Received += async (model, ea) =>
            {
                await _doAsync(channel, ea, handler);

            };
        }
        private async Task _doAsync<T>(IModel channel, BasicDeliverEventArgs ea, Func<T, CancellationToken, Task<bool>> normalHandler)
        {
            var body = ea.Body;
            try
            {
                var msgBody = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                var isConsumeSuccess = await normalHandler(msgBody, default(CancellationToken));
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                channel.BasicAck(ea.DeliveryTag, false);
                logError.Error(ex);
            }


        }
        /// <summary>
        /// 接收延迟消息，通过 通过插件rabbitmq-delayed-message-exchange实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void ReceiveDelayMessageByPlugin<T>(Func<T, bool> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null)
            {
                throw new NullReferenceException("处理事件为null");
            }
            //延迟队列参数，必须

            IDictionary<string, object> args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };
            var channel = GetModel(queueInfo.DelayExchangeName, queueInfo.DelayQueueName, queueInfo.DelayRouteKey, queueInfo.DelayExchangeType, queueInfo.Durable, false, args);

            //var channel = GetModel("tx_exc", "tx_queue_delay", "tx_delay_routekey", "x-delayed-message", true, false, args);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received +=  (model, ea) =>
            {
                var body = ea.Body;
                try
                {
                    var msgBody = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                    handler(msgBody);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    throw new Exception("消息处理异常", ex);
                }
       
            };
            channel.BasicConsume(queueInfo.DelayQueueName, false, consumer);
        }


        /// <summary>
        /// 接收延迟消息 通过死信方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void ReceiveDelayMessageByDeadLetter<T>(Func<T, CancellationToken, Task<bool>> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null)
            {
                throw new NullReferenceException("处理事件为null");
            }
            var channel = GetModel(queueInfo.DeadExchangeName, queueInfo.DeadQueueName, queueInfo.DeadRouteKey, queueInfo.DeadExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {

                    var msgBody = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                    var isOk = await handler(msgBody, default(CancellationToken));
                    if (isOk)
                    {

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception)
                {

                    channel.BasicAck(ea.DeliveryTag, false);
                }

            };
            channel.BasicConsume(queueInfo.DeadQueueName, false, consumer);
        }


        public void Dispose()
        {
            foreach (var item in ModelDic)
            {
                item.Value.Dispose();
            }
            _conn.Dispose();
        }
    }
}
