### A simple .NET standard library to make use of Redis pub/sub for messaging
Build | PivotalServices.Redis.Messaging |
--- | --- |
[![Build Status](https://dev.azure.com/ajaganathan-home/pivotalservices_redis_messaging_library/_apis/build/status/alfusinigoj.pivotalservices_redis_messaging_library?branchName=master)](https://dev.azure.com/ajaganathan-home/pivotalservices_redis_messaging_library/_build/latest?definitionId=2&branchName=master) | [![NuGet](https://img.shields.io/nuget/v/PivotalServices.Redis.Messaging.svg?style=flat-square)](http://www.nuget.org/packages/PivotalServices.Redis.Messaging) | 

#### How to create a consumer/subscriber
- Add the nuget package [PivotalServices.Redis.Messaging](http://www.nuget.org/packages/PivotalServices.Redis.Messaging)
- In the `startup.cs` class, under `ConfigureServices` method add the below extension method as below.

```c#
    using PivotalServices.Redis.Messaging;
	
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddRedisMessagingConsumer();
    }
```

- Next step is to subscribe to a channel to receive messages.
- `PivotalServices.Redis.Messaging.IConsumer` implementation will be in the dependency container
- I would use the `Configure` method of `startup.cs` to demonstrate it (as below). But you can always inject `PivotalServices.Redis.Messaging.IConsumer` into any of the classes and perform the necessary operations there.

```c#
    using PivotalServices.Redis.Messaging;

    public void Configure(IApplicationBuilder app, IConsumer consumer) 
    {
        consumer.StartConsumption("myChannel", (message) =>
        {
            //Any action to be performed when a message is received
	    Console.Out.WriteLine($"Received Message, {message.Id}");
        });
    }
```
- Similarly, if to unsubscribe from a channel you can call `StopConsumption` as below, using `IApplicationLifetime`

```c#
    using PivotalServices.Redis.Messaging;

    public void Configure(IApplicationBuilder app, IApplicationLifetime lifetime, IConsumer consumer)
    {
        lifetime.ApplicationStopping.Register(() => consumer.StopConsumption("myChannel"));
    }
```

#### How to create a producer/publisher
- Add the nuget package [PivotalServices.Redis.Messaging](http://www.nuget.org/packages/PivotalServices.Redis.Messaging)
- In the `startup.cs` class, under `ConfigureServices` method add the below extension method as below.

```c#
    using PivotalServices.Redis.Messaging;
	
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddRedisMessagingProducer();
    }
```

- Next step is to subscribe to a channel to receive messages.
- `PivotalServices.Redis.Messaging.IProducer` implementation will be in the dependency container
- I would use the `Configure` method of `startup.cs` to demonstrate it (as below - publish a message every 10 seconds). But you can always inject `PivotalServices.Redis.Messaging.IProducer` into any of the classes and publish messages there.

```c#
    using PivotalServices.Redis.Messaging;

    public void Configure(IApplicationBuilder app, IProducer producer)
    {
        while(true)
        {
            producer.Publish("myChannel", new Message(Guid.NewGuid().ToString(), DateTime.Now.ToString()));
            Thread.Sleep(10000);
        }
    }
```


### Ongoing development packages in MyGet

Feed | PivotalServices.Redis.Messaging |
--- | --- |
[V3](https://www.myget.org/F/ajaganathan/api/v3/index.json) | [![MyGet](https://img.shields.io/myget/ajaganathan/v/PivotalServices.Redis.Messaging.svg?style=flat-square)](https://www.myget.org/feed/ajaganathan/package/nuget/PivotalServices.Redis.Messaging) | 

### Issues
- Kindly raise any issues under [GitHub Issues](https://github.com/alfusinigoj/pivotal_redis_messaging_library/issues)

### Contributions are welcome!
