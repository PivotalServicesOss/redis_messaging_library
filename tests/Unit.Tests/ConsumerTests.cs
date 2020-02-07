using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace PivotalServices.Redis.Messaging.Tests
{
    public class ConsumerTests
    {
        Mock<IConnectionMultiplexer> connectionMultiplexer;
        Mock<ISubscriber> subscriber;
        Mock<ILogger<Consumer>> logger;

        public ConsumerTests()
        {
            connectionMultiplexer = new Mock<IConnectionMultiplexer>();
            subscriber = new Mock<ISubscriber>();
            logger = new Mock<ILogger<Consumer>>();
        }

        [Fact]
        public void ConsumerIsTypeOfIConsumer()
        {
            connectionMultiplexer.Setup(cm => cm.GetSubscriber(null)).Returns(subscriber.Object);
            Assert.True(new Consumer(connectionMultiplexer.Object, logger.Object) is IConsumer);
            connectionMultiplexer.Verify();
        }

        //TODO:Add tests
    }
}
