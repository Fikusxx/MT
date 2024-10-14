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
            var key = context.GetKey<Guid>(); // receive context key
            var partition = context.Partition();
            var offset = context.Offset(); // latest offset of a batch
            var messageId = context.MessageId; // some bs value, idk
            var time = context.Headers.Get<DateTimeOffset>("time");

            var timeValue = time?.ToLocalTime() ?? DateTimeOffset.UtcNow.ToLocalTime();

            Console.WriteLine(
                $"{messageId} : key {key} partition {partition} offset {offset} at {timeValue} | Address: {this.GetAddress():X}");
        }
    }
}