using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xunit;

namespace PivotalServices.Redis.Messaging.Tests
{
    public class ProducerTests
    {
        Mock<IConnectionMultiplexer> connectionMultiplexer;
        Mock<ISubscriber> subscriber;
        Mock<ILogger<Producer>> logger;

        public ProducerTests()
        {
            connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            subscriber = new Mock<ISubscriber>();
            logger = new Mock<ILogger<Producer>>();
        }

        [Fact]
        public void ProducerIsTypeOfIProducer()
        {
            connectionMultiplexer.Setup(cm => cm.GetSubscriber(null)).Returns(subscriber.Object);
            Assert.True(new Producer(connectionMultiplexer.Object, logger.Object) is IProducer);
            connectionMultiplexer.Verify();
        }

        [Fact]
        public void ProducerCallsSubscriberToPublish()
        {
            connectionMultiplexer.Setup(cm => cm.GetSubscriber(null)).Returns(subscriber.Object);
            var publisher = new Producer(connectionMultiplexer.Object, logger.Object);
            var message = new Message("myId", "my message body");
            publisher.Publish("channel", message);
            subscriber.Verify(s => s.Publish("channel", JsonConvert.SerializeObject(message), CommandFlags.None), Times.Once);
        }
    }
}
