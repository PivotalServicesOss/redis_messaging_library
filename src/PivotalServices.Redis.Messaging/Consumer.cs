using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PivotalServices.Redis.Messaging
{
    public delegate void OnMessageReceivedEventHandler(Message message);
    public class Consumer : IConsumer
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<Consumer> logger;
        private ISubscriber consumer;

        public Consumer(IConnectionMultiplexer connectionMultiplexer, ILogger<Consumer> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            consumer = connectionMultiplexer.GetSubscriber();
        }

        public event OnMessageReceivedEventHandler MessageReceived;

        public void StartConsumption(string channelNameOrPattern)
        {
            logger.LogInformation($"Started consumption from channel '{channelNameOrPattern}'");
            consumer.Subscribe(channelNameOrPattern, (channel, redisValue) =>
             {
                 if (redisValue.IsNullOrEmpty)
                     logger.LogError($"Message is either null or empty, from channel '{channelNameOrPattern}'");
                 else
                 {
                     var message = JsonConvert.DeserializeObject<Message>(redisValue);
                     logger.LogDebug($"Message with Id '{message.Id}' received, channel '{channelNameOrPattern}");
                     MessageReceived?.Invoke(message);
                 }
             });
        }

        public void StopConsumption(string channelNameOrPattern)
        {
            consumer.Unsubscribe(channelNameOrPattern);
        }
    }

    public interface IConsumer
    {
        void StartConsumption(string channelNameOrPattern);
        void StopConsumption(string channelNameOrPattern);
        event OnMessageReceivedEventHandler MessageReceived;
    }
}
