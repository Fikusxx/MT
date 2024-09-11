using Kafka.Messages;
using MassTransit;

namespace Kafka.Consumers;

public class BatchKafkaMessageConsumer : IConsumer<Batch<BatchKafkaMessage>>
{
    public async Task Consume(ConsumeContext<Batch<BatchKafkaMessage>> context)
    {
        foreach (var t in context.Message)
        {
            await Task.Delay(1000);
            var key = context.GetKey<Guid>();
            var partition = context.Partition();
            var offset = context.Offset();
            var messageId = context.MessageId;
            var retry = context.GetRetryAttempt();
            var header = context.Headers.Get<string>("key");
            var value = context.Headers.FirstOrDefault(x => x.Key == "key").Value;
            var time2 = context.Headers.Get<DateTimeOffset>("time");

            var timeValue = time2?.ToLocalTime() ?? DateTimeOffset.UtcNow.ToLocalTime();

            Console.WriteLine(
                $"{messageId} : key {key} partition {partition} offset {offset} at {timeValue} | Address: {this.GetAddress():X}");
        }
    }
}