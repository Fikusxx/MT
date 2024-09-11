using Confluent.Kafka;
using Kafka.Consumers;
using Kafka.Messages;
using MassTransit;

namespace Kafka.Config;

public static class BatchMessageConfiguration
{
    public static void AddBatchMessageEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "group_1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
            AllowAutoCreateTopics = true,
        };

        cfg.TopicEndpoint<Guid, BatchKafkaMessage>(Constants.BatchMessageTopic,
            topic1Group,
            e =>
            {
                e.UseKillSwitch(k => k
                    .SetActivationThreshold(10)
                    .SetRestartTimeout(s: 10)
                    .SetTripThreshold(0.5));

                e.CreateIfMissing(options =>
                {
                    options.NumPartitions = 1;
                    options.ReplicationFactor = 1;
                });
                
                e.PrefetchCount = 20;
                
                e.ConcurrentMessageLimit = 10;

                // this number will go in batches with ordering preserved
                e.ConcurrentDeliveryLimit = 5;
                e.Consumer<BatchKafkaMessageConsumer>();
            });
    }
    
    public static void AddBatchMessageProducer(this IRiderRegistrationConfigurator cfg)
    {
        cfg.AddProducer<Guid, BatchKafkaMessage>(Constants.BatchMessageTopic,
            m => m.Message.Id,
            (_, producerCfg) =>
            {
                producerCfg.EnableIdempotence = true;
            });
    }
}