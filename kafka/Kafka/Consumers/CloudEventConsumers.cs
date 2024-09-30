using CloudNative.CloudEvents;
using Kafka.Messages;
using MassTransit;

namespace Kafka.Consumers;

public class FirstCloudEventConsumer : IConsumer<CloudEvent>
{
    public Task Consume(ConsumeContext<CloudEvent> context)
    {
        // careful with as
        var e = context.Message.Data as FirstCloudEventData;
        
        Console.WriteLine(nameof(FirstCloudEventConsumer));
        
        foreach (var (attr, value) in context.Message.GetPopulatedAttributes())
        {
            Console.WriteLine($"Attr: {attr} | Value: {value}");
        }
        
        return Task.CompletedTask;
    }
}

public class SecondCloudEventConsumer : IConsumer<CloudEvent>
{
    public Task Consume(ConsumeContext<CloudEvent> context)
    {
        if(context.Message.Data is SecondCloudEventData a)
            Console.WriteLine("Yay");
        
        // careful with as
        var e = context.Message.Data as SecondCloudEventData;
        
        Console.WriteLine(nameof(SecondCloudEventConsumer));
        
        foreach (var (attr, value) in context.Message.GetPopulatedAttributes())
        {
            Console.WriteLine($"Attr: {attr} | Value: {value}");
        }
        
        return Task.CompletedTask;
    }
}