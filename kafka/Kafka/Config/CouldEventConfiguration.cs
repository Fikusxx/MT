using System.Net.Mime;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Kafka.Consumers;
using Kafka.Messages;
using MassTransit;

namespace Kafka.Config;

public static class CouldEventConfiguration
{
    public static void AddCloudEventEndpoints(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        AddFirstCloudEventEndpoint(cfg, ctx);
        AddSecondCloudEventEndpoint(cfg, ctx);
    }

    private static void AddFirstCloudEventEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "first_ce",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
        };

        cfg.TopicEndpoint<Guid, CloudEvent>(Constants.FirstCouldEventTopic,
            topic1Group,
            e =>
            {
                e.SetValueDeserializer(new FirstCloudEventDeserializer());
                e.Consumer<FirstCloudEventConsumer>();
            });
    }
    
    private static void AddSecondCloudEventEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "second_ce",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
        };

        cfg.TopicEndpoint<Guid, CloudEvent>(Constants.SecondCouldEventTopic,
            topic1Group,
            e =>
            {
                e.SetValueDeserializer(new SecondCloudEventDeserializer());
                e.Consumer<SecondCloudEventConsumer>();
            });
    }
} 

public class FirstCloudEventDeserializer : IDeserializer<CloudEvent>
{
    public CloudEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var formatter = new JsonEventFormatter<FirstCloudEventData>();
        var cloudEvent = formatter.DecodeStructuredModeMessage(new MemoryStream(data.ToArray()), 
            new ContentType(MediaTypeNames.Application.Json),
            null);
        
        return cloudEvent;
    }
}

public class SecondCloudEventDeserializer : IDeserializer<CloudEvent>
{
    public CloudEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var formatter = new JsonEventFormatter<SecondCloudEventData>();
        var cloudEvent = formatter.DecodeStructuredModeMessage(new MemoryStream(data.ToArray()), 
            new ContentType(MediaTypeNames.Application.Json),
            null);
        
        return cloudEvent;
    }
}