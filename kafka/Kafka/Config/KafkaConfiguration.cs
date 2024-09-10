using Confluent.Kafka;
using Kafka.Consumers;
using Kafka.Filters;
using Kafka.Messages;
using MassTransit;

namespace Kafka.Config;

public static class KafkaConfiguration
{
    public static void AddMessageEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        // https://docs.confluent.io/platform/current/clients/javadocs/javadoc/index.html?org/apache/kafka/clients/consumer/ConsumerConfig.html
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "group_1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,

            // auto commits offsets after 5s since 1st poll, then it restarts
            // AutoCommitIntervalMs = 5000,
            // EnableAutoCommit = true,

            // doesnt work
            //PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,

            // allows consumers to leave a group and then join again w/o re balancing partitions
            // GroupInstanceId = "123",

            // SessionTimeoutMs = 45000,
            // HeartbeatIntervalMs = 3000,

            // time a consumer would wait after a poll() that returned no messages
            // a consumer would essentially sit and wait for 300s for any messages to show up
            // MaxPollIntervalMs = 300000,
            
            // how long internal consumer wait for a fetch and how big is it
            // https://www.baeldung.com/java-kafka-send-large-message
            FetchMinBytes = 1,
            FetchMaxBytes = 52428800,
            FetchWaitMaxMs = 500,
            AllowAutoCreateTopics = true,
        };

        cfg.TopicEndpoint<Guid, KafkaMessage>(Constants.MessageTopic,
            topic1Group,
            e =>
            {
                e.UseKillSwitch(k => k
                    .SetActivationThreshold(10)
                    .SetRestartTimeout(s: 10)
                    .SetTripThreshold(0.5));
                
                // requires AllowAutoCreateTopics = true
                e.CreateIfMissing(options =>
                {
                    options.NumPartitions = 1;
                    options.ReplicationFactor = 1;
                });

                // Transient error handling
                e.UseMessageRetry(retry => retry.Interval(2, TimeSpan.FromSeconds(1)));

                // doesnt work with Kafka
                //e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15)));

                // https://stackoverflow.com/questions/57258424/what-is-the-difference-between-concurrencylimit-and-prefetchcount
                // https://www.youtube.com/watch?v=M_yPAhWgvo4&ab_channel=ChrisPatterson
                // https://masstransit.io/documentation/configuration/transports/kafka
                // # messages to prefetch from kafka topic into memory (internal buffer)
                // MT automatically sets the prefetch count = number of CPU processors (cores) 
                // OR it adds a little buffer based on ConcurrentMessageLimit, unless specified explicitly
                // Example: ConcurrentMessageLimit = 10, then PrefetchCount will be ~12
                e.PrefetchCount = 15;

                // client side
                // receives up to 10 message per partition, the number of concurrent messages, per partition
                // Preserve ordering with different keys.
                // When keys ARE SAME will use ConcurrentDeliveryLimit instead
                // process up to 10 messages at the same time, aka Parallel.ForEach(messages, _ => { })
                e.ConcurrentMessageLimit = 10;

                // Number of Confluent Consumer instances within same topic
                // create up to two Kafka consumers, increases throughput with multiple partitions
                e.ConcurrentConsumerLimit = 1;

                // process only one message per key value within a partition at a time (default)
                // WILL BREAK ordering unless it's = 1 (default)
                e.ConcurrentDeliveryLimit = 1;
                e.Consumer<KafkaMessageConsumer>();
                
                // scoped pipelines
                // e.UseConsumeFilter<KafkaMessageScopedFilter>(ctx);
                // e.UseConsumeFilter(typeof(IdempotencyFilter<>), context, x => x.Include(type => type.HasInterface<IHasIdempotency>()));
            });
    }

    public static void AddMessageProducer(this IRiderRegistrationConfigurator cfg)
    {
        // Producers
        // key assignment gets overriden by manual call Produce(key, value)
        cfg.AddProducer<Guid, KafkaMessage>(Constants.MessageTopic,
            m => m.Message.Id,
            (_, producerCfg) =>
            {
                // Recommended for >= 1.0 Kafka 
                // Default for >= 3.0 Kafka
                producerCfg.EnableIdempotence = true;
                // will be set automatically along with EnableIdempotence = true
                // producerCfg.MessageSendMaxRetries = 2;
                // producerCfg.RetryBackoff = TimeSpan.FromMilliseconds(100);
                // producerCfg.Acks = Acks.All (not really assignable with config here)

                // High throughput producer
                // waits for this amount of ms before sending messages
                // producerCfg.Linger = TimeSpan.FromMilliseconds(20); // default 5ms
                // producerCfg.CompressionType = CompressionType.Snappy;


                // buffers in memory if a broker cant accept any more messages
                producerCfg.QueueBufferingMaxKbytes = 100 * 1024; // default 100mb
                producerCfg.QueueBufferingMaxMessages = 100000; // default 100000
            });
    }

    public static void AddExtraEndpoint(this IKafkaFactoryConfigurator cfg)
    {
        // var topic2Group = new ConsumerConfig()
        // {
        //     GroupId = "group_2",
        //     AutoOffsetReset = AutoOffsetReset.Earliest
        // };
        //
        // cfg.TopicEndpoint<KafkaMessageError>("error", topic2Group,
        //     e => { e.ConfigureConsumer<KafkaMessageErrorConsumer>(context); });
        //
        //
        // // TEST
        // var topic3Group = new ConsumerConfig()
        // {
        //     GroupId = "group_3",
        //     AutoOffsetReset = AutoOffsetReset.Earliest
        // };
        //
        // cfg.TopicEndpoint<ComplicatedKafkaMessage>("demo", topic3Group, e =>
        // {
        //     //e.UseConsumeFilter(typeof(MyExceptionFilter<>), context);
        //
        //     e.ConfigureConsumer<ComplicatedKafkaMessageConsumer>(context);
        // });
    }
}