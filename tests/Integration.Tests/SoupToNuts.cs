using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Redis;
using Xunit;

namespace PivotalServices.Redis.Messaging.Integ.Tests
{
    public class SoupToNuts : IDisposable
    {
        static AutoResetEvent resetEvent = new AutoResetEvent(false);
        IServiceCollection services;
        ServiceProvider serviceProvider;

        public SoupToNuts()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", false, false);

            var configuration = builder.Build();

            services = new ServiceCollection();
            services.AddLogging();
            services.AddRedisConnectionMultiplexer(configuration);
            services.AddSingleton<IProducer, Producer>();
            services.AddSingleton<IConsumer, Consumer>();
            services.AddSingleton<MessageProcessor>();

            serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<ILoggerFactory>().AddConsole().AddDebug();
        }



        [Fact]
        public void Test_E2E_Producer_And_Consumer()
        {
            var producer = serviceProvider.GetService<IProducer>();
            var processor = serviceProvider.GetService<MessageProcessor>();

            var message = new Message("Id", "Body of the message");

            processor.Start();

            producer.Publish("integration", message);

            Message receivedMessage = null;

            processor.OnMessageReceivedEvent+=((m)=>
            {
                receivedMessage = m;
                resetEvent.Set();
            });

            resetEvent.WaitOne(10000);

            Assert.NotNull(receivedMessage);
            Assert.Equal(message.Id, receivedMessage.Id);
            Assert.Equal(message.Body, receivedMessage.Body);
        }

        internal class MessageProcessor
        {
            private readonly IConsumer consumer;

            public MessageProcessor(IConsumer consumer)
            {
                this.consumer = consumer;
                consumer.MessageReceived += OnMessageReceived;
            }

            private void OnMessageReceived(Message message)
            {
                OnMessageReceivedEvent(message);
            }

            public void Start()
            {
                consumer.StartConsumption("integration");
            }

            public Action<Message> OnMessageReceivedEvent;
        }

        public void Dispose()
        {
            var consumer = serviceProvider.GetService<IConsumer>();
            
            if(consumer != null)
                consumer.StopConsumption("integration");
        }
    }
}