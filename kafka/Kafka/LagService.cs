using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Kafka;

public class LagService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var adminClientConfig = new AdminClientConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var adminClient = new AdminClientBuilder(adminClientConfig).Build();
        var groupId = "processing";
        var topicPartitions = new List<TopicPartition> { new("templates", new Partition(0)) };
        var consumerGroup = new ConsumerGroupTopicPartitions(groupId, topicPartitions);
        var options = new ListConsumerGroupOffsetsOptions { RequireStableOffsets = true };

        // Get the latest committed offsets
        var committedOffsets = await adminClient.ListConsumerGroupOffsetsAsync([consumerGroup], options);

        // var result = await adminClient.ListOffsetsAsync([
        //     new TopicPartitionOffsetSpec
        //     {
        //         TopicPartition = new TopicPartition("templates", new Partition(0)),
        //         OffsetSpec = OffsetSpec.Latest()
        //     }
        // ]);

        var totalOffsets = await adminClient.ListOffsetsAsync(committedOffsets
            .SelectMany(x => x.Partitions)
            .Select(x => new TopicPartitionOffsetSpec
            {
                TopicPartition = x.TopicPartition,
                OffsetSpec = OffsetSpec.Latest()
            }));

        // Calculate consumer lag
        var totalMessages = totalOffsets.ResultInfos.Sum(x => x.TopicPartitionOffsetError.Offset);
        var processedMessages = committedOffsets
            .SelectMany(x => x.Partitions)
            .Sum(x => x.TopicPartitionOffset.Offset.Value);

        var lag = totalMessages - processedMessages;

        var results = committedOffsets
            .Select(x => x)
            .SelectMany(x => x.Partitions)
            .Join(totalOffsets.ResultInfos,
                committed => committed.Partition.Value,
                total => total.TopicPartitionOffsetError.Partition.Value,
                (committed, total) => new
                {
                    Topic = committed.Topic,
                    Partition = committed.Partition.Value,
                    Lag = total.TopicPartitionOffsetError.Offset.Value - committed.Offset.Value
                }).ToList();

        foreach (var result in results)
        {
            Console.WriteLine($"Topic: {result.Topic}, Partition: {result.Partition}, Lag: {result.Lag}");
        }

        // foreach (var offset in committedOffsets)
        // {
        //     foreach (var partition in offset.Partitions)
        //     {
        //         var partitionOffset = partition.Offset.Value;
        //         var totalMessagesCount = totalOffsets.ResultInfos
        //             .FirstOrDefault(x => x.TopicPartitionOffsetError.Partition.Value == partition.Partition.Value)?
        //             .TopicPartitionOffsetError.Offset.Value!;
        //
        //         Console.WriteLine(
        //             $"Topic: {partition.Topic}, Partition: {partition.Partition.Value}, Lag: {totalMessagesCount - partitionOffset}");
        //     }
        // }
    }
}