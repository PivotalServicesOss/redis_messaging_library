using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace PivotalServices.Redis.Messaging
{
    public class Producer : IProducer
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<Producer> logger;
        private ISubscriber producer;
        
        public Producer(IConnectionMultiplexer connectionMultiplexer, ILogger<Producer> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            producer = connectionMultiplexer.GetSubscriber();
        }

        public void Publish(string channelNameOrPattern, Message message)
        {
            var noOfConsumersReceived = producer.Publish(channelNameOrPattern, JsonConvert.SerializeObject(message));
            logger.LogDebug($"Message with Id '{message.Id}' published and consumed by {noOfConsumersReceived} consumers, channel '{channelNameOrPattern}'");
        }
    }

    public interface IProducer
    {
        void Publish(string channelNameOrPattern, Message message);
    }
}